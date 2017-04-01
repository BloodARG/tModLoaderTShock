using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.GameInput;

namespace Terraria.UI.Gamepad
{
	public class UILinkPointNavigator
	{
		public static class Shortcuts
		{
			public static int NPCS_IconsPerColumn = 100;
			public static int NPCS_IconsTotal = 0;
			public static int NPCS_LastHovered = -1;
			public static bool NPCS_IconsDisplay = false;
			public static int CRAFT_IconsPerRow = 100;
			public static int CRAFT_IconsPerColumn = 100;
			public static int CRAFT_CurrentIngridientsCount = 0;
			public static int CRAFT_CurrentRecipeBig = 0;
			public static int CRAFT_CurrentRecipeSmall = 0;
			public static bool NPCCHAT_ButtonsLeft = false;
			public static bool NPCCHAT_ButtonsMiddle = false;
			public static bool NPCCHAT_ButtonsRight = false;
			public static int INGAMEOPTIONS_BUTTONS_LEFT = 0;
			public static int INGAMEOPTIONS_BUTTONS_RIGHT = 0;
			public static int OPTIONS_BUTTON_SPECIALFEATURE = 0;
			public static int BackButtonCommand = 0;
			public static bool BackButtonInUse = false;
			public static bool BackButtonLock = false;
			public static int BackButtonGoto = 0;
			public static int FANCYUI_HIGHEST_INDEX = 1;
			public static int FANCYUI_SPECIAL_INSTRUCTIONS = 0;
			public static int INFOACCCOUNT = 0;
			public static int BUILDERACCCOUNT = 0;
			public static int BUFFS_PER_COLUMN = 0;
			public static int BUFFS_DRAWN = 0;
			public static int INV_MOVE_OPTION_CD = 0;
		}

		public static Dictionary<int, UILinkPage> Pages = new Dictionary<int, UILinkPage>();
		public static Dictionary<int, UILinkPoint> Points = new Dictionary<int, UILinkPoint>();
		public static int CurrentPage = 1000;
		public static int OldPage = 1000;
		private static int XCooldown = 0;
		private static int YCooldown = 0;
		private static Vector2 LastInput;
		private static int PageLeftCD = 0;
		private static int PageRightCD = 0;
		public static bool InUse;
		public static int OverridePoint = -1;

		public static int CurrentPoint
		{
			get
			{
				return UILinkPointNavigator.Pages[UILinkPointNavigator.CurrentPage].CurrentPoint;
			}
		}

		public static bool Available
		{
			get
			{
				return Main.playerInventory || Main.ingameOptionsWindow || Main.player[Main.myPlayer].talkNPC != -1 || Main.player[Main.myPlayer].sign != -1 || Main.mapFullscreen || Main.clothesWindow || Main.MenuUI.IsVisible || Main.InGameUI.IsVisible;
			}
		}

		public static void GoToDefaultPage(int specialFlag = 0)
		{
			if (Main.MenuUI.IsVisible)
			{
				UILinkPointNavigator.CurrentPage = 1004;
				return;
			}
			if (Main.InGameUI.IsVisible || specialFlag == 1)
			{
				UILinkPointNavigator.CurrentPage = 1004;
				return;
			}
			if (Main.gameMenu)
			{
				UILinkPointNavigator.CurrentPage = 1000;
				return;
			}
			if (Main.ingameOptionsWindow)
			{
				UILinkPointNavigator.CurrentPage = 1001;
				return;
			}
			if (Main.hairWindow)
			{
				UILinkPointNavigator.CurrentPage = 12;
				return;
			}
			if (Main.clothesWindow)
			{
				UILinkPointNavigator.CurrentPage = 15;
				return;
			}
			if (Main.npcShop != 0)
			{
				UILinkPointNavigator.CurrentPage = 13;
				return;
			}
			if (Main.InGuideCraftMenu)
			{
				UILinkPointNavigator.CurrentPage = 9;
				return;
			}
			if (Main.InReforgeMenu)
			{
				UILinkPointNavigator.CurrentPage = 5;
				return;
			}
			if (Main.player[Main.myPlayer].chest != -1)
			{
				UILinkPointNavigator.CurrentPage = 4;
				return;
			}
			if (Main.player[Main.myPlayer].talkNPC != -1 || Main.player[Main.myPlayer].sign != -1)
			{
				UILinkPointNavigator.CurrentPage = 1003;
				return;
			}
			UILinkPointNavigator.CurrentPage = 0;
		}

