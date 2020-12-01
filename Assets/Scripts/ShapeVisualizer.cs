using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using Tetris.Engine;
using UnityEngine;
using Core;

public class ShapeVisualizer : MonoBehaviour
{
    private List<BlockVisualizer>           m_Blocks;
    public List<BlockVisualizer>            Blocks => m_Blocks;

    public List<Vector2Int>                 Coords => Blocks.Select(n => n.Position).ToList();
    private bool                            m_Locked;
    public bool                             IsActiveShape => m_Locked == false;

    [SerializeField]
    private TrailRenderer                   m_TrailPrefab;

    public Vector2Int                       Position
    {
        get => m_Position;
        set
        {

            if (m_Position == value)
                return;

            // move elements
            var dif = value - m_Position;
            foreach (var element in m_Blocks)
                element.Position += dif;

            // save position
            m_Position = value;
        }
    }

    //public Vector2 PositionWorld => new Vector2(m_Position.x + 0.5f, m_Position.y + 0.5f);

    private Vector2                         m_PositionOffset;
    public Vector2                          PositionOffset
    {
        get => m_PositionOffset;
        set
        {
            m_PositionOffset = value;
            // set blocks offset
            foreach (var element in m_Blocks)
                element.PositionOffset = m_PositionOffset;
        }
    }

    public bool                             Trail
    {
        set
        {
            if (value)
            {
                // instantiate trails
                foreach (var block in m_Blocks.ToLookup(n => n.Position.x, n => n))
                {
                    var topBlock = block.MaxBy(n => n.Position.y);
                    var trail = Instantiate(m_TrailPrefab, topBlock.View.transform, false);

                    // set start & end color to block color with alpha from trail
                    var blockColor = topBlock.View.GetComponent<SpriteRenderer>().color;
                    trail.startColor =  new Color(blockColor.r, blockColor.g, blockColor.b, trail.startColor.a);
                    trail.endColor = new Color(blockColor.r, blockColor.g, blockColor.b, trail.endColor.a);
                }
            }
            else
            {
                // enable trail auto destruct
                foreach (var block in m_Blocks)
                    block.GetComponentInChildren<TrailRenderer>().autodestruct = true;
            }
        }
    }

    private Vector2Int                      m_Position;

    //////////////////////////////////////////////////////////////////////////
    public void Init(Block block)
    {
        // save position
        m_Position = block.Position.ToVector2Int();

        // collect elements
        m_Blocks = GetComponentsInChildren<BlockVisualizer>().ToList();
        
        // unroot all elements & move to the block position
        foreach (var element in m_Blocks)
            element.Init(this, new Vector2Int(block.Position.Column, block.Position.Row));
    }

    public void OnLock(Block block)
    {
        m_Locked = true;
    }

    public void OnMove(Block block, Move move)
    {
        // don't move if locked
        if (m_Locked)
            return;

        // set position
        Position = block.Position.ToVector2Int();

        switch (move)
        {
            case Move.Down:
            case Move.Left:
            case Move.Right:
                break;

            case Move.Fall:
                break;

            case Move.RotateRight:
            case Move.RotateLeft:
            {
                // get closest anchor positions
                var blockCoords = block.GetCoords().Select(n => new Vector2Int(n.Column, n.Row));
                var distanceList = blockCoords
                    .Select(vec => m_Blocks.ToDictionary(c => c, c => (vec, Vector2Int.Distance(vec, c.Position))))
                    .ToList();
                
                // move closest anchor to block pos, remove anchor from other lists
                while (distanceList.IsEmpty() == false)
                {
                    var chain = distanceList.MinBy(n => n.Min(c => c.Value.Item2));
                    var closest = chain.MinBy(n => n.Value.Item2);
                    
                    // move anchor
                    closest.Key.Position = closest.Value.vec;

                    // remove visualization element
                    distanceList.Remove(chain);
                    // remove vector from other lists
                    foreach (var n in distanceList)
                        n.Remove(closest.Key);
                }

            }   break;
            
            case Move.None:
            default:
                break;
        }
    }
}
