using Core.Effect;
using NaughtyAttributes;
using UnityEngine;

public class ShapeRouteEffectModule : ModuleUpdatable
{
    [Range(-0.5f, 1.5f)]
    public float                m_ShapeRoute;

    [SerializeField]
    private Vector2             m_From;
    [SerializeField]
    private Vector2             m_To;
    private Vector2             m_Initial;

    [SerializeField]
    private ShapeVisualizer     m_Shape;

    //////////////////////////////////////////////////////////////////////////
    public Effect Set(ShapeVisualizer shape, Vector2 from, Vector2 to)
    {
        // set values
        m_Shape = shape;
        m_From = from;
        m_To = to;
        m_Initial = m_Shape.Position;

        // return effect
        return Effect;
    }

    protected override void _Update()
    {
        m_Shape.PositionOffset = Vector2.LerpUnclamped(m_From, m_To, m_ShapeRoute) - m_Initial;
    }

    public override void End()
    {
        // restore offset
        m_Shape.PositionOffset = Vector2.zero;

        base.End();
    }

    [Button]
    private void SetActiveShape()
    {
        m_Shape = (TetrisManager.Instance.BoardVisualizer as BoardVisualizer)?.ActiveShape;
    }
}