using System;
using System.Collections.Generic;
using UnityEngine;

namespace HWTools
{
	/// <summary>
	/// Provides extension methods for Vector2s
	/// </summary>
	public static class Vector2Ext
	{
		#region Public Methods

		/// <summary>
		/// Adds int values to this Vector2
		/// </summary>
		/// <param name="v">This Vector2</param>
		/// <param name="x">X distance</param>
		/// <param name="y">Y distance</param>
		/// <returns>The modified Vector2</returns>
		public static Vector2 V2Add(this Vector2 v, int x, int y)
		{
			return v + new Vector2(x, y);
		}

		/// <summary>
		/// Performs modulus operation on this Vector2
		/// </summary>
		/// <param name="v">This Vector2</param>
		/// <param name="op">The divisor</param>
		/// <returns>The modified Vector2</returns>
		public static Vector2 V2Modulus(this Vector2 v, float op = 1)
		{
			return new Vector2(v.x.Mod(op), v.y.Mod(op));
		}

		/// <summary>
		/// Performs floor operation on this Vector2
		/// </summary>
		/// <param name="v">This Vector2</param>
		/// <param name="axisFilter">The axes to evaluate</param>
		/// <returns>The modified Vector2</returns>
		public static Vector2 V2Floor(this Vector2 v, Axis axisFilter = Axis.X | Axis.Y)
		{
			bool[] a =
			{
				axisFilter.HasFlag(Axis.X),
				axisFilter.HasFlag(Axis.Y)
			};

			return new Vector2(
				a[0]? v.x.Floor() : v.x,
				a[1] ? v.y.Floor() : v.y);
		}

		/// <summary>
		/// Performs rounding operation on this Vector2
		/// </summary>
		/// <param name="v">This Vector2</param>
		/// <param name="axisFilter">The axes to evaluate</param>
		/// <returns>The modified Vector2</returns>
		public static Vector2 V2Round(this Vector2 v, Axis axisFilter = Axis.X | Axis.Y)
		{
			bool[] a =
			{
				axisFilter.HasFlag(Axis.X),
				axisFilter.HasFlag(Axis.Y)
			};

			return new Vector2(
				a[0] ? v.x.Round() : v.x,
				a[1] ? v.y.Round() : v.y);
		}

		/// <summary>
		/// Enumerates a set of Vector2s offset by this Vector2
		/// </summary>
		/// <param name="o">This Vector2</param>
		/// <param name="vals">The set of Vector2s to evaluate</param>
		/// <returns>The enumeration of offset Vector2s</returns>
		public static IEnumerable<Vector2> V2OffsetSet(this Vector2 o, params Vector2[] vals)
		{
			foreach(Vector2 v in vals)
			{
				yield return o + v;
			}
		}

		public static float V2Distance(this Vector2 o, Vector2 v)
		{
			return (o - v).magnitude;
		}

		#endregion
	}

	/// <summary>
	/// Provides extension methods for Vector3s
	/// </summary>
	public static class Vector3Ext
	{
		#region Public Methods

		/// <summary>
		/// Performs modulus operation on this Vector3
		/// </summary>
		/// <param name="v">This Vector3</param>
		/// <param name="op">The divisor</param>
		/// <returns>The modified Vector3</returns>
		public static Vector3 V3Modulus(this Vector3 v, float op = 1)
		{
			return new Vector3(v.x.Mod(op), v.y.Mod(op), v.z.Mod(op));
		}

		/// <summary>
		/// Performs floor operation on this Vector3
		/// </summary>
		/// <param name="v">This Vector3</param>
		/// <param name="axisFilter">The axes to evaluate</param>
		/// <returns>The modified Vector3</returns>
		public static Vector3 V3Floor(this Vector3 v, Axis axisFilter = Axis.X | Axis.Y | Axis.Z)
		{
			bool[] a =
			{
				axisFilter.HasFlag(Axis.X),
				axisFilter.HasFlag(Axis.Y),
				axisFilter.HasFlag(Axis.Z)
			};

			return new Vector3(
				a[0] ? v.x.Floor() : v.x, 
				a[1] ? v.y.Floor() : v.y,
				a[2] ? v.z.Floor() : v.z);
		}

		/// <summary>
		/// Performs rounding operation on this Vector3
		/// </summary>
		/// <param name="v">This Vector3</param>
		/// <param name="axisFilter">The axes to evaluate</param>
		/// <returns>The modified Vector3</returns>
		public static Vector3 V3Round(this Vector3 v, Axis axisFilter = Axis.X | Axis.Y | Axis.Z)
		{
			bool[] a =
			{
				axisFilter.HasFlag(Axis.X),
				axisFilter.HasFlag(Axis.Y),
				axisFilter.HasFlag(Axis.Z)
			};

			return new Vector3(
				a[0] ? v.x.Round() : v.x,
				a[1] ? v.y.Round() : v.y,
				a[2] ? v.z.Round() : v.z);
		}

		#endregion
	}

	/// <summary>
	/// Represents axes in 3D space
	/// </summary>
	[Flags]
	public enum Axis
	{
		/// <summary>
		/// X axis
		/// </summary>
		X = 1,
		/// <summary>
		/// Y axis
		/// </summary>
		Y = 2,
		/// <summary>
		/// Z axis
		/// </summary>
		Z = 4
	}
}


