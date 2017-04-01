using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.Utilities;
using Terraria.ModLoader;

namespace Terraria
{
	public class Lighting
	{
		private class LightingSwipeData
		{
			public int outerLoopStart;
			public int outerLoopEnd;
			public int innerLoop1Start;
			public int innerLoop1End;
			public int innerLoop2Start;
			public int innerLoop2End;
			public UnifiedRandom rand;
			public Action<Lighting.LightingSwipeData> function;
			public Lighting.LightingState[][] jaggedArray;

			public LightingSwipeData()
			{
				this.innerLoop1Start = 0;
				this.outerLoopStart = 0;
				this.innerLoop1End = 0;
				this.outerLoopEnd = 0;
				this.innerLoop2Start = 0;
				this.innerLoop2End = 0;
				this.function = null;
				this.rand = new UnifiedRandom();
			}

			public void CopyFrom(Lighting.LightingSwipeData from)
			{
				this.innerLoop1Start = from.innerLoop1Start;
				this.outerLoopStart = from.outerLoopStart;
				this.innerLoop1End = from.innerLoop1End;
				this.outerLoopEnd = from.outerLoopEnd;
				this.innerLoop2Start = from.innerLoop2Start;
				this.innerLoop2End = from.innerLoop2End;
				this.function = from.function;
				this.jaggedArray = from.jaggedArray;
			}
		}

		private class LightingState
		{
			public float r;
			public float r2;
			public float g;
			public float g2;
			public float b;
			public float b2;
			public bool stopLight;
			public bool wetLight;
			public bool honeyLight;

			public Vector3 ToVector3()
			{
				return new Vector3(this.r, this.g, this.b);
			}
		}

		private struct ColorTriplet
		{
			public float r;
			public float g;
			public float b;

			public ColorTriplet(float R, float G, float B)
			{
				this.r = R;
				this.g = G;
				this.b = B;
			}

			public ColorTriplet(float averageColor)
			{
				this.b = averageColor;
				this.g = averageColor;
				this.r = averageColor;
			}
		}

		public static int maxRenderCount = 4;
		public static float brightness = 1f;
		public static float defBrightness = 1f;
		public static int lightMode = 0;
		public static bool RGB = true;
		private static float oldSkyColor = 0f;
		private static float skyColor = 0f;
		private static int lightCounter = 0;
		public static int offScreenTiles = 45;
		public static int offScreenTiles2 = 35;
		private static int firstTileX;
		private static int lastTileX;
		private static int firstTileY;
		private static int lastTileY;
		public static int LightingThreads = 0;
		private static Lighting.LightingState[][] states;
		private static Lighting.LightingState[][] axisFlipStates;
		private static Lighting.LightingSwipeData swipe;
		private static Lighting.LightingSwipeData[] threadSwipes;
		private static CountdownEvent countdown;
		public static int scrX;
		public static int scrY;
		public static int minX;
		public static int maxX;
		public static int minY;
		public static int maxY;
		private static int maxTempLights = 2000;
		private static Dictionary<Point16, Lighting.ColorTriplet> tempLights;
		private static int firstToLightX;
		private static int firstToLightY;
		private static int lastToLightX;
		private static int lastToLightY;
		internal static float negLight = 0.04f;
		private static float negLight2 = 0.16f;
		private static float wetLightR = 0.16f;
		private static float wetLightG = 0.16f;
		private static float wetLightB = 0.16f;
		private static float honeyLightR = 0.16f;
		private static float honeyLightG = 0.16f;
		private static float honeyLightB = 0.16f;
		internal static float blueWave = 1f;
		private static int blueDir = 1;
		private static int minX7;
		private static int maxX7;
		private static int minY7;
		private static int maxY7;
		private static int firstTileX7;
		private static int lastTileX7;
		private static int lastTileY7;
		private static int firstTileY7;
		private static int firstToLightX7;
		private static int lastToLightX7;
		private static int firstToLightY7;
		private static int lastToLightY7;
		private static int firstToLightX27;
		private static int lastToLightX27;
		private static int firstToLightY27;
		private static int lastToLightY27;

		public static void Initialize(bool resize = false)
		{
			if (!resize)
			{
				Lighting.tempLights = new Dictionary<Point16, Lighting.ColorTriplet>();
				Lighting.swipe = new Lighting.LightingSwipeData();
				Lighting.countdown = new CountdownEvent(0);
				Lighting.threadSwipes = new Lighting.LightingSwipeData[Environment.ProcessorCount];
				for (int i = 0; i < Lighting.threadSwipes.Length; i++)
				{
					Lighting.threadSwipes[i] = new Lighting.LightingSwipeData();
				}
			}
			int num = Main.screenWidth / 16 + Lighting.offScreenTiles * 2 + 10;
			int num2 = Main.screenHeight / 16 + Lighting.offScreenTiles * 2 + 10;
			if (Lighting.states == null || Lighting.states.Length < num || Lighting.states[0].Length < num2)
			{
				Lighting.states = new Lighting.LightingState[num][];
				Lighting.axisFlipStates = new Lighting.LightingState[num2][];
				for (int j = 0; j < num2; j++)
				{
					Lighting.axisFlipStates[j] = new Lighting.LightingState[num];
				}
				for (int k = 0; k < num; k++)
				{
					Lighting.LightingState[] array = new Lighting.LightingState[num2];
					for (int l = 0; l < num2; l++)
					{
						Lighting.LightingState lightingState = new Lighting.LightingState();
						array[l] = lightingState;
						Lighting.axisFlipStates[l][k] = lightingState;
					}
					Lighting.states[k] = array;
				}
			}
		}

		public static void LightTiles(int firstX, int lastX, int firstY, int lastY)
		{
			Main.render = true;
			Lighting.oldSkyColor = Lighting.skyColor;
			float num = (float)Main.tileColor.R / 255f;
			float num2 = (float)Main.tileColor.G / 255f;
			float num3 = (float)Main.tileColor.B / 255f;
			Lighting.skyColor = (num + num2 + num3) / 3f;
			if (Lighting.lightMode < 2)
			{
				Lighting.brightness = 1.2f;
				Lighting.offScreenTiles2 = 34;
				Lighting.offScreenTiles = 40;
			}
			else
			{
				Lighting.brightness = 1f;
				Lighting.offScreenTiles2 = 18;
				Lighting.offScreenTiles = 23;
			}
			if (Main.player[Main.myPlayer].blind)
			{
				Lighting.brightness = 1f;
			}
			Lighting.defBrightness = Lighting.brightness;
			Lighting.firstTileX = firstX;
			Lighting.lastTileX = lastX;
			Lighting.firstTileY = firstY;
			Lighting.lastTileY = lastY;
			Lighting.firstToLightX = Lighting.firstTileX - Lighting.offScreenTiles;
			Lighting.firstToLightY = Lighting.firstTileY - Lighting.offScreenTiles;
			Lighting.lastToLightX = Lighting.lastTileX + Lighting.offScreenTiles;
			Lighting.lastToLightY = Lighting.lastTileY + Lighting.offScreenTiles;
			if (Lighting.firstToLightX < 0)
			{
				Lighting.firstToLightX = 0;
			}
			if (Lighting.lastToLightX >= Main.maxTilesX)
			{
				Lighting.lastToLightX = Main.maxTilesX - 1;
			}
			if (Lighting.firstToLightY < 0)
			{
				Lighting.firstToLightY = 0;
			}
			if (Lighting.lastToLightY >= Main.maxTilesY)
			{
				Lighting.lastToLightY = Main.maxTilesY - 1;
			}
			Lighting.lightCounter++;
			Main.renderCount++;
			int num4 = Main.screenWidth / 16 + Lighting.offScreenTiles * 2;
			int num5 = Main.screenHeight / 16 + Lighting.offScreenTiles * 2;
			Vector2 vector = Main.screenLastPosition;
			if (Main.renderCount < 3)
			{
				Lighting.doColors();
			}
			if (Main.renderCount == 2)
			{
				vector = Main.screenPosition;
				int num6 = (int)(Main.screenPosition.X / 16f) - Lighting.scrX;
				int num7 = (int)(Main.screenPosition.Y / 16f) - Lighting.scrY;
				if (num6 > 16)
				{
					num6 = 0;
				}
				if (num7 > 16)
				{
					num7 = 0;
				}
				int num8 = 0;
				int num9 = num4;
				int num10 = 0;
				int num11 = num5;
				if (num6 < 0)
				{
					num8 -= num6;
				}
				else
				{
					num9 -= num6;
				}
				if (num7 < 0)
				{
					num10 -= num7;
				}
				else
				{
					num11 -= num7;
				}
				if (Lighting.RGB)
				{
					int num12 = num4;
					if (Lighting.states.Length <= num12 + num6)
					{
						num12 = Lighting.states.Length - num6 - 1;
					}
					for (int i = num8; i < num12; i++)
					{
						Lighting.LightingState[] array = Lighting.states[i];
						Lighting.LightingState[] array2 = Lighting.states[i + num6];
						int num13 = num11;
						if (array2.Length <= num13 + num6)
						{
							num13 = array2.Length - num7 - 1;
						}
						for (int j = num10; j < num13; j++)
						{
							Lighting.LightingState lightingState = array[j];
							Lighting.LightingState lightingState2 = array2[j + num7];
							lightingState.r = lightingState2.r2;
							lightingState.g = lightingState2.g2;
							lightingState.b = lightingState2.b2;
						}
					}
				}
				else
				{
					int num14 = num9;
					if (Lighting.states.Length <= num14 + num6)
					{
						num14 = Lighting.states.Length - num6 - 1;
					}
					for (int k = num8; k < num14; k++)
					{
						Lighting.LightingState[] array3 = Lighting.states[k];
						Lighting.LightingState[] array4 = Lighting.states[k + num6];
						int num15 = num11;
						if (array4.Length <= num15 + num6)
						{
							num15 = array4.Length - num7 - 1;
						}
						for (int l = num10; l < num15; l++)
						{
							Lighting.LightingState lightingState3 = array3[l];
							Lighting.LightingState lightingState4 = array4[l + num7];
							lightingState3.r = lightingState4.r2;
							lightingState3.g = lightingState4.r2;
							lightingState3.b = lightingState4.r2;
						}
					}
				}
			}
			else if (!Main.renderNow)
			{
				int num16 = (int)(Main.screenPosition.X / 16f) - (int)(vector.X / 16f);
				if (num16 > 5 || num16 < -5)
				{
					num16 = 0;
				}
				int num17;
				int num18;
				int num19;
				if (num16 < 0)
				{
					num17 = -1;
					num16 *= -1;
					num18 = num4;
					num19 = num16;
				}
				else
				{
					num17 = 1;
					num18 = 0;
					num19 = num4 - num16;
				}
				int num20 = (int)(Main.screenPosition.Y / 16f) - (int)(vector.Y / 16f);
				if (num20 > 5 || num20 < -5)
				{
					num20 = 0;
				}
				int num21;
				int num22;
				int num23;
				if (num20 < 0)
				{
					num21 = -1;
					num20 *= -1;
					num22 = num5;
					num23 = num20;
				}
				else
				{
					num21 = 1;
					num22 = 0;
					num23 = num5 - num20;
				}
				if (num16 != 0 || num20 != 0)
				{
					for (int num24 = num18; num24 != num19; num24 += num17)
					{
						Lighting.LightingState[] array5 = Lighting.states[num24];
						Lighting.LightingState[] array6 = Lighting.states[num24 + num16 * num17];
						for (int num25 = num22; num25 != num23; num25 += num21)
						{
							Lighting.LightingState lightingState5 = array5[num25];
							Lighting.LightingState lightingState6 = array6[num25 + num20 * num21];
							lightingState5.r = lightingState6.r;
							lightingState5.g = lightingState6.g;
							lightingState5.b = lightingState6.b;
						}
					}
				}
				if (Netplay.Connection.StatusMax > 0)
				{
					Main.mapTime = 1;
				}
				if (Main.mapTime == 0 && Main.mapEnabled && Main.renderCount == 3)
				{
					try
					{
						Main.mapTime = Main.mapTimeMax;
						Main.updateMap = true;
						Main.mapMinX = Lighting.firstToLightX + Lighting.offScreenTiles;
						Main.mapMaxX = Lighting.lastToLightX - Lighting.offScreenTiles;
						Main.mapMinY = Lighting.firstToLightY + Lighting.offScreenTiles;
						Main.mapMaxY = Lighting.lastToLightY - Lighting.offScreenTiles;
						for (int m = Main.mapMinX; m < Main.mapMaxX; m++)
						{
							Lighting.LightingState[] array7 = Lighting.states[m - Lighting.firstTileX + Lighting.offScreenTiles];
							for (int n = Main.mapMinY; n < Main.mapMaxY; n++)
							{
								Lighting.LightingState lightingState7 = array7[n - Lighting.firstTileY + Lighting.offScreenTiles];
								Tile tile = Main.tile[m, n];
								float num26 = 0f;
								if (lightingState7.r > num26)
								{
									num26 = lightingState7.r;
								}
								if (lightingState7.g > num26)
								{
									num26 = lightingState7.g;
								}
								if (lightingState7.b > num26)
								{
									num26 = lightingState7.b;
								}
								if (Lighting.lightMode < 2)
								{
									num26 *= 1.5f;
								}
								byte b = (byte)Math.Min(255f, num26 * 255f);
								if ((double)n < Main.worldSurface && !tile.active() && tile.wall == 0 && tile.liquid == 0)
								{
									b = 22;
								}
								if (b > 18 || Main.Map[m, n].Light > 0)
								{
									if (b < 22)
									{
										b = 22;
									}
									Main.Map.UpdateLighting(m, n, b);
								}
							}
						}
					}
					catch
					{
					}
				}
				if (Lighting.oldSkyColor != Lighting.skyColor)
				{
					int num27 = Lighting.firstToLightX;
					int num28 = Lighting.firstToLightY;
					int num29 = Lighting.lastToLightX;
					int num30;
					if ((double)Lighting.lastToLightY >= Main.worldSurface)
					{
						num30 = (int)Main.worldSurface - 1;
					}
					else
					{
						num30 = Lighting.lastToLightY;
					}
					if ((double)num28 < Main.worldSurface)
					{
						for (int num31 = num27; num31 < num29; num31++)
						{
							Lighting.LightingState[] array8 = Lighting.states[num31 - Lighting.firstToLightX];
							for (int num32 = num28; num32 < num30; num32++)
							{
								Lighting.LightingState lightingState8 = array8[num32 - Lighting.firstToLightY];
								Tile tile2 = Main.tile[num31, num32];
								if (tile2 == null)
								{
									tile2 = new Tile();
									Main.tile[num31, num32] = tile2;
								}
								if ((!tile2.active() || !Main.tileNoSunLight[(int)tile2.type]) && lightingState8.r < Lighting.skyColor && tile2.liquid < 200 && (Main.wallLight[(int)tile2.wall] || tile2.wall == 73))
								{
									lightingState8.r = num;
									if (lightingState8.g < Lighting.skyColor)
									{
										lightingState8.g = num2;
									}
									if (lightingState8.b < Lighting.skyColor)
									{
										lightingState8.b = num3;
									}
								}
							}
						}
					}
				}
			}
			else
			{
				Lighting.lightCounter = 0;
			}
			if (Main.renderCount > Lighting.maxRenderCount)
			{
				Lighting.PreRenderPhase();
			}
		}

