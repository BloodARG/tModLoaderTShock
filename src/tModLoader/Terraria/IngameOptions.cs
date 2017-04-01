using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.Social;
using Terraria.UI;
using Terraria.UI.Gamepad;

namespace Terraria
{
	public static class IngameOptions
	{
		public const int width = 670;
		public const int height = 480;
		public static float[] leftScale = new float[]
		{
			0.7f,
			0.7f,
			0.7f,
			0.7f,
			0.7f,
			0.7f,
			0.7f,
			0.7f,
			0.7f
		};
		public static float[] rightScale = new float[]
		{
			0.7f,
			0.7f,
			0.7f,
			0.7f,
			0.7f,
			0.7f,
			0.7f,
			0.7f,
			0.7f,
			0.7f,
			0.7f,
			0.7f,
			0.7f,
			0.7f,
			0.7f
		};
		public static int leftHover = -1;
		public static int rightHover = -1;
		public static int oldLeftHover = -1;
		public static int oldRightHover = -1;
		public static int rightLock = -1;
		public static bool inBar = false;
		public static bool notBar = false;
		public static bool noSound = false;
		private static Rectangle _GUIHover = default(Rectangle);
		public static int category = 0;
		public static Vector2 valuePosition = Vector2.Zero;

		public static void Open()
		{
			Main.playerInventory = false;
			Main.editChest = false;
			Main.npcChatText = "";
			Main.PlaySound(10, -1, -1, 1, 1f, 0f);
			Main.ingameOptionsWindow = true;
			IngameOptions.category = 0;
			for (int i = 0; i < IngameOptions.leftScale.Length; i++)
			{
				IngameOptions.leftScale[i] = 0f;
			}
			for (int j = 0; j < IngameOptions.rightScale.Length; j++)
			{
				IngameOptions.rightScale[j] = 0f;
			}
			IngameOptions.leftHover = -1;
			IngameOptions.rightHover = -1;
			IngameOptions.oldLeftHover = -1;
			IngameOptions.oldRightHover = -1;
			IngameOptions.rightLock = -1;
			IngameOptions.inBar = false;
			IngameOptions.notBar = false;
			IngameOptions.noSound = false;
		}

		public static void Close()
		{
			if (Main.setKey != -1)
			{
				return;
			}
			Main.ingameOptionsWindow = false;
			Main.PlaySound(11, -1, -1, 1, 1f, 0f);
			Recipe.FindRecipes();
			Main.playerInventory = true;
			Main.SaveSettings();
		}

