using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace HWTools.Reflection
{
	/// <summary>
	/// Provides extension methods for reflection
	/// </summary>
	static class RefExt
	{
		public static MethodInfo GetGenericMethod(this Type t, string name, int genericarguments)
		{
			var methods = t.GetMethods();
			var result = methods.First(
				m =>
				m.Name == name &&
				m.GetGenericArguments().Count() == genericarguments);

			return result;
		}
	}

	/// <summary>
	/// Provides extension method for adding several components to a GameObject at once
	/// </summary>
	public static class TypeSets
	{
		public static readonly Type[] TextComponents = 
			{typeof(RectTransform),
			typeof(CanvasRenderer),
			typeof(Text)};

		public static readonly Type[] CanvasComponents =
			{typeof(Canvas),
			typeof(CanvasScaler),
			typeof(GraphicRaycaster)};

		public static readonly Type[] ImageComponents =
			{typeof(RectTransform),
			typeof(CanvasRenderer),
			typeof(Image)};

		/// <summary>
		/// Add multiple components to this GameObject
		/// </summary>
		/// <param name="go">This GameObject</param>
		/// <param name="types">The types of components to add</param>
		public static void AddComponentSet(this GameObject go, params Type[] types)
		{
			if (types.Length == 0)
				return;

			//var addComponentSignature = typeof(GameObject).GetGenericMethod("AddComponent", 1);

			foreach(Type t in types)
			{
				go.AddComponent(t);

				//addComponentSignature.MakeGenericMethod(t)
				//	.Invoke(go, null);
			}
		}
	}
}
