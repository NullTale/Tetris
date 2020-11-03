using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using NaughtyAttributes;
using Tetris.Engine;
using UnityEngine;

[Serializable]
public class ShapePositionVisualizer : MonoBehaviour
{
    public List<SpriteRenderer>         m_Sprites;
    public int                          m_MinHeight = 2;

    //////////////////////////////////////////////////////////////////////////
    public void Init(BoardVisualizer visualizer)
    {
        // move sprites to playfield root
        foreach (var sprite in m_Sprites)
            sprite.transform.SetParent(visualizer.PlayfieldRoot.transform);

    }

    public void Set(List<Vector2Int> shape)
    {
        // disable all if block is empty
        if (shape == null)
        {
            foreach (var sprite in m_Sprites)
                sprite.enabled = false;
            return;
        }

        // collision height
        var collisionHeight = TetrisManager.GetCollisionHeight(shape);
        if (collisionHeight <= m_MinHeight)
        {
            foreach (var sprite in m_Sprites)
                sprite.enabled = false;
            return;
        }

        // get active sprites
        var activeSprites = m_Sprites.Take(shape.Count).ToList();

        // disable un active sprites
        foreach (var sprite in m_Sprites.Skip(activeSprites.Count()))
            sprite.enabled = false;

        // set active sprites positions
        var shapePosition =  shape.Select(n => new Vector2Int(n.x, n.y - collisionHeight)).ToList();
        for (var n = 0; n < shapePosition.Count; n++)
        {
            activeSprites[n].transform.localPosition = new Vector3(shapePosition[n].x + 0.5f, shapePosition[n].y + 0.5f, 0.5f);
            activeSprites[n].enabled = true;
        }
    }
}