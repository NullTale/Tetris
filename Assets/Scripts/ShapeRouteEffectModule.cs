using Core.Effect;
using NaughtyAttributes;
using UnityEngine;

public class ShapeRouteEffectModule : ModuleUpdatable
{
    [Range(-0.5f, 1.5f)]
    public float                m_ShapeRoute;

    private Vector2             m_Distance;
    private Vector2             m_Influence;

    [SerializeField]
    private ShapeVisualizer     m_Shape;

    //////////////////////////////////////////////////////////////////////////
    public Effect Set(ShapeVisualizer shape, Vector2 from, Vector2 to)
    {
        // set values
        m_Shape = shape;
        m_Distance = to - from;
        
        shape.PositionOffset -= m_Distance;

        // return effect
        return Effect;
    }

    protected override void _Update()
    {
        var influenceLast = m_Influence;
        m_Influence = m_Distance * m_ShapeRoute;
        var offset = m_Influence - influenceLast;
        m_Shape.PositionOffset += offset;
    }

    public override void End()
    {
        // restore offset
        m_Shape.PositionOffset += m_Distance - m_Influence;

        base.End();
    }

    [Button]
    private void SetActiveShape()
    {
        m_Shape = (TetrisManager.Instance.BoardVisualizer as BoardVisualizer)?.ActiveShape;
    }
}