using Microsoft.Xna.Framework;
using System;
using Terraria.ID;
using Terraria.World.Generation;

namespace Terraria.GameContent.Biomes
{
	public class CorruptionPitBiome : MicroBiome
	{
		public static bool[] ValidTiles = TileID.Sets.Factory.CreateBoolSet(true, new int[]
			{
				21,
				31,
				26
			});

		public override bool Place(Point origin, StructureMap structures)
		{
			if (WorldGen.SolidTile(origin.X, origin.Y) && GenBase._tiles[origin.X, origin.Y].wall == 3)
			{
				return false;
			}
			if (!WorldUtils.Find(origin, Searches.Chain(new Searches.Down(100), new GenCondition[]
					{
						new Conditions.IsSolid()
					}), out origin))
			{
				return false;
			}
			Point point;
			if (!WorldUtils.Find(new Point(origin.X - 4, origin.Y), Searches.Chain(new Searches.Down(5), new GenCondition[]
					{
						new Conditions.IsTile(new ushort[]
							{
								25
							}).AreaAnd(8, 1)
					}), out point))
			{
				return false;
			}
			ShapeData shapeData = new ShapeData();
			ShapeData shapeData2 = new ShapeData();
			ShapeData shapeData3 = new ShapeData();
			for (int i = 0; i < 6; i++)
			{
				WorldUtils.Gen(origin, new Shapes.Circle(GenBase._random.Next(10, 12) + i), Actions.Chain(new GenAction[]
						{
							new Modifiers.Offset(0, 5 * i + 5),
							new Modifiers.Blotches(3, 0.3).Output(shapeData)
						}));
			}
			for (int j = 0; j < 6; j++)
			{
				WorldUtils.Gen(origin, new Shapes.Circle(GenBase._random.Next(5, 7) + j), Actions.Chain(new GenAction[]
						{
							new Modifiers.Offset(0, 2 * j + 18),
							new Modifiers.Blotches(3, 0.3).Output(shapeData2)
						}));
			}
			for (int k = 0; k < 6; k++)
			{
				WorldUtils.Gen(origin, new Shapes.Circle(GenBase._random.Next(4, 6) + k / 2), Actions.Chain(new GenAction[]
						{
							new Modifiers.Offset(0, (int)(7.5f * (float)k) - 10),
							new Modifiers.Blotches(3, 0.3).Output(shapeData3)
						}));
			}
			ShapeData shapeData4 = new ShapeData(shapeData2);
			shapeData2.Subtract(shapeData3, origin, origin);
			shapeData4.Subtract(shapeData2, origin, origin);
			Rectangle bounds = ShapeData.GetBounds(origin, new ShapeData[]
				{
					shapeData,
					shapeData3
				});
			if (!structures.CanPlace(bounds, CorruptionPitBiome.ValidTiles, 2))
			{
				return false;
			}
			WorldUtils.Gen(origin, new ModShapes.All(shapeData), Actions.Chain(new GenAction[]
					{
						new Actions.SetTile(25, true, true),
						new Actions.PlaceWall(3, true)
					}));
			WorldUtils.Gen(origin, new ModShapes.All(shapeData2), new Actions.SetTile(0, true, true));
			WorldUtils.Gen(origin, new ModShapes.All(shapeData3), new Actions.ClearTile(true));
			WorldUtils.Gen(origin, new ModShapes.All(shapeData2), Actions.Chain(new GenAction[]
					{
						new Modifiers.IsTouchingAir(true),
						new Modifiers.NotTouching(false, new ushort[]
							{
								25
							}),
						new Actions.SetTile(23, true, true)
					}));
			WorldUtils.Gen(origin, new ModShapes.All(shapeData4), new Actions.PlaceWall(69, true));
			structures.AddStructure(bounds, 2);
			return true;
		}
	}
}
