using System;
using System.Collections.Generic;
using Core;
using UnityEngine;

namespace Core
{
    [Flags]
    public enum Direction : int
    {
        None		= 0,

        Left		= 1,
        Right		= 1 << 16,		// 15 + n

        Top			= 1 << 1,
        Bottom		= 1 << 17,

        Up		= 1 << 2,
        Down	= 1 << 18,

        LeftTop		= 1 << 3,
        RightBottom	= 1 << 19,

        RightTop		= 1 << 4,
        LeftBottom		= 1 << 20,


        UpLeft			= 1 << 5,
        DownRight		= 1 << 21,

        UpRight			= 1 << 6,
        DownLeft		= 1 << 22,

        UpTop			= 1 << 7,
        DownBottom		= 1 << 23,

        UpBottom		= 1 << 8,
        DownTop			= 1 << 24,

        UpLeftTop		= 1 << 9,
        DownRightBottom	= 1 << 25,

        UpRightTop		= 1 << 10,
        DownLeftBottom	= 1 << 26,

        UpRightBottom	= 1 << 11,
        DownLeftTop		= 1 << 27,

        UpLeftBottom	= 1 << 12,
        DownRightTop	= 1 << 28,

        Center			= 1 << 13 | 1 << 29,
    }
}

public static class DirectionHelper
{
	public static Direction Invert(this Direction direction)
	{
		return (Direction) (((int)direction >> 16) | ((int)direction << 16));
	}

	public static Vector3Int Offset(this Direction direction)
	{
		return g_TilemapOffset[direction];
	}

	/*public static float AngleDegree2D(this Direction direction)
	{
		return g_RotationDegrees[direction];
	}*/

	public static Direction DetermineDirection4Dim(float angleDeg)
	{
		foreach (var n in g_DirectionRange4Dim)
			if (n.InRange(angleDeg))
				return n.m_Direction;

		return Direction.None;
	}

	public static Direction DetermineDirection8Dim(float angleDeg)
	{
		foreach (var n in g_DirectionRange8Dim)
			if (n.InRange(angleDeg))
				return n.m_Direction;

		return Direction.None;
	}

	public static Direction DetermineDirection4Dim(Vector2Int vec)
	{
		var result = Direction.None;

		if (vec.x > 0) result |= Direction.Right;
		else if (vec.x < 0) result |= Direction.Left;

		if (vec.y > 0) result |= Direction.Top;
		else if (vec.y < 0) result |= Direction.Bottom;

		return result;
	}

	public static readonly Vector2 c_Vec2Center = new Vector2(0.5f, 0.5f);

	public static readonly Vector2Int c_Vec2IntLeft = new Vector2Int(-1, 0);
	public static readonly Vector2Int c_Vec2IntRight = new Vector2Int(1, 0);
	public static readonly Vector2Int c_Vec2IntTop = new Vector2Int(0, 1);
	public static readonly Vector2Int c_Vec2IntBottom = new Vector2Int(0, -1);

	public static readonly Vector2Int c_Vec2IntLeftTop = new Vector2Int(-1, 1);
	public static readonly Vector2Int c_Vec2IntLeftBottom = new Vector2Int(-1, -1);
	public static readonly Vector2Int c_Vec2IntRightTop = new Vector2Int(1, 1);
	public static readonly Vector2Int c_Vec2IntRightBottom = new Vector2Int(1, -1);


	public static int DirectionCount4Dim(this Direction direction)
	{
		var result = 0;
		foreach (var n in g_DirectionPure4Dim)
			if (direction.HasFlag(n))
				result++;

		return result;
	}

	/*public static bool IsHorizontal(Direction direction)
	{
		return Direction.Horizontal.HasFlag(direction);
	}

	public static bool IsVertical(Direction direction)
	{
		return Direction.Vertical.HasFlag(direction);
	}*/
	
