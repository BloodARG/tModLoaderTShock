using Microsoft.Xna.Framework;
using System;
using Terraria.World.Generation;

namespace Terraria.GameContent.Generation
{
	public class ActionGrass : GenAction
	{
		public override bool Apply(Point origin, int x, int y, params object[] args)
		{
			if (GenBase._tiles[x, y].active() || GenBase._tiles[x, y - 1].active())
			{
				return false;
			}
			WorldGen.PlaceTile(x, y, (int)Utils.SelectRandom<ushort>(GenBase._random, new ushort[]
					{
						3,
						73
					}), true, false, -1, 0);
			return base.UnitApply(origin, x, y, args);
		}
	}
}
