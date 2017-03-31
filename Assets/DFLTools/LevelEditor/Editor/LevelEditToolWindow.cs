using HWTools;
using HWTools.Edit;
using HWTools.Grid;
using LevelEditTool.Internals;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using ESM = UnityEditor.SceneManagement.EditorSceneManager;

namespace LevelEditTool
{
	/// <summary>
	/// The HTL level editor tool window 
	/// </summary>
	class LevelEditToolWindow : EditorWindow
	{
		#region Private Fields

		/// <summary>
		/// Whether the paint tool is in eraser mode
		/// </summary>
		bool _eraserMode;

		/// <summary>
		/// PrefabByttonSelector instance
		/// </summary>
		PrefabButtonSelector _pbs;

		/// <summary>
		/// Scene name list
		/// </summary>
		IList<string> _scanned;

		/// <summary>
		/// The scroll position of the window
		/// </summary>
		Vector2 _scrollpos;

		/// <summary>
		/// The grid selected by the tool
		/// </summary>
		Grid2DCollection _selectedGrid;

		#endregion

		#region Private Methods

		/// <summary>
		/// Initializes the window
		/// <para/>
		/// Menu button
		/// </summary>
		[MenuItem("DFL Tools/HTL/Level Edit Tool")]
		static void Init()
		{
			var window = GetWindow<LevelEditToolWindow>();

			window.Show();
			window.titleContent = new GUIContent("Level Edit Tool");
		}

		/// <summary>
		/// Guarantees that the workspace exists 
		/// </summary>
		static void WorkspaceSetup()
		{
			if (!File.Exists(Env.WorkspacePath))
			{
				Env.EnsureDirectory(Env.ToolDirectory);
				var scene = ESM.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
				ESM.SaveScene(scene, Env.WorkspacePath);
			}

			ESM.OpenScene(Env.WorkspacePath);
		}

		/// <summary>
		/// Renders level selection GUI
		/// </summary>
		void GUILevelSelect()
		{
			// todo build a class for listing and managing scenes

			// todo scene folder structure

			if (_scanned == null || GUILayout.Button("Scan scenes"))
			{
				var openalready = from i in 0.Until(ESM.sceneCount)
								  select ESM.GetSceneAt(i);

				string[] folders = { "Generated" };

				var pathquery = from a in AssetDatabase.FindAssets("t:Scene", folders)
								let path = AssetDatabase.GUIDToAssetPath(a)
								where !openalready.Select(s => s.path).Contains(path)
								select path;

				var scenequery = (from path in pathquery
								  select ESM.OpenScene(path, OpenSceneMode.Additive));

				List<string> results = new List<string>();

				foreach (var scene in scenequery)
				{
					if (scene.GetRootGameObjects()
						.Any(go => go.GetComponent<Grid2DCollection>()))
						results.Add(scene.path);

					ESM.CloseScene(scene, true);
				}

				_scanned = results;
			}

			string str = _scanned.Stack();

			EditorGUILayout.HelpBox(str, MessageType.None);
		}

		/// <summary>
		/// Renders GUI for interaction with a grid
		/// </summary>
		void GUIWorking()
		{
			EditorGUILayout.LabelField("Grid manipulation");

			if (_pbs == null)
			{
				_pbs = new PrefabButtonSelector();
			}

			_pbs.PopulateCollection(Env.ToolDirectory);

			_eraserMode = GUILayout.Toggle(_eraserMode, "Eraser Mode", "Button",
				GUILayout.Height(64));

			if (_eraserMode)
			{
				_pbs.Deselect();
			}

			_pbs.RenderButtons();

			if (_pbs.Selected != null)
			{
				_eraserMode = false;
				GUILayout.Label(_pbs.SelectedGUIContent);
			}
		}

		/// <summary>
		/// Logic to run when the window is disabled
		/// <para/>
		/// Unity callback method
		/// </summary>
		void OnDisable()
		{
			SceneView.onSceneGUIDelegate -= OnSceneGUI;
		}

		/// <summary>
		/// Logic to run when the window is enabled
		/// <para/>
		/// Unity callback method
		/// </summary>
		void OnEnable()
		{
			SceneView.onSceneGUIDelegate += OnSceneGUI;
		}


