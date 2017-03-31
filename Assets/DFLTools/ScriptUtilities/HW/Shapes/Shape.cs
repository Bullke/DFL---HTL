using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HWTools.Shapes
{
	public interface Shape2D
	{
		Vector2 Center { get; set; }
		float Circumdiameter { get; set; }
		float Circumradius { get; set; }
		float InscribeRadius { get; set; }
		float InscribeDiameter { get; set; }
		float Area { get; set; }
		float EdgeLength { get; set; }
		float Perimeter { get; set; }

		Vector2[] CalculateCorners();
		Vector2[] CalculateNeighbors();
		Vector2[] NeighborIndices();

	}

	public static class ShapeExt
	{
		public static void DrawGizmos(this Shape2D shape, Color? color)
		{
			Color prev = Gizmos.color;

			if (color.HasValue)
			{
				Gizmos.color = color.Value;
			}

			var c = shape.CalculateCorners();

			for (int i = 0; i < c.Length; i++)
			{
				Gizmos.DrawLine(c[i], c[(i + 1) % c.Length]);
			}

			Gizmos.color = prev;
		}
	}
}
