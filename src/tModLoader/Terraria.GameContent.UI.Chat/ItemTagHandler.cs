using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Terraria.GameContent.UI.Chat
{
	public class ItemTagHandler : ITagHandler
	{
		private class ItemSnippet : TextSnippet
		{
			private Item _item;

			public ItemSnippet(Item item)
				: base("")
			{
				this._item = item;
				this.Color = ItemRarity.GetColor(item.rare);
			}

			public override void OnHover()
			{
				Main.toolTip = this._item.Clone();
				Main.instance.MouseText(this._item.name, this._item.rare, 0);
			}

			public override bool UniqueDraw(bool justCheckingString, out Vector2 size, SpriteBatch spriteBatch, Vector2 position = default(Vector2), Color color = default(Color), float scale = 1f)
			{
				float num = 1f;
				float num2 = 1f;
				if (Main.netMode != 2 && !Main.dedServ)
				{
					Texture2D texture2D = Main.itemTexture[this._item.type];
					Rectangle rectangle;
					if (Main.itemAnimations[this._item.type] != null)
					{
						rectangle = Main.itemAnimations[this._item.type].GetFrame(texture2D);
					}
					else
					{
						rectangle = texture2D.Frame(1, 1, 0, 0);
					}
					if (rectangle.Height > 32)
					{
						num2 = 32f / (float)rectangle.Height;
					}
				}
				num2 *= scale;
				num *= num2;
				if (num > 0.75f)
				{
					num = 0.75f;
				}
				if (!justCheckingString && color != Color.Black)
				{
					float inventoryScale = Main.inventoryScale;
					Main.inventoryScale = scale * num;
					ItemSlot.Draw(spriteBatch, ref this._item, 14, position - new Vector2(10f) * scale * num, Color.White);
					Main.inventoryScale = inventoryScale;
				}
				size = new Vector2(32f) * scale * num;
				return true;
			}

			public override float GetStringLength(SpriteFont font)
			{
				return 32f * this.Scale * 0.65f;
			}
		}

		TextSnippet ITagHandler.Parse(string text, Color baseColor, string options)
		{
			Item item = new Item();
			int type;
			if (int.TryParse(text, out type))
			{
				item.netDefaults(type);
			}
			else
			{
				item.SetDefaults(text);
			}
			if (item.type <= 0)
			{
				return new TextSnippet(text);
			}
			item.stack = 1;
			if (options != null)
			{
				string[] array = options.Split(new char[]
					{
						','
					});
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].Length != 0)
					{
						char c = array[i][0];
						int value2;
						if (c != 'p')
						{
							int value;
							if ((c == 's' || c == 'x') && int.TryParse(array[i].Substring(1), out value))
							{
								item.stack = Utils.Clamp<int>(value, 1, item.maxStack);
							}
						}
						else if (int.TryParse(array[i].Substring(1), out value2))
						{
							item.Prefix((int)((byte)Utils.Clamp<int>(value2, 0, 84)));
						}
					}
				}
			}
			string str = "";
			if (item.stack > 1)
			{
				str = " (" + item.stack + ")";
			}
			return new ItemTagHandler.ItemSnippet(item)
			{
				Text = "[" + item.AffixName() + str + "]",
				CheckForHover = true,
				DeleteWhole = true
			};
		}

		public static string GenerateTag(Item I)
		{
			string text = "[i";
			if (I.prefix != 0)
			{
				text = text + "/p" + I.prefix;
			}
			if (I.stack != 1)
			{
				text = text + "/s" + I.stack;
			}
			object obj = text;
			return string.Concat(new object[]
				{
					obj,
					":",
					I.netID,
					"]"
				});
		}
	}
}
