using System;
using UnityEngine;

namespace Core
{
    [Serializable]
    public class LTWValue : LeanTweenWrapper
    {
        [SerializeField] 
        protected GameObject        m_Source;

        [SerializeField] 
        protected LeanTweenType     m_Ease = LeanTweenType.linear;
    
        [SerializeField]
        private float               m_Time = 1.0f;

        [SerializeField]
        private float               m_From;

        [SerializeField]
        private float               m_To;

        private Action<float, float>    m_OnUpdateRatio;

        public override LTDescr     Descriptor
        {
            get
            {
                // initialize if null
                if (m_Descriptor == null)
                {
                    // instantiate descriptor
                    m_Descriptor = LeanTween.value(m_Source, (value, ratio) => m_OnUpdateRatio?.Invoke(value, ratio), m_From, m_To, m_Time);
                
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

        public Action<float, float> OnUpdateRatio
        {
            get => m_OnUpdateRatio;
            set => m_OnUpdateRatio = value;
        }

        public float To
        {
            get => m_To;
            set => m_To = value;
        }

        public float From
        {
            get => m_From;
            set => m_From = value;
        }

        //////////////////////////////////////////////////////////////////////////
        public override LTDescr Start()
        {
            return Descriptor.resume();
        }

        public LTDescr Start(Action<float, float> interpolationRatio)
        {
            OnUpdateRatio = interpolationRatio;
            return Descriptor.resume();
        }

        public override LTDescr Pause()
        {
            return Descriptor.pause();
        }

        public override void Cancel()
        {
            if (m_Descriptor != null)
            {
                LeanTween.cancel(m_Descriptor.uniqueId);
                m_Descriptor = null;
            }
        }

        public void AddToSequence(LTSeq sequence)
        {
            sequence.append(Descriptor);
        }
    }
}