using System;
using Terraria.Social;

namespace Terraria.Achievements
{
	public class ConditionFloatTracker : AchievementTracker<float>
	{
		public ConditionFloatTracker(float maxValue)
			: base(TrackerType.Float)
		{
			this._maxValue = maxValue;
		}

		public ConditionFloatTracker()
			: base(TrackerType.Float)
		{
		}

		public override void ReportUpdate()
		{
			if (SocialAPI.Achievements != null && this._name != null)
			{
				SocialAPI.Achievements.UpdateFloatStat(this._name, this._value);
			}
		}

		protected override void Load()
		{
		}
	}
}
