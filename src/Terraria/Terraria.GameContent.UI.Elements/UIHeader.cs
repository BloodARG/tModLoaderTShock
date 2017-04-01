using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.UI;

namespace Terraria.GameContent.UI.Elements
{
	public class UIHeader : UIElement
	{
		private string _text;

		public string Text
		{
			get
			{
				return this._text;
			}
			set
			{
				if (this._text != value)
				{
					this._text = value;
#if CLIENT
					Vector2 vector = Main.fontDeathText.MeasureString(this.Text);
					this.Width.Pixels = vector.X;
					this.Height.Pixels = vector.Y;
#endif
					this.Width.Precent = 0f;
					this.Height.Precent = 0f;
					this.Recalculate();
				}
			}
		}

		public UIHeader()
		{
			this.Text = "";
		}

		public UIHeader(string text)
		{
			this.Text = text;
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			CalculatedStyle dimensions = base.GetDimensions();
			spriteBatch.DrawString(Main.fontDeathText, this.Text, new Vector2(dimensions.X, dimensions.Y), Color.White);
		}
	}
}
