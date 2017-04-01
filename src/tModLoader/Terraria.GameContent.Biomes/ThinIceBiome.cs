using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria.World.Generation;

namespace Terraria.GameContent.Biomes
{
	public class ThinIceBiome : MicroBiome
	{
		public override bool Place(Point origin, StructureMap structures)
		{
			Dictionary<ushort, int> dictionary = new Dictionary<ushort, int>();
			WorldUtils.Gen(new Point(origin.X - 25, origin.Y - 25), new Shapes.Rectangle(50, 50), new Actions.TileScanner(new ushort[]
					{
						0,
						59,
						147,
						1
					}).Output(dictionary));
			int num = dictionary[0] + dictionary[1];
			int num2 = dictionary[59];
			int num3 = dictionary[147];
			if (num3 <= num2 || num3 <= num)
			{
				return false;
			}
			int num4 = 0;
			for (int i = GenBase._random.Next(10, 15); i > 5; i--)
			{
				int num5 = GenBase._random.Next(-5, 5);
				WorldUtils.Gen(new Point(origin.X + num5, origin.Y + num4), new Shapes.Circle(i), Actions.Chain(new GenAction[]
						{
							new Modifiers.Blotches(4, 0.3),
							new Modifiers.OnlyTiles(new ushort[]
								{
									147,
									161,
									224,
									0,
									1
								}),
							new Actions.SetTile(162, true, true)
						}));
				WorldUtils.Gen(new Point(origin.X + num5, origin.Y + num4), new Shapes.Circle(i), Actions.Chain(new GenAction[]
						{
							new Modifiers.Blotches(4, 0.3),
							new Modifiers.HasLiquid(-1, -1),
							new Actions.SetTile(162, true, true),
							new Actions.SetLiquid(0, 0)
						}));
				num4 += i - 2;
			}
			return true;
		}
	}
}
