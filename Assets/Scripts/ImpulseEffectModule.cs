using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Effect;
using NaughtyAttributes;
using UnityEngine;

public class ImpulseEffectModule : ModuleBase
{
    [SerializeField]
    [CurveRange(0, 0, 1, 1, EColor.White)]
    private AnimationCurve       m_Curve;
    [SerializeField]
    private float                m_Force;
    [SerializeField]
    private float                m_Radius;

    [SerializeField]
    private ShapeVisualizer     m_Shape;

    //////////////////////////////////////////////////////////////////////////
    public Effect Set(ShapeVisualizer shape)
    {
        m_Shape = shape;

        return Effect;
    }

    [Button]
    public void Impulse()
    {
        if (TetrisManager.Instance.BoardVisualizer is BoardVisualizer boardVisualizer)
            _Impulse(boardVisualizer.GetBlocks(n => m_Shape.Blocks.Contains(n) == false).ToList(),
                m_Shape.Blocks.Select(n => n.View.transform.localPosition.To2DXY()).ToList());
    }

    private void _Impulse(List<BlockVisualizer> blocks, List<Vector2> source)
    {
        // move blocks of view by impulse force value (normal * (curve(dist) * force))
        foreach (var blockVisualizer in blocks)
        {
            var blockPos = blockVisualizer.View.transform.localPosition.To2DXY();
            var vec = source.Select(n => blockPos - n).MinBy(n => n.sqrMagnitude);
            var dist = vec.magnitude;
            if (dist > m_Radius)
                continue;

            var force = m_Curve.Evaluate(dist / m_Radius) * m_Force;
            blockVisualizer.View.transform.localPosition += (vec.normalized * force).To3DXY();
        }
    }

    [Button]
    private void SetRandomShape()
    {
        m_Shape = (TetrisManager.Instance.BoardVisualizer as BoardVisualizer)?.ShapeList.RandomItem();
    }
}