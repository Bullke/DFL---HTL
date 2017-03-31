using HWTools;
using HWTools.GameStats;
using HWTools.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HTLWizards
{
    /// <summary>
    /// Enum for identifying wizard variety. For interaction with Wizard Queues in the Wizard Manager
    /// </summary>
    public enum WizardType { Healer, Speed}

	[RequireComponent(typeof(GridTransform))]
	public class Wizard : TileObject
	{
		#region Public Fields

		public float range;

        public WizardType wizardType = WizardType.Healer;

		public Spell spell;


		#endregion

		#region Private Fields

		GridTransform _gTransform;

		Grid2DCollection Collection
		{
			get {
				if(_gTransform == null)
				{
					_gTransform = GetComponent<GridTransform>();
				}
				return _gTransform.parent.GetComponent<Grid2DCollection>(); }
		}

		#endregion

		#region Protected Methods

		new protected void Start()
		{
			spell.cooldown.Loop = false;
			spell.cooldown.Run();
			transform.FindChild("RadiusVisual")
				.GetComponent<Animator>().runtimeAnimatorController = spell.animControl;

			base.Start();
		}

		new protected void Update()
		{
			if (spell.cooldown.Complete)
			{
				var targets = QueryForTargets(SquibbleFilter);

				if(targets.Any())
				{

					foreach (IHasStats s in targets.Select(o=> o.GetComponent<Squibble>()))
					{
						spell.effect.ApplyEffect(s.Stats);
					}

					GetComponentInChildren<SpriteFlash>().Blink();

					spell.cooldown.Run(); // restart the cooldown timer
				}
			}
			base.Update();
		}

		protected void OnDrawGizmos()
		{
			var prev = Gizmos.color;
			foreach (var gt in QueryForTargets(SquibbleFilter))
			{
				Gizmos.color = Color.white.RandMult(Color.gray, Color.white);
				Gizmos.DrawLine(transform.position, gt.transform.position);
			}
			Gizmos.color = prev;
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			foreach (var render in GetComponentsInChildren<SpriteRenderer>())
			{
				render.sprite = null;
			}
		}

		#endregion

		#region Private Methods

		/// <summary>
		///   Checks that a GridTransform is associated with a Squibble 
		/// </summary>
		/// <param name="gt"> The GridTransform to evaluate </param>
		/// <returns> Whether the GridTransform's GameObject has a Squibble script </returns>
		static bool SquibbleFilter(GridTransform gt)
		{
			return gt.GetComponent<Squibble>() != null;
		}

		/// <summary>
		///   Select all objects in range 
		/// </summary>
		/// <param name="filter"> Optional filter </param>
		/// <returns></returns>
		GridTransform[] QueryForTargets(Func<GridTransform, bool> filter = null)
		{
			var squibs = Collection
				.LooseOccupants
				.Where(gt =>
				_gTransform.GridDistance(gt) <= range
				&& (filter != null ? filter(gt) : true))
				.ToArray();

			return squibs;
		}

		/// <summary>
		///   Select random object in range 
		/// </summary>
		/// <param name="filter"> Optional filter </param>
		/// <returns></returns>
		GridTransform RandomInRange(Func<GridTransform, bool> filter = null)
		{
			return QueryForTargets(filter).Random();
		}

		#endregion
	}
}
