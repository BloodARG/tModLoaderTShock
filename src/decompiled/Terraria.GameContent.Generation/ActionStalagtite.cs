using Microsoft.Xna.Framework;
using System;
using Terraria.World.Generation;

namespace Terraria.GameContent.Generation
{
	public class ActionStalagtite : GenAction
	{
		public override bool Apply(Point origin, int x, int y, params object[] args)
		{
			WorldGen.PlaceTight(x, y, 165, false);
			return base.UnitApply(origin, x, y, args);
		}
	}
}
