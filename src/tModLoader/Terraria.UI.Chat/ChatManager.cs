using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Terraria.GameContent.UI.Chat;

namespace Terraria.UI.Chat
{
	public static class ChatManager
	{
		public static class Regexes
		{
			public static readonly Regex Format = new Regex("(?<!\\\\)\\[(?<tag>[a-zA-Z]{1,10})(\\/(?<options>[^:]+))?:(?<text>.+?)(?<!\\\\)\\]", RegexOptions.Compiled);
		}

		private static ConcurrentDictionary<string, ITagHandler> _handlers = new ConcurrentDictionary<string, ITagHandler>();
		public static readonly Vector2[] ShadowDirections = new Vector2[]
		{
			-Vector2.UnitX,
			Vector2.UnitX,
			-Vector2.UnitY,
			Vector2.UnitY
		};

		public static Color WaveColor(Color color)
		{
			float num = (float)Main.mouseTextColor / 255f;
			color = Color.Lerp(color, Color.Black, 1f - num);
			color.A = Main.mouseTextColor;
			return color;
		}

		public static void ConvertNormalSnippets(TextSnippet[] snippets)
		{
			for (int i = 0; i < snippets.Length; i++)
			{
				TextSnippet textSnippet = snippets[i];
				if (snippets[i].GetType() == typeof(TextSnippet))
				{
					PlainTagHandler.PlainSnippet plainSnippet = new PlainTagHandler.PlainSnippet(textSnippet.Text, textSnippet.Color, textSnippet.Scale);
					snippets[i] = plainSnippet;
				}
			}
		}

		public static void Register<T>(params string[] names) where T : ITagHandler, new()
		{
#if MAC
			T t = Activator.CreateInstance<T>();
#else
			T t = (default(T) == null) ? Activator.CreateInstance<T>() : default(T);
#endif
			for (int i = 0; i < names.Length; i++)
			{
				ChatManager._handlers[names[i].ToLower()] = t;
			}
		}

		private static ITagHandler GetHandler(string tagName)
		{
			string key = tagName.ToLower();
			if (ChatManager._handlers.ContainsKey(key))
			{
				return ChatManager._handlers[key];
			}
			return null;
		}

		public static TextSnippet[] ParseMessage(string text, Color baseColor)
		{
			MatchCollection matchCollection = ChatManager.Regexes.Format.Matches(text);
			List<TextSnippet> list = new List<TextSnippet>();
			int num = 0;
			foreach (Match match in matchCollection)
			{
				if (match.Index > num)
				{
					list.Add(new TextSnippet(text.Substring(num, match.Index - num), baseColor, 1f));
				}
				num = match.Index + match.Length;
				string value = match.Groups["tag"].Value;
				string value2 = match.Groups["text"].Value;
				string value3 = match.Groups["options"].Value;
				ITagHandler handler = ChatManager.GetHandler(value);
				if (handler != null)
				{
					list.Add(handler.Parse(value2, baseColor, value3));
					list[list.Count - 1].TextOriginal = match.ToString();
				}
				else
				{
					list.Add(new TextSnippet(value2, baseColor, 1f));
				}
			}
			if (text.Length > num)
			{
				list.Add(new TextSnippet(text.Substring(num, text.Length - num), baseColor, 1f));
			}
			return list.ToArray();
		}

		public static bool AddChatText(SpriteFont font, string text, Vector2 baseScale)
		{
			int num = Main.screenWidth - 330;
			if (ChatManager.GetStringSize(font, Main.chatText + text, baseScale, -1f).X > (float)num)
			{
				return false;
			}
			Main.chatText += text;
			return true;
		}

		public static Vector2 GetStringSize(SpriteFont font, string text, Vector2 baseScale, float maxWidth = -1f)
		{
			TextSnippet[] snippets = ChatManager.ParseMessage(text, Color.White);
			return ChatManager.GetStringSize(font, snippets, baseScale, maxWidth);
		}

