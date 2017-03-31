using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HWTools.Grid
{
	public partial class GridGraph
	{
		protected struct Node
		{

			public Vector2 position;
			public Vector2[] links;
			public Vector2[] corners;

		}
	}
}