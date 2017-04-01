using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.World.Generation;

namespace Terraria.GameContent.Biomes
{
	public class GraniteBiome : MicroBiome
	{
		private struct Magma
		{
			public readonly float Pressure;
			public readonly float Resistance;
			public readonly bool IsActive;

			private Magma(float pressure, float resistance, bool active)
			{
				this.Pressure = pressure;
				this.Resistance = resistance;
				this.IsActive = active;
			}

			public GraniteBiome.Magma ToFlow()
			{
				return new GraniteBiome.Magma(this.Pressure, this.Resistance, true);
			}

			public static GraniteBiome.Magma CreateFlow(float pressure, float resistance = 0f)
			{
				return new GraniteBiome.Magma(pressure, resistance, true);
			}

			public static GraniteBiome.Magma CreateEmpty(float resistance = 0f)
			{
				return new GraniteBiome.Magma(0f, resistance, false);
			}
		}

		private const int MAX_MAGMA_ITERATIONS = 300;
		private static GraniteBiome.Magma[,] _sourceMagmaMap = new GraniteBiome.Magma[200, 200];
		private static GraniteBiome.Magma[,] _targetMagmaMap = new GraniteBiome.Magma[200, 200];

		public override bool Place(Point origin, StructureMap structures)
		{
			if (GenBase._tiles[origin.X, origin.Y].active())
			{
				return false;
			}
			int length = GraniteBiome._sourceMagmaMap.GetLength(0);
			int length2 = GraniteBiome._sourceMagmaMap.GetLength(1);
			int num = length / 2;
			int num2 = length2 / 2;
			origin.X -= num;
			origin.Y -= num2;
			for (int i = 0; i < length; i++)
			{
				for (int j = 0; j < length2; j++)
				{
					int i2 = i + origin.X;
					int j2 = j + origin.Y;
					GraniteBiome._sourceMagmaMap[i, j] = GraniteBiome.Magma.CreateEmpty(WorldGen.SolidTile(i2, j2) ? 4f : 1f);
					GraniteBiome._targetMagmaMap[i, j] = GraniteBiome._sourceMagmaMap[i, j];
				}
			}
			int num3 = num;
			int num4 = num;
			int num5 = num2;
			int num6 = num2;
			for (int k = 0; k < 300; k++)
			{
				for (int l = num3; l <= num4; l++)
				{
					for (int m = num5; m <= num6; m++)
					{
						GraniteBiome.Magma magma = GraniteBiome._sourceMagmaMap[l, m];
						if (magma.IsActive)
						{
							float num7 = 0f;
							Vector2 value = Vector2.Zero;
							for (int n = -1; n <= 1; n++)
							{
								for (int num8 = -1; num8 <= 1; num8++)
								{
									if (n != 0 || num8 != 0)
									{
										Vector2 value2 = new Vector2((float)n, (float)num8);
										value2.Normalize();
										GraniteBiome.Magma magma2 = GraniteBiome._sourceMagmaMap[l + n, m + num8];
										if (magma.Pressure > 0.01f && !magma2.IsActive)
										{
											if (n == -1)
											{
												num3 = Utils.Clamp<int>(l + n, 1, num3);
											}
											else
											{
												num4 = Utils.Clamp<int>(l + n, num4, length - 2);
											}
											if (num8 == -1)
											{
												num5 = Utils.Clamp<int>(m + num8, 1, num5);
											}
											else
											{
												num6 = Utils.Clamp<int>(m + num8, num6, length2 - 2);
											}
											GraniteBiome._targetMagmaMap[l + n, m + num8] = magma2.ToFlow();
										}
										float pressure = magma2.Pressure;
										num7 += pressure;
										value += pressure * value2;
									}
								}
							}
							num7 /= 8f;
							if (num7 > magma.Resistance)
							{
								float num9 = value.Length() / 8f;
								float num10 = Math.Max(num7 - num9 - magma.Pressure, 0f) + num9 + magma.Pressure * 0.875f - magma.Resistance;
								num10 = Math.Max(0f, num10);
								GraniteBiome._targetMagmaMap[l, m] = GraniteBiome.Magma.CreateFlow(num10, Math.Max(0f, magma.Resistance - num10 * 0.02f));
							}
						}
					}
				}
				if (k < 2)
				{
					GraniteBiome._targetMagmaMap[num, num2] = GraniteBiome.Magma.CreateFlow(25f, 0f);
				}
				Utils.Swap<GraniteBiome.Magma[,]>(ref GraniteBiome._sourceMagmaMap, ref GraniteBiome._targetMagmaMap);
			}
			bool flag = origin.Y + num2 > WorldGen.lavaLine - 30;
			bool flag2 = false;
			int num11 = -50;
			while (num11 < 50 && !flag2)
			{
				int num12 = -50;
				while (num12 < 50 && !flag2)
				{
					if (GenBase._tiles[origin.X + num + num11, origin.Y + num2 + num12].active())
					{
						ushort type = GenBase._tiles[origin.X + num + num11, origin.Y + num2 + num12].type;
						if (type != 147)
						{
							switch (type)
							{
								case 161:
								case 162:
								case 163:
									break;
								default:
									if (type != 200)
									{
										goto IL_400;
									}
									break;
							}
						}
						flag = false;
						flag2 = true;
					}
					IL_400:
					num12++;
				}
				num11++;
			}
			for (int num13 = num3; num13 <= num4; num13++)
			{
				for (int num14 = num5; num14 <= num6; num14++)
				{
					GraniteBiome.Magma magma3 = GraniteBiome._sourceMagmaMap[num13, num14];
					if (magma3.IsActive)
					{
						Tile tile = GenBase._tiles[origin.X + num13, origin.Y + num14];
						float num15 = (float)Math.Sin((double)((float)(origin.Y + num14) * 0.4f)) * 0.7f + 1.2f;
						float num16 = 0.2f + 0.5f / (float)Math.Sqrt((double)Math.Max(0f, magma3.Pressure - magma3.Resistance));
						float num17 = 1f - Math.Max(0f, num15 * num16);
						num17 = Math.Max(num17, magma3.Pressure / 15f);
						if (num17 > 0.35f + (WorldGen.SolidTile(origin.X + num13, origin.Y + num14) ? 0f : 0.5f))
						{
							if (TileID.Sets.Ore[(int)tile.type])
							{
								tile.ResetToType(tile.type);
							}
							else
							{
								tile.ResetToType(368);
							}
							tile.wall = 180;
						}
						else if (magma3.Resistance < 0.01f)
						{
							WorldUtils.ClearTile(origin.X + num13, origin.Y + num14, false);
							tile.wall = 180;
						}
						if (tile.liquid > 0 && flag)
						{
							tile.liquidType(1);
						}
					}
				}
			}
			List<Point16> list = new List<Point16>();
			for (int num18 = num3; num18 <= num4; num18++)
			{
				for (int num19 = num5; num19 <= num6; num19++)
				{
					GraniteBiome.Magma magma4 = GraniteBiome._sourceMagmaMap[num18, num19];
					if (magma4.IsActive)
					{
						int num20 = 0;
						int num21 = num18 + origin.X;
						int num22 = num19 + origin.Y;
						if (WorldGen.SolidTile(num21, num22))
						{
							for (int num23 = -1; num23 <= 1; num23++)
							{
								for (int num24 = -1; num24 <= 1; num24++)
								{
									if (WorldGen.SolidTile(num21 + num23, num22 + num24))
									{
										num20++;
									}
								}
							}
							if (num20 < 3)
							{
								list.Add(new Point16(num21, num22));
							}
						}
					}
				}
			}
			foreach (Point16 current in list)
			{
				int x = (int)current.X;
				int y = (int)current.Y;
				WorldUtils.ClearTile(x, y, true);
				GenBase._tiles[x, y].wall = 180;
			}
			list.Clear();
			for (int num25 = num3; num25 <= num4; num25++)
			{
				for (int num26 = num5; num26 <= num6; num26++)
				{
					GraniteBiome.Magma magma5 = GraniteBiome._sourceMagmaMap[num25, num26];
					int num27 = num25 + origin.X;
					int num28 = num26 + origin.Y;
					if (magma5.IsActive)
					{
						WorldUtils.TileFrame(num27, num28, false);
						WorldGen.SquareWallFrame(num27, num28, true);
						if (GenBase._random.Next(8) == 0 && GenBase._tiles[num27, num28].active())
						{
							if (!GenBase._tiles[num27, num28 + 1].active())
							{
								WorldGen.PlaceTight(num27, num28 + 1, 165, false);
							}
							if (!GenBase._tiles[num27, num28 - 1].active())
							{
								WorldGen.PlaceTight(num27, num28 - 1, 165, false);
							}
						}
						if (GenBase._random.Next(2) == 0)
						{
							Tile.SmoothSlope(num27, num28, true);
						}
					}
				}
			}
			return true;
		}
	}
}
