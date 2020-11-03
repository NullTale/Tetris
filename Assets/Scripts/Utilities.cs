using Tetris.Engine;
using UnityEngine;

public static class Utilities
{
    public static Vector2Int ToVector2Int(this Position pos)
    {
        return new Vector2Int(pos.Column, pos.Row);
    }
}