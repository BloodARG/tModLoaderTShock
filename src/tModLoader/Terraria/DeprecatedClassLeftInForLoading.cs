using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria.ID;

namespace Terraria
{
	public class DeprecatedClassLeftInForLoading
	{
		public const int MaxDummies = 1000;
		public static DeprecatedClassLeftInForLoading[] dummies = new DeprecatedClassLeftInForLoading[1000];
		public short x;
		public short y;
		public int npc;
		public int whoAmI;

		public static void UpdateDummies()
		{
			Dictionary<int, Rectangle> dictionary = new Dictionary<int, Rectangle>();
			bool flag = false;
			Rectangle value = new Rectangle(0, 0, 32, 48);
			value.Inflate(1600, 1600);
			int num = value.X;
			int num2 = value.Y;
			for (int i = 0; i < 1000; i++)
			{
				if (DeprecatedClassLeftInForLoading.dummies[i] != null)
				{
					DeprecatedClassLeftInForLoading.dummies[i].whoAmI = i;
					if (DeprecatedClassLeftInForLoading.dummies[i].npc != -1)
					{
						if (!Main.npc[DeprecatedClassLeftInForLoading.dummies[i].npc].active || Main.npc[DeprecatedClassLeftInForLoading.dummies[i].npc].type != 488 || Main.npc[DeprecatedClassLeftInForLoading.dummies[i].npc].ai[0] != (float)DeprecatedClassLeftInForLoading.dummies[i].x || Main.npc[DeprecatedClassLeftInForLoading.dummies[i].npc].ai[1] != (float)DeprecatedClassLeftInForLoading.dummies[i].y)
						{
							DeprecatedClassLeftInForLoading.dummies[i].Deactivate();
						}
					}
					else
					{
						if (!flag)
						{
							for (int j = 0; j < 255; j++)
							{
								if (Main.player[j].active)
								{
									dictionary[j] = Main.player[j].getRect();
								}
							}
							flag = true;
						}
						value.X = (int)(DeprecatedClassLeftInForLoading.dummies[i].x * 16) + num;
						value.Y = (int)(DeprecatedClassLeftInForLoading.dummies[i].y * 16) + num2;
						bool flag2 = false;
						foreach (KeyValuePair<int, Rectangle> current in dictionary)
						{
							if (current.Value.Intersects(value))
							{
								flag2 = true;
								break;
							}
						}
						if (flag2)
						{
							DeprecatedClassLeftInForLoading.dummies[i].Activate();
						}
					}
				}
			}
		}

		public DeprecatedClassLeftInForLoading(int x, int y)
		{
			this.x = (short)x;
			this.y = (short)y;
			this.npc = -1;
		}

		public static int Find(int x, int y)
		{
			for (int i = 0; i < 1000; i++)
			{
				if (DeprecatedClassLeftInForLoading.dummies[i] != null && (int)DeprecatedClassLeftInForLoading.dummies[i].x == x && (int)DeprecatedClassLeftInForLoading.dummies[i].y == y)
				{
					return i;
				}
			}
			return -1;
		}

		public static int Place(int x, int y)
		{
			int num = -1;
			for (int i = 0; i < 1000; i++)
			{
				if (DeprecatedClassLeftInForLoading.dummies[i] == null)
				{
					num = i;
					break;
				}
			}
			if (num == -1)
			{
				return num;
			}
			DeprecatedClassLeftInForLoading.dummies[num] = new DeprecatedClassLeftInForLoading(x, y);
			return num;
		}

		public static void Kill(int x, int y)
		{
			for (int i = 0; i < 1000; i++)
			{
				DeprecatedClassLeftInForLoading deprecatedClassLeftInForLoading = DeprecatedClassLeftInForLoading.dummies[i];
				if (deprecatedClassLeftInForLoading != null && (int)deprecatedClassLeftInForLoading.x == x && (int)deprecatedClassLeftInForLoading.y == y)
				{
					DeprecatedClassLeftInForLoading.dummies[i] = null;
				}
			}
		}

		public static int Hook_AfterPlacement(int x, int y, int type = 21, int style = 0, int direction = 1)
		{
			if (Main.netMode == 1)
			{
				NetMessage.SendTileSquare(Main.myPlayer, x - 1, y - 1, 3, TileChangeType.None);
				NetMessage.SendData(87, -1, -1, "", x - 1, (float)(y - 2), 0f, 0f, 0, 0, 0);
				return -1;
			}
			return DeprecatedClassLeftInForLoading.Place(x - 1, y - 2);
		}

		public void Activate()
		{
			int num = NPC.NewNPC((int)(this.x * 16 + 16), (int)(this.y * 16 + 48), 488, 100, 0f, 0f, 0f, 0f, 255);
			Main.npc[num].ai[0] = (float)this.x;
			Main.npc[num].ai[1] = (float)this.y;
			Main.npc[num].netUpdate = true;
			this.npc = num;
			if (Main.netMode != 1)
			{
				NetMessage.SendData(86, -1, -1, "", this.whoAmI, (float)this.x, (float)this.y, 0f, 0, 0, 0);
			}
		}

		public void Deactivate()
		{
			if (this.npc != -1)
			{
				Main.npc[this.npc].active = false;
			}
			this.npc = -1;
			if (Main.netMode != 1)
			{
				NetMessage.SendData(86, -1, -1, "", this.whoAmI, (float)this.x, (float)this.y, 0f, 0, 0, 0);
			}
		}

		public override string ToString()
		{
			return string.Concat(new object[]
				{
					this.x,
					"x  ",
					this.y,
					"y npc: ",
					this.npc
				});
		}
	}
}
