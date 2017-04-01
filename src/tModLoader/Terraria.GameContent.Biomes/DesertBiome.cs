using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria.World.Generation;

namespace Terraria.GameContent.Biomes
{
	public class DesertBiome : MicroBiome
	{
		private struct Hub
		{
			public Vector2 Position;

			public Hub(Vector2 position)
			{
				this.Position = position;
			}

			public Hub(float x, float y)
			{
				this.Position = new Vector2(x, y);
			}
		}

		private class Cluster : List<DesertBiome.Hub>
		{
		}

		private class ClusterGroup : List<DesertBiome.Cluster>
		{
			public int Width;
			public int Height;

			private void SearchForCluster(bool[,] hubMap, List<Point> pointCluster, int x, int y, int level = 2)
			{
				pointCluster.Add(new Point(x, y));
				hubMap[x, y] = false;
				level--;
				if (level == -1)
				{
					return;
				}
				if (x > 0 && hubMap[x - 1, y])
				{
					this.SearchForCluster(hubMap, pointCluster, x - 1, y, level);
				}
				if (x < hubMap.GetLength(0) - 1 && hubMap[x + 1, y])
				{
					this.SearchForCluster(hubMap, pointCluster, x + 1, y, level);
				}
				if (y > 0 && hubMap[x, y - 1])
				{
					this.SearchForCluster(hubMap, pointCluster, x, y - 1, level);
				}
				if (y < hubMap.GetLength(1) - 1 && hubMap[x, y + 1])
				{
					this.SearchForCluster(hubMap, pointCluster, x, y + 1, level);
				}
			}

			private void AttemptClaim(int x, int y, int[,] clusterIndexMap, List<List<Point>> pointClusters, int index)
			{
				int num = clusterIndexMap[x, y];
				if (num != -1 && num != index)
				{
					int num2 = (WorldGen.genRand.Next(2) == 0) ? -1 : index;
					List<Point> list = pointClusters[num];
					foreach (Point current in list)
					{
						clusterIndexMap[current.X, current.Y] = num2;
					}
				}
			}

			public void Generate(int width, int height)
			{
				this.Width = width;
				this.Height = height;
				base.Clear();
				bool[,] array = new bool[width, height];
				int num = (width >> 1) - 1;
				int num2 = (height >> 1) - 1;
				int num3 = (num + 1) * (num + 1);
				Point point = new Point(num, num2);
				for (int i = point.Y - num2; i <= point.Y + num2; i++)
				{
					float num4 = (float)num / (float)num2 * (float)(i - point.Y);
					int num5 = Math.Min(num, (int)Math.Sqrt((double)((float)num3 - num4 * num4)));
					for (int j = point.X - num5; j <= point.X + num5; j++)
					{
						array[j, i] = (WorldGen.genRand.Next(2) == 0);
					}
				}
				List<List<Point>> list = new List<List<Point>>();
				for (int k = 0; k < array.GetLength(0); k++)
				{
					for (int l = 0; l < array.GetLength(1); l++)
					{
						if (array[k, l] && WorldGen.genRand.Next(2) == 0)
						{
							List<Point> list2 = new List<Point>();
							this.SearchForCluster(array, list2, k, l, 2);
							if (list2.Count > 2)
							{
								list.Add(list2);
							}
						}
					}
				}
				int[,] array2 = new int[array.GetLength(0), array.GetLength(1)];
				for (int m = 0; m < array2.GetLength(0); m++)
				{
					for (int n = 0; n < array2.GetLength(1); n++)
					{
						array2[m, n] = -1;
					}
				}
				for (int num6 = 0; num6 < list.Count; num6++)
				{
					foreach (Point current in list[num6])
					{
						array2[current.X, current.Y] = num6;
					}
				}
				for (int num7 = 0; num7 < list.Count; num7++)
				{
					List<Point> list3 = list[num7];
					foreach (Point current2 in list3)
					{
						int x = current2.X;
						int y = current2.Y;
						if (array2[x, y] == -1)
						{
							break;
						}
						int index = array2[x, y];
						if (x > 0)
						{
							this.AttemptClaim(x - 1, y, array2, list, index);
						}
						if (x < array2.GetLength(0) - 1)
						{
							this.AttemptClaim(x + 1, y, array2, list, index);
						}
						if (y > 0)
						{
							this.AttemptClaim(x, y - 1, array2, list, index);
						}
						if (y < array2.GetLength(1) - 1)
						{
							this.AttemptClaim(x, y + 1, array2, list, index);
						}
					}
				}
				foreach (List<Point> current3 in list)
				{
					current3.Clear();
				}
				for (int num8 = 0; num8 < array2.GetLength(0); num8++)
				{
					for (int num9 = 0; num9 < array2.GetLength(1); num9++)
					{
						if (array2[num8, num9] != -1)
						{
							list[array2[num8, num9]].Add(new Point(num8, num9));
						}
					}
				}
				foreach (List<Point> current4 in list)
				{
					if (current4.Count < 4)
					{
						current4.Clear();
					}
				}
				foreach (List<Point> current5 in list)
				{
					DesertBiome.Cluster cluster = new DesertBiome.Cluster();
					if (current5.Count > 0)
					{
						foreach (Point current6 in current5)
						{
							cluster.Add(new DesertBiome.Hub((float)current6.X + (WorldGen.genRand.NextFloat() - 0.5f) * 0.5f, (float)current6.Y + (WorldGen.genRand.NextFloat() - 0.5f) * 0.5f));
						}
						base.Add(cluster);
					}
				}
			}
		}

