using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HWTools.Edit
{

	/// <summary>
	/// Provides a set of buttons for selecting prefabs
	/// </summary>
	public class PrefabButtonSelector
	{
		#region Private Fields

		/// <summary>
		/// The selected index
		/// </summary>
		int _selection = -1;

		#endregion

		#region Public Properties

		/// <summary>
		/// The enumeration of prefabs
		/// </summary>
		public IEnumerable<GameObject> Collection { get; protected set; }

		/// <summary>
		/// The filter function for limiting prefabs in enumeration
		/// </summary>
		public Func<GameObject, bool> Filter { get; set; }

		/// <summary>
		/// The selected prefab
		/// </summary>
		public GameObject Selected { get; protected set; }

		/// <summary>
		/// GUIContent for the selected prefab
		/// </summary>
		public GUIContent SelectedGUIContent
		{
			get
			{
				if (Selected)
				{
					return new GUIContent {
						text = Selected.name,
						image = AssetPreview.GetAssetPreview(Selected)
					};
				}
				return new GUIContent("No selection");
			}
		}


		#endregion

		#region Public Methods

		/// <summary>
		/// Establishes the enumerable set of prefabs from a folder
		/// </summary>
		/// <param name="folder">The folder to inspect</param>
		public void PopulateCollection(string folder)
		{
			var results = AssetDatabase.FindAssets("t:GameObject", new []{ folder });
			Collection = from a in results
						 let ob = AssetDatabase.LoadAssetAtPath<GameObject>(
							 AssetDatabase.GUIDToAssetPath(a))
						 where ob != null
						 select ob;
		}

		/// <summary>
		/// Render GUI selection buttons
		/// </summary>
		/// <param name="width">The width of individual buttons</param>
		public void RenderButtons(int width = 150)
		{
			if (Collection == null)
			{
				Debug.Log("Prefab Button Set:\nCollection not set.");
				EditorGUILayout.HelpBox("Prefab Button Set:\nCollection not set.", MessageType.Error);
				return;
			}

			if (!Collection.Any())
			{
				EditorGUILayout.HelpBox("Prefab Button Set:\nNo prefabs found.", MessageType.Warning);
				return;
			}

			Selected = null;

			if (Filter == null)
			{
				Filter = go => true;
			}

			int w = (int)EditorGUIUtility.currentViewWidth / (width + 4);

			using (var horiz = new GroupHorizontal(w > 1))
			{ 
				foreach (var s in
					Collection.Where(Filter)
					.Select((o, i) => new { o, i }))
				{
					GUIContent c = new GUIContent
					{
						image = AssetPreview.GetAssetPreview(s.o),
						text = s.o.name
					};					

					if (GUILayout.Toggle(s.i == _selection, c, "Button",
						GUILayout.Width(width), GUILayout.Height(64)))
					{
						Selected = s.o;
						_selection = s.i;
					}
					
					horiz.Restart(w > 1 && (s.i + 1) % w == 0);
				}
			}
			
			//GUILayout.SelectionGrid(_selection,);


			if (Selected == null)
			{
				_selection = -1;
			}
		}

		/// <summary>
		/// Clear selection
		/// </summary>
		internal void Deselect()
		{
			_selection = -1;
			Selected = null;
		}

		#endregion
	}

}