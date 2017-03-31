using System;
using System.Collections.Generic;
using UnityEngine;

/*
 *  Grid2D
 *  A 2D Grid system that defines a "Grid Space" which GameObjects can interact with.
 *  The Grid2D works in 2.5D Space by rotating in 3D space and then projecting onto a 2D X-Y Plane.
 *  This allows for additional potential perspectives of the grid while still using 2D space and sprites.
 *  Objects can be associated with the Grid2D by giving them a GridTransform component.
 *  
 *  Each "Tile", or Grid Cell, is centered at a whole-number coordinate (0,0), (1,1,), etc. in grid space.
 *  
 *  The grid has Two Modes:
 *      Square Mode: Grid is comprised of Quadrilateral Tiles.
 *          Each Tile shares an edge with 4 other tiles.
 *          With no rotation, neighbors are Up, Down, Right, and Left
 *          Tiles are Isometric if Z rotation = 45
 *      Hexagon (Hex) Mode: Grid is comprised of Hexagon tiles.
 *          Each Tile shares an edge with 6 other tiles.
 *          With no rotation, neighbors are Up, Down, Upper Left, Upper Right, Lower Left, Lower Right
 */
 
namespace HWTools.Grid
{
	/// <summary>
	///   Defines a rotated coordinate system projected onto a 2D plane 
	/// </summary>
	public partial class Grid2D : MonoBehaviour
	{
		#region Public Enums

		/// <summary>
		/// Represents the shape of a grid
		/// </summary>
		public enum Mode
		{
			Square,
			Hexagon
		}

		#endregion

		#region Public Fields

		/// <summary>
		/// The color the grid should use when drawing guidelines
		/// </summary>
		public Color debugColor;

        /// <summary>
        /// When Level Editor is active, tiles within Radius Tile Spaces of the mouse cursor are highlighted
        /// </summary>
		public int debugRadius;

        /// <summary>
        /// Current type of Grid to display
        /// </summary>
        public Mode mode = Mode.Square;

        /// <summary>
        /// Dimensions of the Debug Grid.   
        /// Tile at 0,0 always drawn, size draws that many additional tiles in that direction
        /// </summary>
        public Vector2 size;

		/// <summary>
		/// The length of the edges of each tile 
		/// </summary>
		public float tileEdgeLength = 1;

		#endregion

		#region Private Properties

		/// <summary>
		/// Where on the grid to highlight
		/// </summary>
		Vector2? GizSelection { get; set; }

		#endregion

		#region Public Methods

		/// <summary>
		/// Set the cell on the grid to highlight with debug lines
		/// </summary>
		/// <param name="point">The point in grid space to select</param>
		public void SelectDebugGizmoCell(Vector2 point)
		{
			GizSelection = SelectTile(point);
		}

		/// <summary>
		///   Convert a point in 2D world space to the grid 
		///   <para/>
		///   Returns nullable value.
		/// </summary>
		/// <param name="worldPos">The point in world space to convert</param>
		/// <returns>The point in grid space that corresponds to the given point in world space</returns>
		public Vector2? InvertProjection2D(Vector2 worldPos)
		{
			var normal = transform.forward;
			if (Math.Abs(normal.z) < 0.00001f // plane is perpendicular
				|| tileEdgeLength == 0) // the tiles are infinitely small
			{
				return null; // cannot evaluate position, so return null
			}

			var relativetoroot = ((Vector3)worldPos - transform.position);

			// solve for z in the Cartesian coordinate system
			{
				float a = normal.x, b = normal.y, c = normal.z,
					x = relativetoroot.x, y = relativetoroot.y;

				float z = -(a * x + b * y) / c;

				relativetoroot.z = z;
			}

			return (Quaternion.Inverse(transform.rotation) * relativetoroot) / tileEdgeLength;
		}

		/// <summary>
		///   Convert a point on the grid to 2D world space 
		/// </summary>
		/// <param name="xy">The grid position</param>
		/// <returns>The point in world space corresponding to the given grid position</returns>
		public Vector2 Projection2D(Vector2 xy)
		{
			return Projection2D(xy.x, xy.y);
		}

		/// <summary>
		/// Get the center of a tile, given a 2D index
		/// </summary>
		/// <param name="tile">The 2D index</param>
		/// <returns>The center of the tile in grid space. Returns null if tile is null</returns>
		public Vector2? SelectCenter(Vector2? tile)
		{
			var result = tile;
			
			if (!result.HasValue)
				return null;

			if (mode == Mode.Hexagon)
			{
				result = FromHexCoord(result.Value);
			}
			return result;
		}

