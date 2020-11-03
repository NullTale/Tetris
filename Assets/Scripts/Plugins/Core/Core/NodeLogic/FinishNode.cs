namespace Core.NodeLogic
{
    [CreateNodeMenu("NodeLogic/Finish"), NodeTint(LogicGraph.c_FinishColor)]
    public class FinishNode : ActivationNode, IFinishNode
    {
    }
}