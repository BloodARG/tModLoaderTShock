using System;
using Terraria.Social;

namespace Terraria.Achievements
{
	public class ConditionIntTracker : AchievementTracker<int>
	{
		public ConditionIntTracker() : base(TrackerType.Int)
		{
		}

		public ConditionIntTracker(int maxValue) : base(TrackerType.Int)
		{
			this._maxValue = maxValue;
		}

		public override void ReportUpdate()
		{
			if (SocialAPI.Achievements != null && this._name != null)
			{
				SocialAPI.Achievements.UpdateIntStat(this._name, this._value);
			}
		}

		protected override void Load()
		{
		}
	}
}
