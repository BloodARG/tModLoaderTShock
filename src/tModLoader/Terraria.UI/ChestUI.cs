using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI.Chat;
using Terraria.UI.Gamepad;

namespace Terraria.UI
{
	public class ChestUI
	{
		public class ButtonID
		{
			public const int LootAll = 0;
			public const int DepositAll = 1;
			public const int QuickStack = 2;
			public const int Restock = 3;
			public const int Sort = 4;
			public const int RenameChest = 5;
			public const int RenameChestCancel = 6;
			public const int Count = 7;
		}

		public const float buttonScaleMinimum = 0.75f;
		public const float buttonScaleMaximum = 1f;
		public static float[] ButtonScale = new float[7];
		public static bool[] ButtonHovered = new bool[7];

		public static void UpdateHover(int ID, bool hovering)
		{
			if (hovering)
			{
				if (!ChestUI.ButtonHovered[ID])
				{
					Main.PlaySound(12, -1, -1, 1, 1f, 0f);
				}
				ChestUI.ButtonHovered[ID] = true;
				ChestUI.ButtonScale[ID] += 0.05f;
				if (ChestUI.ButtonScale[ID] > 1f)
				{
					ChestUI.ButtonScale[ID] = 1f;
					return;
				}
			}
			else
			{
				ChestUI.ButtonHovered[ID] = false;
				ChestUI.ButtonScale[ID] -= 0.05f;
				if (ChestUI.ButtonScale[ID] < 0.75f)
				{
					ChestUI.ButtonScale[ID] = 0.75f;
				}
			}
		}

		public static void Draw(SpriteBatch spritebatch)
		{
			if (Main.player[Main.myPlayer].chest != -1 && !Main.recBigList)
			{
				Main.inventoryScale = 0.755f;
				if (Utils.FloatIntersect((float)Main.mouseX, (float)Main.mouseY, 0f, 0f, 73f, (float)Main.instance.invBottom, 560f * Main.inventoryScale, 224f * Main.inventoryScale))
				{
					Main.player[Main.myPlayer].mouseInterface = true;
				}
				ChestUI.DrawName(spritebatch);
				ChestUI.DrawButtons(spritebatch);
				ChestUI.DrawSlots(spritebatch);
				return;
			}
			for (int i = 0; i < 7; i++)
			{
				ChestUI.ButtonScale[i] = 0.75f;
				ChestUI.ButtonHovered[i] = false;
			}
		}

		private static void DrawName(SpriteBatch spritebatch)
		{
			Player player = Main.player[Main.myPlayer];
			string text = string.Empty;
			if (Main.editChest)
			{
				text = Main.npcChatText;
				Main.instance.textBlinkerCount++;
				if (Main.instance.textBlinkerCount >= 20)
				{
					if (Main.instance.textBlinkerState == 0)
					{
						Main.instance.textBlinkerState = 1;
					}
					else
					{
						Main.instance.textBlinkerState = 0;
					}
					Main.instance.textBlinkerCount = 0;
				}
				if (Main.instance.textBlinkerState == 1)
				{
					text += "|";
				}
			}
			else if (player.chest > -1)
			{
				if (Main.chest[player.chest] == null)
				{
					Main.chest[player.chest] = new Chest(false);
				}
				Chest chest = Main.chest[player.chest];
				if (chest.name != "")
				{
					text = chest.name;
				}
				else if (Main.tile[player.chestX, player.chestY].type == 21)
				{
					text = Lang.chestType[(int)(Main.tile[player.chestX, player.chestY].frameX / 36)];
				}
				else if (Main.tile[player.chestX, player.chestY].type == 88)
				{
					text = Lang.dresserType[(int)(Main.tile[player.chestX, player.chestY].frameX / 54)];
				}
				else if (TileLoader.IsChest(Main.tile[player.chestX, player.chestY].type))
				{
					text = TileLoader.ModChestName(Main.tile[player.chestX, player.chestY].type);
				}
				else if (TileLoader.IsDresser(Main.tile[player.chestX, player.chestY].type))
				{
					text = TileLoader.ModDresserName(Main.tile[player.chestX, player.chestY].type);
				}
			}
			else if (player.chest == -2)
			{
				text = Lang.inter[32];
			}
			else if (player.chest == -3)
			{
				text = Lang.inter[33];
			}
			else if (player.chest == -4)
			{
				text = Main.itemName[3813];
			}
			Color baseColor = new Color((int)Main.mouseTextColor, (int)Main.mouseTextColor, (int)Main.mouseTextColor, (int)Main.mouseTextColor);
			baseColor = Color.White * (1f - (255f - (float)Main.mouseTextColor) / 255f * 0.5f);
			baseColor.A = 255;
			int num;
			Utils.WordwrapString(text, Main.fontMouseText, 200, 1, out num);
			num++;
			for (int i = 0; i < num; i++)
			{
				ChatManager.DrawColorCodedStringWithShadow(spritebatch, Main.fontMouseText, text, new Vector2(504f, (float)(Main.instance.invBottom + i * 26)), baseColor, 0f, Vector2.Zero, Vector2.One, -1f, 1.5f);
			}
		}

