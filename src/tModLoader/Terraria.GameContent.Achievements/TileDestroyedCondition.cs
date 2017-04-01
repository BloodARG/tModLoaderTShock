using System;
using System.Collections.Generic;
using Terraria.Achievements;

namespace Terraria.GameContent.Achievements
{
	public class TileDestroyedCondition : AchievementCondition
	{
		private const string Identifier = "TILE_DESTROYED";
		private static Dictionary<ushort, List<TileDestroyedCondition>> _listeners = new Dictionary<ushort, List<TileDestroyedCondition>>();
		private static bool _isListenerHooked = false;
		private ushort[] _tileIds;

		private TileDestroyedCondition(ushort[] tileIds)
			: base("TILE_DESTROYED_" + tileIds[0])
		{
			this._tileIds = tileIds;
			TileDestroyedCondition.ListenForDestruction(this);
		}

		private static void ListenForDestruction(TileDestroyedCondition condition)
		{
			if (!TileDestroyedCondition._isListenerHooked)
			{
				AchievementsHelper.OnTileDestroyed += new AchievementsHelper.TileDestroyedEvent(TileDestroyedCondition.TileDestroyedListener);
				TileDestroyedCondition._isListenerHooked = true;
			}
			for (int i = 0; i < condition._tileIds.Length; i++)
			{
				if (!TileDestroyedCondition._listeners.ContainsKey(condition._tileIds[i]))
				{
					TileDestroyedCondition._listeners[condition._tileIds[i]] = new List<TileDestroyedCondition>();
				}
				TileDestroyedCondition._listeners[condition._tileIds[i]].Add(condition);
			}
		}

		private static void TileDestroyedListener(Player player, ushort tileId)
		{
			if (player.whoAmI != Main.myPlayer)
			{
				return;
			}
			if (TileDestroyedCondition._listeners.ContainsKey(tileId))
			{
				List<TileDestroyedCondition> list = TileDestroyedCondition._listeners[tileId];
				foreach (TileDestroyedCondition current in list)
				{
					current.Complete();
				}
			}
		}

		public static AchievementCondition Create(params ushort[] tileIds)
		{
			return new TileDestroyedCondition(tileIds);
		}
	}
}
