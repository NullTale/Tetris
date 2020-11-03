using UltEvents;
using UnityEngine;

namespace Core.Effect
{
    public class ActionModule : ModuleBase
    {
        [SerializeField]
        private UltEvent            m_OnBegin;
        public UltEvent             OnBegin => m_OnBegin;
        
        [SerializeField]
        private UltEvent            m_OnEnd;
        public UltEvent             OnEnd => m_OnEnd;

        //////////////////////////////////////////////////////////////////////////
        public override void Begin()
        {
            m_OnBegin.Invoke();
        }

        public override void End()
        {
            m_OnEnd.Invoke();
        }
    }
}