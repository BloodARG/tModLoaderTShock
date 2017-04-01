using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria.Achievements;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.Graphics;
using Terraria.Localization;
using Terraria.UI;
using Terraria.UI.Gamepad;

namespace Terraria.GameContent.UI.States
{
	public class UIAchievementsMenu : UIState
	{
		private UIList _achievementsList;
		private List<UIAchievementListItem> _achievementElements = new List<UIAchievementListItem>();
		private List<UIToggleImage> _categoryButtons = new List<UIToggleImage>();
		private UIElement _backpanel;
		private UIElement _outerContainer;

		public override void OnInitialize()
		{
			UIElement uIElement = new UIElement();
			uIElement.Width.Set(0f, 0.8f);
			uIElement.MaxWidth.Set(800f, 0f);
			uIElement.MinWidth.Set(600f, 0f);
			uIElement.Top.Set(220f, 0f);
			uIElement.Height.Set(-220f, 1f);
			uIElement.HAlign = 0.5f;
			this._outerContainer = uIElement;
			base.Append(uIElement);
			UIPanel uIPanel = new UIPanel();
			uIPanel.Width.Set(0f, 1f);
			uIPanel.Height.Set(-110f, 1f);
			uIPanel.BackgroundColor = new Color(33, 43, 79) * 0.8f;
			uIPanel.PaddingTop = 0f;
			uIElement.Append(uIPanel);
			this._achievementsList = new UIList();
			this._achievementsList.Width.Set(-25f, 1f);
			this._achievementsList.Height.Set(-50f, 1f);
			this._achievementsList.Top.Set(50f, 0f);
			this._achievementsList.ListPadding = 5f;
			uIPanel.Append(this._achievementsList);
			UITextPanel<LocalizedText> uITextPanel = new UITextPanel<LocalizedText>(Language.GetText("UI.Achievements"), 1f, true);
			uITextPanel.HAlign = 0.5f;
			uITextPanel.Top.Set(-33f, 0f);
			uITextPanel.SetPadding(13f);
			uITextPanel.BackgroundColor = new Color(73, 94, 171);
			uIElement.Append(uITextPanel);
			UITextPanel<LocalizedText> uITextPanel2 = new UITextPanel<LocalizedText>(Language.GetText("UI.Back"), 0.7f, true);
			uITextPanel2.Width.Set(-10f, 0.5f);
			uITextPanel2.Height.Set(50f, 0f);
			uITextPanel2.VAlign = 1f;
			uITextPanel2.HAlign = 0.5f;
			uITextPanel2.Top.Set(-45f, 0f);
			uITextPanel2.OnMouseOver += new UIElement.MouseEvent(this.FadedMouseOver);
			uITextPanel2.OnMouseOut += new UIElement.MouseEvent(this.FadedMouseOut);
			uITextPanel2.OnClick += new UIElement.MouseEvent(this.GoBackClick);
			uIElement.Append(uITextPanel2);
			this._backpanel = uITextPanel2;
			List<Achievement> list = Main.Achievements.CreateAchievementsList();
			for (int i = 0; i < list.Count; i++)
			{
				UIAchievementListItem item = new UIAchievementListItem(list[i]);
				this._achievementsList.Add(item);
				this._achievementElements.Add(item);
			}
			UIScrollbar uIScrollbar = new UIScrollbar();
			uIScrollbar.SetView(100f, 1000f);
			uIScrollbar.Height.Set(-50f, 1f);
			uIScrollbar.Top.Set(50f, 0f);
			uIScrollbar.HAlign = 1f;
			uIPanel.Append(uIScrollbar);
			this._achievementsList.SetScrollbar(uIScrollbar);
			UIElement uIElement2 = new UIElement();
			uIElement2.Width.Set(0f, 1f);
			uIElement2.Height.Set(32f, 0f);
			uIElement2.Top.Set(10f, 0f);
			Texture2D texture = TextureManager.Load("Images/UI/Achievement_Categories");
			for (int j = 0; j < 4; j++)
			{
				UIToggleImage uIToggleImage = new UIToggleImage(texture, 32, 32, new Point(34 * j, 0), new Point(34 * j, 34));
				uIToggleImage.Left.Set((float)(j * 36 + 8), 0f);
				uIToggleImage.SetState(true);
				uIToggleImage.OnClick += new UIElement.MouseEvent(this.FilterList);
				this._categoryButtons.Add(uIToggleImage);
				uIElement2.Append(uIToggleImage);
			}
			uIPanel.Append(uIElement2);
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);
			for (int i = 0; i < this._categoryButtons.Count; i++)
			{
				if (this._categoryButtons[i].IsMouseHovering)
				{
					string textValue;
					switch (i)
					{
						case -1:
							textValue = Language.GetTextValue("Achievements.NoCategory");
							break;
						case 0:
							textValue = Language.GetTextValue("Achievements.SlayerCategory");
							break;
						case 1:
							textValue = Language.GetTextValue("Achievements.CollectorCategory");
							break;
						case 2:
							textValue = Language.GetTextValue("Achievements.ExplorerCategory");
							break;
						case 3:
							textValue = Language.GetTextValue("Achievements.ChallengerCategory");
							break;
						default:
							textValue = Language.GetTextValue("Achievements.NoCategory");
							break;
					}
					float x = Main.fontMouseText.MeasureString(textValue).X;
					Vector2 vector = new Vector2((float)Main.mouseX, (float)Main.mouseY) + new Vector2(16f);
					if (vector.Y > (float)(Main.screenHeight - 30))
					{
						vector.Y = (float)(Main.screenHeight - 30);
					}
					if (vector.X > (float)Main.screenWidth - x)
					{
						vector.X = (float)(Main.screenWidth - 460);
					}
					Utils.DrawBorderStringFourWay(spriteBatch, Main.fontMouseText, textValue, vector.X, vector.Y, new Color((int)Main.mouseTextColor, (int)Main.mouseTextColor, (int)Main.mouseTextColor, (int)Main.mouseTextColor), Color.Black, Vector2.Zero, 1f);
					break;
				}
			}
			this.SetupGamepadPoints(spriteBatch);
		}

