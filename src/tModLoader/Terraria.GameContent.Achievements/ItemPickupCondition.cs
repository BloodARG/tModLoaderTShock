using System;
using System.Collections.Generic;
using Terraria.Achievements;

namespace Terraria.GameContent.Achievements
{
	public class ItemPickupCondition : AchievementCondition
	{
		private const string Identifier = "ITEM_PICKUP";
		private static Dictionary<short, List<ItemPickupCondition>> _listeners = new Dictionary<short, List<ItemPickupCondition>>();
		private static bool _isListenerHooked = false;
		private short[] _itemIds;

		private ItemPickupCondition(short itemId)
			: base("ITEM_PICKUP_" + itemId)
		{
			this._itemIds = new short[]
			{
				itemId
			};
			ItemPickupCondition.ListenForPickup(this);
		}

		private ItemPickupCondition(short[] itemIds)
			: base("ITEM_PICKUP_" + itemIds[0])
		{
			this._itemIds = itemIds;
			ItemPickupCondition.ListenForPickup(this);
		}

		private static void ListenForPickup(ItemPickupCondition condition)
		{
			if (!ItemPickupCondition._isListenerHooked)
			{
				AchievementsHelper.OnItemPickup += new AchievementsHelper.ItemPickupEvent(ItemPickupCondition.ItemPickupListener);
				ItemPickupCondition._isListenerHooked = true;
			}
			for (int i = 0; i < condition._itemIds.Length; i++)
			{
				if (!ItemPickupCondition._listeners.ContainsKey(condition._itemIds[i]))
				{
					ItemPickupCondition._listeners[condition._itemIds[i]] = new List<ItemPickupCondition>();
				}
				ItemPickupCondition._listeners[condition._itemIds[i]].Add(condition);
			}
		}

		private static void ItemPickupListener(Player player, short itemId, int count)
		{
			if (player.whoAmI != Main.myPlayer)
			{
				return;
			}
			if (ItemPickupCondition._listeners.ContainsKey(itemId))
			{
				List<ItemPickupCondition> list = ItemPickupCondition._listeners[itemId];
				foreach (ItemPickupCondition current in list)
				{
					current.Complete();
				}
			}
		}

		public static AchievementCondition Create(params short[] items)
		{
			return new ItemPickupCondition(items);
		}

		public static AchievementCondition Create(short item)
		{
			return new ItemPickupCondition(item);
		}

		public static AchievementCondition[] CreateMany(params short[] items)
		{
			AchievementCondition[] array = new AchievementCondition[items.Length];
			for (int i = 0; i < items.Length; i++)
			{
				array[i] = new ItemPickupCondition(items[i]);
			}
			return array;
		}
	}
}