	public static readonly Dictionary<Direction, Vector3Int> g_TilemapOffset = new Dictionary<Direction, Vector3Int>()
	{
		{ Direction.Left,		new Vector3Int(-1, 0, 0) },
		{ Direction.Right,		new Vector3Int(1, 0, 0) }, 
		{ Direction.Top,		new Vector3Int(0, 1, 0) }, 
		{ Direction.Bottom,		new Vector3Int(0, -1, 0) },

		{ Direction.LeftTop,	new Vector3Int(-1, 1, 0) },
		{ Direction.RightTop,	new Vector3Int(1, 1, 0) }, 
		{ Direction.RightBottom,new Vector3Int(1, -1, 0) },
		{ Direction.LeftBottom,	new Vector3Int(-1, -1, 0) },
		
		// up
		{ Direction.UpLeft,		new Vector3Int(-1, 0, 1) },
		{ Direction.UpRight,	new Vector3Int(1, 0, 1) }, 
		{ Direction.UpTop,		new Vector3Int(0, 1, 1) }, 
		{ Direction.UpBottom,	new Vector3Int(0, -1, 1) },

		{ Direction.UpLeftTop,		new Vector3Int(-1, 1, 1) },
		{ Direction.UpRightTop,		new Vector3Int(1, 1, 1) }, 
		{ Direction.UpRightBottom,	new Vector3Int(1, -1,1) },
		{ Direction.UpLeftBottom,	new Vector3Int(-1, -1, 1) },
		
		// down
		{ Direction.DownLeft,		new Vector3Int(-1, 0, -1) },
		{ Direction.DownRight,		new Vector3Int(1, 0, -1) }, 
		{ Direction.DownTop,		new Vector3Int(0, 1, -1) }, 
		{ Direction.DownBottom,		new Vector3Int(0, -1, -1) },

		{ Direction.DownLeftTop,	new Vector3Int(-1, 1, -1) },
		{ Direction.DownRightTop,	new Vector3Int(1, 1, -1) }, 
		{ Direction.DownRightBottom,new Vector3Int(1, -1, -1) },
		{ Direction.DownLeftBottom,	new Vector3Int(-1, -1, -1) },

		// center
		{ Direction.Center,		new Vector3Int(0, 0, 0) }
		
	};

	public static readonly Dictionary<int, Direction> g_AngleToDirection = new Dictionary<int, Direction>()
	{
		{180, Direction.Left},
		{0, Direction.Right},
		{90, Direction.Top},
		{270, Direction.Bottom},

		{135, Direction.LeftTop},
		{45, Direction.RightTop},
		{315, Direction.RightBottom},
		{225, Direction.LeftBottom},


		{-180, Direction.Left},
		{-270, Direction.Top},
		{-90, Direction.Bottom},

		{-225, Direction.LeftTop},
		{-315, Direction.RightTop},
		{-45, Direction.RightBottom},
		{-135, Direction.LeftBottom}
	};

	public static readonly SortedDictionary<Direction, Vector3> g_DirectionVector =
		new SortedDictionary<Direction, Vector3>()
		{
			{Direction.None, new Vector3(0.0f, 0.0f, 0.0f)},

			{Direction.Left, new Vector3(-1.0f, 0.0f, 0.0f)},
			{Direction.Right, new Vector3(1.0f, 0.0f, 0.0f)},
			{Direction.Top, new Vector3(0.0f, 1.0f, 0.0f)},
			{Direction.Bottom, new Vector3(0.0f, -1.0f, 0.0f)},

			{Direction.Up, new Vector3(0.0f, 1.0f, 0.0f)},
			{Direction.Down, new Vector3(0.0f, -1.0f, 0.0f)},

			{Direction.LeftTop, new Vector3(-0.707106769084930419921875f, 0.707106769084930419921875f, 0.0f)},
			{Direction.RightTop, new Vector3(0.707106769084930419921875f, 0.707106769084930419921875f, 0.0f)},
			{Direction.RightBottom, new Vector3(0.707106769084930419921875f, -0.707106769084930419921875f, 0.0f)},
			{Direction.LeftBottom, new Vector3(-0.707106769084930419921875f, -0.707106769084930419921875f, 0.0f)}
		};

