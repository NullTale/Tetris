using NaughtyAttributes;
using UnityEngine;

namespace Core.EventSystem
{
    public abstract class MessageListener<T> : MonoBehaviour, MessageSystem.IMessageListener<T>
    {
        [SerializeField, Foldout("Listener")]
        private string          m_Channel = MessageSystem.c_DefaultChannel;
        [SerializeField, Foldout("Listener")]
        private int             m_Order;
        [SerializeField, Foldout("Listener"), Tooltip("Enable auto connection OnEnable")]
        private bool            m_AutoConnect;
        [SerializeField, Foldout("Listener"), ReadOnly]
        private bool            m_Connected;

        public string           Name => gameObject.name;
        public string           Channel
        {
            get => m_Channel;
            set
            {
                if (m_Channel == value)
                    return;

                m_Channel = value;

                // reconnect if channel was changed
                if (m_Connected)
                    _Reconnect();
            }
        }
        public int              Order
        {
            get => m_Order;
            set
            {
                if (m_Order == value)
                    return;

                m_Order = value;

                // reconnect if order was changed
                if (m_Connected)
                    _Reconnect();
            }
        }

        public bool             AutoConnect
        {
            get => m_AutoConnect;
            set
            {
                m_AutoConnect = value;

                // do auto connect if enabled
                if (m_AutoConnect && isActiveAndEnabled)
                    ConnectListener();
            }
        }

        //////////////////////////////////////////////////////////////////////////
        public abstract void ProcessMessage(IMessage<T> e);

        public virtual void OnEnable()
        {
            if (m_AutoConnect)
                ConnectListener();
        }

        public virtual void OnDisable()
        {
            DisconnectListener();
        }

        public void ConnectListener()
        {
            // connect if disconnected
            if (m_Connected)
                return;

            MessageSystem.AddListener(this);
            m_Connected = true;
        }

        public void DisconnectListener()
        {
            // disconnect if connected
            if (m_Connected == false)
                return;

            if (MessageSystem.Instance != null)
                MessageSystem.RemoveListener(this);
            m_Connected = false;
        }

        //////////////////////////////////////////////////////////////////////////
        private void _Reconnect()
        {
            DisconnectListener();
            ConnectListener();
        }
    }
}