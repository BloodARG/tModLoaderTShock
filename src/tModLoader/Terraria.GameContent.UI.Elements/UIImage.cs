using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.UI;

namespace Terraria.GameContent.UI.Elements
{
	public class UIImage : UIElement
	{
		private Texture2D _texture;
		public float ImageScale = 1f;

		public UIImage(Texture2D texture)
		{
			this._texture = texture;
			this.Width.Set((float)this._texture.Width, 0f);
			this.Height.Set((float)this._texture.Height, 0f);
		}

		public void SetImage(Texture2D texture)
		{
			this._texture = texture;
			this.Width.Set((float)this._texture.Width, 0f);
			this.Height.Set((float)this._texture.Height, 0f);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			CalculatedStyle dimensions = base.GetDimensions();
			spriteBatch.Draw(this._texture, dimensions.Position() + this._texture.Size() * (1f - this.ImageScale) / 2f, null, Color.White, 0f, Vector2.Zero, this.ImageScale, SpriteEffects.None, 0f);
		}
	}
}
