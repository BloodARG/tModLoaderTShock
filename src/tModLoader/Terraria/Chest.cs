using Microsoft.Xna.Framework;
using System;
using Terraria.DataStructures;
using Terraria.GameContent.Achievements;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ObjectData;
using Terraria.ModLoader;

namespace Terraria
{
	public class Chest
	{
		public const int maxChestTypes = 52;
		public const int maxDresserTypes = 28;
		public const int maxItems = 40;
		public const int MaxNameLength = 20;
		public static int[] chestTypeToIcon = new int[52];
		public static int[] chestItemSpawn = new int[52];
		public static int[] dresserTypeToIcon = new int[28];
		public static int[] dresserItemSpawn = new int[28];
		public Item[] item;
		public int x;
		public int y;
		public bool bankChest;
		public string name;
		public int frameCounter;
		public int frame;

		public Chest(bool bank = false)
		{
			this.item = new Item[40];
			this.bankChest = bank;
			this.name = string.Empty;
		}

		public override string ToString()
		{
			int num = 0;
			for (int i = 0; i < this.item.Length; i++)
			{
				if (this.item[i].stack > 0)
				{
					num++;
				}
			}
			return string.Format("{{X: {0}, Y: {1}, Count: {2}}}", this.x, this.y, num);
		}

		public static void Initialize()
		{
			Chest.chestTypeToIcon[0] = (Chest.chestItemSpawn[0] = 48);
			Chest.chestTypeToIcon[1] = (Chest.chestItemSpawn[1] = 306);
			Chest.chestTypeToIcon[2] = 327;
			Chest.chestItemSpawn[2] = 306;
			Chest.chestTypeToIcon[3] = (Chest.chestItemSpawn[3] = 328);
			Chest.chestTypeToIcon[4] = 329;
			Chest.chestItemSpawn[4] = 328;
			Chest.chestTypeToIcon[5] = (Chest.chestItemSpawn[5] = 343);
			Chest.chestTypeToIcon[6] = (Chest.chestItemSpawn[6] = 348);
			Chest.chestTypeToIcon[7] = (Chest.chestItemSpawn[7] = 625);
			Chest.chestTypeToIcon[8] = (Chest.chestItemSpawn[8] = 626);
			Chest.chestTypeToIcon[9] = (Chest.chestItemSpawn[9] = 627);
			Chest.chestTypeToIcon[10] = (Chest.chestItemSpawn[10] = 680);
			Chest.chestTypeToIcon[11] = (Chest.chestItemSpawn[11] = 681);
			Chest.chestTypeToIcon[12] = (Chest.chestItemSpawn[12] = 831);
			Chest.chestTypeToIcon[13] = (Chest.chestItemSpawn[13] = 838);
			Chest.chestTypeToIcon[14] = (Chest.chestItemSpawn[14] = 914);
			Chest.chestTypeToIcon[15] = (Chest.chestItemSpawn[15] = 952);
			Chest.chestTypeToIcon[16] = (Chest.chestItemSpawn[16] = 1142);
			Chest.chestTypeToIcon[17] = (Chest.chestItemSpawn[17] = 1298);
			Chest.chestTypeToIcon[18] = (Chest.chestItemSpawn[18] = 1528);
			Chest.chestTypeToIcon[19] = (Chest.chestItemSpawn[19] = 1529);
			Chest.chestTypeToIcon[20] = (Chest.chestItemSpawn[20] = 1530);
			Chest.chestTypeToIcon[21] = (Chest.chestItemSpawn[21] = 1531);
			Chest.chestTypeToIcon[22] = (Chest.chestItemSpawn[22] = 1532);
			Chest.chestTypeToIcon[23] = 1533;
			Chest.chestItemSpawn[23] = 1528;
			Chest.chestTypeToIcon[24] = 1534;
			Chest.chestItemSpawn[24] = 1529;
			Chest.chestTypeToIcon[25] = 1535;
			Chest.chestItemSpawn[25] = 1530;
			Chest.chestTypeToIcon[26] = 1536;
			Chest.chestItemSpawn[26] = 1531;
			Chest.chestTypeToIcon[27] = 1537;
			Chest.chestItemSpawn[27] = 1532;
			Chest.chestTypeToIcon[28] = (Chest.chestItemSpawn[28] = 2230);
			Chest.chestTypeToIcon[29] = (Chest.chestItemSpawn[29] = 2249);
			Chest.chestTypeToIcon[30] = (Chest.chestItemSpawn[30] = 2250);
			Chest.chestTypeToIcon[31] = (Chest.chestItemSpawn[31] = 2526);
			Chest.chestTypeToIcon[32] = (Chest.chestItemSpawn[32] = 2544);
			Chest.chestTypeToIcon[33] = (Chest.chestItemSpawn[33] = 2559);
			Chest.chestTypeToIcon[34] = (Chest.chestItemSpawn[34] = 2574);
			Chest.chestTypeToIcon[35] = (Chest.chestItemSpawn[35] = 2612);
			Chest.chestTypeToIcon[36] = 327;
			Chest.chestItemSpawn[36] = 2612;
			Chest.chestTypeToIcon[37] = (Chest.chestItemSpawn[37] = 2613);
			Chest.chestTypeToIcon[38] = 327;
			Chest.chestItemSpawn[38] = 2613;
			Chest.chestTypeToIcon[39] = (Chest.chestItemSpawn[39] = 2614);
			Chest.chestTypeToIcon[40] = 327;
			Chest.chestItemSpawn[40] = 2614;
			Chest.chestTypeToIcon[41] = (Chest.chestItemSpawn[41] = 2615);
			Chest.chestTypeToIcon[42] = (Chest.chestItemSpawn[42] = 2616);
			Chest.chestTypeToIcon[43] = (Chest.chestItemSpawn[43] = 2617);
			Chest.chestTypeToIcon[44] = (Chest.chestItemSpawn[44] = 2618);
			Chest.chestTypeToIcon[45] = (Chest.chestItemSpawn[45] = 2619);
			Chest.chestTypeToIcon[46] = (Chest.chestItemSpawn[46] = 2620);
			Chest.chestTypeToIcon[47] = (Chest.chestItemSpawn[47] = 2748);
			Chest.chestTypeToIcon[48] = (Chest.chestItemSpawn[48] = 2814);
			Chest.chestTypeToIcon[49] = (Chest.chestItemSpawn[49] = 3180);
			Chest.chestTypeToIcon[50] = (Chest.chestItemSpawn[50] = 3125);
			Chest.chestTypeToIcon[51] = (Chest.chestItemSpawn[51] = 3181);
			Chest.dresserTypeToIcon[0] = (Chest.dresserItemSpawn[0] = 334);
			Chest.dresserTypeToIcon[1] = (Chest.dresserItemSpawn[1] = 647);
			Chest.dresserTypeToIcon[2] = (Chest.dresserItemSpawn[2] = 648);
			Chest.dresserTypeToIcon[3] = (Chest.dresserItemSpawn[3] = 649);
			Chest.dresserTypeToIcon[4] = (Chest.dresserItemSpawn[4] = 918);
			Chest.dresserTypeToIcon[5] = (Chest.dresserItemSpawn[5] = 2386);
			Chest.dresserTypeToIcon[6] = (Chest.dresserItemSpawn[6] = 2387);
			Chest.dresserTypeToIcon[7] = (Chest.dresserItemSpawn[7] = 2388);
			Chest.dresserTypeToIcon[8] = (Chest.dresserItemSpawn[8] = 2389);
			Chest.dresserTypeToIcon[9] = (Chest.dresserItemSpawn[9] = 2390);
			Chest.dresserTypeToIcon[10] = (Chest.dresserItemSpawn[10] = 2391);
			Chest.dresserTypeToIcon[11] = (Chest.dresserItemSpawn[11] = 2392);
			Chest.dresserTypeToIcon[12] = (Chest.dresserItemSpawn[12] = 2393);
			Chest.dresserTypeToIcon[13] = (Chest.dresserItemSpawn[13] = 2394);
			Chest.dresserTypeToIcon[14] = (Chest.dresserItemSpawn[14] = 2395);
			Chest.dresserTypeToIcon[15] = (Chest.dresserItemSpawn[15] = 2396);
			Chest.dresserTypeToIcon[16] = (Chest.dresserItemSpawn[16] = 2529);
			Chest.dresserTypeToIcon[17] = (Chest.dresserItemSpawn[17] = 2545);
			Chest.dresserTypeToIcon[18] = (Chest.dresserItemSpawn[18] = 2562);
			Chest.dresserTypeToIcon[19] = (Chest.dresserItemSpawn[19] = 2577);
			Chest.dresserTypeToIcon[20] = (Chest.dresserItemSpawn[20] = 2637);
			Chest.dresserTypeToIcon[21] = (Chest.dresserItemSpawn[21] = 2638);
			Chest.dresserTypeToIcon[22] = (Chest.dresserItemSpawn[22] = 2639);
			Chest.dresserTypeToIcon[23] = (Chest.dresserItemSpawn[23] = 2640);
			Chest.dresserTypeToIcon[24] = (Chest.dresserItemSpawn[24] = 2816);
			Chest.dresserTypeToIcon[25] = (Chest.dresserItemSpawn[25] = 3132);
			Chest.dresserTypeToIcon[26] = (Chest.dresserItemSpawn[26] = 3134);
			Chest.dresserTypeToIcon[27] = (Chest.dresserItemSpawn[27] = 3133);
		}