		public static void PreRenderPhase()
		{
			float num = (float)Main.tileColor.R / 255f;
			float num2 = (float)Main.tileColor.G / 255f;
			float num3 = (float)Main.tileColor.B / 255f;
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			int num4 = 0;
			int num5 = Main.screenWidth / 16 + Lighting.offScreenTiles * 2 + 10;
			int num6 = 0;
			int num7 = Main.screenHeight / 16 + Lighting.offScreenTiles * 2 + 10;
			Lighting.minX = num5;
			Lighting.maxX = num4;
			Lighting.minY = num7;
			Lighting.maxY = num6;
			if (Lighting.lightMode == 0 || Lighting.lightMode == 3)
			{
				Lighting.RGB = true;
			}
			else
			{
				Lighting.RGB = false;
			}
			for (int i = num4; i < num5; i++)
			{
				Lighting.LightingState[] array = Lighting.states[i];
				for (int j = num6; j < num7; j++)
				{
					Lighting.LightingState lightingState = array[j];
					lightingState.r2 = 0f;
					lightingState.g2 = 0f;
					lightingState.b2 = 0f;
					lightingState.stopLight = false;
					lightingState.wetLight = false;
					lightingState.honeyLight = false;
				}
			}
			if (Main.wof >= 0 && Main.player[Main.myPlayer].gross)
			{
				try
				{
					int num8 = (int)Main.screenPosition.Y / 16 - 10;
					int num9 = (int)(Main.screenPosition.Y + (float)Main.screenHeight) / 16 + 10;
					int num10 = (int)Main.npc[Main.wof].position.X / 16;
					if (Main.npc[Main.wof].direction > 0)
					{
						num10 -= 3;
					}
					else
					{
						num10 += 2;
					}
					int num11 = num10 + 8;
					float num12 = 0.5f * Main.demonTorch + 1f * (1f - Main.demonTorch);
					float num13 = 0.3f;
					float num14 = 1f * Main.demonTorch + 0.5f * (1f - Main.demonTorch);
					num12 *= 0.2f;
					num13 *= 0.1f;
					num14 *= 0.3f;
					for (int k = num10; k <= num11; k++)
					{
						Lighting.LightingState[] array2 = Lighting.states[k - num10];
						for (int l = num8; l <= num9; l++)
						{
							Lighting.LightingState lightingState2 = array2[l - Lighting.firstToLightY];
							if (lightingState2.r2 < num12)
							{
								lightingState2.r2 = num12;
							}
							if (lightingState2.g2 < num13)
							{
								lightingState2.g2 = num13;
							}
							if (lightingState2.b2 < num14)
							{
								lightingState2.b2 = num14;
							}
						}
					}
				}
				catch
				{
				}
			}
			Main.sandTiles = 0;
			Main.evilTiles = 0;
			Main.bloodTiles = 0;
			Main.shroomTiles = 0;
			Main.snowTiles = 0;
			Main.holyTiles = 0;
			Main.meteorTiles = 0;
			Main.jungleTiles = 0;
			Main.dungeonTiles = 0;
			Main.campfire = false;
			Main.sunflower = false;
			Main.starInBottle = false;
			Main.heartLantern = false;
			Main.campfire = false;
			Main.clock = false;
			Main.musicBox = -1;
			Main.waterCandles = 0;
			for (int m = 0; m < Main.player[Main.myPlayer].NPCBannerBuff.Length; m++)
			{
				Main.player[Main.myPlayer].NPCBannerBuff[m] = false;
			}
			Main.player[Main.myPlayer].hasBanner = false;
			WorldHooks.ResetNearbyTileEffects();
			int[] screenTileCounts = Main.screenTileCounts;
			Array.Clear(screenTileCounts, 0, screenTileCounts.Length);
			num4 = Lighting.firstToLightX;
			num5 = Lighting.lastToLightX;
			num6 = Lighting.firstToLightY;
			num7 = Lighting.lastToLightY;
			int num15 = (num5 - num4 - Main.zoneX) / 2;
			int num16 = (num7 - num6 - Main.zoneY) / 2;
			Main.fountainColor = -1;
			Main.monolithType = -1;
			for (int n = num4; n < num5; n++)
			{
				Lighting.LightingState[] array3 = Lighting.states[n - Lighting.firstToLightX];
				//patch file: n, num17
				for (int num17 = num6; num17 < num7; num17++)
				{
					Lighting.LightingState lightingState3 = array3[num17 - Lighting.firstToLightY];
					Tile tile = Main.tile[n, num17];
					if (tile == null)
					{
						tile = new Tile();
						Main.tile[n, num17] = tile;
					}
					float num18 = 0f;
					float num19 = 0f;
					float num20 = 0f;
					if ((double)num17 < Main.worldSurface)
					{
						if ((!tile.active() || !Main.tileNoSunLight[(int)tile.type] || ((tile.slope() != 0 || tile.halfBrick()) && Main.tile[n, num17 - 1].liquid == 0 && Main.tile[n, num17 + 1].liquid == 0 && Main.tile[n - 1, num17].liquid == 0 && Main.tile[n + 1, num17].liquid == 0)) && lightingState3.r2 < Lighting.skyColor && (Main.wallLight[(int)tile.wall] || tile.wall == 73 || tile.wall == 227) && tile.liquid < 200 && (!tile.halfBrick() || Main.tile[n, num17 - 1].liquid < 200))
						{
							num18 = num;
							num19 = num2;
							num20 = num3;
						}
						if ((!tile.active() || tile.halfBrick() || !Main.tileNoSunLight[(int)tile.type]) && tile.wall >= 88 && tile.wall <= 93 && tile.liquid < 255)
						{
							num18 = num;
							num19 = num2;
							num20 = num3;
							switch (tile.wall)
							{
								case 88:
									num18 *= 0.9f;
									num19 *= 0.15f;
									num20 *= 0.9f;
									break;
								case 89:
									num18 *= 0.9f;
									num19 *= 0.9f;
									num20 *= 0.15f;
									break;
								case 90:
									num18 *= 0.15f;
									num19 *= 0.15f;
									num20 *= 0.9f;
									break;
								case 91:
									num18 *= 0.15f;
									num19 *= 0.9f;
									num20 *= 0.15f;
									break;
								case 92:
									num18 *= 0.9f;
									num19 *= 0.15f;
									num20 *= 0.15f;
									break;
								case 93:
									{
										float num21 = 0.2f;
										float num22 = 0.7f - num21;
										num18 *= num22 + (float)Main.DiscoR / 255f * num21;
										num19 *= num22 + (float)Main.DiscoG / 255f * num21;
										num20 *= num22 + (float)Main.DiscoB / 255f * num21;
										break;
									}
							}
						}
						if (!Lighting.RGB)
						{
							float num23 = (num18 + num19 + num20) / 3f;
							num19 = (num18 = (num20 = num23));
						}
						if (lightingState3.r2 < num18)
						{
							lightingState3.r2 = num18;
						}
						if (lightingState3.g2 < num19)
						{
							lightingState3.g2 = num19;
						}
						if (lightingState3.b2 < num20)
						{
							lightingState3.b2 = num20;
						}
					}
					float num24 = 0.55f + (float)Math.Sin((double)(Main.GlobalTime * 2f)) * 0.08f;
					if (num17 > Main.maxTilesY - 200)
					{
						if ((!tile.active() || !Main.tileNoSunLight[(int)tile.type] || ((tile.slope() != 0 || tile.halfBrick()) && Main.tile[n, num17 - 1].liquid == 0 && Main.tile[n, num17 + 1].liquid == 0 && Main.tile[n - 1, num17].liquid == 0 && Main.tile[n + 1, num17].liquid == 0)) && lightingState3.r2 < num24 && (Main.wallLight[(int)tile.wall] || tile.wall == 73 || tile.wall == 227) && tile.liquid < 200 && (!tile.halfBrick() || Main.tile[n, num17 - 1].liquid < 200))
						{
							num18 = num24;
							num19 = num24 * 0.6f;
							num20 = num24 * 0.2f;
						}
						if ((!tile.active() || tile.halfBrick() || !Main.tileNoSunLight[(int)tile.type]) && tile.wall >= 88 && tile.wall <= 93 && tile.liquid < 255)
						{
							num18 = num24;
							num19 = num24 * 0.6f;
							num20 = num24 * 0.2f;
							switch (tile.wall)
							{
								case 88:
									num18 *= 0.9f;
									num19 *= 0.15f;
									num20 *= 0.9f;
									break;
								case 89:
									num18 *= 0.9f;
									num19 *= 0.9f;
									num20 *= 0.15f;
									break;
								case 90:
									num18 *= 0.15f;
									num19 *= 0.15f;
									num20 *= 0.9f;
									break;
								case 91:
									num18 *= 0.15f;
									num19 *= 0.9f;
									num20 *= 0.15f;
									break;
								case 92:
									num18 *= 0.9f;
									num19 *= 0.15f;
									num20 *= 0.15f;
									break;
								case 93:
									{
										float num25 = 0.2f;
										float num26 = 0.7f - num25;
										num18 *= num26 + (float)Main.DiscoR / 255f * num25;
										num19 *= num26 + (float)Main.DiscoG / 255f * num25;
										num20 *= num26 + (float)Main.DiscoB / 255f * num25;
										break;
									}
							}
						}
						if (!Lighting.RGB)
						{
							float num27 = (num18 + num19 + num20) / 3f;
							num19 = (num18 = (num20 = num27));
						}
						if (lightingState3.r2 < num18)
						{
							lightingState3.r2 = num18;
						}
						if (lightingState3.g2 < num19)
						{
							lightingState3.g2 = num19;
						}
						if (lightingState3.b2 < num20)
						{
							lightingState3.b2 = num20;
						}
					}
					ushort wall = tile.wall;
					if (wall <= 137)
					{
						if (wall != 33)
						{
							if (wall != 44)
							{
								if (wall == 137)
								{
									if (!tile.active() || !Main.tileBlockLight[(int)tile.type])
									{
										float num28 = 0.4f;
										num28 += (float)(270 - (int)Main.mouseTextColor) / 1500f;
										num28 += (float)Main.rand.Next(0, 50) * 0.0005f;
										num18 = 1f * num28;
										num19 = 0.5f * num28;
										num20 = 0.1f * num28;
									}
								}
							}
							else if (!tile.active() || !Main.tileBlockLight[(int)tile.type])
							{
								num18 = (float)Main.DiscoR / 255f * 0.15f;
								num19 = (float)Main.DiscoG / 255f * 0.15f;
								num20 = (float)Main.DiscoB / 255f * 0.15f;
							}
						}
						else if (!tile.active() || !Main.tileBlockLight[(int)tile.type])
						{
							num18 = 0.0899999961f;
							num19 = 0.0525000021f;
							num20 = 0.24f;
						}
					}
					else if (wall <= 166)
					{
						switch (wall)
						{
							case 153:
								num18 = 0.6f;
								num19 = 0.3f;
								break;
							case 154:
								num18 = 0.6f;
								num20 = 0.6f;
								break;
							case 155:
								num18 = 0.6f;
								num19 = 0.6f;
								num20 = 0.6f;
								break;
							case 156:
								num19 = 0.6f;
								break;
							default:
								switch (wall)
								{
									case 164:
										num18 = 0.6f;
										break;
									case 165:
										num20 = 0.6f;
										break;
									case 166:
										num18 = 0.6f;
										num19 = 0.6f;
										break;
								}
								break;
						}
					}
					else
					{
						switch (wall)
						{
							case 174:
								if (!tile.active() || !Main.tileBlockLight[(int)tile.type])
								{
									num18 = 0.2975f;
								}
								break;
							case 175:
								if (!tile.active() || !Main.tileBlockLight[(int)tile.type])
								{
									num18 = 0.075f;
									num19 = 0.15f;
									num20 = 0.4f;
								}
								break;
							case 176:
								if (!tile.active() || !Main.tileBlockLight[(int)tile.type])
								{
									num18 = 0.1f;
									num19 = 0.1f;
									num20 = 0.1f;
								}
								break;
							default:
								if (wall == 182 && (!tile.active() || !Main.tileBlockLight[(int)tile.type]))
								{
									num18 = 0.24f;
									num19 = 0.12f;
									num20 = 0.0899999961f;
								}
								break;
						}
					}
					WallLoader.ModifyLight(n, num17, wall, ref num18, ref num19, ref num20);
					if (tile.active())
					{
						bool closer = false;
						if (n > num4 + num15 && n < num5 - num15 && num17 > num6 + num16 && num17 < num7 - num16)
						{
							screenTileCounts[(int)tile.type]++;
							if (tile.type == 215 && tile.frameY < 36)
							{
								Main.campfire = true;
							}
							if (tile.type == 405)
							{
								Main.campfire = true;
							}
							if (tile.type == 42 && tile.frameY >= 324 && tile.frameY <= 358)
							{
								Main.heartLantern = true;
							}
							if (tile.type == 42 && tile.frameY >= 252 && tile.frameY <= 286)
							{
								Main.starInBottle = true;
							}
							if (tile.type == 91 && (tile.frameX >= 396 || tile.frameY >= 54))
							{
								int num29 = (int)(tile.frameX / 18 - 21);
								for (int num30 = (int)tile.frameY; num30 >= 54; num30 -= 54)
								{
									num29 += 90;
									num29 += 21;
								}
								int num31 = Item.BannerToItem(num29);
								if (ItemID.Sets.BannerStrength[num31].Enabled)
								{
									Main.player[Main.myPlayer].NPCBannerBuff[num29] = true;
									Main.player[Main.myPlayer].hasBanner = true;
								}
							}
							closer = true;
						}
						ushort type = tile.type;
						if (type != 139)
						{
							if (type != 207)
							{
								if (type == 410)
								{
									if (tile.frameY >= 56)
									{
										int monolithType = (int)(tile.frameX / 36);
										Main.monolithType = monolithType;
									}
								}
							}
							else if (tile.frameY >= 72)
							{
								switch (tile.frameX / 36)
								{
									case 0:
										Main.fountainColor = 0;
										break;
									case 1:
										Main.fountainColor = 6;
										break;
									case 2:
										Main.fountainColor = 3;
										break;
									case 3:
										Main.fountainColor = 5;
										break;
									case 4:
										Main.fountainColor = 2;
										break;
									case 5:
										Main.fountainColor = 10;
										break;
									case 6:
										Main.fountainColor = 4;
										break;
									case 7:
										Main.fountainColor = 9;
										break;
									default:
										Main.fountainColor = -1;
										break;
								}
							}
						}
						else if (tile.frameX >= 36)
						{
							Main.musicBox = (int)(tile.frameY / 36);
						}
						if (TileLoader.IsModMusicBox(tile) && tile.frameX >= 36)
						{
							Main.musicBox = SoundLoader.tileToMusic[tile.type][tile.frameY / 36 * 36];
						}
						TileLoader.NearbyEffects(n, num17, type, closer);
						if (Main.tileBlockLight[(int)tile.type] && (Lighting.lightMode >= 2 || (tile.type != 131 && !tile.inActive() && tile.slope() == 0)))
						{
							lightingState3.stopLight = true;
						}
						if (tile.type == 104)
						{
							Main.clock = true;
						}
						if (Main.tileLighted[(int)tile.type])
						{
							type = tile.type;
							if (type <= 184)
							{
								if (type <= 77)
								{
									if (type <= 37)
									{
										if (type <= 17)
										{
											if (type != 4)
											{
												if (type != 17)
												{
													goto IL_3257;
												}
												goto IL_28AD;
											}
											else
											{
												if (tile.frameX >= 66)
												{
													goto IL_3257;
												}
												switch (tile.frameY / 22)
												{
													case 0:
														num18 = 1f;
														num19 = 0.95f;
														num20 = 0.8f;
														goto IL_3257;
													case 1:
														num18 = 0f;
														num19 = 0.1f;
														num20 = 1.3f;
														goto IL_3257;
													case 2:
														num18 = 1f;
														num19 = 0.1f;
														num20 = 0.1f;
														goto IL_3257;
													case 3:
														num18 = 0f;
														num19 = 1f;
														num20 = 0.1f;
														goto IL_3257;
													case 4:
														num18 = 0.9f;
														num19 = 0f;
														num20 = 0.9f;
														goto IL_3257;
													case 5:
														num18 = 1.3f;
														num19 = 1.3f;
														num20 = 1.3f;
														goto IL_3257;
													case 6:
														num18 = 0.9f;
														num19 = 0.9f;
														num20 = 0f;
														goto IL_3257;
													case 7:
														num18 = 0.5f * Main.demonTorch + 1f * (1f - Main.demonTorch);
														num19 = 0.3f;
														num20 = 1f * Main.demonTorch + 0.5f * (1f - Main.demonTorch);
														goto IL_3257;
													case 8:
														num18 = 0.85f;
														num19 = 1f;
														num20 = 0.7f;
														goto IL_3257;
													case 9:
														num18 = 0.7f;
														num19 = 0.85f;
														num20 = 1f;
														goto IL_3257;
													case 10:
														num18 = 1f;
														num19 = 0.5f;
														num20 = 0f;
														goto IL_3257;
													case 11:
														num18 = 1.25f;
														num19 = 1.25f;
														num20 = 0.8f;
														goto IL_3257;
													case 12:
														num18 = 0.75f;
														num19 = 1.28249991f;
														num20 = 1.2f;
														goto IL_3257;
													case 13:
														num18 = 0.95f;
														num19 = 0.65f;
														num20 = 1.3f;
														goto IL_3257;
													case 14:
														num18 = (float)Main.DiscoR / 255f;
														num19 = (float)Main.DiscoG / 255f;
														num20 = (float)Main.DiscoB / 255f;
														goto IL_3257;
													case 15:
														num18 = 1f;
														num19 = 0f;
														num20 = 1f;
														goto IL_3257;
													default:
														num18 = 1f;
														num19 = 0.95f;
														num20 = 0.8f;
														goto IL_3257;
												}
											}
										}
										else if (type != 22)
										{
											switch (type)
											{
												case 26:
												case 31:
													{
														if ((tile.type == 31 && tile.frameX >= 36) || (tile.type == 26 && tile.frameX >= 54))
														{
															float num32 = (float)Main.rand.Next(-5, 6) * 0.0025f;
															num18 = 0.5f + num32 * 2f;
															num19 = 0.2f + num32;
															num20 = 0.1f;
															goto IL_3257;
														}
														float num33 = (float)Main.rand.Next(-5, 6) * 0.0025f;
														num18 = 0.31f + num33;
														num19 = 0.1f;
														num20 = 0.44f + num33 * 2f;
														goto IL_3257;
													}
												case 27:
													if (tile.frameY < 36)
													{
														num18 = 0.3f;
														num19 = 0.27f;
														goto IL_3257;
													}
													goto IL_3257;
												case 28:
												case 29:
												case 30:
												case 32:
												case 36:
													goto IL_3257;
												case 33:
													if (tile.frameX == 0)
													{
														int num34 = (int)(tile.frameY / 22);
														if (num34 <= 14)
														{
															switch (num34)
															{
																case 0:
																	num18 = 1f;
																	num19 = 0.95f;
																	num20 = 0.65f;
																	goto IL_3257;
																case 1:
																	num18 = 0.55f;
																	num19 = 0.85f;
																	num20 = 0.35f;
																	goto IL_3257;
																case 2:
																	num18 = 0.65f;
																	num19 = 0.95f;
																	num20 = 0.5f;
																	goto IL_3257;
																case 3:
																	num18 = 0.2f;
																	num19 = 0.75f;
																	num20 = 1f;
																	goto IL_3257;
																default:
																	if (num34 == 14)
																	{
																		num18 = 1f;
																		num19 = 1f;
																		num20 = 0.6f;
																		goto IL_3257;
																	}
																	break;
															}
														}
														else
														{
															switch (num34)
															{
																case 19:
																	num18 = 0.37f;
																	num19 = 0.8f;
																	num20 = 1f;
																	goto IL_3257;
																case 20:
																	num18 = 0f;
																	num19 = 0.9f;
																	num20 = 1f;
																	goto IL_3257;
																case 21:
																	num18 = 0.25f;
																	num19 = 0.7f;
																	num20 = 1f;
																	goto IL_3257;
																case 22:
																case 23:
																case 24:
																	break;
																case 25:
																	num18 = 0.5f * Main.demonTorch + 1f * (1f - Main.demonTorch);
																	num19 = 0.3f;
																	num20 = 1f * Main.demonTorch + 0.5f * (1f - Main.demonTorch);
																	goto IL_3257;
																default:
																	if (num34 == 28)
																	{
																		num18 = 0.9f;
																		num19 = 0.75f;
																		num20 = 1f;
																		goto IL_3257;
																	}
																	break;
															}
														}
														num18 = 1f;
														num19 = 0.95f;
														num20 = 0.65f;
														goto IL_3257;
													}
													goto IL_3257;
												case 34:
													if (tile.frameX < 54)
													{
														switch (tile.frameY / 54)
														{
															case 7:
																num18 = 0.95f;
																num19 = 0.95f;
																num20 = 0.5f;
																goto IL_3257;
															case 8:
																num18 = 0.85f;
																num19 = 0.6f;
																num20 = 1f;
																goto IL_3257;
															case 9:
																num18 = 1f;
																num19 = 0.6f;
																num20 = 0.6f;
																goto IL_3257;
															case 11:
															case 17:
																num18 = 0.75f;
																num19 = 0.9f;
																num20 = 1f;
																goto IL_3257;
															case 15:
																num18 = 1f;
																num19 = 1f;
																num20 = 0.7f;
																goto IL_3257;
															case 18:
																num18 = 1f;
																num19 = 1f;
																num20 = 0.6f;
																goto IL_3257;
															case 24:
																num18 = 0.37f;
																num19 = 0.8f;
																num20 = 1f;
																goto IL_3257;
															case 25:
																num18 = 0f;
																num19 = 0.9f;
																num20 = 1f;
																goto IL_3257;
															case 26:
																num18 = 0.25f;
																num19 = 0.7f;
																num20 = 1f;
																goto IL_3257;
															case 27:
																num18 = 0.55f;
																num19 = 0.85f;
																num20 = 0.35f;
																goto IL_3257;
															case 28:
																num18 = 0.65f;
																num19 = 0.95f;
																num20 = 0.5f;
																goto IL_3257;
															case 29:
																num18 = 0.2f;
																num19 = 0.75f;
																num20 = 1f;
																goto IL_3257;
															case 32:
																num18 = 0.5f * Main.demonTorch + 1f * (1f - Main.demonTorch);
																num19 = 0.3f;
																num20 = 1f * Main.demonTorch + 0.5f * (1f - Main.demonTorch);
																goto IL_3257;
															case 35:
																num18 = 0.9f;
																num19 = 0.75f;
																num20 = 1f;
																goto IL_3257;
														}
														num18 = 1f;
														num19 = 0.95f;
														num20 = 0.8f;
														goto IL_3257;
													}
													goto IL_3257;
												case 35:
													if (tile.frameX < 36)
													{
														num18 = 0.75f;
														num19 = 0.6f;
														num20 = 0.3f;
														goto IL_3257;
													}
													goto IL_3257;
												case 37:
													num18 = 0.56f;
													num19 = 0.43f;
													num20 = 0.15f;
													goto IL_3257;
												default:
													goto IL_3257;
											}
										}
									}
									else if (type <= 49)
									{
										if (type != 42)
										{
											if (type != 49)
											{
												goto IL_3257;
											}
											num18 = 0f;
											num19 = 0.35f;
											num20 = 0.8f;
											goto IL_3257;
										}
										else
										{
											if (tile.frameX != 0)
											{
												goto IL_3257;
											}
											int num35 = (int)(tile.frameY / 36);
											int num34 = num35;
											switch (num34)
											{
												case 0:
													num18 = 0.7f;
													num19 = 0.65f;
													num20 = 0.55f;
													goto IL_3257;
												case 1:
													num18 = 0.9f;
													num19 = 0.75f;
													num20 = 0.6f;
													goto IL_3257;
												case 2:
													num18 = 0.8f;
													num19 = 0.6f;
													num20 = 0.6f;
													goto IL_3257;
												case 3:
													num18 = 0.65f;
													num19 = 0.5f;
													num20 = 0.2f;
													goto IL_3257;
												case 4:
													num18 = 0.5f;
													num19 = 0.7f;
													num20 = 0.4f;
													goto IL_3257;
												case 5:
													num18 = 0.9f;
													num19 = 0.4f;
													num20 = 0.2f;
													goto IL_3257;
												case 6:
													num18 = 0.7f;
													num19 = 0.75f;
													num20 = 0.3f;
													goto IL_3257;
												case 7:
													{
														float num36 = Main.demonTorch * 0.2f;
														num18 = 0.9f - num36;
														num19 = 0.9f - num36;
														num20 = 0.7f + num36;
														goto IL_3257;
													}
												case 8:
													num18 = 0.75f;
													num19 = 0.6f;
													num20 = 0.3f;
													goto IL_3257;
												case 9:
													num18 = 1f;
													num19 = 0.3f;
													num20 = 0.5f;
													num20 += Main.demonTorch * 0.2f;
													num18 -= Main.demonTorch * 0.1f;
													num19 -= Main.demonTorch * 0.2f;
													goto IL_3257;
												default:
													switch (num34)
													{
														case 28:
															num18 = 0.37f;
															num19 = 0.8f;
															num20 = 1f;
															goto IL_3257;
														case 29:
															num18 = 0f;
															num19 = 0.9f;
															num20 = 1f;
															goto IL_3257;
														case 30:
															num18 = 0.25f;
															num19 = 0.7f;
															num20 = 1f;
															goto IL_3257;
														case 32:
															num18 = 0.5f * Main.demonTorch + 1f * (1f - Main.demonTorch);
															num19 = 0.3f;
															num20 = 1f * Main.demonTorch + 0.5f * (1f - Main.demonTorch);
															goto IL_3257;
														case 35:
															num18 = 0.7f;
															num19 = 0.6f;
															num20 = 0.9f;
															goto IL_3257;
													}
													num18 = 1f;
													num19 = 1f;
													num20 = 1f;
													goto IL_3257;
											}
										}
									}
									else if (type != 61)
									{
										switch (type)
										{
											case 70:
											case 71:
											case 72:
												goto IL_2D37;
											default:
												if (type != 77)
												{
													goto IL_3257;
												}
												num18 = 0.75f;
												num19 = 0.45f;
												num20 = 0.25f;
												goto IL_3257;
										}
									}
									else
									{
										if (tile.frameX == 144)
										{
											float num37 = 1f + (float)(270 - (int)Main.mouseTextColor) / 400f;
											float num38 = 0.8f - (float)(270 - (int)Main.mouseTextColor) / 400f;
											num18 = 0.42f * num38;
											num19 = 0.81f * num37;
											num20 = 0.52f * num38;
											goto IL_3257;
										}
										goto IL_3257;
									}
								}
								else
								{
									if (type <= 133)
									{
										if (type <= 100)
										{
											switch (type)
											{
												case 83:
													if (tile.frameX == 18 && !Main.dayTime)
													{
														num18 = 0.1f;
														num19 = 0.4f;
														num20 = 0.6f;
													}
													if (tile.frameX == 90 && !Main.raining && Main.time > 40500.0)
													{
														num18 = 0.9f;
														num19 = 0.72f;
														num20 = 0.18f;
														goto IL_3257;
													}
													goto IL_3257;
												case 84:
													{
														int num39 = (int)(tile.frameX / 18);
														if (num39 == 2)
														{
															float num40 = (float)(270 - (int)Main.mouseTextColor) / 800f;
															if (num40 > 1f)
															{
																num40 = 1f;
															}
															else if (num40 < 0f)
															{
																num40 = 0f;
															}
															num18 = num40 * 0.7f;
															num19 = num40;
															num20 = num40 * 0.1f;
															goto IL_3257;
														}
														if (num39 == 5)
														{
															float num40 = 0.9f;
															num18 = num40;
															num19 = num40 * 0.8f;
															num20 = num40 * 0.2f;
															goto IL_3257;
														}
														if (num39 == 6)
														{
															float num40 = 0.08f;
															num19 = num40 * 0.8f;
															num20 = num40;
															goto IL_3257;
														}
														goto IL_3257;
													}
												default:
													switch (type)
													{
														case 92:
															if (tile.frameY <= 18 && tile.frameX == 0)
															{
																num18 = 1f;
																num19 = 1f;
																num20 = 1f;
																goto IL_3257;
															}
															goto IL_3257;
														case 93:
															if (tile.frameX == 0)
															{
																switch (tile.frameY / 54)
																{
																	case 1:
																		num18 = 0.95f;
																		num19 = 0.95f;
																		num20 = 0.5f;
																		goto IL_3257;
																	case 2:
																		num18 = 0.85f;
																		num19 = 0.6f;
																		num20 = 1f;
																		goto IL_3257;
																	case 3:
																		num18 = 0.75f;
																		num19 = 1f;
																		num20 = 0.6f;
																		goto IL_3257;
																	case 4:
																	case 5:
																		num18 = 0.75f;
																		num19 = 0.9f;
																		num20 = 1f;
																		goto IL_3257;
																	case 9:
																		num18 = 1f;
																		num19 = 1f;
																		num20 = 0.7f;
																		goto IL_3257;
																	case 13:
																		num18 = 1f;
																		num19 = 1f;
																		num20 = 0.6f;
																		goto IL_3257;
																	case 19:
																		num18 = 0.37f;
																		num19 = 0.8f;
																		num20 = 1f;
																		goto IL_3257;
																	case 20:
																		num18 = 0f;
																		num19 = 0.9f;
																		num20 = 1f;
																		goto IL_3257;
																	case 21:
																		num18 = 0.25f;
																		num19 = 0.7f;
																		num20 = 1f;
																		goto IL_3257;
																	case 23:
																		num18 = 0.5f * Main.demonTorch + 1f * (1f - Main.demonTorch);
																		num19 = 0.3f;
																		num20 = 1f * Main.demonTorch + 0.5f * (1f - Main.demonTorch);
																		goto IL_3257;
																	case 24:
																		num18 = 0.35f;
																		num19 = 0.5f;
																		num20 = 0.3f;
																		goto IL_3257;
																	case 25:
																		num18 = 0.34f;
																		num19 = 0.4f;
																		num20 = 0.31f;
																		goto IL_3257;
																	case 26:
																		num18 = 0.25f;
																		num19 = 0.32f;
																		num20 = 0.5f;
																		goto IL_3257;
																	case 29:
																		num18 = 0.9f;
																		num19 = 0.75f;
																		num20 = 1f;
																		goto IL_3257;
																}
																num18 = 1f;
																num19 = 0.97f;
																num20 = 0.85f;
																goto IL_3257;
															}
															goto IL_3257;
														case 94:
														case 97:
														case 99:
															goto IL_3257;
														case 95:
															if (tile.frameX < 36)
															{
																num18 = 1f;
																num19 = 0.95f;
																num20 = 0.8f;
																goto IL_3257;
															}
															goto IL_3257;
														case 96:
															if (tile.frameX >= 36)
															{
																num18 = 0.5f;
																num19 = 0.35f;
																num20 = 0.1f;
																goto IL_3257;
															}
															goto IL_3257;
														case 98:
															if (tile.frameY == 0)
															{
																num18 = 1f;
																num19 = 0.97f;
																num20 = 0.85f;
																goto IL_3257;
															}
															goto IL_3257;
														case 100:
															break;
														default:
															goto IL_3257;
													}
													break;
											}
										}
										else
										{
											switch (type)
											{
												case 125:
													{
														float num41 = (float)Main.rand.Next(28, 42) * 0.01f;
														num41 += (float)(270 - (int)Main.mouseTextColor) / 800f;
														num19 = (lightingState3.g2 = 0.3f * num41);
														num20 = (lightingState3.b2 = 0.6f * num41);
														goto IL_3257;
													}
												case 126:
													if (tile.frameX < 36)
													{
														num18 = (float)Main.DiscoR / 255f;
														num19 = (float)Main.DiscoG / 255f;
														num20 = (float)Main.DiscoB / 255f;
														goto IL_3257;
													}
													goto IL_3257;
												case 127:
												case 128:
													goto IL_3257;
												case 129:
													switch (tile.frameX / 18 % 3)
													{
														case 0:
															num18 = 0f;
															num19 = 0.05f;
															num20 = 0.25f;
															goto IL_3257;
														case 1:
															num18 = 0.2f;
															num19 = 0f;
															num20 = 0.15f;
															goto IL_3257;
														case 2:
															num18 = 0.1f;
															num19 = 0f;
															num20 = 0.2f;
															goto IL_3257;
														default:
															goto IL_3257;
													}
													break;
												default:
													if (type != 133)
													{
														goto IL_3257;
													}
													goto IL_28AD;
											}
										}
									}
									else if (type <= 149)
									{
										if (type == 140)
										{
											goto IL_28FB;
										}
										if (type != 149)
										{
											goto IL_3257;
										}
										if (tile.frameX <= 36)
										{
											switch (tile.frameX / 18)
											{
												case 0:
													num18 = 0.1f;
													num19 = 0.2f;
													num20 = 0.5f;
													break;
												case 1:
													num18 = 0.5f;
													num19 = 0.1f;
													num20 = 0.1f;
													break;
												case 2:
													num18 = 0.2f;
													num19 = 0.5f;
													num20 = 0.1f;
													break;
											}
											num18 *= (float)Main.rand.Next(970, 1031) * 0.001f;
											num19 *= (float)Main.rand.Next(970, 1031) * 0.001f;
											num20 *= (float)Main.rand.Next(970, 1031) * 0.001f;
											goto IL_3257;
										}
										goto IL_3257;
									}
									else
									{
										if (type == 160)
										{
											num18 = (float)Main.DiscoR / 255f * 0.25f;
											num19 = (float)Main.DiscoG / 255f * 0.25f;
											num20 = (float)Main.DiscoB / 255f * 0.25f;
											goto IL_3257;
										}
										switch (type)
										{
											case 171:
												{
													int num42 = n;
													int num43 = num17;
													if (tile.frameX < 10)
													{
														num42 -= (int)tile.frameX;
														num43 -= (int)tile.frameY;
													}
													switch ((Main.tile[num42, num43].frameY & 15360) >> 10)
													{
														case 1:
															num18 = 0.1f;
															num19 = 0.1f;
															num20 = 0.1f;
															break;
														case 2:
															num18 = 0.2f;
															break;
														case 3:
															num19 = 0.2f;
															break;
														case 4:
															num20 = 0.2f;
															break;
														case 5:
															num18 = 0.125f;
															num19 = 0.125f;
															break;
														case 6:
															num18 = 0.2f;
															num19 = 0.1f;
															break;
														case 7:
															num18 = 0.125f;
															num19 = 0.125f;
															break;
														case 8:
															num18 = 0.08f;
															num19 = 0.175f;
															break;
														case 9:
															num19 = 0.125f;
															num20 = 0.125f;
															break;
														case 10:
															num18 = 0.125f;
															num20 = 0.125f;
															break;
														case 11:
															num18 = 0.1f;
															num19 = 0.1f;
															num20 = 0.2f;
															break;
														default:
															num19 = (num18 = (num20 = 0f));
															break;
													}
													num18 *= 0.5f;
													num19 *= 0.5f;
													num20 *= 0.5f;
													goto IL_3257;
												}
											case 172:
												goto IL_3257;
											case 173:
												break;
											case 174:
												if (tile.frameX == 0)
												{
													num18 = 1f;
													num19 = 0.95f;
													num20 = 0.65f;
													goto IL_3257;
												}
												goto IL_3257;
											default:
												if (type != 184)
												{
													goto IL_3257;
												}
												if (tile.frameX == 110)
												{
													num18 = 0.25f;
													num19 = 0.1f;
													num20 = 0f;
													goto IL_3257;
												}
												goto IL_3257;
										}
									}
									if (tile.frameX < 36)
									{
										int num44 = (int)(tile.frameY / 36);
										int num34 = num44;
										switch (num34)
										{
											case 1:
												num18 = 0.95f;
												num19 = 0.95f;
												num20 = 0.5f;
												goto IL_3257;
											case 2:
											case 4:
											case 5:
											case 7:
											case 8:
											case 10:
											case 12:
											case 14:
											case 15:
											case 16:
											case 17:
											case 18:
												break;
											case 3:
												num18 = 1f;
												num19 = 0.6f;
												num20 = 0.6f;
												goto IL_3257;
											case 6:
											case 9:
												num18 = 0.75f;
												num19 = 0.9f;
												num20 = 1f;
												goto IL_3257;
											case 11:
												num18 = 1f;
												num19 = 1f;
												num20 = 0.7f;
												goto IL_3257;
											case 13:
												num18 = 1f;
												num19 = 1f;
												num20 = 0.6f;
												goto IL_3257;
											case 19:
												num18 = 0.37f;
												num19 = 0.8f;
												num20 = 1f;
												goto IL_3257;
											case 20:
												num18 = 0f;
												num19 = 0.9f;
												num20 = 1f;
												goto IL_3257;
											case 21:
												num18 = 0.25f;
												num19 = 0.7f;
												num20 = 1f;
												goto IL_3257;
											case 22:
												num18 = 0.35f;
												num19 = 0.5f;
												num20 = 0.3f;
												goto IL_3257;
											case 23:
												num18 = 0.34f;
												num19 = 0.4f;
												num20 = 0.31f;
												goto IL_3257;
											case 24:
												num18 = 0.25f;
												num19 = 0.32f;
												num20 = 0.5f;
												goto IL_3257;
											case 25:
												num18 = 0.5f * Main.demonTorch + 1f * (1f - Main.demonTorch);
												num19 = 0.3f;
												num20 = 1f * Main.demonTorch + 0.5f * (1f - Main.demonTorch);
												goto IL_3257;
											default:
												if (num34 == 29)
												{
													num18 = 0.9f;
													num19 = 0.75f;
													num20 = 1f;
													goto IL_3257;
												}
												break;
										}
										num18 = 1f;
										num19 = 0.95f;
										num20 = 0.65f;
										goto IL_3257;
									}
									goto IL_3257;
								}
								IL_28FB:
								num18 = 0.12f;
								num19 = 0.07f;
								num20 = 0.32f;
								goto IL_3257;
							}
							if (type <= 318)
							{
								if (type <= 215)
								{
									if (type <= 204)
									{
										if (type == 190)
										{
											goto IL_2D37;
										}
										if (type != 204)
										{
											goto IL_3257;
										}
									}
									else if (type != 209)
									{
										if (type != 215)
										{
											goto IL_3257;
										}
										if (tile.frameY < 36)
										{
											float num45 = (float)Main.rand.Next(28, 42) * 0.005f;
											num45 += (float)(270 - (int)Main.mouseTextColor) / 700f;
											switch (tile.frameX / 54)
											{
												case 1:
													num18 = 0.7f;
													num19 = 1f;
													num20 = 0.5f;
													break;
												case 2:
													num18 = 0.5f * Main.demonTorch + 1f * (1f - Main.demonTorch);
													num19 = 0.3f;
													num20 = 1f * Main.demonTorch + 0.5f * (1f - Main.demonTorch);
													break;
												case 3:
													num18 = 0.45f;
													num19 = 0.75f;
													num20 = 1f;
													break;
												case 4:
													num18 = 1.15f;
													num19 = 1.15f;
													num20 = 0.5f;
													break;
												case 5:
													num18 = (float)Main.DiscoR / 255f;
													num19 = (float)Main.DiscoG / 255f;
													num20 = (float)Main.DiscoB / 255f;
													break;
												case 6:
													num18 = 0.75f;
													num19 = 1.28249991f;
													num20 = 1.2f;
													break;
												case 7:
													num18 = 0.95f;
													num19 = 0.65f;
													num20 = 1.3f;
													break;
												default:
													num18 = 0.9f;
													num19 = 0.3f;
													num20 = 0.1f;
													break;
											}
											num18 += num45;
											num19 += num45;
											num20 += num45;
											goto IL_3257;
										}
										goto IL_3257;
									}
									else
									{
										if (tile.frameX == 234 || tile.frameX == 252)
										{
											Vector3 vector = PortalHelper.GetPortalColor(Main.myPlayer, 0).ToVector3() * 0.65f;
											num18 = vector.X;
											num19 = vector.Y;
											num20 = vector.Z;
											goto IL_3257;
										}
										if (tile.frameX == 306 || tile.frameX == 324)
										{
											Vector3 vector2 = PortalHelper.GetPortalColor(Main.myPlayer, 1).ToVector3() * 0.65f;
											num18 = vector2.X;
											num19 = vector2.Y;
											num20 = vector2.Z;
											goto IL_3257;
										}
										goto IL_3257;
									}
								}
								else if (type <= 271)
								{
									switch (type)
									{
										case 235:
											if ((double)lightingState3.r2 < 0.6)
											{
												lightingState3.r2 = 0.6f;
											}
											if ((double)lightingState3.g2 < 0.6)
											{
												lightingState3.g2 = 0.6f;
												goto IL_3257;
											}
											goto IL_3257;
										case 236:
											goto IL_3257;
										case 237:
											num18 = 0.1f;
											num19 = 0.1f;
											goto IL_3257;
										case 238:
											if ((double)lightingState3.r2 < 0.5)
											{
												lightingState3.r2 = 0.5f;
											}
											if ((double)lightingState3.b2 < 0.5)
											{
												lightingState3.b2 = 0.5f;
												goto IL_3257;
											}
											goto IL_3257;
										default:
											switch (type)
											{
												case 262:
													num18 = 0.75f;
													num20 = 0.75f;
													goto IL_3257;
												case 263:
													num18 = 0.75f;
													num19 = 0.75f;
													goto IL_3257;
												case 264:
													num20 = 0.75f;
													goto IL_3257;
												case 265:
													num19 = 0.75f;
													goto IL_3257;
												case 266:
													num18 = 0.75f;
													goto IL_3257;
												case 267:
													num18 = 0.75f;
													num19 = 0.75f;
													num20 = 0.75f;
													goto IL_3257;
												case 268:
													num18 = 0.75f;
													num19 = 0.375f;
													goto IL_3257;
												case 269:
													goto IL_3257;
												case 270:
													num18 = 0.73f;
													num19 = 1f;
													num20 = 0.41f;
													goto IL_3257;
												case 271:
													num18 = 0.45f;
													num19 = 0.95f;
													num20 = 1f;
													goto IL_3257;
												default:
													goto IL_3257;
											}
											break;
									}
								}
								else
								{
									if (type == 286)
									{
										num18 = 0.1f;
										num19 = 0.2f;
										num20 = 0.7f;
										goto IL_3257;
									}
									if (type == 302)
									{
										goto IL_28AD;
									}
									switch (type)
									{
										case 316:
										case 317:
										case 318:
											{
												int num46 = n - (int)(tile.frameX / 18);
												int num47 = num17 - (int)(tile.frameY / 18);
												int num48 = num46 / 2 * (num47 / 3);
												num48 %= Main.cageFrames;
												bool flag = Main.jellyfishCageMode[(int)(tile.type - 316), num48] == 2;
												if (tile.type == 316)
												{
													if (flag)
													{
														num18 = 0.2f;
														num19 = 0.3f;
														num20 = 0.8f;
													}
													else
													{
														num18 = 0.1f;
														num19 = 0.2f;
														num20 = 0.5f;
													}
												}
												if (tile.type == 317)
												{
													if (flag)
													{
														num18 = 0.2f;
														num19 = 0.7f;
														num20 = 0.3f;
													}
													else
													{
														num18 = 0.05f;
														num19 = 0.45f;
														num20 = 0.1f;
													}
												}
												if (tile.type != 318)
												{
													goto IL_3257;
												}
												if (flag)
												{
													num18 = 0.7f;
													num19 = 0.2f;
													num20 = 0.5f;
													goto IL_3257;
												}
												num18 = 0.4f;
												num19 = 0.1f;
												num20 = 0.25f;
												goto IL_3257;
											}
										default:
											goto IL_3257;
									}
								}
							}
							else if (type <= 381)
							{
								if (type <= 350)
								{
									if (type == 327)
									{
										float num49 = 0.5f;
										num49 += (float)(270 - (int)Main.mouseTextColor) / 1500f;
										num49 += (float)Main.rand.Next(0, 50) * 0.0005f;
										num18 = 1f * num49;
										num19 = 0.5f * num49;
										num20 = 0.1f * num49;
										goto IL_3257;
									}
									switch (type)
									{
										case 336:
											num18 = 0.85f;
											num19 = 0.5f;
											num20 = 0.3f;
											goto IL_3257;
										case 337:
										case 338:
										case 339:
										case 345:
										case 346:
											goto IL_3257;
										case 340:
											num18 = 0.45f;
											num19 = 1f;
											num20 = 0.45f;
											goto IL_3257;
										case 341:
											num18 = 0.4f * Main.demonTorch + 0.6f * (1f - Main.demonTorch);
											num19 = 0.35f;
											num20 = 1f * Main.demonTorch + 0.6f * (1f - Main.demonTorch);
											goto IL_3257;
										case 342:
											num18 = 0.5f;
											num19 = 0.5f;
											num20 = 1.1f;
											goto IL_3257;
										case 343:
											num18 = 0.85f;
											num19 = 0.85f;
											num20 = 0.3f;
											goto IL_3257;
										case 344:
											num18 = 0.6f;
											num19 = 1.026f;
											num20 = 0.960000038f;
											goto IL_3257;
										case 347:
											break;
										case 348:
										case 349:
											goto IL_2D37;
										case 350:
											{
												double num50 = Main.time * 0.08;
												float num51 = (float)(-(float)Math.Cos(((int)(num50 / 6.283) % 3 == 1) ? num50 : 0.0) * 0.1 + 0.1);
												num18 = num51;
												num19 = num51;
												num20 = num51;
												goto IL_3257;
											}
										default:
											goto IL_3257;
									}
								}
								else
								{
									switch (type)
									{
										case 370:
											num18 = 0.32f;
											num19 = 0.16f;
											num20 = 0.12f;
											goto IL_3257;
										case 371:
											goto IL_3257;
										case 372:
											if (tile.frameX == 0)
											{
												num18 = 0.9f;
												num19 = 0.1f;
												num20 = 0.75f;
												goto IL_3257;
											}
											goto IL_3257;
										default:
											if (type != 381)
											{
												goto IL_3257;
											}
											num18 = 0.25f;
											num19 = 0.1f;
											num20 = 0f;
											goto IL_3257;
									}
								}
							}
							else if (type <= 405)
							{
								switch (type)
								{
									case 390:
										num18 = 0.4f;
										num19 = 0.2f;
										num20 = 0.1f;
										goto IL_3257;
									case 391:
										num18 = 0.3f;
										num19 = 0.1f;
										num20 = 0.25f;
										goto IL_3257;
									default:
										if (type != 405)
										{
											goto IL_3257;
										}
										if (tile.frameX < 54)
										{
											float num52 = (float)Main.rand.Next(28, 42) * 0.005f;
											num52 += (float)(270 - (int)Main.mouseTextColor) / 700f;
											switch (tile.frameX / 54)
											{
												case 1:
													num18 = 0.7f;
													num19 = 1f;
													num20 = 0.5f;
													break;
												case 2:
													num18 = 0.5f * Main.demonTorch + 1f * (1f - Main.demonTorch);
													num19 = 0.3f;
													num20 = 1f * Main.demonTorch + 0.5f * (1f - Main.demonTorch);
													break;
												case 3:
													num18 = 0.45f;
													num19 = 0.75f;
													num20 = 1f;
													break;
												case 4:
													num18 = 1.15f;
													num19 = 1.15f;
													num20 = 0.5f;
													break;
												case 5:
													num18 = (float)Main.DiscoR / 255f;
													num19 = (float)Main.DiscoG / 255f;
													num20 = (float)Main.DiscoB / 255f;
													break;
												default:
													num18 = 0.9f;
													num19 = 0.3f;
													num20 = 0.1f;
													break;
											}
											num18 += num52;
											num19 += num52;
											num20 += num52;
											goto IL_3257;
										}
										goto IL_3257;
								}
							}
							else
							{
								switch (type)
								{
									case 415:
										num18 = 0.7f;
										num19 = 0.5f;
										num20 = 0.1f;
										goto IL_3257;
									case 416:
										num18 = 0f;
										num19 = 0.6f;
										num20 = 0.7f;
										goto IL_3257;
									case 417:
										num18 = 0.6f;
										num19 = 0.2f;
										num20 = 0.6f;
										goto IL_3257;
									case 418:
										num18 = 0.6f;
										num19 = 0.6f;
										num20 = 0.9f;
										goto IL_3257;
									default:
										if (type != 429)
										{
											if (type == 463)
											{
												num18 = 0.2f;
												num19 = 0.4f;
												num20 = 0.8f;
												goto IL_3257;
											}
											goto IL_3257;
										}
										else
										{
											int num53 = (int)(tile.frameX / 18);
											bool flag2 = num53 % 2 >= 1;
											bool flag3 = num53 % 4 >= 2;
											bool flag4 = num53 % 8 >= 4;
											bool flag5 = num53 % 16 >= 8;
											if (flag2)
											{
												num18 += 0.5f;
											}
											if (flag3)
											{
												num19 += 0.5f;
											}
											if (flag4)
											{
												num20 += 0.5f;
											}
											if (flag5)
											{
												num18 += 0.2f;
												num19 += 0.2f;
												goto IL_3257;
											}
											goto IL_3257;
										}
										break;
								}
							}
							num18 = 0.35f;
							goto IL_3257;
							IL_28AD:
							num18 = 0.83f;
							num19 = 0.6f;
							num20 = 0.5f;
							goto IL_3257;
							IL_2D37:
							if (tile.type != 349 || tile.frameX >= 36)
							{
								float num54 = (float)Main.rand.Next(28, 42) * 0.005f;
								num54 += (float)(270 - (int)Main.mouseTextColor) / 1000f;
								num18 = 0.1f;
								num19 = 0.2f + num54 / 2f;
								num20 = 0.7f + num54;
							}
						}
					}
					IL_3257:
					TileLoader.ModifyLight(n, num17, tile.type, ref num18, ref num19, ref num20);
					if (Lighting.RGB)
					{
						if (lightingState3.r2 < num18)
						{
							lightingState3.r2 = num18;
						}
						//patch file: num18, num19
						if (lightingState3.g2 < num19)
						{
							lightingState3.g2 = num19;
						}
						//patch file: num19, num20
						if (lightingState3.b2 < num20)
						{
							lightingState3.b2 = num20;
						}
					}
					else
					{
						float num55 = (num18 + num19 + num20) / 3f;
						if (lightingState3.r2 < num55)
						{
							lightingState3.r2 = num55;
						}
					}
					if (tile.lava() && tile.liquid > 0)
					{
						if (Lighting.RGB)
						{
							float num56 = (float)(tile.liquid / 255) * 0.41f + 0.14f;
							num56 = 0.55f;
							num56 += (float)(270 - (int)Main.mouseTextColor) / 900f;
							if (lightingState3.r2 < num56)
							{
								lightingState3.r2 = num56;
							}
							if (lightingState3.g2 < num56)
							{
								lightingState3.g2 = num56 * 0.6f;
							}
							if (lightingState3.b2 < num56)
							{
								lightingState3.b2 = num56 * 0.2f;
							}
						}
						else
						{
							float num57 = (float)(tile.liquid / 255) * 0.38f + 0.08f;
							num57 += (float)(270 - (int)Main.mouseTextColor) / 2000f;
							if (lightingState3.r2 < num57)
							{
								lightingState3.r2 = num57;
							}
						}
					}
					else if (tile.liquid > 128)
					{
						lightingState3.wetLight = true;
						if (tile.honey())
						{
							lightingState3.honeyLight = true;
						}
					}
					if (lightingState3.r2 > 0f || (Lighting.RGB && (lightingState3.g2 > 0f || lightingState3.b2 > 0f)))
					{
						int num58 = n - Lighting.firstToLightX;
						int num59 = num17 - Lighting.firstToLightY;
						if (Lighting.minX > num58)
						{
							Lighting.minX = num58;
						}
						if (Lighting.maxX < num58 + 1)
						{
							Lighting.maxX = num58 + 1;
						}
						if (Lighting.minY > num59)
						{
							Lighting.minY = num59;
						}
						if (Lighting.maxY < num59 + 1)
						{
							Lighting.maxY = num59 + 1;
						}
					}
				}
			}
			foreach (KeyValuePair<Point16, Lighting.ColorTriplet> current in Lighting.tempLights)
			{
				int num60 = (int)current.Key.X - Lighting.firstTileX + Lighting.offScreenTiles;
				int num61 = (int)current.Key.Y - Lighting.firstTileY + Lighting.offScreenTiles;
				if (num60 >= 0 && num60 < Main.screenWidth / 16 + Lighting.offScreenTiles * 2 + 10 && num61 >= 0 && num61 < Main.screenHeight / 16 + Lighting.offScreenTiles * 2 + 10)
				{
					Lighting.LightingState lightingState4 = Lighting.states[num60][num61];
					if (lightingState4.r2 < current.Value.r)
					{
						lightingState4.r2 = current.Value.r;
					}
					if (lightingState4.g2 < current.Value.g)
					{
						lightingState4.g2 = current.Value.g;
					}
					if (lightingState4.b2 < current.Value.b)
					{
						lightingState4.b2 = current.Value.b;
					}
					if (num60 < Lighting.minX)
					{
						Lighting.minX = num60;
					}
					if (num60 > Lighting.maxX)
					{
						Lighting.maxX = num60;
					}
					if (num61 < Lighting.minY)
					{
						Lighting.minY = num61;
					}
					if (num61 > Lighting.maxY)
					{
						Lighting.maxY = num61;
					}
				}
			}
			if (!Main.gamePaused)
			{
				Lighting.tempLights.Clear();
			}
			if (screenTileCounts[27] > 0)
			{
				Main.sunflower = true;
			}
			Main.holyTiles = screenTileCounts[109] + screenTileCounts[110] + screenTileCounts[113] + screenTileCounts[117] + screenTileCounts[116] + screenTileCounts[164] + screenTileCounts[403] + screenTileCounts[402];
			Main.evilTiles = screenTileCounts[23] + screenTileCounts[24] + screenTileCounts[25] + screenTileCounts[32] + screenTileCounts[112] + screenTileCounts[163] + screenTileCounts[400] + screenTileCounts[398] + -5 * screenTileCounts[27];
			Main.bloodTiles = screenTileCounts[199] + screenTileCounts[203] + screenTileCounts[200] + screenTileCounts[401] + screenTileCounts[399] + screenTileCounts[234] + screenTileCounts[352] - 5 * screenTileCounts[27];
			Main.snowTiles = screenTileCounts[147] + screenTileCounts[148] + screenTileCounts[161] + screenTileCounts[162] + screenTileCounts[164] + screenTileCounts[163] + screenTileCounts[200];
			Main.jungleTiles = screenTileCounts[60] + screenTileCounts[61] + screenTileCounts[62] + screenTileCounts[74] + screenTileCounts[226];
			Main.shroomTiles = screenTileCounts[70] + screenTileCounts[71] + screenTileCounts[72];
			Main.meteorTiles = screenTileCounts[37];
			Main.dungeonTiles = screenTileCounts[41] + screenTileCounts[43] + screenTileCounts[44];
			Main.sandTiles = screenTileCounts[53] + screenTileCounts[112] + screenTileCounts[116] + screenTileCounts[234] + screenTileCounts[397] + screenTileCounts[398] + screenTileCounts[402] + screenTileCounts[399] + screenTileCounts[396] + screenTileCounts[400] + screenTileCounts[403] + screenTileCounts[401];
			Main.waterCandles = screenTileCounts[49];
			Main.peaceCandles = screenTileCounts[372];
			Main.partyMonoliths = screenTileCounts[455];
			if (Main.player[Main.myPlayer].accOreFinder)
			{
				Main.player[Main.myPlayer].bestOre = -1;
				for (int num62 = 0; num62 < Main.tileValue.Length; num62++)
				{
					if (screenTileCounts[num62] > 0 && Main.tileValue[num62] > 0 && (Main.player[Main.myPlayer].bestOre < 0 || Main.tileValue[num62] > Main.tileValue[Main.player[Main.myPlayer].bestOre]))
					{
						Main.player[Main.myPlayer].bestOre = num62;
					}
				}
			}
			WorldHooks.TileCountsAvailable(screenTileCounts);
			if (Main.holyTiles < 0)
			{
				Main.holyTiles = 0;
			}
			if (Main.evilTiles < 0)
			{
				Main.evilTiles = 0;
			}
			if (Main.bloodTiles < 0)
			{
				Main.bloodTiles = 0;
			}
			int holyTiles = Main.holyTiles;
			Main.holyTiles -= Main.evilTiles;
			Main.holyTiles -= Main.bloodTiles;
			Main.evilTiles -= holyTiles;
			Main.bloodTiles -= holyTiles;
			if (Main.holyTiles < 0)
			{
				Main.holyTiles = 0;
			}
			if (Main.evilTiles < 0)
			{
				Main.evilTiles = 0;
			}
			if (Main.bloodTiles < 0)
			{
				Main.bloodTiles = 0;
			}
			Lighting.minX += Lighting.firstToLightX;
			Lighting.maxX += Lighting.firstToLightX;
			Lighting.minY += Lighting.firstToLightY;
			Lighting.maxY += Lighting.firstToLightY;
			Lighting.minX7 = Lighting.minX;
			Lighting.maxX7 = Lighting.maxX;
			Lighting.minY7 = Lighting.minY;
			Lighting.maxY7 = Lighting.maxY;
			Lighting.firstTileX7 = Lighting.firstTileX;
			Lighting.lastTileX7 = Lighting.lastTileX;
			Lighting.lastTileY7 = Lighting.lastTileY;
			Lighting.firstTileY7 = Lighting.firstTileY;
			Lighting.firstToLightX7 = Lighting.firstToLightX;
			Lighting.lastToLightX7 = Lighting.lastToLightX;
			Lighting.firstToLightY7 = Lighting.firstToLightY;
			Lighting.lastToLightY7 = Lighting.lastToLightY;
			Lighting.firstToLightX27 = Lighting.firstTileX - Lighting.offScreenTiles2;
			Lighting.firstToLightY27 = Lighting.firstTileY - Lighting.offScreenTiles2;
			Lighting.lastToLightX27 = Lighting.lastTileX + Lighting.offScreenTiles2;
			Lighting.lastToLightY27 = Lighting.lastTileY + Lighting.offScreenTiles2;
			if (Lighting.firstToLightX27 < 0)
			{
				Lighting.firstToLightX27 = 0;
			}
			if (Lighting.lastToLightX27 >= Main.maxTilesX)
			{
				Lighting.lastToLightX27 = Main.maxTilesX - 1;
			}
			if (Lighting.firstToLightY27 < 0)
			{
				Lighting.firstToLightY27 = 0;
			}
			if (Lighting.lastToLightY27 >= Main.maxTilesY)
			{
				Lighting.lastToLightY27 = Main.maxTilesY - 1;
			}
			Lighting.scrX = (int)Main.screenPosition.X / 16;
			Lighting.scrY = (int)Main.screenPosition.Y / 16;
			Main.renderCount = 0;
			TimeLogger.LightingTime(0, stopwatch.Elapsed.TotalMilliseconds);
			Lighting.doColors();
		}

