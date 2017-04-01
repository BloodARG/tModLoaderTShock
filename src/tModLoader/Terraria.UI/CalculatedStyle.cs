using Microsoft.Xna.Framework;
using System;

namespace Terraria.UI
{
	public struct CalculatedStyle
	{
		public float X;
		public float Y;
		public float Width;
		public float Height;

		public CalculatedStyle(float x, float y, float width, float height)
		{
			this.X = x;
			this.Y = y;
			this.Width = width;
			this.Height = height;
		}

		public Rectangle ToRectangle()
		{
			return new Rectangle((int)this.X, (int)this.Y, (int)this.Width, (int)this.Height);
		}

		public Vector2 Position()
		{
			return new Vector2(this.X, this.Y);
		}

		public Vector2 Center()
		{
			return new Vector2(this.X + this.Width * 0.5f, this.Y + this.Height * 0.5f);
		}
	}
}
