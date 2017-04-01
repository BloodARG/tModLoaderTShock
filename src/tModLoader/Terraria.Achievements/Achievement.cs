using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using Terraria.Localization;
using Terraria.Social;

namespace Terraria.Achievements
{
	[JsonObject(MemberSerialization.OptIn)]
	public class Achievement
	{
		public delegate void AchievementCompleted(Achievement achievement);

		private static int _totalAchievements;
		public readonly string Name;
		public readonly LocalizedText FriendlyName;
		public readonly LocalizedText Description;
		public readonly int Id = Achievement._totalAchievements++;
		private AchievementCategory _category;
		private IAchievementTracker _tracker;
		[JsonProperty("Conditions")]
		private Dictionary<string, AchievementCondition> _conditions = new Dictionary<string, AchievementCondition>();
		private int _completedCount;

		public event Achievement.AchievementCompleted OnCompleted;

		public AchievementCategory Category
		{
			get
			{
				return this._category;
			}
		}

		public bool HasTracker
		{
			get
			{
				return this._tracker != null;
			}
		}

		public bool IsCompleted
		{
			get
			{
				return this._completedCount == this._conditions.Count;
			}
		}

		public IAchievementTracker GetTracker()
		{
			return this._tracker;
		}

		public Achievement(string name)
		{
			this.Name = name;
			this.FriendlyName = Language.GetText("Achievements." + name + "_Name");
			this.Description = Language.GetText("Achievements." + name + "_Description");
		}

		public void ClearProgress()
		{
			this._completedCount = 0;
			foreach (KeyValuePair<string, AchievementCondition> current in this._conditions)
			{
				current.Value.Clear();
			}
			if (this._tracker != null)
			{
				this._tracker.Clear();
			}
		}

		public void Load(Dictionary<string, JObject> conditions)
		{
			foreach (KeyValuePair<string, JObject> current in conditions)
			{
				AchievementCondition achievementCondition;
				if (this._conditions.TryGetValue(current.Key, out achievementCondition))
				{
					achievementCondition.Load(current.Value);
					if (achievementCondition.IsCompleted)
					{
						this._completedCount++;
					}
				}
			}
			if (this._tracker != null)
			{
				this._tracker.Load();
			}
		}

		public void AddCondition(AchievementCondition condition)
		{
			this._conditions[condition.Name] = condition;
			condition.OnComplete += new AchievementCondition.AchievementUpdate(this.OnConditionComplete);
		}

		private void OnConditionComplete(AchievementCondition condition)
		{
			this._completedCount++;
			if (this._completedCount == this._conditions.Count)
			{
				if (this._tracker == null && SocialAPI.Achievements != null)
				{
					SocialAPI.Achievements.CompleteAchievement(this.Name);
				}
				if (this.OnCompleted != null)
				{
					this.OnCompleted(this);
				}
			}
		}

		private void UseTracker(IAchievementTracker tracker)
		{
			tracker.ReportAs("STAT_" + this.Name);
			this._tracker = tracker;
		}

		public void UseTrackerFromCondition(string conditionName)
		{
			this.UseTracker(this.GetConditionTracker(conditionName));
		}

		public void UseConditionsCompletedTracker()
		{
			ConditionsCompletedTracker conditionsCompletedTracker = new ConditionsCompletedTracker();
			foreach (KeyValuePair<string, AchievementCondition> current in this._conditions)
			{
				conditionsCompletedTracker.AddCondition(current.Value);
			}
			this.UseTracker(conditionsCompletedTracker);
		}

		public void UseConditionsCompletedTracker(params string[] conditions)
		{
			ConditionsCompletedTracker conditionsCompletedTracker = new ConditionsCompletedTracker();
			for (int i = 0; i < conditions.Length; i++)
			{
				string key = conditions[i];
				conditionsCompletedTracker.AddCondition(this._conditions[key]);
			}
			this.UseTracker(conditionsCompletedTracker);
		}

		public void ClearTracker()
		{
			this._tracker = null;
		}

		private IAchievementTracker GetConditionTracker(string name)
		{
			return this._conditions[name].GetAchievementTracker();
		}

		public void AddConditions(params AchievementCondition[] conditions)
		{
			for (int i = 0; i < conditions.Length; i++)
			{
				this.AddCondition(conditions[i]);
			}
		}

		public AchievementCondition GetCondition(string conditionName)
		{
			AchievementCondition result;
			if (this._conditions.TryGetValue(conditionName, out result))
			{
				return result;
			}
			return null;
		}

		public void SetCategory(AchievementCategory category)
		{
			this._category = category;
		}
	}
}
