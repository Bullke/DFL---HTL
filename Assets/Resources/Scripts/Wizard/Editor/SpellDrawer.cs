using HTLWizards;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Spell))]
public class SpellDrawer : PropertyDrawer
{
	#region Public Fields

	public bool foldout = true;

	#endregion

	#region Public Methods

	/// <summary>
	/// Render the Spell's properties 
	/// </summary>
	/// <param name="property"></param>
	/// <param name="label">   </param>
	public void DrawContents(SerializedProperty property, GUIContent label)
	{
		string name = property.FindPropertyRelative("displayName").stringValue;
		
		GUIContent topLevelLabel = new GUIContent
		{
			tooltip = label.tooltip,
			image = label.image,
			text = label.text + " [" + name + "]"
		};
		foldout = EditorGUILayout.Foldout(foldout, topLevelLabel);

		if (foldout)
		{
			EditorGUI.indentLevel++;

			var nameLabel = new GUIContent { text = "Name" };
			EditorGUILayout.PropertyField(property.FindPropertyRelative("displayName"), nameLabel);

			var animLabel = new GUIContent { text = "Animation" };
			EditorGUILayout.PropertyField(property.FindPropertyRelative("animControl"), animLabel);

			var targetLabel = new GUIContent { text = "Targeting Style" };
			EditorGUILayout.PropertyField(property.FindPropertyRelative("targetingStyle"), targetLabel);
			var durationLabel = new GUIContent { text = "Cooldown" };
			EditorGUILayout.PropertyField(property.FindPropertyRelative("cooldown"), durationLabel);

			var effectLabel = new GUIContent { text = "Effect" };
			EditorGUILayout.PropertyField(property.FindPropertyRelative("effect"), effectLabel);

			EditorGUI.indentLevel--;
		}
	}

	/// <summary>
	/// Unity callback 
	/// </summary>
	/// <param name="property"></param>
	/// <param name="label">   </param>
	/// <returns></returns>
	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return 0;
	}

	/// <summary>
	/// Unity callback 
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