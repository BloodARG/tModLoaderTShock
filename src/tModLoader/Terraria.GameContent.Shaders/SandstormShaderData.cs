using Microsoft.Xna.Framework;
using System;
using Terraria.Graphics.Shaders;

namespace Terraria.GameContent.Shaders
{
	public class SandstormShaderData : ScreenShaderData
	{
		private Vector2 _texturePosition = Vector2.Zero;

		public SandstormShaderData(string passName)
			: base(passName)
		{
		}

		public override void Update(GameTime gameTime)
		{
			Vector2 vector = new Vector2(-Main.windSpeed, -1f) * new Vector2(20f, 0.1f);
			vector.Normalize();
			vector *= new Vector2(2f, 0.2f);
			if (!Main.gamePaused && Main.hasFocus)
			{
				this._texturePosition += vector * (float)gameTime.ElapsedGameTime.TotalSeconds;
			}
			this._texturePosition.X = this._texturePosition.X % 10f;
			this._texturePosition.Y = this._texturePosition.Y % 10f;
			base.UseDirection(vector);
			base.Update(gameTime);
		}

		public override void Apply()
		{
			base.UseTargetPosition(this._texturePosition);
			base.Apply();
		}
	}
}
