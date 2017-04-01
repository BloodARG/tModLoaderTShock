using Microsoft.Xna.Framework;
using System;
using Terraria.ID;
using Terraria.World.Generation;

namespace Terraria.GameContent.Biomes
{
	public class MarbleBiome : MicroBiome
	{
		private delegate bool SlabState(int x, int y, int scale);

		private class SlabStates
		{
			public static bool Empty(int x, int y, int scale)
			{
				return false;
			}

			public static bool Solid(int x, int y, int scale)
			{
				return true;
			}

			public static bool HalfBrick(int x, int y, int scale)
			{
				return y >= scale / 2;
			}

			public static bool BottomRightFilled(int x, int y, int scale)
			{
				return x >= scale - y;
			}

			public static bool BottomLeftFilled(int x, int y, int scale)
			{
				return x < y;
			}

			public static bool TopRightFilled(int x, int y, int scale)
			{
				return x > y;
			}

			public static bool TopLeftFilled(int x, int y, int scale)
			{
				return x < scale - y;
			}
		}

		private struct Slab
		{
			public readonly MarbleBiome.SlabState State;
			public readonly bool HasWall;

			public bool IsSolid
			{
				get
				{
					return this.State != new MarbleBiome.SlabState(MarbleBiome.SlabStates.Empty);
				}
			}

			private Slab(MarbleBiome.SlabState state, bool hasWall)
			{
				this.State = state;
				this.HasWall = hasWall;
			}

			public MarbleBiome.Slab WithState(MarbleBiome.SlabState state)
			{
				return new MarbleBiome.Slab(state, this.HasWall);
			}

			public static MarbleBiome.Slab Create(MarbleBiome.SlabState state, bool hasWall)
			{
				return new MarbleBiome.Slab(state, hasWall);
			}
		}

		private const int SCALE = 3;
		private MarbleBiome.Slab[,] _slabs;

		private void SmoothSlope(int x, int y)
		{
			MarbleBiome.Slab slab = this._slabs[x, y];
			if (!slab.IsSolid)
			{
				return;
			}
			bool isSolid = this._slabs[x, y - 1].IsSolid;
			bool isSolid2 = this._slabs[x, y + 1].IsSolid;
			bool isSolid3 = this._slabs[x - 1, y].IsSolid;
			bool isSolid4 = this._slabs[x + 1, y].IsSolid;
			switch ((isSolid ? 1 : 0) << 3 | (isSolid2 ? 1 : 0) << 2 | (isSolid3 ? 1 : 0) << 1 | (isSolid4 ? 1 : 0))
			{
				case 4:
					this._slabs[x, y] = slab.WithState(new MarbleBiome.SlabState(MarbleBiome.SlabStates.HalfBrick));
					return;
				case 5:
					this._slabs[x, y] = slab.WithState(new MarbleBiome.SlabState(MarbleBiome.SlabStates.BottomRightFilled));
					return;
				case 6:
					this._slabs[x, y] = slab.WithState(new MarbleBiome.SlabState(MarbleBiome.SlabStates.BottomLeftFilled));
					return;
				case 9:
					this._slabs[x, y] = slab.WithState(new MarbleBiome.SlabState(MarbleBiome.SlabStates.TopRightFilled));
					return;
				case 10:
					this._slabs[x, y] = slab.WithState(new MarbleBiome.SlabState(MarbleBiome.SlabStates.TopLeftFilled));
					return;
			}
			this._slabs[x, y] = slab.WithState(new MarbleBiome.SlabState(MarbleBiome.SlabStates.Solid));
		}

		private void PlaceSlab(MarbleBiome.Slab slab, int originX, int originY, int scale)
		{
			for (int i = 0; i < scale; i++)
			{
				for (int j = 0; j < scale; j++)
				{
					Tile tile = GenBase._tiles[originX + i, originY + j];
					if (TileID.Sets.Ore[(int)tile.type])
					{
						tile.ResetToType(tile.type);
					}
					else
					{
						tile.ResetToType(367);
					}
					bool active = slab.State(i, j, scale);
					tile.active(active);
					if (slab.HasWall)
					{
						tile.wall = 178;
					}
					WorldUtils.TileFrame(originX + i, originY + j, true);
					WorldGen.SquareWallFrame(originX + i, originY + j, true);
					Tile.SmoothSlope(originX + i, originY + j, true);
					if (WorldGen.SolidTile(originX + i, originY + j - 1) && GenBase._random.Next(4) == 0)
					{
						WorldGen.PlaceTight(originX + i, originY + j, 165, false);
					}
					if (WorldGen.SolidTile(originX + i, originY + j) && GenBase._random.Next(4) == 0)
					{
						WorldGen.PlaceTight(originX + i, originY + j - 1, 165, false);
					}
				}
			}
		}

