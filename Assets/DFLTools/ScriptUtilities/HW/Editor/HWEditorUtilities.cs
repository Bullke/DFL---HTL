using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace HWTools.Edit
{
	/// <summary>
	/// Provides methods for establishing a directory for generated content
	/// </summary>
	public static class EmitterUtils
	{
		#region Private Fields

		static readonly string _path = "Generated";

		#endregion

		#region Private Properties

		static string AssetPath { get { return "Assets/" + _path; } }

		#endregion

		#region Internal Methods

		internal static string PrepareDirectory(string v)
		{
			string result = _path + "/" + v + "/";

			if (!AssetDatabase.IsValidFolder(AssetPath))
			{
				AssetDatabase.CreateFolder("Assets", _path);
			}
			if (!AssetDatabase.IsValidFolder(AssetPath + "/" + v))
			{
				AssetDatabase.CreateFolder(AssetPath, v);
			}

			return result;
		}

		#endregion
	}

	/// <summary>
	/// Provides extra GUI layout methods
	/// </summary>
	public static class HWEdLayout
	{
		#region Public Methods

		public static T ObjectField<T>
			(GUIContent label, T obj, bool allowSceneObjects = true, params GUILayoutOption[] options)
			where T : UnityEngine.Object
		{
			return EditorGUILayout.ObjectField(label, obj, typeof(T), allowSceneObjects, options) as T;
		}

		public static T ObjectField<T>
			(string label, T obj, bool allowSceneObjects = true, params GUILayoutOption[] options)
			where T : UnityEngine.Object
		{
			return EditorGUILayout.ObjectField(label, obj, typeof(T), allowSceneObjects, options) as T;
		}

		public static T ObjectField<T>
			(T obj, bool allowSceneObjects = true, params GUILayoutOption[] options)
			where T : UnityEngine.Object
		{
			return EditorGUILayout.ObjectField(obj, typeof(T), allowSceneObjects, options) as T;
		}

		#endregion
	}

	/// <summary>
	/// Handles a GUI conditional group
	/// <para/>
	/// Used with using(){} structure
	/// </summary>
	public class GroupConditional : IDisposable
	{
		#region Private Fields

		/// <summary>
		///   Used for nesting 
		/// </summary>
		bool _previousState;

		#endregion

		#region Public Constructors

		/// <summary>
		/// Starts the group
		/// </summary>
		/// <param name="condition">The enabled/disabled state of the group</param>
		public GroupConditional(bool condition)
		{
			_previousState = GUI.enabled;
			GUI.enabled = condition;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Ends the group
		/// </summary>
		public void Dispose()
		{
			GUI.enabled = _previousState;
		}

		#endregion
	}

	/// <summary>
	/// Handles a GUI horizontal group
	/// <para/>
	/// Used with using(){} structure
	/// </summary>
	public class GroupHorizontal : IDisposable
	{
		bool _inHorizontal;
		GUILayoutOption[] _opts;
		#region Public Constructors

		/// <summary>
		/// Starts the group
		/// </summary>
		/// <param name="options"></param>
		public GroupHorizontal(params GUILayoutOption[] options)
		{
			_opts = options;
			Restart();
		}

		/// <summary>
		/// Starts the group
		/// </summary>
		/// <param name="condition">The enabled/disabled state of the group</param>
		/// <param name="options"></param>
		public GroupHorizontal(bool condition, params GUILayoutOption[] options)
		{
			_opts = options;
			if (condition)
				Restart();
		}

		#endregion

		/// <summary>
		/// Starts a new horizontal group
		/// </summary>
		/// <param name="condition"></param>
		public void Restart(bool condition = true)
		{
			if (!condition)
				return;

			if(_inHorizontal)
			{
				GUILayout.EndHorizontal();
			}
			_inHorizontal = true;
			GUILayout.BeginHorizontal(_opts);
		}

		/// <summary>
		/// Ends the current horizontal group
		/// </summary>
		void End()
		{
			_inHorizontal = false;
			GUILayout.EndHorizontal();
		}
		#region Public Methods

		/// <summary>
		/// Ends the group
		/// </summary>
		public void Dispose()
		{
			if(_inHorizontal)
			{
				End();
			}
		}

		#endregion
	}

	/// <summary>
	/// Handles a GUI scroll group
	/// <para/>
	/// Used with using(){} structure
	/// </summary>
	public class GroupScrollView : IDisposable
	{
		#region Public Constructors

		/// <summary>
		/// Starts the group
		/// </summary>
		/// <param name="pos">The scroll position</param>
		public GroupScrollView(ref Vector2 pos)
		{
			pos = EditorGUILayout.BeginScrollView(pos);
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Ends the group
		/// </summary>
		public void Dispose()
		{
			EditorGUILayout.EndScrollView();
		}

		#endregion
	}

	/// <summary>
	/// Handles a GUI toggle group
	/// <para/>
	/// Used with using(){} structure
	/// </summary>
	public class GroupToggle : IDisposable
	{
		#region Public Constructors
		
		/// <summary>
		/// Starts the group
		/// </summary>
		/// <param name="label">The group's label</param>
		/// <param name="toggle">The toggle state</param>
		public GroupToggle(GUIContent label, ref bool toggle)
		{
			toggle = EditorGUILayout.BeginToggleGroup(label, toggle);
		}

		/// <summary>
		/// Starts the group
		/// </summary>
		/// <param name="label">The group's label</param>
		/// <param name="toggle">The toggle state</param>
		public GroupToggle(string label, ref bool toggle)
		{
			toggle = EditorGUILayout.BeginToggleGroup(label, toggle);
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Ends the group
		/// </summary>
		public void Dispose()
		{
			EditorGUILayout.EndToggleGroup();
		}

		#endregion
	}

	/// <summary>
	/// Handles a GUI vertical group
	/// <para/>
	/// Used with using(){} structure
	/// </summary>
	public class GroupVertical : IDisposable
	{
		#region Public Constructors

		/// <summary>
		/// Starts the group
		/// </summary>
		/// <param name="options"></param>
		public GroupVertical(params GUILayoutOption[] options)
		{
			GUILayout.BeginVertical(options);
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Ends the group
		/// </summary>
		public void Dispose()
		{
			GUILayout.EndVertical();
		}

		#endregion
	}

}
