using Microsoft.Xna.Framework;
using System;

namespace Terraria.World.Generation
{
	public abstract class GenAction : GenBase
	{
		public GenAction NextAction;
		public ShapeData OutputData;
		private bool _returnFalseOnFailure = true;

		public abstract bool Apply(Point origin, int x, int y, params object[] args);

		protected bool UnitApply(Point origin, int x, int y, params object[] args)
		{
			if (this.OutputData != null)
			{
				this.OutputData.Add(x - origin.X, y - origin.Y);
			}
			return this.NextAction == null || this.NextAction.Apply(origin, x, y, args);
		}

		public GenAction IgnoreFailures()
		{
			this._returnFalseOnFailure = false;
			return this;
		}

		protected bool Fail()
		{
			return !this._returnFalseOnFailure;
		}

		public GenAction Output(ShapeData data)
		{
			this.OutputData = data;
			return this;
		}
	}
}
