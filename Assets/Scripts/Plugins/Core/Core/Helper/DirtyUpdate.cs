using System;
using System.Collections;
using Core;
using UnityEngine;
using Action = System.Action;

namespace Core
{
    [Serializable]
    public class DirtyUpdate
    {
        // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
        private const Dirty         c_DirtyFlags = Dirty.FixedUpdate | Dirty.Update;
    

        [SerializeField]
        private Dirty               m_CurrentState;
        private	Coroutine           m_UpdateCoroutine;

        public bool                 IsDirty => (c_DirtyFlags | m_CurrentState) != Dirty.None;

        public Dirty                CurrentState
        {
            get => m_CurrentState;
            private set => m_CurrentState = value;
        }

        //////////////////////////////////////////////////////////////////////////
        public void Clean()
        {
            // clean
            _CancelUpdate();
            m_CurrentState = Dirty.Clean;
        }

        public void SetDirty(Dirty dirty)
        {
            if (m_CurrentState == dirty)
                return;

            // start or stop update
            switch (dirty)
            {
                case Dirty.Clean:
                    _CancelUpdate();
                    break;

                case Dirty.Update:
                case Dirty.FixedUpdate:
                case Dirty.ImmediateUpdate:
                    throw new ArgumentOutOfRangeException(nameof(dirty), "invalid parameter use action argument function");

                case Dirty.None:
                default:
                    throw new ArgumentOutOfRangeException(nameof(dirty), dirty, null);
            }
		
            // save state
            m_CurrentState = dirty;
        }

        public void SetDirty(Dirty dirty, Action updateAction)
        {
            SetDirty(dirty, _ActionCall(updateAction));
        }

        public void SetDirty(Dirty dirty, IEnumerator updateAction)
        { 
            if (m_CurrentState == dirty)
                return;

            // if state was changed always stop current update
            _CancelUpdate();

            // start update
            switch (dirty)
            {
                // delayed update
                case Dirty.Update:
                    m_UpdateCoroutine = global::Core.Core.Instance.StartCoroutine(_CleanUpdate(updateAction));
                    break;
                case Dirty.FixedUpdate:
                    m_UpdateCoroutine = global::Core.Core.Instance.StartCoroutine(_CleanFixedUpdate(updateAction));
                    break;
                case Dirty.ImmediateUpdate:
                    m_UpdateCoroutine = global::Core.Core.Instance.StartCoroutine(_CleanImmediate(updateAction));
                    break;

                // clean (_StopUpdate() call's upper)
                case Dirty.Clean:
                    break;


                case Dirty.None:
                default:
                    throw new ArgumentOutOfRangeException(nameof(dirty), dirty, null);
            }
		
            // save state
            m_CurrentState = dirty;
        }
    
        //////////////////////////////////////////////////////////////////////////
        private IEnumerator _CleanFixedUpdate(IEnumerator action) 
        {
            // call update action delayed
            yield return new WaitForFixedUpdate();

            yield return action;
            m_CurrentState = Dirty.Clean;
            m_UpdateCoroutine = null;
        }

        private IEnumerator _CleanUpdate(IEnumerator action) 
        {
            // call update action delayed
            yield return null;
        
            yield return action;
            m_CurrentState = Dirty.Clean;
            m_UpdateCoroutine = null;
        }

        private IEnumerator _CleanImmediate(IEnumerator action) 
        {
            yield return action;
            m_CurrentState = Dirty.Clean;
            m_UpdateCoroutine = null;
        }
    
        private IEnumerator _ActionCall(Action action)
        {
            // call action
            action?.Invoke();

            yield break;
        }

        private void _CancelUpdate()
        {
            // interrupt update coroutine
            if (m_UpdateCoroutine != null)
            {
                global::Core.Core.Instance.StopCoroutine(m_UpdateCoroutine);
                m_UpdateCoroutine = null;
            }
        }
    }
}

[Serializable]
public class DirtyUpdate<T> : DirtyUpdate
{
    public Action       UpdateAction { get; private set; }

    [SerializeField]
    private Dirty       m_UpdateMode = Dirty.ImmediateUpdate;

    public T            m_Value;
    public T            Value
    {
        get => m_Value;
        set
        {
            // set only new value
            if (m_Value.Equals(value))
                return;

            // set value, start update coroutine
            m_Value = value;
            SetDirty(m_UpdateMode, UpdateAction);
        }
    }
}

[Serializable]
public enum Dirty
{
	None	            = 0,

    Clean	            = 1,
	Update	            = 1 << 2,
	FixedUpdate		    = 1 << 3,
    ImmediateUpdate     = 1 << 4,
}