		public static void doColors()
		{
			if (Lighting.lightMode < 2)
			{
				Lighting.blueWave += (float)Lighting.blueDir * 0.0001f;
				if (Lighting.blueWave > 1f)
				{
					Lighting.blueWave = 1f;
					Lighting.blueDir = -1;
				}
				else if (Lighting.blueWave < 0.97f)
				{
					Lighting.blueWave = 0.97f;
					Lighting.blueDir = 1;
				}
				if (Lighting.RGB)
				{
					Lighting.negLight = 0.91f;
					Lighting.negLight2 = 0.56f;
					Lighting.honeyLightG = 0.7f * Lighting.negLight * Lighting.blueWave;
					Lighting.honeyLightR = 0.75f * Lighting.negLight * Lighting.blueWave;
					Lighting.honeyLightB = 0.6f * Lighting.negLight * Lighting.blueWave;
					switch (Main.waterStyle)
					{
						case 0:
						case 1:
						case 7:
						case 8:
							Lighting.wetLightG = 0.96f * Lighting.negLight * Lighting.blueWave;
							Lighting.wetLightR = 0.88f * Lighting.negLight * Lighting.blueWave;
							Lighting.wetLightB = 1.015f * Lighting.negLight * Lighting.blueWave;
							break;
						case 2:
							Lighting.wetLightG = 0.85f * Lighting.negLight * Lighting.blueWave;
							Lighting.wetLightR = 0.94f * Lighting.negLight * Lighting.blueWave;
							Lighting.wetLightB = 1.01f * Lighting.negLight * Lighting.blueWave;
							break;
						case 3:
							Lighting.wetLightG = 0.95f * Lighting.negLight * Lighting.blueWave;
							Lighting.wetLightR = 0.84f * Lighting.negLight * Lighting.blueWave;
							Lighting.wetLightB = 1.015f * Lighting.negLight * Lighting.blueWave;
							break;
						case 4:
							Lighting.wetLightG = 0.86f * Lighting.negLight * Lighting.blueWave;
							Lighting.wetLightR = 0.9f * Lighting.negLight * Lighting.blueWave;
							Lighting.wetLightB = 1.01f * Lighting.negLight * Lighting.blueWave;
							break;
						case 5:
							Lighting.wetLightG = 0.99f * Lighting.negLight * Lighting.blueWave;
							Lighting.wetLightR = 0.84f * Lighting.negLight * Lighting.blueWave;
							Lighting.wetLightB = 1.01f * Lighting.negLight * Lighting.blueWave;
							break;
						case 6:
							Lighting.wetLightG = 0.98f * Lighting.negLight * Lighting.blueWave;
							Lighting.wetLightR = 0.95f * Lighting.negLight * Lighting.blueWave;
							Lighting.wetLightB = 0.85f * Lighting.negLight * Lighting.blueWave;
							break;
						case 9:
							Lighting.wetLightG = 0.88f * Lighting.negLight * Lighting.blueWave;
							Lighting.wetLightR = 1f * Lighting.negLight * Lighting.blueWave;
							Lighting.wetLightB = 0.84f * Lighting.negLight * Lighting.blueWave;
							break;
						case 10:
							Lighting.wetLightG = 1f * Lighting.negLight * Lighting.blueWave;
							Lighting.wetLightR = 0.83f * Lighting.negLight * Lighting.blueWave;
							Lighting.wetLightB = 1f * Lighting.negLight * Lighting.blueWave;
							break;
						default:
							Lighting.wetLightG = 0f;
							Lighting.wetLightR = 0f;
							Lighting.wetLightB = 0f;
							break;
					}
					WaterStyleLoader.LightColorMultiplier(Main.waterStyle, ref Lighting.wetLightR, ref Lighting.wetLightG, ref Lighting.wetLightB);
				}
				else
				{
					Lighting.negLight = 0.9f;
					Lighting.negLight2 = 0.54f;
					Lighting.wetLightR = 0.95f * Lighting.negLight * Lighting.blueWave;
				}
				if (Main.player[Main.myPlayer].nightVision)
				{
					Lighting.negLight *= 1.03f;
					Lighting.negLight2 *= 1.03f;
				}
				if (Main.player[Main.myPlayer].blind)
				{
					Lighting.negLight *= 0.95f;
					Lighting.negLight2 *= 0.95f;
				}
				if (Main.player[Main.myPlayer].blackout)
				{
					Lighting.negLight *= 0.85f;
					Lighting.negLight2 *= 0.85f;
				}
				if (Main.player[Main.myPlayer].headcovered)
				{
					Lighting.negLight *= 0.85f;
					Lighting.negLight2 *= 0.85f;
				}
			}
			else
			{
				Lighting.negLight = 0.04f;
				Lighting.negLight2 = 0.16f;
				if (Main.player[Main.myPlayer].nightVision)
				{
					Lighting.negLight -= 0.013f;
					Lighting.negLight2 -= 0.04f;
				}
				if (Main.player[Main.myPlayer].blind)
				{
					Lighting.negLight += 0.03f;
					Lighting.negLight2 += 0.06f;
				}
				if (Main.player[Main.myPlayer].blackout)
				{
					Lighting.negLight += 0.09f;
					Lighting.negLight2 += 0.18f;
				}
				if (Main.player[Main.myPlayer].headcovered)
				{
					Lighting.negLight += 0.09f;
					Lighting.negLight2 += 0.18f;
				}
				Lighting.wetLightR = Lighting.negLight * 1.2f;
				Lighting.wetLightG = Lighting.negLight * 1.1f;
			}
			int num;
			int num2;
			switch (Main.renderCount)
			{
				case 0:
					num = 0;
					num2 = 1;
					break;
				case 1:
					num = 1;
					num2 = 3;
					break;
				case 2:
					num = 3;
					num2 = 4;
					break;
				default:
					num = 0;
					num2 = 0;
					break;
			}
			if (Lighting.LightingThreads < 0)
			{
				Lighting.LightingThreads = 0;
			}
			if (Lighting.LightingThreads >= Environment.ProcessorCount)
			{
				Lighting.LightingThreads = Environment.ProcessorCount - 1;
			}
			int num3 = Lighting.LightingThreads;
			if (num3 > 0)
			{
				num3++;
			}
			Stopwatch stopwatch = new Stopwatch();
			for (int i = num; i < num2; i++)
			{
				stopwatch.Restart();
				switch (i)
				{
					case 0:
						Lighting.swipe.innerLoop1Start = Lighting.minY7 - Lighting.firstToLightY7;
						Lighting.swipe.innerLoop1End = Lighting.lastToLightY27 + Lighting.maxRenderCount - Lighting.firstToLightY7;
						Lighting.swipe.innerLoop2Start = Lighting.maxY7 - Lighting.firstToLightY;
						Lighting.swipe.innerLoop2End = Lighting.firstTileY7 - Lighting.maxRenderCount - Lighting.firstToLightY7;
						Lighting.swipe.outerLoopStart = Lighting.minX7 - Lighting.firstToLightX7;
						Lighting.swipe.outerLoopEnd = Lighting.maxX7 - Lighting.firstToLightX7;
						Lighting.swipe.jaggedArray = Lighting.states;
						break;
					case 1:
						Lighting.swipe.innerLoop1Start = Lighting.minX7 - Lighting.firstToLightX7;
						Lighting.swipe.innerLoop1End = Lighting.lastTileX7 + Lighting.maxRenderCount - Lighting.firstToLightX7;
						Lighting.swipe.innerLoop2Start = Lighting.maxX7 - Lighting.firstToLightX7;
						Lighting.swipe.innerLoop2End = Lighting.firstTileX7 - Lighting.maxRenderCount - Lighting.firstToLightX7;
						Lighting.swipe.outerLoopStart = Lighting.firstToLightY7 - Lighting.firstToLightY7;
						Lighting.swipe.outerLoopEnd = Lighting.lastToLightY7 - Lighting.firstToLightY7;
						Lighting.swipe.jaggedArray = Lighting.axisFlipStates;
						break;
					case 2:
						Lighting.swipe.innerLoop1Start = Lighting.firstToLightY27 - Lighting.firstToLightY7;
						Lighting.swipe.innerLoop1End = Lighting.lastTileY7 + Lighting.maxRenderCount - Lighting.firstToLightY7;
						Lighting.swipe.innerLoop2Start = Lighting.lastToLightY27 - Lighting.firstToLightY;
						Lighting.swipe.innerLoop2End = Lighting.firstTileY7 - Lighting.maxRenderCount - Lighting.firstToLightY7;
						Lighting.swipe.outerLoopStart = Lighting.firstToLightX27 - Lighting.firstToLightX7;
						Lighting.swipe.outerLoopEnd = Lighting.lastToLightX27 - Lighting.firstToLightX7;
						Lighting.swipe.jaggedArray = Lighting.states;
						break;
					case 3:
						Lighting.swipe.innerLoop1Start = Lighting.firstToLightX27 - Lighting.firstToLightX7;
						Lighting.swipe.innerLoop1End = Lighting.lastTileX7 + Lighting.maxRenderCount - Lighting.firstToLightX7;
						Lighting.swipe.innerLoop2Start = Lighting.lastToLightX27 - Lighting.firstToLightX7;
						Lighting.swipe.innerLoop2End = Lighting.firstTileX7 - Lighting.maxRenderCount - Lighting.firstToLightX7;
						Lighting.swipe.outerLoopStart = Lighting.firstToLightY27 - Lighting.firstToLightY7;
						Lighting.swipe.outerLoopEnd = Lighting.lastToLightY27 - Lighting.firstToLightY7;
						Lighting.swipe.jaggedArray = Lighting.axisFlipStates;
						break;
				}
				if (Lighting.swipe.innerLoop1Start > Lighting.swipe.innerLoop1End)
				{
					Lighting.swipe.innerLoop1Start = Lighting.swipe.innerLoop1End;
				}
				if (Lighting.swipe.innerLoop2Start < Lighting.swipe.innerLoop2End)
				{
					Lighting.swipe.innerLoop2Start = Lighting.swipe.innerLoop2End;
				}
				if (Lighting.swipe.outerLoopStart > Lighting.swipe.outerLoopEnd)
				{
					Lighting.swipe.outerLoopStart = Lighting.swipe.outerLoopEnd;
				}
				switch (Lighting.lightMode)
				{
					case 0:
						Lighting.swipe.function = new Action<Lighting.LightingSwipeData>(Lighting.doColors_Mode0_Swipe);
						break;
					case 1:
						Lighting.swipe.function = new Action<Lighting.LightingSwipeData>(Lighting.doColors_Mode1_Swipe);
						break;
					case 2:
						Lighting.swipe.function = new Action<Lighting.LightingSwipeData>(Lighting.doColors_Mode2_Swipe);
						break;
					case 3:
						Lighting.swipe.function = new Action<Lighting.LightingSwipeData>(Lighting.doColors_Mode3_Swipe);
						break;
					default:
						Lighting.swipe.function = null;
						break;
				}
				if (num3 == 0)
				{
					Lighting.swipe.function(Lighting.swipe);
				}
				else
				{
					int num4 = Lighting.swipe.outerLoopEnd - Lighting.swipe.outerLoopStart;
					int num5 = num4 / num3;
					int num6 = num4 % num3;
					int num7 = Lighting.swipe.outerLoopStart;
					Lighting.countdown.Reset(num3);
					for (int j = 0; j < num3; j++)
					{
						Lighting.LightingSwipeData lightingSwipeData = Lighting.threadSwipes[j];
						lightingSwipeData.CopyFrom(Lighting.swipe);
						lightingSwipeData.outerLoopStart = num7;
						num7 += num5;
						if (num6 > 0)
						{
							num7++;
							num6--;
						}
						lightingSwipeData.outerLoopEnd = num7;
						ThreadPool.QueueUserWorkItem(new WaitCallback(Lighting.callback_LightingSwipe), lightingSwipeData);
					}
					Lighting.countdown.Wait();
				}
				TimeLogger.LightingTime(i + 1, stopwatch.Elapsed.TotalMilliseconds);
			}
		}