		private void PlaceSand(DesertBiome.ClusterGroup clusters, Point start, Vector2 scale)
		{
			int num = (int)(scale.X * (float)clusters.Width);
			int num2 = (int)(scale.Y * (float)clusters.Height);
			int num3 = 5;
			int num4 = start.Y + (num2 >> 1);
			float num5 = 0f;
			short[] array = new short[num + num3 * 2];
			for (int i = -num3; i < num + num3; i++)
			{
				for (int j = 150; j < num4; j++)
				{
					if (WorldGen.SolidOrSlopedTile(i + start.X, j))
					{
						num5 += (float)(j - 1);
						array[i + num3] = (short)(j - 1);
						break;
					}
				}
			}
			float num6 = num5 / (float)(num + num3 * 2);
			int num7 = 0;
			for (int k = -num3; k < num + num3; k++)
			{
				float num8 = Math.Abs((float)(k + num3) / (float)(num + num3 * 2)) * 2f - 1f;
				num8 = MathHelper.Clamp(num8, -1f, 1f);
				if (k % 3 == 0)
				{
					num7 = Utils.Clamp<int>(num7 + GenBase._random.Next(-1, 2), -10, 10);
				}
				float num9 = (float)Math.Sqrt((double)(1f - num8 * num8 * num8 * num8));
				int num10 = num4 - (int)(num9 * ((float)num4 - num6)) + num7;
				int num11 = num4 - (int)(((float)num4 - num6) * (num9 - 0.15f / (float)Math.Sqrt(Math.Max(0.01, (double)Math.Abs(8f * num8) - 0.1)) + 0.25f));
				num11 = Math.Min(num4, num11);
				int num12 = (int)(70f - Utils.SmoothStep(0.5f, 0.8f, Math.Abs(num8)) * 70f);
				if (num10 - (int)array[k + num3] < num12)
				{
					for (int l = 0; l < num12; l++)
					{
						int num13 = k + start.X;
						int num14 = l + num10 - 70;
						GenBase._tiles[num13, num14].active(false);
						GenBase._tiles[num13, num14].wall = 0;
					}
					array[k + num3] = (short)Utils.Clamp<int>(num12 + num10 - 70, num10, (int)array[k + num3]);
				}
				for (int m = num4 - 1; m >= num10; m--)
				{
					int num15 = k + start.X;
					int num16 = m;
					Tile tile = GenBase._tiles[num15, num16];
					tile.liquid = 0;
					Tile testTile = GenBase._tiles[num15, num16 + 1];
					Tile testTile2 = GenBase._tiles[num15, num16 + 2];
					tile.type = ((WorldGen.SolidTile(testTile) && WorldGen.SolidTile(testTile2)) ? (ushort)53 : (ushort)397);
					if (m > num10 + 5)
					{
						tile.wall = 187;
					}
					tile.active(true);
					if (tile.wall != 187)
					{
						tile.wall = 0;
					}
					if (m < num11)
					{
						if (m > num10 + 5)
						{
							tile.wall = 187;
						}
						tile.active(false);
					}
					WorldGen.SquareWallFrame(num15, num16, true);
				}
			}
		}

