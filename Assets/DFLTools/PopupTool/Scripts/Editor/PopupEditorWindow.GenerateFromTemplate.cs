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
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace PopupTool
{
	partial class PopupToolWindow
	{
		#region Private Methods
		
		/// <summary>
		/// Renders the GUI for spawning a popup from a template
		/// </summary>
		void GUIGenerateFromTemplate()
		{
			CanvasCheck();

			EditorGUILayout.HelpBox("[Information for making a popup from a template]", MessageType.Info);
			GUILayout.Label("Make a popup:");

			Env.EnsureDirectory(Env.OutputPath);

			var files =
				(from f in new DirectoryInfo(Env.OutputPath).GetFiles()
				 where f.Name.EndsWith(".prefab", StringComparison.OrdinalIgnoreCase)
				 select f).ToArray();

			var str =
				(from f in files
				 let i = f.Name.LastIndexOf(".", StringComparison.OrdinalIgnoreCase)
				 select f.Name.Substring(0, i)).ToArray();

			_fileSelector.Field(files, str);

			using (new GroupConditional(files.Length > 0))
				if (GUILayout.Button("Spawn new prefab from selected template"))
				{
					var toSpawn =
						AssetDatabase.LoadAssetAtPath<GameObject>
							(Env.OutputPath + _fileSelector.Value.Name)
								.GetComponent<PopupTemplate>();

					var result = toSpawn.EmitPopup();
					var clickevent = 
						result.transform.FindChild("Button").GetComponent<Button>().onClick;

					
					UnityEventTools.AddPersistentListener(clickevent, result.GetComponent<PopupAssistant>().Hide);
				}

		}

		#endregion
	}
}