		public static Vector2 GetStringSize(SpriteFont font, TextSnippet[] snippets, Vector2 baseScale, float maxWidth = -1f)
		{
			Vector2 vec = new Vector2((float)Main.mouseX, (float)Main.mouseY);
			Vector2 zero = Vector2.Zero;
			Vector2 vector = zero;
			Vector2 result = vector;
			float x = font.MeasureString(" ").X;
			float num = 0f;
			for (int i = 0; i < snippets.Length; i++)
			{
				TextSnippet textSnippet = snippets[i];
				textSnippet.Update();
				float scale = textSnippet.Scale;
				Vector2 vector2;
				if (textSnippet.UniqueDraw(true, out vector2, null, default(Vector2), default(Color), 1f))
				{
					vector.X += vector2.X * baseScale.X * scale;
					result.X = Math.Max(result.X, vector.X);
					result.Y = Math.Max(result.Y, vector.Y + vector2.Y);
				}
				else
				{
					string[] array = textSnippet.Text.Split(new char[]
						{
							'\n'
						});
					string[] array2 = array;
					for (int j = 0; j < array2.Length; j++)
					{
						string text = array2[j];
						string[] array3 = text.Split(new char[]
							{
								' '
							});
						for (int k = 0; k < array3.Length; k++)
						{
							if (k != 0)
							{
								vector.X += x * baseScale.X * scale;
							}
							if (maxWidth > 0f)
							{
								float num2 = font.MeasureString(array3[k]).X * baseScale.X * scale;
								if (vector.X - zero.X + num2 > maxWidth)
								{
									vector.X = zero.X;
									vector.Y += (float)font.LineSpacing * num * baseScale.Y;
									result.Y = Math.Max(result.Y, vector.Y);
									num = 0f;
								}
							}
							if (num < scale)
							{
								num = scale;
							}
							Vector2 value = font.MeasureString(array3[k]);
							vec.Between(vector, vector + value);
							vector.X += value.X * baseScale.X * scale;
							result.X = Math.Max(result.X, vector.X);
							result.Y = Math.Max(result.Y, vector.Y + value.Y);
						}
						if (array.Length > 1)
						{
							vector.X = zero.X;
							vector.Y += (float)font.LineSpacing * num * baseScale.Y;
							result.Y = Math.Max(result.Y, vector.Y);
							num = 0f;
						}
					}
				}
			}
			return result;
		}

		public static void DrawColorCodedStringShadow(SpriteBatch spriteBatch, SpriteFont font, TextSnippet[] snippets, Vector2 position, Color baseColor, float rotation, Vector2 origin, Vector2 baseScale, float maxWidth = -1f, float spread = 2f)
		{
			for (int i = 0; i < ChatManager.ShadowDirections.Length; i++)
			{
				int num;
				ChatManager.DrawColorCodedString(spriteBatch, font, snippets, position + ChatManager.ShadowDirections[i] * spread, baseColor, rotation, origin, baseScale, out num, maxWidth, true);
			}
		}

		public static Vector2 DrawColorCodedString(SpriteBatch spriteBatch, SpriteFont font, TextSnippet[] snippets, Vector2 position, Color baseColor, float rotation, Vector2 origin, Vector2 baseScale, out int hoveredSnippet, float maxWidth, bool ignoreColors = false)
		{
			int num = -1;
			Vector2 vec = new Vector2((float)Main.mouseX, (float)Main.mouseY);
			Vector2 vector = position;
			Vector2 result = vector;
			float x = font.MeasureString(" ").X;
			Color color = baseColor;
			float num2 = 0f;
			for (int i = 0; i < snippets.Length; i++)
			{
				TextSnippet textSnippet = snippets[i];
				textSnippet.Update();
				if (!ignoreColors)
				{
					color = textSnippet.GetVisibleColor();
				}
				float scale = textSnippet.Scale;
				Vector2 value;
				if (textSnippet.UniqueDraw(false, out value, spriteBatch, vector, color, scale))
				{
					if (vec.Between(vector, vector + value))
					{
						num = i;
					}
					vector.X += value.X * baseScale.X * scale;
					result.X = Math.Max(result.X, vector.X);
				}
				else
				{
					string[] array = textSnippet.Text.Split(new char[]
						{
							'\n'
						});
					string[] array2 = array;
					for (int j = 0; j < array2.Length; j++)
					{
						string text = array2[j];
						string[] array3 = text.Split(new char[]
							{
								' '
							});
						for (int k = 0; k < array3.Length; k++)
						{
							if (k != 0)
							{
								vector.X += x * baseScale.X * scale;
							}
							if (maxWidth > 0f)
							{
								float num3 = font.MeasureString(array3[k]).X * baseScale.X * scale;
								if (vector.X - position.X + num3 > maxWidth)
								{
									vector.X = position.X;
									vector.Y += (float)font.LineSpacing * num2 * baseScale.Y;
									result.Y = Math.Max(result.Y, vector.Y);
									num2 = 0f;
								}
							}
							if (num2 < scale)
							{
								num2 = scale;
							}
							spriteBatch.DrawString(font, array3[k], vector, color, rotation, origin, baseScale * textSnippet.Scale * scale, SpriteEffects.None, 0f);
							Vector2 value2 = font.MeasureString(array3[k]);
							if (vec.Between(vector, vector + value2))
							{
								num = i;
							}
							vector.X += value2.X * baseScale.X * scale;
							result.X = Math.Max(result.X, vector.X);
						}
						if (array.Length > 1)
						{
							vector.Y += (float)font.LineSpacing * num2 * baseScale.Y;
							vector.X = position.X;
							result.Y = Math.Max(result.Y, vector.Y);
							num2 = 0f;
						}
					}
				}
			}
			hoveredSnippet = num;
			return result;
		}

