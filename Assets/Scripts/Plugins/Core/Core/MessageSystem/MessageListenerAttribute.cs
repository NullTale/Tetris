using System;

namespace Core.EventSystem
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class MessageListenerAttribute : Attribute
    {
        /// <summary> Is enabled </summary>
        public bool Active { get; set; } = true;
        /// <summary> Event key, if function takes as argument Core.EventSystem.Event that can be ignored </summary>
        public string EventKey { get; set; } = null;
        /// <summary> Listener order, putting on top is equals to other order listeners </summary>
        public int Order { get; set; } = MessageSystem.c_DefaultOrder;
        /// <summary> Listener name, if not set it will be MethodInfo name </summary>
        public string Name { get; set; } = null;
        /// <summary> Listener chanel </summary>
        public string Channel { get; set; } = MessageSystem.c_DefaultChannel;
    }
}