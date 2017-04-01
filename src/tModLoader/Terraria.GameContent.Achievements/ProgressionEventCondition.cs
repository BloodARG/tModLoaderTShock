using System;
using System.Collections.Generic;
using Terraria.Achievements;

namespace Terraria.GameContent.Achievements
{
	public class ProgressionEventCondition : AchievementCondition
	{
		private const string Identifier = "PROGRESSION_EVENT";
		private static Dictionary<int, List<ProgressionEventCondition>> _listeners = new Dictionary<int, List<ProgressionEventCondition>>();
		private static bool _isListenerHooked = false;
		private int[] _eventIDs;

		private ProgressionEventCondition(int eventID)
			: base("PROGRESSION_EVENT_" + eventID)
		{
			this._eventIDs = new int[]
			{
				eventID
			};
			ProgressionEventCondition.ListenForPickup(this);
		}

		private ProgressionEventCondition(int[] eventIDs)
			: base("PROGRESSION_EVENT_" + eventIDs[0])
		{
			this._eventIDs = eventIDs;
			ProgressionEventCondition.ListenForPickup(this);
		}

		private static void ListenForPickup(ProgressionEventCondition condition)
		{
			if (!ProgressionEventCondition._isListenerHooked)
			{
				AchievementsHelper.OnProgressionEvent += new AchievementsHelper.ProgressionEventEvent(ProgressionEventCondition.ProgressionEventListener);
				ProgressionEventCondition._isListenerHooked = true;
			}
			for (int i = 0; i < condition._eventIDs.Length; i++)
			{
				if (!ProgressionEventCondition._listeners.ContainsKey(condition._eventIDs[i]))
				{
					ProgressionEventCondition._listeners[condition._eventIDs[i]] = new List<ProgressionEventCondition>();
				}
				ProgressionEventCondition._listeners[condition._eventIDs[i]].Add(condition);
			}
		}

		private static void ProgressionEventListener(int eventID)
		{
			if (ProgressionEventCondition._listeners.ContainsKey(eventID))
			{
				List<ProgressionEventCondition> list = ProgressionEventCondition._listeners[eventID];
				foreach (ProgressionEventCondition current in list)
				{
					current.Complete();
				}
			}
		}

		public static ProgressionEventCondition Create(params int[] eventIDs)
		{
			return new ProgressionEventCondition(eventIDs);
		}

		public static ProgressionEventCondition Create(int eventID)
		{
			return new ProgressionEventCondition(eventID);
		}

		public static ProgressionEventCondition[] CreateMany(params int[] eventIDs)
		{
			ProgressionEventCondition[] array = new ProgressionEventCondition[eventIDs.Length];
			for (int i = 0; i < eventIDs.Length; i++)
			{
				array[i] = new ProgressionEventCondition(eventIDs[i]);
			}
			return array;
		}
	}
}
