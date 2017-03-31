using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace HWTools.Grid
{

	/// <summary>
	/// Represents a direction
	/// </summary>
	public enum Direction
	{
		/// <summary>
		/// East
		/// </summary>
		E,
		/// <summary>
		/// South East
		/// </summary>
		SE,
		/// <summary>
		/// South
		/// </summary>
		S,
		/// <summary>
		/// South West
		/// </summary>
		SW,
		/// <summary>
		/// West
		/// </summary>
		W,
		/// <summary>
		/// North West
		/// </summary>
		NW,
		/// <summary>
		/// North
		/// </summary>
		N,
		/// <summary>
		/// North East
		/// </summary>
		NE
	}

	/// <summary>
	/// Provides extension methods to enhance the Direction enum
	/// </summary>
	public static class DirectionExt
	{
		/// <summary>
		/// The number of unique values in the Direction enum
		/// </summary>
		const int enumSize = 8;

		/// <summary>
		/// Provides a set of Directions associated with a grid's shape
		/// </summary>
		/// <param name="mode">The grid mode to evaluate</param>
		/// <returns>An enumeration of Directions starting clockwise from east</returns>
		public static IEnumerable<Direction> Directions(this Grid2D.Mode mode)
		{
			switch (mode)
			{
				default:
					yield return Direction.E;
					yield return Direction.S;
					yield return Direction.W;
					yield return Direction.N;
					break;
				case Grid2D.Mode.Hexagon:
					foreach (var d in _hexDirs)
					{
						yield return d;
					}
					break;
			}
		}

		/// <summary>
		/// Directions associated with a hexagonal grid
		/// </summary>
		static readonly Direction[] _hexDirs =
		{
			Direction.E,
			Direction.SE,
			Direction.S,
			Direction.W,
			Direction.NW,
			Direction.N
		};

		/// <summary>
		/// Inverts _hexDirs to provide an index given a Direction
		/// </summary>
		static readonly IDictionary<Direction, int> _hexDirKeys =
			0.To(5).ToDictionary(i => _hexDirs[i]);

		/// <summary>
		/// Provides a Direction clockwise from this Direction
		/// </summary>
		/// <param name="d">This Direction</param>
		/// <param name="times">How many steps clockwise</param>
		/// <returns>The Direction clockwise from the starting direction</returns>
		public static Direction CW(this Direction d, int times = 1)
		{
			int i = ((int)d + times).Mod(enumSize);
			return (Direction)i;
		}

		/// <summary>
		/// Provides a Direction clockwise from this direction.
		/// <para/>
		/// Limited to a subset of Directions based on the provided grid Mode.
		/// </summary>
		/// <param name="d">This Direction</param>
		/// <param name="mode">The grid Mode to use</param>
		/// <param name="times">How many steps clockwise</param>
		/// <returns>The Direction clockwise from the starting direction</returns>
		public static Direction CW(this Direction d, Grid2D.Mode mode, int times = 1)
		{
			switch (mode)
			{
				default:
					{
						if ((int)d % 2 == 1)
						{
							d--;
						}

						int i = ((int)d + times * 2).Mod(enumSize);
						return (Direction)i;
					}
				case Grid2D.Mode.Hexagon:
					{
						if (d == Direction.NE || d == Direction.SW)
						{
							d--;
						}
						int i = _hexDirKeys[d];
						i = (i + times).Mod(6);
						return _hexDirs[i];
					}
			}
		}

		/// <summary>
		/// Provides a Direction counterclockwise from this direction.
		/// <para/>
		/// Limited to a subset of Directions based on the provided grid Mode.
		/// </summary>
		/// <param name="d">This Direction</param>
		/// <param name="mode">The grid Mode to use</param>
		/// <param name="times">How many steps counterclockwise</param>
		/// <returns>The Direction counterclockwise from the starting direction</returns>
		public static Direction CCW(this Direction d, Grid2D.Mode mode, int times = 1)
		{
			return CW(d, mode, -times);
		}

		/// <summary>
		/// Provides a Direction counterclockwise from this Direction
		/// </summary>
		/// <param name="d">This Direction</param>
		/// <param name="times">How many steps counterclockwise</param>
		/// <returns>The Direction counterclockwise from the starting direction</returns>
		public static Direction CCW(this Direction d, int times = 1)
		{
			return CW(d, -times);
		}

		/// <summary>
		/// Inverts this Direction
		/// </summary>
		/// <param name="d">The direction to invert</param>
		/// <returns>The opposing direction</returns>
		public static Direction Opposite(this Direction d)
		{
			int i = ((int)d + 4).Mod(enumSize);
			return (Direction)i;
		}

		/// <summary>
		/// Associates a Direction with this Vector2
		/// </summary>
		/// <param name="v2">The Vector2 to evaluate</param>
		/// <returns>The Direction most associated with the given Vector2</returns>
		public static Direction Dir(this Vector2 v2)
		{

			if (v2.x > 0)
			{
				if (v2.y > v2.x / 2)
				{
					if (v2.y > v2.x * 2)
					{
						return Direction.N;
					}
					return Direction.NE;
				}
				if (v2.y < -v2.x / 2)
				{
					if (v2.y < -v2.x * 2)
					{
						return Direction.S;
					}
					return Direction.SE;
				}
				return Direction.E;
			}
			if (v2.y > -v2.x / 2)
			{
				if (v2.y > -v2.x * 2)
				{
					return Direction.N;
				}
				return Direction.NW;
			}
			if (v2.y < v2.x / 2)
			{
				if (v2.y < v2.x * 2)
				{
					return Direction.S;
				}
				return Direction.SW;
			}
			return Direction.W;
		}

		/// <summary>
		/// Gives a Vector2 associated with a Direction.
		/// <para/>
		/// Diagonals are of magnitude √2. (Ex. (1,-1))
		/// </summary>
		/// <param name="d">The Direction to evaluate</param>
		/// <returns>The Vector2 associated with the Direction</returns>
		public static Vector2 Vec2(this Direction d)
		{
			switch (d)
			{
				default:
					return Vector2.right;

				case Direction.SE:
					return new Vector2(1, -1);

				case Direction.S:
					return Vector2.down;

				case Direction.SW:
					return new Vector2(-1, -1);

				case Direction.W:
					return Vector2.left;

				case Direction.NW:
					return new Vector2(1, -1);

				case Direction.N:
					return Vector2.up;

				case Direction.NE:
					return new Vector2(1, 1);
			}
		}

		/// <summary>
		/// Enumerates a set of Directions associated with a grid Mode, excluding the starting Direction.
		/// </summary>
		/// <param name="start">The starting Direction</param>
		/// <param name="mode">The grid Mode</param>
		/// <returns>
		/// An enumeration of Directions clockwise from the starting Direction,
		/// excluding the starting Direction.
		/// </returns>
		public static IEnumerable<Direction> Sweep(this Direction start, Grid2D.Mode mode)
		{
			Direction result = start;
			while (true)
			{
				result = result.CW(mode);
				if (result == start)
				{
					break;
				}
				yield return result;
			}
		}
	}
}