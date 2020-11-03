using UnityEngine;
using System;
using UnityEngine.Events;

namespace Core
{
    public class OnDestroyCallback : CallbackBase
    {
        public event Action<GameObject>		OnDestroyAction;
        public UnityEvent					OnDestroyEvent;

        //////////////////////////////////////////////////////////////////////////
        private void OnDestroy()
        {
            OnDestroyAction?.Invoke(gameObject);
            OnDestroyEvent?.Invoke();
        }
    }
}