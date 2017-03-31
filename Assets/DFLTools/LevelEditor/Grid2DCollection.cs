using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HWTools.Grid
{
	/// <summary>
	/// Manages a set of objects associated with a Grid2D
	/// </summary>
	[RequireComponent(typeof(Grid2D))]
	public class Grid2DCollection : MonoBehaviour, ISerializationCallbackReceiver
	{
		#region Private Fields

		/// <summary>
		/// The grid this collection is associated with
		/// </summary>
		[SerializeField, HideInInspector]
		Grid2D _grid;

		/// <summary>
		/// The serializable representation of positions of objects in this collection
		/// </summary>
		[SerializeField, HideInInspector]
		List<Vector2> _positions;

		/// <summary>
		/// The serializable representation of objects in this collection
		/// </summary>
		[SerializeField, HideInInspector]
		List<GameObject> _tiles;
		
		/// <summary>
		/// The runtime collection of objects in this collection
		/// </summary>
		Dictionary<Vector2, GameObject> _tileSet;

		#endregion

		#region Public Properties

		/// <summary>
		/// An enumeration of all objects contained by this collection
		/// </summary>
		public IEnumerable<KeyValuePair<Vector2, GameObject>> Enumerable
		{
			get { return TileSet.Where(v => v.Value != null); }
		}

		public IEnumerable<GridTransform>LooseOccupants
		{
			// todo implement subscribable LooseOccupants list
			get { return transform.GetComponentsInChildren<GridTransform>(); }
		}

		/// <summary>
		/// Gets the Grid2D associated with this collection.
		/// <para/>
		/// Initializes if null.
		/// </summary>
		public Grid2D Grid
		{
			get
			{
				if (!_grid)
				{
					_grid = gameObject.GetComponent<Grid2D>();
				}
				return _grid;
			}
		}

		#endregion

		#region Private Properties

		/// <summary>
		/// Gets the runtime set of objects in the collection.
		/// <para/>
		/// Initializes if null.
		/// </summary>
		Dictionary<Vector2, GameObject> TileSet
		{
			get
			{
				if (_tileSet == null)
				{
					_tileSet = new Dictionary<Vector2, GameObject>();
				}
				return _tileSet;
			}
		}

		#endregion

		#region Public Indexers

		/// <summary>
		/// Indexes an object in the collection.
		/// </summary>
		/// <param name="x">X index</param>
		/// <param name="y">Y index</param>
		/// <returns></returns>
		public GameObject this[int x, int y]
		{
			get
			{
				Vector2 index = new Vector2(x, y);
				GameObject result;
				TileSet.TryGetValue(index, out result);
				return result;
			}
			set
			{
				Vector2 index = new Vector2(x, y);

				{
					GameObject prev;
					if (TileSet.TryGetValue(index, out prev))
					{
						DestroyImmediate(prev);
						TileSet.Remove(index);
					}
				}

				TileSet.Add(index, value);
			}
		}

		/// <summary>
		/// Indexes an object in the collection.
		/// </summary>
		/// <param name="index2D">The 2D index</param>
		/// <param name="round">Whether to round the index value</param>
		/// <returns></returns>
		public GameObject this[Vector2 index2D, bool round = true]
		{
			get
			{
				var index = round ? index2D.V2Round() : index2D;
				GameObject result;
				TileSet.TryGetValue(index, out result);
				return result;
			}
			set
			{
				Vector2 index = round ? index2D.V2Round() : index2D;

				{
					GameObject prev;
					if (TileSet.TryGetValue(index, out prev))
					{
						DestroyImmediate(prev);
						TileSet.Remove(index);
					}
				}

				TileSet.Add(index, value);
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Acquires the keys of neighboring objects
		/// </summary>
		/// <param name="root">The 2D index to evaluate</param>
		/// <param name="validOnly">Whether to filter out empty spaces</param>
		/// <returns></returns>
		public IEnumerable<Vector2> NeighborKeys(Vector2 root, bool validOnly = true)
		{
			Vector2 rounded = root.V2Round();
			if(!validOnly)
			{
				foreach (var d in Grid.mode.Directions())
				{
					yield return rounded + d.Vec2();
				}
			}
			else
			{
				GameObject test = null;
				foreach (var d in Grid.mode.Directions())
				{
					Vector2 result = rounded + d.Vec2();
					if (TileSet.TryGetValue(result, out test) && test)
					{
						yield return result;
					}
				}
			}
		}

		public IEnumerable<Vector2> SweepKeys(Vector2 root, Direction fromDir, bool validOnly = true)
		{
			Vector2 rounded = root.V2Round();
			if (!validOnly)
			{
				foreach (var d in fromDir.Sweep(Grid.mode))
				{
					yield return rounded + d.Vec2();
				}
			}
			else
			{
				GameObject test = null;
				foreach (var d in fromDir.Sweep(Grid.mode))
				{
					Vector2 result = rounded + d.Vec2();
					if (TileSet.TryGetValue(result, out test) && test)
					{
						yield return result;
					}
				}
			}
		}

		/// <summary>
		/// Reads serializable data into runtime data
		/// <para/>
		/// Method implemented for ISerializationCallbackReceiver
		/// </summary>
		public void OnAfterDeserialize()
		{
			TileSet.Clear();

			for (int i = 0; i < _positions.Count; i++)
			{
				TileSet.Add(_positions[i], _tiles[i]);
			}
		}

		/// <summary>
		/// Writes runtime data into serializable data
		/// <para/>
		/// Method implemented for ISerializationCallbackReceiver
		/// </summary>
		public void OnBeforeSerialize()
		{
			if (_positions == null)
			{
				_positions = new List<Vector2>();
			}
			if (_tiles == null)
			{
				_tiles = new List<GameObject>();
			}

			_positions.Clear();
			_tiles.Clear();

			foreach (var pair in TileSet)
			{
				if(pair.Value == null)
				{
					continue;
				}
				_positions.Add(pair.Key);
				_tiles.Add(pair.Value);
			}
		}

		/// <summary>
		/// Returns a Vector2 containing the minimum x and y Grid-Space coordinates among tiles contained within the dictionary.
		/// </summary>
		/// <returns></returns>
		public Vector2 getMinKeyVals()
		{
			Vector2 minKeys = new Vector2(float.MaxValue, float.MaxValue);
			foreach (var pair in TileSet)
			{
				if (pair.Value == null)
				{
					continue;
				}
				minKeys.x = Mathf.Min(minKeys.x, pair.Key.x);
				minKeys.y = Mathf.Min(minKeys.y, pair.Key.y);
			}
			return minKeys;
		}

		/// <summary>
		/// Returns a Vector2 containing the Maximum x and y Grid-Space coordinates among tiles contained within the dictionary.
		/// </summary>
		/// <returns></returns>
		public Vector2 getMaxKeyVals()
		{
			Vector2 maxKeys = new Vector2(float.MinValue, float.MinValue);
			foreach (var pair in TileSet)
			{
				if (pair.Value == null)
				{
					continue;
				}
				maxKeys.x = Mathf.Max(maxKeys.x, pair.Key.x);
				maxKeys.y = Mathf.Max(maxKeys.y, pair.Key.y);
			}
			return maxKeys;
		}

		//TODO: Adapt these functions for general use
		public Vector2 getSouthernmostTilePos()
		{
			Vector2 minXminYPos = Vector2.zero;
			// the "N-to-S value of a square (in isometric mode).
			float minXminYVal = float.MaxValue;
			float tempVal;
			foreach (var pair in TileSet)
			{
				if (pair.Value == null)
				{
					continue;
				}
				tempVal = pair.Key.x + pair.Key.y;
				//if tempVal less, this GameObject is further South in the Game Grid
				if (tempVal < minXminYVal)
				{
					minXminYVal = tempVal;
					minXminYPos = pair.Key;
				}
			}
			return minXminYPos;
		}

		//TODO: Adapt these functions for general use
		public Vector2 getNorthernmostTilePos()
		{
			Vector2 maxXmaxYPos = Vector2.zero;
			// the "N-to-S value of a square (in isometric mode).
			float maxXmaxYVal = float.MinValue;
			float tempVal;
			foreach (var pair in TileSet)
			{
				if (pair.Value == null)
				{
					continue;
				}
				tempVal = pair.Key.x + pair.Key.y;
				//if tempVal greater, this GameObject is further North in the Game Grid that previously examined tiles
				if (tempVal > maxXmaxYVal)
				{
					maxXmaxYVal = tempVal;
					maxXmaxYPos = pair.Key;
				}
			}
			return maxXmaxYPos;
		}

		//TODO: Adapt these functions for general use
		public Vector2 getEasternmostTilePos()
		{
			Vector2 maxXminYPos = Vector2.zero;
			// the "N-to-S value of a square (in isometric mode).
			float maxXminYVal = float.MinValue;
			float tempVal;
			foreach (var pair in TileSet)
			{
				if (pair.Value == null)
				{
					continue;
				}
				// because negative y counts towards being east, postive y should reduce from easternmost value while negative y increases
				tempVal = pair.Key.x - pair.Key.y;
				//if tempVal greater, this GameObject is further North in the Game Grid that previously examined tiles
				if (tempVal > maxXminYVal)
				{
					maxXminYVal = tempVal;
					maxXminYPos = pair.Key;
				}
			}
			return maxXminYPos;
		}

		//TODO: Adapt these functions for general use
		public Vector2 getWesternmostTilePos()
		{
			Vector2 minXmaxYPos = Vector2.zero;
			// the "N-to-S value of a square (in isometric mode).
			float minXmaxYVal = float.MinValue;
			float tempVal;
			foreach (var pair in TileSet)
			{
				if (pair.Value == null)
				{
					continue;
				}
				// because negative x counts towards being west, postive x should reduce from easternmost value while negative x increases
				tempVal = pair.Key.y - pair.Key.x;
				//if tempVal greater, this GameObject is further North in the Game Grid that previously examined tiles
				if (tempVal > minXmaxYVal)
				{
					minXmaxYVal = tempVal;
					minXmaxYPos = pair.Key;
				}
			}
			return minXmaxYPos;
		}
		#endregion
	}
}
