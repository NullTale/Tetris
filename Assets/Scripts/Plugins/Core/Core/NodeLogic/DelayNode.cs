using System;
using System.Collections;
using UnityEngine;

namespace Core.NodeLogic
{
    [CreateNodeMenu("NodeLogic/Delay"),  NodeTint(LogicGraph.c_ActionColor)]
    public class DelayNode : ActionNode
    {
        [SerializeField]
        private float      m_Delay;

        //////////////////////////////////////////////////////////////////////////
        public override IEnumerator Activate()
        {
            // wait delay
            yield return new WaitForSeconds(m_Delay);
            
            // activate next node
            yield return ActivateNext();
        }

        public override void Run()
        {
            throw new NotImplementedException();
        }
    }
}