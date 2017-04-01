using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using Terraria.UI.Chat;

namespace Terraria.GameContent.UI.Chat
{
	public class GlyphTagHandler : ITagHandler
	{
		private class GlyphSnippet : TextSnippet
		{
			private int _glyphIndex;

			public GlyphSnippet(int index)
				: base("")
			{
				this._glyphIndex = index;
				this.Color = Color.White;
			}

			public override bool UniqueDraw(bool justCheckingString, out Vector2 size, SpriteBatch spriteBatch, Vector2 position = default(Vector2), Color color = default(Color), float scale = 1f)
			{
				if (!justCheckingString && color != Color.Black)
				{
					int num = this._glyphIndex;
					int glyphIndex = this._glyphIndex;
					if (glyphIndex == 25)
					{
						num = ((Main.GlobalTime % 0.6f < 0.3f) ? 17 : 18);
					}
					Texture2D texture2D = Main.textGlyphTexture[0];
					spriteBatch.Draw(texture2D, position, new Rectangle?(texture2D.Frame(25, 1, num, num / 25)), color, 0f, Vector2.Zero, GlyphTagHandler.GlyphsScale, SpriteEffects.None, 0f);
				}
				size = new Vector2(26f) * GlyphTagHandler.GlyphsScale;
				return true;
			}

			public override float GetStringLength(SpriteFont font)
			{
				return 26f * GlyphTagHandler.GlyphsScale;
			}
		}

		private const int GlyphsPerLine = 25;
		private const int MaxGlyphs = 26;
		public static float GlyphsScale = 1f;
		private static Dictionary<string, int> GlyphIndexes = new Dictionary<string, int>
		{
			{
				Buttons.A.ToString(),
				0
			},
			{
				Buttons.B.ToString(),
				1
			},
			{
				Buttons.Back.ToString(),
				4
			},
			{
				Buttons.DPadDown.ToString(),
				15
			},
			{
				Buttons.DPadLeft.ToString(),
				14
			},
			{
				Buttons.DPadRight.ToString(),
				13
			},
			{
				Buttons.DPadUp.ToString(),
				16
			},
			{
				Buttons.LeftShoulder.ToString(),
				6
			},
			{
				Buttons.LeftStick.ToString(),
				10
			},
			{
				Buttons.LeftThumbstickDown.ToString(),
				20
			},
			{
				Buttons.LeftThumbstickLeft.ToString(),
				17
			},
			{
				Buttons.LeftThumbstickRight.ToString(),
				18
			},
			{
				Buttons.LeftThumbstickUp.ToString(),
				19
			},
			{
				Buttons.LeftTrigger.ToString(),
				8
			},
			{
				Buttons.RightShoulder.ToString(),
				7
			},
			{
				Buttons.RightStick.ToString(),
				11
			},
			{
				Buttons.RightThumbstickDown.ToString(),
				24
			},
			{
				Buttons.RightThumbstickLeft.ToString(),
				21
			},
			{
				Buttons.RightThumbstickRight.ToString(),
				22
			},
			{
				Buttons.RightThumbstickUp.ToString(),
				23
			},
			{
				Buttons.RightTrigger.ToString(),
				9
			},
			{
				Buttons.Start.ToString(),
				5
			},
			{
				Buttons.X.ToString(),
				2
			},
			{
				Buttons.Y.ToString(),
				3
			},
			{
				"LR",
				25
			}
		};

		TextSnippet ITagHandler.Parse(string text, Color baseColor, string options)
		{
			int num;
			if (!int.TryParse(text, out num) || num >= 26)
			{
				return new TextSnippet(text);
			}
			return new GlyphTagHandler.GlyphSnippet(num)
			{
				DeleteWhole = true,
				Text = "[g:" + num + "]"
			};
		}

		public static string GenerateTag(int index)
		{
			string text = "[g";
			object obj = text;
			return string.Concat(new object[]
				{
					obj,
					":",
					index,
					"]"
				});
		}

		public static string GenerateTag(string keyname)
		{
			int index;
			if (GlyphTagHandler.GlyphIndexes.TryGetValue(keyname, out index))
			{
				return GlyphTagHandler.GenerateTag(index);
			}
			return keyname;
		}
	}
}
