using System;
using System.Collections.Generic;

namespace Terraria.Achievements
{
	public class ConditionsCompletedTracker : ConditionIntTracker
	{
		private List<AchievementCondition> _conditions = new List<AchievementCondition>();

		public void AddCondition(AchievementCondition condition)
		{
			this._maxValue++;
			condition.OnComplete += new AchievementCondition.AchievementUpdate(this.OnConditionCompleted);
			this._conditions.Add(condition);
		}

		private void OnConditionCompleted(AchievementCondition condition)
		{
			base.SetValue(Math.Min(this._value + 1, this._maxValue), true);
		}

		protected override void Load()
		{
			for (int i = 0; i < this._conditions.Count; i++)
			{
				if (this._conditions[i].IsCompleted)
				{
					this._value++;
				}
			}
		}
	}
}
