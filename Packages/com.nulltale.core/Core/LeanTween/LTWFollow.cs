using System;
using UnityEngine;

namespace Core
{
    [Serializable]
    public class LTWFollow : LeanTweenWrapper
    {
        [SerializeField]
        private FollowMode      m_FollowMode = FollowMode.Linear;
        [SerializeField]
        private LeanProp        m_FollowProperty = LeanProp.position;

        [SerializeField]
        private Vector3         m_Offset;

        [SerializeField]
        private Transform       m_Source;
        [SerializeField]
        private Transform       m_Target;
    
        [SerializeField]
        [DrawIf("m_FollowMode", DrawIfAttribute.DisablingType.DontDraw, FollowMode.Linear, FollowMode.Damp)]
        private float           m_Speed = 1.0f;
        [SerializeField]
        [DrawIf("m_FollowMode", DrawIfAttribute.DisablingType.DontDraw, FollowMode.Damp, FollowMode.BounceOut)]
        private float           m_MaxSpeed = -1.0f;
        [SerializeField]
        [DrawIf("m_FollowMode", DrawIfAttribute.DisablingType.DontDraw, FollowMode.BounceOut, FollowMode.Spring)]
        private float           m_SmoothTime = 0.1f;
        [SerializeField]
        [DrawIf("m_FollowMode", DrawIfAttribute.DisablingType.DontDraw, FollowMode.BounceOut, FollowMode.Spring)]
        private float           m_Friction = 20f;
        [SerializeField]
        [DrawIf("m_FollowMode", DrawIfAttribute.DisablingType.DontDraw, FollowMode.BounceOut, FollowMode.Spring)]
        private float           m_AccelRate = 0.9f;
        [SerializeField]
        [DrawIf("m_FollowMode", FollowMode.BounceOut)]
        private float           m_HitDumping = 0.9f;

        public override LTDescr Descriptor
        {
            get
            {
                // initialize if null
                if (m_Descriptor == null)
                {
                    switch (m_FollowMode)
                    {
                        case FollowMode.Linear:
                            m_Descriptor = LeanTween.followLinear(m_Source, m_Target, m_FollowProperty, m_Speed);
                            break;
                        case FollowMode.Damp:
                            m_Descriptor = LeanTween.followDamp(m_Source, m_Target, m_FollowProperty, m_Speed, m_MaxSpeed);
                            break;
                        case FollowMode.BounceOut:
                            m_Descriptor = LeanTween.followBounceOut(m_Source, m_Target, m_FollowProperty, m_SmoothTime, m_MaxSpeed, m_Friction, m_AccelRate, m_HitDumping);
                            break;
                        case FollowMode.Spring:
                            m_Descriptor = LeanTween.followSpring(m_Source, m_Target, m_FollowProperty, m_SmoothTime, m_MaxSpeed, m_Friction, m_AccelRate);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                
                    // initialize
                    m_Descriptor
                        .setTarget(m_Target)
                        .setOffset(m_Offset)
                        .setFrom(Vector3.zero)
                        .setDiff(m_Source.transform.position)
                        .setAxis(m_Source.transform.position)
                        /*.setOnStart(() =>
                    {
                        m_Descriptor
                            .setDiff(m_Source.transform.position)
                            .setAxis(m_Source.transform.position)
                            .setTarget(m_Target)
                            .setOffset(m_Offset);
                    })*/
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
    
        public Vector3 Offset
        {
            get => m_Offset;
            set
            {
                m_Offset = value;

                // set offset at runtime
                m_Descriptor?.setOffset(m_Offset);
            }
        }

        public Transform Target
        {
            get => m_Target;
            set
            {
                m_Target = value;

                // if initialized and not paused, set target at runtime
                if (m_Descriptor != null && m_Descriptor.direction != 0.0f)
                    m_Descriptor.setTarget(m_Target);
            }
        }
    
        //////////////////////////////////////////////////////////////////////////
        [Serializable]
        public enum FollowMode
        {
            Linear,
            Damp,
            BounceOut,
            Spring
        }

        //////////////////////////////////////////////////////////////////////////
        public override LTDescr Start()
        {
            m_Descriptor?
                //.setAxis(m_Source.transform.position)
                .setDiff(m_Source.transform.position);
            //.setFrom(Vector3.zero)*/;

            return Descriptor.resume();
        }

        public override LTDescr Pause()
        {
            return Descriptor.pause();
        }

        public override void Cancel()
        {
            if (m_Descriptor != null)
                LeanTween.cancel(m_Descriptor.uniqueId, true);
        }
    }
}