using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria.GameInput;

namespace Terraria.UI.Gamepad
{
	public class GamepadMainMenuHandler
	{
		public static int LastMainMenu = -1;
		public static List<Vector2> MenuItemPositions = new List<Vector2>(20);
		public static int LastDrew = -1;
		public static bool CanRun = false;

		public static void Update()
		{
			if (!GamepadMainMenuHandler.CanRun)
			{
				UILinkPage uILinkPage = UILinkPointNavigator.Pages[1000];
				uILinkPage.CurrentPoint = uILinkPage.DefaultPoint;
				Vector2 value = new Vector2((float)Math.Cos((double)(Main.GlobalTime * 6.28318548f)), (float)Math.Sin((double)(Main.GlobalTime * 6.28318548f * 2f))) * new Vector2(30f, 15f) + Vector2.UnitY * 20f;
				UILinkPointNavigator.SetPosition(2000, new Vector2((float)Main.screenWidth, (float)Main.screenHeight) / 2f + value);
				return;
			}
			if (!Main.gameMenu)
			{
				return;
			}
			if (Main.MenuUI.IsVisible)
			{
				return;
			}
			if (GamepadMainMenuHandler.LastDrew != Main.menuMode)
			{
				return;
			}
			int lastMainMenu = GamepadMainMenuHandler.LastMainMenu;
			GamepadMainMenuHandler.LastMainMenu = Main.menuMode;
			switch (Main.menuMode)
			{
				case 17:
				case 18:
				case 19:
				case 21:
				case 22:
				case 23:
				case 24:
				case 26:
					if (GamepadMainMenuHandler.MenuItemPositions.Count >= 4)
					{
						Vector2 item = GamepadMainMenuHandler.MenuItemPositions[3];
						GamepadMainMenuHandler.MenuItemPositions.RemoveAt(3);
						if (Main.menuMode == 17)
						{
							GamepadMainMenuHandler.MenuItemPositions.Insert(0, item);
						}
					}
					break;
				case 28:
					if (GamepadMainMenuHandler.MenuItemPositions.Count >= 3)
					{
						GamepadMainMenuHandler.MenuItemPositions.RemoveAt(1);
					}
					break;
			}
			UILinkPage uILinkPage2 = UILinkPointNavigator.Pages[1000];
			if (lastMainMenu != Main.menuMode)
			{
				uILinkPage2.CurrentPoint = uILinkPage2.DefaultPoint;
			}
			for (int i = 0; i < GamepadMainMenuHandler.MenuItemPositions.Count; i++)
			{
				if (i == 0 && lastMainMenu != GamepadMainMenuHandler.LastMainMenu && PlayerInput.UsingGamepad && Main.InvisibleCursorForGamepad)
				{
					Main.mouseX = (PlayerInput.MouseX = (int)GamepadMainMenuHandler.MenuItemPositions[i].X);
					Main.mouseY = (PlayerInput.MouseY = (int)GamepadMainMenuHandler.MenuItemPositions[i].Y);
					Main.menuFocus = -1;
				}
				UILinkPoint uILinkPoint = uILinkPage2.LinkMap[2000 + i];
				uILinkPoint.Position = GamepadMainMenuHandler.MenuItemPositions[i];
				if (i == 0)
				{
					uILinkPoint.Up = -1;
				}
				else
				{
					uILinkPoint.Up = 2000 + i - 1;
				}
				uILinkPoint.Left = -3;
				uILinkPoint.Right = -4;
				if (i == GamepadMainMenuHandler.MenuItemPositions.Count - 1)
				{
					uILinkPoint.Down = -2;
				}
				else
				{
					uILinkPoint.Down = 2000 + i + 1;
				}
			}
			GamepadMainMenuHandler.MenuItemPositions.Clear();
		}
	}
}
