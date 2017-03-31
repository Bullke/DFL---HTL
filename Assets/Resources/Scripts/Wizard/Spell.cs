using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HTLWizards
{
	[Serializable]
	public class Spell
	{
		public enum Targeting
		{
			AreaPulse
		}

		public Targeting targetingStyle;

		public string displayName;

		public Timer cooldown;

		public SpellEffect effect;

		public RuntimeAnimatorController animControl;

	}
}
