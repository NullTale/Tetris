using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core
{
    [Serializable]
    public class LTWMove : LeanTweenWrapper
    {
        [SerializeField] 
        protected GameObject        m_Source;

        [Space]
        [SerializeField] 
        protected LeanTweenType     m_Ease = LeanTweenType.linear;
        //[SerializeField, SerializeReference] 
        //protected IAnimationCurve    m_Curve;
    
        [Space]
        [SerializeField]
        private MoveTime            m_MoveTime = MoveTime.Constant;
        [SerializeField]
        private float               m_Time = 1.0f;

        [Space]
        [SerializeField] 
        private MoveMode          m_MoveMode = MoveMode.Target;

        [SerializeField]
        [DrawIf("m_MoveMode", DrawIfAttribute.DisablingType.DontDraw, MoveMode.Position)]
        private Vector3             m_MovePosition;
        [SerializeField]
        [DrawIf("m_MoveMode", DrawIfAttribute.DisablingType.DontDraw, MoveMode.Target)]
        private Transform           m_MoveTarget;

        [SerializeField]
        private List<Transform>     m_MoveSpline;
        [SerializeField]
        private List<Transform>     m_MoveBezier;
    
        private LTSpline            m_LTSpline;
        private LTBezierPath        m_LTBezier;
    
        public Vector3              MovePosition
        {
            set
            {
                m_MovePosition = value;
            }
        }
        public Transform            MoveTarget
        {
            set
            {
                m_MoveTarget = value;
            }
        }
        public Transform[]          MoveSpline
        {
            set
            {
                if (value != null)
                {
                    m_MoveSpline.Clear();
                    m_MoveSpline.AddRange(value);
                    m_LTSpline = new LTSpline(value.Select(n => n.position).ToArray());
                }
                else
                {
                    m_MoveSpline.Clear();
                    m_LTSpline = null;
                }
            }
        }
        public Transform[]          MoveBezier
        {
            set
            {
                if (value != null)
                {
                    m_MoveBezier.Clear();
                    m_MoveBezier.AddRange(value);
                    m_LTBezier = new LTBezierPath(value.Select(n => n.position).ToArray());
                }
                else
                {
                    m_MoveBezier.Clear();
                    m_LTBezier = null;
                }
            }
        }

        public override LTDescr Descriptor
        {
            get
            {
                // initialize if null
                if (m_Descriptor == null)
                {
                    // instantiate descriptor
                    switch (m_MoveMode)
                    {
                        case MoveMode.Position:
                            m_Descriptor = LeanTween.move(m_Source, m_MovePosition, getTime());
                            break;
                        case MoveMode.Target:
                            m_Descriptor = LeanTween.move(m_Source, m_MoveTarget, getTime());
                            break;
                        case MoveMode.Spline:
                            m_LTSpline = new LTSpline(m_MoveSpline.Select(n => n.position).ToArray());
                            m_Descriptor = LeanTween.move(m_Source, m_LTSpline, getTime());
                            break;
                        case MoveMode.Bezier:
                            m_LTBezier = new LTBezierPath(m_MoveBezier.Select(n => n.position).ToArray());
                            m_Descriptor = LeanTween.move(m_Source, m_LTBezier, getTime());
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

                /////////////////////////////////////
                float getTime()
                {
                    // get time
                    var time = 0.0f;
                    switch (m_MoveTime)
                    {
                        case MoveTime.Constant:
                            time = m_Time;
                            break;
                        case MoveTime.Distance:
                            switch (m_MoveMode)
                            {
                                case MoveMode.Position:
                                    time = Vector3.Distance(m_Source.transform.position, m_MovePosition) * m_Time;
                                    break;
                                case MoveMode.Target:
                                    time = Vector3.Distance(m_Source.transform.position, m_MoveTarget.transform.position) * m_Time;
                                    break;
                                case MoveMode.Spline:
                                    time = m_LTSpline.distance * m_Time;
                                    break;
                                case MoveMode.Bezier:
                                    time = m_LTBezier.distance * m_Time;
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }

                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    return time;
                }
            }
        }

        //////////////////////////////////////////////////////////////////////////
        [Serializable]
        public enum MoveMode
        {
            Position,
            Target,
            Spline,
            Bezier
        }

        [Serializable]
        public enum MoveTime
        {
            Constant,
            Distance
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
            {
                LeanTween.cancel(m_Descriptor.uniqueId);
                m_Descriptor = null;
            }
        }
    }
}