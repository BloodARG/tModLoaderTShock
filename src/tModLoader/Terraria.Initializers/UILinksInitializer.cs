using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria.GameContent.UI.States;
using Terraria.GameInput;
using Terraria.ModLoader.UI;
using Terraria.UI;
using Terraria.UI.Gamepad;

namespace Terraria.Initializers
{
	public class UILinksInitializer
	{
		public class SomeVarsForUILinkers
		{
			public static Recipe SequencedCraftingCurrent;
			public static int HairMoveCD;
		}

		public static bool NothingMoreImportantThanNPCChat()
		{
			return !Main.hairWindow && Main.npcShop == 0 && Main.player[Main.myPlayer].chest == -1;
		}

		public static float HandleSlider(float currentValue, float min, float max, float deadZone = 0.2f, float sensitivity = 0.5f)
		{
			float num = PlayerInput.GamepadThumbstickLeft.X;
			if (num < -deadZone || num > deadZone)
			{
				num = MathHelper.Lerp(0f, sensitivity / 60f, (Math.Abs(num) - deadZone) / (1f - deadZone)) * (float)Math.Sign(num);
			}
			else
			{
				num = 0f;
			}
			float num2 = (currentValue - min) / (max - min);
			num2 = MathHelper.Clamp(num2 + num, 0f, 1f);
			return num2 * (max - min) + min;
		}

