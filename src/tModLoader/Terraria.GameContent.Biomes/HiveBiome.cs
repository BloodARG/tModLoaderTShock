using Microsoft.Xna.Framework;
using System;
using Terraria.World.Generation;

namespace Terraria.GameContent.Biomes
{
	public class HiveBiome : MicroBiome
	{
		public override bool Place(Point origin, StructureMap structures)
		{
			Ref<int> @ref = new Ref<int>(0);
			Ref<int> ref2 = new Ref<int>(0);
			Ref<int> count = new Ref<int>(0);
			Ref<int> ref3 = new Ref<int>(0);
			WorldUtils.Gen(origin, new Shapes.Circle(15), Actions.Chain(new GenAction[]
					{
						new Actions.Scanner(count),
						new Modifiers.IsSolid(),
						new Actions.Scanner(@ref),
						new Modifiers.OnlyTiles(new ushort[]
							{
								60,
								59
							}),
						new Actions.Scanner(ref2),
						new Modifiers.OnlyTiles(new ushort[]
							{
								60
							}),
						new Actions.Scanner(ref3)
					}));
			if ((float)ref2.Value / (float)@ref.Value < 0.75f || ref3.Value < 2)
			{
				return false;
			}
			if (!structures.CanPlace(new Rectangle(origin.X - 50, origin.Y - 50, 100, 100), 0))
			{
				return false;
			}
			int x = origin.X;
			int y = origin.Y;
			int num = 150;
			for (int i = x - num; i < x + num; i += 10)
			{
				if (i > 0 && i <= Main.maxTilesX - 1)
				{
					for (int j = y - num; j < y + num; j += 10)
					{
						if (j > 0 && j <= Main.maxTilesY - 1)
						{
							if (Main.tile[i, j].active() && Main.tile[i, j].type == 226)
							{
								return false;
							}
							if (Main.tile[i, j].wall == 87 || Main.tile[i, j].wall == 3 || Main.tile[i, j].wall == 83)
							{
								return false;
							}
						}
					}
				}
			}
			int x2 = origin.X;
			int y2 = origin.Y;
			int num2 = 0;
			int[] array = new int[10];
			int[] array2 = new int[10];
			Vector2 vector = new Vector2((float)x2, (float)y2);
			Vector2 vector2 = vector;
			int num3 = WorldGen.genRand.Next(2, 5);
			for (int k = 0; k < num3; k++)
			{
				int num4 = WorldGen.genRand.Next(2, 5);
				for (int l = 0; l < num4; l++)
				{
					vector2 = WorldGen.Hive((int)vector.X, (int)vector.Y);
				}
				vector = vector2;
				array[num2] = (int)vector.X;
				array2[num2] = (int)vector.Y;
				num2++;
			}
			for (int m = 0; m < num2; m++)
			{
				int num5 = array[m];
				int num6 = array2[m];
				bool flag = false;
				int num7 = 1;
				if (WorldGen.genRand.Next(2) == 0)
				{
					num7 = -1;
				}
				while (num5 > 10 && num5 < Main.maxTilesX - 10 && num6 > 10 && num6 < Main.maxTilesY - 10 && (!Main.tile[num5, num6].active() || !Main.tile[num5, num6 + 1].active() || !Main.tile[num5 + 1, num6].active() || !Main.tile[num5 + 1, num6 + 1].active()))
				{
					num5 += num7;
					if (Math.Abs(num5 - array[m]) > 50)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					num5 += num7;
					for (int n = num5 - 1; n <= num5 + 2; n++)
					{
						for (int num8 = num6 - 1; num8 <= num6 + 2; num8++)
						{
							if (n < 10 || n > Main.maxTilesX - 10)
							{
								flag = true;
							}
							else if (Main.tile[n, num8].active() && Main.tile[n, num8].type != 225)
							{
								flag = true;
								break;
							}
						}
					}
					if (!flag)
					{
						for (int num9 = num5 - 1; num9 <= num5 + 2; num9++)
						{
							for (int num10 = num6 - 1; num10 <= num6 + 2; num10++)
							{
								if (num9 >= num5 && num9 <= num5 + 1 && num10 >= num6 && num10 <= num6 + 1)
								{
									Main.tile[num9, num10].active(false);
									Main.tile[num9, num10].liquid = 255;
									Main.tile[num9, num10].honey(true);
								}
								else
								{
									Main.tile[num9, num10].active(true);
									Main.tile[num9, num10].type = 225;
								}
							}
						}
						num7 *= -1;
						num6++;
						int num11 = 0;
						while ((num11 < 4 || WorldGen.SolidTile(num5, num6)) && num5 > 10 && num5 < Main.maxTilesX - 10)
						{
							num11++;
							num5 += num7;
							if (WorldGen.SolidTile(num5, num6))
							{
								WorldGen.PoundTile(num5, num6);
								if (!Main.tile[num5, num6 + 1].active())
								{
									Main.tile[num5, num6 + 1].active(true);
									Main.tile[num5, num6 + 1].type = 225;
								}
							}
						}
					}
				}
			}
			WorldGen.larvaX[WorldGen.numLarva] = Utils.Clamp<int>((int)vector.X, 5, Main.maxTilesX - 5);
			WorldGen.larvaY[WorldGen.numLarva] = Utils.Clamp<int>((int)vector.Y, 5, Main.maxTilesY - 5);
			WorldGen.numLarva++;
			int num12 = (int)vector.X;
			int num13 = (int)vector.Y;
			int num14 = num12 - 1;
			while (num14 <= num12 + 1 && num14 > 0 && num14 < Main.maxTilesX)
			{
				int num15 = num13 - 2;
				while (num15 <= num13 + 1 && num15 > 0 && num15 < Main.maxTilesY)
				{
					if (num15 != num13 + 1)
					{
						Main.tile[num14, num15].active(false);
					}
					else
					{
						Main.tile[num14, num15].active(true);
						Main.tile[num14, num15].type = 225;
						Main.tile[num14, num15].slope(0);
						Main.tile[num14, num15].halfBrick(false);
					}
					num15++;
				}
				num14++;
			}
			structures.AddStructure(new Rectangle(origin.X - 50, origin.Y - 50, 100, 100), 5);
			return true;
		}
	}
}
