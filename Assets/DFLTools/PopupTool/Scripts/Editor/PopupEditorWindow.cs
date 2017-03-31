using HWTools;
using HWTools.Edit;
using HWTools.Reflection;
using PopupTool.Internals;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PopupTool
{
	/// <summary>
	/// The popup template editor tool window 
	/// </summary>
	partial class PopupToolWindow : EditorWindow
	{
		#region Private Fields

		/// <summary>
		/// The canvas to work with
		/// </summary>
		static Canvas _canvas;

		/// <summary>
		/// The template being modified
		/// </summary>
		static PopupTemplate _workingTemplate;

		/// <summary>
		/// Template file selector
		/// </summary>
		HWDropdown<FileInfo> _fileSelector = new HWDropdown<FileInfo>();

		/// <summary>
		/// The scroll position of the window
		/// </summary>
		Vector2 _scrollpos;

		#endregion

		#region Private Methods

		/// <summary>
		/// Guarantee that a canvas is available for use. 
		/// </summary>
		static void CanvasCheck()
		{
			_canvas = FindObjectOfType<Canvas>();
			if (_canvas == null)
			{
				var go = new GameObject();
				go.AddComponentSet(TypeSets.CanvasComponents);
				_canvas = go.GetComponent<Canvas>();
				_canvas.renderMode = RenderMode.ScreenSpaceOverlay;
				go.name = "Canvas";
			}
		}

		/// <summary>
		/// Spawn a template to work on
		/// </summary>
		/// <param name="original">Optional prefab</param>
		static void CreateTemplate(GameObject original = null)
		{
			if (_workingTemplate != null)
			{
				DestroyImmediate(_workingTemplate.gameObject);
				_workingTemplate = null;
			}

			if (original == null)
			{
				//todo implement sensible default template

				original = _canvas.gameObject.CreateChild();
				original.name = "PopupTemplate";
				original.AddComponentSet(TypeSets.ImageComponents);
				original.AddComponent<PopupTemplate>();
			}
			else
			{
				original = Instantiate(original);
				original.transform.SetParent(_canvas.transform);
				original.gameObject.CenterOnParent();
			}

			_workingTemplate = original.GetComponent<PopupTemplate>();

			_workingTemplate.gameObject.CenterOnParent();

			Recenter();
		}

		/// <summary>
		/// Initialize the window
		/// <para/>
		/// Menu button
		/// </summary>
		[MenuItem("DFL Tools/Popup Tool")]
		static void Init()
		{
			var window =
				GetWindow(typeof(PopupToolWindow)) as PopupToolWindow;

			window.Show();

			window.titleContent = new GUIContent("Popup Tool");
		}

		/// <summary>
		/// Setup variables for the tool's work environment 
		/// </summary>
		static void InitializeScene()
		{
			CanvasCheck();
			Recenter();
		}

		/// <summary>
		/// Focus the camera to the popup 
		/// </summary>
		static void Recenter()
		{
			if (_workingTemplate != null)
			{
				var f = _workingTemplate.GetComponent<RectTransform>();
				SceneView.lastActiveSceneView.LookAt(
					f.transform.position,
					Quaternion.Euler(0, 0, 0),
					f.rect.Span());
			}
		}

		/// <summary>
		/// Guarantee that the workspace exists 
		/// </summary>
		static void WorkspaceSetup()
		{
			if (!File.Exists(Env.WorkspacePath))
			{
				Env.EnsureDirectory(Env.ToolDirectory);
				var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
				EditorSceneManager.SaveScene(scene, Env.WorkspacePath);
			}

			EditorSceneManager.OpenScene(Env.WorkspacePath);
		}

		/// <summary>
		/// Render GUI for selecting a template to work on
		/// </summary>
		void GUIInWorkspace()
		{
			CanvasCheck();
			EditorGUILayout.HelpBox(Res.InformationMessage, MessageType.Info);
			GUILayout.Label("Select a template", EditorStyles.boldLabel);

			Env.EnsureDirectory(Env.OutputPath);

			var files =
				(from f in new DirectoryInfo(Env.OutputPath).GetFiles()
				 where f.Name.EndsWith(".prefab", StringComparison.OrdinalIgnoreCase)
				 select f).ToArray();

			var str =
				(from f in files
				 let i = f.Name.LastIndexOf(".", StringComparison.OrdinalIgnoreCase)
				 select f.Name.Substring(0, i)).ToArray();

			using (new GroupHorizontal())
			{
				_fileSelector.Field(files, str);

				if (GUILayout.Button("Open"))
				{
					var go =
						AssetDatabase.LoadAssetAtPath<GameObject>
							(Env.OutputPath + _fileSelector.Value.Name);
					if (go)
					{
						CreateTemplate(go);
						_workingTemplate.name = _fileSelector.Name;
					}
				}
				if (GUILayout.Button("New"))
				{
					CreateTemplate();
				}
			}
		}

		/// <summary>
		/// Render GUI for navigating to work space, or spawning a popup
		/// </summary>
		void GUIOutOfWorkspace()
		{
			EditorGUILayout.HelpBox(Res.IncorrectSceneMessage, MessageType.Warning);
			if (GUILayout.Button("Go to workspace scene"))
			{
				EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

				Debug.Log("Going to workspace");
				WorkspaceSetup();
				InitializeScene();
			}

			GUILayout.Space(24);
			GUIGenerateFromTemplate();
		}

		/// <summary>
		/// Renders tool GUI
		/// <para/>
		/// Unity callback method
		/// </summary>
		void OnGUI()
		{
			using (new GroupScrollView(ref _scrollpos))
			{
				if (SceneManager.GetActiveScene().name == Env.WorkspaceName)
				{
					_workingTemplate = FindObjectOfType<PopupTemplate>();

					if (_workingTemplate)
						GUIWorking();
					else
						GUIInWorkspace();
				}
				else
				{
					GUIOutOfWorkspace();
				}
			}
		}

		#endregion
	}
}