		private static void DrawButtons(SpriteBatch spritebatch)
		{
			for (int i = 0; i < 7; i++)
			{
				ChestUI.DrawButton(spritebatch, i, 506, Main.instance.invBottom + 40);
			}
		}

		private static void DrawButton(SpriteBatch spriteBatch, int ID, int X, int Y)
		{
			Player player = Main.player[Main.myPlayer];
			if ((ID == 5 && player.chest < -1) || (ID == 6 && !Main.editChest))
			{
				ChestUI.UpdateHover(ID, false);
				return;
			}
			Y += ID * 26;
			float num = ChestUI.ButtonScale[ID];
			string text = "";
			switch (ID)
			{
				case 0:
					text = Lang.inter[29];
					break;
				case 1:
					text = Lang.inter[30];
					break;
				case 2:
					text = Lang.inter[31];
					break;
				case 3:
					text = Lang.inter[82];
					break;
				case 4:
					text = Lang.inter[122];
					break;
				case 5:
					text = Lang.inter[Main.editChest ? 47 : 61];
					break;
				case 6:
					text = Lang.inter[63];
					break;
			}
			Vector2 vector = Main.fontMouseText.MeasureString(text);
			Color baseColor = new Color((int)Main.mouseTextColor, (int)Main.mouseTextColor, (int)Main.mouseTextColor, (int)Main.mouseTextColor) * num;
			baseColor = Color.White * 0.97f * (1f - (255f - (float)Main.mouseTextColor) / 255f * 0.5f);
			baseColor.A = 255;
			X += (int)(vector.X * num / 2f);
			ChatManager.DrawColorCodedStringWithShadow(spriteBatch, Main.fontMouseText, text, new Vector2((float)X, (float)Y), baseColor, 0f, vector / 2f, new Vector2(num), -1f, 1.5f);
			vector *= num;
			switch (ID)
			{
				case 0:
					UILinkPointNavigator.SetPosition(500, new Vector2((float)X - vector.X * num / 2f * 0.8f, (float)Y));
					break;
				case 1:
					UILinkPointNavigator.SetPosition(501, new Vector2((float)X - vector.X * num / 2f * 0.8f, (float)Y));
					break;
				case 2:
					UILinkPointNavigator.SetPosition(502, new Vector2((float)X - vector.X * num / 2f * 0.8f, (float)Y));
					break;
				case 3:
					UILinkPointNavigator.SetPosition(503, new Vector2((float)X - vector.X * num / 2f * 0.8f, (float)Y));
					break;
				case 4:
					UILinkPointNavigator.SetPosition(505, new Vector2((float)X - vector.X * num / 2f * 0.8f, (float)Y));
					break;
				case 5:
					UILinkPointNavigator.SetPosition(504, new Vector2((float)X, (float)Y));
					break;
				case 6:
					UILinkPointNavigator.SetPosition(504, new Vector2((float)X, (float)Y));
					break;
			}
			if (!Utils.FloatIntersect((float)Main.mouseX, (float)Main.mouseY, 0f, 0f, (float)X - vector.X / 2f, (float)Y - vector.Y / 2f, vector.X, vector.Y))
			{
				ChestUI.UpdateHover(ID, false);
				return;
			}
			ChestUI.UpdateHover(ID, true);
			if (!PlayerInput.IgnoreMouseInterface)
			{
				player.mouseInterface = true;
				if (!Main.mouseLeft || !Main.mouseLeftRelease)
				{
					return;
				}
				switch (ID)
				{
					case 0:
						ChestUI.LootAll();
						break;
					case 1:
						ChestUI.DepositAll();
						break;
					case 2:
						ChestUI.QuickStack();
						break;
					case 3:
						ChestUI.Restock();
						break;
					case 4:
						ItemSorting.SortChest();
						break;
					case 5:
						ChestUI.RenameChest();
						break;
					case 6:
						ChestUI.RenameChestCancel();
						break;
				}
				Recipe.FindRecipes();
			}
		}

