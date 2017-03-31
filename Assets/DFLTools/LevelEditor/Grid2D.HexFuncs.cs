using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Hex = HWTools.Shapes.Hexagon2D;

namespace HWTools.Grid
{
	public partial class Grid2D
	{
		#region Public Fields

		public bool hexDebugLines;

		#endregion

		#region Private Methods

		/// <summary>
		/// Given a value in hex space, produces the position in square space.
		/// </summary>
		/// <param name="coord">The position in hex space</param>
		/// <returns>The position in square space</returns>
		Vector2 FromHexCoord(Vector2 coord)
		{
			var result = new Vector2(
				coord.x * 1.5f,
				coord.y * Hex.InDiameterRatio + coord.x * Hex.InRadiusRatio
				);
			
			return result; 
		}

		/// <summary>
		/// Draws a hex with debug lines
		/// </summary>
		/// <param name="root">The origin of the hex</param>
		/// <param name="color">Optional color for rendering the hex</param>
		void GizmoSingleHex(Vector2 root, Color? color = null)
		{

			float x = root.x * Hex.One.Circumradius * 1.5f;
			float y = root.y * Hex.InDiameterRatio;


			y += root.x * Hex.InRadiusRatio;


			var hex = new Hex
			{
				EdgeLength = 1,
				Center = new Vector2(x, y)
			};

			var points = hex.CalculateCorners();

			Color prev = Gizmos.color;

			if (color.HasValue)
			{
				Gizmos.color = color.Value;
			}

			for (int i = 0; i < points.Length; i++)
			{
				var p1 = Projection2D(points[i]);
				var p2 = Projection2D(points[(i + 1) % points.Length]);
				Gizmos.DrawLine(p1, p2);
			}
			if (hexDebugLines)
			{
				{
					var p1 = Projection2D(points[1]);
					var p2 = Projection2D(points[5]);
					Gizmos.DrawLine(p1, p2);
				}
				{
					var p1 = Projection2D(points[0]);
					var p2 = Projection2D(points[3]);
					Gizmos.DrawLine(p1, p2);
				}
				{
					var p1 = Projection2D(points[2]);
					var p2 = Projection2D(points[4]);
					Gizmos.DrawLine(p1, p2);
				}
			}

			Gizmos.color = prev;
		}

		/// <summary>
		/// Given a point in square space, produces an index in hex space
		/// </summary>
		/// <param name="pos">The point in square space to evaluate</param>
		/// <returns>The 2D index in hex space</returns>
		Vector2 SelectHex(Vector2 pos)
		{
			pos -= Hex.One.MinBoundCorner;

			float x = pos.x;
			float y = pos.y;

			// odd numbered column?
			bool odd = x.Mod(3) > 1.5f;
			
			//Position in sub-rectangle
			float xr = x.Mod(1.5f);
			float yr = y.Mod(Hex.InDiameterRatio);

			if (xr < .5f) // 1/3rd column
			{
				// Upper half of associated hex?
				bool upper = odd ^ (yr > Hex.InRadiusRatio);

				float halfy = yr.Mod(Hex.InRadiusRatio);

				float funcY = upper ?
					xr * Hex.InDiameterRatio :
					Hex.InRadiusRatio - xr * Hex.InDiameterRatio;

				bool outside = upper ?
					halfy > funcY :
					halfy < funcY;

				if (outside)
				{
					x -= 1.5f;// x--
				}
			}

			x = (x / 1.5f).Floor();

			y -= x * Hex.InRadiusRatio;

			y /= Hex.InDiameterRatio;

			return new Vector2(x, y.Floor());
		}

		#endregion
	}
}