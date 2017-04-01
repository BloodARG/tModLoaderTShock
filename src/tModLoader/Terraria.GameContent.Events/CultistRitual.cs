using Microsoft.Xna.Framework;
using System;

namespace Terraria.GameContent.Events
{
	public class CultistRitual
	{
		public const int delayStart = 86400;
		public const int respawnDelay = 43200;
		private const int timePerCultist = 3600;
		private const int recheckStart = 600;
		public static int delay;
		public static int recheck;

		public static void UpdateTime()
		{
			if (Main.netMode == 1)
			{
				return;
			}
			CultistRitual.delay -= Main.dayRate;
			if (CultistRitual.delay < 0)
			{
				CultistRitual.delay = 0;
			}
			CultistRitual.recheck -= Main.dayRate;
			if (CultistRitual.recheck < 0)
			{
				CultistRitual.recheck = 0;
			}
			if (CultistRitual.delay == 0 && CultistRitual.recheck == 0)
			{
				CultistRitual.recheck = 600;
				bool flag = NPC.AnyDanger();
				if (flag)
				{
					CultistRitual.recheck *= 6;
					return;
				}
				CultistRitual.TrySpawning(Main.dungeonX, Main.dungeonY);
			}
		}

		public static void CultistSlain()
		{
			CultistRitual.delay -= 3600;
		}

		public static void TabletDestroyed()
		{
			CultistRitual.delay = 43200;
		}

		public static void TrySpawning(int x, int y)
		{
			if (WorldGen.PlayerLOS(x - 6, y) || WorldGen.PlayerLOS(x + 6, y))
			{
				return;
			}
			if (!CultistRitual.CheckRitual(x, y))
			{
				return;
			}
			NPC.NewNPC(x * 16 + 8, (y - 4) * 16 - 8, 437, 0, 0f, 0f, 0f, 0f, 255);
		}

		private static bool CheckRitual(int x, int y)
		{
			if (CultistRitual.delay != 0 || !Main.hardMode || !NPC.downedGolemBoss || !NPC.downedBoss3)
			{
				return false;
			}
			if (y < 7 || WorldGen.SolidTile(Main.tile[x, y - 7]))
			{
				return false;
			}
			if (NPC.AnyNPCs(437))
			{
				return false;
			}
			Vector2 center = new Vector2((float)(x * 16 + 8), (float)(y * 16 - 64 - 8 - 27));
			Point[] array = null;
			return CultistRitual.CheckFloor(center, out array);
		}

		public static bool CheckFloor(Vector2 Center, out Point[] spawnPoints)
		{
			Point[] array = new Point[4];
			int num = 0;
			Point point = Center.ToTileCoordinates();
			for (int i = -5; i <= 5; i += 2)
			{
				if (i != -1 && i != 1)
				{
					for (int j = -5; j < 12; j++)
					{
						int num2 = point.X + i * 2;
						int num3 = point.Y + j;
						if (WorldGen.SolidTile(num2, num3) && !Collision.SolidTiles(num2 - 1, num2 + 1, num3 - 3, num3 - 1))
						{
							array[num++] = new Point(num2, num3);
							break;
						}
					}
				}
			}
			if (num != 4)
			{
				spawnPoints = null;
				return false;
			}
			spawnPoints = array;
			return true;
		}
	}
}