		public void GotoAchievement(Achievement achievement)
		{
			this._achievementsList.Goto(delegate(UIElement element)
				{
					UIAchievementListItem uIAchievementListItem = element as UIAchievementListItem;
					return uIAchievementListItem != null && uIAchievementListItem.GetAchievement() == achievement;
				});
		}

		private void GoBackClick(UIMouseEvent evt, UIElement listeningElement)
		{
			Main.menuMode = 0;
			IngameFancyUI.Close();
		}

		private void FadedMouseOver(UIMouseEvent evt, UIElement listeningElement)
		{
			Main.PlaySound(12, -1, -1, 1, 1f, 0f);
			((UIPanel)evt.Target).BackgroundColor = new Color(73, 94, 171);
		}

		private void FadedMouseOut(UIMouseEvent evt, UIElement listeningElement)
		{
			((UIPanel)evt.Target).BackgroundColor = new Color(63, 82, 151) * 0.8f;
		}

		private void FilterList(UIMouseEvent evt, UIElement listeningElement)
		{
			this._achievementsList.Clear();
			foreach (UIAchievementListItem current in this._achievementElements)
			{
				if (this._categoryButtons[(int)current.GetAchievement().Category].IsOn)
				{
					this._achievementsList.Add(current);
				}
			}
			this.Recalculate();
		}

		public override void OnActivate()
		{
			if (Main.gameMenu)
			{
				this._outerContainer.Top.Set(220f, 0f);
				this._outerContainer.Height.Set(-220f, 1f);
			}
			else
			{
				this._outerContainer.Top.Set(120f, 0f);
				this._outerContainer.Height.Set(-120f, 1f);
			}
			this._achievementsList.UpdateOrder();
			if (PlayerInput.UsingGamepadUI)
			{
				UILinkPointNavigator.ChangePoint(3002);
			}
		}

		private void SetupGamepadPoints(SpriteBatch spriteBatch)
		{
			UILinkPointNavigator.Shortcuts.BackButtonCommand = 3;
			int num = 3000;
			UILinkPointNavigator.SetPosition(num, this._backpanel.GetInnerDimensions().ToRectangle().Center.ToVector2());
			UILinkPointNavigator.SetPosition(num + 1, this._outerContainer.GetInnerDimensions().ToRectangle().Center.ToVector2());
			int num2 = num;
			UILinkPoint uILinkPoint = UILinkPointNavigator.Points[num2];
			uILinkPoint.Unlink();
			uILinkPoint.Up = num2 + 1;
			num2++;
			uILinkPoint = UILinkPointNavigator.Points[num2];
			uILinkPoint.Unlink();
			uILinkPoint.Up = num2 + 1;
			uILinkPoint.Down = num2 - 1;
			for (int i = 0; i < this._categoryButtons.Count; i++)
			{
				num2++;
				UILinkPointNavigator.Shortcuts.FANCYUI_HIGHEST_INDEX = num2;
				UILinkPointNavigator.SetPosition(num2, this._categoryButtons[i].GetInnerDimensions().ToRectangle().Center.ToVector2());
				uILinkPoint = UILinkPointNavigator.Points[num2];
				uILinkPoint.Unlink();
				uILinkPoint.Left = ((i == 0) ? -3 : (num2 - 1));
				uILinkPoint.Right = ((i == this._categoryButtons.Count - 1) ? -4 : (num2 + 1));
				uILinkPoint.Down = num;
			}
		}
	}
}
