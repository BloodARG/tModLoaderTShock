using Microsoft.Xna.Framework;
using System;
using Terraria.Localization;

namespace Terraria
{
	public class ItemText
	{
		public Vector2 position;
		public Vector2 velocity;
		public float alpha;
		public int alphaDir = 1;
		public string name;
		public int stack;
		public float scale = 1f;
		public float rotation;
		public Color color;
		public bool active;
		public int lifeTime;
		public static int activeTime = 60;
		public static int numActive;
		public bool NoStack;
		public bool coinText;
		public int coinValue;
		public bool expert;

		public static void NewText(Item newItem, int stack, bool noStack = false, bool longText = false)
		{
			bool flag = newItem.type >= 71 && newItem.type <= 74;
			if (!Main.showItemText)
			{
				return;
			}
			if (newItem.name == null || !newItem.active)
			{
				return;
			}
			if (Main.netMode == 2)
			{
				return;
			}
			for (int i = 0; i < 20; i++)
			{
				if (Main.itemText[i].active && (Main.itemText[i].name == newItem.AffixName() || (flag && Main.itemText[i].coinText)) && !Main.itemText[i].NoStack && !noStack)
				{
					string text = string.Concat(new object[]
						{
							newItem.name,
							" (",
							Main.itemText[i].stack + stack,
							")"
						});
					string text2 = newItem.name;
					if (Main.itemText[i].stack > 1)
					{
						object obj = text2;
						text2 = string.Concat(new object[]
							{
								obj,
								" (",
								Main.itemText[i].stack,
								")"
							});
					}
					Vector2 vector = Main.fontMouseText.MeasureString(text2);
					vector = Main.fontMouseText.MeasureString(text);
					if (Main.itemText[i].lifeTime < 0)
					{
						Main.itemText[i].scale = 1f;
					}
					if (Main.itemText[i].lifeTime < 60)
					{
						Main.itemText[i].lifeTime = 60;
					}
					if (flag && Main.itemText[i].coinText)
					{
						int num = 0;
						if (newItem.type == 71)
						{
							num += newItem.stack;
						}
						else if (newItem.type == 72)
						{
							num += 100 * newItem.stack;
						}
						else if (newItem.type == 73)
						{
							num += 10000 * newItem.stack;
						}
						else if (newItem.type == 74)
						{
							num += 1000000 * newItem.stack;
						}
						Main.itemText[i].coinValue += num;
						text = ItemText.ValueToName(Main.itemText[i].coinValue);
						vector = Main.fontMouseText.MeasureString(text);
						Main.itemText[i].name = text;
						if (Main.itemText[i].coinValue >= 1000000)
						{
							if (Main.itemText[i].lifeTime < 300)
							{
								Main.itemText[i].lifeTime = 300;
							}
							Main.itemText[i].color = new Color(220, 220, 198);
						}
						else if (Main.itemText[i].coinValue >= 10000)
						{
							if (Main.itemText[i].lifeTime < 240)
							{
								Main.itemText[i].lifeTime = 240;
							}
							Main.itemText[i].color = new Color(224, 201, 92);
						}
						else if (Main.itemText[i].coinValue >= 100)
						{
							if (Main.itemText[i].lifeTime < 180)
							{
								Main.itemText[i].lifeTime = 180;
							}
							Main.itemText[i].color = new Color(181, 192, 193);
						}
						else if (Main.itemText[i].coinValue >= 1)
						{
							if (Main.itemText[i].lifeTime < 120)
							{
								Main.itemText[i].lifeTime = 120;
							}
							Main.itemText[i].color = new Color(246, 138, 96);
						}
					}
					Main.itemText[i].stack += stack;
					Main.itemText[i].scale = 0f;
					Main.itemText[i].rotation = 0f;
					Main.itemText[i].position.X = newItem.position.X + (float)newItem.width * 0.5f - vector.X * 0.5f;
					Main.itemText[i].position.Y = newItem.position.Y + (float)newItem.height * 0.25f - vector.Y * 0.5f;
					Main.itemText[i].velocity.Y = -7f;
					if (Main.itemText[i].coinText)
					{
						Main.itemText[i].stack = 1;
					}
					return;
				}
			}
			int num2 = -1;
			for (int j = 0; j < 20; j++)
			{
				if (!Main.itemText[j].active)
				{
					num2 = j;
					break;
				}
			}
			if (num2 == -1)
			{
				double num3 = (double)Main.bottomWorld;
				for (int k = 0; k < 20; k++)
				{
					if (num3 > (double)Main.itemText[k].position.Y)
					{
						num2 = k;
						num3 = (double)Main.itemText[k].position.Y;
					}
				}
			}
			if (num2 >= 0)
			{
				string text3 = newItem.AffixName();
				if (stack > 1)
				{
					object obj2 = text3;
					text3 = string.Concat(new object[]
						{
							obj2,
							" (",
							stack,
							")"
						});
				}
				Vector2 vector2 = Main.fontMouseText.MeasureString(text3);
				Main.itemText[num2].alpha = 1f;
				Main.itemText[num2].alphaDir = -1;
				Main.itemText[num2].active = true;
				Main.itemText[num2].scale = 0f;
				Main.itemText[num2].NoStack = noStack;
				Main.itemText[num2].rotation = 0f;
				Main.itemText[num2].position.X = newItem.position.X + (float)newItem.width * 0.5f - vector2.X * 0.5f;
				Main.itemText[num2].position.Y = newItem.position.Y + (float)newItem.height * 0.25f - vector2.Y * 0.5f;
				Main.itemText[num2].color = Color.White;
				if (newItem.rare == 1)
				{
					Main.itemText[num2].color = new Color(150, 150, 255);
				}
				else if (newItem.rare == 2)
				{
					Main.itemText[num2].color = new Color(150, 255, 150);
				}
				else if (newItem.rare == 3)
				{
					Main.itemText[num2].color = new Color(255, 200, 150);
				}
				else if (newItem.rare == 4)
				{
					Main.itemText[num2].color = new Color(255, 150, 150);
				}
				else if (newItem.rare == 5)
				{
					Main.itemText[num2].color = new Color(255, 150, 255);
				}
				else if (newItem.rare == -11)
				{
					Main.itemText[num2].color = new Color(255, 175, 0);
				}
				else if (newItem.rare == -1)
				{
					Main.itemText[num2].color = new Color(130, 130, 130);
				}
				else if (newItem.rare == 6)
				{
					Main.itemText[num2].color = new Color(210, 160, 255);
				}
				else if (newItem.rare == 7)
				{
					Main.itemText[num2].color = new Color(150, 255, 10);
				}
				else if (newItem.rare == 8)
				{
					Main.itemText[num2].color = new Color(255, 255, 10);
				}
				else if (newItem.rare == 9)
				{
					Main.itemText[num2].color = new Color(5, 200, 255);
				}
				else if (newItem.rare == 10)
				{
					Main.itemText[num2].color = new Color(255, 40, 100);
				}
				else if (newItem.rare >= 11)
				{
					Main.itemText[num2].color = new Color(180, 40, 255);
				}
				Main.itemText[num2].expert = newItem.expert;
				Main.itemText[num2].name = newItem.AffixName();
				Main.itemText[num2].stack = stack;
				Main.itemText[num2].velocity.Y = -7f;
				Main.itemText[num2].lifeTime = 60;
				if (longText)
				{
					Main.itemText[num2].lifeTime *= 5;
				}
				Main.itemText[num2].coinValue = 0;
				Main.itemText[num2].coinText = (newItem.type >= 71 && newItem.type <= 74);
				if (Main.itemText[num2].coinText)
				{
					if (newItem.type == 71)
					{
						Main.itemText[num2].coinValue += Main.itemText[num2].stack;
					}
					else if (newItem.type == 72)
					{
						Main.itemText[num2].coinValue += 100 * Main.itemText[num2].stack;
					}
					else if (newItem.type == 73)
					{
						Main.itemText[num2].coinValue += 10000 * Main.itemText[num2].stack;
					}
					else if (newItem.type == 74)
					{
						Main.itemText[num2].coinValue += 1000000 * Main.itemText[num2].stack;
					}
					Main.itemText[num2].ValueToName();
					Main.itemText[num2].stack = 1;
					int num4 = num2;
					if (Main.itemText[num4].coinValue >= 1000000)
					{
						if (Main.itemText[num4].lifeTime < 300)
						{
							Main.itemText[num4].lifeTime = 300;
						}
						Main.itemText[num4].color = new Color(220, 220, 198);
						return;
					}
					if (Main.itemText[num4].coinValue >= 10000)
					{
						if (Main.itemText[num4].lifeTime < 240)
						{
							Main.itemText[num4].lifeTime = 240;
						}
						Main.itemText[num4].color = new Color(224, 201, 92);
						return;
					}
					if (Main.itemText[num4].coinValue >= 100)
					{
						if (Main.itemText[num4].lifeTime < 180)
						{
							Main.itemText[num4].lifeTime = 180;
						}
						Main.itemText[num4].color = new Color(181, 192, 193);
						return;
					}
					if (Main.itemText[num4].coinValue >= 1)
					{
						if (Main.itemText[num4].lifeTime < 120)
						{
							Main.itemText[num4].lifeTime = 120;
						}
						Main.itemText[num4].color = new Color(246, 138, 96);
					}
				}
			}
		}

