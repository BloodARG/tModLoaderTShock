using Microsoft.Xna.Framework;
using System;
using Terraria.DataStructures;
using Terraria.World.Generation;

namespace Terraria.GameContent.Generation
{
	public class ActionPlaceStatue : GenAction
	{
		private int _statueIndex;

		public ActionPlaceStatue(int index = -1)
		{
			this._statueIndex = index;
		}

		public override bool Apply(Point origin, int x, int y, params object[] args)
		{
			Point16 point;
			if (this._statueIndex == -1)
			{
				point = WorldGen.statueList[GenBase._random.Next(2, WorldGen.statueList.Length)];
			}
			else
			{
				point = WorldGen.statueList[this._statueIndex];
			}
			WorldGen.PlaceTile(x, y, (int)point.X, true, false, -1, (int)point.Y);
			return base.UnitApply(origin, x, y, args);
		}
	}
}
