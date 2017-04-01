using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria.Achievements;
using Terraria.GameInput;
using Terraria.Graphics;

namespace Terraria.UI
{
	public class AchievementCompleteUI
	{
		public class DrawCache
		{
			private const int _iconSize = 64;
			private const int _iconSizeWithSpace = 66;
			private const int _iconsPerRow = 8;
			public Achievement theAchievement;
			public int IconIndex;
			public Rectangle Frame;
			public string Title;
			public int TimeLeft;

			public float Scale
			{
				get
				{
					if (this.TimeLeft < 30)
					{
						return MathHelper.Lerp(0f, 1f, (float)this.TimeLeft / 30f);
					}
					if (this.TimeLeft > 285)
					{
						return MathHelper.Lerp(1f, 0f, ((float)this.TimeLeft - 285f) / 15f);
					}
					return 1f;
				}
			}

			public float Alpha
			{
				get
				{
					float scale = this.Scale;
					if (scale <= 0.5f)
					{
						return 0f;
					}
					return (scale - 0.5f) / 0.5f;
				}
			}

			public void Update()
			{
				this.TimeLeft--;
				if (this.TimeLeft < 0)
				{
					this.TimeLeft = 0;
				}
			}

			public DrawCache(Achievement achievement)
			{
				this.theAchievement = achievement;
				this.Title = achievement.FriendlyName.Value;
				int iconIndex = Main.Achievements.GetIconIndex(achievement.Name);
				this.IconIndex = iconIndex;
				this.Frame = new Rectangle(iconIndex % 8 * 66, iconIndex / 8 * 66, 64, 64);
				this.TimeLeft = 300;
			}

			public void ApplyHeight(ref Vector2 v)
			{
				v.Y -= 50f * this.Alpha;
			}
		}

		private static Texture2D AchievementsTexture;
		private static Texture2D AchievementsTextureBorder;
		private static List<AchievementCompleteUI.DrawCache> caches = new List<AchievementCompleteUI.DrawCache>();

		public static void LoadContent()
		{
			AchievementCompleteUI.AchievementsTexture = TextureManager.Load("Images/UI/Achievements");
			AchievementCompleteUI.AchievementsTextureBorder = TextureManager.Load("Images/UI/Achievement_Borders");
		}

		public static void Initialize()
		{
			Main.Achievements.OnAchievementCompleted += new Achievement.AchievementCompleted(AchievementCompleteUI.AddCompleted);
		}

		public static void Draw(SpriteBatch sb)
		{
			float num = (float)(Main.screenHeight - 40);
			if (PlayerInput.UsingGamepad)
			{
				num -= 25f;
			}
			Vector2 vector = new Vector2((float)(Main.screenWidth / 2), num);
			foreach (AchievementCompleteUI.DrawCache current in AchievementCompleteUI.caches)
			{
				AchievementCompleteUI.DrawAchievement(sb, ref vector, current);
				if (vector.Y < -100f)
				{
					break;
				}
			}
		}

		public static void AddCompleted(Achievement achievement)
		{
			if (Main.netMode == 2)
			{
				return;
			}
			AchievementCompleteUI.caches.Add(new AchievementCompleteUI.DrawCache(achievement));
		}

		public static void Clear()
		{
			AchievementCompleteUI.caches.Clear();
		}

		public static void Update()
		{
			foreach (AchievementCompleteUI.DrawCache current in AchievementCompleteUI.caches)
			{
				current.Update();
			}
			for (int i = 0; i < AchievementCompleteUI.caches.Count; i++)
			{
				if (AchievementCompleteUI.caches[i].TimeLeft == 0)
				{
					AchievementCompleteUI.caches.Remove(AchievementCompleteUI.caches[i]);
					i--;
				}
			}
		}

		private static void DrawAchievement(SpriteBatch sb, ref Vector2 center, AchievementCompleteUI.DrawCache ach)
		{
			float alpha = ach.Alpha;
			if (alpha > 0f)
			{
				string title = ach.Title;
				Vector2 center2 = center;
				Vector2 value = Main.fontItemStack.MeasureString(title);
				float num = ach.Scale * 1.1f;
				Rectangle r = Utils.CenteredRectangle(center2, (value + new Vector2(58f, 10f)) * num);
				Vector2 mouseScreen = Main.MouseScreen;
				bool flag = r.Contains(mouseScreen.ToPoint());
				Color c = flag ? (new Color(64, 109, 164) * 0.75f) : (new Color(64, 109, 164) * 0.5f);
				Utils.DrawInvBG(sb, r, c);
				float num2 = num * 0.3f;
				Color value2 = new Color((int)Main.mouseTextColor, (int)Main.mouseTextColor, (int)(Main.mouseTextColor / 5), (int)Main.mouseTextColor);
				Vector2 vector = r.Right() - Vector2.UnitX * num * (12f + num2 * (float)ach.Frame.Width);
				sb.Draw(AchievementCompleteUI.AchievementsTexture, vector, new Rectangle?(ach.Frame), Color.White * alpha, 0f, new Vector2(0f, (float)(ach.Frame.Height / 2)), num2, SpriteEffects.None, 0f);
				sb.Draw(AchievementCompleteUI.AchievementsTextureBorder, vector, null, Color.White * alpha, 0f, new Vector2(0f, (float)(ach.Frame.Height / 2)), num2, SpriteEffects.None, 0f);
				Utils.DrawBorderString(sb, title, vector - Vector2.UnitX * 10f, value2 * alpha, num * 0.9f, 1f, 0.4f, -1);
				if (flag && !PlayerInput.IgnoreMouseInterface)
				{
					Main.player[Main.myPlayer].mouseInterface = true;
					if (Main.mouseLeft && Main.mouseLeftRelease)
					{
						IngameFancyUI.OpenAchievementsAndGoto(ach.theAchievement);
						ach.TimeLeft = 0;
					}
				}
			}
			ach.ApplyHeight(ref center);
		}
	}
}
