using System;
using UnityEngine;

namespace Core
{
    [Serializable]
    public class LTWScale : LeanTweenWrapper
    {
        [SerializeField] 
        protected GameObject        m_Source;

        [SerializeField] 
        protected LeanTweenType     m_Ease = LeanTweenType.linear;
    
        [SerializeField]
        private float               m_Time = 1.0f;

        [SerializeField]
        private Vector3             m_Scale;
    
        public Vector3              Scale
        {
            set
            {
                m_Scale = value;
            }
        }

        public override LTDescr     Descriptor
        {
            get
            {
                // initialize if null
                if (m_Descriptor == null)
                {
                    // instantiate descriptor
                    m_Descriptor = LeanTween.scale(m_Source, m_Scale, m_Time);
                
                    // set ease & onComplete event (set descriptor to null)
                    m_Descriptor
                        .setEase(m_Ease)
                        .setOnComplete(() =>
                        {
                            m_Descriptor = null;
                        });

                    // pause tween
                    m_Descriptor.pause();
                }

                return m_Descriptor;
            }
        }

        public float                Time => m_Time;

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

        public void AddToSequence(LTSeq sequence)
        {
            sequence.append(Descriptor);
        }
    }
}