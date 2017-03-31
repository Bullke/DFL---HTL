using HTLWizards;
using HWTools.Edit;
using UnityEditor;
using UnityEngine;
using EGL = UnityEditor.EditorGUILayout;


/// SpellEffectDrawer
/// <summary>
/// Exposes the properties of a SpellEffect for modification within the editor
/// </summary>
[CustomPropertyDrawer(typeof(SpellEffect))]
public class SpellEffectDrawer : PropertyDrawer
{
	#region Public Fields

	public bool foldout = true;

	#endregion

	#region Public Methods

	/// <summary>
	/// Render the SpellEffect's properties
	/// </summary>
	/// <param name="property"></param>
	/// <param name="label"></param>
	public void DrawContents(SerializedProperty property, GUIContent label)
	{

		GUIContent topLevelLabel = new GUIContent
		{
			tooltip = label.tooltip,
			image = label.image,
			text = label.text
		};
		foldout = EGL.Foldout(foldout, topLevelLabel);

		if (foldout)
		{
			EditorGUI.indentLevel++;

			var idLabel = new GUIContent { text = "ID" };
			EGL.PropertyField(property.FindPropertyRelative("id"), idLabel);

			var statLabel = new GUIContent { text = "Modified Stat" };
			EGL.PropertyField(property.FindPropertyRelative("targetStatKey"), statLabel);

			var durationProperty = property.FindPropertyRelative("durationTimer");

			var durationLabel = new GUIContent { text = "Duration" };
			EGL.PropertyField(durationProperty.FindPropertyRelative("_end"), durationLabel);

			durationProperty // Make sure timer doesn't loop
				.FindPropertyRelative("_loop").boolValue = false;

			var modeProperty = property.FindPropertyRelative("mode");

			var modeLabel = new GUIContent { text = "Mode" };
			EGL.PropertyField(modeProperty, modeLabel);


			var startEffectLabel = new GUIContent { text = "Initial Effect" };

			if (modeProperty.enumNames[modeProperty.enumValueIndex] == "Modifier")
			{
				startEffectLabel.text = "Modifier";

				var startEffectProp = property.FindPropertyRelative("start");

				startEffectProp.FindPropertyRelative("enabled").boolValue = true;

				EditorGUI.indentLevel++;

				var magLabel = new GUIContent { text = "Magnitude" };
				EGL.PropertyField(startEffectProp.FindPropertyRelative("magnitude"), startEffectLabel);

				var modifierModeLabel = new GUIContent { text = "Modifier Mode" };
				EGL.PropertyField(startEffectProp.FindPropertyRelative("mode"), modifierModeLabel);

				EditorGUI.indentLevel--;

				property
					.FindPropertyRelative("tick")
					.FindPropertyRelative("enabled").boolValue = false;
				property
					.FindPropertyRelative("end")
					.FindPropertyRelative("enabled").boolValue = false;

			}
			else
			{
				EGL.PropertyField(property.FindPropertyRelative("start"), startEffectLabel);


				var tickEffectLabel = new GUIContent { text = "Per-Second Effect" };
				var endEffectLabel = new GUIContent { text = "End Effect" };

				EGL.PropertyField(property.FindPropertyRelative("tick"), tickEffectLabel);
				EGL.PropertyField(property.FindPropertyRelative("end"), endEffectLabel);

			}


			EditorGUI.indentLevel--;
		}
	}

	/// <summary>
	///   Unity callback 
	/// </summary>
	/// <param name="property"></param>
	/// <param name="label">   </param>
	/// <returns></returns>
	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return 0;
	}

	/// <summary>
	///   Unity callback 
	/// </summary>
	/// <param name="position"></param>
	/// <param name="property"></param>
	/// <param name="label">   </param>
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		DrawContents(property, label);
	}

	#endregion
}


[CustomPropertyDrawer(typeof(SpellEffect.FloatEffect))]
public class FloatEffectDrawer : PropertyDrawer
{
	#region Public Fields

	public bool foldout;

	#endregion

	#region Public Methods

	/// <summary>
	/// Render the SpellEffect's properties
	/// </summary>
	/// <param name="property"></param>
	/// <param name="label"></param>
	public void DrawContents(SerializedProperty property, GUIContent label)
    {
        bool enabled = property.FindPropertyRelative("enabled").boolValue;
        GUIContent topLevelLabel = new GUIContent
		{
			tooltip = label.tooltip,
			image = label.image,
			text = (enabled ? "☑ " : "☐ ") + label.text
		};
		foldout = EGL.Foldout(foldout, topLevelLabel);

		if (foldout)
		{
			EditorGUI.indentLevel++;

			var enabledLabel = new GUIContent { text = "Enabled" };
			EGL.PropertyField(property.FindPropertyRelative("enabled"), enabledLabel);

			using (new GroupConditional(property.FindPropertyRelative("enabled").boolValue))
			{
				var modeLabel = new GUIContent { text = "Mode" };
				EGL.PropertyField(property.FindPropertyRelative("mode"), modeLabel);

				var magLabel = new GUIContent { text = "Magnitude" };
				EGL.PropertyField(property.FindPropertyRelative("magnitude"), magLabel);
			}

			EditorGUI.indentLevel--;
		}
	}

	/// <summary>
	///   Unity callback 
	/// </summary>
	/// <param name="property"></param>
	/// <param name="label">   </param>
	/// <returns></returns>
	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return 0;
	}

	/// <summary>
	///   Unity callback 
	/// </summary>
	/// <param name="position"></param>
	/// <param name="property"></param>
	/// <param name="label">   </param>
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		DrawContents(property, label);
	}

	#endregion
}