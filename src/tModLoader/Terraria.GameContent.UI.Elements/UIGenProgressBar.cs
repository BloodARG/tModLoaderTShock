using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.Graphics;
using Terraria.UI;

namespace Terraria.GameContent.UI.Elements
{
	public class UIGenProgressBar : UIElement
	{
		private Texture2D _texInnerDirt;
		private Texture2D _texOuterCrimson;
		private Texture2D _texOuterCorrupt;
		private Texture2D _texOuterLower;
		private float _visualOverallProgress;
		private float _targetOverallProgress;
		private float _visualCurrentProgress;
		private float _targetCurrentProgress;

		public UIGenProgressBar()
		{
			if (Main.netMode != 2)
			{
				this._texInnerDirt = TextureManager.Load("Images/UI/WorldGen/Outer Dirt");
				this._texOuterCorrupt = TextureManager.Load("Images/UI/WorldGen/Outer Corrupt");
				this._texOuterCrimson = TextureManager.Load("Images/UI/WorldGen/Outer Crimson");
				this._texOuterLower = TextureManager.Load("Images/UI/WorldGen/Outer Lower");
			}
			this.Recalculate();
		}

		public override void Recalculate()
		{
			this.Width.Precent = 0f;
			this.Height.Precent = 0f;
			this.Width.Pixels = 612f;
			this.Height.Pixels = 70f;
			base.Recalculate();
		}

		public void SetProgress(float overallProgress, float currentProgress)
		{
			this._targetCurrentProgress = currentProgress;
			this._targetOverallProgress = overallProgress;
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			this._visualOverallProgress = this._targetOverallProgress;
			this._visualCurrentProgress = this._targetCurrentProgress;
			CalculatedStyle dimensions = base.GetDimensions();
			int completedWidth = (int)(this._visualOverallProgress * 504f);
			int completedWidth2 = (int)(this._visualCurrentProgress * 504f);
			Vector2 value = new Vector2(dimensions.X, dimensions.Y);
			Color color = default(Color);
			color.PackedValue = (WorldGen.crimson ? 4286836223u : 4283888223u);
			this.DrawFilling2(spriteBatch, value + new Vector2(20f, 40f), 16, completedWidth, 564, color, Color.Lerp(color, Color.Black, 0.5f), new Color(48, 48, 48));
			color.PackedValue = 4290947159u;
			this.DrawFilling2(spriteBatch, value + new Vector2(50f, 60f), 8, completedWidth2, 504, color, Color.Lerp(color, Color.Black, 0.5f), new Color(33, 33, 33));
			Rectangle r = base.GetDimensions().ToRectangle();
			r.X -= 8;
			spriteBatch.Draw(WorldGen.crimson ? this._texOuterCrimson : this._texOuterCorrupt, r.TopLeft(), Color.White);
			spriteBatch.Draw(this._texOuterLower, r.TopLeft() + new Vector2(44f, 60f), Color.White);
		}

		private void DrawFilling(SpriteBatch spritebatch, Texture2D tex, Texture2D texShadow, Vector2 topLeft, int completedWidth, int totalWidth, Color separator, Color empty)
		{
			if (completedWidth % 2 != 0)
			{
				completedWidth--;
			}
			Vector2 position = topLeft + (float)completedWidth * Vector2.UnitX;
			int i = completedWidth;
			Rectangle value = tex.Frame(1, 1, 0, 0);
			while (i > 0)
			{
				if (value.Width > i)
				{
					value.X += value.Width - i;
					value.Width = i;
				}
				spritebatch.Draw(tex, position, new Rectangle?(value), Color.White, 0f, new Vector2((float)value.Width, 0f), 1f, SpriteEffects.None, 0f);
				position.X -= (float)value.Width;
				i -= value.Width;
			}
			if (texShadow != null)
			{
				spritebatch.Draw(texShadow, topLeft, new Rectangle?(new Rectangle(0, 0, completedWidth, texShadow.Height)), Color.White);
			}
			spritebatch.Draw(Main.magicPixel, new Rectangle((int)topLeft.X + completedWidth, (int)topLeft.Y, totalWidth - completedWidth, tex.Height), new Rectangle?(new Rectangle(0, 0, 1, 1)), empty);
			spritebatch.Draw(Main.magicPixel, new Rectangle((int)topLeft.X + completedWidth - 2, (int)topLeft.Y, 2, tex.Height), new Rectangle?(new Rectangle(0, 0, 1, 1)), separator);
		}

		private void DrawFilling2(SpriteBatch spritebatch, Vector2 topLeft, int height, int completedWidth, int totalWidth, Color filled, Color separator, Color empty)
		{
			if (completedWidth % 2 != 0)
			{
				completedWidth--;
			}
			spritebatch.Draw(Main.magicPixel, new Rectangle((int)topLeft.X, (int)topLeft.Y, completedWidth, height), new Rectangle?(new Rectangle(0, 0, 1, 1)), filled);
			spritebatch.Draw(Main.magicPixel, new Rectangle((int)topLeft.X + completedWidth, (int)topLeft.Y, totalWidth - completedWidth, height), new Rectangle?(new Rectangle(0, 0, 1, 1)), empty);
			spritebatch.Draw(Main.magicPixel, new Rectangle((int)topLeft.X + completedWidth - 2, (int)topLeft.Y, 2, height), new Rectangle?(new Rectangle(0, 0, 1, 1)), separator);
		}
	}
}