		private static string ValueToName(int coinValue)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int i = coinValue;
			while (i > 0)
			{
				if (i >= 1000000)
				{
					i -= 1000000;
					num++;
				}
				else if (i >= 10000)
				{
					i -= 10000;
					num2++;
				}
				else if (i >= 100)
				{
					i -= 100;
					num3++;
				}
				else if (i >= 1)
				{
					i--;
					num4++;
				}
			}
			string text = "";
			if (num > 0)
			{
				text = text + num + string.Format(" {0} ", Language.GetTextValue("Currency.Platinum"));
			}
			if (num2 > 0)
			{
				text = text + num2 + string.Format(" {0} ", Language.GetTextValue("Currency.Gold"));
			}
			if (num3 > 0)
			{
				text = text + num3 + string.Format(" {0} ", Language.GetTextValue("Currency.Silver"));
			}
			if (num4 > 0)
			{
				text = text + num4 + string.Format(" {0} ", Language.GetTextValue("Currency.Copper"));
			}
			if (text.Length > 1)
			{
				text = text.Substring(0, text.Length - 1);
			}
			return text;
		}

		private void ValueToName()
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int i = this.coinValue;
			while (i > 0)
			{
				if (i >= 1000000)
				{
					i -= 1000000;
					num++;
				}
				else if (i >= 10000)
				{
					i -= 10000;
					num2++;
				}
				else if (i >= 100)
				{
					i -= 100;
					num3++;
				}
				else if (i >= 1)
				{
					i--;
					num4++;
				}
			}
			this.name = "";
			if (num > 0)
			{
				this.name = this.name + num + string.Format(" {0} ", Language.GetTextValue("Currency.Platinum"));
			}
			if (num2 > 0)
			{
				this.name = this.name + num2 + string.Format(" {0} ", Language.GetTextValue("Currency.Gold"));
			}
			if (num3 > 0)
			{
				this.name = this.name + num3 + string.Format(" {0} ", Language.GetTextValue("Currency.Silver"));
			}
			if (num4 > 0)
			{
				this.name = this.name + num4 + string.Format(" {0} ", Language.GetTextValue("Currency.Copper"));
			}
			if (this.name.Length > 1)
			{
				this.name = this.name.Substring(0, this.name.Length - 1);
			}
		}

		public void Update(int whoAmI)
		{
			if (this.active)
			{
				this.alpha += (float)this.alphaDir * 0.01f;
				if ((double)this.alpha <= 0.7)
				{
					this.alpha = 0.7f;
					this.alphaDir = 1;
				}
				if (this.alpha >= 1f)
				{
					this.alpha = 1f;
					this.alphaDir = -1;
				}
				if (this.expert && this.expert)
				{
					this.color = new Color((int)((byte)Main.DiscoR), (int)((byte)Main.DiscoG), (int)((byte)Main.DiscoB), (int)Main.mouseTextColor);
				}
				bool flag = false;
				string text = this.name;
				if (this.stack > 1)
				{
					object obj = text;
					text = string.Concat(new object[]
						{
							obj,
							" (",
							this.stack,
							")"
						});
				}
				Vector2 value = Main.fontMouseText.MeasureString(text);
				value *= this.scale;
				value.Y *= 0.8f;
				Rectangle rectangle = new Rectangle((int)(this.position.X - value.X / 2f), (int)(this.position.Y - value.Y / 2f), (int)value.X, (int)value.Y);
				for (int i = 0; i < 20; i++)
				{
					if (Main.itemText[i].active && i != whoAmI)
					{
						string text2 = Main.itemText[i].name;
						if (Main.itemText[i].stack > 1)
						{
							object obj2 = text2;
							text2 = string.Concat(new object[]
								{
									obj2,
									" (",
									Main.itemText[i].stack,
									")"
								});
						}
						Vector2 value2 = Main.fontMouseText.MeasureString(text2);
						value2 *= Main.itemText[i].scale;
						value2.Y *= 0.8f;
						Rectangle value3 = new Rectangle((int)(Main.itemText[i].position.X - value2.X / 2f), (int)(Main.itemText[i].position.Y - value2.Y / 2f), (int)value2.X, (int)value2.Y);
						if (rectangle.Intersects(value3) && (this.position.Y < Main.itemText[i].position.Y || (this.position.Y == Main.itemText[i].position.Y && whoAmI < i)))
						{
							flag = true;
							int num = ItemText.numActive;
							if (num > 3)
							{
								num = 3;
							}
							Main.itemText[i].lifeTime = ItemText.activeTime + 15 * num;
							this.lifeTime = ItemText.activeTime + 15 * num;
						}
					}
				}
				if (!flag)
				{
					this.velocity.Y = this.velocity.Y * 0.86f;
					if (this.scale == 1f)
					{
						this.velocity.Y = this.velocity.Y * 0.4f;
					}
				}
				else if (this.velocity.Y > -6f)
				{
					this.velocity.Y = this.velocity.Y - 0.2f;
				}
				else
				{
					this.velocity.Y = this.velocity.Y * 0.86f;
				}
				this.velocity.X = this.velocity.X * 0.93f;
				this.position += this.velocity;
				this.lifeTime--;
				if (this.lifeTime <= 0)
				{
					this.scale -= 0.03f;
					if ((double)this.scale < 0.1)
					{
						this.active = false;
					}
					this.lifeTime = 0;
					return;
				}
				if (this.scale < 1f)
				{
					this.scale += 0.1f;
				}
				if (this.scale > 1f)
				{
					this.scale = 1f;
				}
			}
		}

		public static void UpdateItemText()
		{
			int num = 0;
			for (int i = 0; i < 20; i++)
			{
				if (Main.itemText[i].active)
				{
					num++;
					Main.itemText[i].Update(i);
				}
			}
			ItemText.numActive = num;
		}
	}
}