		private void PlaceClusters(DesertBiome.ClusterGroup clusters, Point start, Vector2 scale)
		{
			int num = (int)(scale.X * (float)clusters.Width);
			int num2 = (int)(scale.Y * (float)clusters.Height);
			Vector2 value = new Vector2((float)num, (float)num2);
			Vector2 value2 = new Vector2((float)clusters.Width, (float)clusters.Height);
			for (int i = -20; i < num + 20; i++)
			{
				for (int j = -20; j < num2 + 20; j++)
				{
					float num3 = 0f;
					int num4 = -1;
					float num5 = 0f;
					int num6 = i + start.X;
					int num7 = j + start.Y;
					Vector2 value3 = new Vector2((float)i, (float)j) / value * value2;
					float num8 = (new Vector2((float)i, (float)j) / value * 2f - Vector2.One).Length();
					for (int k = 0; k < clusters.Count; k++)
					{
						DesertBiome.Cluster cluster = clusters[k];
						if (Math.Abs(cluster[0].Position.X - value3.X) <= 10f && Math.Abs(cluster[0].Position.Y - value3.Y) <= 10f)
						{
							float num9 = 0f;
							foreach (DesertBiome.Hub current in cluster)
							{
								num9 += 1f / Vector2.DistanceSquared(current.Position, value3);
							}
							if (num9 > num3)
							{
								if (num3 > num5)
								{
									num5 = num3;
								}
								num3 = num9;
								num4 = k;
							}
							else if (num9 > num5)
							{
								num5 = num9;
							}
						}
					}
					float num10 = num3 + num5;
					Tile tile = GenBase._tiles[num6, num7];
					bool flag = num8 >= 0.8f;
					if (num10 > 3.5f)
					{
						tile.ClearEverything();
						tile.wall = 187;
						tile.liquid = 0;
						if (num4 % 15 == 2)
						{
							tile.ResetToType(404);
							tile.wall = 187;
							tile.active(true);
						}
						Tile.SmoothSlope(num6, num7, true);
					}
					else if (num10 > 1.8f)
					{
						tile.wall = 187;
						if (!flag || tile.active())
						{
							tile.ResetToType(396);
							tile.wall = 187;
							tile.active(true);
							Tile.SmoothSlope(num6, num7, true);
						}
						tile.liquid = 0;
					}
					else if (num10 > 0.7f || !flag)
					{
						if (!flag || tile.active())
						{
							tile.ResetToType(397);
							tile.active(true);
							Tile.SmoothSlope(num6, num7, true);
						}
						tile.liquid = 0;
						tile.wall = 216;
					}
					else if (num10 > 0.25f)
					{
						float num11 = (num10 - 0.25f) / 0.45f;
						if (GenBase._random.NextFloat() < num11)
						{
							if (tile.active())
							{
								tile.ResetToType(397);
								tile.active(true);
								Tile.SmoothSlope(num6, num7, true);
								tile.wall = 216;
							}
							tile.liquid = 0;
							tile.wall = 187;
						}
					}
				}
			}
		}

