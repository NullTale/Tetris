using UnityEngine;
using NaughtyAttributes;

namespace Core
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PhysicsAffector : MonoBehaviour
    {
        private Rigidbody2D			m_Rigidbody;
	
        public bool					m_MoveEnable;
        public Vector2				m_MovePosition;
        public float				m_MoveRotation;
	
        public bool					m_ForceEnable;
        public Vector2				m_Force;
        public ForceMode2D			m_PositionForceMode;
        public float				m_Tourque;
        public ForceMode2D			m_TourqueForceMode;

        //////////////////////////////////////////////////////////////////////////
        private void Awake()
        {
            m_Rigidbody = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            if (m_MoveEnable)
            {
                m_Rigidbody.MovePosition(m_Rigidbody.position + m_MovePosition);
                m_Rigidbody.MoveRotation(m_Rigidbody.rotation + m_MoveRotation);
            }

            if (m_ForceEnable)
            {
                m_Rigidbody.AddForce(m_Force * Time.fixedDeltaTime, m_PositionForceMode);
                m_Rigidbody.AddTorque(m_Tourque * Time.fixedDeltaTime, m_TourqueForceMode);
            }
        }

        //////////////////////////////////////////////////////////////////////////
        [Button]
        void Zero()
        {
            ZeroVelocity();
            ZeroRotation();
        }
        [Button]
        void ZeroVelocity()
        {
            m_Rigidbody.velocity = Vector2.zero;
        }
        [Button]
        void ZeroRotation()
        {
            m_Rigidbody.angularVelocity = 0.0f;
        }

        [Button]
        void ForceOnce()
        {
            PosForceOnce();
            RotForceOnce();
        }
        [Button]
        void PosForceOnce()
        {
            m_Rigidbody.AddForce(m_Force, m_PositionForceMode);
        }
        [Button]
        void RotForceOnce()
        {
            m_Rigidbody.AddTorque(m_Tourque, m_TourqueForceMode);
        }
    }
}