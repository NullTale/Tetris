using UnityEngine;
using System;
using UnityEngine.Events;

namespace Core
{
    [DefaultExecutionOrder(-1)]
    public class OnStartCallback : CallbackBase
    {
        public event Action<GameObject>		OnStartAction;
        public UnityEvent					OnStartEvent;
        public bool                         m_DestroyAfterExecution;

        //////////////////////////////////////////////////////////////////////////
        private void Start()
        {
            OnStartAction?.Invoke(gameObject);
            OnStartEvent?.Invoke();

            if (m_DestroyAfterExecution)
                Destroy(this);
        }
    }
}