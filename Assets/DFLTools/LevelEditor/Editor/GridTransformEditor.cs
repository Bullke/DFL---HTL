using HWTools;
using HWTools.Edit;
using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using EGL = UnityEditor.EditorGUILayout;

/*
 *  GridTransform Editor
 *  Class that overrides the default Unity Inspector GUI when inspecting a GridTransform Component
 *  Allows the user to automatically attach the GridTransform to a Grid2D,
 *   snap the transform to the Grid2D, and manipulate the GameObject such that it moves about the attached Grid2D 
 */

namespace HWTools.Grid
{

	/// <summary>
	/// Provides inspector behavior for GridTransform components
	/// </summary>
	[CustomEditor(typeof(GridTransform))]
	public class GridTransformEditor : Editor
	{
		#region Private Fields

		/// <summary>
		/// The grid transform being modified
		/// </summary>
		GridTransform gt;

		#endregion

		#region Public Methods

		/// <summary>
		/// Renders inspector GUI
		/// <para/>
		/// Unity callback function
		/// </summary>
		public override void OnInspectorGUI()
		{

			while (UnityEditorInternal.ComponentUtility.MoveComponentUp(gt))
			{
				//Move the GridTransform component to the top of the inspector
			}

			if (gt.parent)
            {
                // Button Toggle: if activated, GridTransform will Snap to the Grid2D's tile's corners
                gt.Snap = GUILayout.Toggle(gt.Snap, "Snap", "Button");

                // Display Vector3 containing current GridPosition
                gt.GridPosition = EGL.Vector3Field("Grid position", gt.FromLocalPosition);

                // If in Hexagon mode, also display 3rd Hex Axis
                if (gt.parent.mode == Grid2D.Mode.Hexagon)
				{
					gt.GridPosS = EGL.FloatField("+X-Y Axis", gt.GridPosS);
				}
			}
            // Else GridTransform not connected to a Grid2D
            else
            {
				EGL.HelpBox("This object must be attached to a grid", MessageType.Error);
				
			}

            // Button Finds and connects the object's GridTransform to a Grid2D
            if (GUILayout.Button("Find grid"))
			{
				var p = GameObject.FindObjectOfType<Grid2D>();
				if (p)
				{
					gt.SetReference(p);
				}
			}

		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Logic to run when the window is enabled
		/// <para/>
		/// Unity callback method
		/// </summary>
		void OnEnable()
		{
			gt = target as GridTransform;
			//gt.GetComponent<Transform>().hideFlags = HideFlags.HideInInspector;
		}

		/// <summary>
		/// Logic to run when the window is disabled
		/// <para/>
		/// Unity callback method
		/// </summary>
		void OnDisable()
		{
			//gt.GetComponent<Transform>().hideFlags = HideFlags.None;
		}

		#endregion
	}
}
