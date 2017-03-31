using HWTools;
using HWTools.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace PopupTool
{
	public partial class PopupTemplate
	{
		#region Public Methods

		/// <summary>
		/// Instantiates a popup based on this template
		/// </summary>
		/// <returns>The instantiated popup</returns>
		public GameObject EmitPopup()
		{
			var result = Instantiate(gameObject); // Clone this template

			result.name = "New Popup";

			DestroyImmediate(result.GetComponent<PopupTemplate>());

			result.AddComponent<CanvasGroup>();

			var assistant = result.AddComponent<PopupAssistant>();

			result.transform.FindChild("TextBox").GetComponent<Text>().text = "Enter Message";
			var bord = result.transform.FindChild("Border").GetComponent<Image>();

			if (bord.sprite == null)
			{
				DestroyImmediate(bord.gameObject);
			}

			// Set up image nodes
			var nodes = (from n in result.transform.Children()
						where n.name.StartsWith("ImageNode", StringComparison.OrdinalIgnoreCase)
						select n.gameObject).ToArray();

			assistant.imageNodes = nodes;

			result.transform.SetParent(FindObjectOfType<Canvas>().transform);

			result.CenterOnParent();

			return result;
		}

		#endregion
	}
}
