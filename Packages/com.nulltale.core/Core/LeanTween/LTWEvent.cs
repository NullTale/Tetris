using System;
using UnityEngine;
using UnityEngine.Events;

namespace Core
{
    [Serializable]
    public class LTWEvent : LeanTweenWrapper
    {
        [SerializeField]
        private float       m_Delay;
        [SerializeField] 
        private UnityEvent  m_Event;
    
        public override LTDescr Descriptor
        {
            get
            {
                if (m_Descriptor == null)
                {
                    m_Descriptor = LeanTween.delayedCall(m_Delay, () =>
                    {
                        m_Event.Invoke();
                        m_Descriptor = null;
                    });

                    m_Descriptor.pause();
                }

                return m_Descriptor;
            }
        }

        //////////////////////////////////////////////////////////////////////////
        public override LTDescr Start()
        {
            return Descriptor.resume();
        }

        public override LTDescr Pause()
        {
            return Descriptor.pause();
        }

        public override void Cancel()
        {
            if (m_Descriptor != null)
                LeanTween.cancel(m_Descriptor.uniqueId);
        }
    }
}