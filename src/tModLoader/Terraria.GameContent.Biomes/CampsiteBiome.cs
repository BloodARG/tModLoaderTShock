using Microsoft.Xna.Framework;
using System;
using Terraria.World.Generation;

namespace Terraria.GameContent.Biomes
{
	public class CampsiteBiome : MicroBiome
	{
		public override bool Place(Point origin, StructureMap structures)
		{
			Ref<int> @ref = new Ref<int>(0);
			Ref<int> ref2 = new Ref<int>(0);
			WorldUtils.Gen(origin, new Shapes.Circle(10), Actions.Chain(new GenAction[]
					{
						new Actions.Scanner(ref2),
						new Modifiers.IsSolid(),
						new Actions.Scanner(@ref)
					}));
			if (@ref.Value < ref2.Value - 5)
			{
				return false;
			}
			int num = GenBase._random.Next(6, 10);
			int num2 = GenBase._random.Next(5);
			if (!structures.CanPlace(new Rectangle(origin.X - num, origin.Y - num, num * 2, num * 2), 0))
			{
				return false;
			}
			ShapeData data = new ShapeData();
			Point arg_12A_0 = origin;
			GenShape arg_12A_1 = new Shapes.Slime(num);
			GenAction[] array = new GenAction[6];
			array[0] = new Modifiers.Blotches(num2, num2, num2, 1, 0.3).Output(data);
			array[1] = new Modifiers.Offset(0, -2);
			array[2] = new Modifiers.OnlyTiles(new ushort[]
				{
					53
				});
			array[3] = new Actions.SetTile(397, true, true);
			GenAction[] arg_116_0 = array;
			int arg_116_1 = 4;
			byte[] types = new byte[1];
			arg_116_0[arg_116_1] = new Modifiers.OnlyWalls(types);
			array[5] = new Actions.PlaceWall(16, true);
			WorldUtils.Gen(arg_12A_0, arg_12A_1, Actions.Chain(array));
			Point arg_185_0 = origin;
			GenShape arg_185_1 = new ModShapes.All(data);
			GenAction[] array2 = new GenAction[5];
			array2[0] = new Actions.ClearTile(false);
			array2[1] = new Actions.SetLiquid(0, 0);
			array2[2] = new Actions.SetFrames(true);
			GenAction[] arg_171_0 = array2;
			int arg_171_1 = 3;
			byte[] types2 = new byte[1];
			arg_171_0[arg_171_1] = new Modifiers.OnlyWalls(types2);
			array2[4] = new Actions.PlaceWall(16, true);
			WorldUtils.Gen(arg_185_0, arg_185_1, Actions.Chain(array2));
			Point point;
			if (!WorldUtils.Find(origin, Searches.Chain(new Searches.Down(10), new GenCondition[]
					{
						new Conditions.IsSolid()
					}), out point))
			{
				return false;
			}
			int num3 = point.Y - 1;
			bool flag = GenBase._random.Next() % 2 == 0;
			if (GenBase._random.Next() % 10 != 0)
			{
				int num4 = GenBase._random.Next(1, 4);
				int num5 = flag ? 4 : (-(num >> 1));
				for (int i = 0; i < num4; i++)
				{
					int num6 = GenBase._random.Next(1, 3);
					for (int j = 0; j < num6; j++)
					{
						WorldGen.PlaceTile(origin.X + num5 - i, num3 - j, 331, false, false, -1, 0);
					}
				}
			}
			int num7 = (num - 3) * (flag ? -1 : 1);
			if (GenBase._random.Next() % 10 != 0)
			{
				WorldGen.PlaceTile(origin.X + num7, num3, 186, false, false, -1, 0);
			}
			if (GenBase._random.Next() % 10 != 0)
			{
				WorldGen.PlaceTile(origin.X, num3, 215, true, false, -1, 0);
				if (GenBase._tiles[origin.X, num3].active() && GenBase._tiles[origin.X, num3].type == 215)
				{
					Tile expr_305 = GenBase._tiles[origin.X, num3];
					expr_305.frameY += 36;
					Tile expr_329 = GenBase._tiles[origin.X - 1, num3];
					expr_329.frameY += 36;
					Tile expr_34D = GenBase._tiles[origin.X + 1, num3];
					expr_34D.frameY += 36;
					Tile expr_371 = GenBase._tiles[origin.X, num3 - 1];
					expr_371.frameY += 36;
					Tile expr_397 = GenBase._tiles[origin.X - 1, num3 - 1];
					expr_397.frameY += 36;
					Tile expr_3BD = GenBase._tiles[origin.X + 1, num3 - 1];
					expr_3BD.frameY += 36;
				}
			}
			structures.AddStructure(new Rectangle(origin.X - num, origin.Y - num, num * 2, num * 2), 4);
			return true;
		}
	}
}
