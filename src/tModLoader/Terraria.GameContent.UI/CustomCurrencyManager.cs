using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria.ID;

namespace Terraria.GameContent.UI
{
	public class CustomCurrencyManager
	{
		private static int _nextCurrencyIndex = 0;
		private static Dictionary<int, CustomCurrencySystem> _currencies = new Dictionary<int, CustomCurrencySystem>();

		public static void Initialize()
		{
			CustomCurrencyManager._nextCurrencyIndex = 0;
			CustomCurrencyID.DefenderMedals = CustomCurrencyManager.RegisterCurrency(new CustomCurrencySingleCoin(3817, 999L));
		}

		public static int RegisterCurrency(CustomCurrencySystem collection)
		{
			int nextCurrencyIndex = CustomCurrencyManager._nextCurrencyIndex;
			CustomCurrencyManager._nextCurrencyIndex++;
			CustomCurrencyManager._currencies[nextCurrencyIndex] = collection;
			return nextCurrencyIndex;
		}

		public static void DrawSavings(SpriteBatch sb, int currencyIndex, float shopx, float shopy, bool horizontal = false)
		{
			CustomCurrencySystem customCurrencySystem = CustomCurrencyManager._currencies[currencyIndex];
			Player player = Main.player[Main.myPlayer];
			bool flag;
			long num = customCurrencySystem.CountCurrency(out flag, player.bank.item, new int[0]);
			long num2 = customCurrencySystem.CountCurrency(out flag, player.bank2.item, new int[0]);
			long num3 = customCurrencySystem.CountCurrency(out flag, player.bank3.item, new int[0]);
			long num4 = customCurrencySystem.CombineStacks(out flag, new long[]
				{
					num,
					num2,
					num3
				});
			if (num4 > 0L)
			{
				if (num3 > 0L)
				{
					sb.Draw(Main.itemTexture[3813], Utils.CenteredRectangle(new Vector2(shopx + 80f, shopy + 50f), Main.itemTexture[3813].Size() * 0.65f), null, Color.White);
				}
				if (num2 > 0L)
				{
					sb.Draw(Main.itemTexture[346], Utils.CenteredRectangle(new Vector2(shopx + 80f, shopy + 50f), Main.itemTexture[346].Size() * 0.65f), null, Color.White);
				}
				if (num > 0L)
				{
					sb.Draw(Main.itemTexture[87], Utils.CenteredRectangle(new Vector2(shopx + 70f, shopy + 60f), Main.itemTexture[87].Size() * 0.65f), null, Color.White);
				}
				Utils.DrawBorderStringFourWay(sb, Main.fontMouseText, Lang.inter[66], shopx, shopy + 40f, Color.White * ((float)Main.mouseTextColor / 255f), Color.Black, Vector2.Zero, 1f);
				customCurrencySystem.DrawSavingsMoney(sb, Lang.inter[66], shopx, shopy, num4, horizontal);
			}
		}

		public static void GetPriceText(int currencyIndex, string[] lines, ref int currentLine, int price)
		{
			CustomCurrencySystem customCurrencySystem = CustomCurrencyManager._currencies[currencyIndex];
			customCurrencySystem.GetPriceText(lines, ref currentLine, price);
		}

		public static bool BuyItem(Player player, int price, int currencyIndex)
		{
			CustomCurrencySystem customCurrencySystem = CustomCurrencyManager._currencies[currencyIndex];
			bool flag;
			long num = customCurrencySystem.CountCurrency(out flag, player.inventory, new int[]
				{
					58,
					57,
					56,
					55,
					54
				});
			long num2 = customCurrencySystem.CountCurrency(out flag, player.bank.item, new int[0]);
			long num3 = customCurrencySystem.CountCurrency(out flag, player.bank2.item, new int[0]);
			long num4 = customCurrencySystem.CountCurrency(out flag, player.bank3.item, new int[0]);
			long num5 = customCurrencySystem.CombineStacks(out flag, new long[]
				{
					num,
					num2,
					num3,
					num4
				});
			if (num5 < (long)price)
			{
				return false;
			}
			List<Item[]> list = new List<Item[]>();
			Dictionary<int, List<int>> dictionary = new Dictionary<int, List<int>>();
			List<Point> list2 = new List<Point>();
			List<Point> list3 = new List<Point>();
			List<Point> list4 = new List<Point>();
			List<Point> list5 = new List<Point>();
			List<Point> list6 = new List<Point>();
			list.Add(player.inventory);
			list.Add(player.bank.item);
			list.Add(player.bank2.item);
			list.Add(player.bank3.item);
			for (int i = 0; i < list.Count; i++)
			{
				dictionary[i] = new List<int>();
			}
			dictionary[0] = new List<int>
			{
				58,
				57,
				56,
				55,
				54
			};
			for (int j = 0; j < list.Count; j++)
			{
				for (int k = 0; k < list[j].Length; k++)
				{
					if (!dictionary[j].Contains(k) && customCurrencySystem.Accepts(list[j][k]))
					{
						list3.Add(new Point(j, k));
					}
				}
			}
			CustomCurrencyManager.FindEmptySlots(list, dictionary, list2, 0);
			CustomCurrencyManager.FindEmptySlots(list, dictionary, list4, 1);
			CustomCurrencyManager.FindEmptySlots(list, dictionary, list5, 2);
			CustomCurrencyManager.FindEmptySlots(list, dictionary, list6, 3);
			return customCurrencySystem.TryPurchasing(price, list, list3, list2, list4, list5, list6);
		}

		private static void FindEmptySlots(List<Item[]> inventories, Dictionary<int, List<int>> slotsToIgnore, List<Point> emptySlots, int currentInventoryIndex)
		{
			for (int i = inventories[currentInventoryIndex].Length - 1; i >= 0; i--)
			{
				if (!slotsToIgnore[currentInventoryIndex].Contains(i) && (inventories[currentInventoryIndex][i].type == 0 || inventories[currentInventoryIndex][i].stack == 0))
				{
					emptySlots.Add(new Point(currentInventoryIndex, i));
				}
			}
		}
	}
}