		private static void DrawSlots(SpriteBatch spriteBatch)
		{
			Player player = Main.player[Main.myPlayer];
			int context = 0;
			Item[] inv = null;
			if (player.chest > -1)
			{
				context = 3;
				inv = Main.chest[player.chest].item;
			}
			if (player.chest == -2)
			{
				context = 4;
				inv = player.bank.item;
			}
			if (player.chest == -3)
			{
				context = 4;
				inv = player.bank2.item;
			}
			if (player.chest == -4)
			{
				context = 4;
				inv = player.bank3.item;
			}
			Main.inventoryScale = 0.755f;
			if (Utils.FloatIntersect((float)Main.mouseX, (float)Main.mouseY, 0f, 0f, 73f, (float)Main.instance.invBottom, 560f * Main.inventoryScale, 224f * Main.inventoryScale) && !PlayerInput.IgnoreMouseInterface)
			{
				player.mouseInterface = true;
			}
			for (int i = 0; i < 10; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					int num = (int)(73f + (float)(i * 56) * Main.inventoryScale);
					int num2 = (int)((float)Main.instance.invBottom + (float)(j * 56) * Main.inventoryScale);
					int slot = i + j * 10;
					new Color(100, 100, 100, 100);
					if (Utils.FloatIntersect((float)Main.mouseX, (float)Main.mouseY, 0f, 0f, (float)num, (float)num2, (float)Main.inventoryBackTexture.Width * Main.inventoryScale, (float)Main.inventoryBackTexture.Height * Main.inventoryScale) && !PlayerInput.IgnoreMouseInterface)
					{
						player.mouseInterface = true;
						ItemSlot.Handle(inv, context, slot);
					}
					ItemSlot.Draw(spriteBatch, inv, context, slot, new Vector2((float)num, (float)num2), default(Color));
				}
			}
		}

		public static void LootAll()
		{
			Player player = Main.player[Main.myPlayer];
			if (player.chest > -1)
			{
				Chest chest = Main.chest[player.chest];
				for (int i = 0; i < 40; i++)
				{
					if (chest.item[i].type > 0)
					{
						chest.item[i].position = player.Center;
						chest.item[i] = player.GetItem(Main.myPlayer, chest.item[i], false, false);
						if (Main.netMode == 1)
						{
							NetMessage.SendData(32, -1, -1, "", player.chest, (float)i, 0f, 0f, 0, 0, 0);
						}
					}
				}
				return;
			}
			if (player.chest == -3)
			{
				for (int j = 0; j < 40; j++)
				{
					if (player.bank2.item[j].type > 0)
					{
						player.bank2.item[j].position = player.Center;
						player.bank2.item[j] = player.GetItem(Main.myPlayer, player.bank2.item[j], false, false);
					}
				}
				return;
			}
			if (player.chest == -4)
			{
				for (int k = 0; k < 40; k++)
				{
					if (player.bank3.item[k].type > 0)
					{
						player.bank3.item[k].position = player.Center;
						player.bank3.item[k] = player.GetItem(Main.myPlayer, player.bank3.item[k], false, false);
					}
				}
				return;
			}
			for (int l = 0; l < 40; l++)
			{
				if (player.bank.item[l].type > 0)
				{
					player.bank.item[l].position = player.Center;
					player.bank.item[l] = player.GetItem(Main.myPlayer, player.bank.item[l], false, false);
				}
			}
		}

