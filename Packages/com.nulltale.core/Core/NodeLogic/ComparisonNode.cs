using System;
using System.Collections;
using UnityEngine;
using XNode;

namespace Core.NodeLogic
{
    [CreateNodeMenu("NodeLogic/Comparison"), NodeTint(LogicGraph.c_ConditionColor)]
    public class ComparisonNode : InputNode
    {
        [Output(backingValue = ShowBackingValue.Never, connectionType = ConnectionType.Override)]
        public LogicNode            m_True;
        [Output(backingValue = ShowBackingValue.Never, connectionType = ConnectionType.Override)]
        public LogicNode            m_False;

        
        [Input(backingValue = ShowBackingValue.Never, connectionType = ConnectionType.Override)]
        public ValueNode            m_A;
        [Input(backingValue = ShowBackingValue.Never, connectionType = ConnectionType.Override)]
        public ValueNode            m_B;

        public Core.ComparisonOperation         m_Condition = Core.ComparisonOperation.Equal;

        //////////////////////////////////////////////////////////////////////////
        public override IEnumerator Activate()
        {
            NodePort next = null;
            try
            {
                // get values
                var a = getValue(nameof(m_A)) as IComparable;
                var b = getValue(nameof(m_B)) as IComparable;

                // try to compare
                if (a != null && b != null)
                    next = m_Condition.Check(a, b) ? GetOutputPort(nameof(m_True)) : GetOutputPort(nameof(m_False));

            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }

            // activate next
            if (next != null && next.IsConnected 
                             && next.Connection?.node is IActivationNode logicNode)
                yield return logicNode.Activate();

            /////////////////////////////////////
            object getValue(string portName)
            {
                var port = GetInputPort(portName);
                if (port == null || port.IsConnected == false)
                    return null;

                var firstConnection = port.GetConnection(0);
                if (firstConnection.node is IValueNode valueNode)
                    return valueNode.GetValue();

                return null;
            }
        }
    }
}