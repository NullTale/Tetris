using System.Diagnostics.Tracing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Action = System.Action;

namespace Core.EventSystem
{
    /// <summary> Event messages receiver interface </summary>
    public interface IMessageReceiver
    {
        void Send<T>(IMessage<T> e);
    }
    [DefaultExecutionOrder(Core.c_ManagerDefaultExecutionOrder)]
    public abstract class MessageSystem : MonoBehaviour, IMessageReceiver
    {
        public struct ListenerData
        {
            public string           Channel;
            public int              Order;
        }

        public interface IMessageListenerBase
        {
            string      Name { get; }
            string      Channel { get; }
            int         Order { get; }
        }
        
        public interface IMessageListener<T> : IMessageListenerBase 
        {
            void ProcessMessage(IMessage<T> e);
        }
        
        public class EventListenerWrapper
        {
            private IMessageListenerBase      m_MessageListener;
            
            public EventListenerWrapper(IMessageListenerBase iMessageListener, Type type)
            {
                m_MessageListener = iMessageListener;
                KeyType = type;
            }

            public Type                     KeyType { get; }

            public IMessageListenerBase         MessageListener => m_MessageListener;
            public string                       Name => m_MessageListener.Name;
            public string                       Channel => m_MessageListener.Channel ?? c_DefaultChannel;
            public int                          Order => m_MessageListener.Order;
            
            //////////////////////////////////////////////////////////////////////////
            public void ProcessMessage<T>(IMessage<T> e)
            {
                (m_MessageListener as IMessageListener<T>).ProcessMessage(e);
            }
        }

        public abstract class MessageListenerActionBase<T> : IMessageListener<T>
        {
            protected string        m_Name;
            protected string        m_Channel;
            protected int           m_Order;

            public string           Name => m_Name;
            public string           Channel => m_Channel;
            public int              Order => m_Order;

            //////////////////////////////////////////////////////////////////////////
            public abstract void ProcessMessage(IMessage<T> e);
            
            protected MessageListenerActionBase(string name, int order, string channel)
            {
                m_Name = string.IsNullOrEmpty(name) ? Guid.NewGuid().ToString() : name;
                m_Channel = channel;
                m_Order = order;
            }
        }

        public class MessageListenerAction<T> : MessageListenerActionBase<T>
        {
            private T           m_Key;
            public Action       m_Action;

            //////////////////////////////////////////////////////////////////////////
            public override void ProcessMessage(IMessage<T> e)
            {
                // if key matches invoke action
                if (e.Key.Equals(m_Key))
                    m_Action.Invoke();
            }

            public MessageListenerAction(string name, T key, Action action, int order, string channel)
                : base(name, order, channel)
            {
                m_Key = key;
                m_Action = action;
            }
        }

        public class MessageListenerStaticFunction<T> : MessageListenerActionBase<T>
        {
            private delegate void ProcessDelagate(IMessage<T> e);
            private delegate void CallDelagate();
            
            //////////////////////////////////////////////////////////////////////////
            private ProcessDelagate    m_Action;
            
            //////////////////////////////////////////////////////////////////////////
            public override void ProcessMessage(IMessage<T> e)
            {
                // if key matches invoke action
                m_Action(e as IMessage<T>);
            }

            public MessageListenerStaticFunction(string name, MethodInfo method, int order, string channel)
                : base(name, order, channel)
            {
                if (method.GetParameters().Length == 0)
                {
                    // action call
                    var call = (CallDelagate)Delegate.CreateDelegate(typeof(CallDelagate), method);
                    var key = method.GetCustomAttribute<MessageListenerAttribute>().EventKey;
                    m_Action = e =>
                    {
                        if (e.Key.Equals(key))
                            call();
                    };
                }
                else
                {
                    // proceed call
                    var key = method.GetCustomAttribute<MessageListenerAttribute>().EventKey;
                    var action = (ProcessDelagate)Delegate.CreateDelegate(typeof(ProcessDelagate), method);

                    // add key filter if specified
                    if (key == null)
                        m_Action = action;
                    else
                        m_Action = e => 
                        {
                            if (e.Key.Equals(key))
                                action(e);
                        };
                }

                // set defaults from method info
                if (name == null)
                    m_Name = method.Name;

                if (m_Channel == null)
                    m_Channel = c_DefaultChannel;
            }
        }
        
