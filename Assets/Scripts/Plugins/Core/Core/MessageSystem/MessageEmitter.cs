using System;
using System.Linq;
using System.Reflection;
using NaughtyAttributes;
using UnityEngine;

namespace Core.EventSystem
{
    [Serializable]
    public class MessageEmitter : MonoBehaviour
    {
        [SerializeField]
        private string          m_Channel = MessageSystem.c_DefaultChannel;

        //////////////////////////////////////////////////////////////////////////
        public void Send<T, D>(T key, D data)
        {
            MessageSystem.Send(key, data);
        }

        public void Send(object key)
        {
            // send generic message
            var message = Activator.CreateInstance(typeof(Message<>).MakeGenericType(key.GetType()), key);

            typeof(MessageSystem)
                .GetMethod("_Send", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.MakeGenericMethod(key.GetType()).Invoke(MessageSystem.Instance, new []{message, m_Channel});
        }

        public void Send(object key, object data)
        {
            // send generic message data
            var message = Activator.CreateInstance(typeof(MessageData<>).MakeGenericType(key.GetType()), key, data);

            typeof(MessageSystem)
                .GetMethod("_Send", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.MakeGenericMethod(key.GetType()).Invoke(MessageSystem.Instance, new []{message, m_Channel});
        }
    }
}