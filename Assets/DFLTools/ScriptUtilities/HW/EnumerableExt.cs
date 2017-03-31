using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace HWTools
{
	public static class EnumerableExt
	{
		/// <summary>
		/// Retrieves a random item from this IEnumerable
		/// </summary>
		/// <typeparam name="T">Collection item type</typeparam>
		/// <param name="e">This IEnumerable</param>
		/// <param name="min">Minimum index</param>
		/// <param name="max">Maximum index</param>
		/// <returns>A random item from the IEnumerable</returns>
		public static T Random<T>(this IEnumerable<T> e, int min = 0, int max = -1)
		{
			if(!e.Any())
			{
				return default(T);
			}

			var asList = e as IList<T>;
			if (asList == null)
			{
				asList = e.ToList();
			}

			if(max == -1)
			{
				max = asList.Count();
			}
			else
			{
				max = max.Clamp(0, asList.Count - 1);
			}
			min = min.Clamp(0, asList.Count - 1);

			return asList[UnityEngine.Random.Range(min, max)];

		}
	}

}