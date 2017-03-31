using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Timer))]
public class TimerDrawer : PropertyDrawer
{
	#region Public Fields

	public bool foldout;

	#endregion

	#region Public Methods

	/// <summary>
	/// Render the Timer's properties
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
		foldout = EditorGUILayout.Foldout(foldout, topLevelLabel);

		if (foldout)
		{
			EditorGUI.indentLevel++;

			var currentTimeLabel = new GUIContent { text = "Current time" };
			EditorGUILayout.PropertyField(property.FindPropertyRelative("_current"), currentTimeLabel);
			var lengthLabel = new GUIContent { text = "Length" };
			EditorGUILayout.PropertyField(property.FindPropertyRelative("_end"), lengthLabel);
			//var loopLabel = new GUIContent { text = "Loop" };
			//EditorGUILayout.PropertyField(property.FindPropertyRelative("_loop"), loopLabel);
			//var runWhilePausedLabel = new GUIContent { text = "Run while paused" };
			//EditorGUILayout.PropertyField(property.FindPropertyRelative("_runWhilePaused"), runWhilePausedLabel);

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
