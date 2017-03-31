using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Hex = HWTools.Shapes.Hexagon2D;

/*
 * GridTransform
 * A transform relative to a Grid2D. When attached as a component, allows the GameObject to be
 * moved and transformed relative to the parent Grid2D.
 * 
 * Supports both Square and Hex (Hexagon) Mode.
 */

namespace HWTools.Grid
{
	/// <summary>
	/// Models a position relative to a grid space
	/// </summary>
	[ExecuteInEditMode]
	public class GridTransform : MonoBehaviour
	{
		#region Public Fields

		/// <summary>
		/// The grid this transform is associated with
		/// </summary>
		public Grid2D parent;

		#endregion

		#region Private Fields

		/// <summary>
		/// The stored position of the transform
		/// </summary>
		[SerializeField, HideInInspector]
		Vector3 _storedPos;

		#endregion

		#region Public Properties

		/// <summary>
		/// Convert the local position of the transform to grid position
		/// </summary>
		public Vector3 FromLocalPosition
		{
			get
			{
				// If not attached to grid, produce error, return 0 vector.
				if (!parent)
				{
					NoParentWarning();
					return Vector3.zero;
				}

				switch (parent.mode)
				{
					default:
						// Square Mode
						return transform.localPosition / parent.tileEdgeLength;

					case Grid2D.Mode.Hexagon:
						var h = new Hex { EdgeLength = parent.tileEdgeLength };

						float x = transform.localPosition.x,
								y = transform.localPosition.y;

						float q = x / (1.5f) / h.EdgeLength;

						float r = (-x / 3 + Hex.InDiameterRatio / 3 * y) / h.EdgeLength;



						return new Vector3(q, r, transform.localPosition.z / h.EdgeLength);
				}

			}
		}

		/// <summary>
		/// The position of the transform in grid space
		/// </summary>
		public Vector3 GridPosition
		{
			get
			{
				return _storedPos;
			}
			set
			{
				_storedPos = value;

				SetLocalPosition();
			}
		}

		/// <summary>
		/// Third axis for hex positioning
		/// </summary>
		public float GridPosS
		{
			get
			{
				return _storedPos.x - _storedPos.y;
			}
			set
			{
				float difference = GridPosS - value;

				var newX = _storedPos.x - difference;
				var newY = _storedPos.y + difference;

				GridPosition = new Vector3(newX, newY, _storedPos.z);
			}
		}

		/// <summary>
		/// Whether to snap the object to the grid.
		/// </summary>
		public bool Snap { get; set; }

		#endregion

		#region Public Methods

		/// <summary>
		/// Evaluates the distance between two GridTransforms, measured in grid units
		/// <para/>
		/// Returns null if the transforms do not belong to the same grid
		/// </summary>
		/// <param name="other">The other GridTransform</param>
		/// <returns>The computed distance</returns>
		public float? GridDistance(GridTransform other)
		{
			if (other.parent != parent)
				return null;

			switch (parent.mode)
			{
				default: //Square
					return
						(other.transform.localPosition -
						transform.localPosition).magnitude /
						parent.tileEdgeLength;
				case Grid2D.Mode.Hexagon:
					return
						(other.transform.localPosition -
						transform.localPosition).magnitude /
						(parent.tileEdgeLength * Hex.InDiameterRatio);
			}
		}

		/// <summary>
		/// Assigns a parent grid
		/// </summary>
		/// <param name="r">The grid to assign as parent</param>
		public void SetReference(Grid2D r)
		{
			transform.SetParent(r.transform);
			parent = r;

			if ((GridPosition - FromLocalPosition).magnitude > 0.001f)
			{
				_storedPos = FromLocalPosition;
			}
		}

		/// <summary>
		/// Searches for and assigns a grid to this GridTransform
		/// </summary>
		/// <returns>If connect was successful</returns>
		public bool findGrid()
		{
			var grid = GameObject.FindObjectOfType<Grid2D>();
			if (grid != null)
			{
				this.SetReference(grid);
				return true;
			}
			return false;
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Issues a debug warning that the parent grid has not been set
		/// </summary>
		void NoParentWarning()
		{
			Debug.LogError(this + ": Parent grid not set!");
		}

		/// <summary>
		/// Sets local position to match grid position
		/// </summary>
		void SetLocalPosition()
		{
			if (parent)
			{
				Vector3 input = Snap ?
					_storedPos.V3Round(Axis.X | Axis.Y)
					: _storedPos;

				switch (parent.mode)
				{

					default:
						{
							// Square Grid
							transform.localPosition = input * parent.tileEdgeLength;
							break;
						}
					case Grid2D.Mode.Hexagon:
						{
							var h = new Hex { EdgeLength = parent.tileEdgeLength };
							float q = input.x * h.EdgeLength * 1.5f;
							float r =
								(input.y * 2 +
								input.x) * h.InscribeRadius;
							float z = _storedPos.z * h.EdgeLength * Hex.InDiameterRatio;

							if (transform.localPosition.z == z &&
								q == transform.localPosition.x &&
							Mathf.Abs(r - transform.localPosition.y) < 0.00001f)
							{
								return; // avoid floating point drift
							}

							transform.localPosition =
								new Vector3(q, r, z);
							break;
						}
				}
			}
			else
			{
				NoParentWarning();
			}
		}

		/// <summary>
		/// Initializes the GridTransform
		/// <para/>
		/// Unity callback.
		/// </summary>
		void Awake()
		{
			if (!parent)
			{
				Grid2D parentToAttach;
				if (parentToAttach = GetComponentInParent<Grid2D>())
				{
					SetReference(parentToAttach);
				}
				else if (parentToAttach = GameObject.Find("Grid").GetComponent<Grid2D>())
				{
					SetReference(parentToAttach);
				}
				else
				{
					Debug.LogError("Unable find Parent!");
				}
			}

			if (parent && (_storedPos - FromLocalPosition).magnitude > .001f)
				_storedPos = FromLocalPosition;
		}



#if UNITY_EDITOR

		/// <summary>
		/// Currently not useful.
		/// <para/>
		/// Unity callback.
		/// </summary>
		void Update()
		{
			if (Snap && transform.hasChanged)
			{
				GridPosition = GridPosition;
				transform.hasChanged = false;
			}
		}

#endif

		#endregion
	}
}