	public static readonly SortedDictionary<Direction, float> g_RotationDegrees =
		new SortedDictionary<Direction, float>()
		{
			{Direction.None, 0.0f},

			{Direction.Left, 180.0f},
			{Direction.Right, 0.0f},
			{Direction.Top, 90.0f},
			{Direction.Bottom, -90.0f},

			{Direction.LeftTop, 135.0f},
			{Direction.RightTop, 45.0f},
			{Direction.RightBottom, -45.0f},
			{Direction.LeftBottom, -135.0f}
		};

	public static readonly SortedDictionary<Direction, Direction> g_LeftRotation =
		new SortedDictionary<Direction, Direction>()
		{
			{Direction.None, Direction.None},

			{Direction.Left, Direction.Right},
			{Direction.Right, Direction.Left},
			{Direction.Top, Direction.Bottom},
			{Direction.Bottom, Direction.Top},

			{Direction.Up, Direction.Up},
			{Direction.Down, Direction.Down},

			{Direction.LeftTop, Direction.RightBottom},
			{Direction.RightTop, Direction.LeftBottom},
			{Direction.RightBottom, Direction.LeftTop},
			{Direction.LeftBottom, Direction.RightTop}
		};

	public static readonly SortedDictionary<Direction, Direction> g_RightRotation =
		new SortedDictionary<Direction, Direction>()
		{
			{Direction.None, Direction.None},

			{Direction.Left, Direction.Left},
			{Direction.Right, Direction.Right},
			{Direction.Top, Direction.Top},
			{Direction.Bottom, Direction.Bottom},

			{Direction.Up, Direction.Up},
			{Direction.Down, Direction.Down},

			{Direction.LeftTop, Direction.LeftTop},
			{Direction.RightTop, Direction.RightTop},
			{Direction.RightBottom, Direction.RightBottom},
			{Direction.LeftBottom, Direction.LeftBottom}
		};

	public static readonly SortedDictionary<Direction, Direction> g_TopRotation =
		new SortedDictionary<Direction, Direction>()
		{
			{Direction.None, Direction.None},

			{Direction.Left, Direction.Bottom},
			{Direction.Right, Direction.Top},
			{Direction.Top, Direction.Left},
			{Direction.Bottom, Direction.Right},

			{Direction.Up, Direction.Up},
			{Direction.Down, Direction.Down},

			{Direction.LeftTop, Direction.LeftBottom},
			{Direction.RightTop, Direction.LeftTop},
			{Direction.RightBottom, Direction.RightTop},
			{Direction.LeftBottom, Direction.RightBottom}
		};

	public static readonly SortedDictionary<Direction, Direction> g_BottomRotation =
		new SortedDictionary<Direction, Direction>()
		{
			{Direction.None, Direction.None},

			{Direction.Left, Direction.Top},
			{Direction.Right, Direction.Bottom},
			{Direction.Top, Direction.Right},
			{Direction.Bottom, Direction.Left},

			{Direction.Up, Direction.Up},
			{Direction.Down, Direction.Down},

			{Direction.LeftTop, Direction.RightTop},
			{Direction.RightTop, Direction.RightBottom},
			{Direction.RightBottom, Direction.LeftBottom},
			{Direction.LeftBottom, Direction.LeftTop}
		};

	public static readonly SortedDictionary<Direction, SortedDictionary<Direction, Direction>> g_Rotations =
		new SortedDictionary<Direction, SortedDictionary<Direction, Direction>>()
		{
			{Direction.None, g_RightRotation},

			{Direction.Left, g_LeftRotation},
			{Direction.Right, g_RightRotation},
			{Direction.Top, g_TopRotation},
			{Direction.Bottom, g_BottomRotation}
		};

	public static readonly SortedDictionary<Direction, Direction> g_RotationClockwise =
		new SortedDictionary<Direction, Direction>()
		{
			{Direction.Bottom, Direction.Left},
			{Direction.Left, Direction.Top},
			{Direction.Top, Direction.Right},
			{Direction.Right, Direction.Bottom},
		};

	public static readonly SortedDictionary<Direction, Direction> g_RotationCounterClockwise =
		new SortedDictionary<Direction, Direction>()
		{
			{Direction.Bottom, Direction.Right},
			{Direction.Right, Direction.Top},
			{Direction.Top, Direction.Left},
			{Direction.Left, Direction.Bottom},
		};