		/// <summary>
		/// Find a tile given a world position.
		/// <para/>
		/// Returns nullable value
		/// </summary>
		/// <param name="worldPos"></param>
		/// <returns></returns>
		public Vector2? SelectTile(Vector2 worldPos)
		{
			var sample = InvertProjection2D(worldPos);

			if (sample.HasValue)
			{
				switch (mode)
				{
					case Mode.Square:
						return sample.Value.V2Round();

					case Mode.Hexagon:
						return SelectHex(sample.Value);
				}
			}

			return null;
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Render a cell on the grid using Gizmo lines
		/// </summary>
		/// <param name="x">X-index</param>
		/// <param name="y">Y-index</param>
		/// <param name="color">Optional color for the tile.</param>
		void GizmoSingleTile(float x, float y, Color? color = null)
		{
			Color prev = Gizmos.color;

			if (color.HasValue)
			{
				Gizmos.color = color.Value;
			}

            // Calculate the world space position of each of the cell's corners
            // Each cell centered at (x, y) in Grid Space
			x -= .5f;
			y -= .5f;

			var ll = Projection2D(x, y);
			var lr = Projection2D(x + 1, y);
			var ul = Projection2D(x, y + 1);
			var ur = Projection2D(x + 1, y + 1);

			Gizmos.DrawLine(ll, lr);
			Gizmos.DrawLine(lr, ur);
			Gizmos.DrawLine(ur, ul);
			Gizmos.DrawLine(ul, ll);

			Gizmos.color = prev;
		}

		/// <summary>
		/// Draw the shape of the grid
		/// <para/>
		/// Overridden Unity callback
		/// </summary>
		void OnDrawGizmos()
		{
			if (mode == Mode.Square)
			{
                // Draw the full grid
				foreach (int i in 0.To((int)size.x))
				{
					foreach (int j in 0.To((int)size.y))
					{
						GizmoSingleTile(i, j, debugColor.RandMult(0.7f));
					}
				}

                // If Mouse pointer detected, highlight tiles within debug radius in Cyan
                // Then highlight the tile containing the mouse pointer in Green
				if (GizSelection.HasValue)
				{
					foreach (var h in SelectRadius(GizSelection, debugRadius))
					{
						GizmoSingleTile(h.x, h.y, Color.cyan.RandMult(0.6f));
					}

					GizmoSingleTile(GizSelection.Value.x, GizSelection.Value.y, Color.green);
				}
			}

            // Else Hexagon Mode
			else
			{
                // Draw the full grid
                foreach (int i in 0.To((int)size.x))
				{
					foreach (int j in 0.To((int)size.y))
					{
						GizmoSingleHex(new Vector2(i, j), debugColor.RandMult(0.7f));
					}
				}

                // If Mouse pointer detected, highlight tiles within debug radius in Cyan
                // Then highlight the tile containing the mouse pointer in Green
                if (GizSelection.HasValue)
				{
					foreach (var h in SelectRadius(GizSelection, debugRadius))
					{
						GizmoSingleHex(h, Color.cyan.RandMult(0.6f));
					}
					GizmoSingleHex(GizSelection.Value, Color.green);
				}
			}
			//GizSelection = null;
		}

        /// <summary>
        /// Convert from Grid Space to 2D World Space
        /// </summary>
        /// <param name="x">X-index</param>
        /// <param name="y">Y-index</param>
        /// <returns>A Vector2 in World space</returns>
		Vector2 Projection2D(float x, float y)
		{
			var result = transform.position +
				(transform.rotation * new Vector3(x, y, 0) * tileEdgeLength);

			// z is discarded in Vector2 conversion
			return result;
		}

		/// <summary>
		/// Enumerates a set of tiles in a radius around a given tile.
		/// <para/>
		/// If tilePos is null, returns an empty enumeration.
		/// </summary>
		/// <param name="tilePos">The center tile of the selection</param>
		/// <param name="radius">How many steps away from the center to select</param>
		/// <returns></returns>
		IEnumerable<Vector2> SelectRadius(Vector2? tilePos, int radius)
		{
			var sample = tilePos;

			if (sample.HasValue)
			{
				switch (mode)
				{
                    
					case Mode.Square:
                        // Tile selected is within (inclusive) radius tiles distance from tilePos
                        // Tiles are considered adjacent if they share an edge (not a corner).
						for (int x, y = -radius; y <= radius; y++)
						{
							for (x = -radius; x <= radius; x++)
							{
								int dist = x.Abs() + y.Abs();

								if (dist <= radius)
								{
									yield return sample.Value.V2Add(x, y);
								}
							}
						}
						break;

					case Mode.Hexagon:
                        // Tile selected is within (inclusive) radius tiles distance from tilePos
                        // Tiles are considered adjacent if they share an edge.
                        for (int col, row = -radius; row <= radius; row++)
						{
							for (col = -radius; col <= radius; col++)
							{
								int x = col,
									z = row,
									y = -x - z;

								int dist = x.Abs().Max(y.Abs(), z.Abs());

								if (dist <= radius)
								{
									yield return sample.Value.V2Add(col, row);
								}
							}
						}
						break;
				}
			}
		}

		#endregion
	}
}