		private static void callback_LightingSwipe(object obj)
		{
			Lighting.LightingSwipeData lightingSwipeData = obj as Lighting.LightingSwipeData;
			try
			{
				lightingSwipeData.function(lightingSwipeData);
			}
			catch
			{
			}
			Lighting.countdown.Signal();
		}

		private static void doColors_Mode0_Swipe(Lighting.LightingSwipeData swipeData)
		{
			try
			{
				bool flag = true;
				while (true)
				{
					int num;
					int value;
					int num2;
					if (flag)
					{
						num = 1;
						value = swipeData.innerLoop1Start;
						num2 = swipeData.innerLoop1End;
					}
					else
					{
						num = -1;
						value = swipeData.innerLoop2Start;
						num2 = swipeData.innerLoop2End;
					}
					int outerLoopStart = swipeData.outerLoopStart;
					int outerLoopEnd = swipeData.outerLoopEnd;
					int num3 = Utils.Clamp<int>(outerLoopStart, 0, swipeData.jaggedArray.Length - 1);
					for (int i = num3; i < outerLoopEnd; i++)
					{
						Lighting.LightingState[] array = swipeData.jaggedArray[i];
						float num4 = 0f;
						float num5 = 0f;
						float num6 = 0f;
						int num7 = Utils.Clamp<int>(value, 0, array.Length - 1);
						int num8 = num7;
						while (num8 != num2)
						{
							Lighting.LightingState lightingState = array[num8];
							Lighting.LightingState lightingState2 = array[num8 + num];
							bool flag3;
							bool flag2 = flag3 = false;
							if (lightingState.r2 > num4)
							{
								num4 = lightingState.r2;
							}
							else if ((double)num4 <= 0.0185)
							{
								flag3 = true;
							}
							else if (lightingState.r2 < num4)
							{
								lightingState.r2 = num4;
							}
							if (!flag3 && lightingState2.r2 <= num4)
							{
								if (lightingState.stopLight)
								{
									num4 *= Lighting.negLight2;
								}
								else if (lightingState.wetLight)
								{
									if (lightingState.honeyLight)
									{
										num4 *= Lighting.honeyLightR * (float)swipeData.rand.Next(98, 100) * 0.01f;
									}
									else
									{
										num4 *= Lighting.wetLightR * (float)swipeData.rand.Next(98, 100) * 0.01f;
									}
								}
								else
								{
									num4 *= Lighting.negLight;
								}
							}
							if (lightingState.g2 > num5)
							{
								num5 = lightingState.g2;
							}
							else if ((double)num5 <= 0.0185)
							{
								flag2 = true;
							}
							else
							{
								lightingState.g2 = num5;
							}
							if (!flag2 && lightingState2.g2 <= num5)
							{
								if (lightingState.stopLight)
								{
									num5 *= Lighting.negLight2;
								}
								else if (lightingState.wetLight)
								{
									if (lightingState.honeyLight)
									{
										num5 *= Lighting.honeyLightG * (float)swipeData.rand.Next(97, 100) * 0.01f;
									}
									else
									{
										num5 *= Lighting.wetLightG * (float)swipeData.rand.Next(97, 100) * 0.01f;
									}
								}
								else
								{
									num5 *= Lighting.negLight;
								}
							}
							if (lightingState.b2 > num6)
							{
								num6 = lightingState.b2;
								goto IL_253;
							}
							if ((double)num6 > 0.0185)
							{
								lightingState.b2 = num6;
								goto IL_253;
							}
							IL_2D5:
							num8 += num;
							continue;
							IL_253:
							if (lightingState2.b2 >= num6)
							{
								goto IL_2D5;
							}
							if (lightingState.stopLight)
							{
								num6 *= Lighting.negLight2;
								goto IL_2D5;
							}
							if (!lightingState.wetLight)
							{
								num6 *= Lighting.negLight;
								goto IL_2D5;
							}
							if (lightingState.honeyLight)
							{
								num6 *= Lighting.honeyLightB * (float)swipeData.rand.Next(97, 100) * 0.01f;
								goto IL_2D5;
							}
							num6 *= Lighting.wetLightB * (float)swipeData.rand.Next(97, 100) * 0.01f;
							goto IL_2D5;
						}
					}
					if (!flag)
					{
						break;
					}
					flag = false;
				}
			}
			catch
			{
			}
		}