		/// <summary>
		/// Renders the window's GUI
		/// <para/>
		/// Unity callback method
		/// </summary>
		void OnGUI()
		{
			Env.EnsureDirectory(Env.ToolDirectory);

			using (new GroupScrollView(ref _scrollpos))
			{
				GUILevelSelect();

				_selectedGrid = FindObjectOfType<Grid2DCollection>();

				using (new GroupConditional(_selectedGrid))
				{
					GUIWorking();
				}
			}
		}

		/// <summary>
		/// Intercepts events in the viewport
		/// <para/>
		/// Unity callback method
		/// </summary>
		/// <param name="sv">The contextual SceneView</param>
		void OnSceneGUI(SceneView sv)
		{
			if (_selectedGrid == null)
				return;

			var e = Event.current;

			if (e.type == EventType.MouseMove)
			{
				var mousepos = e.mousePosition;
				mousepos.y = sv.camera.pixelHeight - mousepos.y;
				var worldpos = sv.camera.ScreenToWorldPoint(mousepos);

				_selectedGrid.Grid.SelectDebugGizmoCell(worldpos);

				e.Use();
			}

			if (e.type == EventType.MouseDown)
			{
				switch (e.button)
				{
					case 0: // left click
						if (!_pbs.Selected && !_eraserMode) // no object selected, do nothing
							break;

						PaintActon(e, sv);
						e.Use();
						break;

					case 1: // right click
						if (!_pbs.Selected)
							return;

						_pbs.Deselect();
						Repaint();
						e.Use();
						break;
				}
			}

			if (e.type == EventType.MouseDrag)
			{
				var mousepos = e.mousePosition;
				mousepos.y = sv.camera.pixelHeight - mousepos.y;
				var worldpos = sv.camera.ScreenToWorldPoint(mousepos);

				_selectedGrid.Grid.SelectDebugGizmoCell(worldpos);

				switch (e.button)
				{
					case 0: // left click
						if (!_pbs.Selected && !_eraserMode) // no object selected, do nothing
							break;

						PaintActon(e, sv);
						e.Use();
						break;
				}
			}

			int passiveID = GUIUtility.GetControlID(FocusType.Passive);
			HandleUtility.AddDefaultControl(passiveID);
		}

		/// <summary>
		/// Paints or erases a tile on the grid, according to a mouse event
		/// </summary>
		/// <param name="e">The mouse event</param>
		/// <param name="sv">The contextual SceneView</param>
		void PaintActon(Event e, SceneView sv)
		{
			var mousepos = e.mousePosition;
			mousepos.y = sv.camera.pixelHeight - mousepos.y;
			var worldpos = sv.camera.ScreenToWorldPoint(mousepos);

			var gridpos = _selectedGrid.Grid.SelectTile(worldpos);

			if (gridpos.HasValue)
			{
				if (_eraserMode)
				{
					RemoveItem(gridpos.Value);
				}
				else
				{
					ReplaceItem(gridpos.Value);
				}
			}
		}

		/// <summary>
		/// Remove a tile from the grid
		/// </summary>
		/// <param name="sel">The index of the object</param>
		void RemoveItem(Vector2 sel)
		{
			if (_selectedGrid[sel])
			{
				DestroyImmediate(_selectedGrid[sel]);

				EditorUtility.SetDirty(_selectedGrid);
			}
		}

		/// <summary>
		/// Replace a tile on the grid
		/// </summary>
		/// <param name="sel">The index of the object</param>
		void ReplaceItem(Vector2 sel)
		{
			if (!_selectedGrid[sel] ||
				_selectedGrid[sel].name != _pbs.Selected.name)
			{
				if (_selectedGrid[sel])
				{
					DestroyImmediate(_selectedGrid[sel]);
				}

				GameObject current =
					_selectedGrid[sel] = PrefabUtility.InstantiatePrefab(_pbs.Selected) as GameObject;

				current.name = current.name.Replace("(Clone)", "");

				var gt = current.GetComponent<GridTransform>();

				if(gt != null)
				{
					gt.SetReference(_selectedGrid.Grid);
				}
				else
				{
					current.transform.SetParent(_selectedGrid.transform);
				}

				var center = _selectedGrid.Grid.SelectCenter(sel).Value;

				current.transform.localPosition = center;

				current.transform.localRotation = Quaternion.identity;
			}
			EditorUtility.SetDirty(_selectedGrid);
		}

		#endregion
	}
}