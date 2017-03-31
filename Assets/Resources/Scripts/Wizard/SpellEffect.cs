using HWTools.GameStats;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace HTLWizards
{
	[Serializable]
	public class SpellEffect
	{
		#region Public Enums

		public enum Mode
		{
			DirectChange,
			Modifier
		}

		#endregion

		#region Public Fields

		public string id;

		public Timer durationTimer;
		public FloatEffect end;

		public Mode mode;

		public FloatEffect start;

		public string[] tags;

		public string targetStatKey;

		public FloatEffect tick;

		#endregion

		#region Public Methods

		public void ApplyEffect(GameStatContainer target)
		{
			
			var targetStat = target[targetStatKey];

			if(mode == Mode.DirectChange)
			{
				ApplyDirectChange(targetStat);
			}
			else
			{
				ApplyModifier(targetStat);
			}
		}

		void ApplyDirectChange(Stat targetStat)
		{
			var effectTimer = durationTimer.Clone;
			effectTimer.id = id;
			
			if (start.enabled)
			{
				if(start.mode == FloatEffect.Mode.Add)
				{
					targetStat.baseValue += start.magnitude;
				}
				else
				{
					targetStat.baseValue *= start.magnitude;
				}
			}

			if(tick.enabled || end.enabled)
			{ 
				if (tick.enabled)
				{
					if(tick.mode == FloatEffect.Mode.Add)
					{
						effectTimer.OnTick = dt => targetStat.baseValue += dt * tick.magnitude;
					}
					else
					{
						float log = Mathf.Log(tick.magnitude);
						effectTimer.OnTick = dt => targetStat.baseValue *= 
							Mathf.Exp(dt * log);
					}
				}
				if (end.enabled)
				{
					if(end.mode == FloatEffect.Mode.Add)
					{
						effectTimer.OnComplete = () => targetStat.baseValue += end.magnitude;
					}
					else
					{
						effectTimer.OnComplete = () => targetStat.baseValue *= end.magnitude;
					}
				}
				effectTimer.Run();
			}
		}

		void ApplyModifier(Stat targetStat)
		{
			var prev = targetStat.Find(mod => mod.id == id);

			if (prev != null)
			{
				//todo implement append mode
				//todo implement overlap mode

				//this is replace mode
				prev.timer.Current = 0;

			}
			else
			{
				var modifier = new StatModifier();
				modifier.timer = durationTimer.Clone;
				modifier.id = id;

				if (start.mode == FloatEffect.Mode.Multiply)
				{
					modifier.multiplier = start.magnitude;
				}
				else
				{
					modifier.addition = start.magnitude;
				}

				targetStat.AddMod(modifier);

				modifier.timer.OnComplete = () => {
					targetStat.RemoveMod(modifier);
					};

				modifier.timer.Loop = false;
				modifier.timer.Run();
			}

		}


		#endregion

		#region Public Classes

		[Serializable]
		public class FloatEffect
		{
			#region Public Enums

			public enum Mode
			{
				Add, Multiply
			}

			#endregion

			#region Public Fields

			public bool enabled;
			public float magnitude;
			public Mode mode;

			#endregion
		}

		#endregion
	}
}
