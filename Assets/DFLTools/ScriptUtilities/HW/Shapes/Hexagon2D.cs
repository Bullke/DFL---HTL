using System;
using UnityEngine;

namespace HWTools.Shapes
{

	/// <summary>
	/// Represents a flat-topped hexagon in a 2D space
	/// </summary>
	[Serializable]
	struct Hexagon2D : Shape2D
	{
		#region Public Fields

		/// <summary>
		/// The length of the inscribed diameter of a hexagon relative to the edge length.
		/// <para/>
		/// = √3
		/// </summary>
		public const float InDiameterRatio = 1.73205080756887729352f;

		/// <summary>
		/// The length of the inscribed radius of a hexagon relative to the edge length.
		/// <para/>
		/// = (√3)/2
		/// </summary>
		public const float InRadiusRatio = InDiameterRatio / 2;

		/// <summary>
		/// Represents a Hexagon of radius 1
		/// </summary>
		public readonly static Hexagon2D One = new Hexagon2D { _edge = 1 };

		#endregion

		#region Private Fields

		/// <summary>
		/// The serializable edge length of the Hexagon
		/// </summary>
		[SerializeField, HideInInspector]
		float _edge;

		/// <summary>
		/// The serializable position of the Hexagon
		/// </summary>
		[SerializeField, HideInInspector]
		Vector2 _position;

		#endregion

		#region Public Properties

		/// <summary>
		/// The Hexagon's area
		/// <para/>
		/// = Edge^2 * (1.5 * √3)
		/// </summary>
		public float Area
		{
			get { return InscribeDiameter * 1.5f * EdgeLength; }
			set { EdgeLength = (float)Math.Sqrt(value * InDiameterRatio * 2) / 3; }
		}

		/// <summary>
		/// The lower left corner of the rectangle circumscribing the Hexagon
		/// </summary>
		public Vector2 MinBoundCorner
		{
			get { return Center - new Vector2(_edge, InscribeRadius); }
			set { Center = value + new Vector2(_edge, InscribeRadius); }
		}

		/// <summary>
		/// The center of the hexagon
		/// </summary>
		public Vector2 Center
		{
			get { return _position; }
			set { _position = value; }
		}

		/// <summary>
		/// The circumdiameter of the Hexagon, or the diameter of the circumscribed circle
		/// <para/>
		/// = Edge * 2
		/// </summary>
		public float Circumdiameter { get { return EdgeLength * 2; } set { EdgeLength = value / 2; } }

		/// <summary>
		/// The circumradius of the Hexagon, or the radius of the circumscribed circle
		/// <para/>
		/// = Edge
		/// </summary>
		public float Circumradius { get { return EdgeLength; } set { EdgeLength = value; } }

		/// <summary>
		/// The length of the Hexagon's edges
		/// </summary>
		public float EdgeLength { get { return _edge; } set { _edge = value; } }

		/// <summary>
		/// The diameter of the inscribed circle
		/// <para/>
		/// = Edge * √3
		/// </summary>
		public float InscribeDiameter
		{
			get { return _edge * InDiameterRatio; }
			set { _edge = value / InDiameterRatio; }
		}

		/// <summary>
		/// The radius of the inscribed circle
		/// <para/>
		/// = Edge * √3 / 2
		/// </summary>
		public float InscribeRadius
		{
			get { return _edge* InRadiusRatio; }
			set { _edge = value / InRadiusRatio; }
		}

		/// <summary>
		/// The sum of the length of the Hexagon's edges
		/// <para/>
		/// = Edge * 6
		/// </summary>
		public float Perimeter { get { return _edge * 6; } set { _edge = value / 6; } }

		#endregion

		#region Public Methods

		/// <summary>
		/// Calculates the corner positions of the Hexagon
		/// </summary>
		/// <returns></returns>
		public Vector2[] CalculateCorners()
		{
			return new Vector2[] // Clockwise from -x corner
					{
						Center + new Vector2(-Circumradius, 0),
						Center + new Vector2(-Circumradius / 2, InscribeRadius),
						Center + new Vector2(Circumradius / 2, InscribeRadius),
						Center + new Vector2(Circumradius, 0),
						Center + new Vector2(Circumradius / 2, -InscribeRadius),
						Center + new Vector2(-Circumradius / 2, -InscribeRadius)
					};
		}

		public Vector2[] CalculateNeighbors()
		{
			throw new NotImplementedException();
		}
		

		public Vector2[] NeighborIndices()
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}