		private static void doColors_Mode1_Swipe(Lighting.LightingSwipeData swipeData)
		{
			try
			{
				bool flag = true;
				while (true)
				{
					int num;
					int num2;
					int num3;
					if (flag)
					{
						num = 1;
						num2 = swipeData.innerLoop1Start;
						num3 = swipeData.innerLoop1End;
					}
					else
					{
						num = -1;
						num2 = swipeData.innerLoop2Start;
						num3 = swipeData.innerLoop2End;
					}
					int outerLoopStart = swipeData.outerLoopStart;
					int outerLoopEnd = swipeData.outerLoopEnd;
					for (int i = outerLoopStart; i < outerLoopEnd; i++)
					{
						Lighting.LightingState[] array = swipeData.jaggedArray[i];
						float num4 = 0f;
						int num5 = num2;
						while (num5 != num3)
						{
							Lighting.LightingState lightingState = array[num5];
							if (lightingState.r2 > num4)
							{
								num4 = lightingState.r2;
								goto IL_9C;
							}
							if ((double)num4 > 0.0185)
							{
								if (lightingState.r2 < num4)
								{
									lightingState.r2 = num4;
									goto IL_9C;
								}
								goto IL_9C;
							}
							IL_123:
							num5 += num;
							continue;
							IL_9C:
							if (array[num5 + num].r2 > num4)
							{
								goto IL_123;
							}
							if (lightingState.stopLight)
							{
								num4 *= Lighting.negLight2;
								goto IL_123;
							}
							if (!lightingState.wetLight)
							{
								num4 *= Lighting.negLight;
								goto IL_123;
							}
							if (lightingState.honeyLight)
							{
								num4 *= Lighting.honeyLightR * (float)swipeData.rand.Next(98, 100) * 0.01f;
								goto IL_123;
							}
							num4 *= Lighting.wetLightR * (float)swipeData.rand.Next(98, 100) * 0.01f;
							goto IL_123;
						}
					}
					if (!flag)
					{
						break;
					}
					flag = false;
				}
			}
			catch
			{
			}
		}