		public static void DepositAll()
		{
			Player player = Main.player[Main.myPlayer];
			if (player.chest > -1)
			{
				ChestUI.MoveCoins(player.inventory, Main.chest[player.chest].item);
			}
			else if (player.chest == -3)
			{
				ChestUI.MoveCoins(player.inventory, player.bank2.item);
			}
			else if (player.chest == -4)
			{
				ChestUI.MoveCoins(player.inventory, player.bank3.item);
			}
			else
			{
				ChestUI.MoveCoins(player.inventory, player.bank.item);
			}
			for (int i = 49; i >= 10; i--)
			{
				if (player.inventory[i].stack > 0 && player.inventory[i].type > 0 && !player.inventory[i].favorited)
				{
					if (player.inventory[i].maxStack > 1)
					{
						for (int j = 0; j < 40; j++)
						{
							if (player.chest > -1)
							{
								Chest chest = Main.chest[player.chest];
								if (chest.item[j].stack < chest.item[j].maxStack && player.inventory[i].IsTheSameAs(chest.item[j]))
								{
									int num = player.inventory[i].stack;
									if (player.inventory[i].stack + chest.item[j].stack > chest.item[j].maxStack)
									{
										num = chest.item[j].maxStack - chest.item[j].stack;
									}
									player.inventory[i].stack -= num;
									chest.item[j].stack += num;
									Main.PlaySound(7, -1, -1, 1, 1f, 0f);
									if (player.inventory[i].stack <= 0)
									{
										player.inventory[i].SetDefaults(0, false);
										if (Main.netMode == 1)
										{
											NetMessage.SendData(32, -1, -1, "", player.chest, (float)j, 0f, 0f, 0, 0, 0);
											break;
										}
										break;
									}
									else
									{
										if (chest.item[j].type == 0)
										{
											chest.item[j] = player.inventory[i].Clone();
											player.inventory[i].SetDefaults(0, false);
										}
										if (Main.netMode == 1)
										{
											NetMessage.SendData(32, -1, -1, "", player.chest, (float)j, 0f, 0f, 0, 0, 0);
										}
									}
								}
							}
							else if (player.chest == -3)
							{
								if (player.bank2.item[j].stack < player.bank2.item[j].maxStack && player.inventory[i].IsTheSameAs(player.bank2.item[j]))
								{
									int num2 = player.inventory[i].stack;
									if (player.inventory[i].stack + player.bank2.item[j].stack > player.bank2.item[j].maxStack)
									{
										num2 = player.bank2.item[j].maxStack - player.bank2.item[j].stack;
									}
									player.inventory[i].stack -= num2;
									player.bank2.item[j].stack += num2;
									Main.PlaySound(7, -1, -1, 1, 1f, 0f);
									if (player.inventory[i].stack <= 0)
									{
										player.inventory[i].SetDefaults(0, false);
										break;
									}
									if (player.bank2.item[j].type == 0)
									{
										player.bank2.item[j] = player.inventory[i].Clone();
										player.inventory[i].SetDefaults(0, false);
									}
								}
							}
							else if (player.chest == -4)
							{
								if (player.bank3.item[j].stack < player.bank3.item[j].maxStack && player.inventory[i].IsTheSameAs(player.bank3.item[j]))
								{
									int num3 = player.inventory[i].stack;
									if (player.inventory[i].stack + player.bank3.item[j].stack > player.bank3.item[j].maxStack)
									{
										num3 = player.bank3.item[j].maxStack - player.bank3.item[j].stack;
									}
									player.inventory[i].stack -= num3;
									player.bank3.item[j].stack += num3;
									Main.PlaySound(7, -1, -1, 1, 1f, 0f);
									if (player.inventory[i].stack <= 0)
									{
										player.inventory[i].SetDefaults(0, false);
										break;
									}
									if (player.bank3.item[j].type == 0)
									{
										player.bank3.item[j] = player.inventory[i].Clone();
										player.inventory[i].SetDefaults(0, false);
									}
								}
							}
							else if (player.bank.item[j].stack < player.bank.item[j].maxStack && player.inventory[i].IsTheSameAs(player.bank.item[j]))
							{
								int num4 = player.inventory[i].stack;
								if (player.inventory[i].stack + player.bank.item[j].stack > player.bank.item[j].maxStack)
								{
									num4 = player.bank.item[j].maxStack - player.bank.item[j].stack;
								}
								player.inventory[i].stack -= num4;
								player.bank.item[j].stack += num4;
								Main.PlaySound(7, -1, -1, 1, 1f, 0f);
								if (player.inventory[i].stack <= 0)
								{
									player.inventory[i].SetDefaults(0, false);
									break;
								}
								if (player.bank.item[j].type == 0)
								{
									player.bank.item[j] = player.inventory[i].Clone();
									player.inventory[i].SetDefaults(0, false);
								}
							}
						}
					}
					if (player.inventory[i].stack > 0)
					{
						if (player.chest > -1)
						{
							int k = 0;
							while (k < 40)
							{
								if (Main.chest[player.chest].item[k].stack == 0)
								{
									Main.PlaySound(7, -1, -1, 1, 1f, 0f);
									Main.chest[player.chest].item[k] = player.inventory[i].Clone();
									player.inventory[i].SetDefaults(0, false);
									if (Main.netMode == 1)
									{
										NetMessage.SendData(32, -1, -1, "", player.chest, (float)k, 0f, 0f, 0, 0, 0);
										break;
									}
									break;
								}
								else
								{
									k++;
								}
							}
						}
						else if (player.chest == -3)
						{
							for (int l = 0; l < 40; l++)
							{
								if (player.bank2.item[l].stack == 0)
								{
									Main.PlaySound(7, -1, -1, 1, 1f, 0f);
									player.bank2.item[l] = player.inventory[i].Clone();
									player.inventory[i].SetDefaults(0, false);
									break;
								}
							}
						}
						else if (player.chest == -4)
						{
							for (int m = 0; m < 40; m++)
							{
								if (player.bank3.item[m].stack == 0)
								{
									Main.PlaySound(7, -1, -1, 1, 1f, 0f);
									player.bank3.item[m] = player.inventory[i].Clone();
									player.inventory[i].SetDefaults(0, false);
									break;
								}
							}
						}
						else
						{
							for (int n = 0; n < 40; n++)
							{
								if (player.bank.item[n].stack == 0)
								{
									Main.PlaySound(7, -1, -1, 1, 1f, 0f);
									player.bank.item[n] = player.inventory[i].Clone();
									player.inventory[i].SetDefaults(0, false);
									break;
								}
							}
						}
					}
				}
			}
		}

