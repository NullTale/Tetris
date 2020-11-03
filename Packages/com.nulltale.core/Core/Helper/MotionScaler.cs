using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class MotionScaler : MonoBehaviour
    {
        [SerializeField]
        private int		                m_TrackLenght;
        [SerializeField]
        private Vector3		            m_ScaleMultiplyer = new Vector3(1.5f, 0.5f, 1.5f);
        [SerializeField]
        private float		            m_SpeedMaxEffect = 1.2f;
        private float		            m_CurrentSpeed;
        private Vector3		            m_CurrentSpeedVector;

        private LinkedList<Vector3>     m_Track = new LinkedList<Vector3>();
        private Vector3                 m_InitialScale;
        private Vector3                 m_LastPositon;

        //////////////////////////////////////////////////////////////////////////
        private void OnEnable()
        {
            // clear track
            m_Track.Clear();

            // fill track list with current position
            for (var n = 0; n < m_TrackLenght; n++)
                m_Track.AddLast(Vector3.zero);

            // save not modified scale
            m_InitialScale = transform.localScale;

            // clear values
            m_CurrentSpeed = 0.0f;
            m_CurrentSpeedVector = Vector3.zero;
            m_LastPositon = transform.position;
        }

        private void OnDisable()
        {
            // restore initial scale
            transform.localScale = m_InitialScale;
        }

        private void FixedUpdate()
        {
            // calculate average speed vector
            m_CurrentSpeedVector = Vector3.zero;
            var vectorWeight = 1.0f / m_Track.Count;
            foreach (var offset in m_Track)
                m_CurrentSpeedVector += offset * vectorWeight;

            // save current speed
            m_CurrentSpeed = m_CurrentSpeedVector.magnitude;

            // calculate relative scale
            var scale = Mathf.Clamp01(m_CurrentSpeed / m_SpeedMaxEffect);

            // apply scale
            transform.localScale = Vector3.Lerp(m_InitialScale, m_InitialScale.HadamardMul(m_ScaleMultiplyer), scale);

            // push new position
            m_Track.AddLast(transform.position - m_LastPositon);
            m_Track.RemoveFirst();
            m_LastPositon = transform.position;
        }

    }
}