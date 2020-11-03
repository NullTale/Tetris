using System.Collections;
using System.Linq;

namespace Core.NodeLogic
{
    [CreateNodeMenu("NodeLogic/Sequence"), NodeTint(LogicGraph.c_ActionColor)]
    public class SequenceNode : InputNode
    {
        [Output(backingValue = ShowBackingValue.Never, connectionType = ConnectionType.Override, dynamicPortList = true)]
        public LogicNode        m_Next;

        //////////////////////////////////////////////////////////////////////////
        public override IEnumerator Activate()
        {
            // activate nodes
            foreach (var port in DynamicOutputs
                .Where(n => n.fieldName.StartsWith(nameof(m_Next)))
                .OrderBy(n => int.Parse(n.fieldName.Substring(nameof(m_Next).Length)))) // order by index for assurance
            {
                if (port.IsConnected && port.Connection?.node is IActivationNode logicNode)
                    yield return logicNode.Activate();
            }
        }
    }
}