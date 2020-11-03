using System;
using UnityEngine;

namespace Core
{
    [Serializable]
    public class LTWRotate : LeanTweenWrapper
    {
        [Serializable]
        public enum RotationMode
        {
            Rotate,
            RotateLocal,
            RotateX,
            RotateY,
            RotateZ,
        }

        //////////////////////////////////////////////////////////////////////////
        [SerializeField] 
        protected GameObject        m_Source;

        [SerializeField] 
        protected LeanTweenType     m_Ease = LeanTweenType.linear;
    
        [SerializeField]
        private float               m_Time = 1.0f;

        [SerializeField]
        private RotationMode        m_RotationMode;

        [SerializeField]
        private Vector3             m_Rotation;
    
        public Vector3              Rotation
        {
            set
            {
                m_Rotation = value;
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
                    switch (m_RotationMode)
                    {
                        case RotationMode.Rotate:
                            m_Descriptor = LeanTween.rotate(m_Source, m_Rotation, m_Time);
                            break;
                        case RotationMode.RotateLocal:
                            m_Descriptor = LeanTween.rotateLocal(m_Source, m_Rotation, m_Time);
                            break;
                        case RotationMode.RotateX:
                            m_Descriptor = LeanTween.rotateX(m_Source, m_Rotation.x, m_Time);
                            break;
                        case RotationMode.RotateY:
                            m_Descriptor = LeanTween.rotateY(m_Source, m_Rotation.y, m_Time);
                            break;
                        case RotationMode.RotateZ:
                            m_Descriptor = LeanTween.rotateZ(m_Source, m_Rotation.z, m_Time);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                
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