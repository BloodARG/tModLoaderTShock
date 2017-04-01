using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using Terraria.UI.Gamepad;
using Terraria.ModLoader.IO;

namespace Terraria.ModLoader.UI
{
	internal class UIModInfo : UIState
	{
		internal UIElement uIElement;
		public UIMessageBox modInfo;
		public UITextPanel<string> uITextPanel;
		internal UITextPanel<string> modHomepageButton;
		private int gotoMenu = 0;
		private string url = "";
		private string info = "";
		private string modDisplayName = "";

		public override void OnInitialize()
		{
			uIElement = new UIElement();
			uIElement.Width.Set(0f, 0.8f);
			uIElement.MaxWidth.Set(600f, 0f);
			uIElement.Top.Set(220f, 0f);
			uIElement.Height.Set(-220f, 1f);
			uIElement.HAlign = 0.5f;
			UIPanel uIPanel = new UIPanel();
			uIPanel.Width.Set(0f, 1f);
			uIPanel.Height.Set(-110f, 1f);
			uIPanel.BackgroundColor = new Color(33, 43, 79) * 0.8f;
			uIElement.Append(uIPanel);
			modInfo = new UIMessageBox("This is a test of mod info here.");
			modInfo.Width.Set(-25f, 1f);
			modInfo.Height.Set(0f, 1f);
			uIPanel.Append(modInfo);
			UIScrollbar uIScrollbar = new UIScrollbar();
			uIScrollbar.SetView(100f, 1000f);
			uIScrollbar.Height.Set(0f, 1f);
			uIScrollbar.HAlign = 1f;
			uIPanel.Append(uIScrollbar);
			modInfo.SetScrollbar(uIScrollbar);
			uITextPanel = new UITextPanel<string>("Mod Info", 0.8f, true);
			uITextPanel.HAlign = 0.5f;
			uITextPanel.Top.Set(-35f, 0f);
			uITextPanel.SetPadding(15f);
			uITextPanel.BackgroundColor = new Color(73, 94, 171);
			uIElement.Append(uITextPanel);
			modHomepageButton = new UITextPanel<string>("Visit the Mod's Homepage for even more info", 1f, false);
			modHomepageButton.Width.Set(-10f, 1f);
			modHomepageButton.Height.Set(25f, 0f);
			modHomepageButton.VAlign = 1f;
			modHomepageButton.Top.Set(-65f, 0f);
			modHomepageButton.OnMouseOver += UICommon.FadedMouseOver;
			modHomepageButton.OnMouseOut += UICommon.FadedMouseOut;
			modHomepageButton.OnClick += VisitModHomePage;
			uIElement.Append(modHomepageButton);
			UITextPanel<string> backButton = new UITextPanel<string>("Back", 1f, false);
			backButton.Width.Set(-10f, 0.5f);
			backButton.Height.Set(25f, 0f);
			backButton.VAlign = 1f;
			backButton.Top.Set(-20f, 0f);
			backButton.OnMouseOver += UICommon.FadedMouseOver;
			backButton.OnMouseOut += UICommon.FadedMouseOut;
			backButton.OnClick += BackClick;
			uIElement.Append(backButton);
			base.Append(uIElement);
		}

		internal void SetModInfo(string text)
		{
			info = text;
			if (info.Equals(""))
			{
				info = "No description available";
			}
		}

		internal void SetModName(string text)
		{
			modDisplayName = text;
		}

		internal void SetGotoMenu(int gotoMenu)
		{
			this.gotoMenu = gotoMenu;
		}

		internal void SetURL(string url)
		{
			this.url = url;
		}

		private void BackClick(UIMouseEvent evt, UIElement listeningElement)
		{
			Main.PlaySound(11, -1, -1, 1);
			Main.menuMode = this.gotoMenu;
		}

		private void VisitModHomePage(UIMouseEvent evt, UIElement listeningElement)
		{
			Main.PlaySound(10, -1, -1, 1);
			Process.Start(url);
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);
			UILinkPointNavigator.Shortcuts.BackButtonCommand = 100;
			UILinkPointNavigator.Shortcuts.BackButtonGoto = this.gotoMenu;
		}

		public override void OnActivate()
		{
			uITextPanel.SetText("Mod Info: " + modDisplayName, 0.8f, true);
			modInfo.SetText(info);
			if (url.Equals(""))
			{
				modHomepageButton.Remove();
			}
			else
			{
				uIElement.Append(modHomepageButton);
			}
		}
	}
}
