using UnityEngine;

namespace Core.NodeLogic
{
    [CreateNodeMenu("NodeLogic/Log"),  NodeTint(LogicGraph.c_ActionColor)]
    public class LogNode : ActionNode
    {
        [SerializeField]
        private string      m_Log;

        //////////////////////////////////////////////////////////////////////////
        public override void Run()
        {
            Debug.Log(m_Log);
        }
    }
}