using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Terraria.GameContent.UI.Elements
{
	public class UIKeybindingSimpleListItem : UIElement
	{
		private Color _color;
		private Func<string> _GetTextFunction;

		public UIKeybindingSimpleListItem(Func<string> getText, Color color)
		{
			this._color = color;
			Func<string> arg_31_1;
			if (getText == null)
			{
				arg_31_1 = (() => "???");
			}
			else
			{
				arg_31_1 = getText;
			}
			this._GetTextFunction = arg_31_1;
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			float num = 6f;
			base.DrawSelf(spriteBatch);
			CalculatedStyle dimensions = base.GetDimensions();
			float num2 = dimensions.Width + 1f;
			Vector2 vector = new Vector2(dimensions.X, dimensions.Y);
			Vector2 baseScale = new Vector2(0.8f);
			Color color = base.IsMouseHovering ? Color.White : Color.Silver;
			color = Color.Lerp(color, Color.White, base.IsMouseHovering ? 0.5f : 0f);
			Color color2 = base.IsMouseHovering ? this._color : this._color.MultiplyRGBA(new Color(180, 180, 180));
			Vector2 position = vector;
			Utils.DrawSettings2Panel(spriteBatch, position, num2, color2);
			position.X += 8f;
			position.Y += 2f + num;
			string text = this._GetTextFunction();
			Vector2 stringSize = ChatManager.GetStringSize(Main.fontItemStack, text, baseScale, -1f);
			position.X = dimensions.X + dimensions.Width / 2f - stringSize.X / 2f;
			ChatManager.DrawColorCodedStringWithShadow(spriteBatch, Main.fontItemStack, text, position, color, 0f, Vector2.Zero, baseScale, num2, 2f);
		}
	}
}