		public static void Draw(Main mainInstance, SpriteBatch sb)
		{
			if (Main.player[Main.myPlayer].dead && !Main.player[Main.myPlayer].ghost)
			{
				Main.setKey = -1;
				IngameOptions.Close();
				Main.playerInventory = false;
				return;
			}
			new Vector2((float)Main.mouseX, (float)Main.mouseY);
			bool flag = Main.mouseLeft && Main.mouseLeftRelease;
			Vector2 value = new Vector2((float)Main.screenWidth, (float)Main.screenHeight);
			Vector2 value2 = new Vector2(670f, 480f);
			Vector2 value3 = value / 2f - value2 / 2f;
			int num = 20;
			IngameOptions._GUIHover = new Rectangle((int)(value3.X - (float)num), (int)(value3.Y - (float)num), (int)(value2.X + (float)(num * 2)), (int)(value2.Y + (float)(num * 2)));
			Utils.DrawInvBG(sb, value3.X - (float)num, value3.Y - (float)num, value2.X + (float)(num * 2), value2.Y + (float)(num * 2), new Color(33, 15, 91, 255) * 0.685f);
			if (new Rectangle((int)value3.X - num, (int)value3.Y - num, (int)value2.X + num * 2, (int)value2.Y + num * 2).Contains(new Point(Main.mouseX, Main.mouseY)))
			{
				Main.player[Main.myPlayer].mouseInterface = true;
			}
			Utils.DrawInvBG(sb, value3.X + (float)(num / 2), value3.Y + (float)(num * 5 / 2), value2.X / 2f - (float)num, value2.Y - (float)(num * 3), default(Color));
			Utils.DrawInvBG(sb, value3.X + value2.X / 2f + (float)num, value3.Y + (float)(num * 5 / 2), value2.X / 2f - (float)(num * 3 / 2), value2.Y - (float)(num * 3), default(Color));
			Utils.DrawBorderString(sb, Language.GetTextValue("GameUI.SettingsMenu"), value3 + value2 * new Vector2(0.5f, 0f), Color.White, 1f, 0.5f, 0f, -1);
			float num2 = 0.7f;
			float num3 = 0.8f;
			float num4 = 0.01f;
			if (IngameOptions.oldLeftHover != IngameOptions.leftHover && IngameOptions.leftHover != -1)
			{
				Main.PlaySound(12, -1, -1, 1, 1f, 0f);
			}
			if (IngameOptions.oldRightHover != IngameOptions.rightHover && IngameOptions.rightHover != -1)
			{
				Main.PlaySound(12, -1, -1, 1, 1f, 0f);
			}
			if (flag && IngameOptions.rightHover != -1 && !IngameOptions.noSound)
			{
				Main.PlaySound(12, -1, -1, 1, 1f, 0f);
			}
			IngameOptions.oldLeftHover = IngameOptions.leftHover;
			IngameOptions.oldRightHover = IngameOptions.rightHover;
			IngameOptions.noSound = false;
			bool flag2 = SocialAPI.Network != null && SocialAPI.Network.CanInvite();
			int num5 = flag2 ? 1 : 0;
			int num6 = 5 + num5;
			Vector2 vector = new Vector2(value3.X + value2.X / 4f, value3.Y + (float)(num * 5 / 2));
			Vector2 vector2 = new Vector2(0f, value2.Y - (float)(num * 5)) / (float)(num6 + 1);
			UILinkPointNavigator.Shortcuts.INGAMEOPTIONS_BUTTONS_LEFT = num6 + 1;
			for (int i = 0; i <= num6; i++)
			{
				if (IngameOptions.leftHover == i || i == IngameOptions.category)
				{
					IngameOptions.leftScale[i] += num4;
				}
				else
				{
					IngameOptions.leftScale[i] -= num4;
				}
				if (IngameOptions.leftScale[i] < num2)
				{
					IngameOptions.leftScale[i] = num2;
				}
				if (IngameOptions.leftScale[i] > num3)
				{
					IngameOptions.leftScale[i] = num3;
				}
			}
			IngameOptions.leftHover = -1;
			int num7 = IngameOptions.category;
			if (IngameOptions.DrawLeftSide(sb, Lang.menu[114], 0, vector, vector2, IngameOptions.leftScale, 0.7f, 0.8f, 0.01f))
			{
				IngameOptions.leftHover = 0;
				if (flag)
				{
					IngameOptions.category = 0;
					Main.PlaySound(10, -1, -1, 1, 1f, 0f);
				}
			}
			if (IngameOptions.DrawLeftSide(sb, Lang.menu[63], 1, vector, vector2, IngameOptions.leftScale, 0.7f, 0.8f, 0.01f))
			{
				IngameOptions.leftHover = 1;
				if (flag)
				{
					IngameOptions.category = 1;
					Main.PlaySound(10, -1, -1, 1, 1f, 0f);
				}
			}
			if (IngameOptions.DrawLeftSide(sb, Lang.menu[66], 2, vector, vector2, IngameOptions.leftScale, 0.7f, 0.8f, 0.01f))
			{
				IngameOptions.leftHover = 2;
				if (flag)
				{
					IngameOptions.Close();
					IngameFancyUI.OpenKeybinds();
				}
			}
			if (flag2 && IngameOptions.DrawLeftSide(sb, Lang.menu[147], 3, vector, vector2, IngameOptions.leftScale, 0.7f, 0.8f, 0.01f))
			{
				IngameOptions.leftHover = 3;
				if (flag)
				{
					IngameOptions.Close();
					SocialAPI.Network.OpenInviteInterface();
				}
			}
			if (IngameOptions.DrawLeftSide(sb, Lang.menu[131], 3 + num5, vector, vector2, IngameOptions.leftScale, 0.7f, 0.8f, 0.01f))
			{
				IngameOptions.leftHover = 3 + num5;
				if (flag)
				{
					IngameOptions.Close();
					IngameFancyUI.OpenAchievements();
				}
			}
			if (IngameOptions.DrawLeftSide(sb, Lang.menu[118], 4 + num5, vector, vector2, IngameOptions.leftScale, 0.7f, 0.8f, 0.01f))
			{
				IngameOptions.leftHover = 4 + num5;
				if (flag)
				{
					IngameOptions.Close();
				}
			}
			if (IngameOptions.DrawLeftSide(sb, Lang.inter[35], 5 + num5, vector, vector2, IngameOptions.leftScale, 0.7f, 0.8f, 0.01f))
			{
				IngameOptions.leftHover = 5 + num5;
				if (flag)
				{
					IngameOptions.Close();
					Main.menuMode = 10;
					WorldGen.SaveAndQuit(null);
				}
			}
			if (num7 != IngameOptions.category)
			{
				for (int j = 0; j < IngameOptions.rightScale.Length; j++)
				{
					IngameOptions.rightScale[j] = 0f;
				}
			}
			int num8 = 0;
			switch (IngameOptions.category)
			{
				case 0:
					num8 = 11;
					num2 = 1f;
					num3 = 1.001f;
					num4 = 0.001f;
					break;
				case 1:
					num8 = 11;
					num2 = 1f;
					num3 = 1.001f;
					num4 = 0.001f;
					break;
				case 2:
					num8 = 14;
					num2 = 0.8f;
					num3 = 0.801f;
					num4 = 0.001f;
					break;
				case 3:
					num8 = 7;
					num2 = 0.8f;
					num3 = 0.801f;
					num4 = 0.001f;
					break;
			}
			UILinkPointNavigator.Shortcuts.INGAMEOPTIONS_BUTTONS_RIGHT = num8;
			Vector2 vector3 = new Vector2(value3.X + value2.X * 3f / 4f, value3.Y + (float)(num * 5 / 2));
			Vector2 vector4 = new Vector2(0f, value2.Y - (float)(num * 3)) / (float)(num8 + 1);
			if (IngameOptions.category == 2)
			{
				vector4.Y -= 2f;
			}
			for (int k = 0; k < 15; k++)
			{
				if (IngameOptions.rightLock == k || (IngameOptions.rightHover == k && IngameOptions.rightLock == -1))
				{
					IngameOptions.rightScale[k] += num4;
				}
				else
				{
					IngameOptions.rightScale[k] -= num4;
				}
				if (IngameOptions.rightScale[k] < num2)
				{
					IngameOptions.rightScale[k] = num2;
				}
				if (IngameOptions.rightScale[k] > num3)
				{
					IngameOptions.rightScale[k] = num3;
				}
			}
			IngameOptions.inBar = false;
			IngameOptions.rightHover = -1;
			if (!Main.mouseLeft)
			{
				IngameOptions.rightLock = -1;
			}
			if (IngameOptions.rightLock == -1)
			{
				IngameOptions.notBar = false;
			}
			if (IngameOptions.category == 0)
			{
				int num9 = 0;
				vector3.X -= 70f;
				if (IngameOptions.DrawRightSide(sb, string.Concat(new object[]
						{
							Lang.menu[99],
							" ",
							Math.Round((double)(Main.musicVolume * 100f)),
							"%"
						}), num9, vector3, vector4, IngameOptions.rightScale[num9], (IngameOptions.rightScale[num9] - num2) / (num3 - num2), default(Color)))
				{
					if (IngameOptions.rightLock == -1)
					{
						IngameOptions.notBar = true;
					}
					IngameOptions.noSound = true;
					IngameOptions.rightHover = num9;
				}
				IngameOptions.valuePosition.X = value3.X + value2.X - (float)(num / 2) - 20f;
				IngameOptions.valuePosition.Y = IngameOptions.valuePosition.Y - 3f;
				float musicVolume = IngameOptions.DrawValueBar(sb, 0.75f, Main.musicVolume, 0);
				if ((IngameOptions.inBar || IngameOptions.rightLock == num9) && !IngameOptions.notBar)
				{
					IngameOptions.rightHover = num9;
					if (Main.mouseLeft && IngameOptions.rightLock == num9)
					{
						Main.musicVolume = musicVolume;
					}
				}
				if ((float)Main.mouseX > value3.X + value2.X * 2f / 3f + (float)num && (float)Main.mouseX < IngameOptions.valuePosition.X + 3.75f && (float)Main.mouseY > IngameOptions.valuePosition.Y - 10f && (float)Main.mouseY <= IngameOptions.valuePosition.Y + 10f)
				{
					if (IngameOptions.rightLock == -1)
					{
						IngameOptions.notBar = true;
					}
					IngameOptions.rightHover = num9;
				}
				if (IngameOptions.rightHover == num9)
				{
					UILinkPointNavigator.Shortcuts.OPTIONS_BUTTON_SPECIALFEATURE = 2;
				}
				num9++;
				if (IngameOptions.DrawRightSide(sb, string.Concat(new object[]
						{
							Lang.menu[98],
							" ",
							Math.Round((double)(Main.soundVolume * 100f)),
							"%"
						}), num9, vector3, vector4, IngameOptions.rightScale[num9], (IngameOptions.rightScale[num9] - num2) / (num3 - num2), default(Color)))
				{
					if (IngameOptions.rightLock == -1)
					{
						IngameOptions.notBar = true;
					}
					IngameOptions.rightHover = num9;
				}
				IngameOptions.valuePosition.X = value3.X + value2.X - (float)(num / 2) - 20f;
				IngameOptions.valuePosition.Y = IngameOptions.valuePosition.Y - 3f;
				float soundVolume = IngameOptions.DrawValueBar(sb, 0.75f, Main.soundVolume, 0);
				if ((IngameOptions.inBar || IngameOptions.rightLock == num9) && !IngameOptions.notBar)
				{
					IngameOptions.rightHover = num9;
					if (Main.mouseLeft && IngameOptions.rightLock == num9)
					{
						Main.soundVolume = soundVolume;
						IngameOptions.noSound = true;
					}
				}
				if ((float)Main.mouseX > value3.X + value2.X * 2f / 3f + (float)num && (float)Main.mouseX < IngameOptions.valuePosition.X + 3.75f && (float)Main.mouseY > IngameOptions.valuePosition.Y - 10f && (float)Main.mouseY <= IngameOptions.valuePosition.Y + 10f)
				{
					if (IngameOptions.rightLock == -1)
					{
						IngameOptions.notBar = true;
					}
					IngameOptions.rightHover = num9;
				}
				if (IngameOptions.rightHover == num9)
				{
					UILinkPointNavigator.Shortcuts.OPTIONS_BUTTON_SPECIALFEATURE = 3;
				}
				num9++;
				if (IngameOptions.DrawRightSide(sb, string.Concat(new object[]
						{
							Lang.menu[119],
							" ",
							Math.Round((double)(Main.ambientVolume * 100f)),
							"%"
						}), num9, vector3, vector4, IngameOptions.rightScale[num9], (IngameOptions.rightScale[num9] - num2) / (num3 - num2), default(Color)))
				{
					if (IngameOptions.rightLock == -1)
					{
						IngameOptions.notBar = true;
					}
					IngameOptions.rightHover = num9;
				}
				IngameOptions.valuePosition.X = value3.X + value2.X - (float)(num / 2) - 20f;
				IngameOptions.valuePosition.Y = IngameOptions.valuePosition.Y - 3f;
				float ambientVolume = IngameOptions.DrawValueBar(sb, 0.75f, Main.ambientVolume, 0);
				if ((IngameOptions.inBar || IngameOptions.rightLock == num9) && !IngameOptions.notBar)
				{
					IngameOptions.rightHover = num9;
					if (Main.mouseLeft && IngameOptions.rightLock == num9)
					{
						Main.ambientVolume = ambientVolume;
						IngameOptions.noSound = true;
					}
				}
				if ((float)Main.mouseX > value3.X + value2.X * 2f / 3f + (float)num && (float)Main.mouseX < IngameOptions.valuePosition.X + 3.75f && (float)Main.mouseY > IngameOptions.valuePosition.Y - 10f && (float)Main.mouseY <= IngameOptions.valuePosition.Y + 10f)
				{
					if (IngameOptions.rightLock == -1)
					{
						IngameOptions.notBar = true;
					}
					IngameOptions.rightHover = num9;
				}
				if (IngameOptions.rightHover == num9)
				{
					UILinkPointNavigator.Shortcuts.OPTIONS_BUTTON_SPECIALFEATURE = 4;
				}
				num9++;
				vector3.X += 70f;
				if (IngameOptions.DrawRightSide(sb, Main.autoSave ? Lang.menu[67] : Lang.menu[68], num9, vector3, vector4, IngameOptions.rightScale[num9], (IngameOptions.rightScale[num9] - num2) / (num3 - num2), default(Color)))
				{
					IngameOptions.rightHover = num9;
					if (flag)
					{
						Main.autoSave = !Main.autoSave;
					}
				}
				num9++;
				if (IngameOptions.DrawRightSide(sb, Main.autoPause ? Lang.menu[69] : Lang.menu[70], num9, vector3, vector4, IngameOptions.rightScale[num9], (IngameOptions.rightScale[num9] - num2) / (num3 - num2), default(Color)))
				{
					IngameOptions.rightHover = num9;
					if (flag)
					{
						Main.autoPause = !Main.autoPause;
					}
				}
				num9++;
				if (IngameOptions.DrawRightSide(sb, Main.showItemText ? Lang.menu[71] : Lang.menu[72], num9, vector3, vector4, IngameOptions.rightScale[num9], (IngameOptions.rightScale[num9] - num2) / (num3 - num2), default(Color)))
				{
					IngameOptions.rightHover = num9;
					if (flag)
					{
						Main.showItemText = !Main.showItemText;
					}
				}
				num9++;
				if (IngameOptions.DrawRightSide(sb, Main.cSmartCursorToggle ? Lang.menu[121] : Lang.menu[122], num9, vector3, vector4, IngameOptions.rightScale[num9], (IngameOptions.rightScale[num9] - num2) / (num3 - num2), default(Color)))
				{
					IngameOptions.rightHover = num9;
					if (flag)
					{
						Main.cSmartCursorToggle = !Main.cSmartCursorToggle;
					}
				}
				num9++;
				if (IngameOptions.DrawRightSide(sb, Lang.menu[123] + " " + Lang.menu[124 + Main.invasionProgressMode], num9, vector3, vector4, IngameOptions.rightScale[num9], (IngameOptions.rightScale[num9] - num2) / (num3 - num2), default(Color)))
				{
					IngameOptions.rightHover = num9;
					if (flag)
					{
						Main.invasionProgressMode++;
						if (Main.invasionProgressMode >= 3)
						{
							Main.invasionProgressMode = 0;
						}
					}
				}
				num9++;
				if (IngameOptions.DrawRightSide(sb, Main.placementPreview ? Lang.menu[128] : Lang.menu[129], num9, vector3, vector4, IngameOptions.rightScale[num9], (IngameOptions.rightScale[num9] - num2) / (num3 - num2), default(Color)))
				{
					IngameOptions.rightHover = num9;
					if (flag)
					{
						Main.placementPreview = !Main.placementPreview;
					}
				}
				num9++;
				if (IngameOptions.DrawRightSide(sb, ChildSafety.Disabled ? Lang.menu[132] : Lang.menu[133], num9, vector3, vector4, IngameOptions.rightScale[num9], (IngameOptions.rightScale[num9] - num2) / (num3 - num2), default(Color)))
				{
					IngameOptions.rightHover = num9;
					if (flag)
					{
						ChildSafety.Disabled = !ChildSafety.Disabled;
					}
				}
				num9++;
				if (IngameOptions.DrawRightSide(sb, ItemSlot.Options.HighlightNewItems ? Lang.inter[117] : Lang.inter[116], num9, vector3, vector4, IngameOptions.rightScale[num9], (IngameOptions.rightScale[num9] - num2) / (num3 - num2), default(Color)))
				{
					IngameOptions.rightHover = num9;
					if (flag)
					{
						ItemSlot.Options.HighlightNewItems = !ItemSlot.Options.HighlightNewItems;
					}
				}
				num9++;
			}
			if (IngameOptions.category == 1)
			{
				int num10 = 0;
				if (IngameOptions.DrawRightSide(sb, Main.graphics.IsFullScreen ? Lang.menu[49] : Lang.menu[50], num10, vector3, vector4, IngameOptions.rightScale[num10], (IngameOptions.rightScale[num10] - num2) / (num3 - num2), default(Color)))
				{
					IngameOptions.rightHover = num10;
					if (flag)
					{
						Main.ToggleFullScreen();
					}
				}
				num10++;
				if (IngameOptions.DrawRightSide(sb, string.Concat(new object[]
						{
							Lang.menu[51],
							": ",
							Main.PendingResolutionWidth,
							"x",
							Main.PendingResolutionHeight
						}), num10, vector3, vector4, IngameOptions.rightScale[num10], (IngameOptions.rightScale[num10] - num2) / (num3 - num2), default(Color)))
				{
					IngameOptions.rightHover = num10;
					if (flag)
					{
						int num11 = 0;
						for (int l = 0; l < Main.numDisplayModes; l++)
						{
							if (Main.displayWidth[l] == Main.PendingResolutionWidth && Main.displayHeight[l] == Main.PendingResolutionHeight)
							{
								num11 = l;
								break;
							}
						}
						num11++;
						if (num11 >= Main.numDisplayModes)
						{
							num11 = 0;
						}
						Main.PendingResolutionWidth = Main.displayWidth[num11];
						Main.PendingResolutionHeight = Main.displayHeight[num11];
					}
				}
				num10++;
				vector3.X -= 70f;
				if (IngameOptions.DrawRightSide(sb, string.Concat(new object[]
						{
							Lang.menu[52],
							": ",
							Main.bgScroll,
							"%"
						}), num10, vector3, vector4, IngameOptions.rightScale[num10], (IngameOptions.rightScale[num10] - num2) / (num3 - num2), default(Color)))
				{
					if (IngameOptions.rightLock == -1)
					{
						IngameOptions.notBar = true;
					}
					IngameOptions.noSound = true;
					IngameOptions.rightHover = num10;
				}
				IngameOptions.valuePosition.X = value3.X + value2.X - (float)(num / 2) - 20f;
				IngameOptions.valuePosition.Y = IngameOptions.valuePosition.Y - 3f;
				float num12 = IngameOptions.DrawValueBar(sb, 0.75f, (float)Main.bgScroll / 100f, 0);
				if ((IngameOptions.inBar || IngameOptions.rightLock == num10) && !IngameOptions.notBar)
				{
					IngameOptions.rightHover = num10;
					if (Main.mouseLeft && IngameOptions.rightLock == num10)
					{
						Main.bgScroll = (int)(num12 * 100f);
						Main.caveParallax = 1f - (float)Main.bgScroll / 500f;
					}
				}
				if ((float)Main.mouseX > value3.X + value2.X * 2f / 3f + (float)num && (float)Main.mouseX < IngameOptions.valuePosition.X + 3.75f && (float)Main.mouseY > IngameOptions.valuePosition.Y - 10f && (float)Main.mouseY <= IngameOptions.valuePosition.Y + 10f)
				{
					if (IngameOptions.rightLock == -1)
					{
						IngameOptions.notBar = true;
					}
					IngameOptions.rightHover = num10;
				}
				if (IngameOptions.rightHover == num10)
				{
					UILinkPointNavigator.Shortcuts.OPTIONS_BUTTON_SPECIALFEATURE = 1;
				}
				num10++;
				vector3.X += 70f;
				if (IngameOptions.DrawRightSide(sb, Lang.menu[247 + Main.FrameSkipMode], num10, vector3, vector4, IngameOptions.rightScale[num10], (IngameOptions.rightScale[num10] - num2) / (num3 - num2), default(Color)))
				{
					IngameOptions.rightHover = num10;
					if (flag)
					{
						Main.FrameSkipMode++;
						if (Main.FrameSkipMode < 0 || Main.FrameSkipMode > 2)
						{
							Main.FrameSkipMode = 0;
						}
					}
				}
				num10++;
				if (IngameOptions.DrawRightSide(sb, Lang.menu[55 + Lighting.lightMode], num10, vector3, vector4, IngameOptions.rightScale[num10], (IngameOptions.rightScale[num10] - num2) / (num3 - num2), default(Color)))
				{
					IngameOptions.rightHover = num10;
					if (flag)
					{
						Lighting.NextLightMode();
					}
				}
				num10++;
				if (IngameOptions.DrawRightSide(sb, Lang.menu[116] + " " + ((Lighting.LightingThreads > 0) ? string.Concat(Lighting.LightingThreads + 1) : Lang.menu[117]), num10, vector3, vector4, IngameOptions.rightScale[num10], (IngameOptions.rightScale[num10] - num2) / (num3 - num2), default(Color)))
				{
					IngameOptions.rightHover = num10;
					if (flag)
					{
						Lighting.LightingThreads++;
						if (Lighting.LightingThreads > Environment.ProcessorCount - 1)
						{
							Lighting.LightingThreads = 0;
						}
					}
				}
				num10++;
				if (IngameOptions.DrawRightSide(sb, Lang.menu[59 + Main.qaStyle], num10, vector3, vector4, IngameOptions.rightScale[num10], (IngameOptions.rightScale[num10] - num2) / (num3 - num2), default(Color)))
				{
					IngameOptions.rightHover = num10;
					if (flag)
					{
						Main.qaStyle++;
						if (Main.qaStyle > 3)
						{
							Main.qaStyle = 0;
						}
					}
				}
				num10++;
				if (IngameOptions.DrawRightSide(sb, Main.BackgroundEnabled ? Lang.menu[100] : Lang.menu[101], num10, vector3, vector4, IngameOptions.rightScale[num10], (IngameOptions.rightScale[num10] - num2) / (num3 - num2), default(Color)))
				{
					IngameOptions.rightHover = num10;
					if (flag)
					{
						Main.BackgroundEnabled = !Main.BackgroundEnabled;
					}
				}
				num10++;
				if (IngameOptions.DrawRightSide(sb, Language.GetTextValue("GameUI.HeatDistortion", Main.UseHeatDistortion ? Language.GetTextValue("GameUI.Enabled") : Language.GetTextValue("GameUI.Disabled")), num10, vector3, vector4, IngameOptions.rightScale[num10], (IngameOptions.rightScale[num10] - num2) / (num3 - num2), default(Color)))
				{
					IngameOptions.rightHover = num10;
					if (flag)
					{
						Main.UseHeatDistortion = !Main.UseHeatDistortion;
					}
				}
				num10++;
				if (IngameOptions.DrawRightSide(sb, Language.GetTextValue("GameUI.StormEffects", Main.UseStormEffects ? Language.GetTextValue("GameUI.Enabled") : Language.GetTextValue("GameUI.Disabled")), num10, vector3, vector4, IngameOptions.rightScale[num10], (IngameOptions.rightScale[num10] - num2) / (num3 - num2), default(Color)))
				{
					IngameOptions.rightHover = num10;
					if (flag)
					{
						Main.UseStormEffects = !Main.UseStormEffects;
					}
				}
				num10++;
				string textValue;
				switch (Main.WaveQuality)
				{
					case 1:
						textValue = Language.GetTextValue("GameUI.QualityLow");
						break;
					case 2:
						textValue = Language.GetTextValue("GameUI.QualityMedium");
						break;
					case 3:
						textValue = Language.GetTextValue("GameUI.QualityHigh");
						break;
					default:
						textValue = Language.GetTextValue("GameUI.QualityOff");
						break;
				}
				if (IngameOptions.DrawRightSide(sb, Language.GetTextValue("GameUI.WaveQuality", textValue), num10, vector3, vector4, IngameOptions.rightScale[num10], (IngameOptions.rightScale[num10] - num2) / (num3 - num2), default(Color)))
				{
					IngameOptions.rightHover = num10;
					if (flag)
					{
						Main.WaveQuality = (Main.WaveQuality + 1) % 4;
					}
				}
				num10++;
			}
			if (IngameOptions.category == 2)
			{
				int num13 = 0;
				vector3.X -= 30f;
				int num14 = 0;
				if (IngameOptions.DrawRightSide(sb, Lang.menu[74 + num14], num13, vector3, vector4, IngameOptions.rightScale[num13], (IngameOptions.rightScale[num13] - num2) / (num3 - num2), (Main.setKey == num14) ? Color.Gold : default(Color)))
				{
					IngameOptions.rightHover = num13;
					if (flag)
					{
						Main.setKey = num14;
					}
				}
				IngameOptions.valuePosition.X = IngameOptions.valuePosition.X + 10f;
				if (IngameOptions.DrawValue(sb, (Main.setKey == num14) ? "_" : Main.cUp, num13, num3, (Main.setKey == num14) ? Color.Gold : ((IngameOptions.rightHover == num13) ? Color.White : default(Color))))
				{
					IngameOptions.rightHover = num13;
					if (flag)
					{
						Main.setKey = num14;
					}
				}
				num13++;
				num14 = 1;
				if (IngameOptions.DrawRightSide(sb, Lang.menu[74 + num14], num13, vector3, vector4, IngameOptions.rightScale[num13], (IngameOptions.rightScale[num13] - num2) / (num3 - num2), (Main.setKey == num14) ? Color.Gold : default(Color)))
				{
					IngameOptions.rightHover = num13;
					if (flag)
					{
						Main.setKey = num14;
					}
				}
				IngameOptions.valuePosition.X = IngameOptions.valuePosition.X + 10f;
				if (IngameOptions.DrawValue(sb, (Main.setKey == num14) ? "_" : Main.cDown, num13, num3, (Main.setKey == num14) ? Color.Gold : ((IngameOptions.rightHover == num13) ? Color.White : default(Color))))
				{
					IngameOptions.rightHover = num13;
					if (flag)
					{
						Main.setKey = num14;
					}
				}
				num13++;
				num14 = 2;
				if (IngameOptions.DrawRightSide(sb, Lang.menu[74 + num14], num13, vector3, vector4, IngameOptions.rightScale[num13], (IngameOptions.rightScale[num13] - num2) / (num3 - num2), (Main.setKey == num14) ? Color.Gold : default(Color)))
				{
					IngameOptions.rightHover = num13;
					if (flag)
					{
						Main.setKey = num14;
					}
				}
				IngameOptions.valuePosition.X = IngameOptions.valuePosition.X + 10f;
				if (IngameOptions.DrawValue(sb, (Main.setKey == num14) ? "_" : Main.cLeft, num13, num3, (Main.setKey == num14) ? Color.Gold : ((IngameOptions.rightHover == num13) ? Color.White : default(Color))))
				{
					IngameOptions.rightHover = num13;
					if (flag)
					{
						Main.setKey = num14;
					}
				}
				num13++;
				num14 = 3;
				if (IngameOptions.DrawRightSide(sb, Lang.menu[74 + num14], num13, vector3, vector4, IngameOptions.rightScale[num13], (IngameOptions.rightScale[num13] - num2) / (num3 - num2), (Main.setKey == num14) ? Color.Gold : default(Color)))
				{
					IngameOptions.rightHover = num13;
					if (flag)
					{
						Main.setKey = num14;
					}
				}
				IngameOptions.valuePosition.X = IngameOptions.valuePosition.X + 10f;
				if (IngameOptions.DrawValue(sb, (Main.setKey == num14) ? "_" : Main.cRight, num13, num3, (Main.setKey == num14) ? Color.Gold : ((IngameOptions.rightHover == num13) ? Color.White : default(Color))))
				{
					IngameOptions.rightHover = num13;
					if (flag)
					{
						Main.setKey = num14;
					}
				}
				num13++;
				num14 = 4;
				if (IngameOptions.DrawRightSide(sb, Lang.menu[74 + num14], num13, vector3, vector4, IngameOptions.rightScale[num13], (IngameOptions.rightScale[num13] - num2) / (num3 - num2), (Main.setKey == num14) ? Color.Gold : default(Color)))
				{
					IngameOptions.rightHover = num13;
					if (flag)
					{
						Main.setKey = num14;
					}
				}
				IngameOptions.valuePosition.X = IngameOptions.valuePosition.X + 10f;
				if (IngameOptions.DrawValue(sb, (Main.setKey == num14) ? "_" : Main.cJump, num13, num3, (Main.setKey == num14) ? Color.Gold : ((IngameOptions.rightHover == num13) ? Color.White : default(Color))))
				{
					IngameOptions.rightHover = num13;
					if (flag)
					{
						Main.setKey = num14;
					}
				}
				num13++;
				num14 = 5;
				if (IngameOptions.DrawRightSide(sb, Lang.menu[74 + num14], num13, vector3, vector4, IngameOptions.rightScale[num13], (IngameOptions.rightScale[num13] - num2) / (num3 - num2), (Main.setKey == num14) ? Color.Gold : default(Color)))
				{
					IngameOptions.rightHover = num13;
					if (flag)
					{
						Main.setKey = num14;
					}
				}
				IngameOptions.valuePosition.X = IngameOptions.valuePosition.X + 10f;
				if (IngameOptions.DrawValue(sb, (Main.setKey == num14) ? "_" : Main.cThrowItem, num13, num3, (Main.setKey == num14) ? Color.Gold : ((IngameOptions.rightHover == num13) ? Color.White : default(Color))))
				{
					IngameOptions.rightHover = num13;
					if (flag)
					{
						Main.setKey = num14;
					}
				}
				num13++;
				num14 = 6;
				if (IngameOptions.DrawRightSide(sb, Lang.menu[74 + num14], num13, vector3, vector4, IngameOptions.rightScale[num13], (IngameOptions.rightScale[num13] - num2) / (num3 - num2), (Main.setKey == num14) ? Color.Gold : default(Color)))
				{
					IngameOptions.rightHover = num13;
					if (flag)
					{
						Main.setKey = num14;
					}
				}
				IngameOptions.valuePosition.X = IngameOptions.valuePosition.X + 10f;
				if (IngameOptions.DrawValue(sb, (Main.setKey == num14) ? "_" : Main.cInv, num13, num3, (Main.setKey == num14) ? Color.Gold : ((IngameOptions.rightHover == num13) ? Color.White : default(Color))))
				{
					IngameOptions.rightHover = num13;
					if (flag)
					{
						Main.setKey = num14;
					}
				}
				num13++;
				num14 = 7;
				if (IngameOptions.DrawRightSide(sb, Lang.menu[74 + num14], num13, vector3, vector4, IngameOptions.rightScale[num13], (IngameOptions.rightScale[num13] - num2) / (num3 - num2), (Main.setKey == num14) ? Color.Gold : default(Color)))
				{
					IngameOptions.rightHover = num13;
					if (flag)
					{
						Main.setKey = num14;
					}
				}
				IngameOptions.valuePosition.X = IngameOptions.valuePosition.X + 10f;
				if (IngameOptions.DrawValue(sb, (Main.setKey == num14) ? "_" : Main.cHeal, num13, num3, (Main.setKey == num14) ? Color.Gold : ((IngameOptions.rightHover == num13) ? Color.White : default(Color))))
				{
					IngameOptions.rightHover = num13;
					if (flag)
					{
						Main.setKey = num14;
					}
				}
				num13++;
				num14 = 8;
				if (IngameOptions.DrawRightSide(sb, Lang.menu[74 + num14], num13, vector3, vector4, IngameOptions.rightScale[num13], (IngameOptions.rightScale[num13] - num2) / (num3 - num2), (Main.setKey == num14) ? Color.Gold : default(Color)))
				{
					IngameOptions.rightHover = num13;
					if (flag)
					{
						Main.setKey = num14;
					}
				}
				IngameOptions.valuePosition.X = IngameOptions.valuePosition.X + 10f;
				if (IngameOptions.DrawValue(sb, (Main.setKey == num14) ? "_" : Main.cMana, num13, num3, (Main.setKey == num14) ? Color.Gold : ((IngameOptions.rightHover == num13) ? Color.White : default(Color))))
				{
					IngameOptions.rightHover = num13;
					if (flag)
					{
						Main.setKey = num14;
					}
				}
				num13++;
				num14 = 9;
				if (IngameOptions.DrawRightSide(sb, Lang.menu[74 + num14], num13, vector3, vector4, IngameOptions.rightScale[num13], (IngameOptions.rightScale[num13] - num2) / (num3 - num2), (Main.setKey == num14) ? Color.Gold : default(Color)))
				{
					IngameOptions.rightHover = num13;
					if (flag)
					{
						Main.setKey = num14;
					}
				}
				IngameOptions.valuePosition.X = IngameOptions.valuePosition.X + 10f;
				if (IngameOptions.DrawValue(sb, (Main.setKey == num14) ? "_" : Main.cBuff, num13, num3, (Main.setKey == num14) ? Color.Gold : ((IngameOptions.rightHover == num13) ? Color.White : default(Color))))
				{
					IngameOptions.rightHover = num13;
					if (flag)
					{
						Main.setKey = num14;
					}
				}
				num13++;
				num14 = 10;
				if (IngameOptions.DrawRightSide(sb, Lang.menu[74 + num14], num13, vector3, vector4, IngameOptions.rightScale[num13], (IngameOptions.rightScale[num13] - num2) / (num3 - num2), (Main.setKey == num14) ? Color.Gold : default(Color)))
				{
					IngameOptions.rightHover = num13;
					if (flag)
					{
						Main.setKey = num14;
					}
				}
				IngameOptions.valuePosition.X = IngameOptions.valuePosition.X + 10f;
				if (IngameOptions.DrawValue(sb, (Main.setKey == num14) ? "_" : Main.cHook, num13, num3, (Main.setKey == num14) ? Color.Gold : ((IngameOptions.rightHover == num13) ? Color.White : default(Color))))
				{
					IngameOptions.rightHover = num13;
					if (flag)
					{
						Main.setKey = num14;
					}
				}
				num13++;
				num14 = 11;
				if (IngameOptions.DrawRightSide(sb, Lang.menu[74 + num14], num13, vector3, vector4, IngameOptions.rightScale[num13], (IngameOptions.rightScale[num13] - num2) / (num3 - num2), (Main.setKey == num14) ? Color.Gold : default(Color)))
				{
					IngameOptions.rightHover = num13;
					if (flag)
					{
						Main.setKey = num14;
					}
				}
				IngameOptions.valuePosition.X = IngameOptions.valuePosition.X + 10f;
				if (IngameOptions.DrawValue(sb, (Main.setKey == num14) ? "_" : Main.cTorch, num13, num3, (Main.setKey == num14) ? Color.Gold : ((IngameOptions.rightHover == num13) ? Color.White : default(Color))))
				{
					IngameOptions.rightHover = num13;
					if (flag)
					{
						Main.setKey = num14;
					}
				}
				num13++;
				num14 = 12;
				if (IngameOptions.DrawRightSide(sb, Lang.menu[120], num13, vector3, vector4, IngameOptions.rightScale[num13], (IngameOptions.rightScale[num13] - num2) / (num3 - num2), (Main.setKey == num14) ? Color.Gold : default(Color)))
				{
					IngameOptions.rightHover = num13;
					if (flag)
					{
						Main.setKey = num14;
					}
				}
				IngameOptions.valuePosition.X = IngameOptions.valuePosition.X + 10f;
				if (IngameOptions.DrawValue(sb, (Main.setKey == num14) ? "_" : Main.cSmart, num13, num3, (Main.setKey == num14) ? Color.Gold : ((IngameOptions.rightHover == num13) ? Color.White : default(Color))))
				{
					IngameOptions.rightHover = num13;
					if (flag)
					{
						Main.setKey = num14;
					}
				}
				num13++;
				num14 = 13;
				if (IngameOptions.DrawRightSide(sb, Lang.menu[130], num13, vector3, vector4, IngameOptions.rightScale[num13], (IngameOptions.rightScale[num13] - num2) / (num3 - num2), (Main.setKey == num14) ? Color.Gold : default(Color)))
				{
					IngameOptions.rightHover = num13;
					if (flag)
					{
						Main.setKey = num14;
					}
				}
				IngameOptions.valuePosition.X = IngameOptions.valuePosition.X + 10f;
				if (IngameOptions.DrawValue(sb, (Main.setKey == num14) ? "_" : Main.cMount, num13, num3, (Main.setKey == num14) ? Color.Gold : ((IngameOptions.rightHover == num13) ? Color.White : default(Color))))
				{
					IngameOptions.rightHover = num13;
					if (flag)
					{
						Main.setKey = num14;
					}
				}
				num13++;
				vector3.X += 30f;
				if (IngameOptions.DrawRightSide(sb, Lang.menu[86], num13, vector3, vector4, IngameOptions.rightScale[num13], (IngameOptions.rightScale[num13] - num2) / (num3 - num2), default(Color)))
				{
					IngameOptions.rightHover = num13;
					if (flag)
					{
						Main.ResetKeyBindings();
						Main.setKey = -1;
					}
				}
				num13++;
				if (Main.setKey >= 0)
				{
					Main.blockInput = true;
					Keys[] pressedKeys = Main.keyState.GetPressedKeys();
					if (pressedKeys.Length > 0)
					{
						string text = string.Concat(pressedKeys[0]);
						if (text != "None")
						{
							if (Main.setKey == 0)
							{
								Main.cUp = text;
							}
							if (Main.setKey == 1)
							{
								Main.cDown = text;
							}
							if (Main.setKey == 2)
							{
								Main.cLeft = text;
							}
							if (Main.setKey == 3)
							{
								Main.cRight = text;
							}
							if (Main.setKey == 4)
							{
								Main.cJump = text;
							}
							if (Main.setKey == 5)
							{
								Main.cThrowItem = text;
							}
							if (Main.setKey == 6)
							{
								Main.cInv = text;
							}
							if (Main.setKey == 7)
							{
								Main.cHeal = text;
							}
							if (Main.setKey == 8)
							{
								Main.cMana = text;
							}
							if (Main.setKey == 9)
							{
								Main.cBuff = text;
							}
							if (Main.setKey == 10)
							{
								Main.cHook = text;
							}
							if (Main.setKey == 11)
							{
								Main.cTorch = text;
							}
							if (Main.setKey == 12)
							{
								Main.cSmart = text;
							}
							if (Main.setKey == 13)
							{
								Main.cMount = text;
							}
							Main.blockKey = pressedKeys[0].ToString();
							Main.blockInput = false;
							Main.setKey = -1;
						}
					}
				}
			}
			if (IngameOptions.category == 3)
			{
				int num15 = 0;
				vector3.X -= 30f;
				int num16 = 0;
				if (IngameOptions.DrawRightSide(sb, Lang.menu[106 + num16], num15, vector3, vector4, IngameOptions.rightScale[num15], (IngameOptions.rightScale[num15] - num2) / (num3 - num2), (Main.setKey == num16) ? Color.Gold : default(Color)))
				{
					IngameOptions.rightHover = num15;
					if (flag)
					{
						Main.setKey = num16;
					}
				}
				IngameOptions.valuePosition.X = IngameOptions.valuePosition.X + 10f;
				if (IngameOptions.DrawValue(sb, (Main.setKey == num16) ? "_" : Main.cMapStyle, num15, num3, (Main.setKey == num16) ? Color.Gold : ((IngameOptions.rightHover == num15) ? Color.White : default(Color))))
				{
					IngameOptions.rightHover = num15;
					if (flag)
					{
						Main.setKey = num16;
					}
				}
				num15++;
				num16 = 1;
				if (IngameOptions.DrawRightSide(sb, Lang.menu[106 + num16], num15, vector3, vector4, IngameOptions.rightScale[num15], (IngameOptions.rightScale[num15] - num2) / (num3 - num2), (Main.setKey == num16) ? Color.Gold : default(Color)))
				{
					IngameOptions.rightHover = num15;
					if (flag)
					{
						Main.setKey = num16;
					}
				}
				IngameOptions.valuePosition.X = IngameOptions.valuePosition.X + 10f;
				if (IngameOptions.DrawValue(sb, (Main.setKey == num16) ? "_" : Main.cMapFull, num15, num3, (Main.setKey == num16) ? Color.Gold : ((IngameOptions.rightHover == num15) ? Color.White : default(Color))))
				{
					IngameOptions.rightHover = num15;
					if (flag)
					{
						Main.setKey = num16;
					}
				}
				num15++;
				num16 = 2;
				if (IngameOptions.DrawRightSide(sb, Lang.menu[106 + num16], num15, vector3, vector4, IngameOptions.rightScale[num15], (IngameOptions.rightScale[num15] - num2) / (num3 - num2), (Main.setKey == num16) ? Color.Gold : default(Color)))
				{
					IngameOptions.rightHover = num15;
					if (flag)
					{
						Main.setKey = num16;
					}
				}
				IngameOptions.valuePosition.X = IngameOptions.valuePosition.X + 10f;
				if (IngameOptions.DrawValue(sb, (Main.setKey == num16) ? "_" : Main.cMapZoomIn, num15, num3, (Main.setKey == num16) ? Color.Gold : ((IngameOptions.rightHover == num15) ? Color.White : default(Color))))
				{
					IngameOptions.rightHover = num15;
					if (flag)
					{
						Main.setKey = num16;
					}
				}
				num15++;
				num16 = 3;
				if (IngameOptions.DrawRightSide(sb, Lang.menu[106 + num16], num15, vector3, vector4, IngameOptions.rightScale[num15], (IngameOptions.rightScale[num15] - num2) / (num3 - num2), (Main.setKey == num16) ? Color.Gold : default(Color)))
				{
					IngameOptions.rightHover = num15;
					if (flag)
					{
						Main.setKey = num16;
					}
				}
				IngameOptions.valuePosition.X = IngameOptions.valuePosition.X + 10f;
				if (IngameOptions.DrawValue(sb, (Main.setKey == num16) ? "_" : Main.cMapZoomOut, num15, num3, (Main.setKey == num16) ? Color.Gold : ((IngameOptions.rightHover == num15) ? Color.White : default(Color))))
				{
					IngameOptions.rightHover = num15;
					if (flag)
					{
						Main.setKey = num16;
					}
				}
				num15++;
				num16 = 4;
				if (IngameOptions.DrawRightSide(sb, Lang.menu[106 + num16], num15, vector3, vector4, IngameOptions.rightScale[num15], (IngameOptions.rightScale[num15] - num2) / (num3 - num2), (Main.setKey == num16) ? Color.Gold : default(Color)))
				{
					IngameOptions.rightHover = num15;
					if (flag)
					{
						Main.setKey = num16;
					}
				}
				IngameOptions.valuePosition.X = IngameOptions.valuePosition.X + 10f;
				if (IngameOptions.DrawValue(sb, (Main.setKey == num16) ? "_" : Main.cMapAlphaUp, num15, num3, (Main.setKey == num16) ? Color.Gold : ((IngameOptions.rightHover == num15) ? Color.White : default(Color))))
				{
					IngameOptions.rightHover = num15;
					if (flag)
					{
						Main.setKey = num16;
					}
				}
				num15++;
				num16 = 5;
				if (IngameOptions.DrawRightSide(sb, Lang.menu[106 + num16], num15, vector3, vector4, IngameOptions.rightScale[num15], (IngameOptions.rightScale[num15] - num2) / (num3 - num2), (Main.setKey == num16) ? Color.Gold : default(Color)))
				{
					IngameOptions.rightHover = num15;
					if (flag)
					{
						Main.setKey = num16;
					}
				}
				IngameOptions.valuePosition.X = IngameOptions.valuePosition.X + 10f;
				if (IngameOptions.DrawValue(sb, (Main.setKey == num16) ? "_" : Main.cMapAlphaDown, num15, num3, (Main.setKey == num16) ? Color.Gold : ((IngameOptions.rightHover == num15) ? Color.White : default(Color))))
				{
					IngameOptions.rightHover = num15;
					if (flag)
					{
						Main.setKey = num16;
					}
				}
				num15++;
				vector3.X += 30f;
				if (IngameOptions.DrawRightSide(sb, Lang.menu[86], num15, vector3, vector4, IngameOptions.rightScale[num15], (IngameOptions.rightScale[num15] - num2) / (num3 - num2), default(Color)))
				{
					IngameOptions.rightHover = num15;
					if (flag)
					{
						Main.cMapStyle = "Tab";
						Main.cMapFull = "M";
						Main.cMapZoomIn = "Add";
						Main.cMapZoomOut = "Subtract";
						Main.cMapAlphaUp = "PageUp";
						Main.cMapAlphaDown = "PageDown";
						Main.setKey = -1;
					}
				}
				num15++;
				if (Main.setKey >= 0)
				{
					Main.blockInput = true;
					Keys[] pressedKeys2 = Main.keyState.GetPressedKeys();
					if (pressedKeys2.Length > 0)
					{
						string text2 = string.Concat(pressedKeys2[0]);
						if (text2 != "None")
						{
							if (Main.setKey == 0)
							{
								Main.cMapStyle = text2;
							}
							if (Main.setKey == 1)
							{
								Main.cMapFull = text2;
							}
							if (Main.setKey == 2)
							{
								Main.cMapZoomIn = text2;
							}
							if (Main.setKey == 3)
							{
								Main.cMapZoomOut = text2;
							}
							if (Main.setKey == 4)
							{
								Main.cMapAlphaUp = text2;
							}
							if (Main.setKey == 5)
							{
								Main.cMapAlphaDown = text2;
							}
							Main.setKey = -1;
							Main.blockKey = pressedKeys2[0].ToString();
							Main.blockInput = false;
						}
					}
				}
			}
			if (IngameOptions.rightHover != -1 && IngameOptions.rightLock == -1)
			{
				IngameOptions.rightLock = IngameOptions.rightHover;
			}
			for (int m = 0; m < num6 + 1; m++)
			{
				UILinkPointNavigator.SetPosition(2900 + m, vector + vector2 * (float)(m + 1));
			}
			for (int n = 0; n < num8; n++)
			{
				UILinkPointNavigator.SetPosition(2930 + n, vector3 + vector4 * (float)(n + 1));
			}
			Main.DrawGamepadInstructions();
			Main.mouseText = false;
			Main.instance.GUIBarsDraw();
			Main.instance.DrawMouseOver();
			Vector2 bonus = Main.DrawThickCursor(false);
			Main.DrawCursor(bonus, false);
		}

