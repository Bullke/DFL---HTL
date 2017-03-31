using HWTools;
using HWTools.Edit;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using EGL = UnityEditor.EditorGUILayout;

namespace PopupTool
{

	/// <summary>
	/// Provides inspector behavior for PopupAssistant components
	/// </summary>
	[CustomEditor(typeof(PopupAssistant))]
	public class PopupAssistantEditor : Editor
	{
		#region Private Fields

		/// <summary>
		/// Image nodes attached to popup
		/// </summary>
		GameObject[] _nodes;

		/// <summary>
		/// The popup being modified
		/// </summary>
		PopupAssistant _pa;

		#endregion

		#region Public Methods

		/// <summary>
		/// Renders the PopupAssistant inspector GUI
		/// <para/>
		/// Unity callback method
		/// </summary>
		public override void OnInspectorGUI()
		{
			
			_pa.startHidden = EGL.Toggle("Start Hidden", _pa.startHidden);

			Text t = _pa.transform.FindChild("TextBox").GetComponent<Text>();

			EGL.LabelField("Popup text:", EditorStyles.boldLabel);
			t.text = EGL.TextArea(t.text);
			EditorUtility.SetDirty(t);

			EGL.LabelField("Button properties:", EditorStyles.boldLabel);

			Button b = _pa.transform.FindChild("Button").GetComponent<Button>();

			Text bt = b.GetComponentInChildren<Text>();
			bt.text = EGL.TextField(bt.text);
			EditorUtility.SetDirty(bt);

			var sb = new SerializedObject(b);
			sb.Update();

			EGL.PropertyField(sb.FindProperty("m_OnClick"));
			sb.ApplyModifiedProperties();

			EditorUtility.SetDirty(_pa);

			if (_nodes.Length > 0)
			{
				EGL.LabelField(_nodes.Length == 1 ? "Image:" : "Images:",
					EditorStyles.boldLabel);
				foreach (var n in _nodes)
				{
					var i = n.GetComponent<Image>();
					i.sprite = HWEdLayout.ObjectField(i.sprite, false);
					EditorUtility.SetDirty(i);
				}
			}

			
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Sets up the PopupAssistant inspector
		/// <para/>
		/// Unity callback method
		/// </summary>
		void OnEnable()
		{
			_pa = target as PopupAssistant;

			_nodes = (from i in _pa.transform.Children()
					 where i.name.StartsWith("Image", StringComparison.OrdinalIgnoreCase)
					 select i.gameObject).ToArray();
		}

		#endregion
	}
}
