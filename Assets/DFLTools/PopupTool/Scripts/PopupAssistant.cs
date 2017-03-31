using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace PopupTool
{
	/// <summary>
	///   This class provides a simplified interface for performing operations on a popup 
	/// </summary>
	[SelectionBase]
	public class PopupAssistant : MonoBehaviour
	{
		// todo make stuff timed
		// todo dissapear on click
		// click while visible bool calls button function

		#region Public Fields

		/// <summary>
		/// Image nodes attatched to the popup
		/// </summary>
		public GameObject[] imageNodes;

		/// <summary>
		/// Whether the popup starts hidden
		/// </summary>
		public bool startHidden;

		#endregion


		#region Public Properties

		/// <summary>
		/// The visibility of the popup
		/// </summary>
		public float Visiblility
		{
			get { return GetComponent<CanvasGroup>().alpha; }
			set
			{
				GetComponent<CanvasGroup>().alpha = value;
				GetComponent<CanvasGroup>().blocksRaycasts = value > 0;
			}
		}

		#endregion


		#region Public Methods

		/// <summary>
		/// Find a popup in the scene
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public static PopupAssistant Find(string name)
		{
			return GameObject.Find(name).GetComponent<PopupAssistant>();
		}
			
		/// <summary>
		/// Destroy the popup
		/// </summary>
		public void Destroy()
		{
			DestroyImmediate(gameObject);
		}

		/// <summary>
		/// Hide the popup
		/// </summary>
		public void Hide()
		{
			Visiblility = 0;
		}

		/// <summary>
		/// Show the popup
		/// </summary>
		public void Show()
		{
			Visiblility = 1;
		}

		#endregion


		#region Protected Methods

		/// <summary>
		/// Initializes the popup
		/// <para/>
		/// Unity callback method
		/// </summary>
		protected void Awake()
		{
			if (startHidden) Hide();
		}

		#endregion

	}
}
