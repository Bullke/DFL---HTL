using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HWTools.Edit
{
	/// <summary>
	/// Provides a dropdown list and manages a collection of objects
	/// <para/>
	/// GUI element
	/// </summary>
	/// <typeparam name="T">The type of objects in the collection</typeparam>
	public class HWDropdown<T>
	{
		#region Private Fields

		/// <summary>
		/// Index of the selected object
		/// </summary>
		int _selection;

		/// <summary>
		/// The set of names associated with objects
		/// </summary>
		string[] _selectionNames;

		/// <summary>
		/// The set of objects
		/// </summary>
		T[] _selectionValues;

		#endregion

		#region Public Properties

		/// <summary>
		/// Selected object's name
		/// </summary>
		public string Name
		{
			get
			{
				if (_selectionNames.Length == 0)
				{
					return "";
				}
				return _selectionNames[_selection];
			}
		}

		/// <summary>
		/// Selected object
		/// </summary>
		public T Value
		{
			get
			{
				if (_selectionValues.Length == 0)
				{
					return default(T);
				}
				return _selectionValues[_selection];
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Renders the dropdown GUI element
		/// </summary>
		/// <param name="values"></param>
		/// <param name="names"></param>
		public void Field(T[] values = null, string[] names = null)
		{
			if (values != null)
				_selectionValues = values;

			if (names != null)
				_selectionNames = names;

			if (_selectionValues == null || _selectionNames == null ||
				_selectionNames.Length != _selectionValues.Length)
			{
				EditorGUILayout.LabelField("Invalid dropdown data");
			}

			_selection = EditorGUILayout.Popup(_selection, _selectionNames);
		}

		/// <summary>
		/// Assigns values and names to the dropdown's backing collections
		/// </summary>
		/// <param name="values">The set of values</param>
		/// <param name="names">The set of names</param>
		public void SetOptions(T[] values, string[] names)
		{
			_selectionValues = values;
			_selectionNames = names;
		}

		#endregion
	}

}