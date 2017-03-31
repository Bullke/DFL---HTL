using HWTools;
using HWTools.Edit;
using PopupTool.Internals;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

using EGL = UnityEditor.EditorGUILayout;

namespace PopupTool
{

	partial class PopupToolWindow
	{
		/// <summary>
		/// Foldout open/closed states
		/// </summary>
		bool[] _foldout = new bool[5];

		#region Private Methods

		/// <summary>
		/// Renders GUI for modifying a popup template
		/// </summary>
		void GUIWorking()
		{
			CanvasCheck();

			EGL.HelpBox(Res.WorkingMessage, MessageType.Info);
			using (new GroupHorizontal())
			{
				string savestring =
					Env.TargetExists(_workingTemplate.name + ".prefab") ? "Overwrite " : "Save ";

				if (GUILayout.Button(savestring + _workingTemplate.name))
				{
					PrefabUtility.CreatePrefab(
						Env.OutputPath + _workingTemplate.name + ".prefab",
						_workingTemplate.gameObject);
				}
				if (GUILayout.Button("Clear Workspace"))
				{
					DestroyImmediate(_canvas.gameObject);
					return;
				}
			}

			GUILayout.Space(5);

			if (GUILayout.Button("Recenter Camera"))
				Recenter();

			GUILayout.Space(5);

			GUILayout.Label("Template Name", EditorStyles.boldLabel);
			_workingTemplate.name = EGL.TextField(_workingTemplate.name);

			WorkingBackground();
			WorkingBorder();
			WorkingText();
			WorkingButton();
			WorkingImageNodes();
		}

		/// <summary>
		/// Renders GUI for modifying the background
		/// </summary>
		void WorkingBackground()
		{
			if (!(_foldout[0] = EGL.Foldout(_foldout[0], "Background")))
				return;

			if (GUILayout.Button("Select"))
				Selection.objects = new GameObject[] { _workingTemplate.gameObject };
			var bg = _workingTemplate.GetComponent<Image>();
			bg.sprite =
				HWEdLayout.ObjectField(bg.sprite, false);

			using (new GroupHorizontal())
			{
				bg.type = (Image.Type)EGL.EnumPopup("Image Type", bg.type);

				if (bg.type.IsAny(Image.Type.Sliced, Image.Type.Tiled))
				{
					bg.fillCenter =
						GUILayout.Toggle(bg.fillCenter, "Fill center", "Button");
				}
			}

			bg.color = EGL.ColorField(bg.color);
		}

		/// <summary>
		/// Renders GUI for modifying the border
		/// </summary>
		void WorkingBorder()
		{
			_workingTemplate.GuaranteeBorder();
			var borderImage =
				_workingTemplate.Border.GetComponent<Image>();


			if (borderImage.sprite)
				borderImage.GetComponent<CanvasRenderer>().SetAlpha(1);
			else
				borderImage.GetComponent<CanvasRenderer>().SetAlpha(0);

			if (!(_foldout[1] = EGL.Foldout(_foldout[1], "Border")))
				return;

			
			if (GUILayout.Button("Select"))
				Selection.objects = new GameObject[] { _workingTemplate.Border };

			borderImage.sprite =
				HWEdLayout.ObjectField(borderImage.sprite, false);


			if (borderImage.sprite)
				borderImage.GetComponent<CanvasRenderer>().SetAlpha(1);
			else
				borderImage.GetComponent<CanvasRenderer>().SetAlpha(0);


			using (new GroupConditional(borderImage.sprite))
			{
				using (new GroupHorizontal())
				{
					borderImage.type = (Image.Type)EGL.EnumPopup("Image Type", borderImage.type);

					if (borderImage.type.IsAny(Image.Type.Sliced, Image.Type.Tiled))
					{
						borderImage.fillCenter =
							GUILayout.Toggle(borderImage.fillCenter, "Fill center", "Button");
					}
				}

				_workingTemplate.BorderMaxOffset =
					EGL.Vector2Field("Max Corner Offset", _workingTemplate.BorderMaxOffset);
				_workingTemplate.BorderMinOffset =
					EGL.Vector2Field("Min Corner Offset", _workingTemplate.BorderMinOffset);

				borderImage.color = EGL.ColorField(borderImage.color);
			}
		}

