using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

namespace Core
{
    [Serializable]
    public sealed class CoroutineWrapper : ISerializationCallbackReceiver
    {
        [SerializeField]
        private MonoBehaviour		m_Owner;
        [SerializeField]
        private string              m_EnumeratorFunctionName;

        private Coroutine			m_Coroutine;
        private EnumeratorFunction	m_EnumeratorFunction;

        public bool					IsRunning => m_Coroutine != null;
        public bool                 IsInitialized => m_Owner != null;

        private IEnumerator         m_Enumerator;

        public delegate IEnumerator EnumeratorFunction();

        //////////////////////////////////////////////////////////////////////////
        public CoroutineWrapper(EnumeratorFunction func, MonoBehaviour owner = null)
        {
            m_Owner = owner ? owner : global::Core.Core.Instance;
            m_EnumeratorFunction = func;
        }

        public bool Start()
        {
            if (m_Coroutine == null)
            {
                m_Coroutine = m_Owner.StartCoroutine(_EnumeratorWrapper());
                return true;
            }

            return false;
        }

        public bool Stop()
        {
            if (m_Coroutine != null)
            {
                m_Owner.StopCoroutine(m_Coroutine);
                m_Coroutine = null;
                return true;
            }

            return false;
        }

        public void Restart()
        {
            Stop();
            Start();
        }

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            // initialize wrapper if values a set
            if (m_Owner != null && string.IsNullOrEmpty(m_EnumeratorFunctionName) == false)
            {
                var method = m_Owner.GetType().GetMethod(m_EnumeratorFunctionName,
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                if (method != null)
                    m_EnumeratorFunction = (EnumeratorFunction)method.CreateDelegate(typeof(EnumeratorFunction), m_Owner);
            }
        }
	
        //////////////////////////////////////////////////////////////////////////
        private IEnumerator _EnumeratorWrapper()
        {
            yield return m_EnumeratorFunction();
            m_Enumerator = null;
            m_Coroutine = null;
        }

    }
}