		private static void doColors_Mode2_Swipe(Lighting.LightingSwipeData swipeData)
		{
			try
			{
				bool flag = true;
				while (true)
				{
					int num;
					int num2;
					int num3;
					if (flag)
					{
						num = 1;
						num2 = swipeData.innerLoop1Start;
						num3 = swipeData.innerLoop1End;
					}
					else
					{
						num = -1;
						num2 = swipeData.innerLoop2Start;
						num3 = swipeData.innerLoop2End;
					}
					int outerLoopStart = swipeData.outerLoopStart;
					int outerLoopEnd = swipeData.outerLoopEnd;
					for (int i = outerLoopStart; i < outerLoopEnd; i++)
					{
						Lighting.LightingState[] array = swipeData.jaggedArray[i];
						float num4 = 0f;
						int num5 = num2;
						while (num5 != num3)
						{
							Lighting.LightingState lightingState = array[num5];
							if (lightingState.r2 > num4)
							{
								num4 = lightingState.r2;
								goto IL_86;
							}
							if (num4 > 0f)
							{
								lightingState.r2 = num4;
								goto IL_86;
							}
							IL_BA:
							num5 += num;
							continue;
							IL_86:
							if (lightingState.stopLight)
							{
								num4 -= Lighting.negLight2;
								goto IL_BA;
							}
							if (lightingState.wetLight)
							{
								num4 -= Lighting.wetLightR;
								goto IL_BA;
							}
							num4 -= Lighting.negLight;
							goto IL_BA;
						}
					}
					if (!flag)
					{
						break;
					}
					flag = false;
				}
			}
			catch
			{
			}
		}