		/// <summary>
		/// Renders GUI for modifying the text properties
		/// </summary>
		void WorkingText()
		{
			_workingTemplate.GuaranteeTextBox();

			var t = _workingTemplate.TextBox.GetComponent<Text>();
			t.text = Res.Lorem;

			if (!(_foldout[2] = EGL.Foldout(_foldout[2], "Text")))
				return;


			if (GUILayout.Button("Select"))
				Selection.objects = new GameObject[] { _workingTemplate.TextBox };

			t.color = EGL.ColorField(t.color);
			t.font = HWEdLayout.ObjectField("Font", t.font, false);
			t.fontSize = EGL.IntField("Font size", t.fontSize);
			t.alignment = (TextAnchor)EGL.EnumPopup("Alignment", t.alignment);
		}

		/// <summary>
		/// Renders GUI for modifying the button
		/// </summary>
		void WorkingButton()
		{
			//todo implement multiple buttons

			_workingTemplate.GuaranteeButton();

			if (!(_foldout[3] = EGL.Foldout(_foldout[3], "Button")))
				return;

			var b = _workingTemplate.Button.GetComponent<Button>();

			if (GUILayout.Button("Select"))
				Selection.objects = new GameObject[] { _workingTemplate.Button };

			var i = _workingTemplate.Button.GetComponent<Image>();
			i.sprite =
				HWEdLayout.ObjectField(i.sprite, false);
			using (new GroupHorizontal())
			{
				i.type = (Image.Type)EGL.EnumPopup("Image Type", i.type);

				if (i.type.IsAny(Image.Type.Sliced, Image.Type.Tiled))
				{
					i.fillCenter =
						GUILayout.Toggle(i.fillCenter, "Fill center", "Button");
				}
			}

			b.colors = new ColorBlock
			{
				normalColor =
					EGL.ColorField("Normal", b.colors.normalColor),
				highlightedColor =
					EGL.ColorField("Highlighted", b.colors.highlightedColor),
				pressedColor =
					EGL.ColorField("Pressed", b.colors.pressedColor),
				disabledColor =
					EGL.ColorField("Disabled", b.colors.disabledColor),
				colorMultiplier = 1,
				fadeDuration = 0
			};

			GUILayout.Label("Button Text", EditorStyles.boldLabel);
			{
				var t = _workingTemplate.ButtonText.GetComponent<Text>();

				t.color = EGL.ColorField(t.color);
				t.font = HWEdLayout.ObjectField("Font", t.font, false);
				t.fontSize = EGL.IntField("Font size", t.fontSize);
				t.alignment = (TextAnchor)EGL.EnumPopup("Alignment", t.alignment);
			}
		}

		/// <summary>
		/// Renders GUI for modifying the image nodes
		/// </summary>
		void WorkingImageNodes()
		{
			if (!(_foldout[4] = EGL.Foldout(_foldout[4], "Image Nodes")))
				return;

			int imagenodeindex = 0;

			foreach (var go in _workingTemplate.ImageNodes)
			{
				GUILayout.Label("" + imagenodeindex);
				using (new GroupHorizontal())
				{
					if (GUILayout.Button("Select"))
						Selection.objects = new GameObject[] { go };
					if (GUILayout.Button("Delete"))
					{
						_workingTemplate.ImageNodes.Remove(go);
						DestroyImmediate(go);
					}
				}

				// stretch mode
				// add anchor stuff

				imagenodeindex++;
			}
			if (GUILayout.Button("Add Node"))
			{
				_workingTemplate.AddImageNode();
			}
		}

		#endregion
	}
}
