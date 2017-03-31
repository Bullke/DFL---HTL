using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HWTools.GameStats
{
	/// <summary>
	/// Interface for objects with a GameStatContainer.
	/// <para/>
	/// Allows other classes to interact with held stats.
	/// </summary>
	public interface IHasStats
	{
		#region Public Properties

		GameStatContainer Stats { get; }

		#endregion
	}

	[Serializable]
	public class GameStatContainer
	{
		#region Public Fields

		List<Stat> _stats;

		#endregion

		List<Stat> Stats
		{
			get
			{
				if (_stats == null)
				{
					_stats = new List<Stat>();
				}
				return _stats;
			}
			
		}

		#region Public Indexers

		public Stat this[string key]
		{
			get { return Stats.First(x => x.id == key); }
		}

		#endregion

		public Stat AddOrGetUnique(string id)
		{
			Stat result = Stats.FirstOrDefault(s => s.id == id);
			if(result == null)
			{
				result = new Stat { id = id };

				_stats.Add(result);
			}

			return result;
		}
	}

	[Serializable]
	public class Stat
	{
		#region Public Fields

		public float baseValue;
		public bool hasMax;
		public bool hasMin;
		public string id;
		public float max;
		public float min;

		#endregion

		#region Private Fields

		[SerializeField, HideInInspector]
		List<StatModifier> _mods;

		#endregion

		#region Public Properties

		public float EffectiveValue
		{
			get
			{
				float result = baseValue;

				float added = 0;

				foreach (var e in Mods)
				{
					result *= e.multiplier;
					added += e.addition;
				}

				result += added;

				if (hasMax)
				{
					result = result.Max(max);
				}
				if (hasMin)
				{
					result = result.Min(min);
				}

				return result;
			}
		}

		#endregion

		#region Private Properties

		List<StatModifier> Mods
		{
			get
			{
				if (_mods == null)
				{
					_mods = new List<StatModifier>();
				}
				return _mods;
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Adds a given modifier if it doesn't already exist in the list of modifiers. 
		/// <para/>
		/// Returns the given modifier, or a modifier with the same ID. 
		/// </summary>
		/// <param name="mod"> The modifier to try to add </param>
		/// <returns>
		/// If there is no collision, the given StatModifier. Otherwise, the existing StatModifier.
		/// </returns>
		public StatModifier AddIfUnique(StatModifier mod)
		{
			var old = Find(m => m.id == mod.id);

			if (old == null)
			{
				Mods.Add(mod);
			}
			else
			{
				mod = old;
			}

			return mod;
		}

		public void AddMod(StatModifier mod)
		{
			Mods.Add(mod);
		}

		public StatModifier Find(Func<StatModifier, bool> predicate)
		{
			return Mods.FirstOrDefault(predicate);
		}

		public void RemoveMod(StatModifier mod)
		{
			Mods.Remove(mod);
		}

		#endregion
	}

	[Serializable]
	public class StatModifier
	{
		#region Public Fields

		public float addition;
		public string id;
		public float multiplier;
		public Timer timer;

		#endregion
	}
}