		private static bool IsPlayerInChest(int i)
		{
			for (int j = 0; j < 255; j++)
			{
				if (Main.player[j].chest == i)
				{
					return true;
				}
			}
			return false;
		}

		public static bool isLocked(int x, int y)
		{
			return Main.tile[x, y] == null || ((Main.tile[x, y].frameX >= 72 && Main.tile[x, y].frameX <= 106) || (Main.tile[x, y].frameX >= 144 && Main.tile[x, y].frameX <= 178) || (Main.tile[x, y].frameX >= 828 && Main.tile[x, y].frameX <= 1006) || (Main.tile[x, y].frameX >= 1296 && Main.tile[x, y].frameX <= 1330) || (Main.tile[x, y].frameX >= 1368 && Main.tile[x, y].frameX <= 1402) || (Main.tile[x, y].frameX >= 1440 && Main.tile[x, y].frameX <= 1474));
		}

        public static void ServerPlaceItem(int plr, int slot)
        {
            Main.player[plr].inventory[slot] = Chest.PutItemInNearbyChest(Main.player[plr].inventory[slot], Main.player[plr].Center, Main.player[plr]);
            NetMessage.SendData(5, -1, -1, "", plr, (float)slot, (float)Main.player[plr].inventory[slot].prefix, 0f, 0, 0, 0);
        }

        public static Item PutItemInNearbyChest(Item item, Vector2 position, Player player)
        {
            if (Main.netMode == 1)
            {
                return item;
            }
            for (int i = 0; i < 1000; i++)
            {
                bool stacking = false;
                bool emptySlots = false;
                if (Main.chest[i] != null && !Chest.IsPlayerInChest(i) && !Chest.isLocked(Main.chest[i].x, Main.chest[i].y))
                {
                    Vector2 chestPosition = new Vector2((float)(Main.chest[i].x * 16 + 16), (float)(Main.chest[i].y * 16 + 16));
                    if ((chestPosition - position).Length() < 200f)
                    {
                        if (TerrariaApi.Server.ServerApi.Hooks.InvokeItemForceIntoChest(Main.chest[i], item, player))
                        {
                            continue;
                        }

                        for (int j = 0; j < Main.chest[i].item.Length; j++)
                        {
                            if (Main.chest[i].item[j].type <= 0 || Main.chest[i].item[j].stack <= 0)
                            {
                                emptySlots = true;
                            }
                            else if (item.IsTheSameAs(Main.chest[i].item[j]))
                            {
                                //stacks items into the chest
                                stacking = true;
                                int num = Main.chest[i].item[j].maxStack - Main.chest[i].item[j].stack;
                                if (num > 0)
                                {
                                    if (num > item.stack)
                                    {
                                        num = item.stack;
                                    }
                                    item.stack -= num;
                                    Main.chest[i].item[j].stack += num;
                                    if (item.stack <= 0)
                                    {
                                        item.SetDefaults(0, false);
                                        return item;
                                    }
                                }
                            }
                        }
                        if (stacking && emptySlots && item.stack > 0)
                        {
                            //places items into empty slots in the chest
                            for (int k = 0; k < (int)Main.chest[i].item.Length; k++)
                            {
                                if (Main.chest[i].item[k].type == 0 || Main.chest[i].item[k].stack == 0)
                                {
                                    Main.chest[i].item[k] = item.Clone();
                                    item.SetDefaults(0, false);
                                    return item;
                                }
                            }
                        }
                    }
                }
            }
            return item;
        }

        public object Clone()
		{
			return base.MemberwiseClone();
		}

		public static bool Unlock(int X, int Y)
		{
			if (Main.tile[X, Y] == null)
			{
				return false;
			}
			int num = (int)(Main.tile[X, Y].frameX / 36);
			int num2 = num;
			short num3;
			int type;
			switch (num2)
			{
				case 2:
					{
						num3 = 36;
						type = 11;
						AchievementsHelper.NotifyProgressionEvent(19);
						goto IL_B7;
					}
				case 3:
					break;
				case 4:
					{
						num3 = 36;
						type = 11;
						goto IL_B7;
					}
				default:
					switch (num2)
					{
						case 23:
						case 24:
						case 25:
						case 26:
						case 27:
							{
								if (!NPC.downedPlantBoss)
								{
									return false;
								}
								num3 = 180;
								type = 11;
								AchievementsHelper.NotifyProgressionEvent(20);
								goto IL_B7;
							}
						default:
							switch (num2)
							{
								case 36:
								case 38:
								case 40:
									{
										num3 = 36;
										type = 11;
										goto IL_B7;
									}
							}
							break;
					}
					break;
			}
			return false;
			IL_B7:
			Main.PlaySound(22, X * 16, Y * 16, 1, 1f, 0f);
			for (int i = X; i <= X + 1; i++)
			{
				for (int j = Y; j <= Y + 1; j++)
				{
					Tile expr_E8 = Main.tile[i, j];
					expr_E8.frameX -= num3;
					for (int k = 0; k < 4; k++)
					{
						Dust.NewDust(new Vector2((float)(i * 16), (float)(j * 16)), 16, 16, type, 0f, 0f, 0, default(Color), 1f);
					}
				}
			}
			return true;
		}

		public static int UsingChest(int i)
		{
			if (Main.chest[i] != null)
			{
				for (int j = 0; j < 255; j++)
				{
					if (Main.player[j].active && Main.player[j].chest == i)
					{
						return j;
					}
				}
			}
			return -1;
		}

		public static int FindChest(int X, int Y)
		{
			for (int i = 0; i < 1000; i++)
			{
				if (Main.chest[i] != null && Main.chest[i].x == X && Main.chest[i].y == Y)
				{
					return i;
				}
			}
			return -1;
		}

		public static int FindEmptyChest(int x, int y, int type = 21, int style = 0, int direction = 1)
		{
			int num = -1;
			for (int i = 0; i < 1000; i++)
			{
				Chest chest = Main.chest[i];
				if (chest != null)
				{
					if (chest.x == x && chest.y == y)
					{
						return -1;
					}
				}
				else if (num == -1)
				{
					num = i;
				}
			}
			return num;
		}

		public static bool NearOtherChests(int x, int y)
		{
			for (int i = x - 25; i < x + 25; i++)
			{
				for (int j = y - 8; j < y + 8; j++)
				{
					Tile tileSafely = Framing.GetTileSafely(i, j);
					if (tileSafely.active() && tileSafely.type == 21)
					{
						return true;
					}
				}
			}
			return false;
		}

		public static int AfterPlacement_Hook(int x, int y, int type = 21, int style = 0, int direction = 1)
		{
			Point16 point = new Point16(x, y);
			TileObjectData.OriginToTopLeft(type, style, ref point);
			int num = Chest.FindEmptyChest((int)point.X, (int)point.Y, 21, 0, 1);
			if (num == -1)
			{
				return -1;
			}
			if (Main.netMode != 1)
			{
				Chest chest = new Chest(false);
				chest.x = (int)point.X;
				chest.y = (int)point.Y;
				for (int i = 0; i < 40; i++)
				{
					chest.item[i] = new Item();
				}
				Main.chest[num] = chest;
			}
			else if (type == TileID.Containers)
			{
				NetMessage.SendData(34, -1, -1, "", 0, (float)x, (float)y, (float)style, 0, 0, 0);
			}
			else if (type == TileID.Dressers)
			{
				NetMessage.SendData(34, -1, -1, "", 2, (float)x, (float)y, (float)style, 0, 0, 0);
			}
			else if (TileLoader.IsChest(type))
			{
				NetMessage.SendData(34, -1, -1, "", 100, (float)x, (float)y, (float)style, 0, type, 0);
			}
			else if (TileLoader.IsDresser(type))
			{
				NetMessage.SendData(34, -1, -1, "", 102, (float)x, (float)y, (float)style, 0, type, 0);
			}
			return num;
		}

