using Microsoft.Xna.Framework;
using System;
using Terraria.Graphics.Shaders;

namespace Terraria.GameContent.Shaders
{
	public class BlizzardShaderData : ScreenShaderData
	{
		private Vector2 _texturePosition = Vector2.Zero;
		private float windSpeed = 0.1f;

		public BlizzardShaderData(string passName)
			: base(passName)
		{
		}

		public override void Update(GameTime gameTime)
		{
			float num = Main.windSpeed;
			if (num >= 0f && num <= 0.1f)
			{
				num = 0.1f;
			}
			else if (num <= 0f && num >= -0.1f)
			{
				num = -0.1f;
			}
			this.windSpeed = num * 0.05f + this.windSpeed * 0.95f;
			Vector2 vector = new Vector2(-this.windSpeed, -1f) * new Vector2(10f, 2f);
			vector.Normalize();
			vector *= new Vector2(0.8f, 0.6f);
			if (!Main.gamePaused && Main.hasFocus)
			{
				this._texturePosition += vector * (float)gameTime.ElapsedGameTime.TotalSeconds;
			}
			this._texturePosition.X = this._texturePosition.X % 10f;
			this._texturePosition.Y = this._texturePosition.Y % 10f;
			base.UseDirection(vector);
			base.UseTargetPosition(this._texturePosition);
			base.Update(gameTime);
		}

		public override void Apply()
		{
			base.UseTargetPosition(this._texturePosition);
			base.Apply();
		}
	}
}