		public static void Update()
		{
			bool inUse = UILinkPointNavigator.InUse;
			UILinkPointNavigator.InUse = false;
			bool flag = true;
			if (flag)
			{
				switch (PlayerInput.CurrentInputMode)
				{
					case InputMode.Keyboard:
					case InputMode.KeyboardUI:
					case InputMode.Mouse:
						if (!Main.gameMenu)
						{
							flag = false;
						}
						break;
				}
			}
			if (flag && PlayerInput.NavigatorRebindingLock > 0)
			{
				flag = false;
			}
			if (flag && !Main.gameMenu && !PlayerInput.UsingGamepadUI)
			{
				flag = false;
			}
			if (flag && !Main.gameMenu && PlayerInput.InBuildingMode)
			{
				flag = false;
			}
			if (flag && !Main.gameMenu && !UILinkPointNavigator.Available)
			{
				flag = false;
			}
			bool flag2 = false;
			UILinkPage uILinkPage;
			if (!UILinkPointNavigator.Pages.TryGetValue(UILinkPointNavigator.CurrentPage, out uILinkPage))
			{
				flag2 = true;
			}
			else if (!uILinkPage.IsValid())
			{
				flag2 = true;
			}
			if (flag2)
			{
				UILinkPointNavigator.GoToDefaultPage(0);
				UILinkPointNavigator.ProcessChanges();
				flag = false;
			}
			if (inUse != flag)
			{
				if (!flag)
				{
					uILinkPage.Leave();
					UILinkPointNavigator.GoToDefaultPage(0);
					UILinkPointNavigator.ProcessChanges();
				}
				else
				{
					UILinkPointNavigator.GoToDefaultPage(0);
					UILinkPointNavigator.ProcessChanges();
					uILinkPage.Enter();
				}
				if (flag)
				{
					Main.player[Main.myPlayer].releaseInventory = false;
					Main.player[Main.myPlayer].releaseUseTile = false;
					PlayerInput.LockTileUseButton = true;
				}
				if (!Main.gameMenu)
				{
					if (flag)
					{
						PlayerInput.NavigatorCachePosition();
					}
					else
					{
						PlayerInput.NavigatorUnCachePosition();
					}
				}
			}
			if (!flag)
			{
				return;
			}
			UILinkPointNavigator.InUse = true;
			UILinkPointNavigator.OverridePoint = -1;
			if (UILinkPointNavigator.PageLeftCD > 0)
			{
				UILinkPointNavigator.PageLeftCD--;
			}
			if (UILinkPointNavigator.PageRightCD > 0)
			{
				UILinkPointNavigator.PageRightCD--;
			}
			Vector2 navigatorDirections = PlayerInput.Triggers.Current.GetNavigatorDirections();
			bool flag3 = PlayerInput.Triggers.Current.HotbarMinus && !PlayerInput.Triggers.Current.HotbarPlus;
			bool flag4 = PlayerInput.Triggers.Current.HotbarPlus && !PlayerInput.Triggers.Current.HotbarMinus;
			if (!flag3)
			{
				UILinkPointNavigator.PageLeftCD = 0;
			}
			if (!flag4)
			{
				UILinkPointNavigator.PageRightCD = 0;
			}
			flag3 = (flag3 && UILinkPointNavigator.PageLeftCD == 0);
			flag4 = (flag4 && UILinkPointNavigator.PageRightCD == 0);
			if (UILinkPointNavigator.LastInput.X != navigatorDirections.X)
			{
				UILinkPointNavigator.XCooldown = 0;
			}
			if (UILinkPointNavigator.LastInput.Y != navigatorDirections.Y)
			{
				UILinkPointNavigator.YCooldown = 0;
			}
			if (UILinkPointNavigator.XCooldown > 0)
			{
				UILinkPointNavigator.XCooldown--;
			}
			if (UILinkPointNavigator.YCooldown > 0)
			{
				UILinkPointNavigator.YCooldown--;
			}
			UILinkPointNavigator.LastInput = navigatorDirections;
			if (flag3)
			{
				UILinkPointNavigator.PageLeftCD = 16;
			}
			if (flag4)
			{
				UILinkPointNavigator.PageRightCD = 16;
			}
			UILinkPointNavigator.Pages[UILinkPointNavigator.CurrentPage].Update();
			int num = 10;
			if (!Main.gameMenu && Main.playerInventory && !Main.ingameOptionsWindow && !Main.inFancyUI && (UILinkPointNavigator.CurrentPage == 0 || UILinkPointNavigator.CurrentPage == 4 || UILinkPointNavigator.CurrentPage == 2 || UILinkPointNavigator.CurrentPage == 1))
			{
				num = PlayerInput.CurrentProfile.InventoryMoveCD;
			}
			if (navigatorDirections.X == -1f && UILinkPointNavigator.XCooldown == 0)
			{
				UILinkPointNavigator.XCooldown = num;
				UILinkPointNavigator.Pages[UILinkPointNavigator.CurrentPage].TravelLeft();
			}
			if (navigatorDirections.X == 1f && UILinkPointNavigator.XCooldown == 0)
			{
				UILinkPointNavigator.XCooldown = num;
				UILinkPointNavigator.Pages[UILinkPointNavigator.CurrentPage].TravelRight();
			}
			if (navigatorDirections.Y == -1f && UILinkPointNavigator.YCooldown == 0)
			{
				UILinkPointNavigator.YCooldown = num;
				UILinkPointNavigator.Pages[UILinkPointNavigator.CurrentPage].TravelUp();
			}
			if (navigatorDirections.Y == 1f && UILinkPointNavigator.YCooldown == 0)
			{
				UILinkPointNavigator.YCooldown = num;
				UILinkPointNavigator.Pages[UILinkPointNavigator.CurrentPage].TravelDown();
			}
			int yCooldown = Math.Max(UILinkPointNavigator.XCooldown, UILinkPointNavigator.YCooldown);
			UILinkPointNavigator.XCooldown = (UILinkPointNavigator.YCooldown = yCooldown);
			if (flag3)
			{
				UILinkPointNavigator.Pages[UILinkPointNavigator.CurrentPage].SwapPageLeft();
			}
			if (flag4)
			{
				UILinkPointNavigator.Pages[UILinkPointNavigator.CurrentPage].SwapPageRight();
			}
			if (PlayerInput.Triggers.Current.UsedMovementKey)
			{
				Vector2 position = UILinkPointNavigator.Points[UILinkPointNavigator.CurrentPoint].Position;
				Vector2 value = new Vector2((float)PlayerInput.MouseX, (float)PlayerInput.MouseY);
				float amount = 0.3f;
				if (PlayerInput.InvisibleGamepadInMenus)
				{
					amount = 1f;
				}
				Vector2 vector = Vector2.Lerp(value, position, amount);
				if (Main.gameMenu)
				{
					if (Math.Abs(vector.X - position.X) <= 5f)
					{
						vector.X = position.X;
					}
					if (Math.Abs(vector.Y - position.Y) <= 5f)
					{
						vector.Y = position.Y;
					}
				}
				PlayerInput.MouseX = (int)vector.X;
				PlayerInput.MouseY = (int)vector.Y;
			}
			UILinkPointNavigator.ResetFlagsEnd();
		}

