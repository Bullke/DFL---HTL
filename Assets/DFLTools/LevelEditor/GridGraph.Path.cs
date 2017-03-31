using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HWTools.Grid
{
	public partial class GridGraph
	{
		#region Private Fields

		ILookup<int, Vector2> _byNeighborCount;
		List<GraphPath> _paths;

		#endregion

		#region Private Properties

		ILookup<int, Vector2> ByNeighborCount
		{
			get
			{
				if (_byNeighborCount == null)
				{
					UpdateNeighborCountLookup();
				}
				return _byNeighborCount;
			}
		}

		List<GraphPath> Paths
		{
			get
			{
				if (_paths == null)
				{
					_paths = new List<GraphPath>();
				}
				return _paths;
			}
		}

		#endregion

		#region Private Methods

		void GeneratePaths()
		{
			UpdateNeighborCountLookup();

			foreach (var startPoint in
				ByNeighborCount
				.Where(g => g.Key != 2) // Only interested in dead-ends and forks
				.SelectMany(g => g.Select(v => v)))
			{
				var paths = Links[startPoint]
					.Select(v => new GraphPath(this, startPoint, v));

				foreach (var p in paths)
				{

				}
			}
		}

		void UpdateNeighborCountLookup()
		{
			_byNeighborCount = Links.ToLookup(
				keySelector: e => e.Value.Length,
				elementSelector: e => e.Key);
		}

		#endregion

		#region Protected Classes

		protected class GraphPath
		{
			#region Public Fields

			public GridGraph host;

			#endregion

			#region Private Fields

			IDictionary<Vector2, float> _distances;

			List<Node> _nodes;

			#endregion

			#region Public Properties

			public float TotalCost { get; private set; }

			#endregion

			#region Private Properties

			IDictionary<Vector2, float> Distances
			{
				get
				{
					if (_distances == null)
					{
						CalculateDistance();
					}
					return _distances;
				}
			}

			List<Node> Nodes
			{
				get
				{
					if (_nodes == null)
					{
						_nodes = new List<Node>();
					}
					return _nodes;
				}
			}

			#endregion

			#region Public Constructors

			public GraphPath(GridGraph h, params Vector2[] init)
			{
				host = h;
				foreach (var v in init)
				{
					Nodes.Add(new Node { pos = v });
				}
			}

			#endregion

			#region Private Methods

			void CalculateDistance()
			{
				TotalCost = 0;
				foreach (var n in _nodes)
				{
				}
			}

			#endregion

			#region Private Structs

			struct Node
			{
				#region Public Fields

				public Vector2 pos;

				#endregion
			}

			#endregion
		}

		#endregion
	}
}
