using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.Graphics;
using Terraria.UI;

namespace Terraria.GameContent.UI.Elements
{
	public class UIPanel : UIElement
	{
		private const int CORNER_SIZE = 12;
		private const int TEXTURE_PADDING = 3;
		private const int BAR_SIZE = 2;
		private static Texture2D _borderTexture;
		private static Texture2D _backgroundTexture;
		public Color BorderColor = Color.Black;
		public Color BackgroundColor = new Color(63, 82, 151) * 0.7f;

		public UIPanel()
		{
			if (UIPanel._borderTexture == null)
			{
				UIPanel._borderTexture = TextureManager.Load("Images/UI/PanelBorder");
			}
			if (UIPanel._backgroundTexture == null)
			{
				UIPanel._backgroundTexture = TextureManager.Load("Images/UI/PanelBackground");
			}
			base.SetPadding(12f);
		}

		private void DrawPanel(SpriteBatch spriteBatch, Texture2D texture, Color color)
		{
			CalculatedStyle dimensions = base.GetDimensions();
			Point point = new Point((int)dimensions.X, (int)dimensions.Y);
			Point point2 = new Point((int)((float)point.X + dimensions.Width) - 12, (int)Math.Ceiling((double)((float)point.Y + dimensions.Height)) - 12);
			int width = (int)Math.Ceiling((double)dimensions.Width) - 24;
			int height = (int)Math.Ceiling((double)dimensions.Height) - 24;
			spriteBatch.Draw(texture, new Rectangle(point.X, point.Y, 12, 12), new Rectangle?(new Rectangle(0, 0, 12, 12)), color);
			spriteBatch.Draw(texture, new Rectangle(point2.X, point.Y, 12, 12), new Rectangle?(new Rectangle(20, 0, 12, 12)), color);
			spriteBatch.Draw(texture, new Rectangle(point.X, point2.Y, 12, 12), new Rectangle?(new Rectangle(0, 20, 12, 12)), color);
			spriteBatch.Draw(texture, new Rectangle(point2.X, point2.Y, 12, 12), new Rectangle?(new Rectangle(20, 20, 12, 12)), color);
			spriteBatch.Draw(texture, new Rectangle(point.X + 12, point.Y, width, 12), new Rectangle?(new Rectangle(15, 0, 2, 12)), color);
			spriteBatch.Draw(texture, new Rectangle(point.X + 12, point2.Y, width, 12), new Rectangle?(new Rectangle(15, 20, 2, 12)), color);
			spriteBatch.Draw(texture, new Rectangle(point.X, point.Y + 12, 12, height), new Rectangle?(new Rectangle(0, 15, 12, 2)), color);
			spriteBatch.Draw(texture, new Rectangle(point2.X, point.Y + 12, 12, height), new Rectangle?(new Rectangle(20, 15, 12, 2)), color);
			spriteBatch.Draw(texture, new Rectangle(point.X + 12, point.Y + 12, width, height), new Rectangle?(new Rectangle(15, 15, 2, 2)), color);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			this.DrawPanel(spriteBatch, UIPanel._backgroundTexture, this.BackgroundColor);
			this.DrawPanel(spriteBatch, UIPanel._borderTexture, this.BorderColor);
		}
	}
}
