using System;
using UnityEngine;

namespace Core
{
    [DefaultExecutionOrder(1)]
    public class OrientedSprite : MonoBehaviour 
    {
        [Serializable]
        public enum OrientationTarget
        {
            None,
            MainCamera,
            Target,
        }

        [Serializable]
        public enum AxisVector
        {
            X,
            Y,
            Z,
            TargetY,
            Custom,
        }

        //////////////////////////////////////////////////////////////////////////
        public static int     c_DirectionX = Animator.StringToHash("DirectionX"); 
        public static int     c_DirectionY = Animator.StringToHash("DirectionY");

        [SerializeField]
        private OrientationTarget   m_Mode = OrientationTarget.MainCamera;
    
        [DrawIf("m_Mode", OrientationTarget.Target)]
        public Transform            m_Target;
        [NonSerialized]
        public Transform            m_CurrentTarget;

        [Space]
        public Animator             m_Animator;
        [DrawIf("m_Animator")]
        public float                m_AnimatorOrientation;	// in radians
        public float                m_OrientationDegree;
	
        [HideInInspector, NonSerialized]
        public float                m_Atan;
        [HideInInspector, NonSerialized]
        public Vector3              m_ToTarget;

        public OrientationTarget    Target
        {
            get => m_Mode;
            set
            { 
                if (m_Mode == value)
                    return;

                // apply mode
                m_Mode = value;
                _ApplyMode();
            }
        }
        //////////////////////////////////////////////////////////////////////////
        private void Awake()
        {
            _ApplyMode();
        }

        public void Update()
        {
            if (m_CurrentTarget == null)
                return;

            // update values
            m_ToTarget = (m_CurrentTarget.position - transform.position);
            m_Atan = Mathf.Atan2(m_ToTarget.z, m_ToTarget.x);
		
            m_OrientationDegree = (m_Atan + Mathf.PI * 0.5f) * Mathf.Rad2Deg;
        
            // set rotation to camera
            transform.rotation = Quaternion.AngleAxis(-m_OrientationDegree, Vector3.up);
		
            // apply to animator
            if (m_Animator != null)
            {
                m_Animator.SetFloat(c_DirectionX, Mathf.Cos(m_Atan - m_AnimatorOrientation));
                m_Animator.SetFloat(c_DirectionY, Mathf.Sin(m_Atan - m_AnimatorOrientation));
            }
        }

        public void LieDown(Vector2 direction)
        {
            enabled = false;
		
            var atan = Mathf.Atan2(-direction.y, direction.x);
            transform.rotation = Quaternion.AngleAxis((atan + Mathf.PI * 0.5f) * Mathf.Rad2Deg, Vector3.back);

            // look forward
            m_Animator.SetFloat(c_DirectionX, 1.0f);
            m_Animator.SetFloat(c_DirectionY, 0.0f);
        }
	
        public void GetUp()
        {
            enabled = true;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + new Vector3(Mathf.Cos(-m_AnimatorOrientation), 0.0f, Mathf.Sin(m_AnimatorOrientation)));
        }
    
        //////////////////////////////////////////////////////////////////////////
        private void _ApplyMode()
        {
            switch (m_Mode)
            {
                case OrientationTarget.None:
                    m_CurrentTarget = null;
                    break;
                case OrientationTarget.MainCamera:
                    m_CurrentTarget = global::Core.Core.Instance.Camera.transform;
                    break;
                case OrientationTarget.Target:
                    m_CurrentTarget = m_Target;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(m_Mode), m_Mode, null);
            }
        }
        /*
    [SerializeField]
    private AxisVector          m_AxisVectorMode = AxisVector.Y;
    [SerializeField, DrawIf("m_AxisVectorMode", AxisVector.Custom)]
    private Vector3             m_AxisVector;
    [SerializeField, DrawIf("m_AxisVectorMode", AxisVector.TargetY)]
    private Transform           m_AxisTarget;
    
    private Vector3 _GetUpVector()
    {
        switch (m_AxisVectorMode)
        {
            case AxisVector.X:
                return Vector3.right;
            case AxisVector.Y:
                return Vector3.up;
            case AxisVector.Z:
                return Vector3.forward;
            case AxisVector.TargetY:
                return m_AxisTarget.transform.up;
            case AxisVector.Custom:
                return m_AxisVector;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void _ApplyUpMode()
    {
    }*/
    }
}