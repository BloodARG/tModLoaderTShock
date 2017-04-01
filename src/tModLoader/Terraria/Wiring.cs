using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.GameContent.Events;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terraria
{
	public static class Wiring
	{
		private const int MaxPump = 20;
		private const int MaxMech = 1000;
		public static bool blockPlayerTeleportationForOneIteration;
		public static bool running;
		private static Dictionary<Point16, bool> _wireSkip;
		public static DoubleStack<Point16> _wireList;
		public static DoubleStack<byte> _wireDirectionList;
		public static Dictionary<Point16, byte> _toProcess;
		private static Queue<Point16> _GatesCurrent;
		public static Queue<Point16> _LampsToCheck;
		public static Queue<Point16> _GatesNext;
		private static Dictionary<Point16, bool> _GatesDone;
		private static Dictionary<Point16, byte> _PixelBoxTriggers;
		public static Vector2[] _teleport;
		public static int[] _inPumpX;
		public static int[] _inPumpY;
		public static int _numInPump;
		public static int[] _outPumpX;
		public static int[] _outPumpY;
		public static int _numOutPump;
		private static int[] _mechX;
		private static int[] _mechY;
		private static int _numMechs;
		private static int[] _mechTime;
		public static int _currentWireColor;
		private static int CurrentUser = 254;

		public static void SetCurrentUser(int plr = -1)
		{
			if (plr < 0 || plr >= 255)
			{
				plr = 254;
			}
			if (Main.netMode == 0)
			{
				plr = Main.myPlayer;
			}
			Wiring.CurrentUser = plr;
		}

		public static void Initialize()
		{
			Wiring._wireSkip = new Dictionary<Point16, bool>();
			Wiring._wireList = new DoubleStack<Point16>(1024, 0);
			Wiring._wireDirectionList = new DoubleStack<byte>(1024, 0);
			Wiring._toProcess = new Dictionary<Point16, byte>();
			Wiring._GatesCurrent = new Queue<Point16>();
			Wiring._GatesNext = new Queue<Point16>();
			Wiring._GatesDone = new Dictionary<Point16, bool>();
			Wiring._LampsToCheck = new Queue<Point16>();
			Wiring._PixelBoxTriggers = new Dictionary<Point16, byte>();
			Wiring._inPumpX = new int[20];
			Wiring._inPumpY = new int[20];
			Wiring._outPumpX = new int[20];
			Wiring._outPumpY = new int[20];
			Wiring._teleport = new Vector2[2];
			Wiring._mechX = new int[1000];
			Wiring._mechY = new int[1000];
			Wiring._mechTime = new int[1000];
		}

		public static void SkipWire(int x, int y)
		{
			Wiring._wireSkip[new Point16(x, y)] = true;
		}

		public static void SkipWire(Point16 point)
		{
			Wiring._wireSkip[point] = true;
		}

		public static void UpdateMech()
		{
			Wiring.SetCurrentUser(-1);
			for (int i = Wiring._numMechs - 1; i >= 0; i--)
			{
				Wiring._mechTime[i]--;
				if (Main.tile[Wiring._mechX[i], Wiring._mechY[i]].active() && Main.tile[Wiring._mechX[i], Wiring._mechY[i]].type == 144)
				{
					if (Main.tile[Wiring._mechX[i], Wiring._mechY[i]].frameY == 0)
					{
						Wiring._mechTime[i] = 0;
					}
					else
					{
						int num = (int)(Main.tile[Wiring._mechX[i], Wiring._mechY[i]].frameX / 18);
						if (num == 0)
						{
							num = 60;
						}
						else if (num == 1)
						{
							num = 180;
						}
						else if (num == 2)
						{
							num = 300;
						}
						if (Math.IEEERemainder((double)Wiring._mechTime[i], (double)num) == 0.0)
						{
							Wiring._mechTime[i] = 18000;
							Wiring.TripWire(Wiring._mechX[i], Wiring._mechY[i], 1, 1);
						}
					}
				}
				if (Wiring._mechTime[i] <= 0)
				{
					if (Main.tile[Wiring._mechX[i], Wiring._mechY[i]].active() && Main.tile[Wiring._mechX[i], Wiring._mechY[i]].type == 144)
					{
						Main.tile[Wiring._mechX[i], Wiring._mechY[i]].frameY = 0;
						NetMessage.SendTileSquare(-1, Wiring._mechX[i], Wiring._mechY[i], 1, TileChangeType.None);
					}
					if (Main.tile[Wiring._mechX[i], Wiring._mechY[i]].active() && Main.tile[Wiring._mechX[i], Wiring._mechY[i]].type == 411)
					{
						Tile tile = Main.tile[Wiring._mechX[i], Wiring._mechY[i]];
						int num2 = (int)(tile.frameX % 36 / 18);
						int num3 = (int)(tile.frameY % 36 / 18);
						int num4 = Wiring._mechX[i] - num2;
						int num5 = Wiring._mechY[i] - num3;
						int num6 = 36;
						if (Main.tile[num4, num5].frameX >= 36)
						{
							num6 = -36;
						}
						for (int j = num4; j < num4 + 2; j++)
						{
							for (int k = num5; k < num5 + 2; k++)
							{
								Main.tile[j, k].frameX = (short)((int)Main.tile[j, k].frameX + num6);
							}
						}
						NetMessage.SendTileSquare(-1, num4, num5, 2, TileChangeType.None);
					}
					for (int l = i; l < Wiring._numMechs; l++)
					{
						Wiring._mechX[l] = Wiring._mechX[l + 1];
						Wiring._mechY[l] = Wiring._mechY[l + 1];
						Wiring._mechTime[l] = Wiring._mechTime[l + 1];
					}
					Wiring._numMechs--;
				}
			}
		}

		public static void HitSwitch(int i, int j)
		{
			if (!WorldGen.InWorld(i, j, 0))
			{
				return;
			}
			if (Main.tile[i, j] == null)
			{
				return;
			}
			if (Main.tile[i, j].type == 135 || Main.tile[i, j].type == 314 || Main.tile[i, j].type == 423 || Main.tile[i, j].type == 428 || Main.tile[i, j].type == 442)
			{
				Main.PlaySound(28, i * 16, j * 16, 0, 1f, 0f);
				Wiring.TripWire(i, j, 1, 1);
				return;
			}
			if (Main.tile[i, j].type == 440)
			{
				Main.PlaySound(28, i * 16 + 16, j * 16 + 16, 0, 1f, 0f);
				Wiring.TripWire(i, j, 3, 3);
				return;
			}
			if (Main.tile[i, j].type == 136)
			{
				if (Main.tile[i, j].frameY == 0)
				{
					Main.tile[i, j].frameY = 18;
				}
				else
				{
					Main.tile[i, j].frameY = 0;
				}
				Main.PlaySound(28, i * 16, j * 16, 0, 1f, 0f);
				Wiring.TripWire(i, j, 1, 1);
				return;
			}
			if (Main.tile[i, j].type == 144)
			{
				if (Main.tile[i, j].frameY == 0)
				{
					Main.tile[i, j].frameY = 18;
					if (Main.netMode != 1)
					{
						Wiring.CheckMech(i, j, 18000);
					}
				}
				else
				{
					Main.tile[i, j].frameY = 0;
				}
				Main.PlaySound(28, i * 16, j * 16, 0, 1f, 0f);
				return;
			}
			if (Main.tile[i, j].type == 441)
			{
				int num = (int)(Main.tile[i, j].frameX / 18 * -1);
				int num2 = (int)(Main.tile[i, j].frameY / 18 * -1);
				num %= 4;
				if (num < -1)
				{
					num += 2;
				}
				num += i;
				num2 += j;
				Main.PlaySound(28, i * 16, j * 16, 0, 1f, 0f);
				Wiring.TripWire(num, num2, 2, 2);
				return;
			}
			if (Main.tile[i, j].type == 132 || Main.tile[i, j].type == 411)
			{
				short num3 = 36;
				int num4 = (int)(Main.tile[i, j].frameX / 18 * -1);
				int num5 = (int)(Main.tile[i, j].frameY / 18 * -1);
				num4 %= 4;
				if (num4 < -1)
				{
					num4 += 2;
					num3 = -36;
				}
				num4 += i;
				num5 += j;
				if (Main.netMode != 1 && Main.tile[num4, num5].type == 411)
				{
					Wiring.CheckMech(num4, num5, 60);
				}
				for (int k = num4; k < num4 + 2; k++)
				{
					for (int l = num5; l < num5 + 2; l++)
					{
						if (Main.tile[k, l].type == 132 || Main.tile[k, l].type == 411)
						{
							Tile expr_36A = Main.tile[k, l];
							expr_36A.frameX += num3;
						}
					}
				}
				WorldGen.TileFrame(num4, num5, false, false);
				Main.PlaySound(28, i * 16, j * 16, 0, 1f, 0f);
				Wiring.TripWire(num4, num5, 2, 2);
			}
		}

		public static void PokeLogicGate(int lampX, int lampY)
		{
			if (Main.netMode == 1)
			{
				return;
			}
			Wiring._LampsToCheck.Enqueue(new Point16(lampX, lampY));
			Wiring.LogicGatePass();
		}

		public static bool Actuate(int i, int j)
		{
			Tile tile = Main.tile[i, j];
			if (!tile.actuator())
			{
				return false;
			}
			if ((tile.type != 226 || (double)j <= Main.worldSurface || NPC.downedPlantBoss) && ((double)j <= Main.worldSurface || NPC.downedGolemBoss || Main.tile[i, j - 1].type != 237))
			{
				if (tile.inActive())
				{
					Wiring.ReActive(i, j);
				}
				else
				{
					Wiring.DeActive(i, j);
				}
			}
			return true;
		}

		public static void ActuateForced(int i, int j)
		{
			Tile tile = Main.tile[i, j];
			if (tile.type == 226 && (double)j > Main.worldSurface && !NPC.downedPlantBoss)
			{
				return;
			}
			if (tile.inActive())
			{
				Wiring.ReActive(i, j);
				return;
			}
			Wiring.DeActive(i, j);
		}

		public static void MassWireOperation(Point ps, Point pe, Player master)
		{
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < 58; i++)
			{
				if (master.inventory[i].type == 530)
				{
					num += master.inventory[i].stack;
				}
				if (master.inventory[i].type == 849)
				{
					num2 += master.inventory[i].stack;
				}
			}
			int num3 = num;
			int num4 = num2;
			Wiring.MassWireOperationInner(ps, pe, master.Center, master.direction == 1, ref num, ref num2);
			int num5 = num3 - num;
			int num6 = num4 - num2;
			if (Main.netMode == 2)
			{
				NetMessage.SendData(110, master.whoAmI, -1, "", 530, (float)num5, (float)master.whoAmI, 0f, 0, 0, 0);
				NetMessage.SendData(110, master.whoAmI, -1, "", 849, (float)num6, (float)master.whoAmI, 0f, 0, 0, 0);
				return;
			}
			for (int j = 0; j < num5; j++)
			{
				master.ConsumeItem(530, false);
			}
			for (int k = 0; k < num6; k++)
			{
				master.ConsumeItem(849, false);
			}
		}

		public static bool CheckMech(int i, int j, int time)
		{
			for (int k = 0; k < Wiring._numMechs; k++)
			{
				if (Wiring._mechX[k] == i && Wiring._mechY[k] == j)
				{
					return false;
				}
			}
			if (Wiring._numMechs < 999)
			{
				Wiring._mechX[Wiring._numMechs] = i;
				Wiring._mechY[Wiring._numMechs] = j;
				Wiring._mechTime[Wiring._numMechs] = time;
				Wiring._numMechs++;
				return true;
			}
			return false;
		}

		private static void XferWater()
		{
			for (int i = 0; i < Wiring._numInPump; i++)
			{
				int num = Wiring._inPumpX[i];
				int num2 = Wiring._inPumpY[i];
				int liquid = (int)Main.tile[num, num2].liquid;
				if (liquid > 0)
				{
					bool flag = Main.tile[num, num2].lava();
					bool flag2 = Main.tile[num, num2].honey();
					for (int j = 0; j < Wiring._numOutPump; j++)
					{
						int num3 = Wiring._outPumpX[j];
						int num4 = Wiring._outPumpY[j];
						int liquid2 = (int)Main.tile[num3, num4].liquid;
						if (liquid2 < 255)
						{
							bool flag3 = Main.tile[num3, num4].lava();
							bool flag4 = Main.tile[num3, num4].honey();
							if (liquid2 == 0)
							{
								flag3 = flag;
								flag4 = flag2;
							}
							if (flag == flag3 && flag2 == flag4)
							{
								int num5 = liquid;
								if (num5 + liquid2 > 255)
								{
									num5 = 255 - liquid2;
								}
								Tile expr_102 = Main.tile[num3, num4];
								expr_102.liquid += (byte)num5;
								Tile expr_11E = Main.tile[num, num2];
								expr_11E.liquid -= (byte)num5;
								liquid = (int)Main.tile[num, num2].liquid;
								Main.tile[num3, num4].lava(flag);
								Main.tile[num3, num4].honey(flag2);
								WorldGen.SquareTileFrame(num3, num4, true);
								if (Main.tile[num, num2].liquid == 0)
								{
									Main.tile[num, num2].lava(false);
									WorldGen.SquareTileFrame(num, num2, true);
									break;
								}
							}
						}
					}
					WorldGen.SquareTileFrame(num, num2, true);
				}
			}
		}

		public static void TripWire(int left, int top, int width, int height)
		{
			if (Main.netMode == 1)
			{
				return;
			}
			Wiring.running = true;
			if (Wiring._wireList.Count != 0)
			{
				Wiring._wireList.Clear(true);
			}
			if (Wiring._wireDirectionList.Count != 0)
			{
				Wiring._wireDirectionList.Clear(true);
			}
			Vector2[] array = new Vector2[8];
			int num = 0;
			for (int i = left; i < left + width; i++)
			{
				for (int j = top; j < top + height; j++)
				{
					Point16 back = new Point16(i, j);
					Tile tile = Main.tile[i, j];
					if (tile != null && tile.wire())
					{
						Wiring._wireList.PushBack(back);
					}
				}
			}
			Wiring._teleport[0].X = -1f;
			Wiring._teleport[0].Y = -1f;
			Wiring._teleport[1].X = -1f;
			Wiring._teleport[1].Y = -1f;
			if (Wiring._wireList.Count > 0)
			{
				Wiring._numInPump = 0;
				Wiring._numOutPump = 0;
				Wiring.HitWire(Wiring._wireList, 1);
				if (Wiring._numInPump > 0 && Wiring._numOutPump > 0)
				{
					Wiring.XferWater();
				}
			}
			array[num++] = Wiring._teleport[0];
			array[num++] = Wiring._teleport[1];
			for (int k = left; k < left + width; k++)
			{
				for (int l = top; l < top + height; l++)
				{
					Point16 back = new Point16(k, l);
					Tile tile2 = Main.tile[k, l];
					if (tile2 != null && tile2.wire2())
					{
						Wiring._wireList.PushBack(back);
					}
				}
			}
			Wiring._teleport[0].X = -1f;
			Wiring._teleport[0].Y = -1f;
			Wiring._teleport[1].X = -1f;
			Wiring._teleport[1].Y = -1f;
			if (Wiring._wireList.Count > 0)
			{
				Wiring._numInPump = 0;
				Wiring._numOutPump = 0;
				Wiring.HitWire(Wiring._wireList, 2);
				if (Wiring._numInPump > 0 && Wiring._numOutPump > 0)
				{
					Wiring.XferWater();
				}
			}
			array[num++] = Wiring._teleport[0];
			array[num++] = Wiring._teleport[1];
			Wiring._teleport[0].X = -1f;
			Wiring._teleport[0].Y = -1f;
			Wiring._teleport[1].X = -1f;
			Wiring._teleport[1].Y = -1f;
			for (int m = left; m < left + width; m++)
			{
				for (int n = top; n < top + height; n++)
				{
					Point16 back = new Point16(m, n);
					Tile tile3 = Main.tile[m, n];
					if (tile3 != null && tile3.wire3())
					{
						Wiring._wireList.PushBack(back);
					}
				}
			}
			if (Wiring._wireList.Count > 0)
			{
				Wiring._numInPump = 0;
				Wiring._numOutPump = 0;
				Wiring.HitWire(Wiring._wireList, 3);
				if (Wiring._numInPump > 0 && Wiring._numOutPump > 0)
				{
					Wiring.XferWater();
				}
			}
			array[num++] = Wiring._teleport[0];
			array[num++] = Wiring._teleport[1];
			Wiring._teleport[0].X = -1f;
			Wiring._teleport[0].Y = -1f;
			Wiring._teleport[1].X = -1f;
			Wiring._teleport[1].Y = -1f;
			for (int num2 = left; num2 < left + width; num2++)
			{
				for (int num3 = top; num3 < top + height; num3++)
				{
					Point16 back = new Point16(num2, num3);
					Tile tile4 = Main.tile[num2, num3];
					if (tile4 != null && tile4.wire4())
					{
						Wiring._wireList.PushBack(back);
					}
				}
			}
			if (Wiring._wireList.Count > 0)
			{
				Wiring._numInPump = 0;
				Wiring._numOutPump = 0;
				Wiring.HitWire(Wiring._wireList, 4);
				if (Wiring._numInPump > 0 && Wiring._numOutPump > 0)
				{
					Wiring.XferWater();
				}
			}
			array[num++] = Wiring._teleport[0];
			array[num++] = Wiring._teleport[1];
			for (int num4 = 0; num4 < 8; num4 += 2)
			{
				Wiring._teleport[0] = array[num4];
				Wiring._teleport[1] = array[num4 + 1];
				if (Wiring._teleport[0].X >= 0f && Wiring._teleport[1].X >= 0f)
				{
					Wiring.Teleport();
				}
			}
			Wiring.PixelBoxPass();
			Wiring.LogicGatePass();
		}

		private static void PixelBoxPass()
		{
			foreach (KeyValuePair<Point16, byte> current in Wiring._PixelBoxTriggers)
			{
				if (current.Value != 2)
				{
					if (current.Value == 1)
					{
						if (Main.tile[(int)current.Key.X, (int)current.Key.Y].frameX != 0)
						{
							Main.tile[(int)current.Key.X, (int)current.Key.Y].frameX = 0;
							NetMessage.SendTileSquare(-1, (int)current.Key.X, (int)current.Key.Y, 1, TileChangeType.None);
						}
					}
					else if (current.Value == 3 && Main.tile[(int)current.Key.X, (int)current.Key.Y].frameX != 18)
					{
						Main.tile[(int)current.Key.X, (int)current.Key.Y].frameX = 18;
						NetMessage.SendTileSquare(-1, (int)current.Key.X, (int)current.Key.Y, 1, TileChangeType.None);
					}
				}
			}
			Wiring._PixelBoxTriggers.Clear();
		}

		private static void LogicGatePass()
		{
			if (Wiring._GatesCurrent.Count == 0)
			{
				Wiring._GatesDone.Clear();
				while (Wiring._LampsToCheck.Count > 0)
				{
					while (Wiring._LampsToCheck.Count > 0)
					{
						Point16 point = Wiring._LampsToCheck.Dequeue();
						Wiring.CheckLogicGate((int)point.X, (int)point.Y);
					}
					while (Wiring._GatesNext.Count > 0)
					{
						Utils.Swap<Queue<Point16>>(ref Wiring._GatesCurrent, ref Wiring._GatesNext);
						while (Wiring._GatesCurrent.Count > 0)
						{
							Point16 key = Wiring._GatesCurrent.Peek();
							bool flag;
							if (Wiring._GatesDone.TryGetValue(key, out flag) && flag)
							{
								Wiring._GatesCurrent.Dequeue();
							}
							else
							{
								Wiring._GatesDone.Add(key, true);
								Wiring.TripWire((int)key.X, (int)key.Y, 1, 1);
								Wiring._GatesCurrent.Dequeue();
							}
						}
					}
				}
				Wiring._GatesDone.Clear();
				if (Wiring.blockPlayerTeleportationForOneIteration)
				{
					Wiring.blockPlayerTeleportationForOneIteration = false;
				}
			}
		}

		private static void CheckLogicGate(int lampX, int lampY)
		{
			if (!WorldGen.InWorld(lampX, lampY, 1))
			{
				return;
			}
			int i = lampY;
			while (i < Main.maxTilesY)
			{
				Tile tile = Main.tile[lampX, i];
				if (!tile.active())
				{
					return;
				}
				if (tile.type == 420)
				{
					bool flag;
					Wiring._GatesDone.TryGetValue(new Point16(lampX, i), out flag);
					int num = (int)(tile.frameY / 18);
					bool flag2 = tile.frameX == 18;
					bool flag3 = tile.frameX == 36;
					if (num < 0)
					{
						return;
					}
					int num2 = 0;
					int num3 = 0;
					bool flag4 = false;
					for (int j = i - 1; j > 0; j--)
					{
						Tile tile2 = Main.tile[lampX, j];
						if (!tile2.active() || tile2.type != 419)
						{
							break;
						}
						if (tile2.frameX == 36)
						{
							flag4 = true;
							break;
						}
						num2++;
						num3 += (tile2.frameX == 18).ToInt();
					}
					bool flag5;
					switch (num)
					{
						case 0:
							flag5 = (num2 == num3);
							break;
						case 1:
							flag5 = (num3 > 0);
							break;
						case 2:
							flag5 = (num2 != num3);
							break;
						case 3:
							flag5 = (num3 == 0);
							break;
						case 4:
							flag5 = (num3 == 1);
							break;
						case 5:
							flag5 = (num3 != 1);
							break;
						default:
							return;
					}
					bool flag6 = !flag4 && flag3;
					bool flag7 = false;
					if (flag4 && Framing.GetTileSafely(lampX, lampY).frameX == 36)
					{
						flag7 = true;
					}
					if (flag5 != flag2 || flag6 || flag7)
					{
						short arg_183_0 = (short)(tile.frameX % 18 / 18);
						tile.frameX = (short)(18 * flag5.ToInt());
						if (flag4)
						{
							tile.frameX = 36;
						}
						Wiring.SkipWire(lampX, i);
						WorldGen.SquareTileFrame(lampX, i, true);
						NetMessage.SendTileSquare(-1, lampX, i, 1, TileChangeType.None);
						bool flag8 = !flag4 || flag7;
						if (flag7)
						{
							if (num3 == 0 || num2 == 0)
							{
							}
							flag8 = (Main.rand.NextFloat() < (float)num3 / (float)num2);
						}
						if (flag6)
						{
							flag8 = false;
						}
						if (flag8)
						{
							if (!flag)
							{
								Wiring._GatesNext.Enqueue(new Point16(lampX, i));
								return;
							}
							Vector2 position = new Vector2((float)lampX, (float)i) * 16f - new Vector2(10f);
							Utils.PoofOfSmoke(position);
							NetMessage.SendData(106, -1, -1, "", (int)position.X, position.Y, 0f, 0f, 0, 0, 0);
						}
					}
					return;
				}
				else
				{
					if (tile.type != 419)
					{
						return;
					}
					i++;
				}
			}
		}

		private static void HitWire(DoubleStack<Point16> next, int wireType)
		{
			Wiring._wireDirectionList.Clear(true);
			for (int i = 0; i < next.Count; i++)
			{
				Point16 point = next.PopFront();
				Wiring.SkipWire(point);
				Wiring._toProcess.Add(point, 4);
				next.PushBack(point);
				Wiring._wireDirectionList.PushBack(0);
			}
			Wiring._currentWireColor = wireType;
			while (next.Count > 0)
			{
				Point16 point2 = next.PopFront();
				int num = (int)Wiring._wireDirectionList.PopFront();
				int x = (int)point2.X;
				int y = (int)point2.Y;
				if (!Wiring._wireSkip.ContainsKey(point2))
				{
					Wiring.HitWireSingle(x, y);
				}
				for (int j = 0; j < 4; j++)
				{
					int num2;
					int num3;
					switch (j)
					{
						case 0:
							num2 = x;
							num3 = y + 1;
							break;
						case 1:
							num2 = x;
							num3 = y - 1;
							break;
						case 2:
							num2 = x + 1;
							num3 = y;
							break;
						case 3:
							num2 = x - 1;
							num3 = y;
							break;
						default:
							num2 = x;
							num3 = y + 1;
							break;
					}
					if (num2 >= 2 && num2 < Main.maxTilesX - 2 && num3 >= 2 && num3 < Main.maxTilesY - 2)
					{
						Tile tile = Main.tile[num2, num3];
						if (tile != null)
						{
							Tile tile2 = Main.tile[x, y];
							if (tile2 != null)
							{
								byte b = 3;
								if (tile.type == 424 || tile.type == 445)
								{
									b = 0;
								}
								if (tile2.type == 424)
								{
									switch (tile2.frameX / 18)
									{
										case 0:
											if (j != num)
											{
												goto IL_320;
											}
											break;
										case 1:
											if ((num != 0 || j != 3) && (num != 3 || j != 0) && (num != 1 || j != 2))
											{
												if (num != 2)
												{
													goto IL_320;
												}
												if (j != 1)
												{
													goto IL_320;
												}
											}
											break;
										case 2:
											if ((num != 0 || j != 2) && (num != 2 || j != 0) && (num != 1 || j != 3) && (num != 3 || j != 1))
											{
												goto IL_320;
											}
											break;
									}
								}
								if (tile2.type == 445)
								{
									if (j != num)
									{
										goto IL_320;
									}
									if (Wiring._PixelBoxTriggers.ContainsKey(point2))
									{
										Dictionary<Point16, byte> pixelBoxTriggers;
										Point16 key;
										(pixelBoxTriggers = Wiring._PixelBoxTriggers)[key = point2] = (byte)(pixelBoxTriggers[key] | ((j == 0 | j == 1) ? 2 : 1));
									}
									else
									{
										Wiring._PixelBoxTriggers[point2] = (byte)((j == 0 | j == 1) ? 2 : 1);
									}
								}
								bool flag;
								switch (wireType)
								{
									case 1:
										flag = tile.wire();
										break;
									case 2:
										flag = tile.wire2();
										break;
									case 3:
										flag = tile.wire3();
										break;
									case 4:
										flag = tile.wire4();
										break;
									default:
										flag = false;
										break;
								}
								if (flag)
								{
									Point16 point3 = new Point16(num2, num3);
									byte b2;
									if (Wiring._toProcess.TryGetValue(point3, out b2))
									{
										b2 -= 1;
										if (b2 == 0)
										{
											Wiring._toProcess.Remove(point3);
										}
										else
										{
											Wiring._toProcess[point3] = b2;
										}
									}
									else
									{
										next.PushBack(point3);
										Wiring._wireDirectionList.PushBack((byte)j);
										if (b > 0)
										{
											Wiring._toProcess.Add(point3, b);
										}
									}
								}
							}
						}
					}
					IL_320:
					;
				}
			}
			Wiring._wireSkip.Clear();
			Wiring._toProcess.Clear();
			Wiring.running = false;
		}

		private static void HitWireSingle(int i, int j)
		{
			Tile tile = Main.tile[i, j];
			int type = (int)tile.type;
			if (tile.actuator())
			{
				Wiring.ActuateForced(i, j);
			}
			if (tile.active())
			{
				if (!TileLoader.PreHitWire(i, j, type))
				{
					return;
				}
				if (type == 144)
				{
					Wiring.HitSwitch(i, j);
					WorldGen.SquareTileFrame(i, j, true);
					NetMessage.SendTileSquare(-1, i, j, 1, TileChangeType.None);
				}
				else if (type == 421)
				{
					if (!tile.actuator())
					{
						tile.type = 422;
						WorldGen.SquareTileFrame(i, j, true);
						NetMessage.SendTileSquare(-1, i, j, 1, TileChangeType.None);
					}
				}
				else if (type == 422 && !tile.actuator())
				{
					tile.type = 421;
					WorldGen.SquareTileFrame(i, j, true);
					NetMessage.SendTileSquare(-1, i, j, 1, TileChangeType.None);
				}
				if (type >= 255 && type <= 268)
				{
					if (!tile.actuator())
					{
						if (type >= 262)
						{
							Tile expr_D1 = tile;
							expr_D1.type -= 7;
						}
						else
						{
							Tile expr_E2 = tile;
							expr_E2.type += 7;
						}
						WorldGen.SquareTileFrame(i, j, true);
						NetMessage.SendTileSquare(-1, i, j, 1, TileChangeType.None);
						return;
					}
				}
				else
				{
					if (type == 419)
					{
						int num = 18;
						if ((int)tile.frameX >= num)
						{
							num = -num;
						}
						if (tile.frameX == 36)
						{
							num = 0;
						}
						Wiring.SkipWire(i, j);
						tile.frameX = (short)((int)tile.frameX + num);
						WorldGen.SquareTileFrame(i, j, true);
						NetMessage.SendTileSquare(-1, i, j, 1, TileChangeType.None);
						Wiring._LampsToCheck.Enqueue(new Point16(i, j));
						return;
					}
					if (type == 406)
					{
						int num2 = (int)(tile.frameX % 54 / 18);
						int num3 = (int)(tile.frameY % 54 / 18);
						int num4 = i - num2;
						int num5 = j - num3;
						int num6 = 54;
						if (Main.tile[num4, num5].frameY >= 108)
						{
							num6 = -108;
						}
						for (int k = num4; k < num4 + 3; k++)
						{
							for (int l = num5; l < num5 + 3; l++)
							{
								Wiring.SkipWire(k, l);
								Main.tile[k, l].frameY = (short)((int)Main.tile[k, l].frameY + num6);
							}
						}
						NetMessage.SendTileSquare(-1, num4 + 1, num5 + 1, 3, TileChangeType.None);
						return;
					}
					if (type == 452)
					{
						int num7 = (int)(tile.frameX % 54 / 18);
						int num8 = (int)(tile.frameY % 54 / 18);
						int num9 = i - num7;
						int num10 = j - num8;
						int num11 = 54;
						if (Main.tile[num9, num10].frameX >= 54)
						{
							num11 = -54;
						}
						for (int m = num9; m < num9 + 3; m++)
						{
							for (int n = num10; n < num10 + 3; n++)
							{
								Wiring.SkipWire(m, n);
								Main.tile[m, n].frameX = (short)((int)Main.tile[m, n].frameX + num11);
							}
						}
						NetMessage.SendTileSquare(-1, num9 + 1, num10 + 1, 3, TileChangeType.None);
						return;
					}
					if (type == 411)
					{
						int num12 = (int)(tile.frameX % 36 / 18);
						int num13 = (int)(tile.frameY % 36 / 18);
						int num14 = i - num12;
						int num15 = j - num13;
						int num16 = 36;
						if (Main.tile[num14, num15].frameX >= 36)
						{
							num16 = -36;
						}
						for (int num17 = num14; num17 < num14 + 2; num17++)
						{
							for (int num18 = num15; num18 < num15 + 2; num18++)
							{
								Wiring.SkipWire(num17, num18);
								Main.tile[num17, num18].frameX = (short)((int)Main.tile[num17, num18].frameX + num16);
							}
						}
						NetMessage.SendTileSquare(-1, num14, num15, 2, TileChangeType.None);
						return;
					}
					if (type == 425)
					{
						int num19 = (int)(tile.frameX % 36 / 18);
						int num20 = (int)(tile.frameY % 36 / 18);
						int num21 = i - num19;
						int num22 = j - num20;
						for (int num23 = num21; num23 < num21 + 2; num23++)
						{
							for (int num24 = num22; num24 < num22 + 2; num24++)
							{
								Wiring.SkipWire(num23, num24);
							}
						}
						if (!Main.AnnouncementBoxDisabled)
						{
							Color pink = Color.Pink;
							int num25 = Sign.ReadSign(num21, num22, false);
							if (num25 != -1 && Main.sign[num25] != null && !string.IsNullOrWhiteSpace(Main.sign[num25].text))
							{
                                if (TerrariaApi.Server.ServerApi.Hooks.InvokeWireTriggerAnnouncementBox(CurrentUser, i, j, num25, Main.sign[num25].text))
                                {
                                    return;
                                }
                                if (Main.AnnouncementBoxRange == -1)
								{
									if (Main.netMode == 0)
									{
										Main.NewTextMultiline(Main.sign[num25].text, false, pink, 460);
										return;
									}
									if (Main.netMode == 2)
									{
										NetMessage.SendData(107, -1, -1, Main.sign[num25].text, 255, (float)pink.R, (float)pink.G, (float)pink.B, 460, 0, 0);
										return;
									}
								}
								else if (Main.netMode == 0)
								{
									if (Main.player[Main.myPlayer].Distance(new Vector2((float)(num21 * 16 + 16), (float)(num22 * 16 + 16))) <= (float)Main.AnnouncementBoxRange)
									{
										Main.NewTextMultiline(Main.sign[num25].text, false, pink, 460);
										return;
									}
								}
								else if (Main.netMode == 2)
								{
									for (int num26 = 0; num26 < 255; num26++)
									{
										if (Main.player[num26].active && Main.player[num26].Distance(new Vector2((float)(num21 * 16 + 16), (float)(num22 * 16 + 16))) <= (float)Main.AnnouncementBoxRange)
										{
											NetMessage.SendData(107, num26, -1, Main.sign[num25].text, 255, (float)pink.R, (float)pink.G, (float)pink.B, 460, 0, 0);
										}
									}
									return;
								}
							}
						}
					}
					else
					{
						if (type == 405)
						{
							int num27 = (int)(tile.frameX % 54 / 18);
							int num28 = (int)(tile.frameY % 36 / 18);
							int num29 = i - num27;
							int num30 = j - num28;
							int num31 = 54;
							if (Main.tile[num29, num30].frameX >= 54)
							{
								num31 = -54;
							}
							for (int num32 = num29; num32 < num29 + 3; num32++)
							{
								for (int num33 = num30; num33 < num30 + 2; num33++)
								{
									Wiring.SkipWire(num32, num33);
									Main.tile[num32, num33].frameX = (short)((int)Main.tile[num32, num33].frameX + num31);
								}
							}
							NetMessage.SendTileSquare(-1, num29 + 1, num30 + 1, 3, TileChangeType.None);
							return;
						}
						if (type == 209)
						{
							int num34 = (int)(tile.frameX % 72 / 18);
							int num35 = (int)(tile.frameY % 54 / 18);
							int num36 = i - num34;
							int num37 = j - num35;
							int num38 = (int)(tile.frameY / 54);
							int num39 = (int)(tile.frameX / 72);
							int num40 = -1;
							if (num34 == 1 || num34 == 2)
							{
								num40 = num35;
							}
							int num41 = 0;
							if (num34 == 3)
							{
								num41 = -54;
							}
							if (num34 == 0)
							{
								num41 = 54;
							}
							if (num38 >= 8 && num41 > 0)
							{
								num41 = 0;
							}
							if (num38 == 0 && num41 < 0)
							{
								num41 = 0;
							}
							bool flag = false;
							if (num41 != 0)
							{
								for (int num42 = num36; num42 < num36 + 4; num42++)
								{
									for (int num43 = num37; num43 < num37 + 3; num43++)
									{
										Wiring.SkipWire(num42, num43);
										Main.tile[num42, num43].frameY = (short)((int)Main.tile[num42, num43].frameY + num41);
									}
								}
								flag = true;
							}
							if ((num39 == 3 || num39 == 4) && (num40 == 0 || num40 == 1))
							{
								num41 = ((num39 == 3) ? 72 : -72);
								for (int num44 = num36; num44 < num36 + 4; num44++)
								{
									for (int num45 = num37; num45 < num37 + 3; num45++)
									{
										Wiring.SkipWire(num44, num45);
										Main.tile[num44, num45].frameX = (short)((int)Main.tile[num44, num45].frameX + num41);
									}
								}
								flag = true;
							}
							if (flag)
							{
								NetMessage.SendTileSquare(-1, num36 + 1, num37 + 1, 4, TileChangeType.None);
							}
							if (num40 != -1)
							{
								bool flag2 = true;
								if ((num39 == 3 || num39 == 4) && num40 < 2)
								{
									flag2 = false;
								}
								if (Wiring.CheckMech(num36, num37, 30) && flag2)
								{
									WorldGen.ShootFromCannon(num36, num37, num38, num39 + 1, 0, 0f, Wiring.CurrentUser);
									return;
								}
							}
						}
						else if (type == 212)
						{
							int num46 = (int)(tile.frameX % 54 / 18);
							int num47 = (int)(tile.frameY % 54 / 18);
							int num48 = i - num46;
							int num49 = j - num47;
							int num50 = (int)(tile.frameX / 54);
							int num51 = -1;
							if (num46 == 1)
							{
								num51 = num47;
							}
							int num52 = 0;
							if (num46 == 0)
							{
								num52 = -54;
							}
							if (num46 == 2)
							{
								num52 = 54;
							}
							if (num50 >= 1 && num52 > 0)
							{
								num52 = 0;
							}
							if (num50 == 0 && num52 < 0)
							{
								num52 = 0;
							}
							bool flag3 = false;
							if (num52 != 0)
							{
								for (int num53 = num48; num53 < num48 + 3; num53++)
								{
									for (int num54 = num49; num54 < num49 + 3; num54++)
									{
										Wiring.SkipWire(num53, num54);
										Main.tile[num53, num54].frameX = (short)((int)Main.tile[num53, num54].frameX + num52);
									}
								}
								flag3 = true;
							}
							if (flag3)
							{
								NetMessage.SendTileSquare(-1, num48 + 1, num49 + 1, 4, TileChangeType.None);
							}
							if (num51 != -1 && Wiring.CheckMech(num48, num49, 10))
							{
								float num55 = 12f + (float)Main.rand.Next(450) * 0.01f;
								float num56 = (float)Main.rand.Next(85, 105);
								float num57 = (float)Main.rand.Next(-35, 11);
								int type2 = 166;
								int damage = 0;
								float knockBack = 0f;
								Vector2 vector = new Vector2((float)((num48 + 2) * 16 - 8), (float)((num49 + 2) * 16 - 8));
								if (tile.frameX / 54 == 0)
								{
									num56 *= -1f;
									vector.X -= 12f;
								}
								else
								{
									vector.X += 12f;
								}
								float num58 = num56;
								float num59 = num57;
								float num60 = (float)Math.Sqrt((double)(num58 * num58 + num59 * num59));
								num60 = num55 / num60;
								num58 *= num60;
								num59 *= num60;
								Projectile.NewProjectile(vector.X, vector.Y, num58, num59, type2, damage, knockBack, Wiring.CurrentUser, 0f, 0f);
								return;
							}
						}
						else
						{
							if (type == 215)
							{
								int num61 = (int)(tile.frameX % 54 / 18);
								int num62 = (int)(tile.frameY % 36 / 18);
								int num63 = i - num61;
								int num64 = j - num62;
								int num65 = 36;
								if (Main.tile[num63, num64].frameY >= 36)
								{
									num65 = -36;
								}
								for (int num66 = num63; num66 < num63 + 3; num66++)
								{
									for (int num67 = num64; num67 < num64 + 2; num67++)
									{
										Wiring.SkipWire(num66, num67);
										Main.tile[num66, num67].frameY = (short)((int)Main.tile[num66, num67].frameY + num65);
									}
								}
								NetMessage.SendTileSquare(-1, num63 + 1, num64 + 1, 3, TileChangeType.None);
								return;
							}
							if (type == 130)
							{
								if (Main.tile[i, j - 1] != null && Main.tile[i, j - 1].active())
								{
									if (Main.tile[i, j - 1].type == 21 || Main.tile[i, j - 1].type == 441)
									{
										return;
									}
									if (Main.tile[i, j - 1].type == 88)
									{
										return;
									}
								}
								tile.type = 131;
								WorldGen.SquareTileFrame(i, j, true);
								NetMessage.SendTileSquare(-1, i, j, 1, TileChangeType.None);
								return;
							}
							if (type == 131)
							{
								tile.type = 130;
								WorldGen.SquareTileFrame(i, j, true);
								NetMessage.SendTileSquare(-1, i, j, 1, TileChangeType.None);
								return;
							}
							if (type == 387 || type == 386)
							{
								bool value = type == 387;
								int num68 = WorldGen.ShiftTrapdoor(i, j, true, -1).ToInt();
								if (num68 == 0)
								{
									num68 = -WorldGen.ShiftTrapdoor(i, j, false, -1).ToInt();
								}
								if (num68 != 0)
								{
									NetMessage.SendData(19, -1, -1, "", 3 - value.ToInt(), (float)i, (float)j, (float)num68, 0, 0, 0);
									return;
								}
							}
							else
							{
								if (type == 389 || type == 388)
								{
									bool flag4 = type == 389;
									WorldGen.ShiftTallGate(i, j, flag4);
									NetMessage.SendData(19, -1, -1, "", 4 + flag4.ToInt(), (float)i, (float)j, 0f, 0, 0, 0);
									return;
								}
								if (TileLoader.CloseDoorID(Main.tile[i, j]) >= 0)
								{
									if (WorldGen.CloseDoor(i, j, true))
									{
										NetMessage.SendData(19, -1, -1, "", 1, (float)i, (float)j, 0f, 0, 0, 0);
										return;
									}
								}
								else if (TileLoader.OpenDoorID(Main.tile[i, j]) >= 0)
								{
									int num69 = 1;
									if (Main.rand.Next(2) == 0)
									{
										num69 = -1;
									}
									if (WorldGen.OpenDoor(i, j, num69))
									{
										NetMessage.SendData(19, -1, -1, "", 0, (float)i, (float)j, (float)num69, 0, 0, 0);
										return;
									}
									if (WorldGen.OpenDoor(i, j, -num69))
									{
										NetMessage.SendData(19, -1, -1, "", 0, (float)i, (float)j, (float)(-(float)num69), 0, 0, 0);
										return;
									}
								}
								else
								{
									if (type == 216)
									{
										WorldGen.LaunchRocket(i, j);
										Wiring.SkipWire(i, j);
										return;
									}
									if (type == 335)
									{
										int num70 = j - (int)(tile.frameY / 18);
										int num71 = i - (int)(tile.frameX / 18);
										Wiring.SkipWire(num71, num70);
										Wiring.SkipWire(num71, num70 + 1);
										Wiring.SkipWire(num71 + 1, num70);
										Wiring.SkipWire(num71 + 1, num70 + 1);
										if (Wiring.CheckMech(num71, num70, 30))
										{
											WorldGen.LaunchRocketSmall(num71, num70);
											return;
										}
									}
									else if (type == 338)
									{
										int num72 = j - (int)(tile.frameY / 18);
										int num73 = i - (int)(tile.frameX / 18);
										Wiring.SkipWire(num73, num72);
										Wiring.SkipWire(num73, num72 + 1);
										if (Wiring.CheckMech(num73, num72, 30))
										{
											bool flag5 = false;
											for (int num74 = 0; num74 < 1000; num74++)
											{
												if (Main.projectile[num74].active && Main.projectile[num74].aiStyle == 73 && Main.projectile[num74].ai[0] == (float)num73 && Main.projectile[num74].ai[1] == (float)num72)
												{
													flag5 = true;
													break;
												}
											}
											if (!flag5)
											{
												Projectile.NewProjectile((float)(num73 * 16 + 8), (float)(num72 * 16 + 2), 0f, 0f, 419 + Main.rand.Next(4), 0, 0f, Main.myPlayer, (float)num73, (float)num72);
												return;
											}
										}
									}
									else if (type == 235)
									{
										int num75 = i - (int)(tile.frameX / 18);
										if (tile.wall == 87 && (double)j > Main.worldSurface && !NPC.downedPlantBoss)
										{
											return;
										}
										if (Wiring._teleport[0].X == -1f)
										{
											Wiring._teleport[0].X = (float)num75;
											Wiring._teleport[0].Y = (float)j;
											if (tile.halfBrick())
											{
												Vector2[] expr_EFE_cp_0 = Wiring._teleport;
												int expr_EFE_cp_1 = 0;
												expr_EFE_cp_0[expr_EFE_cp_1].Y = expr_EFE_cp_0[expr_EFE_cp_1].Y + 0.5f;
												return;
											}
										}
										else if (Wiring._teleport[0].X != (float)num75 || Wiring._teleport[0].Y != (float)j)
										{
											Wiring._teleport[1].X = (float)num75;
											Wiring._teleport[1].Y = (float)j;
											if (tile.halfBrick())
											{
												Vector2[] expr_F77_cp_0 = Wiring._teleport;
												int expr_F77_cp_1 = 1;
												expr_F77_cp_0[expr_F77_cp_1].Y = expr_F77_cp_0[expr_F77_cp_1].Y + 0.5f;
												return;
											}
										}
									}
									else
									{
										if (TileLoader.IsTorch(type))
										{
											if (tile.frameX < 66)
											{
												Tile expr_F98 = tile;
												expr_F98.frameX += 66;
											}
											else
											{
												Tile expr_FAA = tile;
												expr_FAA.frameX -= 66;
											}
											NetMessage.SendTileSquare(-1, i, j, 1, TileChangeType.None);
											return;
										}
										if (type == 429)
										{
											int num76 = (int)(Main.tile[i, j].frameX / 18);
											bool flag6 = num76 % 2 >= 1;
											bool flag7 = num76 % 4 >= 2;
											bool flag8 = num76 % 8 >= 4;
											bool flag9 = num76 % 16 >= 8;
											bool flag10 = false;
											short num77 = 0;
											switch (Wiring._currentWireColor)
											{
												case 1:
													num77 = 18;
													flag10 = !flag6;
													break;
												case 2:
													num77 = 72;
													flag10 = !flag8;
													break;
												case 3:
													num77 = 36;
													flag10 = !flag7;
													break;
												case 4:
													num77 = 144;
													flag10 = !flag9;
													break;
											}
											if (flag10)
											{
												Tile expr_1078 = tile;
												expr_1078.frameX += num77;
											}
											else
											{
												Tile expr_108A = tile;
												expr_108A.frameX -= num77;
											}
											NetMessage.SendTileSquare(-1, i, j, 1, TileChangeType.None);
											return;
										}
										if (type == 149)
										{
											if (tile.frameX < 54)
											{
												Tile expr_10B7 = tile;
												expr_10B7.frameX += 54;
											}
											else
											{
												Tile expr_10C9 = tile;
												expr_10C9.frameX -= 54;
											}
											NetMessage.SendTileSquare(-1, i, j, 1, TileChangeType.None);
											return;
										}
										if (type == 244)
										{
											int num78;
											for (num78 = (int)(tile.frameX / 18); num78 >= 3; num78 -= 3)
											{
											}
											int num79;
											for (num79 = (int)(tile.frameY / 18); num79 >= 3; num79 -= 3)
											{
											}
											int num80 = i - num78;
											int num81 = j - num79;
											int num82 = 54;
											if (Main.tile[num80, num81].frameX >= 54)
											{
												num82 = -54;
											}
											for (int num83 = num80; num83 < num80 + 3; num83++)
											{
												for (int num84 = num81; num84 < num81 + 2; num84++)
												{
													Wiring.SkipWire(num83, num84);
													Main.tile[num83, num84].frameX = (short)((int)Main.tile[num83, num84].frameX + num82);
												}
											}
											NetMessage.SendTileSquare(-1, num80 + 1, num81 + 1, 3, TileChangeType.None);
											return;
										}
										if (type == 42)
										{
											int num85;
											for (num85 = (int)(tile.frameY / 18); num85 >= 2; num85 -= 2)
											{
											}
											int num86 = j - num85;
											short num87 = 18;
											if (tile.frameX > 0)
											{
												num87 = -18;
											}
											Tile expr_11F9 = Main.tile[i, num86];
											expr_11F9.frameX += num87;
											Tile expr_1217 = Main.tile[i, num86 + 1];
											expr_1217.frameX += num87;
											Wiring.SkipWire(i, num86);
											Wiring.SkipWire(i, num86 + 1);
											NetMessage.SendTileSquare(-1, i, j, 2, TileChangeType.None);
											return;
										}
										if (type == 93)
										{
											int num88;
											for (num88 = (int)(tile.frameY / 18); num88 >= 3; num88 -= 3)
											{
											}
											num88 = j - num88;
											short num89 = 18;
											if (tile.frameX > 0)
											{
												num89 = -18;
											}
											Tile expr_1287 = Main.tile[i, num88];
											expr_1287.frameX += num89;
											Tile expr_12A5 = Main.tile[i, num88 + 1];
											expr_12A5.frameX += num89;
											Tile expr_12C3 = Main.tile[i, num88 + 2];
											expr_12C3.frameX += num89;
											Wiring.SkipWire(i, num88);
											Wiring.SkipWire(i, num88 + 1);
											Wiring.SkipWire(i, num88 + 2);
											NetMessage.SendTileSquare(-1, i, num88 + 1, 3, TileChangeType.None);
											return;
										}
										if (type == 126 || type == 95 || type == 100 || type == 173)
										{
											int num90;
											for (num90 = (int)(tile.frameY / 18); num90 >= 2; num90 -= 2)
											{
											}
											num90 = j - num90;
											int num91 = (int)(tile.frameX / 18);
											if (num91 > 1)
											{
												num91 -= 2;
											}
											num91 = i - num91;
											short num92 = 36;
											if (Main.tile[num91, num90].frameX > 0)
											{
												num92 = -36;
											}
											Tile expr_137C = Main.tile[num91, num90];
											expr_137C.frameX += num92;
											Tile expr_139B = Main.tile[num91, num90 + 1];
											expr_139B.frameX += num92;
											Tile expr_13BA = Main.tile[num91 + 1, num90];
											expr_13BA.frameX += num92;
											Tile expr_13DB = Main.tile[num91 + 1, num90 + 1];
											expr_13DB.frameX += num92;
											Wiring.SkipWire(num91, num90);
											Wiring.SkipWire(num91 + 1, num90);
											Wiring.SkipWire(num91, num90 + 1);
											Wiring.SkipWire(num91 + 1, num90 + 1);
											NetMessage.SendTileSquare(-1, num91, num90, 3, TileChangeType.None);
											return;
										}
										if (type == 34)
										{
											int num93;
											for (num93 = (int)(tile.frameY / 18); num93 >= 3; num93 -= 3)
											{
											}
											int num94 = j - num93;
											int num95 = (int)(tile.frameX / 18);
											if (num95 > 2)
											{
												num95 -= 3;
											}
											num95 = i - num95;
											short num96 = 54;
											if (Main.tile[num95, num94].frameX > 0)
											{
												num96 = -54;
											}
											for (int num97 = num95; num97 < num95 + 3; num97++)
											{
												for (int num98 = num94; num98 < num94 + 3; num98++)
												{
													Tile expr_149D = Main.tile[num97, num98];
													expr_149D.frameX += num96;
													Wiring.SkipWire(num97, num98);
												}
											}
											NetMessage.SendTileSquare(-1, num95 + 1, num94 + 1, 3, TileChangeType.None);
											return;
										}
										if (type == 314)
										{
											if (Wiring.CheckMech(i, j, 5))
											{
												Minecart.FlipSwitchTrack(i, j);
												return;
											}
										}
										else
										{
											if (type == 33 || type == 174)
											{
												short num99 = 18;
												if (tile.frameX > 0)
												{
													num99 = -18;
												}
												Tile expr_151E = tile;
												expr_151E.frameX += num99;
												NetMessage.SendTileSquare(-1, i, j, 3, TileChangeType.None);
												return;
											}
											if (type == 92)
											{
												int num100 = j - (int)(tile.frameY / 18);
												short num101 = 18;
												if (tile.frameX > 0)
												{
													num101 = -18;
												}
												for (int num102 = num100; num102 < num100 + 6; num102++)
												{
													Tile expr_156E = Main.tile[i, num102];
													expr_156E.frameX += num101;
													Wiring.SkipWire(i, num102);
												}
												NetMessage.SendTileSquare(-1, i, num100 + 3, 7, TileChangeType.None);
												return;
											}
											if (type == 137)
											{
												int num103 = (int)(tile.frameY / 18);
												Vector2 zero = Vector2.Zero;
												float speedX = 0f;
												float speedY = 0f;
												int num104 = 0;
												int damage2 = 0;
												switch (num103)
												{
													case 0:
													case 1:
													case 2:
														if (Wiring.CheckMech(i, j, 200))
														{
															int num105 = (tile.frameX == 0) ? -1 : ((tile.frameX == 18) ? 1 : 0);
															int num106 = (tile.frameX < 36) ? 0 : ((tile.frameX < 72) ? -1 : 1);
															zero = new Vector2((float)(i * 16 + 8 + 10 * num105), (float)(j * 16 + 9 + num106 * 9));
															float num107 = 3f;
															if (num103 == 0)
															{
																num104 = 98;
																damage2 = 20;
																num107 = 12f;
															}
															if (num103 == 1)
															{
																num104 = 184;
																damage2 = 40;
																num107 = 12f;
															}
															if (num103 == 2)
															{
																num104 = 187;
																damage2 = 40;
																num107 = 5f;
															}
															speedX = (float)num105 * num107;
															speedY = (float)num106 * num107;
														}
														break;
													case 3:
														if (Wiring.CheckMech(i, j, 300))
														{
															int num108 = 200;
															for (int num109 = 0; num109 < 1000; num109++)
															{
																if (Main.projectile[num109].active && Main.projectile[num109].type == num104)
																{
																	float num110 = (new Vector2((float)(i * 16 + 8), (float)(j * 18 + 8)) - Main.projectile[num109].Center).Length();
																	if (num110 < 50f)
																	{
																		num108 -= 50;
																	}
																	else if (num110 < 100f)
																	{
																		num108 -= 15;
																	}
																	else if (num110 < 200f)
																	{
																		num108 -= 10;
																	}
																	else if (num110 < 300f)
																	{
																		num108 -= 8;
																	}
																	else if (num110 < 400f)
																	{
																		num108 -= 6;
																	}
																	else if (num110 < 500f)
																	{
																		num108 -= 5;
																	}
																	else if (num110 < 700f)
																	{
																		num108 -= 4;
																	}
																	else if (num110 < 900f)
																	{
																		num108 -= 3;
																	}
																	else if (num110 < 1200f)
																	{
																		num108 -= 2;
																	}
																	else
																	{
																		num108--;
																	}
																}
															}
															if (num108 > 0)
															{
																num104 = 185;
																damage2 = 40;
																int num111 = 0;
																int num112 = 0;
																switch (tile.frameX / 18)
																{
																	case 0:
																	case 1:
																		num111 = 0;
																		num112 = 1;
																		break;
																	case 2:
																		num111 = 0;
																		num112 = -1;
																		break;
																	case 3:
																		num111 = -1;
																		num112 = 0;
																		break;
																	case 4:
																		num111 = 1;
																		num112 = 0;
																		break;
																}
																speedX = (float)(4 * num111) + (float)Main.rand.Next(-20 + ((num111 == 1) ? 20 : 0), 21 - ((num111 == -1) ? 20 : 0)) * 0.05f;
																speedY = (float)(4 * num112) + (float)Main.rand.Next(-20 + ((num112 == 1) ? 20 : 0), 21 - ((num112 == -1) ? 20 : 0)) * 0.05f;
																zero = new Vector2((float)(i * 16 + 8 + 14 * num111), (float)(j * 16 + 8 + 14 * num112));
															}
														}
														break;
													case 4:
														if (Wiring.CheckMech(i, j, 90))
														{
															int num113 = 0;
															int num114 = 0;
															switch (tile.frameX / 18)
															{
																case 0:
																case 1:
																	num113 = 0;
																	num114 = 1;
																	break;
																case 2:
																	num113 = 0;
																	num114 = -1;
																	break;
																case 3:
																	num113 = -1;
																	num114 = 0;
																	break;
																case 4:
																	num113 = 1;
																	num114 = 0;
																	break;
															}
															speedX = (float)(8 * num113);
															speedY = (float)(8 * num114);
															damage2 = 60;
															num104 = 186;
															zero = new Vector2((float)(i * 16 + 8 + 18 * num113), (float)(j * 16 + 8 + 18 * num114));
														}
														break;
												}
												switch (num103 + 10)
												{
													case 0:
														if (Wiring.CheckMech(i, j, 200))
														{
															int num115 = -1;
															if (tile.frameX != 0)
															{
																num115 = 1;
															}
															speedX = (float)(12 * num115);
															damage2 = 20;
															num104 = 98;
															zero = new Vector2((float)(i * 16 + 8), (float)(j * 16 + 7));
															zero.X += (float)(10 * num115);
															zero.Y += 2f;
														}
														break;
													case 1:
														if (Wiring.CheckMech(i, j, 200))
														{
															int num116 = -1;
															if (tile.frameX != 0)
															{
																num116 = 1;
															}
															speedX = (float)(12 * num116);
															damage2 = 40;
															num104 = 184;
															zero = new Vector2((float)(i * 16 + 8), (float)(j * 16 + 7));
															zero.X += (float)(10 * num116);
															zero.Y += 2f;
														}
														break;
													case 2:
														if (Wiring.CheckMech(i, j, 200))
														{
															int num117 = -1;
															if (tile.frameX != 0)
															{
																num117 = 1;
															}
															speedX = (float)(5 * num117);
															damage2 = 40;
															num104 = 187;
															zero = new Vector2((float)(i * 16 + 8), (float)(j * 16 + 7));
															zero.X += (float)(10 * num117);
															zero.Y += 2f;
														}
														break;
													case 3:
														if (Wiring.CheckMech(i, j, 300))
														{
															num104 = 185;
															int num118 = 200;
															for (int num119 = 0; num119 < 1000; num119++)
															{
																if (Main.projectile[num119].active && Main.projectile[num119].type == num104)
																{
																	float num120 = (new Vector2((float)(i * 16 + 8), (float)(j * 18 + 8)) - Main.projectile[num119].Center).Length();
																	if (num120 < 50f)
																	{
																		num118 -= 50;
																	}
																	else if (num120 < 100f)
																	{
																		num118 -= 15;
																	}
																	else if (num120 < 200f)
																	{
																		num118 -= 10;
																	}
																	else if (num120 < 300f)
																	{
																		num118 -= 8;
																	}
																	else if (num120 < 400f)
																	{
																		num118 -= 6;
																	}
																	else if (num120 < 500f)
																	{
																		num118 -= 5;
																	}
																	else if (num120 < 700f)
																	{
																		num118 -= 4;
																	}
																	else if (num120 < 900f)
																	{
																		num118 -= 3;
																	}
																	else if (num120 < 1200f)
																	{
																		num118 -= 2;
																	}
																	else
																	{
																		num118--;
																	}
																}
															}
															if (num118 > 0)
															{
																speedX = (float)Main.rand.Next(-20, 21) * 0.05f;
																speedY = 4f + (float)Main.rand.Next(0, 21) * 0.05f;
																damage2 = 40;
																zero = new Vector2((float)(i * 16 + 8), (float)(j * 16 + 16));
																zero.Y += 6f;
																Projectile.NewProjectile((float)((int)zero.X), (float)((int)zero.Y), speedX, speedY, num104, damage2, 2f, Main.myPlayer, 0f, 0f);
															}
														}
														break;
													case 4:
														if (Wiring.CheckMech(i, j, 90))
														{
															speedX = 0f;
															speedY = 8f;
															damage2 = 60;
															num104 = 186;
															zero = new Vector2((float)(i * 16 + 8), (float)(j * 16 + 16));
															zero.Y += 10f;
														}
														break;
												}
												if (num104 != 0)
												{
													Projectile.NewProjectile((float)((int)zero.X), (float)((int)zero.Y), speedX, speedY, num104, damage2, 2f, Main.myPlayer, 0f, 0f);
													return;
												}
											}
											else if (type == 443)
											{
												int num121 = (int)(tile.frameX / 36);
												int num122 = i - ((int)tile.frameX - num121 * 36) / 18;
												if (Wiring.CheckMech(num122, j, 200))
												{
													Vector2 vector2 = Vector2.Zero;
													Vector2 zero2 = Vector2.Zero;
													int num123 = 654;
													int damage3 = 20;
													if (num121 < 2)
													{
														vector2 = new Vector2((float)(num122 + 1), (float)j) * 16f;
														zero2 = new Vector2(0f, -8f);
													}
													else
													{
														vector2 = new Vector2((float)(num122 + 1), (float)(j + 1)) * 16f;
														zero2 = new Vector2(0f, 8f);
													}
													if (num123 != 0)
													{
														Projectile.NewProjectile((float)((int)vector2.X), (float)((int)vector2.Y), zero2.X, zero2.Y, num123, damage3, 2f, Main.myPlayer, 0f, 0f);
														return;
													}
												}
											}
											else
											{
												if (type == 139 || type == 35 || TileLoader.IsModMusicBox(tile))
												{
													WorldGen.SwitchMB(i, j);
													return;
												}
												if (type == 207)
												{
													WorldGen.SwitchFountain(i, j);
													return;
												}
												if (type == 410)
												{
													WorldGen.SwitchMonolith(i, j);
													return;
												}
												if (type == 455)
												{
													BirthdayParty.ToggleManualParty();
													return;
												}
												if (type == 141)
												{
													WorldGen.KillTile(i, j, false, false, true);
													NetMessage.SendTileSquare(-1, i, j, 1, TileChangeType.None);
													Projectile.NewProjectile((float)(i * 16 + 8), (float)(j * 16 + 8), 0f, 0f, 108, 500, 10f, Main.myPlayer, 0f, 0f);
													return;
												}
												if (type == 210)
												{
													WorldGen.ExplodeMine(i, j);
													return;
												}
												if (type == 142 || type == 143)
												{
													int num124 = j - (int)(tile.frameY / 18);
													int num125 = (int)(tile.frameX / 18);
													if (num125 > 1)
													{
														num125 -= 2;
													}
													num125 = i - num125;
													Wiring.SkipWire(num125, num124);
													Wiring.SkipWire(num125, num124 + 1);
													Wiring.SkipWire(num125 + 1, num124);
													Wiring.SkipWire(num125 + 1, num124 + 1);
													if (type == 142)
													{
														for (int num126 = 0; num126 < 4; num126++)
														{
															if (Wiring._numInPump >= 19)
															{
																return;
															}
															int num127;
															int num128;
															if (num126 == 0)
															{
																num127 = num125;
																num128 = num124 + 1;
															}
															else if (num126 == 1)
															{
																num127 = num125 + 1;
																num128 = num124 + 1;
															}
															else if (num126 == 2)
															{
																num127 = num125;
																num128 = num124;
															}
															else
															{
																num127 = num125 + 1;
																num128 = num124;
															}
															Wiring._inPumpX[Wiring._numInPump] = num127;
															Wiring._inPumpY[Wiring._numInPump] = num128;
															Wiring._numInPump++;
														}
														return;
													}
													for (int num129 = 0; num129 < 4; num129++)
													{
														if (Wiring._numOutPump >= 19)
														{
															return;
														}
														int num127;
														int num128;
														if (num129 == 0)
														{
															num127 = num125;
															num128 = num124 + 1;
														}
														else if (num129 == 1)
														{
															num127 = num125 + 1;
															num128 = num124 + 1;
														}
														else if (num129 == 2)
														{
															num127 = num125;
															num128 = num124;
														}
														else
														{
															num127 = num125 + 1;
															num128 = num124;
														}
														Wiring._outPumpX[Wiring._numOutPump] = num127;
														Wiring._outPumpY[Wiring._numOutPump] = num128;
														Wiring._numOutPump++;
													}
													return;
												}
												else if (type == 105)
												{
													int num130 = j - (int)(tile.frameY / 18);
													int num131 = (int)(tile.frameX / 18);
													int num132 = 0;
													while (num131 >= 2)
													{
														num131 -= 2;
														num132++;
													}
													num131 = i - num131;
													num131 = i - (int)(tile.frameX % 36 / 18);
													num130 = j - (int)(tile.frameY % 54 / 18);
													num132 = (int)(tile.frameX / 36 + tile.frameY / 54 * 55);
													Wiring.SkipWire(num131, num130);
													Wiring.SkipWire(num131, num130 + 1);
													Wiring.SkipWire(num131, num130 + 2);
													Wiring.SkipWire(num131 + 1, num130);
													Wiring.SkipWire(num131 + 1, num130 + 1);
													Wiring.SkipWire(num131 + 1, num130 + 2);
													int num133 = num131 * 16 + 16;
													int num134 = (num130 + 3) * 16;
													int num135 = -1;
													int num136 = -1;
													bool flag11 = true;
													bool flag12 = false;
													switch (num132)
													{
														case 51:
															num136 = (int)Utils.SelectRandom<short>(Main.rand, new short[]
																{
																	299,
																	538
																});
															break;
														case 52:
															num136 = 356;
															break;
														case 53:
															num136 = 357;
															break;
														case 54:
															num136 = (int)Utils.SelectRandom<short>(Main.rand, new short[]
																{
																	355,
																	358
																});
															break;
														case 55:
															num136 = (int)Utils.SelectRandom<short>(Main.rand, new short[]
																{
																	367,
																	366
																});
															break;
														case 56:
															num136 = (int)Utils.SelectRandom<short>(Main.rand, new short[]
																{
																	359,
																	359,
																	359,
																	359,
																	360
																});
															break;
														case 57:
															num136 = 377;
															break;
														case 58:
															num136 = 300;
															break;
														case 59:
															num136 = (int)Utils.SelectRandom<short>(Main.rand, new short[]
																{
																	364,
																	362
																});
															break;
														case 60:
															num136 = 148;
															break;
														case 61:
															num136 = 361;
															break;
														case 62:
															num136 = (int)Utils.SelectRandom<short>(Main.rand, new short[]
																{
																	487,
																	486,
																	485
																});
															break;
														case 63:
															num136 = 164;
															flag11 &= NPC.MechSpawn((float)num133, (float)num134, 165);
															break;
														case 64:
															num136 = 86;
															flag12 = true;
															break;
														case 65:
															num136 = 490;
															break;
														case 66:
															num136 = 82;
															break;
														case 67:
															num136 = 449;
															break;
														case 68:
															num136 = 167;
															break;
														case 69:
															num136 = 480;
															break;
														case 70:
															num136 = 48;
															break;
														case 71:
															num136 = (int)Utils.SelectRandom<short>(Main.rand, new short[]
																{
																	170,
																	180,
																	171
																});
															flag12 = true;
															break;
														case 72:
															num136 = 481;
															break;
														case 73:
															num136 = 482;
															break;
														case 74:
															num136 = 430;
															break;
														case 75:
															num136 = 489;
															break;
													}
													if (num136 != -1 && Wiring.CheckMech(num131, num130, 30) && NPC.MechSpawn((float)num133, (float)num134, num136) && flag11)
													{
														if (!flag12 || !Collision.SolidTiles(num131 - 2, num131 + 3, num130, num130 + 2))
														{
															num135 = NPC.NewNPC(num133, num134 - 12, num136, 0, 0f, 0f, 0f, 0f, 255);
														}
														else
														{
															Vector2 position = new Vector2((float)(num133 - 4), (float)(num134 - 22)) - new Vector2(10f);
															Utils.PoofOfSmoke(position);
															NetMessage.SendData(106, -1, -1, "", (int)position.X, position.Y, 0f, 0f, 0, 0, 0);
														}
													}
													if (num135 <= -1)
													{
														if (num132 == 4)
														{
															if (Wiring.CheckMech(num131, num130, 30) && NPC.MechSpawn((float)num133, (float)num134, 1))
															{
																num135 = NPC.NewNPC(num133, num134 - 12, 1, 0, 0f, 0f, 0f, 0f, 255);
															}
														}
														else if (num132 == 7)
														{
															if (Wiring.CheckMech(num131, num130, 30) && NPC.MechSpawn((float)num133, (float)num134, 49))
															{
																num135 = NPC.NewNPC(num133 - 4, num134 - 6, 49, 0, 0f, 0f, 0f, 0f, 255);
															}
														}
														else if (num132 == 8)
														{
															if (Wiring.CheckMech(num131, num130, 30) && NPC.MechSpawn((float)num133, (float)num134, 55))
															{
																num135 = NPC.NewNPC(num133, num134 - 12, 55, 0, 0f, 0f, 0f, 0f, 255);
															}
														}
														else if (num132 == 9)
														{
															if (Wiring.CheckMech(num131, num130, 30) && NPC.MechSpawn((float)num133, (float)num134, 46))
															{
																num135 = NPC.NewNPC(num133, num134 - 12, 46, 0, 0f, 0f, 0f, 0f, 255);
															}
														}
														else if (num132 == 10)
														{
															if (Wiring.CheckMech(num131, num130, 30) && NPC.MechSpawn((float)num133, (float)num134, 21))
															{
																num135 = NPC.NewNPC(num133, num134, 21, 0, 0f, 0f, 0f, 0f, 255);
															}
														}
														else if (num132 == 18)
														{
															if (Wiring.CheckMech(num131, num130, 30) && NPC.MechSpawn((float)num133, (float)num134, 67))
															{
																num135 = NPC.NewNPC(num133, num134 - 12, 67, 0, 0f, 0f, 0f, 0f, 255);
															}
														}
														else if (num132 == 23)
														{
															if (Wiring.CheckMech(num131, num130, 30) && NPC.MechSpawn((float)num133, (float)num134, 63))
															{
																num135 = NPC.NewNPC(num133, num134 - 12, 63, 0, 0f, 0f, 0f, 0f, 255);
															}
														}
														else if (num132 == 27)
														{
															if (Wiring.CheckMech(num131, num130, 30) && NPC.MechSpawn((float)num133, (float)num134, 85))
															{
																num135 = NPC.NewNPC(num133 - 9, num134, 85, 0, 0f, 0f, 0f, 0f, 255);
															}
														}
														else if (num132 == 28)
														{
															if (Wiring.CheckMech(num131, num130, 30) && NPC.MechSpawn((float)num133, (float)num134, 74))
															{
																num135 = NPC.NewNPC(num133, num134 - 12, (int)Utils.SelectRandom<short>(Main.rand, new short[]
																		{
																			74,
																			297,
																			298
																		}), 0, 0f, 0f, 0f, 0f, 255);
															}
														}
														else if (num132 == 34)
														{
															for (int num137 = 0; num137 < 2; num137++)
															{
																for (int num138 = 0; num138 < 3; num138++)
																{
																	Tile tile2 = Main.tile[num131 + num137, num130 + num138];
																	tile2.type = 349;
																	tile2.frameX = (short)(num137 * 18 + 216);
																	tile2.frameY = (short)(num138 * 18);
																}
															}
															Animation.NewTemporaryAnimation(0, 349, num131, num130);
															if (Main.netMode == 2)
															{
																NetMessage.SendTileRange(-1, num131, num130, 2, 3, TileChangeType.None);
															}
														}
														else if (num132 == 42)
														{
															if (Wiring.CheckMech(num131, num130, 30) && NPC.MechSpawn((float)num133, (float)num134, 58))
															{
																num135 = NPC.NewNPC(num133, num134 - 12, 58, 0, 0f, 0f, 0f, 0f, 255);
															}
														}
														else if (num132 == 37)
														{
															if (Wiring.CheckMech(num131, num130, 600) && Item.MechSpawn((float)num133, (float)num134, 58) && Item.MechSpawn((float)num133, (float)num134, 1734) && Item.MechSpawn((float)num133, (float)num134, 1867))
															{
																Item.NewItem(num133, num134 - 16, 0, 0, 58, 1, false, 0, false, false);
															}
														}
														else if (num132 == 50)
														{
															if (Wiring.CheckMech(num131, num130, 30) && NPC.MechSpawn((float)num133, (float)num134, 65))
															{
																if (!Collision.SolidTiles(num131 - 2, num131 + 3, num130, num130 + 2))
																{
																	num135 = NPC.NewNPC(num133, num134 - 12, 65, 0, 0f, 0f, 0f, 0f, 255);
																}
																else
																{
																	Vector2 position2 = new Vector2((float)(num133 - 4), (float)(num134 - 22)) - new Vector2(10f);
																	Utils.PoofOfSmoke(position2);
																	NetMessage.SendData(106, -1, -1, "", (int)position2.X, position2.Y, 0f, 0f, 0, 0, 0);
																}
															}
														}
														else if (num132 == 2)
														{
															if (Wiring.CheckMech(num131, num130, 600) && Item.MechSpawn((float)num133, (float)num134, 184) && Item.MechSpawn((float)num133, (float)num134, 1735) && Item.MechSpawn((float)num133, (float)num134, 1868))
															{
																Item.NewItem(num133, num134 - 16, 0, 0, 184, 1, false, 0, false, false);
															}
														}
														else if (num132 == 17)
														{
															if (Wiring.CheckMech(num131, num130, 600) && Item.MechSpawn((float)num133, (float)num134, 166))
															{
																Item.NewItem(num133, num134 - 20, 0, 0, 166, 1, false, 0, false, false);
															}
														}
														else if (num132 == 40)
														{
															if (Wiring.CheckMech(num131, num130, 300))
															{
																int[] array = new int[10];
																int num139 = 0;
																for (int num140 = 0; num140 < 200; num140++)
																{
																	if (Main.npc[num140].active && (Main.npc[num140].type == 17 || Main.npc[num140].type == 19 || Main.npc[num140].type == 22 || Main.npc[num140].type == 38 || Main.npc[num140].type == 54 || Main.npc[num140].type == 107 || Main.npc[num140].type == 108 || Main.npc[num140].type == 142 || Main.npc[num140].type == 160 || Main.npc[num140].type == 207 || Main.npc[num140].type == 209 || Main.npc[num140].type == 227 || Main.npc[num140].type == 228 || Main.npc[num140].type == 229 || Main.npc[num140].type == 358 || Main.npc[num140].type == 369 || Main.npc[num140].type == 550))
																	{
																		array[num139] = num140;
																		num139++;
																		if (num139 >= 9)
																		{
																			break;
																		}
																	}
																}
																if (num139 > 0)
																{
																	int num141 = array[Main.rand.Next(num139)];
																	Main.npc[num141].position.X = (float)(num133 - Main.npc[num141].width / 2);
																	Main.npc[num141].position.Y = (float)(num134 - Main.npc[num141].height - 1);
																	NetMessage.SendData(23, -1, -1, "", num141, 0f, 0f, 0f, 0, 0, 0);
																}
															}
														}
														else if (num132 == 41 && Wiring.CheckMech(num131, num130, 300))
														{
															int[] array2 = new int[10];
															int num142 = 0;
															for (int num143 = 0; num143 < 200; num143++)
															{
																if (Main.npc[num143].active && (Main.npc[num143].type == 18 || Main.npc[num143].type == 20 || Main.npc[num143].type == 124 || Main.npc[num143].type == 178 || Main.npc[num143].type == 208 || Main.npc[num143].type == 353))
																{
																	array2[num142] = num143;
																	num142++;
																	if (num142 >= 9)
																	{
																		break;
																	}
																}
															}
															if (num142 > 0)
															{
																int num144 = array2[Main.rand.Next(num142)];
																Main.npc[num144].position.X = (float)(num133 - Main.npc[num144].width / 2);
																Main.npc[num144].position.Y = (float)(num134 - Main.npc[num144].height - 1);
																NetMessage.SendData(23, -1, -1, "", num144, 0f, 0f, 0f, 0, 0, 0);
															}
														}
													}
													if (num135 >= 0)
													{
														Main.npc[num135].value = 0f;
														Main.npc[num135].npcSlots = 0f;
														Main.npc[num135].SpawnedFromStatue = true;
														return;
													}
												}
												else if (type == 349)
												{
													int num145 = j - (int)(tile.frameY / 18);
													int num146;
													for (num146 = (int)(tile.frameX / 18); num146 >= 2; num146 -= 2)
													{
													}
													num146 = i - num146;
													Wiring.SkipWire(num146, num145);
													Wiring.SkipWire(num146, num145 + 1);
													Wiring.SkipWire(num146, num145 + 2);
													Wiring.SkipWire(num146 + 1, num145);
													Wiring.SkipWire(num146 + 1, num145 + 1);
													Wiring.SkipWire(num146 + 1, num145 + 2);
													short num147;
													if (Main.tile[num146, num145].frameX == 0)
													{
														num147 = 216;
													}
													else
													{
														num147 = -216;
													}
													for (int num148 = 0; num148 < 2; num148++)
													{
														for (int num149 = 0; num149 < 3; num149++)
														{
															Tile expr_2F51 = Main.tile[num146 + num148, num145 + num149];
															expr_2F51.frameX += num147;
														}
													}
													if (Main.netMode == 2)
													{
														NetMessage.SendTileRange(-1, num146, num145, 2, 3, TileChangeType.None);
													}
													Animation.NewTemporaryAnimation((num147 > 0) ? 0 : 1, 349, num146, num145);
												}
											}
										}
									}
								}
							}
						}
					}
				}
				TileLoader.HitWire(i, j, type);
			}
		}

		private static void Teleport()
		{
			if (Wiring._teleport[0].X < Wiring._teleport[1].X + 3f && Wiring._teleport[0].X > Wiring._teleport[1].X - 3f && Wiring._teleport[0].Y > Wiring._teleport[1].Y - 3f && Wiring._teleport[0].Y < Wiring._teleport[1].Y)
			{
				return;
			}
			Rectangle[] array = new Rectangle[2];
			array[0].X = (int)(Wiring._teleport[0].X * 16f);
			array[0].Width = 48;
			array[0].Height = 48;
			array[0].Y = (int)(Wiring._teleport[0].Y * 16f - (float)array[0].Height);
			array[1].X = (int)(Wiring._teleport[1].X * 16f);
			array[1].Width = 48;
			array[1].Height = 48;
			array[1].Y = (int)(Wiring._teleport[1].Y * 16f - (float)array[1].Height);
			for (int i = 0; i < 2; i++)
			{
				Vector2 value = new Vector2((float)(array[1].X - array[0].X), (float)(array[1].Y - array[0].Y));
				if (i == 1)
				{
					value = new Vector2((float)(array[0].X - array[1].X), (float)(array[0].Y - array[1].Y));
				}
				if (!Wiring.blockPlayerTeleportationForOneIteration)
				{
					for (int j = 0; j < 255; j++)
					{
						if (Main.player[j].active && !Main.player[j].dead && !Main.player[j].teleporting && array[i].Intersects(Main.player[j].getRect()))
						{
							Vector2 vector = Main.player[j].position + value;
							Main.player[j].teleporting = true;
							if (Main.netMode == 2)
							{
								RemoteClient.CheckSection(j, vector, 1);
							}
							Main.player[j].Teleport(vector, 0, 0);
							if (Main.netMode == 2)
							{
								NetMessage.SendData(65, -1, -1, "", 0, (float)j, vector.X, vector.Y, 0, 0, 0);
							}
						}
					}
				}
				for (int k = 0; k < 200; k++)
				{
					if (Main.npc[k].active && !Main.npc[k].teleporting && Main.npc[k].lifeMax > 5 && !Main.npc[k].boss && !Main.npc[k].noTileCollide)
					{
						int type = Main.npc[k].type;
						if (!NPCID.Sets.TeleportationImmune[type] && array[i].Intersects(Main.npc[k].getRect()))
						{
							Main.npc[k].teleporting = true;
							Main.npc[k].Teleport(Main.npc[k].position + value, 0, 0);
						}
					}
				}
			}
			for (int l = 0; l < 255; l++)
			{
				Main.player[l].teleporting = false;
			}
			for (int m = 0; m < 200; m++)
			{
				Main.npc[m].teleporting = false;
			}
		}

		public static void DeActive(int i, int j)
		{
			if (!Main.tile[i, j].active())
			{
				return;
			}
			bool flag = Main.tileSolid[(int)Main.tile[i, j].type] && !TileID.Sets.NotReallySolid[(int)Main.tile[i, j].type];
			ushort type = Main.tile[i, j].type;
			if (type != 314)
			{
				switch (type)
				{
					case 386:
					case 387:
					case 388:
					case 389:
						break;
					default:
						goto IL_85;
				}
			}
			flag = false;
			IL_85:
			if (!flag)
			{
				return;
			}
			if (Main.tile[i, j - 1].active() && (Main.tile[i, j - 1].type == 5 || Main.tile[i, j - 1].type == 21 || Main.tile[i, j - 1].type == 26 || Main.tile[i, j - 1].type == 77 || Main.tile[i, j - 1].type == 72 || Main.tile[i, j - 1].type == 88))
			{
				return;
			}
			Main.tile[i, j].inActive(true);
			WorldGen.SquareTileFrame(i, j, false);
			if (Main.netMode != 1)
			{
				NetMessage.SendTileSquare(-1, i, j, 1, TileChangeType.None);
			}
		}

		public static void ReActive(int i, int j)
		{
			Main.tile[i, j].inActive(false);
			WorldGen.SquareTileFrame(i, j, false);
			if (Main.netMode != 1)
			{
				NetMessage.SendTileSquare(-1, i, j, 1, TileChangeType.None);
			}
		}

		private static void MassWireOperationInner(Point ps, Point pe, Vector2 dropPoint, bool dir, ref int wireCount, ref int actuatorCount)
		{
			Math.Abs(ps.X - pe.X);
			Math.Abs(ps.Y - pe.Y);
			int num = Math.Sign(pe.X - ps.X);
			int num2 = Math.Sign(pe.Y - ps.Y);
			WiresUI.Settings.MultiToolMode toolMode = WiresUI.Settings.ToolMode;
			Point pt = default(Point);
			bool flag = false;
			Item.StartCachingType(530);
			Item.StartCachingType(849);
			int num3;
			int num4;
			int num5;
			if (dir)
			{
				pt.X = ps.X;
				num3 = ps.Y;
				num4 = pe.Y;
				num5 = num2;
			}
			else
			{
				pt.Y = ps.Y;
				num3 = ps.X;
				num4 = pe.X;
				num5 = num;
			}
			int num6 = num3;
			while (num6 != num4 && !flag)
			{
				if (dir)
				{
					pt.Y = num6;
				}
				else
				{
					pt.X = num6;
				}
				bool? flag2 = Wiring.MassWireOperationStep(pt, toolMode, ref wireCount, ref actuatorCount);
				if (flag2.HasValue && !flag2.Value)
				{
					flag = true;
					break;
				}
				num6 += num5;
			}
			if (dir)
			{
				pt.Y = pe.Y;
				num3 = ps.X;
				num4 = pe.X;
				num5 = num;
			}
			else
			{
				pt.X = pe.X;
				num3 = ps.Y;
				num4 = pe.Y;
				num5 = num2;
			}
			int num7 = num3;
			while (num7 != num4 && !flag)
			{
				if (!dir)
				{
					pt.Y = num7;
				}
				else
				{
					pt.X = num7;
				}
				bool? flag3 = Wiring.MassWireOperationStep(pt, toolMode, ref wireCount, ref actuatorCount);
				if (flag3.HasValue && !flag3.Value)
				{
					flag = true;
					break;
				}
				num7 += num5;
			}
			if (!flag)
			{
				Wiring.MassWireOperationStep(pe, toolMode, ref wireCount, ref actuatorCount);
			}
			Item.DropCache(dropPoint, Vector2.Zero, 530, true);
			Item.DropCache(dropPoint, Vector2.Zero, 849, true);
		}

		private static bool? MassWireOperationStep(Point pt, WiresUI.Settings.MultiToolMode mode, ref int wiresLeftToConsume, ref int actuatorsLeftToConstume)
		{
			if (!WorldGen.InWorld(pt.X, pt.Y, 1))
			{
				return null;
			}
			Tile tile = Main.tile[pt.X, pt.Y];
			if (tile == null)
			{
				return null;
			}
			if (!mode.HasFlag(WiresUI.Settings.MultiToolMode.Cutter))
			{
				if (mode.HasFlag(WiresUI.Settings.MultiToolMode.Red) && !tile.wire())
				{
					if (wiresLeftToConsume <= 0)
					{
						return new bool?(false);
					}
					wiresLeftToConsume--;
					WorldGen.PlaceWire(pt.X, pt.Y);
					NetMessage.SendData(17, -1, -1, "", 5, (float)pt.X, (float)pt.Y, 0f, 0, 0, 0);
				}
				if (mode.HasFlag(WiresUI.Settings.MultiToolMode.Green) && !tile.wire3())
				{
					if (wiresLeftToConsume <= 0)
					{
						return new bool?(false);
					}
					wiresLeftToConsume--;
					WorldGen.PlaceWire3(pt.X, pt.Y);
					NetMessage.SendData(17, -1, -1, "", 12, (float)pt.X, (float)pt.Y, 0f, 0, 0, 0);
				}
				if (mode.HasFlag(WiresUI.Settings.MultiToolMode.Blue) && !tile.wire2())
				{
					if (wiresLeftToConsume <= 0)
					{
						return new bool?(false);
					}
					wiresLeftToConsume--;
					WorldGen.PlaceWire2(pt.X, pt.Y);
					NetMessage.SendData(17, -1, -1, "", 10, (float)pt.X, (float)pt.Y, 0f, 0, 0, 0);
				}
				if (mode.HasFlag(WiresUI.Settings.MultiToolMode.Yellow) && !tile.wire4())
				{
					if (wiresLeftToConsume <= 0)
					{
						return new bool?(false);
					}
					wiresLeftToConsume--;
					WorldGen.PlaceWire4(pt.X, pt.Y);
					NetMessage.SendData(17, -1, -1, "", 16, (float)pt.X, (float)pt.Y, 0f, 0, 0, 0);
				}
				if (mode.HasFlag(WiresUI.Settings.MultiToolMode.Actuator) && !tile.actuator())
				{
					if (actuatorsLeftToConstume <= 0)
					{
						return new bool?(false);
					}
					actuatorsLeftToConstume--;
					WorldGen.PlaceActuator(pt.X, pt.Y);
					NetMessage.SendData(17, -1, -1, "", 8, (float)pt.X, (float)pt.Y, 0f, 0, 0, 0);
				}
			}
			if (mode.HasFlag(WiresUI.Settings.MultiToolMode.Cutter))
			{
				if (mode.HasFlag(WiresUI.Settings.MultiToolMode.Red) && tile.wire() && WorldGen.KillWire(pt.X, pt.Y))
				{
					NetMessage.SendData(17, -1, -1, "", 6, (float)pt.X, (float)pt.Y, 0f, 0, 0, 0);
				}
				if (mode.HasFlag(WiresUI.Settings.MultiToolMode.Green) && tile.wire3() && WorldGen.KillWire3(pt.X, pt.Y))
				{
					NetMessage.SendData(17, -1, -1, "", 13, (float)pt.X, (float)pt.Y, 0f, 0, 0, 0);
				}
				if (mode.HasFlag(WiresUI.Settings.MultiToolMode.Blue) && tile.wire2() && WorldGen.KillWire2(pt.X, pt.Y))
				{
					NetMessage.SendData(17, -1, -1, "", 11, (float)pt.X, (float)pt.Y, 0f, 0, 0, 0);
				}
				if (mode.HasFlag(WiresUI.Settings.MultiToolMode.Yellow) && tile.wire4() && WorldGen.KillWire4(pt.X, pt.Y))
				{
					NetMessage.SendData(17, -1, -1, "", 17, (float)pt.X, (float)pt.Y, 0f, 0, 0, 0);
				}
				if (mode.HasFlag(WiresUI.Settings.MultiToolMode.Actuator) && tile.actuator() && WorldGen.KillActuator(pt.X, pt.Y))
				{
					NetMessage.SendData(17, -1, -1, "", 9, (float)pt.X, (float)pt.Y, 0f, 0, 0, 0);
				}
			}
			Dust.QuickDust(pt, Color.Silver);
			return new bool?(true);
		}
	}
}