        private static class MessageIDDictionary
        {
            private const int                           c_MessageIDDictionarySize = 1024;
            private static Dictionary<string, int>      m_MessageID = new Dictionary<string, int>(c_MessageIDDictionarySize);

            // ReSharper disable once MemberHidesStaticFromOuterClass
            public static int StringToID(string message)
            {
                // get or add new message
                if (m_MessageID.TryGetValue(message, out var id) == false)
                {
                    id = m_MessageID.Count;
                    m_MessageID.Add(message, id);
                }

                return id;
            }
        }

        //////////////////////////////////////////////////////////////////////////
        private static MessageSystem         c_Instance;
        public static MessageSystem          Instance
        {
            get => c_Instance;
            protected set
            {
                c_Instance = value;

                // instance discarded
                if (c_Instance == null)
                    return;

                // need to parse assembly
                if (Instance.CollectClasses || Instance.CollectFunctions)
                {
                    var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(n => n.GetTypes());

                    // create listener instances
                    if (Instance.CollectClasses)
                    {
                        foreach (var type in types)
                        {
                            var attribure = type.GetCustomAttribute<MessageListenerAttribute>();
                            // not null & active
                            if (attribure != null && attribure.Active)
                            {
                                // must be creatable class
                                if (type.IsAbstract || type.IsClass == false || type.IsGenericType)
                                    continue;

                                // must implement event listener interface
                                if (typeof(IMessageListenerBase).IsAssignableFrom(type) == false)
                                    continue;
                                
                                // create & register listener
                                if (typeof(MonoBehaviour).IsAssignableFrom(type))
                                {
                                    // listener is monobehaviour type
                                    var el = new GameObject("", type).GetComponent(type) as MonoBehaviour;
                                    el.gameObject.name = el.name;
                                    el.transform.SetParent(Core.Instance.transform, true);

                                    AddListener(el as IMessageListenerBase);
                                }
                                else
                                {
                                    // listener is class
                                    var el = (IMessageListenerBase)Activator.CreateInstance(type);
                                    AddListener(el);
                                }
                            }
                        }
                    }

                    // create static function listeners
                    if (Instance.CollectFunctions)
                    {
                        foreach (var type in types)
                        {
                            // check all static methods
                            foreach (var methodInfo in type.GetMethods())
                            {
                                // must be static
                                if (methodInfo.IsStatic == false)
                                    continue;

                                // not generic
                                if (methodInfo.IsGenericMethod)
                                    continue;

                                var attribure = methodInfo.GetCustomAttribute<MessageListenerAttribute>();
                                // not null & active attribute
                                if (attribure == null || attribure.Active == false)
                                    continue;

                                var args = methodInfo.GetParameters();

                                // must have empty input or data input
                                if (args.Length != 0 &&
                                    args.Length != 1)
                                    continue;
                                // TODO: add IEvent support ~(isSubclassOfRawGeneric(typeof(IEvent<>), args[0].ParameterType)
                                
                                // create & register listener
                                var keyType = args.Length == 0 ? typeof(object) : args[0].ParameterType;
                                var el = Activator.CreateInstance(typeof(MessageListenerStaticFunction<>).MakeGenericType(keyType),
                                    attribure.Name ?? methodInfo.Name, methodInfo, attribure.Order, attribure.Channel) as IMessageListenerBase;
                                AddListener(el);
                            }
                        }
                    }
                }
            }
        }

        public const string                 c_DefaultChannel = "";
        public const object                 c_DefaultEventData = null;
        public const int                    c_DefaultOrder = 1;

        public bool                         SkipEvent { get; set; }

