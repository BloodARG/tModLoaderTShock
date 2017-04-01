using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.Localization;
using Terraria.UI.Chat;

namespace Terraria.GameContent.UI
{
	public class CustomCurrencySingleCoin : CustomCurrencySystem
	{
		public float CurrencyDrawScale = 0.8f;
		public string CurrencyTextKey = "Currency.DefenderMedals";
		public Color CurrencyTextColor = new Color(240, 100, 120);

		public CustomCurrencySingleCoin(int coinItemID, long currencyCap)
		{
			base.Include(coinItemID, 1);
			base.SetCurrencyCap(currencyCap);
		}

		public override bool TryPurchasing(int price, List<Item[]> inv, List<Point> slotCoins, List<Point> slotsEmpty, List<Point> slotEmptyBank, List<Point> slotEmptyBank2, List<Point> slotEmptyBank3)
		{
			List<Tuple<Point, Item>> cache = base.ItemCacheCreate(inv);
			int num = price;
			for (int i = 0; i < slotCoins.Count; i++)
			{
				Point item = slotCoins[i];
				int num2 = num;
				if (inv[item.X][item.Y].stack < num2)
				{
					num2 = inv[item.X][item.Y].stack;
				}
				num -= num2;
				inv[item.X][item.Y].stack -= num2;
				if (inv[item.X][item.Y].stack == 0)
				{
					switch (item.X)
					{
						case 0:
							slotsEmpty.Add(item);
							break;
						case 1:
							slotEmptyBank.Add(item);
							break;
						case 2:
							slotEmptyBank2.Add(item);
							break;
						case 3:
							slotEmptyBank3.Add(item);
							break;
					}
					slotCoins.Remove(item);
					i--;
				}
				if (num == 0)
				{
					break;
				}
			}
			if (num != 0)
			{
				base.ItemCacheRestore(cache, inv);
				return false;
			}
			return true;
		}

		public override void DrawSavingsMoney(SpriteBatch sb, string text, float shopx, float shopy, long totalCoins, bool horizontal = false)
		{
			int num = this._valuePerUnit.Keys.ElementAt(0);
			Texture2D texture2D = Main.itemTexture[num];
			if (horizontal)
			{
				Vector2 position = new Vector2(shopx + ChatManager.GetStringSize(Main.fontMouseText, text, Vector2.One, -1f).X + 45f, shopy + 50f);
				sb.Draw(texture2D, position, null, Color.White, 0f, texture2D.Size() / 2f, this.CurrencyDrawScale, SpriteEffects.None, 0f);
				Utils.DrawBorderStringFourWay(sb, Main.fontItemStack, totalCoins.ToString(), position.X - 11f, position.Y, Color.White, Color.Black, new Vector2(0.3f), 0.75f);
				return;
			}
			int num2 = (totalCoins > 99L) ? -6 : 0;
			sb.Draw(texture2D, new Vector2(shopx + 11f, shopy + 75f), null, Color.White, 0f, texture2D.Size() / 2f, this.CurrencyDrawScale, SpriteEffects.None, 0f);
			Utils.DrawBorderStringFourWay(sb, Main.fontItemStack, totalCoins.ToString(), shopx + (float)num2, shopy + 75f, Color.White, Color.Black, new Vector2(0.3f), 0.75f);
		}

		public override void GetPriceText(string[] lines, ref int currentLine, int price)
		{
			Color color = this.CurrencyTextColor * ((float)Main.mouseTextColor / 255f);
			lines[currentLine++] = string.Format("[c/{0:X2}{1:X2}{2:X2}:{3} {4} {5}]", new object[]
				{
					color.R,
					color.G,
					color.B,
					Lang.tip[50],
					price,
					Language.GetTextValue(this.CurrencyTextKey).ToLower()
				});
		}
	}
}
