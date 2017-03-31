using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace HWTools
{
	/// <summary>
	/// Provides extension methods for floats
	/// </summary>
	public static class FloatExt
	{
		/// <summary>
		/// Performs floored modulus operation on this float
		/// <para/>
		/// Distinct from truncated remainder ( % )
		/// </summary>
		/// <param name="x">This float</param>
		/// <param name="m">The divisor</param>
		/// <returns>The result of the modulus operation</returns>
		public static float Mod(this float x, float m = 1.0f)
		{
			return x - (m * Mathf.Floor(x / m));
		}

		/// <summary>
		/// Floors this float
		/// </summary>
		/// <param name="x">This float</param>
		/// <returns></returns>
		public static float Floor(this float x)
		{
			return Mathf.Floor(x);
		}

		/// <summary>
		/// Rounds this float
		/// </summary>
		/// <param name="x">This float</param>
		/// <returns></returns>
		public static float Round(this float x)
		{
			return Mathf.Round(x);
		}

		public static float Max(this float x, params float[] floats)
		{
			for (int i = 0; i < floats.Length; i++)
			{
				x = Math.Max(x, floats[i]);
			}
			return x;
		}

		public static float Min(this float x, params float[] floats)
		{
			for (int i = 0; i < floats.Length; i++)
			{
				x = Math.Min(x, floats[i]);
			}
			return x;
		}
		public static float Clamp(this float f, float min, float max)
		{
			return Math.Max(Math.Min(f, max), min);
		}
	}

	/// <summary>
	/// Provides extension methods for ints
	/// </summary>
	public static class IntegerExt
	{ 
		/// <summary>
		/// Enumerates a sequence of integers.
		/// <para/>
		/// Includes final value.
		/// </summary>
		/// <param name="start">First value</param>
		/// <param name="end">Last value (inclusive)</param>
		/// <returns></returns>
		public static IEnumerable<int> To(this int start, int end, int step = 1)
		{
			step = Math.Abs(step);
			if (start < end)
			{
				for (int i = start; i <= end; i += step)
					yield return i;
			}
			else
			{
				for (int i = start; i >= end; i -= step)
					yield return i;
			}
		}

		/// <summary>
		/// Enumerates a sequence of integers.
		/// <para/>
		/// Excludes final value.
		/// </summary>
		/// <param name="start">First value</param>
		/// <param name="end">Last value (exclusive)</param>
		/// <returns></returns>
		public static IEnumerable<int> Until(this int start, int end, int step = 1)
		{
			step = Math.Abs(step);
			if (start < end)
			{
				for (int i = start; i < end; i += step)
					yield return i;
			}
			else
			{
				for (int i = start; i > end; i -= step)
					yield return i;
			}
		}

		/// <summary>
		/// Performs floored modulus operation on this int
		/// <para/>
		/// Distinct from truncated remainder ( % )
		/// </summary>
		/// <param name="x">This int</param>
		/// <param name="m">The divisor</param>
		/// <returns>The result of the modulus operation</returns>
		public static int Mod(this int x, int m)
		{
			return x - (m * (int)Mathf.Floor(x / m));
		}
		
		/// <summary>
		/// Calculates absolute value of this int
		/// </summary>
		/// <param name="i">This int</param>
		/// <returns>The absolute value of this int</returns>
		public static int Abs(this int i)
		{
			return Math.Abs(i);
		}

		/// <summary>
		/// Evaluates the maximum value of a set of ints including this int
		/// </summary>
		/// <param name="x">This int</param>
		/// <param name="ints">The set of ints to evaluate</param>
		/// <returns>The maximum int</returns>
		public static int Max(this int x, params int[] ints)
		{
			for (int i = 0; i < ints.Length; i++)
			{
				x = Math.Max(x, ints[i]);
			}
			return x;
		}

		/// <summary>
		/// Evaluates the minimum value of a set of ints including this int
		/// </summary>
		/// <param name="x">This int</param>
		/// <param name="ints">The set of ints to evaluate</param>
		/// <returns>The minimum int</returns>
		public static int Min(this int x, params int[] ints)
		{
			for (int i = 0; i < ints.Length; i++)
			{
				x = Math.Min(x, ints[i]);
			}
			return x;
		}

		/// <summary>
		/// Reports whether this int is even
		/// </summary>
		/// <param name="i">This int</param>
		/// <returns>Whether this int is even</returns>
		public static bool Even(this int i)
		{
			return (i & 1) == 0;
		}

		/// <summary>
		/// Reports whether this int is odd
		/// </summary>
		/// <param name="i">This int</param>
		/// <returns>Whether this int is odd</returns>
		public static bool Odd(this int i)
		{
			return (i & 1) == 1;
		}

		public static int Clamp(this int i, int min, int max)
		{
			return Math.Max(Math.Min(i, max), min);
		}
	}

}