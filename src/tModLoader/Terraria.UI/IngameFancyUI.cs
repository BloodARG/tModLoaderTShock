using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.Achievements;
using Terraria.GameContent.UI.States;
using Terraria.Localization;
using Terraria.UI.Gamepad;
using Terraria.ModLoader;

namespace Terraria.UI
{
	public class IngameFancyUI
	{
		private static bool CoverForOneUIFrame;

		public static void CoverNextFrame()
		{
			IngameFancyUI.CoverForOneUIFrame = true;
		}

		public static bool CanCover()
		{
			if (IngameFancyUI.CoverForOneUIFrame)
			{
				IngameFancyUI.CoverForOneUIFrame = false;
				return true;
			}
			return false;
		}

		public static void OpenAchievements()
		{
			IngameFancyUI.CoverNextFrame();
			Main.playerInventory = false;
			Main.editChest = false;
			Main.npcChatText = "";
			Main.inFancyUI = true;
			Main.InGameUI.SetState(Main.AchievementsMenu);
		}

		public static void OpenAchievementsAndGoto(Achievement achievement)
		{
			IngameFancyUI.OpenAchievements();
			Main.AchievementsMenu.GotoAchievement(achievement);
		}

		public static void OpenKeybinds()
		{
			IngameFancyUI.CoverNextFrame();
			Main.playerInventory = false;
			Main.editChest = false;
			Main.npcChatText = "";
			Main.inFancyUI = true;
			Main.InGameUI.SetState(Main.ManageControlsMenu);
		}

		public static bool CanShowVirtualKeyboard(int context)
		{
			return UIVirtualKeyboard.CanDisplay(context);
		}

		public static void OpenVirtualKeyboard(int keyboardContext)
		{
			IngameFancyUI.CoverNextFrame();
			Main.PlaySound(12, -1, -1, 1, 1f, 0f);
			string labelText = "";
			switch (keyboardContext)
			{
				case 1:
					Main.editSign = true;
					labelText = Language.GetTextValue("UI.EnterMessage");
					break;
				case 2:
					{
						labelText = Language.GetTextValue("UI.EnterNewName");
						Player player = Main.player[Main.myPlayer];
						Chest chest = Main.chest[player.chest];
						Main.npcChatText = chest.name;
						if (Main.tile[player.chestX, player.chestY].type == 21)
						{
							Main.defaultChestName = Lang.chestType[(int)(Main.tile[player.chestX, player.chestY].frameX / 36)];
						}
						if (Main.tile[player.chestX, player.chestY].type == 88)
						{
							Main.defaultChestName = Lang.dresserType[(int)(Main.tile[player.chestX, player.chestY].frameX / 54)];
						}
						if (TileLoader.IsChest(Main.tile[player.chestX, player.chestY].type))
						{
							Main.defaultChestName = TileLoader.ModChestName(Main.tile[player.chestX, player.chestY].type);
						}
						if (TileLoader.IsDresser(Main.tile[player.chestX, player.chestY].type))
						{
							Main.defaultChestName = TileLoader.ModDresserName(Main.tile[player.chestX, player.chestY].type);
						}
						if (Main.npcChatText == "")
						{
							Main.npcChatText = Main.defaultChestName;
						}
						Main.editChest = true;
						break;
					}
			}
			Main.clrInput();
			if (!IngameFancyUI.CanShowVirtualKeyboard(keyboardContext))
			{
				return;
			}
			Main.inFancyUI = true;
			switch (keyboardContext)
			{
				case 1:
					Main.InGameUI.SetState(new UIVirtualKeyboard(labelText, Main.npcChatText, delegate(string s)
							{
								Main.SubmitSignText();
								IngameFancyUI.Close();
							}, delegate
							{
								Main.InputTextSignCancel();
								IngameFancyUI.Close();
							}, keyboardContext, false));
					break;
				case 2:
					Main.InGameUI.SetState(new UIVirtualKeyboard(labelText, Main.npcChatText, delegate(string s)
							{
								ChestUI.RenameChestSubmit(Main.player[Main.myPlayer]);
								IngameFancyUI.Close();
							}, delegate
							{
								ChestUI.RenameChestCancel();
								IngameFancyUI.Close();
							}, keyboardContext, false));
					break;
			}
			UILinkPointNavigator.GoToDefaultPage(1);
		}

		public static void Close()
		{
			Main.inFancyUI = false;
			Main.PlaySound(11, -1, -1, 1, 1f, 0f);
			if (!Main.gameMenu && (!(Main.InGameUI.CurrentState is UIVirtualKeyboard) || UIVirtualKeyboard.KeyboardContext == 2))
			{
				Main.playerInventory = true;
			}
			Main.InGameUI.SetState(null);
			UILinkPointNavigator.Shortcuts.FANCYUI_SPECIAL_INSTRUCTIONS = 0;
		}

		public static bool Draw(SpriteBatch spriteBatch, GameTime gameTime)
		{
			if (!Main.gameMenu && Main.player[Main.myPlayer].dead && !Main.player[Main.myPlayer].ghost)
			{
				IngameFancyUI.Close();
				Main.playerInventory = false;
				return false;
			}
			bool result = false;
			if (Main.InGameUI.CurrentState is UIVirtualKeyboard && UIVirtualKeyboard.KeyboardContext > 0)
			{
				if (!Main.inFancyUI)
				{
					Main.InGameUI.SetState(null);
				}
				if (Main.screenWidth >= 1705)
				{
					result = true;
				}
			}
			if (!Main.gameMenu)
			{
				Main.mouseText = false;
				if (Main.InGameUI != null && Main.InGameUI.IsElementUnderMouse())
				{
					Main.player[Main.myPlayer].mouseInterface = true;
				}
				Main.instance.GUIBarsDraw();
				if (Main.InGameUI.CurrentState is UIVirtualKeyboard && UIVirtualKeyboard.KeyboardContext > 0)
				{
					Main.instance.GUIChatDraw();
				}
				if (!Main.inFancyUI)
				{
					Main.InGameUI.SetState(null);
				}
				Main.instance.DrawMouseOver();
				Vector2 bonus = Main.DrawThickCursor(false);
				Main.DrawCursor(bonus, false);
			}
			return result;
		}

		public static void MouseOver()
		{
			if (!Main.inFancyUI)
			{
				return;
			}
			if (Main.InGameUI.IsElementUnderMouse())
			{
				Main.mouseText = true;
			}
		}
	}
}
