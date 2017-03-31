using HWTools;
using HWTools.Reflection;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PopupTool
{

	/// <summary>
	/// A template for spawning popups
	/// </summary>
	[Serializable]
	[SelectionBase]
	public partial class PopupTemplate : UIBehaviour
	{
		#region Private Fields

		/// <summary>
		/// The offsets of the border's corners
		/// </summary>
		[SerializeField, HideInInspector]
		Rect _borderOffset;

		/// <summary>
		/// The image nodes attached to the template
		/// </summary>
		FEList<GameObject> _imageNodes;

		[SerializeField, HideInInspector]
		GameObject _textBox, _border, _button;

		#endregion

		#region Public Properties

		/// <summary>
		/// The border component of the template
		/// </summary>
		public GameObject Border
		{
			get
			{
				GuaranteeBorder();
				return _border;
			}
		}

		/// <summary>
		/// The offset of the border's upper right corner
		/// </summary>
		public Vector2 BorderMaxOffset
		{
			get { return _borderOffset.max; }
			set { _borderOffset.max = value; }
		}

		/// <summary>
		/// The offset of the border's lower left corner
		/// </summary>
		public Vector2 BorderMinOffset
		{
			get { return _borderOffset.min; }
			set { _borderOffset.min = value; }
		}

		/// <summary>
		/// The button component of the template
		/// </summary>
		public GameObject Button
		{
			get
			{
				GuaranteeButton();
				return _button;
			}
		}

		/// <summary>
		/// The button text of the template
		/// </summary>
		public GameObject ButtonText
		{
			get
			{
				GuaranteeButton();
				return _button.transform.GetChild(0).gameObject;
			}
		}

		/// <summary>
		/// The image nodes attatched to the template
		/// </summary>
		public FEList<GameObject> ImageNodes
		{
			get
			{
				CheckImageNodes();
				return _imageNodes;
			}
		}

		/// <summary>
		/// The text box of the template
		/// </summary>
		public GameObject TextBox
		{
			get
			{
				GuaranteeTextBox();
				return _textBox;
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Adds an image node to the template
		/// </summary>
		public void AddImageNode()
		{
			CheckImageNodes();
			var go = gameObject.CreateChild();
			go.AddComponentSet(TypeSets.ImageComponents);
			go.CenterOnParent();
			go.name = "ImageNode" + _imageNodes.Count;
			_imageNodes.Add(go);
		}

		/// <summary>
		/// Scans template for attached image nodes
		/// </summary>
		public void CheckImageNodes()
		{
			if (_imageNodes != null && _imageNodes.Count != 0)
				return;

			_imageNodes = new FEList<GameObject>();
			foreach (var t in transform.Children())
			{
				if (t.name.StartsWith("Image", StringComparison.OrdinalIgnoreCase))
				{
					_imageNodes.Add(t.gameObject);
				}
			}
		}

		/// <summary>
		/// Guarantees that the template has a border
		/// </summary>
		public void GuaranteeBorder()
		{
			if (!_border)
			{
				if (transform.FindChild("Border"))
				{
					_border = transform.FindChild("Border").gameObject;
				}
				else
				{
					_border = gameObject.CreateChild();
					_border.AddComponentSet(TypeSets.ImageComponents);
					_border.GetComponent<Image>().type = Image.Type.Tiled;
					_border.GetComponent<Image>().fillCenter = false;
					_border.GetComponent<Image>().raycastTarget = false;
				}
			}

			_border.name = "Border";
			_border.GetComponent<RectTransform>().StretchFill();
			_border.GetComponent<RectTransform>().offsetMax = BorderMaxOffset;
			_border.GetComponent<RectTransform>().offsetMin = BorderMinOffset;
		}

		/// <summary>
		/// Guarantees that the template has a button
		/// </summary>
		public void GuaranteeButton()
		{
			if (!_button)
			{
				var t = transform.FindChild("Button");
				if (t)
				{
					_button = t.gameObject;
				}
				else
				{
					_button = gameObject.CreateChild();
					_button.AddComponentSet(TypeSets.ImageComponents);
					_button.AddComponent<Button>();

					var rt = _button.GetComponent<RectTransform>();
					rt.anchorMax = new Vector2(1, 0);
					rt.anchorMin = new Vector2(1, 0);
					_button.CenterOnParent();
				}
			}
			GameObject tex;
			if (!_button.transform.FindChild("Text"))
			{
				tex = _button.CreateChild();
				tex.AddComponentSet(TypeSets.TextComponents);

				tex.GetComponent<RectTransform>().StretchFill();
				tex.CenterOnParent();

				tex.GetComponent<Text>().text = "Button";
				tex.GetComponent<Text>().color = Color.gray;
				tex.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
				tex.name = "Text";
			}
			else
			{
				tex = _button.transform.FindChild("Text").gameObject;
			}
			tex.name = "Text";
			_button.name = "Button";
		}

		/// <summary>
		/// Guarantees that the template has a text box
		/// </summary>
		public void GuaranteeTextBox()
		{
			if (!_textBox)
			{
				var t = transform.FindChild("TextBox");
				if (t)
				{
					_textBox = t.gameObject;
				}
				else
				{
					_textBox = gameObject.CreateChild();
					_textBox.AddComponentSet(TypeSets.TextComponents);
					_textBox.CenterOnParent();
					_textBox.GetComponent<Text>().color = Color.gray;
					_textBox.GetComponent<Text>().raycastTarget = false;
					_textBox.GetComponent<RectTransform>().StretchFill();
				}
			}
			_textBox.name = "TextBox";
		}

		#endregion

		#region Protected Methods

		/// <summary>
		/// Performs sanity checks when the dimensions of the RectTransform are changed
		/// <para/>
		/// Unity callback method
		/// </summary>
		protected override void OnRectTransformDimensionsChange()
		{
			base.OnRectTransformDimensionsChange();

			GuaranteeTextBox();
			GuaranteeBorder();
			GuaranteeButton();
		}

		#endregion
	}
}