		public static void Load()
		{
			Func<string> value = () => PlayerInput.BuildCommand(Lang.misc[53], false, new List<string>[]
				{
					PlayerInput.ProfileGamepadUI.KeyStatus["MouseLeft"]
				});
			UILinkPage uILinkPage = new UILinkPage();
			uILinkPage.UpdateEvent += delegate
			{
				PlayerInput.GamepadAllowScrolling = true;
			};
			for (int i = 0; i < 20; i++)
			{
				uILinkPage.LinkMap.Add(2000 + i, new UILinkPoint(2000 + i, true, -3, -4, -1, -2));
			}
			uILinkPage.OnSpecialInteracts += (() => PlayerInput.BuildCommand(Lang.misc[53], false, new List<string>[]
				{
					PlayerInput.ProfileGamepadUI.KeyStatus["MouseLeft"]
				}) + PlayerInput.BuildCommand(Lang.misc[82], true, new List<string>[]
				{
					PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]
				}));
			uILinkPage.UpdateEvent += delegate
			{
				if (PlayerInput.Triggers.JustPressed.Inventory)
				{
					UILinksInitializer.FancyExit();
				}
				UILinkPointNavigator.Shortcuts.BackButtonInUse = PlayerInput.Triggers.JustPressed.Inventory;
				UILinksInitializer.HandleOptionsSpecials();
			};
			uILinkPage.IsValidEvent += (() => Main.gameMenu && !Main.MenuUI.IsVisible);
			uILinkPage.CanEnterEvent += (() => Main.gameMenu && !Main.MenuUI.IsVisible);
			UILinkPointNavigator.RegisterPage(uILinkPage, 1000, true);
			UILinkPage cp2 = new UILinkPage();
			cp2.LinkMap.Add(2500, new UILinkPoint(2500, true, -3, 2501, -1, -2));
			cp2.LinkMap.Add(2501, new UILinkPoint(2501, true, 2500, 2502, -1, -2));
			cp2.LinkMap.Add(2502, new UILinkPoint(2502, true, 2501, -4, -1, -2));
			cp2.UpdateEvent += delegate
			{
				cp2.LinkMap[2501].Right = (UILinkPointNavigator.Shortcuts.NPCCHAT_ButtonsRight ? 2502 : -4);
			};
			cp2.OnSpecialInteracts += (() => PlayerInput.BuildCommand(Lang.misc[53], false, new List<string>[]
				{
					PlayerInput.ProfileGamepadUI.KeyStatus["MouseLeft"]
				}) + PlayerInput.BuildCommand(Lang.misc[56], true, new List<string>[]
				{
					PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]
				}));
			cp2.IsValidEvent += (() => (Main.player[Main.myPlayer].talkNPC != -1 || Main.player[Main.myPlayer].sign != -1) && UILinksInitializer.NothingMoreImportantThanNPCChat());
			cp2.CanEnterEvent += (() => (Main.player[Main.myPlayer].talkNPC != -1 || Main.player[Main.myPlayer].sign != -1) && UILinksInitializer.NothingMoreImportantThanNPCChat());
			cp2.EnterEvent += delegate
			{
				Main.player[Main.myPlayer].releaseInventory = false;
			};
			cp2.LeaveEvent += delegate
			{
				Main.npcChatRelease = false;
				Main.player[Main.myPlayer].releaseUseTile = false;
			};
			UILinkPointNavigator.RegisterPage(cp2, 1003, true);
			UILinkPage cp3 = new UILinkPage();
			cp3.OnSpecialInteracts += (() => PlayerInput.BuildCommand(Lang.misc[56], false, new List<string>[]
				{
					PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]
				}) + PlayerInput.BuildCommand(Lang.misc[64], true, new List<string>[]
				{
					PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"],
					PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]
				}));
			Func<string> value2 = delegate
			{
				int currentPoint = UILinkPointNavigator.CurrentPoint;
				return ItemSlot.GetGamepadInstructions(Main.player[Main.myPlayer].inventory, 0, currentPoint);
			};
			Func<string> value3 = () => ItemSlot.GetGamepadInstructions(ref Main.player[Main.myPlayer].trashItem, 6);
			for (int j = 0; j <= 49; j++)
			{
				UILinkPoint uILinkPoint = new UILinkPoint(j, true, j - 1, j + 1, j - 10, j + 10);
				uILinkPoint.OnSpecialInteracts += value2;
				int num = j;
				if (num < 10)
				{
					uILinkPoint.Up = -1;
				}
				if (num >= 40)
				{
					uILinkPoint.Down = -2;
				}
				if (num % 10 == 9)
				{
					uILinkPoint.Right = -4;
				}
				if (num % 10 == 0)
				{
					uILinkPoint.Left = -3;
				}
				cp3.LinkMap.Add(j, uILinkPoint);
			}
			cp3.LinkMap[9].Right = 0;
			cp3.LinkMap[19].Right = 50;
			cp3.LinkMap[29].Right = 51;
			cp3.LinkMap[39].Right = 52;
			cp3.LinkMap[49].Right = 53;
			cp3.LinkMap[0].Left = 9;
			cp3.LinkMap[10].Left = 54;
			cp3.LinkMap[20].Left = 55;
			cp3.LinkMap[30].Left = 56;
			cp3.LinkMap[40].Left = 57;
			cp3.LinkMap.Add(300, new UILinkPoint(300, true, 302, 301, 49, -2));
			cp3.LinkMap.Add(301, new UILinkPoint(301, true, 300, 302, 53, 50));
			cp3.LinkMap.Add(302, new UILinkPoint(302, true, 301, 300, 57, 54));
			cp3.LinkMap[301].OnSpecialInteracts += value;
			cp3.LinkMap[302].OnSpecialInteracts += value;
			cp3.LinkMap[300].OnSpecialInteracts += value3;
			cp3.UpdateEvent += delegate
			{
				bool inReforgeMenu = Main.InReforgeMenu;
				bool flag = Main.player[Main.myPlayer].chest != -1;
				bool flag2 = Main.npcShop != 0;
				for (int num24 = 40; num24 <= 49; num24++)
				{
					if (inReforgeMenu)
					{
						cp3.LinkMap[num24].Down = ((num24 < 45) ? 303 : 304);
					}
					else if (flag)
					{
						cp3.LinkMap[num24].Down = 400 + num24 - 40;
					}
					else if (flag2)
					{
						cp3.LinkMap[num24].Down = 2700 + num24 - 40;
					}
					else
					{
						cp3.LinkMap[num24].Down = -2;
					}
				}
				if (flag)
				{
					cp3.LinkMap[300].Up = 439;
					cp3.LinkMap[300].Right = -4;
					cp3.LinkMap[300].Left = -3;
				}
				else if (flag2)
				{
					cp3.LinkMap[300].Up = 2739;
					cp3.LinkMap[300].Right = -4;
					cp3.LinkMap[300].Left = -3;
				}
				else
				{
					cp3.LinkMap[300].Up = 49;
					cp3.LinkMap[300].Right = 301;
					cp3.LinkMap[300].Left = 302;
					cp3.LinkMap[49].Down = 300;
				}
				cp3.LinkMap[10].Left = 54;
				cp3.LinkMap[20].Left = 55;
				cp3.LinkMap[30].Left = 56;
				cp3.LinkMap[40].Left = 57;
				if (UILinkPointNavigator.Shortcuts.BUILDERACCCOUNT > 8)
				{
					cp3.LinkMap[0].Left = 4000;
					cp3.LinkMap[10].Left = 4002;
					cp3.LinkMap[20].Left = 4004;
					cp3.LinkMap[30].Left = 4006;
					cp3.LinkMap[40].Left = 4008;
				}
				else
				{
					cp3.LinkMap[0].Left = 9;
					if (UILinkPointNavigator.Shortcuts.BUILDERACCCOUNT > 0)
					{
						cp3.LinkMap[10].Left = 4000;
					}
					if (UILinkPointNavigator.Shortcuts.BUILDERACCCOUNT > 2)
					{
						cp3.LinkMap[20].Left = 4002;
					}
					if (UILinkPointNavigator.Shortcuts.BUILDERACCCOUNT > 4)
					{
						cp3.LinkMap[30].Left = 4004;
					}
					if (UILinkPointNavigator.Shortcuts.BUILDERACCCOUNT > 6)
					{
						cp3.LinkMap[40].Left = 4006;
					}
				}
				cp3.PageOnLeft = (Main.InReforgeMenu ? 5 : 9);
			};
			cp3.IsValidEvent += (() => Main.playerInventory);
			cp3.PageOnLeft = 9;
			cp3.PageOnRight = 2;
			UILinkPointNavigator.RegisterPage(cp3, 0, true);
			UILinkPage cp4 = new UILinkPage();
			cp4.OnSpecialInteracts += (() => PlayerInput.BuildCommand(Lang.misc[56], false, new List<string>[]
				{
					PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]
				}) + PlayerInput.BuildCommand(Lang.misc[64], true, new List<string>[]
				{
					PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"],
					PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]
				}));
			Func<string> value4 = delegate
			{
				int currentPoint = UILinkPointNavigator.CurrentPoint;
				return ItemSlot.GetGamepadInstructions(Main.player[Main.myPlayer].inventory, 1, currentPoint);
			};
			for (int k = 50; k <= 53; k++)
			{
				UILinkPoint uILinkPoint2 = new UILinkPoint(k, true, -3, -4, k - 1, k + 1);
				uILinkPoint2.OnSpecialInteracts += value4;
				cp4.LinkMap.Add(k, uILinkPoint2);
			}
			cp4.LinkMap[50].Left = 19;
			cp4.LinkMap[51].Left = 29;
			cp4.LinkMap[52].Left = 39;
			cp4.LinkMap[53].Left = 49;
			cp4.LinkMap[50].Right = 54;
			cp4.LinkMap[51].Right = 55;
			cp4.LinkMap[52].Right = 56;
			cp4.LinkMap[53].Right = 57;
			cp4.LinkMap[50].Up = -1;
			cp4.LinkMap[53].Down = -2;
			cp4.UpdateEvent += delegate
			{
				if (Main.player[Main.myPlayer].chest == -1 && Main.npcShop == 0)
				{
					cp4.LinkMap[50].Up = 301;
					cp4.LinkMap[53].Down = 301;
					return;
				}
				cp4.LinkMap[50].Up = 504;
				cp4.LinkMap[53].Down = 500;
			};
			cp4.IsValidEvent += (() => Main.playerInventory);
			cp4.PageOnLeft = 0;
			cp4.PageOnRight = 2;
			UILinkPointNavigator.RegisterPage(cp4, 1, true);
			UILinkPage cp5 = new UILinkPage();
			cp5.OnSpecialInteracts += (() => PlayerInput.BuildCommand(Lang.misc[56], false, new List<string>[]
				{
					PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]
				}) + PlayerInput.BuildCommand(Lang.misc[64], true, new List<string>[]
				{
					PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"],
					PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]
				}));
			Func<string> value5 = delegate
			{
				int currentPoint = UILinkPointNavigator.CurrentPoint;
				return ItemSlot.GetGamepadInstructions(Main.player[Main.myPlayer].inventory, 2, currentPoint);
			};
			for (int l = 54; l <= 57; l++)
			{
				UILinkPoint uILinkPoint3 = new UILinkPoint(l, true, -3, -4, l - 1, l + 1);
				uILinkPoint3.OnSpecialInteracts += value5;
				cp5.LinkMap.Add(l, uILinkPoint3);
			}
			cp5.LinkMap[54].Left = 50;
			cp5.LinkMap[55].Left = 51;
			cp5.LinkMap[56].Left = 52;
			cp5.LinkMap[57].Left = 53;
			cp5.LinkMap[54].Right = 10;
			cp5.LinkMap[55].Right = 20;
			cp5.LinkMap[56].Right = 30;
			cp5.LinkMap[57].Right = 40;
			cp5.LinkMap[54].Up = -1;
			cp5.LinkMap[57].Down = -2;
			cp5.UpdateEvent += delegate
			{
				if (Main.player[Main.myPlayer].chest == -1 && Main.npcShop == 0)
				{
					cp5.LinkMap[54].Up = 302;
					cp5.LinkMap[57].Down = 302;
					return;
				}
				cp5.LinkMap[54].Up = 504;
				cp5.LinkMap[57].Down = 500;
			};
			cp5.PageOnLeft = 0;
			cp5.PageOnRight = 8;
			UILinkPointNavigator.RegisterPage(cp5, 2, true);
			UILinkPage cp6 = new UILinkPage();
			cp6.OnSpecialInteracts += (() => PlayerInput.BuildCommand(Lang.misc[56], false, new List<string>[]
				{
					PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]
				}) + PlayerInput.BuildCommand(Lang.misc[64], true, new List<string>[]
				{
					PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"],
					PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]
				}));
			Func<string> value6 = delegate
			{
				int num24 = UILinkPointNavigator.CurrentPoint - 100;
				return ItemSlot.GetGamepadInstructions(Main.player[Main.myPlayer].armor, (num24 < 10) ? 8 : 9, num24);
			};
			Func<string> value7 = delegate
			{
				int slot = UILinkPointNavigator.CurrentPoint - 120;
				return ItemSlot.GetGamepadInstructions(Main.player[Main.myPlayer].dye, 12, slot);
			};
			for (int m = 100; m <= 119; m++)
			{
				UILinkPoint uILinkPoint4 = new UILinkPoint(m, true, m + 10, m - 10, m - 1, m + 1);
				uILinkPoint4.OnSpecialInteracts += value6;
				int num2 = m - 100;
				if (num2 == 0)
				{
					uILinkPoint4.Up = 305;
				}
				if (num2 == 10)
				{
					uILinkPoint4.Up = 306;
				}
				if (num2 == 9 || num2 == 19)
				{
					uILinkPoint4.Down = -2;
				}
				if (num2 >= 10)
				{
					uILinkPoint4.Left = 120 + num2 % 10;
				}
				else
				{
					uILinkPoint4.Right = -4;
				}
				cp6.LinkMap.Add(m, uILinkPoint4);
			}
			for (int n = 120; n <= 129; n++)
			{
				UILinkPoint uILinkPoint4 = new UILinkPoint(n, true, -3, -10 + n, n - 1, n + 1);
				uILinkPoint4.OnSpecialInteracts += value7;
				int num3 = n - 120;
				if (num3 == 0)
				{
					uILinkPoint4.Up = 307;
				}
				if (num3 == 9)
				{
					uILinkPoint4.Down = 308;
					uILinkPoint4.Left = 1557;
				}
				cp6.LinkMap.Add(n, uILinkPoint4);
			}
			cp6.IsValidEvent += (() => Main.playerInventory && Main.EquipPage == 0);
			cp6.UpdateEvent += delegate
			{
				int num24 = 107;
				int extraAccessorySlots = Main.player[Main.myPlayer].extraAccessorySlots;
				for (int num25 = 0; num25 < extraAccessorySlots; num25++)
				{
					cp6.LinkMap[num24 + num25].Down = num24 + num25 + 1;
					cp6.LinkMap[num24 - 100 + 120 + num25].Down = num24 - 100 + 120 + num25 + 1;
					cp6.LinkMap[num24 + 10 + num25].Down = num24 + 10 + num25 + 1;
				}
				cp6.LinkMap[num24 + extraAccessorySlots].Down = 308;
				cp6.LinkMap[num24 - 100 + 120 + extraAccessorySlots].Down = 308;
				cp6.LinkMap[num24 + 10 + extraAccessorySlots].Down = 308;
				bool shouldPVPDraw = Main.ShouldPVPDraw;
				for (int num26 = 120; num26 <= 129; num26++)
				{
					UILinkPoint uILinkPoint15 = cp6.LinkMap[num26];
					int num27 = num26 - 120;
					if (num27 == 0)
					{
						uILinkPoint15.Left = (shouldPVPDraw ? 1550 : -3);
					}
					if (num27 == 1)
					{
						uILinkPoint15.Left = (shouldPVPDraw ? 1552 : -3);
					}
					if (num27 == 2)
					{
						uILinkPoint15.Left = (shouldPVPDraw ? 1556 : -3);
					}
					if (num27 == 3)
					{
						uILinkPoint15.Left = ((UILinkPointNavigator.Shortcuts.INFOACCCOUNT >= 1) ? 1558 : -3);
					}
					if (num27 == 4)
					{
						uILinkPoint15.Left = ((UILinkPointNavigator.Shortcuts.INFOACCCOUNT >= 5) ? 1562 : -3);
					}
					if (num27 == 5)
					{
						uILinkPoint15.Left = ((UILinkPointNavigator.Shortcuts.INFOACCCOUNT >= 9) ? 1566 : -3);
					}
					if (num27 == 7)
					{
						uILinkPoint15.Left = (shouldPVPDraw ? 1557 : -3);
					}
				}
			};
			cp6.PageOnLeft = 8;
			cp6.PageOnRight = 8;
			UILinkPointNavigator.RegisterPage(cp6, 3, true);
			UILinkPage uILinkPage2 = new UILinkPage();
			uILinkPage2.OnSpecialInteracts += (() => PlayerInput.BuildCommand(Lang.misc[56], false, new List<string>[]
				{
					PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]
				}) + PlayerInput.BuildCommand(Lang.misc[64], true, new List<string>[]
				{
					PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"],
					PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]
				}));
			Func<string> value8 = delegate
			{
				int slot = UILinkPointNavigator.CurrentPoint - 400;
				int context = 4;
				Item[] item = Main.player[Main.myPlayer].bank.item;
				switch (Main.player[Main.myPlayer].chest)
				{
					case -4:
						item = Main.player[Main.myPlayer].bank3.item;
						break;
					case -3:
						item = Main.player[Main.myPlayer].bank2.item;
						break;
					case -2:
						break;
					case -1:
						return "";
					default:
						item = Main.chest[Main.player[Main.myPlayer].chest].item;
						context = 3;
						break;
				}
				return ItemSlot.GetGamepadInstructions(item, context, slot);
			};
			for (int num4 = 400; num4 <= 439; num4++)
			{
				UILinkPoint uILinkPoint5 = new UILinkPoint(num4, true, num4 - 1, num4 + 1, num4 - 10, num4 + 10);
				uILinkPoint5.OnSpecialInteracts += value8;
				int num5 = num4 - 400;
				if (num5 < 10)
				{
					uILinkPoint5.Up = 40 + num5;
				}
				if (num5 >= 30)
				{
					uILinkPoint5.Down = -2;
				}
				if (num5 % 10 == 9)
				{
					uILinkPoint5.Right = -4;
				}
				if (num5 % 10 == 0)
				{
					uILinkPoint5.Left = -3;
				}
				uILinkPage2.LinkMap.Add(num4, uILinkPoint5);
			}
			uILinkPage2.LinkMap.Add(500, new UILinkPoint(500, true, 409, -4, 53, 501));
			uILinkPage2.LinkMap.Add(501, new UILinkPoint(501, true, 419, -4, 500, 502));
			uILinkPage2.LinkMap.Add(502, new UILinkPoint(502, true, 429, -4, 501, 503));
			uILinkPage2.LinkMap.Add(503, new UILinkPoint(503, true, 439, -4, 502, 505));
			uILinkPage2.LinkMap.Add(505, new UILinkPoint(505, true, 439, -4, 503, 504));
			uILinkPage2.LinkMap.Add(504, new UILinkPoint(504, true, 439, -4, 505, 50));
			uILinkPage2.LinkMap[500].OnSpecialInteracts += value;
			uILinkPage2.LinkMap[501].OnSpecialInteracts += value;
			uILinkPage2.LinkMap[502].OnSpecialInteracts += value;
			uILinkPage2.LinkMap[503].OnSpecialInteracts += value;
			uILinkPage2.LinkMap[504].OnSpecialInteracts += value;
			uILinkPage2.LinkMap[505].OnSpecialInteracts += value;
			uILinkPage2.LinkMap[409].Right = 500;
			uILinkPage2.LinkMap[419].Right = 501;
			uILinkPage2.LinkMap[429].Right = 502;
			uILinkPage2.LinkMap[439].Right = 503;
			uILinkPage2.LinkMap[439].Down = 300;
			uILinkPage2.PageOnLeft = 0;
			uILinkPage2.PageOnRight = 0;
			uILinkPage2.DefaultPoint = 500;
			UILinkPointNavigator.RegisterPage(uILinkPage2, 4, false);
			uILinkPage2.IsValidEvent += (() => Main.playerInventory && Main.player[Main.myPlayer].chest != -1);
			UILinkPage uILinkPage3 = new UILinkPage();
			uILinkPage3.OnSpecialInteracts += (() => PlayerInput.BuildCommand(Lang.misc[56], false, new List<string>[]
				{
					PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]
				}) + PlayerInput.BuildCommand(Lang.misc[64], true, new List<string>[]
				{
					PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"],
					PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]
				}));
			Func<string> value9 = delegate
			{
				int slot = UILinkPointNavigator.CurrentPoint - 2700;
				return ItemSlot.GetGamepadInstructions(Main.instance.shop[Main.npcShop].item, 15, slot);
			};
			for (int num6 = 2700; num6 <= 2739; num6++)
			{
				UILinkPoint uILinkPoint6 = new UILinkPoint(num6, true, num6 - 1, num6 + 1, num6 - 10, num6 + 10);
				uILinkPoint6.OnSpecialInteracts += value9;
				int num7 = num6 - 2700;
				if (num7 < 10)
				{
					uILinkPoint6.Up = 40 + num7;
				}
				if (num7 >= 30)
				{
					uILinkPoint6.Down = -2;
				}
				if (num7 % 10 == 9)
				{
					uILinkPoint6.Right = -4;
				}
				if (num7 % 10 == 0)
				{
					uILinkPoint6.Left = -3;
				}
				uILinkPage3.LinkMap.Add(num6, uILinkPoint6);
			}
			uILinkPage3.LinkMap[2739].Down = 300;
			uILinkPage3.PageOnLeft = 0;
			uILinkPage3.PageOnRight = 0;
			UILinkPointNavigator.RegisterPage(uILinkPage3, 13, true);
			uILinkPage3.IsValidEvent += (() => Main.playerInventory && Main.npcShop != 0);
			UILinkPage cp7 = new UILinkPage();
			cp7.LinkMap.Add(303, new UILinkPoint(303, true, 304, 304, 40, -2));
			cp7.LinkMap.Add(304, new UILinkPoint(304, true, 303, 303, 40, -2));
			cp7.OnSpecialInteracts += (() => PlayerInput.BuildCommand(Lang.misc[56], false, new List<string>[]
				{
					PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]
				}) + PlayerInput.BuildCommand(Lang.misc[64], true, new List<string>[]
				{
					PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"],
					PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]
				}));
			Func<string> value10 = () => ItemSlot.GetGamepadInstructions(ref Main.reforgeItem, 5);
			cp7.LinkMap[303].OnSpecialInteracts += value10;
			cp7.LinkMap[304].OnSpecialInteracts += (() => Lang.misc[53]);
			cp7.UpdateEvent += delegate
			{
				bool flag = Main.reforgeItem.type > 0;
				if (flag)
				{
					cp7.LinkMap[303].Left = (cp7.LinkMap[303].Right = 304);
					return;
				}
				if (UILinkPointNavigator.OverridePoint == -1 && cp7.CurrentPoint == 304)
				{
					UILinkPointNavigator.ChangePoint(303);
				}
				cp7.LinkMap[303].Left = -3;
				cp7.LinkMap[303].Right = -4;
			};
			cp7.IsValidEvent += (() => Main.playerInventory && Main.InReforgeMenu);
			cp7.PageOnLeft = 0;
			cp7.PageOnRight = 0;
			UILinkPointNavigator.RegisterPage(cp7, 5, true);
			UILinkPage cp8 = new UILinkPage();
			cp8.OnSpecialInteracts += delegate
			{
				if (PlayerInput.Triggers.JustPressed.Grapple)
				{
					Point point = Main.player[Main.myPlayer].Center.ToTileCoordinates();
					if (UILinkPointNavigator.CurrentPoint == 600)
					{
						if (WorldGen.MoveNPC(point.X, point.Y, -1))
						{
							Main.NewText(Lang.inter[39], 255, 240, 20, false);
						}
						Main.PlaySound(12, -1, -1, 1, 1f, 0f);
					}
					else if (WorldGen.MoveNPC(point.X, point.Y, UILinkPointNavigator.Shortcuts.NPCS_LastHovered))
					{
						WorldGen.moveRoom(point.X, point.Y, UILinkPointNavigator.Shortcuts.NPCS_LastHovered);
						Main.PlaySound(12, -1, -1, 1, 1f, 0f);
					}
				}
				if (PlayerInput.Triggers.JustPressed.SmartSelect)
				{
					UILinkPointNavigator.Shortcuts.NPCS_IconsDisplay = !UILinkPointNavigator.Shortcuts.NPCS_IconsDisplay;
				}
				return PlayerInput.BuildCommand(Lang.misc[56], false, new List<string>[]
					{
						PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]
					}) + PlayerInput.BuildCommand(Lang.misc[64], false, new List<string>[]
					{
						PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"],
						PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]
					}) + PlayerInput.BuildCommand(Lang.misc[70], false, new List<string>[]
					{
						PlayerInput.ProfileGamepadUI.KeyStatus["Grapple"]
					}) + PlayerInput.BuildCommand(Lang.misc[69], true, new List<string>[]
					{
						PlayerInput.ProfileGamepadUI.KeyStatus["SmartSelect"]
					});
			};
			for (int num8 = 600; num8 <= 650; num8++)
			{
				UILinkPoint value11 = new UILinkPoint(num8, true, num8 + 10, num8 - 10, num8 - 1, num8 + 1);
				cp8.LinkMap.Add(num8, value11);
			}
			cp8.UpdateEvent += delegate
			{
				int num24 = UILinkPointNavigator.Shortcuts.NPCS_IconsPerColumn;
				if (num24 == 0)
				{
					num24 = 100;
				}
				for (int num25 = 0; num25 < 50; num25++)
				{
					cp8.LinkMap[600 + num25].Up = ((num25 % num24 == 0) ? -1 : (600 + num25 - 1));
					if (cp8.LinkMap[600 + num25].Up == -1)
					{
						if (num25 >= num24 * 2)
						{
							cp8.LinkMap[600 + num25].Up = 307;
						}
						else if (num25 >= num24)
						{
							cp8.LinkMap[600 + num25].Up = 306;
						}
						else
						{
							cp8.LinkMap[600 + num25].Up = 305;
						}
					}
					cp8.LinkMap[600 + num25].Down = (((num25 + 1) % num24 == 0 || num25 == UILinkPointNavigator.Shortcuts.NPCS_IconsTotal - 1) ? 308 : (600 + num25 + 1));
					cp8.LinkMap[600 + num25].Left = ((num25 < UILinkPointNavigator.Shortcuts.NPCS_IconsTotal - num24) ? (600 + num25 + num24) : -3);
					cp8.LinkMap[600 + num25].Right = ((num25 < num24) ? -4 : (600 + num25 - num24));
				}
			};
			cp8.IsValidEvent += (() => Main.playerInventory && Main.EquipPage == 1);
			cp8.PageOnLeft = 8;
			cp8.PageOnRight = 8;
			UILinkPointNavigator.RegisterPage(cp8, 6, true);
			UILinkPage cp9 = new UILinkPage();
			cp9.OnSpecialInteracts += (() => PlayerInput.BuildCommand(Lang.misc[56], false, new List<string>[]
				{
					PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]
				}) + PlayerInput.BuildCommand(Lang.misc[64], true, new List<string>[]
				{
					PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"],
					PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]
				}));
			Func<string> value12 = delegate
			{
				int slot = UILinkPointNavigator.CurrentPoint - 180;
				return ItemSlot.GetGamepadInstructions(Main.player[Main.myPlayer].miscEquips, 20, slot);
			};
			Func<string> value13 = delegate
			{
				int slot = UILinkPointNavigator.CurrentPoint - 180;
				return ItemSlot.GetGamepadInstructions(Main.player[Main.myPlayer].miscEquips, 19, slot);
			};
			Func<string> value14 = delegate
			{
				int slot = UILinkPointNavigator.CurrentPoint - 180;
				return ItemSlot.GetGamepadInstructions(Main.player[Main.myPlayer].miscEquips, 18, slot);
			};
			Func<string> value15 = delegate
			{
				int slot = UILinkPointNavigator.CurrentPoint - 180;
				return ItemSlot.GetGamepadInstructions(Main.player[Main.myPlayer].miscEquips, 17, slot);
			};
			Func<string> value16 = delegate
			{
				int slot = UILinkPointNavigator.CurrentPoint - 180;
				return ItemSlot.GetGamepadInstructions(Main.player[Main.myPlayer].miscEquips, 16, slot);
			};
			Func<string> value17 = delegate
			{
				int slot = UILinkPointNavigator.CurrentPoint - 185;
				return ItemSlot.GetGamepadInstructions(Main.player[Main.myPlayer].miscDyes, 12, slot);
			};
			for (int num9 = 180; num9 <= 184; num9++)
			{
				UILinkPoint uILinkPoint7 = new UILinkPoint(num9, true, 185 + num9 - 180, -4, num9 - 1, num9 + 1);
				int num10 = num9 - 180;
				if (num10 == 0)
				{
					uILinkPoint7.Up = 305;
				}
				if (num10 == 4)
				{
					uILinkPoint7.Down = 308;
				}
				cp9.LinkMap.Add(num9, uILinkPoint7);
				switch (num9)
				{
					case 180:
						uILinkPoint7.OnSpecialInteracts += value13;
						break;
					case 181:
						uILinkPoint7.OnSpecialInteracts += value12;
						break;
					case 182:
						uILinkPoint7.OnSpecialInteracts += value14;
						break;
					case 183:
						uILinkPoint7.OnSpecialInteracts += value15;
						break;
					case 184:
						uILinkPoint7.OnSpecialInteracts += value16;
						break;
				}
			}
			for (int num11 = 185; num11 <= 189; num11++)
			{
				UILinkPoint uILinkPoint7 = new UILinkPoint(num11, true, -3, -5 + num11, num11 - 1, num11 + 1);
				uILinkPoint7.OnSpecialInteracts += value17;
				int num12 = num11 - 185;
				if (num12 == 0)
				{
					uILinkPoint7.Up = 306;
				}
				if (num12 == 4)
				{
					uILinkPoint7.Down = 308;
				}
				cp9.LinkMap.Add(num11, uILinkPoint7);
			}
			cp9.UpdateEvent += delegate
			{
				cp9.LinkMap[184].Down = ((UILinkPointNavigator.Shortcuts.BUFFS_DRAWN > 0) ? 9000 : 308);
				cp9.LinkMap[189].Down = ((UILinkPointNavigator.Shortcuts.BUFFS_DRAWN > 0) ? 9000 : 308);
			};
			cp9.IsValidEvent += (() => Main.playerInventory && Main.EquipPage == 2);
			cp9.PageOnLeft = 8;
			cp9.PageOnRight = 8;
			UILinkPointNavigator.RegisterPage(cp9, 7, true);
			UILinkPage cp10 = new UILinkPage();
			cp10.OnSpecialInteracts += (() => PlayerInput.BuildCommand(Lang.misc[56], false, new List<string>[]
				{
					PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]
				}) + PlayerInput.BuildCommand(Lang.misc[64], true, new List<string>[]
				{
					PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"],
					PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]
				}));
			cp10.LinkMap.Add(305, new UILinkPoint(305, true, 306, -4, 308, -2));
			cp10.LinkMap.Add(306, new UILinkPoint(306, true, 307, 305, 308, -2));
			cp10.LinkMap.Add(307, new UILinkPoint(307, true, -3, 306, 308, -2));
			cp10.LinkMap.Add(308, new UILinkPoint(308, true, -3, -4, -1, 305));
			cp10.LinkMap[305].OnSpecialInteracts += value;
			cp10.LinkMap[306].OnSpecialInteracts += value;
			cp10.LinkMap[307].OnSpecialInteracts += value;
			cp10.LinkMap[308].OnSpecialInteracts += value;
			cp10.UpdateEvent += delegate
			{
				switch (Main.EquipPage)
				{
					case 0:
						cp10.LinkMap[305].Down = 100;
						cp10.LinkMap[306].Down = 110;
						cp10.LinkMap[307].Down = 120;
						cp10.LinkMap[308].Up = 108 + Main.player[Main.myPlayer].extraAccessorySlots - 1;
						return;
					case 1:
						{
							cp10.LinkMap[305].Down = 600;
							cp10.LinkMap[306].Down = ((UILinkPointNavigator.Shortcuts.NPCS_IconsTotal / UILinkPointNavigator.Shortcuts.NPCS_IconsPerColumn > 0) ? (600 + UILinkPointNavigator.Shortcuts.NPCS_IconsPerColumn) : -2);
							cp10.LinkMap[307].Down = ((UILinkPointNavigator.Shortcuts.NPCS_IconsTotal / UILinkPointNavigator.Shortcuts.NPCS_IconsPerColumn > 1) ? (600 + UILinkPointNavigator.Shortcuts.NPCS_IconsPerColumn * 2) : -2);
							int num24 = UILinkPointNavigator.Shortcuts.NPCS_IconsPerColumn;
							if (num24 == 0)
							{
								num24 = 100;
							}
							if (num24 == 100)
							{
								num24 = UILinkPointNavigator.Shortcuts.NPCS_IconsTotal;
							}
							cp10.LinkMap[308].Up = 600 + num24 - 1;
							return;
						}
					case 2:
						cp10.LinkMap[305].Down = 180;
						cp10.LinkMap[306].Down = 185;
						cp10.LinkMap[307].Down = -2;
						cp10.LinkMap[308].Up = ((UILinkPointNavigator.Shortcuts.BUFFS_DRAWN > 0) ? 9000 : 184);
						break;
					case 3:
						break;
					default:
						return;
				}
			};
			cp10.IsValidEvent += (() => Main.playerInventory);
			cp10.PageOnLeft = 0;
			cp10.PageOnRight = 0;
			UILinkPointNavigator.RegisterPage(cp10, 8, true);
			UILinkPage cp11 = new UILinkPage();
			cp11.OnSpecialInteracts += (() => PlayerInput.BuildCommand(Lang.misc[56], false, new List<string>[]
				{
					PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]
				}) + PlayerInput.BuildCommand(Lang.misc[64], true, new List<string>[]
				{
					PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"],
					PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]
				}));
			Func<string> value18 = () => ItemSlot.GetGamepadInstructions(ref Main.guideItem, 7);
			Func<string> HandleItem2 = delegate
			{
				if (Main.mouseItem.type < 1)
				{
					return "";
				}
				return ItemSlot.GetGamepadInstructions(ref Main.mouseItem, 22);
			};
			for (int num13 = 1500; num13 < 1550; num13++)
			{
				UILinkPoint uILinkPoint8 = new UILinkPoint(num13, true, num13, num13, -1, -2);
				if (num13 != 1500)
				{
					uILinkPoint8.OnSpecialInteracts += HandleItem2;
				}
				cp11.LinkMap.Add(num13, uILinkPoint8);
			}
			cp11.LinkMap[1500].OnSpecialInteracts += value18;
			cp11.UpdateEvent += delegate
			{
				int num24 = UILinkPointNavigator.Shortcuts.CRAFT_CurrentIngridientsCount;
				int num25 = num24;
				if (Main.numAvailableRecipes > 0)
				{
					num25 += 2;
				}
				if (num24 < num25)
				{
					num24 = num25;
				}
				if (UILinkPointNavigator.OverridePoint == -1 && cp11.CurrentPoint > 1500 + num24)
				{
					UILinkPointNavigator.ChangePoint(1500);
				}
				if (UILinkPointNavigator.OverridePoint == -1 && cp11.CurrentPoint == 1500 && !Main.InGuideCraftMenu)
				{
					UILinkPointNavigator.ChangePoint(1501);
				}
				for (int num26 = 1; num26 < num24; num26++)
				{
					cp11.LinkMap[1500 + num26].Left = 1500 + num26 - 1;
					cp11.LinkMap[1500 + num26].Right = ((num26 == num24 - 2) ? -4 : (1500 + num26 + 1));
				}
				cp11.LinkMap[1501].Left = -3;
				cp11.LinkMap[1500 + num24 - 1].Right = -4;
				cp11.LinkMap[1500].Down = ((num24 >= 2) ? 1502 : -2);
				cp11.LinkMap[1500].Left = ((num24 >= 1) ? 1501 : -3);
				cp11.LinkMap[1502].Up = (Main.InGuideCraftMenu ? 1500 : -1);
			};
			cp11.LinkMap[1501].OnSpecialInteracts += delegate
			{
				if (Main.InGuideCraftMenu)
				{
					return "";
				}
				string str = "";
				Player player = Main.player[Main.myPlayer];
				bool flag = false;
				if (Main.mouseItem.type == 0 && player.ItemSpace(Main.recipe[Main.availableRecipe[Main.focusRecipe]].createItem) && !player.IsStackingItems())
				{
					flag = true;
					if (PlayerInput.Triggers.Current.Grapple && Main.stackSplit <= 1)
					{
						if (PlayerInput.Triggers.JustPressed.Grapple)
						{
							UILinksInitializer.SomeVarsForUILinkers.SequencedCraftingCurrent = Main.recipe[Main.availableRecipe[Main.focusRecipe]];
						}
						if (Main.stackSplit == 0)
						{
							Main.stackSplit = 15;
						}
						else
						{
							Main.stackSplit = Main.stackDelay;
						}
						if (UILinksInitializer.SomeVarsForUILinkers.SequencedCraftingCurrent == Main.recipe[Main.availableRecipe[Main.focusRecipe]])
						{
							Main.CraftItem(Main.recipe[Main.availableRecipe[Main.focusRecipe]]);
							Main.mouseItem = player.GetItem(player.whoAmI, Main.mouseItem, false, false);
						}
					}
				}
				else if (Main.mouseItem.type > 0 && Main.mouseItem.maxStack == 1 && ItemSlot.Equippable(ref Main.mouseItem, 0))
				{
					str += PlayerInput.BuildCommand(Lang.misc[67], false, new List<string>[]
						{
							PlayerInput.ProfileGamepadUI.KeyStatus["Grapple"]
						});
					if (PlayerInput.Triggers.JustPressed.Grapple)
					{
						ItemSlot.SwapEquip(ref Main.mouseItem, 0);
						if (Main.player[Main.myPlayer].ItemSpace(Main.mouseItem))
						{
							Main.mouseItem = player.GetItem(player.whoAmI, Main.mouseItem, false, false);
						}
					}
				}
				bool flag2 = Main.mouseItem.stack <= 0;
				if (flag2 || (Main.mouseItem.type == Main.recipe[Main.availableRecipe[Main.focusRecipe]].createItem.type && Main.mouseItem.stack < Main.mouseItem.maxStack))
				{
					if (flag2)
					{
						str += PlayerInput.BuildCommand(Lang.misc[72], false, new List<string>[]
							{
								PlayerInput.ProfileGamepadUI.KeyStatus["MouseLeft"],
								PlayerInput.ProfileGamepadUI.KeyStatus["MouseRight"]
							});
					}
					else
					{
						str += PlayerInput.BuildCommand(Lang.misc[72], false, new List<string>[]
							{
								PlayerInput.ProfileGamepadUI.KeyStatus["MouseLeft"]
							});
					}
				}
				if (!flag2 && Main.mouseItem.type == Main.recipe[Main.availableRecipe[Main.focusRecipe]].createItem.type && Main.mouseItem.stack < Main.mouseItem.maxStack)
				{
					str += PlayerInput.BuildCommand(Lang.misc[93], false, new List<string>[]
						{
							PlayerInput.ProfileGamepadUI.KeyStatus["MouseRight"]
						});
				}
				if (flag)
				{
					str += PlayerInput.BuildCommand(Lang.misc[71], false, new List<string>[]
						{
							PlayerInput.ProfileGamepadUI.KeyStatus["Grapple"]
						});
				}
				return str + HandleItem2();
			};
			cp11.ReachEndEvent += delegate(int current, int next)
			{
				if (current == 1500)
				{
					return;
				}
				if (current == 1501)
				{
					if (next == -1)
					{
						if (Main.focusRecipe > 0)
						{
							Main.focusRecipe--;
							return;
						}
					}
					else if (next == -2 && Main.focusRecipe < Main.numAvailableRecipes - 1)
					{
						Main.focusRecipe++;
						return;
					}
				}
				else if (next == -1)
				{
					if (Main.focusRecipe > 0)
					{
						UILinkPointNavigator.ChangePoint(1501);
						Main.focusRecipe--;
						return;
					}
				}
				else if (next == -2 && Main.focusRecipe < Main.numAvailableRecipes - 1)
				{
					UILinkPointNavigator.ChangePoint(1501);
					Main.focusRecipe++;
				}
			};
			cp11.EnterEvent += delegate
			{
				Main.recBigList = false;
			};
			cp11.CanEnterEvent += (() => Main.playerInventory && (Main.numAvailableRecipes > 0 || Main.InGuideCraftMenu));
			cp11.IsValidEvent += (() => Main.playerInventory && (Main.numAvailableRecipes > 0 || Main.InGuideCraftMenu));
			cp11.PageOnLeft = 10;
			cp11.PageOnRight = 0;
			UILinkPointNavigator.RegisterPage(cp11, 9, true);
			UILinkPage cp12 = new UILinkPage();
			cp12.OnSpecialInteracts += (() => PlayerInput.BuildCommand(Lang.misc[56], false, new List<string>[]
				{
					PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]
				}) + PlayerInput.BuildCommand(Lang.misc[64], true, new List<string>[]
				{
					PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"],
					PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]
				}));
			for (int num14 = 700; num14 < 1500; num14++)
			{
				UILinkPoint uILinkPoint9 = new UILinkPoint(num14, true, num14, num14, num14, num14);
				int IHateLambda = num14;
				uILinkPoint9.OnSpecialInteracts += delegate
				{
					string text = "";
					bool flag = false;
					Player player = Main.player[Main.myPlayer];
					if (IHateLambda + Main.recStart < Main.numAvailableRecipes)
					{
						int num24 = Main.recStart + IHateLambda - 700;
						if (Main.mouseItem.type == 0 && player.ItemSpace(Main.recipe[Main.availableRecipe[num24]].createItem) && !player.IsStackingItems())
						{
							flag = true;
							if (PlayerInput.Triggers.JustPressed.Grapple)
							{
								UILinksInitializer.SomeVarsForUILinkers.SequencedCraftingCurrent = Main.recipe[Main.availableRecipe[num24]];
							}
							if (PlayerInput.Triggers.Current.Grapple && Main.stackSplit <= 1)
							{
								if (Main.stackSplit == 0)
								{
									Main.stackSplit = 15;
								}
								else
								{
									Main.stackSplit = Main.stackDelay;
								}
								if (UILinksInitializer.SomeVarsForUILinkers.SequencedCraftingCurrent == Main.recipe[Main.availableRecipe[num24]])
								{
									Main.CraftItem(Main.recipe[Main.availableRecipe[num24]]);
									Main.mouseItem = player.GetItem(player.whoAmI, Main.mouseItem, false, false);
								}
							}
						}
					}
					text += PlayerInput.BuildCommand(Lang.misc[73], !flag, new List<string>[]
						{
							PlayerInput.ProfileGamepadUI.KeyStatus["MouseLeft"]
						});
					if (flag)
					{
						text += PlayerInput.BuildCommand(Lang.misc[71], true, new List<string>[]
							{
								PlayerInput.ProfileGamepadUI.KeyStatus["Grapple"]
							});
					}
					return text;
				};
				cp12.LinkMap.Add(num14, uILinkPoint9);
			}
			cp12.UpdateEvent += delegate
			{
				int num24 = UILinkPointNavigator.Shortcuts.CRAFT_IconsPerRow;
				int cRAFT_IconsPerColumn = UILinkPointNavigator.Shortcuts.CRAFT_IconsPerColumn;
				if (num24 == 0)
				{
					num24 = 100;
				}
				int num25 = num24 * cRAFT_IconsPerColumn;
				if (num25 > 800)
				{
					num25 = 800;
				}
				if (num25 > Main.numAvailableRecipes)
				{
					num25 = Main.numAvailableRecipes;
				}
				for (int num26 = 0; num26 < num25; num26++)
				{
					cp12.LinkMap[700 + num26].Left = ((num26 % num24 == 0) ? -3 : (700 + num26 - 1));
					cp12.LinkMap[700 + num26].Right = (((num26 + 1) % num24 == 0 || num26 == Main.numAvailableRecipes - 1) ? -4 : (700 + num26 + 1));
					cp12.LinkMap[700 + num26].Down = ((num26 < num25 - num24) ? (700 + num26 + num24) : -2);
					cp12.LinkMap[700 + num26].Up = ((num26 < num24) ? -1 : (700 + num26 - num24));
				}
			};
			cp12.ReachEndEvent += delegate(int current, int next)
			{
				int cRAFT_IconsPerRow = UILinkPointNavigator.Shortcuts.CRAFT_IconsPerRow;
				if (next == -1)
				{
					Main.recStart -= cRAFT_IconsPerRow;
					if (Main.recStart < 0)
					{
						Main.recStart = 0;
						return;
					}
				}
				else if (next == -2)
				{
					Main.recStart += cRAFT_IconsPerRow;
					Main.PlaySound(12, -1, -1, 1, 1f, 0f);
					if (Main.recStart > Main.numAvailableRecipes - cRAFT_IconsPerRow)
					{
						Main.recStart = Main.numAvailableRecipes - cRAFT_IconsPerRow;
					}
				}
			};
			cp12.EnterEvent += delegate
			{
				Main.recBigList = true;
			};
			cp12.LeaveEvent += delegate
			{
				Main.recBigList = false;
			};
			cp12.CanEnterEvent += (() => Main.playerInventory && Main.numAvailableRecipes > 0);
			cp12.IsValidEvent += (() => Main.playerInventory && Main.recBigList && Main.numAvailableRecipes > 0);
			cp12.PageOnLeft = 0;
			cp12.PageOnRight = 9;
			UILinkPointNavigator.RegisterPage(cp12, 10, true);
			UILinkPage cp13 = new UILinkPage();
			cp13.OnSpecialInteracts += (() => PlayerInput.BuildCommand(Lang.misc[56], false, new List<string>[]
				{
					PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]
				}) + PlayerInput.BuildCommand(Lang.misc[64], true, new List<string>[]
				{
					PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"],
					PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]
				}));
			for (int num15 = 2605; num15 < 2620; num15++)
			{
				UILinkPoint uILinkPoint10 = new UILinkPoint(num15, true, num15, num15, num15, num15);
				uILinkPoint10.OnSpecialInteracts += (() => PlayerInput.BuildCommand(Lang.misc[73], true, new List<string>[]
					{
						PlayerInput.ProfileGamepadUI.KeyStatus["MouseLeft"]
					}));
				cp13.LinkMap.Add(num15, uILinkPoint10);
			}
			cp13.UpdateEvent += delegate
			{
				int num24 = 5;
				int num25 = 3;
				int num26 = num24 * num25;
				int num27 = Main.UnlockedMaxHair();
				for (int num28 = 0; num28 < num26; num28++)
				{
					cp13.LinkMap[2605 + num28].Left = ((num28 % num24 == 0) ? -3 : (2605 + num28 - 1));
					cp13.LinkMap[2605 + num28].Right = (((num28 + 1) % num24 == 0 || num28 == num27 - 1) ? -4 : (2605 + num28 + 1));
					cp13.LinkMap[2605 + num28].Down = ((num28 < num26 - num24) ? (2605 + num28 + num24) : -2);
					cp13.LinkMap[2605 + num28].Up = ((num28 < num24) ? -1 : (2605 + num28 - num24));
				}
			};
			cp13.ReachEndEvent += delegate(int current, int next)
			{
				int num24 = 5;
				if (next == -1)
				{
					Main.hairStart -= num24;
					Main.PlaySound(12, -1, -1, 1, 1f, 0f);
					return;
				}
				if (next == -2)
				{
					Main.hairStart += num24;
					Main.PlaySound(12, -1, -1, 1, 1f, 0f);
				}
			};
			cp13.CanEnterEvent += (() => Main.hairWindow);
			cp13.IsValidEvent += (() => Main.hairWindow);
			cp13.PageOnLeft = 12;
			cp13.PageOnRight = 12;
			UILinkPointNavigator.RegisterPage(cp13, 11, true);
			UILinkPage uILinkPage4 = new UILinkPage();
			uILinkPage4.OnSpecialInteracts += (() => PlayerInput.BuildCommand(Lang.misc[56], false, new List<string>[]
				{
					PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]
				}) + PlayerInput.BuildCommand(Lang.misc[64], true, new List<string>[]
				{
					PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"],
					PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]
				}));
			uILinkPage4.LinkMap.Add(2600, new UILinkPoint(2600, true, -3, -4, -1, 2601));
			uILinkPage4.LinkMap.Add(2601, new UILinkPoint(2601, true, -3, -4, 2600, 2602));
			uILinkPage4.LinkMap.Add(2602, new UILinkPoint(2602, true, -3, -4, 2601, 2603));
			uILinkPage4.LinkMap.Add(2603, new UILinkPoint(2603, true, -3, 2604, 2602, -2));
			uILinkPage4.LinkMap.Add(2604, new UILinkPoint(2604, true, 2603, -4, 2602, -2));
			uILinkPage4.UpdateEvent += delegate
			{
				Vector3 value20 = Main.rgbToHsl(Main.selColor);
				float interfaceDeadzoneX = PlayerInput.CurrentProfile.InterfaceDeadzoneX;
				float num24 = PlayerInput.GamepadThumbstickLeft.X;
				if (num24 < -interfaceDeadzoneX || num24 > interfaceDeadzoneX)
				{
					num24 = MathHelper.Lerp(0f, 0.008333334f, (Math.Abs(num24) - interfaceDeadzoneX) / (1f - interfaceDeadzoneX)) * (float)Math.Sign(num24);
				}
				else
				{
					num24 = 0f;
				}
				int currentPoint = UILinkPointNavigator.CurrentPoint;
				if (currentPoint == 2600)
				{
					Main.hBar = MathHelper.Clamp(Main.hBar + num24, 0f, 1f);
				}
				if (currentPoint == 2601)
				{
					Main.sBar = MathHelper.Clamp(Main.sBar + num24, 0f, 1f);
				}
				if (currentPoint == 2602)
				{
					Main.lBar = MathHelper.Clamp(Main.lBar + num24, 0.15f, 1f);
				}
				value20 = Vector3.Clamp(value20, Vector3.Zero, Vector3.One);
				if (num24 != 0f)
				{
					if (Main.hairWindow)
					{
						Main.player[Main.myPlayer].hairColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
					}
					Main.PlaySound(12, -1, -1, 1, 1f, 0f);
				}
			};
			uILinkPage4.CanEnterEvent += (() => Main.hairWindow);
			uILinkPage4.IsValidEvent += (() => Main.hairWindow);
			uILinkPage4.PageOnLeft = 11;
			uILinkPage4.PageOnRight = 11;
			UILinkPointNavigator.RegisterPage(uILinkPage4, 12, true);
			UILinkPage cp14 = new UILinkPage();
			for (int num16 = 0; num16 < 30; num16++)
			{
				cp14.LinkMap.Add(2900 + num16, new UILinkPoint(2900 + num16, true, -3, -4, -1, -2));
				cp14.LinkMap[2900 + num16].OnSpecialInteracts += value;
			}
			cp14.OnSpecialInteracts += (() => PlayerInput.BuildCommand(Lang.misc[56], false, new List<string>[]
				{
					PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]
				}) + PlayerInput.BuildCommand(Lang.misc[64], true, new List<string>[]
				{
					PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"],
					PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]
				}));
			cp14.TravelEvent += delegate
			{
				if (UILinkPointNavigator.CurrentPage == cp14.ID)
				{
					int num24 = cp14.CurrentPoint - 2900;
					if (num24 < 2)
					{
						IngameOptions.category = num24;
					}
				}
			};
			cp14.UpdateEvent += delegate
			{
				int num24 = UILinkPointNavigator.Shortcuts.INGAMEOPTIONS_BUTTONS_LEFT;
				if (num24 == 0)
				{
					num24 = 5;
				}
				if (UILinkPointNavigator.OverridePoint == -1 && cp14.CurrentPoint < 2930 && cp14.CurrentPoint > 2900 + num24 - 1)
				{
					UILinkPointNavigator.ChangePoint(2900);
				}
				for (int num25 = 2900; num25 < 2900 + num24; num25++)
				{
					cp14.LinkMap[num25].Up = num25 - 1;
					cp14.LinkMap[num25].Down = num25 + 1;
				}
				cp14.LinkMap[2900].Up = 2900 + num24 - 1;
				cp14.LinkMap[2900 + num24 - 1].Down = 2900;
				int num26 = cp14.CurrentPoint - 2900;
				if (num26 < 2 && PlayerInput.Triggers.JustPressed.MouseLeft)
				{
					IngameOptions.category = num26;
					UILinkPointNavigator.ChangePage(1002);
				}
			};
			cp14.EnterEvent += delegate
			{
				cp14.CurrentPoint = 2900 + IngameOptions.category;
			};
			cp14.PageOnLeft = (cp14.PageOnRight = 1002);
			cp14.IsValidEvent += (() => Main.ingameOptionsWindow && !Main.InGameUI.IsVisible);
			cp14.CanEnterEvent += (() => Main.ingameOptionsWindow && !Main.InGameUI.IsVisible);
			UILinkPointNavigator.RegisterPage(cp14, 1001, true);
			UILinkPage cp15 = new UILinkPage();
			for (int num17 = 0; num17 < 30; num17++)
			{
				cp15.LinkMap.Add(2930 + num17, new UILinkPoint(2930 + num17, true, -3, -4, -1, -2));
				cp15.LinkMap[2930 + num17].OnSpecialInteracts += value;
			}
			cp15.OnSpecialInteracts += (() => PlayerInput.BuildCommand(Lang.misc[56], false, new List<string>[]
				{
					PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]
				}) + PlayerInput.BuildCommand(Lang.misc[64], true, new List<string>[]
				{
					PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"],
					PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]
				}));
			cp15.UpdateEvent += delegate
			{
				int num24 = UILinkPointNavigator.Shortcuts.INGAMEOPTIONS_BUTTONS_RIGHT;
				if (num24 == 0)
				{
					num24 = 5;
				}
				if (UILinkPointNavigator.OverridePoint == -1 && cp15.CurrentPoint >= 2930 && cp15.CurrentPoint > 2930 + num24 - 1)
				{
					UILinkPointNavigator.ChangePoint(2930);
				}
				for (int num25 = 2930; num25 < 2930 + num24; num25++)
				{
					cp15.LinkMap[num25].Up = num25 - 1;
					cp15.LinkMap[num25].Down = num25 + 1;
				}
				cp15.LinkMap[2930].Up = -1;
				cp15.LinkMap[2930 + num24 - 1].Down = -2;
				bool arg_D7_0 = PlayerInput.Triggers.JustPressed.Inventory;
				UILinksInitializer.HandleOptionsSpecials();
			};
			cp15.PageOnLeft = (cp15.PageOnRight = 1001);
			cp15.IsValidEvent += (() => Main.ingameOptionsWindow);
			cp15.CanEnterEvent += (() => Main.ingameOptionsWindow);
			UILinkPointNavigator.RegisterPage(cp15, 1002, true);
			UILinkPage cp16 = new UILinkPage();
			cp16.OnSpecialInteracts += (() => PlayerInput.BuildCommand(Lang.misc[56], false, new List<string>[]
				{
					PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]
				}) + PlayerInput.BuildCommand(Lang.misc[64], true, new List<string>[]
				{
					PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"],
					PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]
				}));
			for (int num18 = 1550; num18 < 1558; num18++)
			{
				UILinkPoint uILinkPoint11 = new UILinkPoint(num18, true, -3, -4, -1, -2);
				switch (num18 - 1550)
				{
					case 1:
					case 3:
					case 5:
						uILinkPoint11.Up = uILinkPoint11.ID - 2;
						uILinkPoint11.Down = uILinkPoint11.ID + 2;
						uILinkPoint11.Right = uILinkPoint11.ID + 1;
						break;
					case 2:
					case 4:
					case 6:
						uILinkPoint11.Up = uILinkPoint11.ID - 2;
						uILinkPoint11.Down = uILinkPoint11.ID + 2;
						uILinkPoint11.Left = uILinkPoint11.ID - 1;
						break;
				}
				cp16.LinkMap.Add(num18, uILinkPoint11);
			}
			cp16.LinkMap[1550].Down = 1551;
			cp16.LinkMap[1550].Right = 120;
			cp16.LinkMap[1550].Up = 307;
			cp16.LinkMap[1551].Up = 1550;
			cp16.LinkMap[1552].Up = 1550;
			cp16.LinkMap[1552].Right = 121;
			cp16.LinkMap[1554].Right = 121;
			cp16.LinkMap[1555].Down = 1557;
			cp16.LinkMap[1556].Down = 1557;
			cp16.LinkMap[1556].Right = 122;
			cp16.LinkMap[1557].Up = 1555;
			cp16.LinkMap[1557].Down = 308;
			cp16.LinkMap[1557].Right = 127;
			for (int num19 = 0; num19 < 7; num19++)
			{
				cp16.LinkMap[1550 + num19].OnSpecialInteracts += value;
			}
			cp16.UpdateEvent += delegate
			{
				if (!Main.ShouldPVPDraw)
				{
					if (UILinkPointNavigator.OverridePoint == -1 && cp16.CurrentPoint != 1557)
					{
						UILinkPointNavigator.ChangePoint(1557);
					}
					cp16.LinkMap[1557].Up = -1;
					cp16.LinkMap[1557].Down = 308;
					cp16.LinkMap[1557].Right = 127;
				}
				else
				{
					cp16.LinkMap[1557].Up = 1555;
					cp16.LinkMap[1557].Down = 308;
					cp16.LinkMap[1557].Right = 127;
				}
				int iNFOACCCOUNT = UILinkPointNavigator.Shortcuts.INFOACCCOUNT;
				if (iNFOACCCOUNT > 0)
				{
					cp16.LinkMap[1557].Up = 1558 + (iNFOACCCOUNT - 1) / 2 * 2;
				}
				if (Main.ShouldPVPDraw)
				{
					if (iNFOACCCOUNT >= 1)
					{
						cp16.LinkMap[1555].Down = 1558;
						cp16.LinkMap[1556].Down = 1558;
					}
					else
					{
						cp16.LinkMap[1555].Down = 1557;
						cp16.LinkMap[1556].Down = 1557;
					}
					if (iNFOACCCOUNT >= 2)
					{
						cp16.LinkMap[1556].Down = 1559;
						return;
					}
					cp16.LinkMap[1556].Down = 1557;
				}
			};
			cp16.IsValidEvent += (() => Main.playerInventory);
			cp16.PageOnLeft = 8;
			cp16.PageOnRight = 8;
			UILinkPointNavigator.RegisterPage(cp16, 16, true);
			UILinkPage cp17 = new UILinkPage();
			cp17.OnSpecialInteracts += (() => PlayerInput.BuildCommand(Lang.misc[56], false, new List<string>[]
				{
					PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]
				}) + PlayerInput.BuildCommand(Lang.misc[64], true, new List<string>[]
				{
					PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"],
					PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]
				}));
			for (int num20 = 1558; num20 < 1570; num20++)
			{
				UILinkPoint uILinkPoint12 = new UILinkPoint(num20, true, -3, -4, -1, -2);
				uILinkPoint12.OnSpecialInteracts += value;
				switch (num20 - 1558)
				{
					case 1:
					case 3:
					case 5:
						uILinkPoint12.Up = uILinkPoint12.ID - 2;
						uILinkPoint12.Down = uILinkPoint12.ID + 2;
						uILinkPoint12.Right = uILinkPoint12.ID + 1;
						break;
					case 2:
					case 4:
					case 6:
						uILinkPoint12.Up = uILinkPoint12.ID - 2;
						uILinkPoint12.Down = uILinkPoint12.ID + 2;
						uILinkPoint12.Left = uILinkPoint12.ID - 1;
						break;
				}
				cp17.LinkMap.Add(num20, uILinkPoint12);
			}
			cp17.UpdateEvent += delegate
			{
				int iNFOACCCOUNT = UILinkPointNavigator.Shortcuts.INFOACCCOUNT;
				if (UILinkPointNavigator.OverridePoint == -1 && cp17.CurrentPoint - 1558 >= iNFOACCCOUNT)
				{
					UILinkPointNavigator.ChangePoint(1558 + iNFOACCCOUNT - 1);
				}
				for (int num24 = 0; num24 < iNFOACCCOUNT; num24++)
				{
					bool flag = num24 % 2 == 0;
					int num25 = num24 + 1558;
					cp17.LinkMap[num25].Down = ((num24 < iNFOACCCOUNT - 2) ? (num25 + 2) : 1557);
					cp17.LinkMap[num25].Up = ((num24 > 1) ? (num25 - 2) : (Main.ShouldPVPDraw ? (flag ? 1555 : 1556) : -1));
					cp17.LinkMap[num25].Right = ((flag && num24 + 1 < iNFOACCCOUNT) ? (num25 + 1) : (123 + num24 / 4));
					cp17.LinkMap[num25].Left = (flag ? -3 : (num25 - 1));
				}
			};
			cp17.IsValidEvent += (() => Main.playerInventory && UILinkPointNavigator.Shortcuts.INFOACCCOUNT > 0);
			cp17.PageOnLeft = 8;
			cp17.PageOnRight = 8;
			UILinkPointNavigator.RegisterPage(cp17, 17, true);
			UILinkPage cp18 = new UILinkPage();
			cp18.OnSpecialInteracts += (() => PlayerInput.BuildCommand(Lang.misc[56], false, new List<string>[]
				{
					PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]
				}) + PlayerInput.BuildCommand(Lang.misc[64], true, new List<string>[]
				{
					PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"],
					PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]
				}));
			for (int num21 = 4000; num21 < 4010; num21++)
			{
				UILinkPoint uILinkPoint13 = new UILinkPoint(num21, true, -3, -4, -1, -2);
				switch (num21)
				{
					case 4000:
					case 4001:
						uILinkPoint13.Right = 0;
						break;
					case 4002:
					case 4003:
						uILinkPoint13.Right = 10;
						break;
					case 4004:
					case 4005:
						uILinkPoint13.Right = 20;
						break;
					case 4006:
					case 4007:
						uILinkPoint13.Right = 30;
						break;
					case 4008:
					case 4009:
						uILinkPoint13.Right = 40;
						break;
				}
				cp18.LinkMap.Add(num21, uILinkPoint13);
			}
			cp18.UpdateEvent += delegate
			{
				int bUILDERACCCOUNT = UILinkPointNavigator.Shortcuts.BUILDERACCCOUNT;
				if (UILinkPointNavigator.OverridePoint == -1 && cp18.CurrentPoint - 4000 >= bUILDERACCCOUNT)
				{
					UILinkPointNavigator.ChangePoint(4000 + bUILDERACCCOUNT - 1);
				}
				for (int num24 = 0; num24 < bUILDERACCCOUNT; num24++)
				{
					int arg_37_0 = num24 % 2;
					int num25 = num24 + 4000;
					cp18.LinkMap[num25].Down = ((num24 < bUILDERACCCOUNT - 1) ? (num25 + 1) : -2);
					cp18.LinkMap[num25].Up = ((num24 > 0) ? (num25 - 1) : -1);
				}
			};
			cp18.IsValidEvent += (() => Main.playerInventory && UILinkPointNavigator.Shortcuts.BUILDERACCCOUNT > 0);
			cp18.PageOnLeft = 8;
			cp18.PageOnRight = 8;
			UILinkPointNavigator.RegisterPage(cp18, 18, true);
			UILinkPage uILinkPage5 = new UILinkPage();
			uILinkPage5.OnSpecialInteracts += (() => PlayerInput.BuildCommand(Lang.misc[56], false, new List<string>[]
				{
					PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]
				}) + PlayerInput.BuildCommand(Lang.misc[64], true, new List<string>[]
				{
					PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"],
					PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]
				}));
			uILinkPage5.LinkMap.Add(2806, new UILinkPoint(2806, true, 2805, 2807, -1, 2808));
			uILinkPage5.LinkMap.Add(2807, new UILinkPoint(2807, true, 2806, -4, -1, 2809));
			uILinkPage5.LinkMap.Add(2808, new UILinkPoint(2808, true, 2805, 2809, 2806, -2));
			uILinkPage5.LinkMap.Add(2809, new UILinkPoint(2809, true, 2808, -4, 2807, -2));
			uILinkPage5.LinkMap.Add(2805, new UILinkPoint(2805, true, -3, 2806, -1, -2));
			uILinkPage5.LinkMap[2806].OnSpecialInteracts += value;
			uILinkPage5.LinkMap[2807].OnSpecialInteracts += value;
			uILinkPage5.LinkMap[2808].OnSpecialInteracts += value;
			uILinkPage5.LinkMap[2809].OnSpecialInteracts += value;
			uILinkPage5.LinkMap[2805].OnSpecialInteracts += value;
			uILinkPage5.CanEnterEvent += (() => Main.clothesWindow);
			uILinkPage5.IsValidEvent += (() => Main.clothesWindow);
			uILinkPage5.EnterEvent += delegate
			{
				Main.player[Main.myPlayer].releaseInventory = false;
			};
			uILinkPage5.LeaveEvent += delegate
			{
				Main.player[Main.myPlayer].releaseUseTile = false;
			};
			uILinkPage5.PageOnLeft = 15;
			uILinkPage5.PageOnRight = 15;
			UILinkPointNavigator.RegisterPage(uILinkPage5, 14, true);
			UILinkPage uILinkPage6 = new UILinkPage();
			uILinkPage6.OnSpecialInteracts += (() => PlayerInput.BuildCommand(Lang.misc[56], false, new List<string>[]
				{
					PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]
				}) + PlayerInput.BuildCommand(Lang.misc[64], true, new List<string>[]
				{
					PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"],
					PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]
				}));
			uILinkPage6.LinkMap.Add(2800, new UILinkPoint(2800, true, -3, -4, -1, 2801));
			uILinkPage6.LinkMap.Add(2801, new UILinkPoint(2801, true, -3, -4, 2800, 2802));
			uILinkPage6.LinkMap.Add(2802, new UILinkPoint(2802, true, -3, -4, 2801, 2803));
			uILinkPage6.LinkMap.Add(2803, new UILinkPoint(2803, true, -3, 2804, 2802, -2));
			uILinkPage6.LinkMap.Add(2804, new UILinkPoint(2804, true, 2803, -4, 2802, -2));
			uILinkPage6.LinkMap[2800].OnSpecialInteracts += value;
			uILinkPage6.LinkMap[2801].OnSpecialInteracts += value;
			uILinkPage6.LinkMap[2802].OnSpecialInteracts += value;
			uILinkPage6.LinkMap[2803].OnSpecialInteracts += value;
			uILinkPage6.LinkMap[2804].OnSpecialInteracts += value;
			uILinkPage6.UpdateEvent += delegate
			{
				Vector3 value20 = Main.rgbToHsl(Main.selColor);
				float interfaceDeadzoneX = PlayerInput.CurrentProfile.InterfaceDeadzoneX;
				float num24 = PlayerInput.GamepadThumbstickLeft.X;
				if (num24 < -interfaceDeadzoneX || num24 > interfaceDeadzoneX)
				{
					num24 = MathHelper.Lerp(0f, 0.008333334f, (Math.Abs(num24) - interfaceDeadzoneX) / (1f - interfaceDeadzoneX)) * (float)Math.Sign(num24);
				}
				else
				{
					num24 = 0f;
				}
				int currentPoint = UILinkPointNavigator.CurrentPoint;
				if (currentPoint == 2800)
				{
					Main.hBar = MathHelper.Clamp(Main.hBar + num24, 0f, 1f);
				}
				if (currentPoint == 2801)
				{
					Main.sBar = MathHelper.Clamp(Main.sBar + num24, 0f, 1f);
				}
				if (currentPoint == 2802)
				{
					Main.lBar = MathHelper.Clamp(Main.lBar + num24, 0.15f, 1f);
				}
				value20 = Vector3.Clamp(value20, Vector3.Zero, Vector3.One);
				if (num24 != 0f)
				{
					if (Main.clothesWindow)
					{
						Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar);
						switch (Main.selClothes)
						{
							case 0:
								Main.player[Main.myPlayer].shirtColor = Main.selColor;
								break;
							case 1:
								Main.player[Main.myPlayer].underShirtColor = Main.selColor;
								break;
							case 2:
								Main.player[Main.myPlayer].pantsColor = Main.selColor;
								break;
							case 3:
								Main.player[Main.myPlayer].shoeColor = Main.selColor;
								break;
						}
					}
					Main.PlaySound(12, -1, -1, 1, 1f, 0f);
				}
			};
			uILinkPage6.CanEnterEvent += (() => Main.clothesWindow);
			uILinkPage6.IsValidEvent += (() => Main.clothesWindow);
			uILinkPage6.EnterEvent += delegate
			{
				Main.player[Main.myPlayer].releaseInventory = false;
			};
			uILinkPage6.LeaveEvent += delegate
			{
				Main.player[Main.myPlayer].releaseUseTile = false;
			};
			uILinkPage6.PageOnLeft = 14;
			uILinkPage6.PageOnRight = 14;
			UILinkPointNavigator.RegisterPage(uILinkPage6, 15, true);
			UILinkPage cp19 = new UILinkPage();
			cp19.UpdateEvent += delegate
			{
				PlayerInput.GamepadAllowScrolling = true;
			};
			for (int num22 = 0; num22 < 200; num22++)
			{
				cp19.LinkMap.Add(3000 + num22, new UILinkPoint(3000 + num22, true, -3, -4, -1, -2));
			}
			cp19.OnSpecialInteracts += (() => PlayerInput.BuildCommand(Lang.misc[53], false, new List<string>[]
				{
					PlayerInput.ProfileGamepadUI.KeyStatus["MouseLeft"]
				}) + PlayerInput.BuildCommand(Lang.misc[82], false, new List<string>[]
				{
					PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]
				}) + UILinksInitializer.FancyUISpecialInstructions());
			cp19.UpdateEvent += delegate
			{
				if (PlayerInput.Triggers.JustPressed.Inventory)
				{
					UILinksInitializer.FancyExit();
				}
				UILinkPointNavigator.Shortcuts.BackButtonInUse = false;
			};
			cp19.EnterEvent += delegate
			{
				cp19.CurrentPoint = 3002;
			};
			cp19.CanEnterEvent += (() => Main.MenuUI.IsVisible || Main.InGameUI.IsVisible);
			cp19.IsValidEvent += (() => Main.MenuUI.IsVisible || Main.InGameUI.IsVisible);
			UILinkPointNavigator.RegisterPage(cp19, 1004, true);
			UILinkPage cp = new UILinkPage();
			cp.OnSpecialInteracts += (() => PlayerInput.BuildCommand(Lang.misc[56], false, new List<string>[]
				{
					PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]
				}) + PlayerInput.BuildCommand(Lang.misc[64], false, new List<string>[]
				{
					PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"],
					PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]
				}));
			Func<string> value19 = () => PlayerInput.BuildCommand(Lang.misc[94], false, new List<string>[]
				{
					PlayerInput.ProfileGamepadUI.KeyStatus["MouseLeft"]
				});
			for (int num23 = 9000; num23 <= 9050; num23++)
			{
				UILinkPoint uILinkPoint14 = new UILinkPoint(num23, true, num23 + 10, num23 - 10, num23 - 1, num23 + 1);
				cp.LinkMap.Add(num23, uILinkPoint14);
				uILinkPoint14.OnSpecialInteracts += value19;
			}
			cp.UpdateEvent += delegate
			{
				int num24 = UILinkPointNavigator.Shortcuts.BUFFS_PER_COLUMN;
				if (num24 == 0)
				{
					num24 = 100;
				}
				for (int num25 = 0; num25 < 50; num25++)
				{
					cp.LinkMap[9000 + num25].Up = ((num25 % num24 == 0) ? -1 : (9000 + num25 - 1));
					if (cp.LinkMap[9000 + num25].Up == -1)
					{
						if (num25 >= num24)
						{
							cp.LinkMap[9000 + num25].Up = 184;
						}
						else
						{
							cp.LinkMap[9000 + num25].Up = 189;
						}
					}
					cp.LinkMap[9000 + num25].Down = (((num25 + 1) % num24 == 0 || num25 == UILinkPointNavigator.Shortcuts.BUFFS_DRAWN - 1) ? 308 : (9000 + num25 + 1));
					cp.LinkMap[9000 + num25].Left = ((num25 < UILinkPointNavigator.Shortcuts.BUFFS_DRAWN - num24) ? (9000 + num25 + num24) : -3);
					cp.LinkMap[9000 + num25].Right = ((num25 < num24) ? -4 : (9000 + num25 - num24));
				}
			};
			cp.IsValidEvent += (() => Main.playerInventory && Main.EquipPage == 2 && UILinkPointNavigator.Shortcuts.BUFFS_DRAWN > 0);
			cp.PageOnLeft = 8;
			cp.PageOnRight = 8;
			UILinkPointNavigator.RegisterPage(cp, 19, true);
			UILinkPage uILinkPage7 = UILinkPointNavigator.Pages[UILinkPointNavigator.CurrentPage];
			uILinkPage7.CurrentPoint = uILinkPage7.DefaultPoint;
			uILinkPage7.Enter();
		}

		public static void FancyExit()
		{
			switch (UILinkPointNavigator.Shortcuts.BackButtonCommand)
			{
				case 1:
					Main.PlaySound(11, -1, -1, 1, 1f, 0f);
					Main.menuMode = 0;
					return;
				case 2:
					Main.PlaySound(11, -1, -1, 1, 1f, 0f);
					Main.menuMode = (Main.menuMultiplayer ? 12 : 1);
					return;
				case 3:
					Main.menuMode = 0;
					IngameFancyUI.Close();
					return;
				case 4:
					Main.PlaySound(11, -1, -1, 1, 1f, 0f);
					Main.menuMode = 11;
					return;
				case 5:
					Main.PlaySound(11, -1, -1, 1, 1f, 0f);
					Main.menuMode = 11;
					return;
				case 6:
					UIVirtualKeyboard.Cancel();
					return;
				case 100:
					Main.PlaySound(11, -1, -1, 1, 1f, 0f);
					Main.menuMode = UILinkPointNavigator.Shortcuts.BackButtonGoto;
					return;
				case 101:
					UIModBrowser.BackClick(null, null);
					return;
				default:
					return;
			}
		}

		public static string FancyUISpecialInstructions()
		{
			string text = "";
			int fANCYUI_SPECIAL_INSTRUCTIONS = UILinkPointNavigator.Shortcuts.FANCYUI_SPECIAL_INSTRUCTIONS;
			if (fANCYUI_SPECIAL_INSTRUCTIONS == 1)
			{
				if (PlayerInput.Triggers.JustPressed.HotbarMinus)
				{
					UIVirtualKeyboard.CycleSymbols();
				}
				text += PlayerInput.BuildCommand(Lang.menu[235], false, new List<string>[]
					{
						PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"]
					});
				if (PlayerInput.Triggers.JustPressed.MouseRight)
				{
					UIVirtualKeyboard.BackSpace();
				}
				text += PlayerInput.BuildCommand(Lang.menu[236], false, new List<string>[]
					{
						PlayerInput.ProfileGamepadUI.KeyStatus["MouseRight"]
					});
				if (PlayerInput.Triggers.JustPressed.SmartCursor)
				{
					UIVirtualKeyboard.Write(" ");
				}
				text += PlayerInput.BuildCommand(Lang.menu[238], false, new List<string>[]
					{
						PlayerInput.ProfileGamepadUI.KeyStatus["SmartCursor"]
					});
				if (UIVirtualKeyboard.CanSubmit)
				{
					if (PlayerInput.Triggers.JustPressed.HotbarPlus)
					{
						UIVirtualKeyboard.Submit();
					}
					text += PlayerInput.BuildCommand(Lang.menu[237], false, new List<string>[]
						{
							PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]
						});
				}
			}
			return text;
		}

		public static void HandleOptionsSpecials()
		{
			switch (UILinkPointNavigator.Shortcuts.OPTIONS_BUTTON_SPECIALFEATURE)
			{
				case 1:
					Main.bgScroll = (int)UILinksInitializer.HandleSlider((float)Main.bgScroll, 0f, 100f, PlayerInput.CurrentProfile.InterfaceDeadzoneX, 1f);
					Main.caveParallax = 1f - (float)Main.bgScroll / 500f;
					return;
				case 2:
					Main.musicVolume = UILinksInitializer.HandleSlider(Main.musicVolume, 0f, 1f, PlayerInput.CurrentProfile.InterfaceDeadzoneX, 0.35f);
					return;
				case 3:
					Main.soundVolume = UILinksInitializer.HandleSlider(Main.soundVolume, 0f, 1f, PlayerInput.CurrentProfile.InterfaceDeadzoneX, 0.35f);
					return;
				case 4:
					Main.ambientVolume = UILinksInitializer.HandleSlider(Main.ambientVolume, 0f, 1f, PlayerInput.CurrentProfile.InterfaceDeadzoneX, 0.35f);
					return;
				case 5:
					{
						float hBar = Main.hBar;
						float num = Main.hBar = UILinksInitializer.HandleSlider(hBar, 0f, 1f, 0.2f, 0.5f);
						if (hBar != num)
						{
							int menuMode = Main.menuMode;
							switch (menuMode)
							{
								case 17:
									Main.player[Main.myPlayer].hairColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
									break;
								case 18:
									Main.player[Main.myPlayer].eyeColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
									break;
								case 19:
									Main.player[Main.myPlayer].skinColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
									break;
								case 20:
									break;
								case 21:
									Main.player[Main.myPlayer].shirtColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
									break;
								case 22:
									Main.player[Main.myPlayer].underShirtColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
									break;
								case 23:
									Main.player[Main.myPlayer].pantsColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
									break;
								case 24:
									Main.player[Main.myPlayer].shoeColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
									break;
								case 25:
									Main.mouseColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
									break;
								default:
									if (menuMode == 252)
									{
										Color selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar);
										selColor.A = (byte)(Main.aBar * 255f);
										Main.MouseBorderColor = (Main.selColor = selColor);
									}
									break;
							}
							Main.PlaySound(12, -1, -1, 1, 1f, 0f);
							return;
						}
						break;
					}
				case 6:
					{
						float sBar = Main.sBar;
						float num2 = Main.sBar = UILinksInitializer.HandleSlider(sBar, 0f, 1f, PlayerInput.CurrentProfile.InterfaceDeadzoneX, 0.5f);
						if (sBar != num2)
						{
							int menuMode2 = Main.menuMode;
							switch (menuMode2)
							{
								case 17:
									Main.player[Main.myPlayer].hairColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
									break;
								case 18:
									Main.player[Main.myPlayer].eyeColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
									break;
								case 19:
									Main.player[Main.myPlayer].skinColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
									break;
								case 20:
									break;
								case 21:
									Main.player[Main.myPlayer].shirtColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
									break;
								case 22:
									Main.player[Main.myPlayer].underShirtColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
									break;
								case 23:
									Main.player[Main.myPlayer].pantsColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
									break;
								case 24:
									Main.player[Main.myPlayer].shoeColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
									break;
								case 25:
									Main.mouseColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
									break;
								default:
									if (menuMode2 == 252)
									{
										Color selColor2 = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar);
										selColor2.A = (byte)(Main.aBar * 255f);
										Main.MouseBorderColor = (Main.selColor = selColor2);
									}
									break;
							}
							Main.PlaySound(12, -1, -1, 1, 1f, 0f);
							return;
						}
						break;
					}
				case 7:
					{
						float lBar = Main.lBar;
						float min = 0.15f;
						if (Main.menuMode == 252)
						{
							min = 0f;
						}
						float num3 = Main.lBar = UILinksInitializer.HandleSlider(lBar, min, 1f, PlayerInput.CurrentProfile.InterfaceDeadzoneX, 0.5f);
						if (lBar != num3)
						{
							int menuMode3 = Main.menuMode;
							switch (menuMode3)
							{
								case 17:
									Main.player[Main.myPlayer].hairColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
									break;
								case 18:
									Main.player[Main.myPlayer].eyeColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
									break;
								case 19:
									Main.player[Main.myPlayer].skinColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
									break;
								case 20:
									break;
								case 21:
									Main.player[Main.myPlayer].shirtColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
									break;
								case 22:
									Main.player[Main.myPlayer].underShirtColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
									break;
								case 23:
									Main.player[Main.myPlayer].pantsColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
									break;
								case 24:
									Main.player[Main.myPlayer].shoeColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
									break;
								case 25:
									Main.mouseColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
									break;
								default:
									if (menuMode3 == 252)
									{
										Color selColor3 = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar);
										selColor3.A = (byte)(Main.aBar * 255f);
										Main.MouseBorderColor = (Main.selColor = selColor3);
									}
									break;
							}
							Main.PlaySound(12, -1, -1, 1, 1f, 0f);
							return;
						}
						break;
					}
				case 8:
					{
						float aBar = Main.aBar;
						float num4 = Main.aBar = UILinksInitializer.HandleSlider(aBar, 0f, 1f, PlayerInput.CurrentProfile.InterfaceDeadzoneX, 0.5f);
						if (aBar != num4)
						{
							int menuMode4 = Main.menuMode;
							switch (menuMode4)
							{
								case 17:
									Main.player[Main.myPlayer].hairColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
									break;
								case 18:
									Main.player[Main.myPlayer].eyeColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
									break;
								case 19:
									Main.player[Main.myPlayer].skinColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
									break;
								case 20:
									break;
								case 21:
									Main.player[Main.myPlayer].shirtColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
									break;
								case 22:
									Main.player[Main.myPlayer].underShirtColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
									break;
								case 23:
									Main.player[Main.myPlayer].pantsColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
									break;
								case 24:
									Main.player[Main.myPlayer].shoeColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
									break;
								case 25:
									Main.mouseColor = (Main.selColor = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar));
									break;
								default:
									if (menuMode4 == 252)
									{
										Color selColor4 = Main.hslToRgb(Main.hBar, Main.sBar, Main.lBar);
										selColor4.A = (byte)(Main.aBar * 255f);
										Main.MouseBorderColor = (Main.selColor = selColor4);
									}
									break;
							}
							Main.PlaySound(12, -1, -1, 1, 1f, 0f);
							return;
						}
						break;
					}
				case 9:
					{
						bool left = PlayerInput.Triggers.Current.Left;
						bool right = PlayerInput.Triggers.Current.Right;
						if (PlayerInput.Triggers.JustPressed.Left || PlayerInput.Triggers.JustPressed.Right)
						{
							UILinksInitializer.SomeVarsForUILinkers.HairMoveCD = 0;
						}
						else if (UILinksInitializer.SomeVarsForUILinkers.HairMoveCD > 0)
						{
							UILinksInitializer.SomeVarsForUILinkers.HairMoveCD--;
						}
						if (UILinksInitializer.SomeVarsForUILinkers.HairMoveCD == 0 && (left || right))
						{
							if (left)
							{
								Main.PendingPlayer.hair--;
							}
							if (right)
							{
								Main.PendingPlayer.hair++;
							}
							UILinksInitializer.SomeVarsForUILinkers.HairMoveCD = 12;
						}
						int num5 = 51;
						if (Main.PendingPlayer.hair >= num5)
						{
							Main.PendingPlayer.hair = 0;
						}
						if (Main.PendingPlayer.hair < 0)
						{
							Main.PendingPlayer.hair = num5 - 1;
						}
						break;
					}
				default:
					return;
			}
		}
	}
}
