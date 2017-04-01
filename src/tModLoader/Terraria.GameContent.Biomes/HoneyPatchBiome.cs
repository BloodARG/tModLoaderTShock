using Microsoft.Xna.Framework;
using System;
using Terraria.World.Generation;

namespace Terraria.GameContent.Biomes
{
	public class HoneyPatchBiome : MicroBiome
	{
		public override bool Place(Point origin, StructureMap structures)
		{
			if (GenBase._tiles[origin.X, origin.Y].active() && WorldGen.SolidTile(origin.X, origin.Y))
			{
				return false;
			}
			Point origin2;
			if (!WorldUtils.Find(origin, Searches.Chain(new Searches.Down(80), new GenCondition[]
					{
						new Conditions.IsSolid()
					}), out origin2))
			{
				return false;
			}
			origin2.Y += 2;
			Ref<int> @ref = new Ref<int>(0);
			WorldUtils.Gen(origin2, new Shapes.Circle(8), Actions.Chain(new GenAction[]
					{
						new Modifiers.IsSolid(),
						new Actions.Scanner(@ref)
					}));
			if (@ref.Value < 20)
			{
				return false;
			}
			if (!structures.CanPlace(new Rectangle(origin2.X - 8, origin2.Y - 8, 16, 16), 0))
			{
				return false;
			}
			WorldUtils.Gen(origin2, new Shapes.Circle(8), Actions.Chain(new GenAction[]
					{
						new Modifiers.RadialDither(0f, 10f),
						new Modifiers.IsSolid(),
						new Actions.SetTile(229, true, true)
					}));
			ShapeData data = new ShapeData();
			WorldUtils.Gen(origin2, new Shapes.Circle(4, 3), Actions.Chain(new GenAction[]
					{
						new Modifiers.Blotches(2, 0.3),
						new Modifiers.IsSolid(),
						new Actions.ClearTile(true),
						new Modifiers.RectangleMask(-6, 6, 0, 3).Output(data),
						new Actions.SetLiquid(2, 255)
					}));
			WorldUtils.Gen(new Point(origin2.X, origin2.Y + 1), new ModShapes.InnerOutline(data, true), Actions.Chain(new GenAction[]
					{
						new Modifiers.IsEmpty(),
						new Modifiers.RectangleMask(-6, 6, 1, 3),
						new Actions.SetTile(59, true, true)
					}));
			structures.AddStructure(new Rectangle(origin2.X - 8, origin2.Y - 8, 16, 16), 0);
			return true;
		}
	}
}
