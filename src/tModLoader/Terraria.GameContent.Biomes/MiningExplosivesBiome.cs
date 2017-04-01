using Microsoft.Xna.Framework;
using System;
using Terraria.GameContent.Generation;
using Terraria.World.Generation;

namespace Terraria.GameContent.Biomes
{
	public class MiningExplosivesBiome : MicroBiome
	{
		public override bool Place(Point origin, StructureMap structures)
		{
			if (WorldGen.SolidTile(origin.X, origin.Y))
			{
				return false;
			}
			ushort type = Utils.SelectRandom<ushort>(GenBase._random, new ushort[]
				{
					(WorldGen.goldBar == 19) ? (ushort)8 : (ushort)169,
					(WorldGen.silverBar == 21) ? (ushort)9 : (ushort)168,
					(WorldGen.ironBar == 22) ? (ushort)6 : (ushort)167,
					(WorldGen.copperBar == 20) ? (ushort)7 : (ushort)166
				});
			double num = GenBase._random.NextDouble() * 2.0 - 1.0;
			if (!WorldUtils.Find(origin, Searches.Chain((num > 0.0) ? (GenSearch)new Searches.Right(40) : (GenSearch)new Searches.Left(40), new GenCondition[]
					{
						new Conditions.IsSolid()
					}), out origin))
			{
				return false;
			}
			if (!WorldUtils.Find(origin, Searches.Chain(new Searches.Down(80), new GenCondition[]
					{
						new Conditions.IsSolid()
					}), out origin))
			{
				return false;
			}
			ShapeData shapeData = new ShapeData();
			Ref<int> @ref = new Ref<int>(0);
			Ref<int> ref2 = new Ref<int>(0);
			WorldUtils.Gen(origin, new ShapeRunner(10f, 20, new Vector2((float)num, 1f)).Output(shapeData), Actions.Chain(new GenAction[]
					{
						new Modifiers.Blotches(2, 0.3),
						new Actions.Scanner(@ref),
						new Modifiers.IsSolid(),
						new Actions.Scanner(ref2)
					}));
			if (ref2.Value < @ref.Value / 2)
			{
				return false;
			}
			Rectangle area = new Rectangle(origin.X - 15, origin.Y - 10, 30, 20);
			if (!structures.CanPlace(area, 0))
			{
				return false;
			}
			WorldUtils.Gen(origin, new ModShapes.All(shapeData), new Actions.SetTile(type, true, true));
			WorldUtils.Gen(new Point(origin.X - (int)(num * -5.0), origin.Y - 5), new Shapes.Circle(5), Actions.Chain(new GenAction[]
					{
						new Modifiers.Blotches(2, 0.3),
						new Actions.ClearTile(true)
					}));
			bool flag = true;
			Point start;
			flag &= WorldUtils.Find(new Point(origin.X - ((num > 0.0) ? 3 : -3), origin.Y - 3), Searches.Chain(new Searches.Down(10), new GenCondition[]
					{
						new Conditions.IsSolid()
					}), out start);
			int num2 = (GenBase._random.Next(4) == 0) ? 3 : 7;
			Point end;
			if (!(flag & WorldUtils.Find(new Point(origin.X - ((num > 0.0) ? (-num2) : num2), origin.Y - 3), Searches.Chain(new Searches.Down(10), new GenCondition[]
					{
						new Conditions.IsSolid()
					}), out end)))
			{
				return false;
			}
			start.Y--;
			end.Y--;
			Tile tile = GenBase._tiles[start.X, start.Y + 1];
			tile.slope(0);
			tile.halfBrick(false);
			for (int i = -1; i <= 1; i++)
			{
				WorldUtils.ClearTile(end.X + i, end.Y, false);
				Tile tile2 = GenBase._tiles[end.X + i, end.Y + 1];
				if (!WorldGen.SolidOrSlopedTile(tile2))
				{
					tile2.ResetToType(1);
					tile2.active(true);
				}
				tile2.slope(0);
				tile2.halfBrick(false);
				WorldUtils.TileFrame(end.X + i, end.Y + 1, true);
			}
			WorldGen.PlaceTile(start.X, start.Y, 141, false, false, -1, 0);
			WorldGen.PlaceTile(end.X, end.Y, 411, true, true, -1, 0);
			WorldUtils.WireLine(start, end);
			structures.AddStructure(area, 5);
			return true;
		}
	}
}
