using Microsoft.Xna.Framework;
using System;
using Terraria.World.Generation;

namespace Terraria.GameContent.Generation
{
	public class ShapeRunner : GenShape
	{
		private float _startStrength;
		private int _steps;
		private Vector2 _startVelocity;

		public ShapeRunner(float strength, int steps, Vector2 velocity)
		{
			this._startStrength = strength;
			this._steps = steps;
			this._startVelocity = velocity;
		}

		public override bool Perform(Point origin, GenAction action)
		{
			float num = (float)this._steps;
			float num2 = (float)this._steps;
			double num3 = (double)this._startStrength;
			Vector2 value = new Vector2((float)origin.X, (float)origin.Y);
			Vector2 vector = (this._startVelocity == Vector2.Zero) ? Utils.RandomVector2(GenBase._random, -1f, 1f) : this._startVelocity;
			while (num > 0f && num3 > 0.0)
			{
				num3 = (double)(this._startStrength * (num / num2));
				num -= 1f;
				int num4 = Math.Max(1, (int)((double)value.X - num3 * 0.5));
				int num5 = Math.Max(1, (int)((double)value.Y - num3 * 0.5));
				int num6 = Math.Min(GenBase._worldWidth, (int)((double)value.X + num3 * 0.5));
				int num7 = Math.Min(GenBase._worldHeight, (int)((double)value.Y + num3 * 0.5));
				for (int i = num4; i < num6; i++)
				{
					for (int j = num5; j < num7; j++)
					{
						if ((double)(Math.Abs((float)i - value.X) + Math.Abs((float)j - value.Y)) < num3 * 0.5 * (1.0 + (double)GenBase._random.Next(-10, 11) * 0.015))
						{
							base.UnitApply(action, origin, i, j, new object[0]);
						}
					}
				}
				int num8 = (int)(num3 / 50.0) + 1;
				num -= (float)num8;
				value += vector;
				for (int k = 0; k < num8; k++)
				{
					value += vector;
					vector += Utils.RandomVector2(GenBase._random, -0.5f, 0.5f);
				}
				vector += Utils.RandomVector2(GenBase._random, -0.5f, 0.5f);
				vector = Vector2.Clamp(vector, -Vector2.One, Vector2.One);
			}
			return true;
		}
	}
}
