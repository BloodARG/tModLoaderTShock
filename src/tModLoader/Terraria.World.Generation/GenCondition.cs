using System;

namespace Terraria.World.Generation
{
	public abstract class GenCondition : GenBase
	{
		private enum AreaType
		{
			And,
			Or,
			None
		}

		private bool InvertResults;
		private int _width;
		private int _height;
		private GenCondition.AreaType _areaType = GenCondition.AreaType.None;

		public bool IsValid(int x, int y)
		{
			switch (this._areaType)
			{
				case GenCondition.AreaType.And:
					for (int i = x; i < x + this._width; i++)
					{
						for (int j = y; j < y + this._height; j++)
						{
							if (!this.CheckValidity(i, j))
							{
								return this.InvertResults;
							}
						}
					}
					return !this.InvertResults;
				case GenCondition.AreaType.Or:
					for (int k = x; k < x + this._width; k++)
					{
						for (int l = y; l < y + this._height; l++)
						{
							if (this.CheckValidity(k, l))
							{
								return !this.InvertResults;
							}
						}
					}
					return this.InvertResults;
				case GenCondition.AreaType.None:
					return this.CheckValidity(x, y) ^ this.InvertResults;
				default:
					return true;
			}
		}

		public GenCondition Not()
		{
			this.InvertResults = !this.InvertResults;
			return this;
		}

		public GenCondition AreaOr(int width, int height)
		{
			this._areaType = GenCondition.AreaType.Or;
			this._width = width;
			this._height = height;
			return this;
		}

		public GenCondition AreaAnd(int width, int height)
		{
			this._areaType = GenCondition.AreaType.And;
			this._width = width;
			this._height = height;
			return this;
		}

		protected abstract bool CheckValidity(int x, int y);
	}
}