		public static void QuickStack()
		{
			Player player = Main.player[Main.myPlayer];
			if (player.chest == -4)
			{
				ChestUI.MoveCoins(player.inventory, player.bank3.item);
			}
			else if (player.chest == -3)
			{
				ChestUI.MoveCoins(player.inventory, player.bank2.item);
			}
			else if (player.chest == -2)
			{
				ChestUI.MoveCoins(player.inventory, player.bank.item);
			}
			Item[] inventory = player.inventory;
			Item[] item = player.bank.item;
			if (player.chest > -1)
			{
				item = Main.chest[player.chest].item;
			}
			else if (player.chest == -2)
			{
				item = player.bank.item;
			}
			else if (player.chest == -3)
			{
				item = player.bank2.item;
			}
			else if (player.chest == -4)
			{
				item = player.bank3.item;
			}
			List<int> list = new List<int>();
			List<int> list2 = new List<int>();
			List<int> list3 = new List<int>();
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			List<int> list4 = new List<int>();
			bool[] array = new bool[item.Length];
			for (int i = 0; i < 40; i++)
			{
				if (item[i].type > 0 && item[i].stack > 0 && item[i].maxStack > 1 && (item[i].type < 71 || item[i].type > 74))
				{
					list2.Add(i);
					list.Add(item[i].netID);
				}
				if (item[i].type == 0 || item[i].stack <= 0)
				{
					list3.Add(i);
				}
			}
			int num = 50;
			if (player.chest <= -2)
			{
				num += 4;
			}
			for (int j = 0; j < num; j++)
			{
				if (list.Contains(inventory[j].netID) && !inventory[j].favorited)
				{
					dictionary.Add(j, inventory[j].netID);
				}
			}
			for (int k = 0; k < list2.Count; k++)
			{
				int num2 = list2[k];
				int netID = item[num2].netID;
				foreach (KeyValuePair<int, int> current in dictionary)
				{
					if (current.Value == netID && inventory[current.Key].netID == netID)
					{
						int num3 = inventory[current.Key].stack;
						int num4 = item[num2].maxStack - item[num2].stack;
						if (num4 == 0)
						{
							break;
						}
						if (num3 > num4)
						{
							num3 = num4;
						}
						Main.PlaySound(7, -1, -1, 1, 1f, 0f);
						item[num2].stack += num3;
						inventory[current.Key].stack -= num3;
						if (inventory[current.Key].stack == 0)
						{
							inventory[current.Key].SetDefaults(0, false);
						}
						array[num2] = true;
					}
				}
			}
			foreach (KeyValuePair<int, int> current2 in dictionary)
			{
				if (inventory[current2.Key].stack == 0)
				{
					list4.Add(current2.Key);
				}
			}
			foreach (int current3 in list4)
			{
				dictionary.Remove(current3);
			}
			for (int l = 0; l < list3.Count; l++)
			{
				int num5 = list3[l];
				bool flag = true;
				int num6 = item[num5].netID;
				foreach (KeyValuePair<int, int> current4 in dictionary)
				{
					if ((current4.Value == num6 && inventory[current4.Key].netID == num6) || (flag && inventory[current4.Key].stack > 0))
					{
						Main.PlaySound(7, -1, -1, 1, 1f, 0f);
						if (flag)
						{
							num6 = current4.Value;
							item[num5] = inventory[current4.Key];
							inventory[current4.Key] = new Item();
						}
						else
						{
							int num7 = inventory[current4.Key].stack;
							int num8 = item[num5].maxStack - item[num5].stack;
							if (num8 == 0)
							{
								break;
							}
							if (num7 > num8)
							{
								num7 = num8;
							}
							item[num5].stack += num7;
							inventory[current4.Key].stack -= num7;
							if (inventory[current4.Key].stack == 0)
							{
								inventory[current4.Key] = new Item();
							}
						}
						array[num5] = true;
						flag = false;
					}
				}
			}
			if (Main.netMode == 1 && player.chest >= 0)
			{
				for (int m = 0; m < array.Length; m++)
				{
					NetMessage.SendData(32, -1, -1, "", player.chest, (float)m, 0f, 0f, 0, 0, 0);
				}
			}
			list.Clear();
			list2.Clear();
			list3.Clear();
			dictionary.Clear();
			list4.Clear();
		}

		public static void RenameChest()
		{
			Player player = Main.player[Main.myPlayer];
			if (!Main.editChest)
			{
				IngameFancyUI.OpenVirtualKeyboard(2);
				return;
			}
			ChestUI.RenameChestSubmit(player);
		}

