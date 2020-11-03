using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using NaughtyAttributes;
using UnityEngine;


namespace Core.EventSystem
{
    // propagates received events to Listeners
    public class MessageSystemDefault : MessageSystem
    {
        private Dictionary<string, Dictionary<Type, SortedDictionary<int, List<EventListenerWrapper>>>>    m_Listeners = new Dictionary<string, Dictionary<Type, SortedDictionary<int, List<EventListenerWrapper>>>>();

        private sealed class EventListenerComparer : IComparer<EventListenerWrapper>
        {
            public int Compare(EventListenerWrapper x, EventListenerWrapper y)
            {
                // already contains, return equal
                if (ReferenceEquals(x.MessageListener, y.MessageListener))
                    return 0;
                
                // lowest is first
                var dif = x.Order - y.Order;
                
                // if order is equal put last
                if (dif == 0)
                    return 1;
                
                return dif;
            }
        }

        //////////////////////////////////////////////////////////////////////////
        public override void Init()
        {
            // do not init twice
            if (Instance == this)
                return;

            // set instance
            Instance = this;
        }

        //////////////////////////////////////////////////////////////////////////
	    protected override void _Send<T>(IMessage<T> e, string channel)
	    {
            // null check
		    if (e == null)
			    return;

            // propagate in channel to type group
            if (m_Listeners.TryGetValue(channel ?? c_DefaultChannel, out var typeGroup))
                foreach (var group in typeGroup)
                    if (group.Key == typeof(T))
                        foreach (var listener in group.Value)
                        foreach (var listenerWrapper in listener.Value)
                            if (SkipEvent)  return;
                            else            listenerWrapper.ProcessMessage(e);
	    }

	    protected override void _AddListener(EventListenerWrapper listener)
	    {
            // get or create channel collection
            if (m_Listeners.TryGetValue(listener.Channel, out var channel) == false)
            {
                // create new chanel
                channel = new Dictionary<Type, SortedDictionary<int, List<EventListenerWrapper>>>();
                m_Listeners.Add(listener.Channel, channel);
            }

            // get or create group
            if (channel.TryGetValue(listener.KeyType, out var group) == false)
            {
                group = new SortedDictionary<int, List<EventListenerWrapper>>();
                channel.Add(listener.KeyType, group);
            }

            // add or create order group
            if (group.TryGetValue(listener.Order, out var orderGroup) == false)
            {
                orderGroup = new List<EventListenerWrapper>(4);
                group.Add(listener.Order, orderGroup);
            }

            // finally add listener if collection doesn't contains it
            if (orderGroup.Exists(n => n.MessageListener == listener.MessageListener) == false)
                orderGroup.Add(listener);
	    }

	    protected override void _RemoveEventListener(EventListenerWrapper listener)
        {
            if (listener == null) 
                return;

            // remove first much
            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            
            if (m_Listeners.TryGetValue(listener.Channel, out var channel) &&
                channel.TryGetValue(listener.KeyType, out var group))
                foreach (var orderGroup in group)
                {
                    var wrappers = orderGroup.Value;
                    var index = wrappers.FindIndex(n => n.MessageListener == listener.MessageListener);
                    if (index != -1)
                    {
                        wrappers.RemoveAt(index);
                        // remove unused groups
                        if (wrappers.Count == 0)
                        {
                            // remove ordered group
                            group.Remove(orderGroup.Key);

                            if (group.Count == 0)
                            {
                                // remove type group
                                channel.Remove(listener.KeyType);

                                if (channel.Count == 0)
                                {
                                    // remove channel
                                    m_Listeners.Remove(listener.Channel);
                                }
                            }
                        }
                        return;
                    }
                }
        }

        protected override IEnumerable<EventListenerWrapper> _GetListeners()
        {
            return m_Listeners.SelectMany(group => group.Value.SelectMany(n => n.Value.SelectMany(l => l.Value)));
        }

        [Button]
        public void LogListeners()
        {
            foreach (var channel in m_Listeners)
            {
                Debug.Log($"group: {channel.Key}");
                foreach (var group in channel.Value)
                foreach (var listenerWrappers in group.Value)
                foreach (var eventListenerWrapper in listenerWrappers.Value)
                    Debug.Log($"--> name: {eventListenerWrapper.Name}; listener: {eventListenerWrapper.MessageListener.GetType()}; type: {group.Key}");
            }
        }
    }
}