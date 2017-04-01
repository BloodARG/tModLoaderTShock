using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.UI;

namespace Terraria.GameContent.UI.Elements
{
	public class UITextPanel<T> : UIPanel
	{
		private T _text = default(T);
		private float _textScale = 1f;
		private Vector2 _textSize = Vector2.Zero;
		private bool _isLarge;
		private Color _color = Color.White;
		private bool _drawPanel = true;

		public bool IsLarge
		{
			get
			{
				return this._isLarge;
			}
		}

		public bool DrawPanel
		{
			get
			{
				return this._drawPanel;
			}
			set
			{
				this._drawPanel = value;
			}
		}

		public float TextScale
		{
			get
			{
				return this._textScale;
			}
			set
			{
				this._textScale = value;
			}
		}

		public Vector2 TextSize
		{
			get
			{
				return this._textSize;
			}
		}

		public string Text
		{
			get
			{
				if (this._text != null)
				{
					return this._text.ToString();
				}
				return "";
			}
		}

		public Color TextColor
		{
			get
			{
				return this._color;
			}
			set
			{
				this._color = value;
			}
		}

		public UITextPanel(T text, float textScale = 1f, bool large = false)
		{
			this.SetText(text, textScale, large);
		}

		public override void Recalculate()
		{
			this.SetText(this._text, this._textScale, this._isLarge);
			base.Recalculate();
		}

		public void SetText(T text)
		{
			this.SetText(text, this._textScale, this._isLarge);
		}

		public virtual void SetText(T text, float textScale, bool large)
		{
			SpriteFont spriteFont = large ? Main.fontDeathText : Main.fontMouseText;
			Vector2 textSize = new Vector2(spriteFont.MeasureString(text.ToString()).X, large ? 32f : 16f) * textScale;
			this._text = text;
			this._textScale = textScale;
			this._textSize = textSize;
			this._isLarge = large;
			this.MinWidth.Set(textSize.X + this.PaddingLeft + this.PaddingRight, 0f);
			this.MinHeight.Set(textSize.Y + this.PaddingTop + this.PaddingBottom, 0f);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			if (this._drawPanel)
			{
				base.DrawSelf(spriteBatch);
			}
			CalculatedStyle innerDimensions = base.GetInnerDimensions();
			Vector2 pos = innerDimensions.Position();
			if (this._isLarge)
			{
				pos.Y -= 10f * this._textScale * this._textScale;
			}
			else
			{
				pos.Y -= 2f * this._textScale;
			}
			pos.X += (innerDimensions.Width - this._textSize.X) * 0.5f;
			if (this._isLarge)
			{
				Utils.DrawBorderStringBig(spriteBatch, this.Text, pos, this._color, this._textScale, 0f, 0f, -1);
				return;
			}
			Utils.DrawBorderString(spriteBatch, this.Text, pos, this._color, this._textScale, 0f, 0f, -1);
		}
	}
}