	public static readonly SortedDictionary<Direction, Vector3Int> g_WallMapDirectonOffset =
		new SortedDictionary<Direction, Vector3Int>()
		{
			{Direction.Left, new Vector3Int(0, 0, 0)},
			{Direction.Right, new Vector3Int(1, 0, 0)},
			{Direction.Bottom, new Vector3Int(0, 0, 0)},
			{Direction.Top, new Vector3Int(0, 1, 0)},
		};

	public struct AngleDirection
	{
		public float m_Max;
		public float m_Min;
		public Direction m_Direction;

		public bool InRange(float angle)
		{
			return angle >= m_Min && angle < m_Max;
		}
	}

	public static readonly List<Direction> g_DirectionPure4Dim = new List<Direction>()
	{
		Direction.Left,
		Direction.Right,
		Direction.Top,
		Direction.Bottom
	};

	public static readonly List<AngleDirection> g_DirectionRange4Dim = new List<AngleDirection>()
	{
		new AngleDirection() {m_Min = 45.0f, m_Max = 135.0f, m_Direction = Direction.Top},
		new AngleDirection() {m_Min = -135.0f, m_Max = -45.0f, m_Direction = Direction.Bottom},

		new AngleDirection() {m_Min = 0.0f, m_Max = 45.0f, m_Direction = Direction.Right},
		new AngleDirection() {m_Min = -45.0f, m_Max = 0.0f, m_Direction = Direction.Right},

		new AngleDirection() {m_Min = 135.0f, m_Max = 180.0f, m_Direction = Direction.Left},
		new AngleDirection() {m_Min = -180.0f, m_Max = -135.0f, m_Direction = Direction.Left},
	};

	public static readonly List<AngleDirection> g_DirectionRange8Dim = new List<AngleDirection>()
	{
		new AngleDirection() {m_Min = 67.5f, m_Max = 112.5f, m_Direction = Direction.Top},
		new AngleDirection() {m_Min = -112.5f, m_Max = -67.5f, m_Direction = Direction.Bottom},

		new AngleDirection() {m_Min = 22.5f, m_Max = 67.5f, m_Direction = Direction.RightTop},
		new AngleDirection() {m_Min = -67.5f, m_Max = -22.5f, m_Direction = Direction.RightBottom},

		new AngleDirection() {m_Min = 112.5f, m_Max = 157.5f, m_Direction = Direction.LeftTop},
		new AngleDirection() {m_Min = -157.5f, m_Max = -112.5f, m_Direction = Direction.LeftBottom},

		new AngleDirection() {m_Min = 0.0f, m_Max = 22.5f, m_Direction = Direction.Right},
		new AngleDirection() {m_Min = -22.5f, m_Max = 0.0f, m_Direction = Direction.Right},

		new AngleDirection() {m_Min = 157.5f, m_Max = 180.0f, m_Direction = Direction.Left},
		new AngleDirection() {m_Min = -180.0f, m_Max = -157.5f, m_Direction = Direction.Left}
	};


	public static readonly Dictionary<SpriteAlignment, Vector2> g_SpriteAlignmentValues =
		new Dictionary<SpriteAlignment, Vector2>
		{
			{SpriteAlignment.BottomCenter, new Vector2(0.5f, 0.0f)},
			{SpriteAlignment.BottomLeft, new Vector2(0.0f, 0.0f)},
			{SpriteAlignment.BottomRight, new Vector2(1.0f, 0.0f)},

			{SpriteAlignment.Center, new Vector2(0.50f, 0.5f)},
			{SpriteAlignment.LeftCenter, new Vector2(0.0f, 0.5f)},
			{SpriteAlignment.RightCenter, new Vector2(1.0f, 0.5f)},

			{SpriteAlignment.TopCenter, new Vector2(0.5f, 1.0f)},
			{SpriteAlignment.TopLeft, new Vector2(0.0f, 1.0f)},
			{SpriteAlignment.TopRight, new Vector2(1.0f, 1.0f)},
		};
}