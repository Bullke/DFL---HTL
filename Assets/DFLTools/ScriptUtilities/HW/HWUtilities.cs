using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using UnityEngine;

namespace HWTools
{
	/// <summary>
	/// Provides extension methods for various classes
	/// </summary>
	public static class HWUtilities
	{
		#region Public Methods
		
		/// <summary>
		/// Formats an enumeration of strings with newlines
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public static string Stack(this IEnumerable<string> source)
		{
			StringBuilder result = new StringBuilder();
			foreach(var s in source)
			{
				result.AppendLine(s);
			}

			return result.ToString();
		}

		/// <summary>
		/// Enumerates the children of a GameObject
		/// </summary>
		/// <param name="t">This GameObject</param>
		/// <returns></returns>
		public static IEnumerable<Transform> Children(this Transform t)
		{
			for(int i = 0; i < t.childCount; i++)
			{
				yield return t.GetChild(i);
			}
		}

		/// <summary>
		/// Returns true if an enum has a given flag
		/// </summary>
		/// <param name="e">This enum</param>
		/// <param name="flag">the flag(s) to check</param>
		/// <returns>Whether the enum contains given flag(s)</returns>
		public static bool HasFlag(this Enum e, Enum flag)
		{
			if(e.GetType() == flag.GetType())
			{
				var eVal = Convert.ToUInt64(e);
				var fVal = Convert.ToUInt64(flag);

				return (eVal & fVal) == fVal;
			}
			return false;
		}

		/// <summary>
		/// Checks if an enum matches any of a set of given enum values
		/// </summary>
		/// <param name="e">This enum</param>
		/// <param name="others">The set of enums to evaluate</param>
		/// <returns></returns>
		public static bool IsAny(this Enum e, params Enum[] others)
		{
			return others.Any(o => e.GetType() == o.GetType() && e.Equals(o));
		}

		/// <summary>
		/// Calculates the length of this Rectangle's diagonal line segment.
		/// <para/>
		/// √(height^2 + width^2)
		/// </summary>
		/// <param name="r">This Rect</param>
		/// <returns>√(height^2 + width^2)</returns>
		public static float Span(this Rect r)
		{
			return Mathf.Sqrt(
				Mathf.Pow(r.height, 2) + Mathf.Pow(r.width, 2)
				);
		}

		/// <summary>
		/// Evaluates whether this string contains a character
		/// </summary>
		/// <param name="s">This string</param>
		/// <param name="c">The char to find</param>
		/// <returns>Whether this string contains the given char</returns>
		public static bool Contains(this string s, char c)
		{
			return s.Any(ch=> ch == c);
		}

		/// <summary>
		/// Instantiates a copy of this GameObject
		/// </summary>
		/// <param name="go">This GameObject</param>
		/// <returns>The instantiated copy</returns>
		public static GameObject Instantiate(this GameObject go)
		{
			return UnityEngine.Object.Instantiate(go);
		}

		/// <summary>
		/// Instantiates a GameObject as a child of this GameObject
		/// </summary>
		/// <param name="go">This GameObject</param>
		/// <param name="original">Optional GameObject</param>
		/// <returns></returns>
		public static GameObject CreateChild(this GameObject go, GameObject original = null)
		{
			var added =
				(original != null) ?
					GameObject.Instantiate(original) :
					new GameObject();

			added.transform.SetParent(go.transform);
			return added;
		}

		/// <summary>
		/// Sets this RectTransform's anchors to cover parent
		/// </summary>
		/// <param name="rt">This RectTransform</param>
		public static void StretchFill(this RectTransform rt)
		{
			rt.anchorMin = Vector2.zero;
			rt.anchorMax = Vector2.one;

			rt.offsetMin = Vector2.zero;
			rt.offsetMax = Vector2.zero;
		}

		/// <summary>
		/// Centers this RectTransform relative to the parent GameObject
		/// </summary>
		/// <param name="go">This GameObject</param>
		public static void CenterOnParent(this GameObject go)
		{
			var rt = go.GetComponent<RectTransform>();
			if (rt)
			{
				rt.anchoredPosition = Vector2.zero;
			}
			else
			{
				go.transform.localPosition = Vector2.zero;
			}
		}

		/// <summary>
		/// Gets the parent of this RectTransform
		/// </summary>
		/// <param name="rt">This RectTransform</param>
		/// <returns>The RectTransform of the parent GameObject</returns>
		public static RectTransform GetRectParent(this RectTransform rt)
		{
			return rt.parent.GetComponent<RectTransform>();
		}

		/// <summary>
		/// Currently unused
		/// </summary>
		/// <param name="toSplit"></param>
		/// <returns></returns>
		public static IEnumerable<IntPair> SplitIndicies(this string toSplit)
		{
			var result = new IntPair { a = -1, b = -1 };
			char prev = '\0';
			for (int i = 0; i < toSplit.Length; i++)
			{
				if (result.b >= result.a)
				{
					if (!(" \t\n".Contains(toSplit[i])) && " \t\n\0".Contains(prev))
					{
						result.a = i;
					}
				}
				if (result.b < result.a)
				{
					if (i + 1 == toSplit.Length || " \t\n".Contains(toSplit[i + 1]))
					{
						result.b = i;
						yield return result;
						result = new IntPair(result);
					}
				}
				prev = toSplit[i];
			}
		}

		#endregion
	}

	/// <summary>
	/// Currently unused
	/// </summary>
	[Serializable]
	public class IntPair
	{
		#region Public Fields

		[SerializeField]
		public int a,b;

		#endregion

		#region Public Constructors

		public IntPair()
		{ }

		public IntPair(IntPair other)
		{
			a = other.a;
			b = other.b;
		}

		#endregion

		#region Public Methods

		public IntPair Shift(int offset)
		{
			a += offset;
			b += offset;
			return this;
		}

		public override string ToString()
		{
			return a + " " + b;
		}

		#endregion
	}
}