		public static void MouseOver()
		{
			if (!Main.ingameOptionsWindow)
			{
				return;
			}
			if (IngameOptions._GUIHover.Contains(Main.MouseScreen.ToPoint()))
			{
				Main.mouseText = true;
			}
		}

		public static bool DrawLeftSide(SpriteBatch sb, string txt, int i, Vector2 anchor, Vector2 offset, float[] scales, float minscale = 0.7f, float maxscale = 0.8f, float scalespeed = 0.01f)
		{
			bool flag = i == IngameOptions.category;
			Color color = Color.Lerp(Color.Gray, Color.White, (scales[i] - minscale) / (maxscale - minscale));
			if (flag)
			{
				color = Color.Gold;
			}
			Vector2 vector = Utils.DrawBorderStringBig(sb, txt, anchor + offset * (float)(1 + i), color, scales[i], 0.5f, 0.5f, -1);
			return new Rectangle((int)anchor.X - (int)vector.X / 2, (int)anchor.Y + (int)(offset.Y * (float)(1 + i)) - (int)vector.Y / 2, (int)vector.X, (int)vector.Y).Contains(new Point(Main.mouseX, Main.mouseY));
		}

		public static bool DrawRightSide(SpriteBatch sb, string txt, int i, Vector2 anchor, Vector2 offset, float scale, float colorScale, Color over = default(Color))
		{
			Color color = Color.Lerp(Color.Gray, Color.White, colorScale);
			if (over != default(Color))
			{
				color = over;
			}
			Vector2 value = Utils.DrawBorderString(sb, txt, anchor + offset * (float)(1 + i), color, scale, 0.5f, 0.5f, -1);
			IngameOptions.valuePosition = anchor + offset * (float)(1 + i) + value * new Vector2(0.5f, 0f);
			return new Rectangle((int)anchor.X - (int)value.X / 2, (int)anchor.Y + (int)(offset.Y * (float)(1 + i)) - (int)value.Y / 2, (int)value.X, (int)value.Y).Contains(new Point(Main.mouseX, Main.mouseY));
		}

