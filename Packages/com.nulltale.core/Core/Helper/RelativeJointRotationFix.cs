using UnityEngine;

namespace Core
{
    [RequireComponent(typeof(RelativeJoint2D))]
    public class RelativeJointRotationFix : MonoBehaviour
    {
        private	Rigidbody2D		m_RigidBody;
        private RelativeJoint2D	m_Joint;
        private float			m_PrevRotation;

        //////////////////////////////////////////////////////////////////////////
        private void Awake()
        {
            m_Joint = GetComponent<RelativeJoint2D>();
            if(m_Joint == null)			{ enabled = false; return; }

            m_RigidBody = m_Joint.connectedBody;
            if(m_RigidBody == null)		{ enabled = false; return; }

            m_PrevRotation = m_RigidBody.rotation;
        }

        private void FixedUpdate()
        {
            if(m_PrevRotation - m_RigidBody.rotation > 180.0f)
                m_Joint.angularOffset = m_Joint.angularOffset * Mathf.Deg2Rad - Mathf.PI * 2.0f;
            else
            if(m_PrevRotation - m_RigidBody.rotation < -180.0f)
                m_Joint.angularOffset = m_Joint.angularOffset * Mathf.Deg2Rad + Mathf.PI * 2.0f;

            m_PrevRotation = m_RigidBody.rotation;
        }

    }
}