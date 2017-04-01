using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Terraria.GameContent.Events
{
	public class ScreenObstruction
	{
		public static float screenObstruction;

		public static void Update()
		{
			float value = 0f;
			float amount = 0.1f;
			if (Main.player[Main.myPlayer].headcovered)
			{
				value = 0.95f;
				amount = 0.3f;
			}
			ScreenObstruction.screenObstruction = MathHelper.Lerp(ScreenObstruction.screenObstruction, value, amount);
		}

		public static void Draw(SpriteBatch spriteBatch)
		{
			if (ScreenObstruction.screenObstruction == 0f)
			{
				return;
			}
			Color color = Color.Black * ScreenObstruction.screenObstruction;
			int width = Main.extraTexture[49].Width;
			int num = 10;
			Rectangle rect = Main.player[Main.myPlayer].getRect();
			rect.Inflate((width - rect.Width) / 2, (width - rect.Height) / 2 + num / 2);
			rect.Offset(-(int)Main.screenPosition.X, -(int)Main.screenPosition.Y + (int)Main.player[Main.myPlayer].gfxOffY - num);
			Rectangle destinationRectangle = Rectangle.Union(new Rectangle(0, 0, 1, 1), new Rectangle(rect.Right - 1, rect.Top - 1, 1, 1));
			Rectangle destinationRectangle2 = Rectangle.Union(new Rectangle(Main.screenWidth - 1, 0, 1, 1), new Rectangle(rect.Right, rect.Bottom - 1, 1, 1));
			Rectangle destinationRectangle3 = Rectangle.Union(new Rectangle(Main.screenWidth - 1, Main.screenHeight - 1, 1, 1), new Rectangle(rect.Left, rect.Bottom, 1, 1));
			Rectangle destinationRectangle4 = Rectangle.Union(new Rectangle(0, Main.screenHeight - 1, 1, 1), new Rectangle(rect.Left - 1, rect.Top, 1, 1));
			spriteBatch.Draw(Main.magicPixel, destinationRectangle, new Rectangle?(new Rectangle(0, 0, 1, 1)), color);
			spriteBatch.Draw(Main.magicPixel, destinationRectangle2, new Rectangle?(new Rectangle(0, 0, 1, 1)), color);
			spriteBatch.Draw(Main.magicPixel, destinationRectangle3, new Rectangle?(new Rectangle(0, 0, 1, 1)), color);
			spriteBatch.Draw(Main.magicPixel, destinationRectangle4, new Rectangle?(new Rectangle(0, 0, 1, 1)), color);
			spriteBatch.Draw(Main.extraTexture[49], rect, color);
		}
	}
}