		public static bool DrawValue(SpriteBatch sb, string txt, int i, float scale, Color over = default(Color))
		{
			Color color = Color.Gray;
			Vector2 vector = Main.fontMouseText.MeasureString(txt) * scale;
			bool flag = new Rectangle((int)IngameOptions.valuePosition.X, (int)IngameOptions.valuePosition.Y - (int)vector.Y / 2, (int)vector.X, (int)vector.Y).Contains(new Point(Main.mouseX, Main.mouseY));
			if (flag)
			{
				color = Color.White;
			}
			if (over != default(Color))
			{
				color = over;
			}
			Utils.DrawBorderString(sb, txt, IngameOptions.valuePosition, color, scale, 0f, 0.5f, -1);
			IngameOptions.valuePosition.X = IngameOptions.valuePosition.X + vector.X;
			return flag;
		}

		public static float DrawValueBar(SpriteBatch sb, float scale, float perc, int lockState = 0)
		{
			Texture2D colorBarTexture = Main.colorBarTexture;
			Vector2 vector = new Vector2((float)colorBarTexture.Width, (float)colorBarTexture.Height) * scale;
			IngameOptions.valuePosition.X = IngameOptions.valuePosition.X - (float)((int)vector.X);
			Rectangle rectangle = new Rectangle((int)IngameOptions.valuePosition.X, (int)IngameOptions.valuePosition.Y - (int)vector.Y / 2, (int)vector.X, (int)vector.Y);
			Rectangle destinationRectangle = rectangle;
			sb.Draw(colorBarTexture, rectangle, Color.White);
			int num = 167;
			float num2 = (float)rectangle.X + 5f * scale;
			float num3 = (float)rectangle.Y + 4f * scale;
			for (float num4 = 0f; num4 < (float)num; num4 += 1f)
			{
				float amount = num4 / (float)num;
				sb.Draw(Main.colorBlipTexture, new Vector2(num2 + num4 * scale, num3), null, Color.Lerp(Color.Black, Color.White, amount), 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
			}
			rectangle.X = (int)num2;
			rectangle.Y = (int)num3;
			bool flag = rectangle.Contains(new Point(Main.mouseX, Main.mouseY));
			if (lockState == 2)
			{
				flag = false;
			}
			if (flag || lockState == 1)
			{
				sb.Draw(Main.colorHighlightTexture, destinationRectangle, Main.OurFavoriteColor);
			}
			sb.Draw(Main.colorSliderTexture, new Vector2(num2 + 167f * scale * perc, num3 + 4f * scale), null, Color.White, 0f, new Vector2(0.5f * (float)Main.colorSliderTexture.Width, 0.5f * (float)Main.colorSliderTexture.Height), scale, SpriteEffects.None, 0f);
			if (Main.mouseX >= rectangle.X && Main.mouseX <= rectangle.X + rectangle.Width)
			{
				IngameOptions.inBar = flag;
				return (float)(Main.mouseX - rectangle.X) / (float)rectangle.Width;
			}
			IngameOptions.inBar = false;
			if (rectangle.X >= Main.mouseX)
			{
				return 0f;
			}
			return 1f;
		}
	}
}