		public static int CreateChest(int X, int Y, int id = -1)
		{
			int num = id;
			if (num == -1)
			{
				num = Chest.FindEmptyChest(X, Y, 21, 0, 1);
				if (num == -1)
				{
					return -1;
				}
				if (Main.netMode == 1)
				{
					return num;
				}
			}
			Main.chest[num] = new Chest(false);
			Main.chest[num].x = X;
			Main.chest[num].y = Y;
			for (int i = 0; i < 40; i++)
			{
				Main.chest[num].item[i] = new Item();
			}
			return num;
		}

		public static bool CanDestroyChest(int X, int Y)
		{
			for (int i = 0; i < 1000; i++)
			{
				Chest chest = Main.chest[i];
				if (chest != null && chest.x == X && chest.y == Y)
				{
					for (int j = 0; j < 40; j++)
					{
						if (chest.item[j] != null && chest.item[j].type > 0 && chest.item[j].stack > 0)
						{
							return false;
						}
					}
					return true;
				}
			}
			return true;
		}

		public static bool DestroyChest(int X, int Y)
		{
			for (int i = 0; i < 1000; i++)
			{
				Chest chest = Main.chest[i];
				if (chest != null && chest.x == X && chest.y == Y)
				{
					for (int j = 0; j < 40; j++)
					{
						if (chest.item[j] != null && chest.item[j].type > 0 && chest.item[j].stack > 0)
						{
							return false;
						}
					}
					Main.chest[i] = null;
					if (Main.player[Main.myPlayer].chest == i)
					{
						Main.player[Main.myPlayer].chest = -1;
					}
					Recipe.FindRecipes();
					return true;
				}
			}
			return true;
		}

		public static void DestroyChestDirect(int X, int Y, int id)
		{
			if (id < 0 || id >= Main.chest.Length)
			{
				return;
			}
			try
			{
				Chest chest = Main.chest[id];
				if (chest != null)
				{
					if (chest.x == X && chest.y == Y)
					{
						Main.chest[id] = null;
						if (Main.player[Main.myPlayer].chest == id)
						{
							Main.player[Main.myPlayer].chest = -1;
						}
						Recipe.FindRecipes();
					}
				}
			}
			catch
			{
			}
		}

		public void AddShop(Item newItem)
		{
			int i = 0;
			while (i < 39)
			{
				if (this.item[i] == null || this.item[i].type == 0)
				{
					this.item[i] = newItem.Clone();
					this.item[i].favorited = false;
					this.item[i].buyOnce = true;
					if (this.item[i].value <= 0)
					{
						break;
					}
					this.item[i].value = this.item[i].value / 5;
					if (this.item[i].value < 1)
					{
						this.item[i].value = 1;
						return;
					}
					break;
				}
				else
				{
					i++;
				}
			}
		}

		public static void SetupTravelShop()
		{
			for (int i = 0; i < 40; i++)
			{
				Main.travelShop[i] = 0;
			}
			int num = Main.rand.Next(4, 7);
			if (Main.rand.Next(4) == 0)
			{
				num++;
			}
			if (Main.rand.Next(8) == 0)
			{
				num++;
			}
			if (Main.rand.Next(16) == 0)
			{
				num++;
			}
			if (Main.rand.Next(32) == 0)
			{
				num++;
			}
			if (Main.expertMode && Main.rand.Next(2) == 0)
			{
				num++;
			}
			int num2 = 0;
			int j = 0;
			int[] array = new int[]
			{
				100,
				200,
				300,
				400,
				500,
				600
			};
			while (j < num)
			{
				int num3 = 0;
				if (Main.rand.Next(array[4]) == 0)
				{
					num3 = 3309;
				}
				if (Main.rand.Next(array[3]) == 0)
				{
					num3 = 3314;
				}
				if (Main.rand.Next(array[5]) == 0)
				{
					num3 = 1987;
				}
				if (Main.rand.Next(array[4]) == 0 && Main.hardMode)
				{
					num3 = 2270;
				}
				if (Main.rand.Next(array[4]) == 0)
				{
					num3 = 2278;
				}
				if (Main.rand.Next(array[4]) == 0)
				{
					num3 = 2271;
				}
				if (Main.rand.Next(array[3]) == 0 && Main.hardMode && NPC.downedPlantBoss)
				{
					num3 = 2223;
				}
				if (Main.rand.Next(array[3]) == 0)
				{
					num3 = 2272;
				}
				if (Main.rand.Next(array[3]) == 0)
				{
					num3 = 2219;
				}
				if (Main.rand.Next(array[3]) == 0)
				{
					num3 = 2276;
				}
				if (Main.rand.Next(array[3]) == 0)
				{
					num3 = 2284;
				}
				if (Main.rand.Next(array[3]) == 0)
				{
					num3 = 2285;
				}
				if (Main.rand.Next(array[3]) == 0)
				{
					num3 = 2286;
				}
				if (Main.rand.Next(array[3]) == 0)
				{
					num3 = 2287;
				}
				if (Main.rand.Next(array[3]) == 0)
				{
					num3 = 2296;
				}
				if (Main.rand.Next(array[3]) == 0)
				{
					num3 = 3628;
				}
				if (Main.rand.Next(array[2]) == 0 && WorldGen.shadowOrbSmashed)
				{
					num3 = 2269;
				}
				if (Main.rand.Next(array[2]) == 0)
				{
					num3 = 2177;
				}
				if (Main.rand.Next(array[2]) == 0)
				{
					num3 = 1988;
				}
				if (Main.rand.Next(array[2]) == 0)
				{
					num3 = 2275;
				}
				if (Main.rand.Next(array[2]) == 0)
				{
					num3 = 2279;
				}
				if (Main.rand.Next(array[2]) == 0)
				{
					num3 = 2277;
				}
				if (Main.rand.Next(array[2]) == 0 && NPC.downedBoss1)
				{
					num3 = 3262;
				}
				if (Main.rand.Next(array[2]) == 0 && NPC.downedMechBossAny)
				{
					num3 = 3284;
				}
				if (Main.rand.Next(array[2]) == 0 && Main.hardMode && NPC.downedMoonlord)
				{
					num3 = 3596;
				}
				if (Main.rand.Next(array[2]) == 0 && Main.hardMode && NPC.downedMartians)
				{
					num3 = 2865;
				}
				if (Main.rand.Next(array[2]) == 0 && Main.hardMode && NPC.downedMartians)
				{
					num3 = 2866;
				}
				if (Main.rand.Next(array[2]) == 0 && Main.hardMode && NPC.downedMartians)
				{
					num3 = 2867;
				}
				if (Main.rand.Next(array[2]) == 0 && Main.xMas)
				{
					num3 = 3055;
				}
				if (Main.rand.Next(array[2]) == 0 && Main.xMas)
				{
					num3 = 3056;
				}
				if (Main.rand.Next(array[2]) == 0 && Main.xMas)
				{
					num3 = 3057;
				}
				if (Main.rand.Next(array[2]) == 0 && Main.xMas)
				{
					num3 = 3058;
				}
				if (Main.rand.Next(array[2]) == 0 && Main.xMas)
				{
					num3 = 3059;
				}
				if (Main.rand.Next(array[1]) == 0)
				{
					num3 = 2214;
				}
				if (Main.rand.Next(array[1]) == 0)
				{
					num3 = 2215;
				}
				if (Main.rand.Next(array[1]) == 0)
				{
					num3 = 2216;
				}
				if (Main.rand.Next(array[1]) == 0)
				{
					num3 = 2217;
				}
				if (Main.rand.Next(array[1]) == 0)
				{
					num3 = 3624;
				}
				if (Main.rand.Next(array[1]) == 0)
				{
					num3 = 2273;
				}
				if (Main.rand.Next(array[1]) == 0)
				{
					num3 = 2274;
				}
				if (Main.rand.Next(array[0]) == 0)
				{
					num3 = 2266;
				}
				if (Main.rand.Next(array[0]) == 0)
				{
					num3 = 2267;
				}
				if (Main.rand.Next(array[0]) == 0)
				{
					num3 = 2268;
				}
				if (Main.rand.Next(array[0]) == 0)
				{
					num3 = 2281 + Main.rand.Next(3);
				}
				if (Main.rand.Next(array[0]) == 0)
				{
					num3 = 2258;
				}
				if (Main.rand.Next(array[0]) == 0)
				{
					num3 = 2242;
				}
				if (Main.rand.Next(array[0]) == 0)
				{
					num3 = 2260;
				}
				if (Main.rand.Next(array[0]) == 0)
				{
					num3 = 3637;
				}
				if (Main.rand.Next(array[0]) == 0)
				{
					num3 = 3119;
				}
				if (Main.rand.Next(array[0]) == 0)
				{
					num3 = 3118;
				}
				if (Main.rand.Next(array[0]) == 0)
				{
					num3 = 3099;
				}
				if (num3 != 0)
				{
					for (int k = 0; k < 40; k++)
					{
						if (Main.travelShop[k] == num3)
						{
							num3 = 0;
							break;
						}
						if (num3 == 3637)
						{
							switch (Main.travelShop[k])
							{
								case 3621:
								case 3622:
								case 3633:
								case 3634:
								case 3635:
								case 3636:
								case 3637:
								case 3638:
								case 3639:
								case 3640:
								case 3641:
								case 3642:
									num3 = 0;
									break;
							}
							if (num3 == 0)
							{
								break;
							}
						}
					}
				}
				if (num3 != 0)
				{
					j++;
					Main.travelShop[num2] = num3;
					num2++;
					if (num3 == 2260)
					{
						Main.travelShop[num2] = 2261;
						num2++;
						Main.travelShop[num2] = 2262;
						num2++;
					}
					if (num3 == 3637)
					{
						num2--;
						switch (Main.rand.Next(6))
						{
							case 0:
								Main.travelShop[num2++] = 3637;
								Main.travelShop[num2++] = 3642;
								break;
							case 1:
								Main.travelShop[num2++] = 3621;
								Main.travelShop[num2++] = 3622;
								break;
							case 2:
								Main.travelShop[num2++] = 3634;
								Main.travelShop[num2++] = 3639;
								break;
							case 3:
								Main.travelShop[num2++] = 3633;
								Main.travelShop[num2++] = 3638;
								break;
							case 4:
								Main.travelShop[num2++] = 3635;
								Main.travelShop[num2++] = 3640;
								break;
							case 5:
								Main.travelShop[num2++] = 3636;
								Main.travelShop[num2++] = 3641;
								break;
						}
						//patch file: num2
					}
				}
			}
			NPCLoader.SetupTravelShop(Main.travelShop, ref num2);
		}

