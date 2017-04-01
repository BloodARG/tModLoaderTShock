using Microsoft.Xna.Framework;
using System;
using Terraria.World.Generation;

namespace Terraria.GameContent.Generation
{
	public class ActionVines : GenAction
	{
		private int _minLength;
		private int _maxLength;
		private int _vineId;

		public ActionVines(int minLength = 6, int maxLength = 10, int vineId = 52)
		{
			this._minLength = minLength;
			this._maxLength = maxLength;
			this._vineId = vineId;
		}

		public override bool Apply(Point origin, int x, int y, params object[] args)
		{
			int num = GenBase._random.Next(this._minLength, this._maxLength + 1);
			int num2 = 0;
			while (num2 < num && !GenBase._tiles[x, y + num2].active())
			{
				GenBase._tiles[x, y + num2].type = (ushort)this._vineId;
				GenBase._tiles[x, y + num2].active(true);
				num2++;
			}
			return num2 > 0 && base.UnitApply(origin, x, y, args);
		}
	}
}
