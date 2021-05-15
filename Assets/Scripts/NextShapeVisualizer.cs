using System;
using System.Linq;
using Core;
using Core.EventSystem;
using UnityEngine;

[DefaultExecutionOrder(10)]
public class NextShapeVisualizer : MessageListener<BoardEvent>
{
    [SerializeField]
    private SpriteRenderer      m_Preview;

    [SerializeField]
    private bool                m_Dynamic;
    [SerializeField]
    [DrawIf(nameof(m_Dynamic), true)]
    private float               m_MoveDuration;
    [SerializeField]
    [DrawIf(nameof(m_Dynamic), true)]
    private LeanTweenType       m_Ease;
    [SerializeField]
    [DrawIf(nameof(m_Dynamic), true)]
    private GameObject          m_Root;

    public Sprite   m_HatePreview;
    public Sprite   m_LovePreview;

    //////////////////////////////////////////////////////////////////////////
    public override void ProcessMessage(IMessage<BoardEvent> e)
    {
        switch (e.Key)
        {
            case BoardEvent.BlockSpawned:
            {
                if (TetrisManager.Instance.BoardVisualizer is BoardVisualizer bv)
                {
                    switch (TetrisManager.Instance.BlockProvider.Mode)
                    {
                        case BlockProvider.RandomMode.Random:
                            if (bv.ShapesData.TryGetValue(TetrisManager.Instance.BlockProvider.NextBlock, out var shapeData))
                                m_Preview.sprite = shapeData.m_BlockPreview;
                            break;
                        case BlockProvider.RandomMode.Hate:
                            m_Preview.sprite = m_HatePreview;
                            break;
                        case BlockProvider.RandomMode.Love:
                            m_Preview.sprite = m_LovePreview;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            } break;
            case BoardEvent.CollapseRows:
            case BoardEvent.LockBlock:
                _UpdatePosition();
                break;
        }
    }

    //////////////////////////////////////////////////////////////////////////
    private void _UpdatePosition()
    {
        if (m_Dynamic == false)
            return;

        var blocks = TetrisManager.Instance.GameManager.BoardManager.GetBoardBlocks().ToList();
        if (blocks.IsEmpty())
            return;

        var averageY = (float)blocks.Average(n => n.Row);
        LeanTween.cancel(m_Root);
        LeanTween.moveLocalY(m_Root, averageY, m_MoveDuration).setEase(m_Ease);
    }
}
