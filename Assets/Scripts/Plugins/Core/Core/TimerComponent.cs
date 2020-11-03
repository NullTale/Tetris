using UltEvents;
using UnityEngine;

namespace Core
{
    public class TimerComponent : MonoBehaviour
    {
        public TinyTimer	m_Timer;
        public UltEvent		m_Expired;
        public bool			m_Loop;
        private bool		m_Expiered;

        //////////////////////////////////////////////////////////////////////////
        private void FixedUpdate()
        {
            if (m_Expiered)
                return;

            if (m_Timer.AddTime(Time.deltaTime))
            {
                m_Expired.Invoke();
                if (m_Loop)
                    m_Timer.Reset();
                else
                    m_Expiered = true;
            }
        }


    }
}