		private static void doColors_Mode3_Swipe(Lighting.LightingSwipeData swipeData)
		{
			try
			{
				bool flag = true;
				while (true)
				{
					int num;
					int num2;
					int num3;
					if (flag)
					{
						num = 1;
						num2 = swipeData.innerLoop1Start;
						num3 = swipeData.innerLoop1End;
					}
					else
					{
						num = -1;
						num2 = swipeData.innerLoop2Start;
						num3 = swipeData.innerLoop2End;
					}
					int outerLoopStart = swipeData.outerLoopStart;
					int outerLoopEnd = swipeData.outerLoopEnd;
					for (int i = outerLoopStart; i < outerLoopEnd; i++)
					{
						Lighting.LightingState[] array = swipeData.jaggedArray[i];
						float num4 = 0f;
						float num5 = 0f;
						float num6 = 0f;
						int num7 = num2;
						while (num7 != num3)
						{
							Lighting.LightingState lightingState = array[num7];
							bool flag3;
							bool flag2 = flag3 = false;
							if (lightingState.r2 > num4)
							{
								num4 = lightingState.r2;
							}
							else if (num4 <= 0f)
							{
								flag3 = true;
							}
							else
							{
								lightingState.r2 = num4;
							}
							if (!flag3)
							{
								if (lightingState.stopLight)
								{
									num4 -= Lighting.negLight2;
								}
								else if (lightingState.wetLight)
								{
									num4 -= Lighting.wetLightR;
								}
								else
								{
									num4 -= Lighting.negLight;
								}
							}
							if (lightingState.g2 > num5)
							{
								num5 = lightingState.g2;
							}
							else if (num5 <= 0f)
							{
								flag2 = true;
							}
							else
							{
								lightingState.g2 = num5;
							}
							if (!flag2)
							{
								if (lightingState.stopLight)
								{
									num5 -= Lighting.negLight2;
								}
								else if (lightingState.wetLight)
								{
									num5 -= Lighting.wetLightG;
								}
								else
								{
									num5 -= Lighting.negLight;
								}
							}
							if (lightingState.b2 > num6)
							{
								num6 = lightingState.b2;
								goto IL_167;
							}
							if (num6 > 0f)
							{
								lightingState.b2 = num6;
								goto IL_167;
							}
							IL_186:
							num7 += num;
							continue;
							IL_167:
							if (lightingState.stopLight)
							{
								num6 -= Lighting.negLight2;
								goto IL_186;
							}
							num6 -= Lighting.negLight;
							goto IL_186;
						}
					}
					if (!flag)
					{
						break;
					}
					flag = false;
				}
			}
			catch
			{
			}
		}

		public static void AddLight(Vector2 position, Vector3 rgb)
		{
			Lighting.AddLight((int)(position.X / 16f), (int)(position.Y / 16f), rgb.X, rgb.Y, rgb.Z);
		}

		public static void AddLight(Vector2 position, float R, float G, float B)
		{
			Lighting.AddLight((int)(position.X / 16f), (int)(position.Y / 16f), R, G, B);
		}

		public static void AddLight(int i, int j, float R, float G, float B)
		{
			if (Main.gamePaused)
			{
				return;
			}
			if (Main.netMode == 2)
			{
				return;
			}
			if (i - Lighting.firstTileX + Lighting.offScreenTiles >= 0 && i - Lighting.firstTileX + Lighting.offScreenTiles < Main.screenWidth / 16 + Lighting.offScreenTiles * 2 + 10 && j - Lighting.firstTileY + Lighting.offScreenTiles >= 0 && j - Lighting.firstTileY + Lighting.offScreenTiles < Main.screenHeight / 16 + Lighting.offScreenTiles * 2 + 10)
			{
				if (Lighting.tempLights.Count == Lighting.maxTempLights)
				{
					return;
				}
				Point16 key = new Point16(i, j);
				Lighting.ColorTriplet value;
				if (Lighting.tempLights.TryGetValue(key, out value))
				{
					if (Lighting.RGB)
					{
						if (value.r < R)
						{
							value.r = R;
						}
						if (value.g < G)
						{
							value.g = G;
						}
						if (value.b < B)
						{
							value.b = B;
						}
						Lighting.tempLights[key] = value;
						return;
					}
					float num = (R + G + B) / 3f;
					if (value.r < num)
					{
						Lighting.tempLights[key] = new Lighting.ColorTriplet(num);
						return;
					}
				}
				else
				{
					if (Lighting.RGB)
					{
						value = new Lighting.ColorTriplet(R, G, B);
					}
					else
					{
						value = new Lighting.ColorTriplet((R + G + B) / 3f);
					}
					Lighting.tempLights.Add(key, value);
				}
			}
		}

		public static void NextLightMode()
		{
			Lighting.lightCounter += 100;
			Lighting.lightMode++;
			if (Lighting.lightMode >= 4)
			{
				Lighting.lightMode = 0;
			}
			if (Lighting.lightMode == 2 || Lighting.lightMode == 0)
			{
				Main.renderCount = 0;
				Main.renderNow = true;
				Lighting.BlackOut();
			}
		}

		public static void BlackOut()
		{
			int num = Main.screenWidth / 16 + Lighting.offScreenTiles * 2;
			int num2 = Main.screenHeight / 16 + Lighting.offScreenTiles * 2;
			for (int i = 0; i < num; i++)
			{
				Lighting.LightingState[] array = Lighting.states[i];
				for (int j = 0; j < num2; j++)
				{
					Lighting.LightingState lightingState = array[j];
					lightingState.r = 0f;
					lightingState.g = 0f;
					lightingState.b = 0f;
				}
			}
		}

		public static Color GetColor(int x, int y, Color oldColor)
		{
			int num = x - Lighting.firstTileX + Lighting.offScreenTiles;
			int num2 = y - Lighting.firstTileY + Lighting.offScreenTiles;
			if (Main.gameMenu)
			{
				return oldColor;
			}
			if (num < 0 || num2 < 0 || num >= Main.screenWidth / 16 + Lighting.offScreenTiles * 2 + 10 || num2 >= Main.screenHeight / 16 + Lighting.offScreenTiles * 2 + 10)
			{
				return Color.Black;
			}
			Color white = Color.White;
			Lighting.LightingState lightingState = Lighting.states[num][num2];
			int num3 = (int)((float)oldColor.R * lightingState.r * Lighting.brightness);
			int num4 = (int)((float)oldColor.G * lightingState.g * Lighting.brightness);
			int num5 = (int)((float)oldColor.B * lightingState.b * Lighting.brightness);
			if (num3 > 255)
			{
				num3 = 255;
			}
			if (num4 > 255)
			{
				num4 = 255;
			}
			if (num5 > 255)
			{
				num5 = 255;
			}
			white.R = (byte)num3;
			white.G = (byte)num4;
			white.B = (byte)num5;
			return white;
		}

		public static Color GetColor(int x, int y)
		{
			int num = x - Lighting.firstTileX + Lighting.offScreenTiles;
			int num2 = y - Lighting.firstTileY + Lighting.offScreenTiles;
			if (Main.gameMenu)
			{
				return Color.White;
			}
			if (num < 0 || num2 < 0 || num >= Main.screenWidth / 16 + Lighting.offScreenTiles * 2 + 10 || num2 >= Main.screenHeight / 16 + Lighting.offScreenTiles * 2)
			{
				return Color.Black;
			}
			Lighting.LightingState lightingState = Lighting.states[num][num2];
			int num3 = (int)(255f * lightingState.r * Lighting.brightness);
			int num4 = (int)(255f * lightingState.g * Lighting.brightness);
			int num5 = (int)(255f * lightingState.b * Lighting.brightness);
			if (num3 > 255)
			{
				num3 = 255;
			}
			if (num4 > 255)
			{
				num4 = 255;
			}
			if (num5 > 255)
			{
				num5 = 255;
			}
			Color result = new Color((int)((byte)num3), (int)((byte)num4), (int)((byte)num5), 255);
			return result;
		}

		public static void GetColor9Slice(int centerX, int centerY, ref Color[] slices)
		{
			int num = centerX - Lighting.firstTileX + Lighting.offScreenTiles;
			int num2 = centerY - Lighting.firstTileY + Lighting.offScreenTiles;
			if (num <= 0 || num2 <= 0 || num >= Main.screenWidth / 16 + Lighting.offScreenTiles * 2 + 10 - 1 || num2 >= Main.screenHeight / 16 + Lighting.offScreenTiles * 2 - 1)
			{
				for (int i = 0; i < 9; i++)
				{
					slices[i] = Color.Black;
				}
				return;
			}
			int num3 = 0;
			for (int j = num - 1; j <= num + 1; j++)
			{
				Lighting.LightingState[] array = Lighting.states[j];
				for (int k = num2 - 1; k <= num2 + 1; k++)
				{
					Lighting.LightingState lightingState = array[k];
					int num4 = (int)(255f * lightingState.r * Lighting.brightness);
					int num5 = (int)(255f * lightingState.g * Lighting.brightness);
					int num6 = (int)(255f * lightingState.b * Lighting.brightness);
					if (num4 > 255)
					{
						num4 = 255;
					}
					if (num5 > 255)
					{
						num5 = 255;
					}
					if (num6 > 255)
					{
						num6 = 255;
					}
					slices[num3] = new Color((int)((byte)num4), (int)((byte)num5), (int)((byte)num6), 255);
					num3 += 3;
				}
				num3 -= 8;
			}
		}

		public static Vector3 GetSubLight(Vector2 position)
		{
			Vector2 vector = position / 16f - new Vector2(0.5f, 0.5f);
			Vector2 vector2 = new Vector2(vector.X % 1f, vector.Y % 1f);
			int num = (int)vector.X - Lighting.firstTileX + Lighting.offScreenTiles;
			int num2 = (int)vector.Y - Lighting.firstTileY + Lighting.offScreenTiles;
			if (num <= 0 || num2 <= 0 || num >= Main.screenWidth / 16 + Lighting.offScreenTiles * 2 + 10 - 1 || num2 >= Main.screenHeight / 16 + Lighting.offScreenTiles * 2 - 1)
			{
				return Vector3.One;
			}
			Vector3 value = Lighting.states[num][num2].ToVector3();
			Vector3 value2 = Lighting.states[num + 1][num2].ToVector3();
			Vector3 value3 = Lighting.states[num][num2 + 1].ToVector3();
			Vector3 value4 = Lighting.states[num + 1][num2 + 1].ToVector3();
			Vector3 value5 = Vector3.Lerp(value, value2, vector2.X);
			Vector3 value6 = Vector3.Lerp(value3, value4, vector2.X);
			return Vector3.Lerp(value5, value6, vector2.Y);
		}