		public static void RenameChestSubmit(Player player)
		{
			Main.PlaySound(12, -1, -1, 1, 1f, 0f);
			Main.editChest = false;
			int chest = player.chest;
			if (Main.npcChatText == Main.defaultChestName)
			{
				Main.npcChatText = "";
			}
			if (Main.chest[chest].name != Main.npcChatText)
			{
				Main.chest[chest].name = Main.npcChatText;
				if (Main.netMode == 1)
				{
					player.editedChestName = true;
				}
			}
		}

		public static void RenameChestCancel()
		{
			Main.PlaySound(12, -1, -1, 1, 1f, 0f);
			Main.editChest = false;
			Main.npcChatText = string.Empty;
			Main.blockKey = Keys.Escape.ToString();
		}

		public static void Restock()
		{
			Player player = Main.player[Main.myPlayer];
			Item[] inventory = player.inventory;
			Item[] item = player.bank.item;
			if (player.chest > -1)
			{
				item = Main.chest[player.chest].item;
			}
			else if (player.chest == -2)
			{
				item = player.bank.item;
			}
			else if (player.chest == -3)
			{
				item = player.bank2.item;
			}
			else if (player.chest == -4)
			{
				item = player.bank3.item;
			}
			HashSet<int> hashSet = new HashSet<int>();
			List<int> list = new List<int>();
			List<int> list2 = new List<int>();
			for (int i = 57; i >= 0; i--)
			{
				if ((i < 50 || i >= 54) && (inventory[i].type < 71 || inventory[i].type > 74))
				{
					if (inventory[i].stack > 0 && inventory[i].maxStack > 1 && inventory[i].prefix == 0)
					{
						hashSet.Add(inventory[i].netID);
						if (inventory[i].stack < inventory[i].maxStack)
						{
							list.Add(i);
						}
					}
					else if (inventory[i].stack == 0 || inventory[i].netID == 0 || inventory[i].type == 0)
					{
						list2.Add(i);
					}
				}
			}
			bool flag = false;
			for (int j = 0; j < item.Length; j++)
			{
				if (item[j].stack >= 1 && item[j].prefix == 0 && hashSet.Contains(item[j].netID))
				{
					bool flag2 = false;
					for (int k = 0; k < list.Count; k++)
					{
						int num = list[k];
						int context = 0;
						if (num >= 50)
						{
							context = 2;
						}
						if (inventory[num].netID == item[j].netID && ItemSlot.PickItemMovementAction(inventory, context, num, item[j]) != -1)
						{
							int num2 = item[j].stack;
							if (inventory[num].maxStack - inventory[num].stack < num2)
							{
								num2 = inventory[num].maxStack - inventory[num].stack;
							}
							inventory[num].stack += num2;
							item[j].stack -= num2;
							flag = true;
							if (inventory[num].stack == inventory[num].maxStack)
							{
								if (Main.netMode == 1 && Main.player[Main.myPlayer].chest > -1)
								{
									NetMessage.SendData(32, -1, -1, "", Main.player[Main.myPlayer].chest, (float)j, 0f, 0f, 0, 0, 0);
								}
								list.RemoveAt(k);
								k--;
							}
							if (item[j].stack == 0)
							{
								item[j] = new Item();
								flag2 = true;
								if (Main.netMode == 1 && Main.player[Main.myPlayer].chest > -1)
								{
									NetMessage.SendData(32, -1, -1, "", Main.player[Main.myPlayer].chest, (float)j, 0f, 0f, 0, 0, 0);
									break;
								}
								break;
							}
						}
					}
					if (!flag2 && list2.Count > 0 && item[j].ammo != 0)
					{
						for (int l = 0; l < list2.Count; l++)
						{
							int context2 = 0;
							if (list2[l] >= 50)
							{
								context2 = 2;
							}
							if (ItemSlot.PickItemMovementAction(inventory, context2, list2[l], item[j]) != -1)
							{
								Utils.Swap<Item>(ref inventory[list2[l]], ref item[j]);
								if (Main.netMode == 1 && Main.player[Main.myPlayer].chest > -1)
								{
									NetMessage.SendData(32, -1, -1, "", Main.player[Main.myPlayer].chest, (float)j, 0f, 0f, 0, 0, 0);
								}
								list.Add(list2[l]);
								list2.RemoveAt(l);
								flag = true;
								break;
							}
						}
					}
				}
			}
			if (flag)
			{
				Main.PlaySound(7, -1, -1, 1, 1f, 0f);
			}
		}

