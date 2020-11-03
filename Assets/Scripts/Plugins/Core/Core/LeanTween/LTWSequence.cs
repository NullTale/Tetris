using System;
using Malee;
using UnityEngine;

namespace Core
{
    [Serializable]
    public class LTWSequence : LeanTweenWrapper
    {
        private LeanTweenWrapper        m_CurrentTween;
        private int                     m_CurrentTweenIndex;
        [SerializeField]
        private TweenList               m_Sequence;

        public override LTDescr Descriptor
        {
            get
            {
                if (m_Descriptor == null)
                {
                    // instantiate process tween
                    m_Descriptor = LeanTween.delayedCall(float.MaxValue, () => {});
                    m_Descriptor.setOnComplete(() =>
                    {
                        // clear data on cancel
                        m_Descriptor = null;

                        m_CurrentTween.Cancel();
                        m_CurrentTween = null;
                    });

                    m_Descriptor.setOnStart(() =>
                    {
                        /*if (m_Sequence.Count == 0)
                        m_Descriptor.cancel().*/

                        // start execution from current tween
                        if (m_CurrentTween == null)
                        {
                            // start from begin
                            m_CurrentTweenIndex = 0;
                            m_CurrentTween = m_Sequence[m_CurrentTweenIndex].m_LeanTweenWrapper;
                        
                            // include in sequence
                            setOnComplete(m_CurrentTween.Descriptor);
                        }

                        m_CurrentTween.Start();
                    });

                    // pause execution
                    m_Descriptor.pause();
                }


                return m_Descriptor;

                /////////////////////////////////////
                void setOnComplete(LTDescr tween)
                {
                    // add move next function onComplete
                    tween._optional.onComplete += () =>
                    {
                        // if running
                        if (m_Descriptor != null)
                        {
                            m_CurrentTweenIndex++;
                            if (m_CurrentTweenIndex < m_Sequence.Count)
                            {
                                // set next tween
                                m_CurrentTween = m_Sequence[m_CurrentTweenIndex].m_LeanTweenWrapper;
                                // include in sequence
                                setOnComplete(m_CurrentTween.Descriptor);
                                // start tween
                                m_CurrentTween.Start();
                            }
                            else
                            {
                                // cancel main descriptor
                                LeanTween.cancel(m_Descriptor.uniqueId, true);
                            }
                        }
                    };
                    tween.hasExtraOnCompletes = true;
                }
            }
        }

        //////////////////////////////////////////////////////////////////////////
        [Serializable]
        public class TweenList : ReorderableArray<SequenceElement> {}

        [Serializable]
        public class SequenceElement
        {
            [SerializeReference, ClassReference]
            public LeanTweenWrapper  m_LeanTweenWrapper;
        }

        //////////////////////////////////////////////////////////////////////////
        public override LTDescr Start()
        {
            var descriptor = Descriptor;
            m_CurrentTween?.Start();
            m_Descriptor.resume();

            return descriptor;
        }

        public override LTDescr Pause()
        {
            var descriptor = Descriptor;
            m_CurrentTween?.Pause();
            m_Descriptor.pause();

            return descriptor;
        }

        public override void Cancel()
        {
            if (m_Descriptor != null)
                LeanTween.cancel(m_Descriptor.uniqueId);
        }
    }
}