		public static void GetColor4Slice_New(int centerX, int centerY, out VertexColors vertices, float scale = 1f)
		{
			int num = centerX - Lighting.firstTileX + Lighting.offScreenTiles;
			int num2 = centerY - Lighting.firstTileY + Lighting.offScreenTiles;
			if (num <= 0 || num2 <= 0 || num >= Main.screenWidth / 16 + Lighting.offScreenTiles * 2 + 10 - 1 || num2 >= Main.screenHeight / 16 + Lighting.offScreenTiles * 2 - 1)
			{
				vertices.BottomLeftColor = Color.Black;
				vertices.BottomRightColor = Color.Black;
				vertices.TopLeftColor = Color.Black;
				vertices.TopRightColor = Color.Black;
				return;
			}
			Lighting.LightingState lightingState = Lighting.states[num][num2];
			Lighting.LightingState lightingState2 = Lighting.states[num][num2 - 1];
			Lighting.LightingState lightingState3 = Lighting.states[num][num2 + 1];
			Lighting.LightingState lightingState4 = Lighting.states[num - 1][num2];
			Lighting.LightingState lightingState5 = Lighting.states[num + 1][num2];
			Lighting.LightingState lightingState6 = Lighting.states[num - 1][num2 - 1];
			Lighting.LightingState lightingState7 = Lighting.states[num + 1][num2 - 1];
			Lighting.LightingState lightingState8 = Lighting.states[num - 1][num2 + 1];
			Lighting.LightingState lightingState9 = Lighting.states[num + 1][num2 + 1];
			float num3 = Lighting.brightness * scale * 255f * 0.25f;
			float num4 = (lightingState2.r + lightingState6.r + lightingState4.r + lightingState.r) * num3;
			float num5 = (lightingState2.g + lightingState6.g + lightingState4.g + lightingState.g) * num3;
			float num6 = (lightingState2.b + lightingState6.b + lightingState4.b + lightingState.b) * num3;
			if (num4 > 255f)
			{
				num4 = 255f;
			}
			if (num5 > 255f)
			{
				num5 = 255f;
			}
			if (num6 > 255f)
			{
				num6 = 255f;
			}
			vertices.TopLeftColor = new Color((int)((byte)num4), (int)((byte)num5), (int)((byte)num6), 255);
			num4 = (lightingState2.r + lightingState7.r + lightingState5.r + lightingState.r) * num3;
			num5 = (lightingState2.g + lightingState7.g + lightingState5.g + lightingState.g) * num3;
			num6 = (lightingState2.b + lightingState7.b + lightingState5.b + lightingState.b) * num3;
			if (num4 > 255f)
			{
				num4 = 255f;
			}
			if (num5 > 255f)
			{
				num5 = 255f;
			}
			if (num6 > 255f)
			{
				num6 = 255f;
			}
			vertices.TopRightColor = new Color((int)((byte)num4), (int)((byte)num5), (int)((byte)num6), 255);
			num4 = (lightingState3.r + lightingState8.r + lightingState4.r + lightingState.r) * num3;
			num5 = (lightingState3.g + lightingState8.g + lightingState4.g + lightingState.g) * num3;
			num6 = (lightingState3.b + lightingState8.b + lightingState4.b + lightingState.b) * num3;
			if (num4 > 255f)
			{
				num4 = 255f;
			}
			if (num5 > 255f)
			{
				num5 = 255f;
			}
			if (num6 > 255f)
			{
				num6 = 255f;
			}
			vertices.BottomLeftColor = new Color((int)((byte)num4), (int)((byte)num5), (int)((byte)num6), 255);
			num4 = (lightingState3.r + lightingState9.r + lightingState5.r + lightingState.r) * num3;
			num5 = (lightingState3.g + lightingState9.g + lightingState5.g + lightingState.g) * num3;
			num6 = (lightingState3.b + lightingState9.b + lightingState5.b + lightingState.b) * num3;
			if (num4 > 255f)
			{
				num4 = 255f;
			}
			if (num5 > 255f)
			{
				num5 = 255f;
			}
			if (num6 > 255f)
			{
				num6 = 255f;
			}
			vertices.BottomRightColor = new Color((int)((byte)num4), (int)((byte)num5), (int)((byte)num6), 255);
		}

		public static void GetColor4Slice_New(int centerX, int centerY, out VertexColors vertices, Color centerColor, float scale = 1f)
		{
			int num = centerX - Lighting.firstTileX + Lighting.offScreenTiles;
			int num2 = centerY - Lighting.firstTileY + Lighting.offScreenTiles;
			if (num <= 0 || num2 <= 0 || num >= Main.screenWidth / 16 + Lighting.offScreenTiles * 2 + 10 - 1 || num2 >= Main.screenHeight / 16 + Lighting.offScreenTiles * 2 - 1)
			{
				vertices.BottomLeftColor = Color.Black;
				vertices.BottomRightColor = Color.Black;
				vertices.TopLeftColor = Color.Black;
				vertices.TopRightColor = Color.Black;
				return;
			}
			float num3 = (float)centerColor.R / 255f;
			float num4 = (float)centerColor.G / 255f;
			float num5 = (float)centerColor.B / 255f;
			Lighting.LightingState lightingState = Lighting.states[num][num2 - 1];
			Lighting.LightingState lightingState2 = Lighting.states[num][num2 + 1];
			Lighting.LightingState lightingState3 = Lighting.states[num - 1][num2];
			Lighting.LightingState lightingState4 = Lighting.states[num + 1][num2];
			Lighting.LightingState lightingState5 = Lighting.states[num - 1][num2 - 1];
			Lighting.LightingState lightingState6 = Lighting.states[num + 1][num2 - 1];
			Lighting.LightingState lightingState7 = Lighting.states[num - 1][num2 + 1];
			Lighting.LightingState lightingState8 = Lighting.states[num + 1][num2 + 1];
			float num6 = Lighting.brightness * scale * 255f * 0.25f;
			float num7 = (lightingState.r + lightingState5.r + lightingState3.r + num3) * num6;
			float num8 = (lightingState.g + lightingState5.g + lightingState3.g + num4) * num6;
			float num9 = (lightingState.b + lightingState5.b + lightingState3.b + num5) * num6;
			if (num7 > 255f)
			{
				num7 = 255f;
			}
			if (num8 > 255f)
			{
				num8 = 255f;
			}
			if (num9 > 255f)
			{
				num9 = 255f;
			}
			vertices.TopLeftColor = new Color((int)((byte)num7), (int)((byte)num8), (int)((byte)num9), 255);
			num7 = (lightingState.r + lightingState6.r + lightingState4.r + num3) * num6;
			num8 = (lightingState.g + lightingState6.g + lightingState4.g + num4) * num6;
			num9 = (lightingState.b + lightingState6.b + lightingState4.b + num5) * num6;
			if (num7 > 255f)
			{
				num7 = 255f;
			}
			if (num8 > 255f)
			{
				num8 = 255f;
			}
			if (num9 > 255f)
			{
				num9 = 255f;
			}
			vertices.TopRightColor = new Color((int)((byte)num7), (int)((byte)num8), (int)((byte)num9), 255);
			num7 = (lightingState2.r + lightingState7.r + lightingState3.r + num3) * num6;
			num8 = (lightingState2.g + lightingState7.g + lightingState3.g + num4) * num6;
			num9 = (lightingState2.b + lightingState7.b + lightingState3.b + num5) * num6;
			if (num7 > 255f)
			{
				num7 = 255f;
			}
			if (num8 > 255f)
			{
				num8 = 255f;
			}
			if (num9 > 255f)
			{
				num9 = 255f;
			}
			vertices.BottomLeftColor = new Color((int)((byte)num7), (int)((byte)num8), (int)((byte)num9), 255);
			num7 = (lightingState2.r + lightingState8.r + lightingState4.r + num3) * num6;
			num8 = (lightingState2.g + lightingState8.g + lightingState4.g + num4) * num6;
			num9 = (lightingState2.b + lightingState8.b + lightingState4.b + num5) * num6;
			if (num7 > 255f)
			{
				num7 = 255f;
			}
			if (num8 > 255f)
			{
				num8 = 255f;
			}
			if (num9 > 255f)
			{
				num9 = 255f;
			}
			vertices.BottomRightColor = new Color((int)((byte)num7), (int)((byte)num8), (int)((byte)num9), 255);
		}

		public static void GetColor4Slice(int centerX, int centerY, ref Color[] slices)
		{
			int i = centerX - Lighting.firstTileX + Lighting.offScreenTiles;
			int num = centerY - Lighting.firstTileY + Lighting.offScreenTiles;
			if (i <= 0 || num <= 0 || i >= Main.screenWidth / 16 + Lighting.offScreenTiles * 2 + 10 - 1 || num >= Main.screenHeight / 16 + Lighting.offScreenTiles * 2 - 1)
			{
				for (i = 0; i < 4; i++)
				{
					slices[i] = Color.Black;
				}
				return;
			}
			Lighting.LightingState lightingState = Lighting.states[i][num - 1];
			Lighting.LightingState lightingState2 = Lighting.states[i][num + 1];
			Lighting.LightingState lightingState3 = Lighting.states[i - 1][num];
			Lighting.LightingState lightingState4 = Lighting.states[i + 1][num];
			float num2 = lightingState.r + lightingState.g + lightingState.b;
			float num3 = lightingState2.r + lightingState2.g + lightingState2.b;
			float num4 = lightingState4.r + lightingState4.g + lightingState4.b;
			float num5 = lightingState3.r + lightingState3.g + lightingState3.b;
			if (num2 >= num5)
			{
				int num6 = (int)(255f * lightingState3.r * Lighting.brightness);
				int num7 = (int)(255f * lightingState3.g * Lighting.brightness);
				int num8 = (int)(255f * lightingState3.b * Lighting.brightness);
				if (num6 > 255)
				{
					num6 = 255;
				}
				if (num7 > 255)
				{
					num7 = 255;
				}
				if (num8 > 255)
				{
					num8 = 255;
				}
				slices[0] = new Color((int)((byte)num6), (int)((byte)num7), (int)((byte)num8), 255);
			}
			else
			{
				int num9 = (int)(255f * lightingState.r * Lighting.brightness);
				int num10 = (int)(255f * lightingState.g * Lighting.brightness);
				int num11 = (int)(255f * lightingState.b * Lighting.brightness);
				if (num9 > 255)
				{
					num9 = 255;
				}
				if (num10 > 255)
				{
					num10 = 255;
				}
				if (num11 > 255)
				{
					num11 = 255;
				}
				slices[0] = new Color((int)((byte)num9), (int)((byte)num10), (int)((byte)num11), 255);
			}
			if (num2 >= num4)
			{
				int num12 = (int)(255f * lightingState4.r * Lighting.brightness);
				int num13 = (int)(255f * lightingState4.g * Lighting.brightness);
				int num14 = (int)(255f * lightingState4.b * Lighting.brightness);
				if (num12 > 255)
				{
					num12 = 255;
				}
				if (num13 > 255)
				{
					num13 = 255;
				}
				if (num14 > 255)
				{
					num14 = 255;
				}
				slices[1] = new Color((int)((byte)num12), (int)((byte)num13), (int)((byte)num14), 255);
			}
			else
			{
				int num15 = (int)(255f * lightingState.r * Lighting.brightness);
				int num16 = (int)(255f * lightingState.g * Lighting.brightness);
				int num17 = (int)(255f * lightingState.b * Lighting.brightness);
				if (num15 > 255)
				{
					num15 = 255;
				}
				if (num16 > 255)
				{
					num16 = 255;
				}
				if (num17 > 255)
				{
					num17 = 255;
				}
				slices[1] = new Color((int)((byte)num15), (int)((byte)num16), (int)((byte)num17), 255);
			}
			if (num3 >= num5)
			{
				int num18 = (int)(255f * lightingState3.r * Lighting.brightness);
				int num19 = (int)(255f * lightingState3.g * Lighting.brightness);
				int num20 = (int)(255f * lightingState3.b * Lighting.brightness);
				if (num18 > 255)
				{
					num18 = 255;
				}
				if (num19 > 255)
				{
					num19 = 255;
				}
				if (num20 > 255)
				{
					num20 = 255;
				}
				slices[2] = new Color((int)((byte)num18), (int)((byte)num19), (int)((byte)num20), 255);
			}
			else
			{
				int num21 = (int)(255f * lightingState2.r * Lighting.brightness);
				int num22 = (int)(255f * lightingState2.g * Lighting.brightness);
				int num23 = (int)(255f * lightingState2.b * Lighting.brightness);
				if (num21 > 255)
				{
					num21 = 255;
				}
				if (num22 > 255)
				{
					num22 = 255;
				}
				if (num23 > 255)
				{
					num23 = 255;
				}
				slices[2] = new Color((int)((byte)num21), (int)((byte)num22), (int)((byte)num23), 255);
			}
			if (num3 >= num4)
			{
				int num24 = (int)(255f * lightingState4.r * Lighting.brightness);
				int num25 = (int)(255f * lightingState4.g * Lighting.brightness);
				int num26 = (int)(255f * lightingState4.b * Lighting.brightness);
				if (num24 > 255)
				{
					num24 = 255;
				}
				if (num25 > 255)
				{
					num25 = 255;
				}
				if (num26 > 255)
				{
					num26 = 255;
				}
				slices[3] = new Color((int)((byte)num24), (int)((byte)num25), (int)((byte)num26), 255);
				return;
			}
			int num27 = (int)(255f * lightingState2.r * Lighting.brightness);
			int num28 = (int)(255f * lightingState2.g * Lighting.brightness);
			int num29 = (int)(255f * lightingState2.b * Lighting.brightness);
			if (num27 > 255)
			{
				num27 = 255;
			}
			if (num28 > 255)
			{
				num28 = 255;
			}
			if (num29 > 255)
			{
				num29 = 255;
			}
			slices[3] = new Color((int)((byte)num27), (int)((byte)num28), (int)((byte)num29), 255);
		}

		public static Color GetBlackness(int x, int y)
		{
			int num = x - Lighting.firstTileX + Lighting.offScreenTiles;
			int num2 = y - Lighting.firstTileY + Lighting.offScreenTiles;
			if (num < 0 || num2 < 0 || num >= Main.screenWidth / 16 + Lighting.offScreenTiles * 2 + 10 || num2 >= Main.screenHeight / 16 + Lighting.offScreenTiles * 2 + 10)
			{
				return Color.Black;
			}
			Color result = new Color(0, 0, 0, (int)((byte)(255f - 255f * Lighting.states[num][num2].r)));
			return result;
		}

		public static float Brightness(int x, int y)
		{
			int num = x - Lighting.firstTileX + Lighting.offScreenTiles;
			int num2 = y - Lighting.firstTileY + Lighting.offScreenTiles;
			if (num < 0 || num2 < 0 || num >= Main.screenWidth / 16 + Lighting.offScreenTiles * 2 + 10 || num2 >= Main.screenHeight / 16 + Lighting.offScreenTiles * 2 + 10)
			{
				return 0f;
			}
			Lighting.LightingState lightingState = Lighting.states[num][num2];
			return Lighting.brightness * (lightingState.r + lightingState.g + lightingState.b) / 3f;
		}

		public static float BrightnessAverage(int x, int y, int width, int height)
		{
			int num = x - Lighting.firstTileX + Lighting.offScreenTiles;
			int num2 = y - Lighting.firstTileY + Lighting.offScreenTiles;
			int num3 = num + width;
			int num4 = num2 + height;
			if (num < 0)
			{
				num = 0;
			}
			if (num2 < 0)
			{
				num2 = 0;
			}
			if (num3 >= Main.screenWidth / 16 + Lighting.offScreenTiles * 2 + 10)
			{
				num3 = Main.screenWidth / 16 + Lighting.offScreenTiles * 2 + 10;
			}
			if (num4 >= Main.screenHeight / 16 + Lighting.offScreenTiles * 2 + 10)
			{
				num4 = Main.screenHeight / 16 + Lighting.offScreenTiles * 2 + 10;
			}
			float num5 = 0f;
			float num6 = 0f;
			for (int i = num; i < num3; i++)
			{
				for (int j = num2; j < num4; j++)
				{
					num5 += 1f;
					Lighting.LightingState lightingState = Lighting.states[i][j];
					num6 += (lightingState.r + lightingState.g + lightingState.b) / 3f;
				}
			}
			if (num5 == 0f)
			{
				return 0f;
			}
			return num6 / num5;
		}
	}
}