		public static void MoveCoins(Item[] pInv, Item[] cInv)
		{
			int[] array = new int[4];
			List<int> list = new List<int>();
			List<int> list2 = new List<int>();
			bool flag = false;
			int[] array2 = new int[40];
			for (int i = 0; i < cInv.Length; i++)
			{
				array2[i] = -1;
				if (cInv[i].stack < 1 || cInv[i].type < 1)
				{
					list2.Add(i);
					cInv[i] = new Item();
				}
				if (cInv[i] != null && cInv[i].stack > 0)
				{
					int num = 0;
					if (cInv[i].type == 71)
					{
						num = 1;
					}
					if (cInv[i].type == 72)
					{
						num = 2;
					}
					if (cInv[i].type == 73)
					{
						num = 3;
					}
					if (cInv[i].type == 74)
					{
						num = 4;
					}
					array2[i] = num - 1;
					if (num > 0)
					{
						array[num - 1] += cInv[i].stack;
						list2.Add(i);
						cInv[i] = new Item();
						flag = true;
					}
				}
			}
			if (!flag)
			{
				return;
			}
			Main.PlaySound(7, -1, -1, 1, 1f, 0f);
			for (int j = 0; j < pInv.Length; j++)
			{
				if (j != 58 && pInv[j] != null && pInv[j].stack > 0)
				{
					int num2 = 0;
					if (pInv[j].type == 71)
					{
						num2 = 1;
					}
					if (pInv[j].type == 72)
					{
						num2 = 2;
					}
					if (pInv[j].type == 73)
					{
						num2 = 3;
					}
					if (pInv[j].type == 74)
					{
						num2 = 4;
					}
					if (num2 > 0)
					{
						array[num2 - 1] += pInv[j].stack;
						list.Add(j);
						pInv[j] = new Item();
					}
				}
			}
			for (int k = 0; k < 3; k++)
			{
				while (array[k] >= 100)
				{
					array[k] -= 100;
					array[k + 1]++;
				}
			}
			for (int l = 0; l < 40; l++)
			{
				if (array2[l] >= 0 && cInv[l].type == 0)
				{
					int num3 = l;
					int num4 = array2[l];
					if (array[num4] > 0)
					{
						cInv[num3].SetDefaults(71 + num4, false);
						cInv[num3].stack = array[num4];
						if (cInv[num3].stack > cInv[num3].maxStack)
						{
							cInv[num3].stack = cInv[num3].maxStack;
						}
						array[num4] -= cInv[num3].stack;
						array2[l] = -1;
					}
					if (Main.netMode == 1 && Main.player[Main.myPlayer].chest > -1)
					{
						NetMessage.SendData(32, -1, -1, "", Main.player[Main.myPlayer].chest, (float)num3, 0f, 0f, 0, 0, 0);
					}
					list2.Remove(num3);
				}
			}
			for (int m = 0; m < 40; m++)
			{
				if (array2[m] >= 0 && cInv[m].type == 0)
				{
					int num5 = m;
					int n = 3;
					while (n >= 0)
					{
						if (array[n] > 0)
						{
							cInv[num5].SetDefaults(71 + n, false);
							cInv[num5].stack = array[n];
							if (cInv[num5].stack > cInv[num5].maxStack)
							{
								cInv[num5].stack = cInv[num5].maxStack;
							}
							array[n] -= cInv[num5].stack;
							array2[m] = -1;
							break;
						}
						if (array[n] == 0)
						{
							n--;
						}
					}
					if (Main.netMode == 1 && Main.player[Main.myPlayer].chest > -1)
					{
						NetMessage.SendData(32, -1, -1, "", Main.player[Main.myPlayer].chest, (float)num5, 0f, 0f, 0, 0, 0);
					}
					list2.Remove(num5);
				}
			}
			while (list2.Count > 0)
			{
				int num6 = list2[0];
				int num7 = 3;
				while (num7 >= 0)
				{
					if (array[num7] > 0)
					{
						cInv[num6].SetDefaults(71 + num7, false);
						cInv[num6].stack = array[num7];
						if (cInv[num6].stack > cInv[num6].maxStack)
						{
							cInv[num6].stack = cInv[num6].maxStack;
						}
						array[num7] -= cInv[num6].stack;
						break;
					}
					if (array[num7] == 0)
					{
						num7--;
					}
				}
				if (Main.netMode == 1 && Main.player[Main.myPlayer].chest > -1)
				{
					NetMessage.SendData(32, -1, -1, "", Main.player[Main.myPlayer].chest, (float)list2[0], 0f, 0f, 0, 0, 0);
				}
				list2.RemoveAt(0);
			}
			int num8 = 3;
			while (num8 >= 0 && list.Count > 0)
			{
				int num9 = list[0];
				if (array[num8] > 0)
				{
					pInv[num9].SetDefaults(71 + num8, false);
					pInv[num9].stack = array[num8];
					if (pInv[num9].stack > pInv[num9].maxStack)
					{
						pInv[num9].stack = pInv[num9].maxStack;
					}
					array[num8] -= pInv[num9].stack;
				}
				if (array[num8] == 0)
				{
					num8--;
				}
				list.RemoveAt(0);
			}
		}