		private void AddTileVariance(DesertBiome.ClusterGroup clusters, Point start, Vector2 scale)
		{
			int num = (int)(scale.X * (float)clusters.Width);
			int num2 = (int)(scale.Y * (float)clusters.Height);
			for (int i = -20; i < num + 20; i++)
			{
				for (int j = -20; j < num2 + 20; j++)
				{
					int num3 = i + start.X;
					int num4 = j + start.Y;
					Tile tile = GenBase._tiles[num3, num4];
					Tile testTile = GenBase._tiles[num3, num4 + 1];
					Tile testTile2 = GenBase._tiles[num3, num4 + 2];
					if (tile.type == 53 && (!WorldGen.SolidTile(testTile) || !WorldGen.SolidTile(testTile2)))
					{
						tile.type = 397;
					}
				}
			}
			for (int k = -20; k < num + 20; k++)
			{
				for (int l = -20; l < num2 + 20; l++)
				{
					int num5 = k + start.X;
					int num6 = l + start.Y;
					Tile tile2 = GenBase._tiles[num5, num6];
					if (tile2.active() && tile2.type == 396)
					{
						bool flag = true;
						for (int m = -1; m >= -3; m--)
						{
							if (GenBase._tiles[num5, num6 + m].active())
							{
								flag = false;
								break;
							}
						}
						bool flag2 = true;
						for (int n = 1; n <= 3; n++)
						{
							if (GenBase._tiles[num5, num6 + n].active())
							{
								flag2 = false;
								break;
							}
						}
						if ((flag ^ flag2) && GenBase._random.Next(5) == 0)
						{
							WorldGen.PlaceTile(num5, num6 + (flag ? -1 : 1), 165, true, true, -1, 0);
						}
						else if (flag && GenBase._random.Next(5) == 0)
						{
							WorldGen.PlaceTile(num5, num6 - 1, 187, true, true, -1, 29 + GenBase._random.Next(6));
						}
					}
				}
			}
		}

		private bool FindStart(Point origin, Vector2 scale, int xHubCount, int yHubCount, out Point start)
		{
			start = new Point(0, 0);
			int num = (int)(scale.X * (float)xHubCount);
			int height = (int)(scale.Y * (float)yHubCount);
			origin.X -= num >> 1;
			int num2 = 220;
			for (int i = -20; i < num + 20; i++)
			{
				int j = 220;
				while (j < Main.maxTilesY)
				{
					if (WorldGen.SolidTile(i + origin.X, j))
					{
						ushort type = GenBase._tiles[i + origin.X, j].type;
						if (type == 59 || type == 60)
						{
							return false;
						}
						if (j > num2)
						{
							num2 = j;
							break;
						}
						break;
					}
					else
					{
						j++;
					}
				}
			}
			WorldGen.UndergroundDesertLocation = new Rectangle(origin.X, num2, num, height);
			start = new Point(origin.X, num2);
			return true;
		}

		public override bool Place(Point origin, StructureMap structures)
		{
			float num = (float)Main.maxTilesX / 4200f;
			int num2 = (int)(80f * num);
			int num3 = (int)((GenBase._random.NextFloat() + 1f) * 80f * num);
			Vector2 scale = new Vector2(4f, 2f);
			Point start;
			if (!this.FindStart(origin, scale, num2, num3, out start))
			{
				return false;
			}
			DesertBiome.ClusterGroup clusterGroup = new DesertBiome.ClusterGroup();
			clusterGroup.Generate(num2, num3);
			this.PlaceSand(clusterGroup, start, scale);
			this.PlaceClusters(clusterGroup, start, scale);
			this.AddTileVariance(clusterGroup, start, scale);
			int num4 = (int)(scale.X * (float)clusterGroup.Width);
			int num5 = (int)(scale.Y * (float)clusterGroup.Height);
			for (int i = -20; i < num4 + 20; i++)
			{
				for (int j = -20; j < num5 + 20; j++)
				{
					if (i + start.X > 0 && i + start.X < Main.maxTilesX - 1 && j + start.Y > 0 && j + start.Y < Main.maxTilesY - 1)
					{
						WorldGen.SquareWallFrame(i + start.X, j + start.Y, true);
						WorldUtils.TileFrame(i + start.X, j + start.Y, true);
					}
				}
			}
			return true;
		}
	}
}
