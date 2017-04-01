using System;
using Terraria.Social;

namespace Terraria.Achievements
{
	public abstract class AchievementTracker<T> : IAchievementTracker
	{
		protected T _value;
		protected T _maxValue;
		protected string _name;
		private TrackerType _type;

		public T Value
		{
			get
			{
				return this._value;
			}
		}

		public T MaxValue
		{
			get
			{
				return this._maxValue;
			}
		}

		protected AchievementTracker(TrackerType type)
		{
			this._type = type;
		}

		void IAchievementTracker.ReportAs(string name)
		{
			this._name = name;
		}

		TrackerType IAchievementTracker.GetTrackerType()
		{
			return this._type;
		}

		void IAchievementTracker.Clear()
		{
			this.SetValue(default(T), true);
		}

		public void SetValue(T newValue, bool reportUpdate = true)
		{
			if (!newValue.Equals(this._value))
			{
				this._value = newValue;
				if (reportUpdate)
				{
					this.ReportUpdate();
					if (this._value.Equals(this._maxValue))
					{
						this.OnComplete();
					}
				}
			}
		}

		public abstract void ReportUpdate();

		protected abstract void Load();

		void IAchievementTracker.Load()
		{
			this.Load();
		}

		protected void OnComplete()
		{
			if (SocialAPI.Achievements != null)
			{
				SocialAPI.Achievements.StoreStats();
			}
		}
	}
}
