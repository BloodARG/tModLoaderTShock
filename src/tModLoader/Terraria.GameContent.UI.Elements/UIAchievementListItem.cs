using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.Achievements;
using Terraria.Graphics;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Terraria.GameContent.UI.Elements
{
	public class UIAchievementListItem : UIPanel
	{
		private const int _iconSize = 64;
		private const int _iconSizeWithSpace = 66;
		private const int _iconsPerRow = 8;
		private Achievement _achievement;
		private UIImageFramed _achievementIcon;
		private UIImage _achievementIconBorders;
		private int _iconIndex;
		private Rectangle _iconFrame;
		private Rectangle _iconFrameUnlocked;
		private Rectangle _iconFrameLocked;
		private Texture2D _innerPanelTopTexture;
		private Texture2D _innerPanelBottomTexture;
		private Texture2D _categoryTexture;
		private bool _locked;

		public UIAchievementListItem(Achievement achievement)
		{
			this.BackgroundColor = new Color(26, 40, 89) * 0.8f;
			this.BorderColor = new Color(13, 20, 44) * 0.8f;
			this._achievement = achievement;
			this.Height.Set(82f, 0f);
			this.Width.Set(0f, 1f);
			this.PaddingTop = 8f;
			this.PaddingLeft = 9f;
			int iconIndex = Main.Achievements.GetIconIndex(achievement.Name);
			this._iconIndex = iconIndex;
			this._iconFrameUnlocked = new Rectangle(iconIndex % 8 * 66, iconIndex / 8 * 66, 64, 64);
			this._iconFrameLocked = this._iconFrameUnlocked;
			this._iconFrameLocked.X = this._iconFrameLocked.X + 528;
			this._iconFrame = this._iconFrameLocked;
			this.UpdateIconFrame();
			this._achievementIcon = new UIImageFramed(TextureManager.Load("Images/UI/Achievements"), this._iconFrame);
			base.Append(this._achievementIcon);
			this._achievementIconBorders = new UIImage(TextureManager.Load("Images/UI/Achievement_Borders"));
			this._achievementIconBorders.Left.Set(-4f, 0f);
			this._achievementIconBorders.Top.Set(-4f, 0f);
			base.Append(this._achievementIconBorders);
			this._innerPanelTopTexture = TextureManager.Load("Images/UI/Achievement_InnerPanelTop");
			this._innerPanelBottomTexture = TextureManager.Load("Images/UI/Achievement_InnerPanelBottom");
			this._categoryTexture = TextureManager.Load("Images/UI/Achievement_Categories");
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			base.DrawSelf(spriteBatch);
			this._locked = !this._achievement.IsCompleted;
			this.UpdateIconFrame();
			CalculatedStyle innerDimensions = base.GetInnerDimensions();
			CalculatedStyle dimensions = this._achievementIconBorders.GetDimensions();
			float num = dimensions.X + dimensions.Width;
			Vector2 value = new Vector2(num + 7f, innerDimensions.Y);
			Tuple<decimal, decimal> trackerValues = this.GetTrackerValues();
			bool flag = false;
			if ((!(trackerValues.Item1 == 0m) || !(trackerValues.Item2 == 0m)) && this._locked)
			{
				flag = true;
			}
			float num2 = innerDimensions.Width - dimensions.Width + 1f;
			Vector2 baseScale = new Vector2(0.85f);
			Vector2 baseScale2 = new Vector2(0.92f);
			Vector2 stringSize = ChatManager.GetStringSize(Main.fontItemStack, this._achievement.Description.Value, baseScale2, num2);
			if (stringSize.Y > 38f)
			{
				baseScale2.Y *= 38f / stringSize.Y;
			}
			Color color = this._locked ? Color.Silver : Color.Gold;
			color = Color.Lerp(color, Color.White, base.IsMouseHovering ? 0.5f : 0f);
			Color color2 = this._locked ? Color.DarkGray : Color.Silver;
			color2 = Color.Lerp(color2, Color.White, base.IsMouseHovering ? 1f : 0f);
			Color color3 = base.IsMouseHovering ? Color.White : Color.Gray;
			Vector2 vector = value - Vector2.UnitY * 2f;
			this.DrawPanelTop(spriteBatch, vector, num2, color3);
			AchievementCategory category = this._achievement.Category;
			vector.Y += 2f;
			vector.X += 4f;
			spriteBatch.Draw(this._categoryTexture, vector, new Rectangle?(this._categoryTexture.Frame(4, 2, (int)category, 0)), base.IsMouseHovering ? Color.White : Color.Silver, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
			vector.X += 4f;
			vector.X += 17f;
			ChatManager.DrawColorCodedStringWithShadow(spriteBatch, Main.fontItemStack, this._achievement.FriendlyName.Value, vector, color, 0f, Vector2.Zero, baseScale, num2, 2f);
			vector.X -= 17f;
			Vector2 position = value + Vector2.UnitY * 27f;
			this.DrawPanelBottom(spriteBatch, position, num2, color3);
			position.X += 8f;
			position.Y += 4f;
			ChatManager.DrawColorCodedStringWithShadow(spriteBatch, Main.fontItemStack, this._achievement.Description.Value, position, color2, 0f, Vector2.Zero, baseScale2, num2 - 10f, 2f);
			if (flag)
			{
				Vector2 vector2 = vector + Vector2.UnitX * num2 + Vector2.UnitY;
				string text = ((int)trackerValues.Item1).ToString() + "/" + ((int)trackerValues.Item2).ToString();
				Vector2 baseScale3 = new Vector2(0.75f);
				Vector2 stringSize2 = ChatManager.GetStringSize(Main.fontItemStack, text, baseScale3, -1f);
				float progress = (float)(trackerValues.Item1 / trackerValues.Item2);
				float num3 = 80f;
				Color color4 = new Color(100, 255, 100);
				if (!base.IsMouseHovering)
				{
					color4 = Color.Lerp(color4, Color.Black, 0.25f);
				}
				Color color5 = new Color(255, 255, 255);
				if (!base.IsMouseHovering)
				{
					color5 = Color.Lerp(color5, Color.Black, 0.25f);
				}
				this.DrawProgressBar(spriteBatch, progress, vector2 - Vector2.UnitX * num3 * 0.7f, num3, color5, color4, color4.MultiplyRGBA(new Color(new Vector4(1f, 1f, 1f, 0.5f))));
				vector2.X -= num3 * 1.4f + stringSize2.X;
				ChatManager.DrawColorCodedStringWithShadow(spriteBatch, Main.fontItemStack, text, vector2, color, 0f, new Vector2(0f, 0f), baseScale3, 90f, 2f);
			}
		}

		private void UpdateIconFrame()
		{
			if (!this._locked)
			{
				this._iconFrame = this._iconFrameUnlocked;
			}
			else
			{
				this._iconFrame = this._iconFrameLocked;
			}
			if (this._achievementIcon != null)
			{
				this._achievementIcon.SetFrame(this._iconFrame);
			}
		}

		private void DrawPanelTop(SpriteBatch spriteBatch, Vector2 position, float width, Color color)
		{
			spriteBatch.Draw(this._innerPanelTopTexture, position, new Rectangle?(new Rectangle(0, 0, 2, this._innerPanelTopTexture.Height)), color);
			spriteBatch.Draw(this._innerPanelTopTexture, new Vector2(position.X + 2f, position.Y), new Rectangle?(new Rectangle(2, 0, 2, this._innerPanelTopTexture.Height)), color, 0f, Vector2.Zero, new Vector2((width - 4f) / 2f, 1f), SpriteEffects.None, 0f);
			spriteBatch.Draw(this._innerPanelTopTexture, new Vector2(position.X + width - 2f, position.Y), new Rectangle?(new Rectangle(4, 0, 2, this._innerPanelTopTexture.Height)), color);
		}

		private void DrawPanelBottom(SpriteBatch spriteBatch, Vector2 position, float width, Color color)
		{
			spriteBatch.Draw(this._innerPanelBottomTexture, position, new Rectangle?(new Rectangle(0, 0, 6, this._innerPanelBottomTexture.Height)), color);
			spriteBatch.Draw(this._innerPanelBottomTexture, new Vector2(position.X + 6f, position.Y), new Rectangle?(new Rectangle(6, 0, 7, this._innerPanelBottomTexture.Height)), color, 0f, Vector2.Zero, new Vector2((width - 12f) / 7f, 1f), SpriteEffects.None, 0f);
			spriteBatch.Draw(this._innerPanelBottomTexture, new Vector2(position.X + width - 6f, position.Y), new Rectangle?(new Rectangle(13, 0, 6, this._innerPanelBottomTexture.Height)), color);
		}

		public override void MouseOver(UIMouseEvent evt)
		{
			base.MouseOver(evt);
			this.BackgroundColor = new Color(46, 60, 119);
			this.BorderColor = new Color(20, 30, 56);
		}

		public override void MouseOut(UIMouseEvent evt)
		{
			base.MouseOut(evt);
			this.BackgroundColor = new Color(26, 40, 89) * 0.8f;
			this.BorderColor = new Color(13, 20, 44) * 0.8f;
		}

		public Achievement GetAchievement()
		{
			return this._achievement;
		}

		private Tuple<decimal, decimal> GetTrackerValues()
		{
			if (!this._achievement.HasTracker)
			{
				return Tuple.Create<decimal, decimal>(0m, 0m);
			}
			IAchievementTracker tracker = this._achievement.GetTracker();
			if (tracker.GetTrackerType() == TrackerType.Int)
			{
				AchievementTracker<int> achievementTracker = (AchievementTracker<int>)tracker;
				return Tuple.Create<decimal, decimal>(achievementTracker.Value, achievementTracker.MaxValue);
			}
			if (tracker.GetTrackerType() == TrackerType.Float)
			{
				AchievementTracker<float> achievementTracker2 = (AchievementTracker<float>)tracker;
				return Tuple.Create<decimal, decimal>((decimal)achievementTracker2.Value, (decimal)achievementTracker2.MaxValue);
			}
			return Tuple.Create<decimal, decimal>(0m, 0m);
		}

		private void DrawProgressBar(SpriteBatch spriteBatch, float progress, Vector2 spot, float Width = 169f, Color BackColor = default(Color), Color FillingColor = default(Color), Color BlipColor = default(Color))
		{
			if (BlipColor == Color.Transparent)
			{
				BlipColor = new Color(255, 165, 0, 127);
			}
			if (FillingColor == Color.Transparent)
			{
				FillingColor = new Color(255, 241, 51);
			}
			if (BackColor == Color.Transparent)
			{
				FillingColor = new Color(255, 255, 255);
			}
			Texture2D colorBarTexture = Main.colorBarTexture;
			Texture2D arg_72_0 = Main.colorBlipTexture;
			Texture2D magicPixel = Main.magicPixel;
			float num = MathHelper.Clamp(progress, 0f, 1f);
			float num2 = Width * 1f;
			float num3 = 8f;
			float num4 = num2 / 169f;
			Vector2 vector = spot + Vector2.UnitY * num3 + Vector2.UnitX * 1f;
			spriteBatch.Draw(colorBarTexture, spot, new Rectangle?(new Rectangle(5, 0, colorBarTexture.Width - 9, colorBarTexture.Height)), BackColor, 0f, new Vector2(84.5f, 0f), new Vector2(num4, 1f), SpriteEffects.None, 0f);
			spriteBatch.Draw(colorBarTexture, spot + new Vector2(-num4 * 84.5f - 5f, 0f), new Rectangle?(new Rectangle(0, 0, 5, colorBarTexture.Height)), BackColor, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0f);
			spriteBatch.Draw(colorBarTexture, spot + new Vector2(num4 * 84.5f, 0f), new Rectangle?(new Rectangle(colorBarTexture.Width - 4, 0, 4, colorBarTexture.Height)), BackColor, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0f);
			vector += Vector2.UnitX * (num - 0.5f) * num2;
			vector.X -= 1f;
			spriteBatch.Draw(magicPixel, vector, new Rectangle?(new Rectangle(0, 0, 1, 1)), FillingColor, 0f, new Vector2(1f, 0.5f), new Vector2(num2 * num, num3), SpriteEffects.None, 0f);
			if (progress != 0f)
			{
				spriteBatch.Draw(magicPixel, vector, new Rectangle?(new Rectangle(0, 0, 1, 1)), BlipColor, 0f, new Vector2(1f, 0.5f), new Vector2(2f, num3), SpriteEffects.None, 0f);
			}
			spriteBatch.Draw(magicPixel, vector, new Rectangle?(new Rectangle(0, 0, 1, 1)), Color.Black, 0f, new Vector2(0f, 0.5f), new Vector2(num2 * (1f - num), num3), SpriteEffects.None, 0f);
		}

		public override int CompareTo(object obj)
		{
			UIAchievementListItem uIAchievementListItem = obj as UIAchievementListItem;
			if (uIAchievementListItem == null)
			{
				return 0;
			}
			if (this._achievement.IsCompleted && !uIAchievementListItem._achievement.IsCompleted)
			{
				return -1;
			}
			if (!this._achievement.IsCompleted && uIAchievementListItem._achievement.IsCompleted)
			{
				return 1;
			}
			return this._achievement.Id.CompareTo(uIAchievementListItem._achievement.Id);
		}
	}
}
