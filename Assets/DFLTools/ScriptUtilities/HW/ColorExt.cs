using UnityEngine;

namespace HWTools
{

	/// <summary>
	/// Provides extension methods for colors
	/// </summary>
	public static class ColorExt
	{
		public static Color Mult(this Color c, Color cx)
		{
			return new Color(c.r * cx.r, c.g * cx.g, c.b * cx.b, c.a * cx.a);
		}

		/// <summary>
		/// Multiplies a Color by a random float.
		/// </summary>
		/// <param name="c">This Color</param>
		/// <param name="rangeMin">Random minimum</param>
		/// <param name="rangeMax">Random maximum</param>
		/// <returns>The modified Color</returns>
		public static Color RandMult(this Color c, float rangeMin = 0, float rangeMax = 1)
		{
			float rand = Random.Range(rangeMin, rangeMax);

			c.r *= rand;
			c.g *= rand;
			c.b *= rand;
			
			return c;
		}

		/// <summary>
		/// Multiplies a Color by a random Color.
		/// </summary>
		/// <param name="c">This Color</param>
		/// <param name="rangeMin">Random minimum</param>
		/// <param name="rangeMax">Random maximum</param>
		/// <returns>The modified Color</returns>
		public static Color RandMult(this Color c, Color rangeMin, Color rangeMax)
		{
			c.r *= Random.Range(rangeMin.r, rangeMax.r);
			c.g *= Random.Range(rangeMin.g, rangeMax.g);
			c.b *= Random.Range(rangeMin.b, rangeMax.b);
			c.a *= Random.Range(rangeMin.a, rangeMax.a);

			return c;
		}

	}

}