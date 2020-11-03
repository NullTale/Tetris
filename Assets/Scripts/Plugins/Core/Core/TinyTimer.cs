using UnityEngine;
using System;

namespace Core
{
    [Serializable]
    public class TinyTimer
    {
        [SerializeField]
        private float		m_CurrentTime = 0.0f;
        public float		TimePassed => m_CurrentTime;
        public float		Scale => m_CurrentTime / m_FinishTime;

        [SerializeField]
        private bool        m_AutoReset;
        public bool AutoReset
        {
            get => m_AutoReset;
            set
            {
                if (m_AutoReset == value)
                    return;

                m_AutoReset = value;

                // reset by excess of time
                if (m_AutoReset && Expired)
                    Reset(Excess, m_FinishTime);
            }
        }

        public bool			Running	=> m_CurrentTime < m_FinishTime;
        public bool			Expired	=> m_CurrentTime >= m_FinishTime;
        public float		Excess => Mathf.Max(m_CurrentTime - m_FinishTime, 0.0f);
        public float		Remainder => Mathf.Max(m_FinishTime - m_CurrentTime, 0.0f);

        [SerializeField]
        private float		m_FinishTime;
        public float		FinishTime
        {
            get => m_FinishTime;
            set => m_FinishTime = value;
        }

        //////////////////////////////////////////////////////////////////////////
        public bool AddTime(float time)
        {
            m_CurrentTime += time;

            var expired = Expired;
            if (expired && m_AutoReset)
                Reset(Excess, m_FinishTime);

            return expired;
        }

        /// <summary>
        /// Set current & finish time
        /// </summary>
        /// <param name="currentTimer"></param>
        /// <param name="finishTime"></param>
        public void Reset(float currentTimer, float finishTime)
        {
            m_CurrentTime = currentTimer;
            m_FinishTime = finishTime;
        }
        /// <summary>
        /// Set finish time & discard current time
        /// </summary>
        /// <param name="finishTime"></param>
        public void Reset(float finishTime)
        {
            m_CurrentTime = 0.0f;
            m_FinishTime = finishTime;
        }
	
        /// <summary>
        /// Set current time to 0.0f
        /// </summary>
        public void Reset()
        {
            m_CurrentTime = 0.0f;
        }

        /// <summary>
        /// Subtract time length from current time, clamp current tine to zero
        /// </summary>
        public void CloseCircle()
        {
            m_CurrentTime = Mathf.Max(m_CurrentTime - m_FinishTime, 0.0f);
        }
	
        public TinyTimer()
        {
        }

        public TinyTimer(float finishTime, bool autoReset)
        {
            m_FinishTime = finishTime;
            m_AutoReset = autoReset;
        }
    }
}