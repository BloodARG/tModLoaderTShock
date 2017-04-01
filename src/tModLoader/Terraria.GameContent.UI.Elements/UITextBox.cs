using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.UI;

namespace Terraria.GameContent.UI.Elements
{
	public class UITextBox : UITextPanel<string>
	{
		private int _cursor;
		private int _frameCount;
		private int _maxLength = 20;

		public UITextBox(string text, float textScale = 1f, bool large = false)
			: base(text, textScale, large)
		{
		}

		public void Write(string text)
		{
			base.SetText(base.Text.Insert(this._cursor, text));
			this._cursor += text.Length;
		}

		public override void SetText(string text, float textScale, bool large)
		{
			if (text.ToString().Length > this._maxLength)
			{
				text = text.ToString().Substring(0, this._maxLength);
			}
			base.SetText(text, textScale, large);
			this._cursor = Math.Min(base.Text.Length, this._cursor);
		}

		public void SetTextMaxLength(int maxLength)
		{
			this._maxLength = maxLength;
		}

		public void Backspace()
		{
			if (this._cursor == 0)
			{
				return;
			}
			base.SetText(base.Text.Substring(0, base.Text.Length - 1));
		}

		public void CursorLeft()
		{
			if (this._cursor == 0)
			{
				return;
			}
			this._cursor--;
		}

		public void CursorRight()
		{
			if (this._cursor < base.Text.Length)
			{
				this._cursor++;
			}
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			this._cursor = base.Text.Length;
			base.DrawSelf(spriteBatch);
			this._frameCount++;
			if ((this._frameCount %= 40) > 20)
			{
				return;
			}
			CalculatedStyle innerDimensions = base.GetInnerDimensions();
			Vector2 pos = innerDimensions.Position();
			SpriteFont spriteFont = base.IsLarge ? Main.fontDeathText : Main.fontMouseText;
			Vector2 vector = new Vector2(spriteFont.MeasureString(base.Text.Substring(0, this._cursor)).X, base.IsLarge ? 32f : 16f) * base.TextScale;
			if (base.IsLarge)
			{
				pos.Y -= 8f * base.TextScale;
			}
			else
			{
				pos.Y += 2f * base.TextScale;
			}
			pos.X += (innerDimensions.Width - base.TextSize.X) * 0.5f + vector.X - (base.IsLarge ? 8f : 4f) * base.TextScale + 6f;
			if (base.IsLarge)
			{
				Utils.DrawBorderStringBig(spriteBatch, "|", pos, base.TextColor, base.TextScale, 0f, 0f, -1);
				return;
			}
			Utils.DrawBorderString(spriteBatch, "|", pos, base.TextColor, base.TextScale, 0f, 0f, -1);
		}
	}
}