		private bool IsGroupSolid(int x, int y, int scale)
		{
			int num = 0;
			for (int i = 0; i < scale; i++)
			{
				for (int j = 0; j < scale; j++)
				{
					if (WorldGen.SolidOrSlopedTile(x + i, y + j))
					{
						num++;
					}
				}
			}
			return num > scale / 4 * 3;
		}

		public override bool Place(Point origin, StructureMap structures)
		{
			if (this._slabs == null)
			{
				this._slabs = new MarbleBiome.Slab[56, 26];
			}
			int num = GenBase._random.Next(80, 150) / 3;
			int num2 = GenBase._random.Next(40, 60) / 3;
			int num3 = (num2 * 3 - GenBase._random.Next(20, 30)) / 3;
			origin.X -= num * 3 / 2;
			origin.Y -= num2 * 3 / 2;
			for (int i = -1; i < num + 1; i++)
			{
				float num4 = (float)(i - num / 2) / (float)num + 0.5f;
				int num5 = (int)((0.5f - Math.Abs(num4 - 0.5f)) * 5f) - 2;
				for (int j = -1; j < num2 + 1; j++)
				{
					bool hasWall = true;
					bool flag = false;
					bool flag2 = this.IsGroupSolid(i * 3 + origin.X, j * 3 + origin.Y, 3);
					int num6 = Math.Abs(j - num2 / 2);
					int num7 = num6 - num3 / 4 + num5;
					if (num7 > 3)
					{
						flag = flag2;
						hasWall = false;
					}
					else if (num7 > 0)
					{
						flag = (j - num2 / 2 > 0 || flag2);
						hasWall = (j - num2 / 2 < 0 || num7 <= 2);
					}
					else if (num7 == 0)
					{
						flag = (GenBase._random.Next(2) == 0 && (j - num2 / 2 > 0 || flag2));
					}
					if (Math.Abs(num4 - 0.5f) > 0.35f + GenBase._random.NextFloat() * 0.1f && !flag2)
					{
						hasWall = false;
						flag = false;
					}
					this._slabs[i + 1, j + 1] = MarbleBiome.Slab.Create(flag ? new MarbleBiome.SlabState(MarbleBiome.SlabStates.Solid) : new MarbleBiome.SlabState(MarbleBiome.SlabStates.Empty), hasWall);
				}
			}
			for (int k = 0; k < num; k++)
			{
				for (int l = 0; l < num2; l++)
				{
					this.SmoothSlope(k + 1, l + 1);
				}
			}
			int num8 = num / 2;
			int num9 = num2 / 2;
			int num10 = (num9 + 1) * (num9 + 1);
			float value = GenBase._random.NextFloat() * 2f - 1f;
			float num11 = GenBase._random.NextFloat() * 2f - 1f;
			float value2 = GenBase._random.NextFloat() * 2f - 1f;
			float num12 = 0f;
			for (int m = 0; m <= num; m++)
			{
				float num13 = (float)num9 / (float)num8 * (float)(m - num8);
				int num14 = Math.Min(num9, (int)Math.Sqrt((double)Math.Max(0f, (float)num10 - num13 * num13)));
				if (m < num / 2)
				{
					num12 += MathHelper.Lerp(value, num11, (float)m / (float)(num / 2));
				}
				else
				{
					num12 += MathHelper.Lerp(num11, value2, (float)m / (float)(num / 2) - 1f);
				}
				for (int n = num9 - num14; n <= num9 + num14; n++)
				{
					this.PlaceSlab(this._slabs[m + 1, n + 1], m * 3 + origin.X, n * 3 + origin.Y + (int)num12, 3);
				}
			}
			return true;
		}
	}
}
