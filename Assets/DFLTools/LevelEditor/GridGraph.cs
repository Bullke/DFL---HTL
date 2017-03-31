using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HWTools.Grid
{
	/// <summary>
	///   Models a graph built upon a grid 
	///	  <para/>
	///   Unfinished and likely to change.
	/// </summary>
	[RequireComponent(typeof(Grid2DCollection))]
	public partial class GridGraph : MonoBehaviour
	{
		#region Private Fields

		Dictionary<Vector2, Vector2[]> _corners;
		Dictionary<Vector2, Vector2[]> _links;
		Grid2DCollection _parent;

		#endregion

		#region Private Properties

		Grid2DCollection Collection
		{
			get
			{
				if (!_parent)
				{
					_parent = GetComponent<Grid2DCollection>();
				}
				return _parent;
			}
		}

		Dictionary<Vector2, Vector2[]> Links
		{
			get
			{
				if (_links == null)
				{
					_links = new Dictionary<Vector2, Vector2[]>();
				}
				return _links;
			}
		}

		Dictionary<Vector2, Vector2[]> Corners
		{
			get
			{
				if (_corners == null)
				{
					_corners = new Dictionary<Vector2, Vector2[]>();
				}
				return _corners;
			}
		}

		#endregion

		#region Private Methods

		void Populate()
		{
			Links.Clear();

			var paths = Collection.Enumerable.Where(t => t.Value.GetComponent<BasicPath>());

			foreach (var v in paths.Select(p => p.Key))
			{
				UpdateConnections(v);
			}
		}

		void UpdateConnections(Vector2 node)
		{
			var sample = Collection.NeighborKeys(node);

			Links[node] = sample.Where(v2 => Collection[v2].GetComponent<BasicPath>()).ToArray();

		}

		void PopulateCorners()
		{
			var borders = new List<List<Vector2>>();

			


		}

		#endregion
	}
}
