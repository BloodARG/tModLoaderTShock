using Microsoft.Xna.Framework;
using System;

namespace Terraria.World.Generation
{
	public abstract class GenSearch : GenBase
	{
		public static Point NOT_FOUND = new Point(2147483647, 2147483647);
		private bool _requireAll = true;
		private GenCondition[] _conditions;

		public GenSearch Conditions(params GenCondition[] conditions)
		{
			this._conditions = conditions;
			return this;
		}

		public abstract Point Find(Point origin);

		protected bool Check(int x, int y)
		{
			for (int i = 0; i < this._conditions.Length; i++)
			{
				if (this._requireAll ^ this._conditions[i].IsValid(x, y))
				{
					return !this._requireAll;
				}
			}
			return this._requireAll;
		}

		public GenSearch RequireAll(bool mode)
		{
			this._requireAll = mode;
			return this;
		}
	}
}
