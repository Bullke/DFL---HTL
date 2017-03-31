using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Hex = HWTools.Shapes.Hexagon2D;


namespace HWTools.Grid.Deprecated
{
	/// <summary>
	/// CURRENTLY DEPRECATED
	/// </summary>
	public class GridTile : MonoBehaviour
	{
		// todo shape classes that provide

		public Grid2DCollection parent;

		[SerializeField,HideInInspector]
		Vector2 _key;

		[SerializeField, HideInInspector]
		List<GridTile> _neighbors;

		List<GridTile> Neighbors
		{
			get
			{
				if(_neighbors == null)
				{
					GetNeighbors();
				}
				return _neighbors;
			}
		}

		void GetNeighbors()
		{
			List<GridTile> results = new List<GridTile>();

			foreach (Vector2 k in parent.Grid.mode == Grid2D.Mode.Square ?
				_key.V2OffsetSet(Vector2.up, Vector2.right, Vector2.down, Vector2.left) :
				_key.V2OffsetSet(Vector2.up, Vector2.right, Vector2.right + Vector2.down,
				Vector2.down, Vector2.left, Vector2.up + Vector2.left))
			{
				GridTile gt = parent[k].GetComponent<GridTile>();
				if(gt)
				{
					results.Add(gt);
				}
			}

			_neighbors = results;
		}
	}
}