		public static void ResetFlagsEnd()
		{
			UILinkPointNavigator.Shortcuts.OPTIONS_BUTTON_SPECIALFEATURE = 0;
			UILinkPointNavigator.Shortcuts.BackButtonLock = false;
			UILinkPointNavigator.Shortcuts.BackButtonCommand = 0;
		}

		public static string GetInstructions()
		{
			string text = UILinkPointNavigator.Pages[UILinkPointNavigator.CurrentPage].SpecialInteractions();
			string text2 = UILinkPointNavigator.Points[UILinkPointNavigator.CurrentPoint].SpecialInteractions();
			if (!string.IsNullOrEmpty(text2))
			{
				if (string.IsNullOrEmpty(text))
				{
					return text2;
				}
				text = text + "   " + text2;
			}
			return text;
		}

		public static void SetPosition(int ID, Vector2 Position)
		{
			UILinkPointNavigator.Points[ID].Position = Position;
		}

		public static void RegisterPage(UILinkPage page, int ID, bool automatedDefault = true)
		{
			if (automatedDefault)
			{
				page.DefaultPoint = page.LinkMap.Keys.First<int>();
			}
			page.CurrentPoint = page.DefaultPoint;
			page.ID = ID;
			UILinkPointNavigator.Pages.Add(page.ID, page);
			foreach (KeyValuePair<int, UILinkPoint> current in page.LinkMap)
			{
				current.Value.SetPage(ID);
				UILinkPointNavigator.Points.Add(current.Key, current.Value);
			}
		}

		public static void ChangePage(int PageID)
		{
			if (UILinkPointNavigator.Pages.ContainsKey(PageID) && UILinkPointNavigator.Pages[PageID].CanEnter())
			{
				UILinkPointNavigator.CurrentPage = PageID;
				UILinkPointNavigator.ProcessChanges();
			}
		}

		public static void ChangePoint(int PointID)
		{
			if (UILinkPointNavigator.Points.ContainsKey(PointID))
			{
				UILinkPointNavigator.CurrentPage = UILinkPointNavigator.Points[PointID].Page;
				UILinkPointNavigator.OverridePoint = PointID;
				UILinkPointNavigator.ProcessChanges();
			}
		}

		public static void ProcessChanges()
		{
			UILinkPage uILinkPage = UILinkPointNavigator.Pages[UILinkPointNavigator.OldPage];
			if (UILinkPointNavigator.OldPage != UILinkPointNavigator.CurrentPage)
			{
				uILinkPage.Leave();
				if (!UILinkPointNavigator.Pages.TryGetValue(UILinkPointNavigator.CurrentPage, out uILinkPage))
				{
					UILinkPointNavigator.GoToDefaultPage(0);
					UILinkPointNavigator.ProcessChanges();
					UILinkPointNavigator.OverridePoint = -1;
				}
				uILinkPage.CurrentPoint = uILinkPage.DefaultPoint;
				uILinkPage.Enter();
				uILinkPage.Update();
				UILinkPointNavigator.OldPage = UILinkPointNavigator.CurrentPage;
			}
			if (UILinkPointNavigator.OverridePoint != -1 && uILinkPage.LinkMap.ContainsKey(UILinkPointNavigator.OverridePoint))
			{
				uILinkPage.CurrentPoint = UILinkPointNavigator.OverridePoint;
			}
		}
	}
}
