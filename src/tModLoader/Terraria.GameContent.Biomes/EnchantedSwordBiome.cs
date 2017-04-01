using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria.GameContent.Generation;
using Terraria.World.Generation;

namespace Terraria.GameContent.Biomes
{
	public class EnchantedSwordBiome : MicroBiome
	{
		public override bool Place(Point origin, StructureMap structures)
		{
			Dictionary<ushort, int> dictionary = new Dictionary<ushort, int>();
			WorldUtils.Gen(new Point(origin.X - 25, origin.Y - 25), new Shapes.Rectangle(50, 50), new Actions.TileScanner(new ushort[]
					{
						0,
						1
					}).Output(dictionary));
			int num = dictionary[0] + dictionary[1];
			if (num < 1250)
			{
				return false;
			}
			Point point;
			bool flag = WorldUtils.Find(origin, Searches.Chain(new Searches.Up(1000), new GenCondition[]
					{
						new Conditions.IsSolid().AreaOr(1, 50).Not()
					}), out point);
			Point point2;
			bool flag2 = WorldUtils.Find(origin, Searches.Chain(new Searches.Up(origin.Y - point.Y), new GenCondition[]
					{
						new Conditions.IsTile(new ushort[]
							{
								53
							})
					}), out point2);
			if (flag2)
			{
				return false;
			}
			if (!flag)
			{
				return false;
			}
			point.Y += 50;
			ShapeData shapeData = new ShapeData();
			ShapeData shapeData2 = new ShapeData();
			Point point3 = new Point(origin.X, origin.Y + 20);
			Point point4 = new Point(origin.X, origin.Y + 30);
			float num2 = 0.8f + GenBase._random.NextFloat() * 0.5f;
			if (!structures.CanPlace(new Rectangle(point3.X - (int)(20f * num2), point3.Y - 20, (int)(40f * num2), 40), 0))
			{
				return false;
			}
			if (!structures.CanPlace(new Rectangle(origin.X, point.Y + 10, 1, origin.Y - point.Y - 9), 2))
			{
				return false;
			}
			WorldUtils.Gen(point3, new Shapes.Slime(20, num2, 1f), Actions.Chain(new GenAction[]
					{
						new Modifiers.Blotches(2, 0.4),
						new Actions.ClearTile(true).Output(shapeData)
					}));
			WorldUtils.Gen(point4, new Shapes.Mound(14, 14), Actions.Chain(new GenAction[]
					{
						new Modifiers.Blotches(2, 1, 0.8),
						new Actions.SetTile(0, false, true),
						new Actions.SetFrames(true).Output(shapeData2)
					}));
			shapeData.Subtract(shapeData2, point3, point4);
			WorldUtils.Gen(point3, new ModShapes.InnerOutline(shapeData, true), Actions.Chain(new GenAction[]
					{
						new Actions.SetTile(2, false, true),
						new Actions.SetFrames(true)
					}));
			WorldUtils.Gen(point3, new ModShapes.All(shapeData), Actions.Chain(new GenAction[]
					{
						new Modifiers.RectangleMask(-40, 40, 0, 40),
						new Modifiers.IsEmpty(),
						new Actions.SetLiquid(0, 255)
					}));
			WorldUtils.Gen(point3, new ModShapes.All(shapeData), Actions.Chain(new GenAction[]
					{
						new Actions.PlaceWall(68, true),
						new Modifiers.OnlyTiles(new ushort[]
							{
								2
							}),
						new Modifiers.Offset(0, 1),
						new ActionVines(3, 5, 52)
					}));
			ShapeData data = new ShapeData();
			WorldUtils.Gen(new Point(origin.X, point.Y + 10), new Shapes.Rectangle(1, origin.Y - point.Y - 9), Actions.Chain(new GenAction[]
					{
						new Modifiers.Blotches(2, 0.2),
						new Actions.ClearTile(false).Output(data),
						new Modifiers.Expand(1),
						new Modifiers.OnlyTiles(new ushort[]
							{
								53
							}),
						new Actions.SetTile(397, false, true).Output(data)
					}));
			WorldUtils.Gen(new Point(origin.X, point.Y + 10), new ModShapes.All(data), new Actions.SetFrames(true));
			if (GenBase._random.Next(3) == 0)
			{
				WorldGen.PlaceTile(point4.X, point4.Y - 15, 187, true, false, -1, 17);
			}
			else
			{
				WorldGen.PlaceTile(point4.X, point4.Y - 15, 186, true, false, -1, 15);
			}
			WorldUtils.Gen(point4, new ModShapes.All(shapeData2), Actions.Chain(new GenAction[]
					{
						new Modifiers.Offset(0, -1),
						new Modifiers.OnlyTiles(new ushort[]
							{
								2
							}),
						new Modifiers.Offset(0, -1),
						new ActionGrass()
					}));
			structures.AddStructure(new Rectangle(point3.X - (int)(20f * num2), point3.Y - 20, (int)(40f * num2), 40), 4);
			return true;
		}
	}
}
