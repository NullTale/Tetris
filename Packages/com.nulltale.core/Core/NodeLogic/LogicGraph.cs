using System;
using System.Collections;
using System.Linq;
using XNode;

namespace Core.NodeLogic
{
    [Serializable]
    public abstract class LogicGraph : NodeGraph 
    {
        public const string         c_StateColor = "#3a8ac9";
        public const string         c_ActionColor = "#ea444e";
        public const string         c_ConditionColor = "#915f92";
        public const string         c_ValueColor = "#857e6e";
        public const string         c_StartColor = "#77a705";
        public const string         c_FinishColor = "#1a3459";
        public const string         c_EventColor = "#f2efde";
        
        //////////////////////////////////////////////////////////////////////////
        public IEnumerator Start()
        {
            // activate start node
            yield return ActivateNode<IStartNode>();
        }

        public IEnumerator Finish()
        {
            // activate finish node
            yield return ActivateNode<IFinishNode>();
        }

        public IEnumerator ActivateNode<T>(Func<T, bool> checkFunc) where T : IActivationNode
        {
            var pretenders = nodes.OfType<T>();
            // activate first suitable, or first
            var node = checkFunc != null
                ? pretenders.FirstOrDefault(checkFunc)
                : pretenders.FirstOrDefault();

            yield return node != null ? node.Activate() : null;
        }

        public IEnumerator ActivateNode<T>() where T : IActivationNode
        {
            var pretenders = nodes.OfType<T>();
            // activate first suitable, or first
            var node = pretenders.FirstOrDefault();
            yield return node != null ? node.Activate() : null;
        }
    }
    
    public interface IStartNode : IActivationNode
    {
    }

    public interface IFinishNode : IActivationNode
    {
    }

    public interface IActivationNode
    {
        IEnumerator Activate();
    }

    public interface IValueNode
    {
        object GetValue();
    }
    
    [Serializable]
    public abstract class LogicNode : Node
    {
        public override object GetValue(NodePort port)
        {
            return null;
        }

        public T GetPortValue<T>(string portName, T defaultValue = default)
        {
            var port = GetInputPort(portName);
            if (port == null || port.IsConnected == false)
                return defaultValue;

            var firstConnection = port.GetConnection(0);
            if (firstConnection.node is IValueNode valueNode)
                return valueNode.GetValue() is T value ? value : defaultValue;

            return defaultValue;
        }

        public IEnumerator ActivateNode(string nodeName)
        {
            // activate next node
            var next = GetOutputPort(nodeName);
            if (next != null && next.IsConnected 
                             && next.Connection?.node is IActivationNode logicNode)
                yield return logicNode.Activate();
        }
    }


    [CreateNodeMenu("Action"),  NodeTint(LogicGraph.c_ActionColor)]
    public abstract class ActionNode : InputNode
    {
        [Output(backingValue = ShowBackingValue.Never, connectionType = ConnectionType.Override)]
        public LogicNode         m_Next;

        //////////////////////////////////////////////////////////////////////////
        public override IEnumerator Activate()
        {
            // run action
            Run();
            
            // activate next node
            yield return ActivateNext();
        }

        public IEnumerator ActivateNext()
        {
            // activate next node
            yield return ActivateNode(nameof(m_Next));
        }

        public abstract void Run();
    }


    [CreateNodeMenu("Value"), NodeTint(LogicGraph.c_ValueColor)]
    public abstract class ValueNode : LogicNode, IValueNode
    {
        [Output(backingValue = ShowBackingValue.Never, connectionType = ConnectionType.Multiple)]
        public LogicNode   m_Output;

        //////////////////////////////////////////////////////////////////////////
        public override object GetValue(NodePort port)
        {
            return null;
        }

        public abstract object GetValue();
    }
    
    [CreateNodeMenu(""), NodeTint(LogicGraph.c_StartColor)]
    public class ActivationNode : LogicNode, IActivationNode
    {
        [Output(backingValue = ShowBackingValue.Never, connectionType = ConnectionType.Override)]
        public LogicNode   m_Next;

        //////////////////////////////////////////////////////////////////////////
        public IEnumerator Activate()
        {
            // activate next node
            yield return ActivateNode(nameof(m_Next));
        }
    }
    
    [CreateNodeMenu(""), NodeTint(LogicGraph.c_ActionColor)]
    public abstract class InputNode : LogicNode, IActivationNode
    {
        [Input(backingValue = ShowBackingValue.Never, connectionType = ConnectionType.Multiple)]
        public LogicNode        m_Activate;
        
        //////////////////////////////////////////////////////////////////////////
        public abstract IEnumerator Activate();
    }
}