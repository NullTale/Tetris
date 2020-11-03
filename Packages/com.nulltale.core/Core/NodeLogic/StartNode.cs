namespace Core.NodeLogic
{
    [CreateNodeMenu("NodeLogic/Start"), NodeTint(LogicGraph.c_StartColor)]
    public class StartNode : ActivationNode, IStartNode
    {
    }
}