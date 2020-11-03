using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.EventSystem;
using Tetris.Engine;
using UnityEngine;
using UnityEngine.UI;

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

    //////////////////////////////////////////////////////////////////////////
    public override void ProcessMessage(IMessage<BoardEvent> e)
    {
        switch (e.Key)
        {
            case BoardEvent.BlockSpawned:
            {
                //var block = e.GetData<Block>();

                if (TetrisManager.Instance.BoardVisualizer is BoardVisualizer bv 
                    && bv.ShapesData.TryGetValue(TetrisManager.Instance.BlockProvider.NextBlock, out var shapeData))
                {
                    m_Preview.sprite = shapeData.m_BlockPreview;
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
