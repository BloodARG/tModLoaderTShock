using Microsoft.Xna.Framework;
using System;

namespace Terraria.World.Generation
{
	public static class Shapes
	{
		public class Circle : GenShape
		{
			private int _verticalRadius;
			private int _horizontalRadius;

			public Circle(int radius)
			{
				this._verticalRadius = radius;
				this._horizontalRadius = radius;
			}

			public Circle(int horizontalRadius, int verticalRadius)
			{
				this._horizontalRadius = horizontalRadius;
				this._verticalRadius = verticalRadius;
			}

			public override bool Perform(Point origin, GenAction action)
			{
				int num = (this._horizontalRadius + 1) * (this._horizontalRadius + 1);
				for (int i = origin.Y - this._verticalRadius; i <= origin.Y + this._verticalRadius; i++)
				{
					float num2 = (float)this._horizontalRadius / (float)this._verticalRadius * (float)(i - origin.Y);
					int num3 = Math.Min(this._horizontalRadius, (int)Math.Sqrt((double)((float)num - num2 * num2)));
					for (int j = origin.X - num3; j <= origin.X + num3; j++)
					{
						if (!base.UnitApply(action, origin, j, i, new object[0]) && this._quitOnFail)
						{
							return false;
						}
					}
				}
				return true;
			}
		}

		public class HalfCircle : GenShape
		{
			private int _radius;

			public HalfCircle(int radius)
			{
				this._radius = radius;
			}

			public override bool Perform(Point origin, GenAction action)
			{
				int num = (this._radius + 1) * (this._radius + 1);
				for (int i = origin.Y - this._radius; i <= origin.Y; i++)
				{
					int num2 = Math.Min(this._radius, (int)Math.Sqrt((double)(num - (i - origin.Y) * (i - origin.Y))));
					for (int j = origin.X - num2; j <= origin.X + num2; j++)
					{
						if (!base.UnitApply(action, origin, j, i, new object[0]) && this._quitOnFail)
						{
							return false;
						}
					}
				}
				return true;
			}
		}

		public class Slime : GenShape
		{
			private int _radius;
			private float _xScale;
			private float _yScale;

			public Slime(int radius)
			{
				this._radius = radius;
				this._xScale = 1f;
				this._yScale = 1f;
			}

			public Slime(int radius, float xScale, float yScale)
			{
				this._radius = radius;
				this._xScale = xScale;
				this._yScale = yScale;
			}

			public override bool Perform(Point origin, GenAction action)
			{
				float num = (float)this._radius;
				int num2 = (this._radius + 1) * (this._radius + 1);
				for (int i = origin.Y - (int)(num * this._yScale); i <= origin.Y; i++)
				{
					float num3 = (float)(i - origin.Y) / this._yScale;
					int num4 = (int)Math.Min((float)this._radius * this._xScale, this._xScale * (float)Math.Sqrt((double)((float)num2 - num3 * num3)));
					for (int j = origin.X - num4; j <= origin.X + num4; j++)
					{
						if (!base.UnitApply(action, origin, j, i, new object[0]) && this._quitOnFail)
						{
							return false;
						}
					}
				}
				for (int k = origin.Y + 1; k <= origin.Y + (int)(num * this._yScale * 0.5f) - 1; k++)
				{
					float num5 = (float)(k - origin.Y) * (2f / this._yScale);
					int num6 = (int)Math.Min((float)this._radius * this._xScale, this._xScale * (float)Math.Sqrt((double)((float)num2 - num5 * num5)));
					for (int l = origin.X - num6; l <= origin.X + num6; l++)
					{
						if (!base.UnitApply(action, origin, l, k, new object[0]) && this._quitOnFail)
						{
							return false;
						}
					}
				}
				return true;
			}
		}

		public class Rectangle : GenShape
		{
			private int _width;
			private int _height;

			public Rectangle(int width, int height)
			{
				this._width = width;
				this._height = height;
			}

			public override bool Perform(Point origin, GenAction action)
			{
				for (int i = origin.X; i < origin.X + this._width; i++)
				{
					for (int j = origin.Y; j < origin.Y + this._height; j++)
					{
						if (!base.UnitApply(action, origin, i, j, new object[0]) && this._quitOnFail)
						{
							return false;
						}
					}
				}
				return true;
			}
		}

		public class Tail : GenShape
		{
			private float _width;
			private Vector2 _endOffset;

			public Tail(float width, Vector2 endOffset)
			{
				this._width = width * 16f;
				this._endOffset = endOffset * 16f;
			}

			public override bool Perform(Point origin, GenAction action)
			{
				Vector2 vector = new Vector2((float)(origin.X << 4), (float)(origin.Y << 4));
				return Utils.PlotTileTale(vector, vector + this._endOffset, this._width, (int x, int y) => this.UnitApply(action, origin, x, y, new object[0]) || !this._quitOnFail);
			}
		}

		public class Mound : GenShape
		{
			private int _halfWidth;
			private int _height;

			public Mound(int halfWidth, int height)
			{
				this._halfWidth = halfWidth;
				this._height = height;
			}

			public override bool Perform(Point origin, GenAction action)
			{
				float num = (float)this._halfWidth;
				for (int i = -this._halfWidth; i <= this._halfWidth; i++)
				{
					int num2 = Math.Min(this._height, (int)(-((float)(this._height + 1) / (num * num)) * ((float)i + num) * ((float)i - num)));
					for (int j = 0; j < num2; j++)
					{
						if (!base.UnitApply(action, origin, i + origin.X, origin.Y - j, new object[0]) && this._quitOnFail)
						{
							return false;
						}
					}
				}
				return true;
			}
		}
	}
}