		public void SetupShop(int type)
		{
			for (int i = 0; i < 40; i++)
			{
				this.item[i] = new Item();
			}
			int num = 0;
			if (type == 1)
			{
				this.item[num].SetDefaults("Mining Helmet");
				num++;
				this.item[num].SetDefaults("Piggy Bank");
				num++;
				this.item[num].SetDefaults("Iron Anvil");
				num++;
				this.item[num].SetDefaults(1991, false);
				num++;
				this.item[num].SetDefaults("Copper Pickaxe");
				num++;
				this.item[num].SetDefaults("Copper Axe");
				num++;
				this.item[num].SetDefaults("Torch");
				num++;
				this.item[num].SetDefaults("Lesser Healing Potion");
				num++;
				this.item[num].SetDefaults("Lesser Mana Potion");
				num++;
				this.item[num].SetDefaults("Wooden Arrow");
				num++;
				this.item[num].SetDefaults("Shuriken");
				num++;
				this.item[num].SetDefaults("Rope");
				num++;
				if (Main.player[Main.myPlayer].ZoneSnow)
				{
					this.item[num].SetDefaults(967, false);
					num++;
				}
				if (Main.bloodMoon)
				{
					this.item[num].SetDefaults("Throwing Knife");
					num++;
				}
				if (!Main.dayTime)
				{
					this.item[num].SetDefaults("Glowstick");
					num++;
				}
				if (NPC.downedBoss3)
				{
					this.item[num].SetDefaults("Safe");
					num++;
				}
				if (Main.hardMode)
				{
					this.item[num].SetDefaults(488, false);
					num++;
				}
				for (int j = 0; j < 58; j++)
				{
					if (Main.player[Main.myPlayer].inventory[j].type == 930)
					{
						this.item[num].SetDefaults(931, false);
						num++;
						this.item[num].SetDefaults(1614, false);
						num++;
						break;
					}
				}
				this.item[num].SetDefaults(1786, false);
				num++;
				if (Main.hardMode)
				{
					this.item[num].SetDefaults(1348, false);
					num++;
				}
				if (Main.player[Main.myPlayer].HasItem(3107))
				{
					this.item[num].SetDefaults(3108, false);
					num++;
				}
				if (Main.halloween)
				{
					this.item[num++].SetDefaults(3242, false);
					this.item[num++].SetDefaults(3243, false);
					this.item[num++].SetDefaults(3244, false);
				}
			}
			else if (type == 2)
			{
				this.item[num].SetDefaults("Musket Ball");
				num++;
				if (Main.bloodMoon || Main.hardMode)
				{
					this.item[num].SetDefaults("Silver Bullet");
					num++;
				}
				if ((NPC.downedBoss2 && !Main.dayTime) || Main.hardMode)
				{
					this.item[num].SetDefaults(47, false);
					num++;
				}
				this.item[num].SetDefaults("Flintlock Pistol");
				num++;
				this.item[num].SetDefaults("Minishark");
				num++;
				if (!Main.dayTime)
				{
					this.item[num].SetDefaults(324, false);
					num++;
				}
				if (Main.hardMode)
				{
					this.item[num].SetDefaults(534, false);
					num++;
				}
				if (Main.hardMode)
				{
					this.item[num].SetDefaults(1432, false);
					num++;
				}
				if (Main.player[Main.myPlayer].HasItem(1258))
				{
					this.item[num].SetDefaults(1261, false);
					num++;
				}
				if (Main.player[Main.myPlayer].HasItem(1835))
				{
					this.item[num].SetDefaults(1836, false);
					num++;
				}
				if (Main.player[Main.myPlayer].HasItem(3107))
				{
					this.item[num].SetDefaults(3108, false);
					num++;
				}
				if (Main.player[Main.myPlayer].HasItem(1782))
				{
					this.item[num].SetDefaults(1783, false);
					num++;
				}
				if (Main.player[Main.myPlayer].HasItem(1784))
				{
					this.item[num].SetDefaults(1785, false);
					num++;
				}
				if (Main.halloween)
				{
					this.item[num].SetDefaults(1736, false);
					num++;
					this.item[num].SetDefaults(1737, false);
					num++;
					this.item[num].SetDefaults(1738, false);
					num++;
				}
			}
			else if (type == 3)
			{
				if (Main.bloodMoon)
				{
					if (WorldGen.crimson)
					{
						this.item[num].SetDefaults(2886, false);
						num++;
						this.item[num].SetDefaults(2171, false);
						num++;
					}
					else
					{
						this.item[num].SetDefaults(67, false);
						num++;
						this.item[num].SetDefaults(59, false);
						num++;
					}
				}
				else
				{
					this.item[num].SetDefaults("Purification Powder");
					num++;
					this.item[num].SetDefaults("Grass Seeds");
					num++;
					this.item[num].SetDefaults("Sunflower");
					num++;
				}
				this.item[num].SetDefaults("Acorn");
				num++;
				this.item[num].SetDefaults(114, false);
				num++;
				this.item[num].SetDefaults(1828, false);
				num++;
				this.item[num].SetDefaults(745, false);
				num++;
				this.item[num].SetDefaults(747, false);
				num++;
				if (Main.hardMode)
				{
					this.item[num].SetDefaults(746, false);
					num++;
				}
				if (Main.hardMode)
				{
					this.item[num].SetDefaults(369, false);
					num++;
				}
				if (Main.shroomTiles > 50)
				{
					this.item[num].SetDefaults(194, false);
					num++;
				}
				if (Main.halloween)
				{
					this.item[num].SetDefaults(1853, false);
					num++;
					this.item[num].SetDefaults(1854, false);
					num++;
				}
				if (NPC.downedSlimeKing)
				{
					this.item[num].SetDefaults(3215, false);
					num++;
				}
				if (NPC.downedQueenBee)
				{
					this.item[num].SetDefaults(3216, false);
					num++;
				}
				if (NPC.downedBoss1)
				{
					this.item[num].SetDefaults(3219, false);
					num++;
				}
				if (NPC.downedBoss2)
				{
					if (WorldGen.crimson)
					{
						this.item[num].SetDefaults(3218, false);
						num++;
					}
					else
					{
						this.item[num].SetDefaults(3217, false);
						num++;
					}
				}
				if (NPC.downedBoss3)
				{
					this.item[num].SetDefaults(3220, false);
					num++;
					this.item[num].SetDefaults(3221, false);
					num++;
				}
				if (Main.hardMode)
				{
					this.item[num].SetDefaults(3222, false);
					num++;
				}
			}
			else if (type == 4)
			{
				this.item[num].SetDefaults("Grenade");
				num++;
				this.item[num].SetDefaults("Bomb");
				num++;
				this.item[num].SetDefaults("Dynamite");
				num++;
				if (Main.hardMode)
				{
					this.item[num].SetDefaults("Hellfire Arrow");
					num++;
				}
				if (Main.hardMode && NPC.downedPlantBoss && NPC.downedPirates)
				{
					this.item[num].SetDefaults(937, false);
					num++;
				}
				if (Main.hardMode)
				{
					this.item[num].SetDefaults(1347, false);
					num++;
				}
			}
			else if (type == 5)
			{
				this.item[num].SetDefaults(254, false);
				num++;
				this.item[num].SetDefaults(981, false);
				num++;
				if (Main.dayTime)
				{
					this.item[num].SetDefaults(242, false);
					num++;
				}
				if (Main.moonPhase == 0)
				{
					this.item[num].SetDefaults(245, false);
					num++;
					this.item[num].SetDefaults(246, false);
					num++;
					if (!Main.dayTime)
					{
						this.item[num++].SetDefaults(1288, false);
						this.item[num++].SetDefaults(1289, false);
					}
				}
				else if (Main.moonPhase == 1)
				{
					this.item[num].SetDefaults(325, false);
					num++;
					this.item[num].SetDefaults(326, false);
					num++;
				}
				this.item[num].SetDefaults(269, false);
				num++;
				this.item[num].SetDefaults(270, false);
				num++;
				this.item[num].SetDefaults(271, false);
				num++;
				if (NPC.downedClown)
				{
					this.item[num].SetDefaults(503, false);
					num++;
					this.item[num].SetDefaults(504, false);
					num++;
					this.item[num].SetDefaults(505, false);
					num++;
				}
				if (Main.bloodMoon)
				{
					this.item[num].SetDefaults(322, false);
					num++;
					if (!Main.dayTime)
					{
						this.item[num++].SetDefaults(3362, false);
						this.item[num++].SetDefaults(3363, false);
					}
				}
				if (NPC.downedAncientCultist)
				{
					if (Main.dayTime)
					{
						this.item[num++].SetDefaults(2856, false);
						this.item[num++].SetDefaults(2858, false);
					}
					else
					{
						this.item[num++].SetDefaults(2857, false);
						this.item[num++].SetDefaults(2859, false);
					}
				}
				if (NPC.AnyNPCs(441))
				{
					this.item[num++].SetDefaults(3242, false);
					this.item[num++].SetDefaults(3243, false);
					this.item[num++].SetDefaults(3244, false);
				}
				if (Main.player[Main.myPlayer].ZoneSnow)
				{
					this.item[num].SetDefaults(1429, false);
					num++;
				}
				if (Main.halloween)
				{
					this.item[num].SetDefaults(1740, false);
					num++;
				}
				if (Main.hardMode)
				{
					if (Main.moonPhase == 2)
					{
						this.item[num].SetDefaults(869, false);
						num++;
					}
					if (Main.moonPhase == 4)
					{
						this.item[num].SetDefaults(864, false);
						num++;
						this.item[num].SetDefaults(865, false);
						num++;
					}
					if (Main.moonPhase == 6)
					{
						this.item[num].SetDefaults(873, false);
						num++;
						this.item[num].SetDefaults(874, false);
						num++;
						this.item[num].SetDefaults(875, false);
						num++;
					}
				}
				if (NPC.downedFrost)
				{
					this.item[num].SetDefaults(1275, false);
					num++;
					this.item[num].SetDefaults(1276, false);
					num++;
				}
				if (Main.halloween)
				{
					this.item[num++].SetDefaults(3246, false);
					this.item[num++].SetDefaults(3247, false);
				}
				if (BirthdayParty.PartyIsUp)
				{
					this.item[num++].SetDefaults(3730, false);
					this.item[num++].SetDefaults(3731, false);
					this.item[num++].SetDefaults(3733, false);
					this.item[num++].SetDefaults(3734, false);
					this.item[num++].SetDefaults(3735, false);
				}
			}
			else if (type == 6)
			{
				this.item[num].SetDefaults(128, false);
				num++;
				this.item[num].SetDefaults(486, false);
				num++;
				this.item[num].SetDefaults(398, false);
				num++;
				this.item[num].SetDefaults(84, false);
				num++;
				this.item[num].SetDefaults(407, false);
				num++;
				this.item[num].SetDefaults(161, false);
				num++;
			}
			else if (type == 7)
			{
				this.item[num].SetDefaults(487, false);
				num++;
				this.item[num].SetDefaults(496, false);
				num++;
				this.item[num].SetDefaults(500, false);
				num++;
				this.item[num].SetDefaults(507, false);
				num++;
				this.item[num].SetDefaults(508, false);
				num++;
				this.item[num].SetDefaults(531, false);
				num++;
				this.item[num].SetDefaults(576, false);
				num++;
				this.item[num].SetDefaults(3186, false);
				num++;
				if (Main.halloween)
				{
					this.item[num].SetDefaults(1739, false);
					num++;
				}
			}
			else if (type == 8)
			{
				this.item[num].SetDefaults(509, false);
				num++;
				this.item[num].SetDefaults(850, false);
				num++;
				this.item[num].SetDefaults(851, false);
				num++;
				this.item[num].SetDefaults(3612, false);
				num++;
				this.item[num].SetDefaults(510, false);
				num++;
				this.item[num].SetDefaults(530, false);
				num++;
				this.item[num].SetDefaults(513, false);
				num++;
				this.item[num].SetDefaults(538, false);
				num++;
				this.item[num].SetDefaults(529, false);
				num++;
				this.item[num].SetDefaults(541, false);
				num++;
				this.item[num].SetDefaults(542, false);
				num++;
				this.item[num].SetDefaults(543, false);
				num++;
				this.item[num].SetDefaults(852, false);
				num++;
				this.item[num].SetDefaults(853, false);
				num++;
				this.item[num++].SetDefaults(3707, false);
				this.item[num].SetDefaults(2739, false);
				num++;
				this.item[num].SetDefaults(849, false);
				num++;
				this.item[num++].SetDefaults(3616, false);
				this.item[num++].SetDefaults(2799, false);
				this.item[num++].SetDefaults(3619, false);
				this.item[num++].SetDefaults(3627, false);
				this.item[num++].SetDefaults(3629, false);
				if (NPC.AnyNPCs(369) && Main.hardMode && Main.moonPhase == 3)
				{
					this.item[num].SetDefaults(2295, false);
					num++;
				}
			}
			else if (type == 9)
			{
				this.item[num].SetDefaults(588, false);
				num++;
				this.item[num].SetDefaults(589, false);
				num++;
				this.item[num].SetDefaults(590, false);
				num++;
				this.item[num].SetDefaults(597, false);
				num++;
				this.item[num].SetDefaults(598, false);
				num++;
				this.item[num].SetDefaults(596, false);
				num++;
				for (int k = 1873; k < 1906; k++)
				{
					this.item[num].SetDefaults(k, false);
					num++;
				}
			}
			else if (type == 10)
			{
				if (NPC.downedMechBossAny)
				{
					this.item[num].SetDefaults(756, false);
					num++;
					this.item[num].SetDefaults(787, false);
					num++;
				}
				this.item[num].SetDefaults(868, false);
				num++;
				if (NPC.downedPlantBoss)
				{
					this.item[num].SetDefaults(1551, false);
					num++;
				}
				this.item[num].SetDefaults(1181, false);
				num++;
				this.item[num].SetDefaults(783, false);
				num++;
			}
			else if (type == 11)
			{
				this.item[num].SetDefaults(779, false);
				num++;
				if (Main.moonPhase >= 4)
				{
					this.item[num].SetDefaults(748, false);
					num++;
				}
				else
				{
					this.item[num].SetDefaults(839, false);
					num++;
					this.item[num].SetDefaults(840, false);
					num++;
					this.item[num].SetDefaults(841, false);
					num++;
				}
				if (NPC.downedGolemBoss)
				{
					this.item[num].SetDefaults(948, false);
					num++;
				}
				this.item[num++].SetDefaults(3623, false);
				this.item[num++].SetDefaults(3603, false);
				this.item[num++].SetDefaults(3604, false);
				this.item[num++].SetDefaults(3607, false);
				this.item[num++].SetDefaults(3605, false);
				this.item[num++].SetDefaults(3606, false);
				this.item[num++].SetDefaults(3608, false);
				this.item[num++].SetDefaults(3618, false);
				this.item[num++].SetDefaults(3602, false);
				this.item[num++].SetDefaults(3663, false);
				this.item[num++].SetDefaults(3609, false);
				this.item[num++].SetDefaults(3610, false);
				this.item[num].SetDefaults(995, false);
				num++;
				if (NPC.downedBoss1 && NPC.downedBoss2 && NPC.downedBoss3)
				{
					this.item[num].SetDefaults(2203, false);
					num++;
				}
				if (WorldGen.crimson)
				{
					this.item[num].SetDefaults(2193, false);
					num++;
				}
				this.item[num].SetDefaults(1263, false);
				num++;
				if (Main.eclipse || Main.bloodMoon)
				{
					if (WorldGen.crimson)
					{
						this.item[num].SetDefaults(784, false);
						num++;
					}
					else
					{
						this.item[num].SetDefaults(782, false);
						num++;
					}
				}
				else if (Main.player[Main.myPlayer].ZoneHoly)
				{
					this.item[num].SetDefaults(781, false);
					num++;
				}
				else
				{
					this.item[num].SetDefaults(780, false);
					num++;
				}
				if (Main.hardMode)
				{
					this.item[num].SetDefaults(1344, false);
					num++;
				}
				if (Main.halloween)
				{
					this.item[num].SetDefaults(1742, false);
					num++;
				}
			}
			else if (type == 12)
			{
				this.item[num].SetDefaults(1037, false);
				num++;
				this.item[num].SetDefaults(2874, false);
				num++;
				this.item[num].SetDefaults(1120, false);
				num++;
				if (Main.netMode == 1)
				{
					this.item[num].SetDefaults(1969, false);
					num++;
				}
				if (Main.halloween)
				{
					this.item[num].SetDefaults(3248, false);
					num++;
					this.item[num].SetDefaults(1741, false);
					num++;
				}
				if (Main.moonPhase == 0)
				{
					this.item[num].SetDefaults(2871, false);
					num++;
					this.item[num].SetDefaults(2872, false);
					num++;
				}
			}
			else if (type == 13)
			{
				this.item[num].SetDefaults(859, false);
				num++;
				this.item[num].SetDefaults(1000, false);
				num++;
				this.item[num].SetDefaults(1168, false);
				num++;
				this.item[num].SetDefaults(1449, false);
				num++;
				this.item[num].SetDefaults(1345, false);
				num++;
				this.item[num].SetDefaults(1450, false);
				num++;
				this.item[num++].SetDefaults(3253, false);
				this.item[num++].SetDefaults(2700, false);
				this.item[num++].SetDefaults(2738, false);
				if (Main.player[Main.myPlayer].HasItem(3548))
				{
					this.item[num].SetDefaults(3548, false);
					num++;
				}
				if (NPC.AnyNPCs(229))
				{
					this.item[num++].SetDefaults(3369, false);
				}
				if (Main.hardMode)
				{
					this.item[num].SetDefaults(3214, false);
					num++;
					this.item[num].SetDefaults(2868, false);
					num++;
					this.item[num].SetDefaults(970, false);
					num++;
					this.item[num].SetDefaults(971, false);
					num++;
					this.item[num].SetDefaults(972, false);
					num++;
					this.item[num].SetDefaults(973, false);
					num++;
				}
				this.item[num++].SetDefaults(3747, false);
				this.item[num++].SetDefaults(3732, false);
				this.item[num++].SetDefaults(3742, false);
				if (BirthdayParty.PartyIsUp)
				{
					this.item[num++].SetDefaults(3749, false);
					this.item[num++].SetDefaults(3746, false);
					this.item[num++].SetDefaults(3739, false);
					this.item[num++].SetDefaults(3740, false);
					this.item[num++].SetDefaults(3741, false);
					this.item[num++].SetDefaults(3737, false);
					this.item[num++].SetDefaults(3738, false);
					this.item[num++].SetDefaults(3736, false);
					this.item[num++].SetDefaults(3745, false);
					this.item[num++].SetDefaults(3744, false);
					this.item[num++].SetDefaults(3743, false);
				}
			}
			else if (type == 14)
			{
				this.item[num].SetDefaults(771, false);
				num++;
				if (Main.bloodMoon)
				{
					this.item[num].SetDefaults(772, false);
					num++;
				}
				if (!Main.dayTime || Main.eclipse)
				{
					this.item[num].SetDefaults(773, false);
					num++;
				}
				if (Main.eclipse)
				{
					this.item[num].SetDefaults(774, false);
					num++;
				}
				if (Main.hardMode)
				{
					this.item[num].SetDefaults(760, false);
					num++;
				}
				if (Main.hardMode)
				{
					this.item[num].SetDefaults(1346, false);
					num++;
				}
				if (Main.halloween)
				{
					this.item[num].SetDefaults(1743, false);
					num++;
					this.item[num].SetDefaults(1744, false);
					num++;
					this.item[num].SetDefaults(1745, false);
					num++;
				}
				if (NPC.downedMartians)
				{
					this.item[num++].SetDefaults(2862, false);
					this.item[num++].SetDefaults(3109, false);
				}
				if (Main.player[Main.myPlayer].HasItem(3384) || Main.player[Main.myPlayer].HasItem(3664))
				{
					this.item[num].SetDefaults(3664, false);
					num++;
				}
			}
			else if (type == 15)
			{
				this.item[num].SetDefaults(1071, false);
				num++;
				this.item[num].SetDefaults(1072, false);
				num++;
				this.item[num].SetDefaults(1100, false);
				num++;
				for (int l = 1073; l <= 1084; l++)
				{
					this.item[num].SetDefaults(l, false);
					num++;
				}
				this.item[num].SetDefaults(1097, false);
				num++;
				this.item[num].SetDefaults(1099, false);
				num++;
				this.item[num].SetDefaults(1098, false);
				num++;
				this.item[num].SetDefaults(1966, false);
				num++;
				if (Main.hardMode)
				{
					this.item[num].SetDefaults(1967, false);
					num++;
					this.item[num].SetDefaults(1968, false);
					num++;
				}
				this.item[num].SetDefaults(1490, false);
				num++;
				if (Main.moonPhase <= 1)
				{
					this.item[num].SetDefaults(1481, false);
					num++;
				}
				else if (Main.moonPhase <= 3)
				{
					this.item[num].SetDefaults(1482, false);
					num++;
				}
				else if (Main.moonPhase <= 5)
				{
					this.item[num].SetDefaults(1483, false);
					num++;
				}
				else
				{
					this.item[num].SetDefaults(1484, false);
					num++;
				}
				if (Main.player[Main.myPlayer].ZoneCrimson)
				{
					this.item[num].SetDefaults(1492, false);
					num++;
				}
				if (Main.player[Main.myPlayer].ZoneCorrupt)
				{
					this.item[num].SetDefaults(1488, false);
					num++;
				}
				if (Main.player[Main.myPlayer].ZoneHoly)
				{
					this.item[num].SetDefaults(1489, false);
					num++;
				}
				if (Main.player[Main.myPlayer].ZoneJungle)
				{
					this.item[num].SetDefaults(1486, false);
					num++;
				}
				if (Main.player[Main.myPlayer].ZoneSnow)
				{
					this.item[num].SetDefaults(1487, false);
					num++;
				}
				if (Main.sandTiles > 1000)
				{
					this.item[num].SetDefaults(1491, false);
					num++;
				}
				if (Main.bloodMoon)
				{
					this.item[num].SetDefaults(1493, false);
					num++;
				}
				if ((double)(Main.player[Main.myPlayer].position.Y / 16f) < Main.worldSurface * 0.34999999403953552)
				{
					this.item[num].SetDefaults(1485, false);
					num++;
				}
				if ((double)(Main.player[Main.myPlayer].position.Y / 16f) < Main.worldSurface * 0.34999999403953552 && Main.hardMode)
				{
					this.item[num].SetDefaults(1494, false);
					num++;
				}
				if (Main.xMas)
				{
					for (int m = 1948; m <= 1957; m++)
					{
						this.item[num].SetDefaults(m, false);
						num++;
					}
				}
				for (int n = 2158; n <= 2160; n++)
				{
					if (num < 39)
					{
						this.item[num].SetDefaults(n, false);
					}
					num++;
				}
				for (int num2 = 2008; num2 <= 2014; num2++)
				{
					if (num < 39)
					{
						this.item[num].SetDefaults(num2, false);
					}
					num++;
				}
			}
			else if (type == 16)
			{
				this.item[num].SetDefaults(1430, false);
				num++;
				this.item[num].SetDefaults(986, false);
				num++;
				if (NPC.AnyNPCs(108))
				{
					this.item[num++].SetDefaults(2999, false);
				}
				if (Main.hardMode && NPC.downedPlantBoss)
				{
					if (Main.player[Main.myPlayer].HasItem(1157))
					{
						this.item[num].SetDefaults(1159, false);
						num++;
						this.item[num].SetDefaults(1160, false);
						num++;
						this.item[num].SetDefaults(1161, false);
						num++;
						if (!Main.dayTime)
						{
							this.item[num].SetDefaults(1158, false);
							num++;
						}
						if (Main.player[Main.myPlayer].ZoneJungle)
						{
							this.item[num].SetDefaults(1167, false);
							num++;
						}
					}
					this.item[num].SetDefaults(1339, false);
					num++;
				}
				if (Main.hardMode && Main.player[Main.myPlayer].ZoneJungle)
				{
					this.item[num].SetDefaults(1171, false);
					num++;
					if (!Main.dayTime)
					{
						this.item[num].SetDefaults(1162, false);
						num++;
					}
				}
				this.item[num].SetDefaults(909, false);
				num++;
				this.item[num].SetDefaults(910, false);
				num++;
				this.item[num].SetDefaults(940, false);
				num++;
				this.item[num].SetDefaults(941, false);
				num++;
				this.item[num].SetDefaults(942, false);
				num++;
				this.item[num].SetDefaults(943, false);
				num++;
				this.item[num].SetDefaults(944, false);
				num++;
				this.item[num].SetDefaults(945, false);
				num++;
				if (Main.player[Main.myPlayer].HasItem(1835))
				{
					this.item[num].SetDefaults(1836, false);
					num++;
				}
				if (Main.player[Main.myPlayer].HasItem(1258))
				{
					this.item[num].SetDefaults(1261, false);
					num++;
				}
				if (Main.halloween)
				{
					this.item[num].SetDefaults(1791, false);
					num++;
				}
			}
			else if (type == 17)
			{
				this.item[num].SetDefaults(928, false);
				num++;
				this.item[num].SetDefaults(929, false);
				num++;
				this.item[num].SetDefaults(876, false);
				num++;
				this.item[num].SetDefaults(877, false);
				num++;
				this.item[num].SetDefaults(878, false);
				num++;
				this.item[num].SetDefaults(2434, false);
				num++;
				int num3 = (int)((Main.screenPosition.X + (float)(Main.screenWidth / 2)) / 16f);
				if ((double)(Main.screenPosition.Y / 16f) < Main.worldSurface + 10.0 && (num3 < 380 || num3 > Main.maxTilesX - 380))
				{
					this.item[num].SetDefaults(1180, false);
					num++;
				}
				if (Main.hardMode && NPC.downedMechBossAny && NPC.AnyNPCs(208))
				{
					this.item[num].SetDefaults(1337, false);
					num++;
				}
			}
			else if (type == 18)
			{
				this.item[num].SetDefaults(1990, false);
				num++;
				this.item[num].SetDefaults(1979, false);
				num++;
				if (Main.player[Main.myPlayer].statLifeMax >= 400)
				{
					this.item[num].SetDefaults(1977, false);
					num++;
				}
				if (Main.player[Main.myPlayer].statManaMax >= 200)
				{
					this.item[num].SetDefaults(1978, false);
					num++;
				}
				long num4 = 0L;
				for (int num5 = 0; num5 < 54; num5++)
				{
					if (Main.player[Main.myPlayer].inventory[num5].type == 71)
					{
						num4 += (long)Main.player[Main.myPlayer].inventory[num5].stack;
					}
					if (Main.player[Main.myPlayer].inventory[num5].type == 72)
					{
						num4 += (long)(Main.player[Main.myPlayer].inventory[num5].stack * 100);
					}
					if (Main.player[Main.myPlayer].inventory[num5].type == 73)
					{
						num4 += (long)(Main.player[Main.myPlayer].inventory[num5].stack * 10000);
					}
					if (Main.player[Main.myPlayer].inventory[num5].type == 74)
					{
						num4 += (long)(Main.player[Main.myPlayer].inventory[num5].stack * 1000000);
					}
				}
				if (num4 >= 1000000L)
				{
					this.item[num].SetDefaults(1980, false);
					num++;
				}
				if ((Main.moonPhase % 2 == 0 && Main.dayTime) || (Main.moonPhase % 2 == 1 && !Main.dayTime))
				{
					this.item[num].SetDefaults(1981, false);
					num++;
				}
				if (Main.player[Main.myPlayer].team != 0)
				{
					this.item[num].SetDefaults(1982, false);
					num++;
				}
				if (Main.hardMode)
				{
					this.item[num].SetDefaults(1983, false);
					num++;
				}
				if (NPC.AnyNPCs(208))
				{
					this.item[num].SetDefaults(1984, false);
					num++;
				}
				if (Main.hardMode && NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3)
				{
					this.item[num].SetDefaults(1985, false);
					num++;
				}
				if (Main.hardMode && NPC.downedMechBossAny)
				{
					this.item[num].SetDefaults(1986, false);
					num++;
				}
				if (Main.hardMode && NPC.downedMartians)
				{
					this.item[num].SetDefaults(2863, false);
					num++;
					this.item[num].SetDefaults(3259, false);
					num++;
				}
			}
			else if (type == 19)
			{
				for (int num6 = 0; num6 < 40; num6++)
				{
					if (Main.travelShop[num6] != 0)
					{
						this.item[num].netDefaults(Main.travelShop[num6]);
						num++;
					}
				}
			}
			else if (type == 20)
			{
				if (Main.moonPhase % 2 == 0)
				{
					this.item[num].SetDefaults(3001, false);
				}
				else
				{
					this.item[num].SetDefaults(28, false);
				}
				num++;
				if (!Main.dayTime || Main.moonPhase == 0)
				{
					this.item[num].SetDefaults(3002, false);
				}
				else
				{
					this.item[num].SetDefaults(282, false);
				}
				num++;
				if (Main.time % 60.0 * 60.0 * 6.0 <= 10800.0)
				{
					this.item[num].SetDefaults(3004, false);
				}
				else
				{
					this.item[num].SetDefaults(8, false);
				}
				num++;
				if (Main.moonPhase == 0 || Main.moonPhase == 1 || Main.moonPhase == 4 || Main.moonPhase == 5)
				{
					this.item[num].SetDefaults(3003, false);
				}
				else
				{
					this.item[num].SetDefaults(40, false);
				}
				num++;
				if (Main.moonPhase % 4 == 0)
				{
					this.item[num].SetDefaults(3310, false);
				}
				else if (Main.moonPhase % 4 == 1)
				{
					this.item[num].SetDefaults(3313, false);
				}
				else if (Main.moonPhase % 4 == 2)
				{
					this.item[num].SetDefaults(3312, false);
				}
				else
				{
					this.item[num].SetDefaults(3311, false);
				}
				num++;
				this.item[num].SetDefaults(166, false);
				num++;
				this.item[num].SetDefaults(965, false);
				num++;
				if (Main.hardMode)
				{
					if (Main.moonPhase < 4)
					{
						this.item[num].SetDefaults(3316, false);
					}
					else
					{
						this.item[num].SetDefaults(3315, false);
					}
					num++;
					this.item[num].SetDefaults(3334, false);
					num++;
					if (Main.bloodMoon)
					{
						this.item[num].SetDefaults(3258, false);
						num++;
					}
				}
				if (Main.moonPhase == 0 && !Main.dayTime)
				{
					this.item[num].SetDefaults(3043, false);
					num++;
				}
			}
			else if (type == 21)
			{
				bool flag = Main.hardMode && NPC.downedMechBossAny;
				bool flag2 = Main.hardMode && NPC.downedGolemBoss;
				this.item[num].SetDefaults(353, false);
				num++;
				this.item[num].SetDefaults(3828, false);
				if (flag2)
				{
					this.item[num].shopCustomPrice = new int?(Item.buyPrice(0, 4, 0, 0));
				}
				else if (flag)
				{
					this.item[num].shopCustomPrice = new int?(Item.buyPrice(0, 1, 0, 0));
				}
				else
				{
					this.item[num].shopCustomPrice = new int?(Item.buyPrice(0, 0, 25, 0));
				}
				num++;
				this.item[num].SetDefaults(3816, false);
				num++;
				this.item[num].SetDefaults(3813, false);
				this.item[num].shopCustomPrice = new int?(75);
				this.item[num].shopSpecialCurrency = CustomCurrencyID.DefenderMedals;
				num++;
				num = 10;
				this.item[num].SetDefaults(3818, false);
				this.item[num].shopCustomPrice = new int?(5);
				this.item[num].shopSpecialCurrency = CustomCurrencyID.DefenderMedals;
				num++;
				this.item[num].SetDefaults(3824, false);
				this.item[num].shopCustomPrice = new int?(5);
				this.item[num].shopSpecialCurrency = CustomCurrencyID.DefenderMedals;
				num++;
				this.item[num].SetDefaults(3832, false);
				this.item[num].shopCustomPrice = new int?(5);
				this.item[num].shopSpecialCurrency = CustomCurrencyID.DefenderMedals;
				num++;
				this.item[num].SetDefaults(3829, false);
				this.item[num].shopCustomPrice = new int?(5);
				this.item[num].shopSpecialCurrency = CustomCurrencyID.DefenderMedals;
				if (flag)
				{
					num = 20;
					this.item[num].SetDefaults(3819, false);
					this.item[num].shopCustomPrice = new int?(25);
					this.item[num].shopSpecialCurrency = CustomCurrencyID.DefenderMedals;
					num++;
					this.item[num].SetDefaults(3825, false);
					this.item[num].shopCustomPrice = new int?(25);
					this.item[num].shopSpecialCurrency = CustomCurrencyID.DefenderMedals;
					num++;
					this.item[num].SetDefaults(3833, false);
					this.item[num].shopCustomPrice = new int?(25);
					this.item[num].shopSpecialCurrency = CustomCurrencyID.DefenderMedals;
					num++;
					this.item[num].SetDefaults(3830, false);
					this.item[num].shopCustomPrice = new int?(25);
					this.item[num].shopSpecialCurrency = CustomCurrencyID.DefenderMedals;
				}
				if (flag2)
				{
					num = 30;
					this.item[num].SetDefaults(3820, false);
					this.item[num].shopCustomPrice = new int?(100);
					this.item[num].shopSpecialCurrency = CustomCurrencyID.DefenderMedals;
					num++;
					this.item[num].SetDefaults(3826, false);
					this.item[num].shopCustomPrice = new int?(100);
					this.item[num].shopSpecialCurrency = CustomCurrencyID.DefenderMedals;
					num++;
					this.item[num].SetDefaults(3834, false);
					this.item[num].shopCustomPrice = new int?(100);
					this.item[num].shopSpecialCurrency = CustomCurrencyID.DefenderMedals;
					num++;
					this.item[num].SetDefaults(3831, false);
					this.item[num].shopCustomPrice = new int?(100);
					this.item[num].shopSpecialCurrency = CustomCurrencyID.DefenderMedals;
				}
				if (flag)
				{
					num = 4;
					this.item[num].SetDefaults(3800, false);
					this.item[num].shopCustomPrice = new int?(25);
					this.item[num].shopSpecialCurrency = CustomCurrencyID.DefenderMedals;
					num++;
					this.item[num].SetDefaults(3801, false);
					this.item[num].shopCustomPrice = new int?(25);
					this.item[num].shopSpecialCurrency = CustomCurrencyID.DefenderMedals;
					num++;
					this.item[num].SetDefaults(3802, false);
					this.item[num].shopCustomPrice = new int?(25);
					this.item[num].shopSpecialCurrency = CustomCurrencyID.DefenderMedals;
					num++;
					num = 14;
					this.item[num].SetDefaults(3797, false);
					this.item[num].shopCustomPrice = new int?(25);
					this.item[num].shopSpecialCurrency = CustomCurrencyID.DefenderMedals;
					num++;
					this.item[num].SetDefaults(3798, false);
					this.item[num].shopCustomPrice = new int?(25);
					this.item[num].shopSpecialCurrency = CustomCurrencyID.DefenderMedals;
					num++;
					this.item[num].SetDefaults(3799, false);
					this.item[num].shopCustomPrice = new int?(25);
					this.item[num].shopSpecialCurrency = CustomCurrencyID.DefenderMedals;
					num++;
					num = 24;
					this.item[num].SetDefaults(3803, false);
					this.item[num].shopCustomPrice = new int?(25);
					this.item[num].shopSpecialCurrency = CustomCurrencyID.DefenderMedals;
					num++;
					this.item[num].SetDefaults(3804, false);
					this.item[num].shopCustomPrice = new int?(25);
					this.item[num].shopSpecialCurrency = CustomCurrencyID.DefenderMedals;
					num++;
					this.item[num].SetDefaults(3805, false);
					this.item[num].shopCustomPrice = new int?(25);
					this.item[num].shopSpecialCurrency = CustomCurrencyID.DefenderMedals;
					num++;
					num = 34;
					this.item[num].SetDefaults(3806, false);
					this.item[num].shopCustomPrice = new int?(25);
					this.item[num].shopSpecialCurrency = CustomCurrencyID.DefenderMedals;
					num++;
					this.item[num].SetDefaults(3807, false);
					this.item[num].shopCustomPrice = new int?(25);
					this.item[num].shopSpecialCurrency = CustomCurrencyID.DefenderMedals;
					num++;
					this.item[num].SetDefaults(3808, false);
					this.item[num].shopCustomPrice = new int?(25);
					this.item[num].shopSpecialCurrency = CustomCurrencyID.DefenderMedals;
					num++;
				}
				if (flag2)
				{
					num = 7;
					this.item[num].SetDefaults(3871, false);
					this.item[num].shopCustomPrice = new int?(75);
					this.item[num].shopSpecialCurrency = CustomCurrencyID.DefenderMedals;
					num++;
					this.item[num].SetDefaults(3872, false);
					this.item[num].shopCustomPrice = new int?(75);
					this.item[num].shopSpecialCurrency = CustomCurrencyID.DefenderMedals;
					num++;
					this.item[num].SetDefaults(3873, false);
					this.item[num].shopCustomPrice = new int?(75);
					this.item[num].shopSpecialCurrency = CustomCurrencyID.DefenderMedals;
					num++;
					num = 17;
					this.item[num].SetDefaults(3874, false);
					this.item[num].shopCustomPrice = new int?(75);
					this.item[num].shopSpecialCurrency = CustomCurrencyID.DefenderMedals;
					num++;
					this.item[num].SetDefaults(3875, false);
					this.item[num].shopCustomPrice = new int?(75);
					this.item[num].shopSpecialCurrency = CustomCurrencyID.DefenderMedals;
					num++;
					this.item[num].SetDefaults(3876, false);
					this.item[num].shopCustomPrice = new int?(75);
					this.item[num].shopSpecialCurrency = CustomCurrencyID.DefenderMedals;
					num++;
					num = 27;
					this.item[num].SetDefaults(3877, false);
					this.item[num].shopCustomPrice = new int?(75);
					this.item[num].shopSpecialCurrency = CustomCurrencyID.DefenderMedals;
					num++;
					this.item[num].SetDefaults(3878, false);
					this.item[num].shopCustomPrice = new int?(75);
					this.item[num].shopSpecialCurrency = CustomCurrencyID.DefenderMedals;
					num++;
					this.item[num].SetDefaults(3879, false);
					this.item[num].shopCustomPrice = new int?(75);
					this.item[num].shopSpecialCurrency = CustomCurrencyID.DefenderMedals;
					num++;
					num = 37;
					this.item[num].SetDefaults(3880, false);
					this.item[num].shopCustomPrice = new int?(75);
					this.item[num].shopSpecialCurrency = CustomCurrencyID.DefenderMedals;
					num++;
					this.item[num].SetDefaults(3881, false);
					this.item[num].shopCustomPrice = new int?(75);
					this.item[num].shopSpecialCurrency = CustomCurrencyID.DefenderMedals;
					num++;
					this.item[num].SetDefaults(3882, false);
					this.item[num].shopCustomPrice = new int?(75);
					this.item[num].shopSpecialCurrency = CustomCurrencyID.DefenderMedals;
					num++;
				}
			}
			NPCLoader.SetupShop(type, this, ref num);
			if (Main.player[Main.myPlayer].discount)
			{
				for (int num7 = 0; num7 < num; num7++)
				{
					this.item[num7].value = (int)((float)this.item[num7].value * 0.8f);
				}
			}
		}

		public static void UpdateChestFrames()
		{
			bool[] array = new bool[1000];
			for (int i = 0; i < 255; i++)
			{
				if (Main.player[i].active && Main.player[i].chest >= 0 && Main.player[i].chest < 1000)
				{
					array[Main.player[i].chest] = true;
				}
			}
			for (int j = 0; j < 1000; j++)
			{
				Chest chest = Main.chest[j];
				if (chest != null)
				{
					if (array[j])
					{
						chest.frameCounter++;
					}
					else
					{
						chest.frameCounter--;
					}
					if (chest.frameCounter < 0)
					{
						chest.frameCounter = 0;
					}
					if (chest.frameCounter > 10)
					{
						chest.frameCounter = 10;
					}
					if (chest.frameCounter == 0)
					{
						chest.frame = 0;
					}
					else if (chest.frameCounter == 10)
					{
						chest.frame = 2;
					}
					else
					{
						chest.frame = 1;
					}
				}
			}
		}
	}
}