		public static Vector2 DrawColorCodedStringWithShadow(SpriteBatch spriteBatch, SpriteFont font, TextSnippet[] snippets, Vector2 position, float rotation, Vector2 origin, Vector2 baseScale, out int hoveredSnippet, float maxWidth = -1f, float spread = 2f)
		{
			ChatManager.DrawColorCodedStringShadow(spriteBatch, font, snippets, position, Color.Black, rotation, origin, baseScale, maxWidth, spread);
			return ChatManager.DrawColorCodedString(spriteBatch, font, snippets, position, Color.White, rotation, origin, baseScale, out hoveredSnippet, maxWidth, false);
		}

		public static void DrawColorCodedStringShadow(SpriteBatch spriteBatch, SpriteFont font, string text, Vector2 position, Color baseColor, float rotation, Vector2 origin, Vector2 baseScale, float maxWidth = -1f, float spread = 2f)
		{
			for (int i = 0; i < ChatManager.ShadowDirections.Length; i++)
			{
				ChatManager.DrawColorCodedString(spriteBatch, font, text, position + ChatManager.ShadowDirections[i] * spread, baseColor, rotation, origin, baseScale, maxWidth, true);
			}
		}

		public static Vector2 DrawColorCodedString(SpriteBatch spriteBatch, SpriteFont font, string text, Vector2 position, Color baseColor, float rotation, Vector2 origin, Vector2 baseScale, float maxWidth = -1f, bool ignoreColors = false)
		{
			Vector2 vector = position;
			Vector2 result = vector;
			string[] array = text.Split(new char[]
				{
					'\n'
				});
			float x = font.MeasureString(" ").X;
			Color color = baseColor;
			float num = 1f;
			float num2 = 0f;
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string text2 = array2[i];
				string[] array3 = text2.Split(new char[]
					{
						':'
					});
				string[] array4 = array3;
				for (int j = 0; j < array4.Length; j++)
				{
					string text3 = array4[j];
					if (text3.StartsWith("sss"))
					{
						if (text3.StartsWith("sss1"))
						{
							if (!ignoreColors)
							{
								color = Color.Red;
							}
						}
						else if (text3.StartsWith("sss2"))
						{
							if (!ignoreColors)
							{
								color = Color.Blue;
							}
						}
						else if (text3.StartsWith("sssr") && !ignoreColors)
						{
							color = Color.White;
						}
					}
					else
					{
						string[] array5 = text3.Split(new char[]
							{
								' '
							});
						for (int k = 0; k < array5.Length; k++)
						{
							if (k != 0)
							{
								vector.X += x * baseScale.X * num;
							}
							if (maxWidth > 0f)
							{
								float num3 = font.MeasureString(array5[k]).X * baseScale.X * num;
								if (vector.X - position.X + num3 > maxWidth)
								{
									vector.X = position.X;
									vector.Y += (float)font.LineSpacing * num2 * baseScale.Y;
									result.Y = Math.Max(result.Y, vector.Y);
									num2 = 0f;
								}
							}
							if (num2 < num)
							{
								num2 = num;
							}
							spriteBatch.DrawString(font, array5[k], vector, color, rotation, origin, baseScale * num, SpriteEffects.None, 0f);
							vector.X += font.MeasureString(array5[k]).X * baseScale.X * num;
							result.X = Math.Max(result.X, vector.X);
						}
					}
				}
				vector.X = position.X;
				vector.Y += (float)font.LineSpacing * num2 * baseScale.Y;
				result.Y = Math.Max(result.Y, vector.Y);
				num2 = 0f;
			}
			return result;
		}

		public static Vector2 DrawColorCodedStringWithShadow(SpriteBatch spriteBatch, SpriteFont font, string text, Vector2 position, Color baseColor, float rotation, Vector2 origin, Vector2 baseScale, float maxWidth = -1f, float spread = 2f)
		{
			TextSnippet[] snippets = ChatManager.ParseMessage(text, baseColor);
			ChatManager.ConvertNormalSnippets(snippets);
			ChatManager.DrawColorCodedStringShadow(spriteBatch, font, snippets, position, Color.Black, rotation, origin, baseScale, maxWidth, spread);
			int num;
			return ChatManager.DrawColorCodedString(spriteBatch, font, snippets, position, Color.White, rotation, origin, baseScale, out num, maxWidth, false);
		}
	}
}