		public static bool TryPlacingInChest(Item I, bool justCheck)
		{
			bool flag = false;
			Player player = Main.player[Main.myPlayer];
			Item[] item = player.bank.item;
			if (player.chest > -1)
			{
				item = Main.chest[player.chest].item;
				flag = (Main.netMode == 1);
			}
			else if (player.chest == -2)
			{
				item = player.bank.item;
			}
			else if (player.chest == -3)
			{
				item = player.bank2.item;
			}
			else if (player.chest == -4)
			{
				item = player.bank3.item;
			}
			bool flag2 = false;
			if (I.maxStack > 1)
			{
				for (int i = 0; i < 40; i++)
				{
					if (item[i].stack < item[i].maxStack && I.IsTheSameAs(item[i]))
					{
						int num = I.stack;
						if (I.stack + item[i].stack > item[i].maxStack)
						{
							num = item[i].maxStack - item[i].stack;
						}
						if (justCheck)
						{
							flag2 = (flag2 || num > 0);
							break;
						}
						I.stack -= num;
						item[i].stack += num;
						Main.PlaySound(7, -1, -1, 1, 1f, 0f);
						if (I.stack <= 0)
						{
							I.SetDefaults(0, false);
							if (flag)
							{
								NetMessage.SendData(32, -1, -1, "", player.chest, (float)i, 0f, 0f, 0, 0, 0);
								break;
							}
							break;
						}
						else
						{
							if (item[i].type == 0)
							{
								item[i] = I.Clone();
								I.SetDefaults(0, false);
							}
							if (flag)
							{
								NetMessage.SendData(32, -1, -1, "", player.chest, (float)i, 0f, 0f, 0, 0, 0);
							}
						}
					}
				}
			}
			if (I.stack > 0)
			{
				int j = 0;
				while (j < 40)
				{
					if (item[j].stack == 0)
					{
						if (justCheck)
						{
							flag2 = true;
							break;
						}
						Main.PlaySound(7, -1, -1, 1, 1f, 0f);
						item[j] = I.Clone();
						I.SetDefaults(0, false);
						if (flag)
						{
							NetMessage.SendData(32, -1, -1, "", player.chest, (float)j, 0f, 0f, 0, 0, 0);
							break;
						}
						break;
					}
					else
					{
						j++;
					}
				}
			}
			return flag2;
		}

		public static bool TryPlacingInPlayer(int slot, bool justCheck)
		{
			bool flag = false;
			Player player = Main.player[Main.myPlayer];
			Item[] inventory = player.inventory;
			Item[] item = player.bank.item;
			if (player.chest > -1)
			{
				item = Main.chest[player.chest].item;
				flag = (Main.netMode == 1);
			}
			else if (player.chest == -2)
			{
				item = player.bank.item;
			}
			else if (player.chest == -3)
			{
				item = player.bank2.item;
			}
			else if (player.chest == -4)
			{
				item = player.bank3.item;
			}
			Item item2 = item[slot];
			bool flag2 = false;
			if (item2.maxStack > 1)
			{
				for (int i = 49; i >= 0; i--)
				{
					if (inventory[i].stack < inventory[i].maxStack && item2.IsTheSameAs(inventory[i]))
					{
						int num = item2.stack;
						if (item2.stack + inventory[i].stack > inventory[i].maxStack)
						{
							num = inventory[i].maxStack - inventory[i].stack;
						}
						if (justCheck)
						{
							flag2 = (flag2 || num > 0);
							break;
						}
						item2.stack -= num;
						inventory[i].stack += num;
						Main.PlaySound(7, -1, -1, 1, 1f, 0f);
						if (item2.stack <= 0)
						{
							item2.SetDefaults(0, false);
							if (flag)
							{
								NetMessage.SendData(32, -1, -1, "", player.chest, (float)i, 0f, 0f, 0, 0, 0);
								break;
							}
							break;
						}
						else
						{
							if (inventory[i].type == 0)
							{
								inventory[i] = item2.Clone();
								item2.SetDefaults(0, false);
							}
							if (flag)
							{
								NetMessage.SendData(32, -1, -1, "", player.chest, (float)i, 0f, 0f, 0, 0, 0);
							}
						}
					}
				}
			}
			if (item2.stack > 0)
			{
				int j = 49;
				while (j >= 0)
				{
					if (inventory[j].stack == 0)
					{
						if (justCheck)
						{
							flag2 = true;
							break;
						}
						Main.PlaySound(7, -1, -1, 1, 1f, 0f);
						inventory[j] = item2.Clone();
						item2.SetDefaults(0, false);
						if (flag)
						{
							NetMessage.SendData(32, -1, -1, "", player.chest, (float)j, 0f, 0f, 0, 0, 0);
							break;
						}
						break;
					}
					else
					{
						j--;
					}
				}
			}
			return flag2;
		}
	}
}