        public bool                         CollectClasses;
        public bool                         CollectFunctions;
        
        //////////////////////////////////////////////////////////////////////////
        protected void Awake()
        {
            Init();
        }

        protected void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        //////////////////////////////////////////////////////////////////////////
        void IMessageReceiver.Send<T>(IMessage<T> e)
        {
            _Send(e, e.Channel);
        }

        //////////////////////////////////////////////////////////////////////////
        public abstract void Init();

	    protected abstract void _Send<T>(IMessage<T> e, string channel);

	    protected abstract void _AddListener(EventListenerWrapper listener);

	    protected abstract void _RemoveEventListener(EventListenerWrapper eventListenerWrapper);

        protected abstract IEnumerable<EventListenerWrapper> _GetListeners();

	    //////////////////////////////////////////////////////////////////////////
        public static int StringToID(string message)
        {
            return MessageIDDictionary.StringToID(message);
        }

	    public static void RemoveListeners<T>()
        {
            // remove all of type
            RemoveListeners((listener) => listener is T);
        }

        public static void RemoveListeners(string name)
        {
            // remove with name
            RemoveListeners((listener) => listener.Name == name);
        }

        public static void RemoveListeners(Func<IMessageListenerBase, bool> condition)
        {
            // remove all matched listeners
            foreach (var eventListener in Instance._GetListeners().ToList())
            {
                if (condition(eventListener.MessageListener))
                    Instance._RemoveEventListener(eventListener);
            }
        }

        public static void Send<T>(IMessage<T> e)
        { 
            Instance._Send(e, e.Channel);
        }
        
        public static void Send<T>(IMessage<T> e, string channel)
        { 
            Instance._Send(e, channel);
        }
	    
	    public static void Send<T>(T key, string channel = c_DefaultChannel)
	    {
		    Instance._Send(new Message<T>(key, channel), channel);
	    }
        
        public static void Send<T, D>(T key, D data, string channel = c_DefaultChannel)
        {
            Instance._Send(new MessageData<T>(key, channel, data), channel);
        }
        
        public static void Send<T>(T key, params object[] data)
        {
            Instance._Send(new MessageData<T>(key, c_DefaultChannel, data), c_DefaultChannel);
        }
        
	    public static void AddListener(IMessageListenerBase listener)
	    {
            // allow multiply listeners in one
            var listeners = _extractListenerWrappers(listener);

            // push listeners in to the message system
            foreach (var listenerWrapper in listeners)
                Instance._AddListener(listenerWrapper);

	    }

	    public static void AddListener<T>(IMessageListener<T> listener)
	    {
		    Instance._AddListener(new EventListenerWrapper(listener, typeof(T)));
	    }
	    
	    public static void AddListener<T>(T descKey, Action action, int order = c_DefaultOrder, string name = "", string channel = c_DefaultChannel)
	    {
		    Instance._AddListener(new EventListenerWrapper(new MessageListenerAction<T>(name, descKey, action, order, channel), typeof(T)));
	    }

	    public static IMessageListener<T> GetEventListener<T>(string name)
	    {
		    return Instance._GetListeners().FirstOrDefault(n => n.KeyType == typeof(T) && n.Name == name)?.MessageListener as IMessageListener<T>;
	    }

	    public static void RemoveListener(IMessageListenerBase listener)
	    {
            // allow multiply listeners in one
            var listeners = _extractListenerWrappers(listener);

            // push listeners in to the message system
            foreach (var listenerWrapper in listeners)
		        Instance._RemoveEventListener(listenerWrapper);
	    }

        //////////////////////////////////////////////////////////////////////////
        private static IEnumerable<EventListenerWrapper> _extractListenerWrappers(IMessageListenerBase listener)
        {
            return listener
                .GetType()
                .GetInterfaces()
                .Where(n => n.IsGenericType && n.GetGenericTypeDefinition() == typeof(IMessageListener<>))
                .Select(n => new EventListenerWrapper(listener, n.GetGenericArguments()[0]));
        }
    }
}