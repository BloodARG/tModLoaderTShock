using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Terraria.DataStructures;
using Terraria.UI.Chat;
using Terraria.Utilities;

namespace Terraria
{
	public static class Utils
	{
		public delegate bool PerLinePoint(int x, int y);

		public delegate void LaserLineFraming(int stage, Vector2 currentPosition, float distanceLeft, Rectangle lastFrame, out float distanceCovered, out Rectangle frame, out Vector2 origin, out Color color);

		[StructLayout(LayoutKind.Explicit)]
		private struct IntFloat
		{
			[FieldOffset(0)]
			public readonly int IntValue;

			[FieldOffset(0)]
			public readonly float FloatValue;

			public IntFloat(int value)
			{
				this.FloatValue = 0f;
				this.IntValue = value;
			}

			public IntFloat(float value)
			{
				this.IntValue = 0;
				this.FloatValue = value;
			}
		}

		[StructLayout(LayoutKind.Explicit)]
		private struct UIntFloat
		{
			[FieldOffset(0)]
			public readonly uint UIntValue;

			[FieldOffset(0)]
			public readonly float FloatValue;

			public UIntFloat(uint value)
			{
				this.FloatValue = 0f;
				this.UIntValue = value;
			}

			public UIntFloat(float value)
			{
				this.UIntValue = 0u;
				this.FloatValue = value;
			}
		}

		public const long MaxCoins = 999999999L;

		private const ulong RANDOM_MULTIPLIER = 25214903917uL;

		private const ulong RANDOM_ADD = 11uL;

		private const ulong RANDOM_MASK = 281474976710655uL;

		public static Dictionary<SpriteFont, float[]> charLengths = new Dictionary<SpriteFont, float[]>();

		public static float ReadUIntAsFloat(uint value)
		{
			return new Utils.UIntFloat(value).FloatValue;
		}

		public static float ReadIntAsFloat(int value)
		{
			return new Utils.IntFloat(value).FloatValue;
		}

		public static uint ReadFloatAsUInt(float value)
		{
			return new Utils.UIntFloat(value).UIntValue;
		}

		public static int ReadFloatAsInt(float value)
		{
			return new Utils.IntFloat(value).IntValue;
		}

		public static bool IsPowerOfTwo(int x)
		{
			return x != 0 && (x & x - 1) == 0;
		}

		public static float SmoothStep(float min, float max, float x)
		{
			return MathHelper.Clamp((x - min) / (max - min), 0f, 1f);
		}

		public static Dictionary<string, string> ParseArguements(string[] args)
		{
			string text = null;
			string text2 = "";
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			for (int i = 0; i < args.Length; i++)
			{
				if (args[i].Length != 0)
				{
					if (args[i][0] == '-' || args[i][0] == '+')
					{
						if (text != null)
						{
							dictionary.Add(text.ToLower(), text2);
						}
						text = args[i];
						text2 = "";
					}
					else
					{
						if (text2 != "")
						{
							text2 += " ";
						}
						text2 += args[i];
					}
				}
			}
			if (text != null)
			{
				dictionary.Add(text.ToLower(), text2);
			}
			return dictionary;
		}

		public static void Swap<T>(ref T t1, ref T t2)
		{
			T t3 = t1;
			t1 = t2;
			t2 = t3;
		}

		public static T Clamp<T>(T value, T min, T max) where T : IComparable<T>
		{
			if (value.CompareTo(max) > 0)
			{
				return max;
			}
			if (value.CompareTo(min) < 0)
			{
				return min;
			}
			return value;
		}

		public static float InverseLerp(float from, float to, float t, bool clamped = false)
		{
			if (clamped)
			{
				if (from < to)
				{
					if (t < from)
					{
						return 0f;
					}
					if (t > to)
					{
						return 1f;
					}
				}
				else
				{
					if (t < to)
					{
						return 1f;
					}
					if (t > from)
					{
						return 0f;
					}
				}
			}
			return (t - from) / (to - from);
		}

		public static string[] FixArgs(string[] brokenArgs)
		{
			ArrayList arrayList = new ArrayList();
			string text = "";
			for (int i = 0; i < brokenArgs.Length; i++)
			{
				if (brokenArgs[i].StartsWith("-"))
				{
					if (text != "")
					{
						arrayList.Add(text);
						text = "";
					}
					else
					{
						arrayList.Add("");
					}
					arrayList.Add(brokenArgs[i]);
				}
				else
				{
					if (text != "")
					{
						text += " ";
					}
					text += brokenArgs[i];
				}
			}
			arrayList.Add(text);
			string[] array = new string[arrayList.Count];
			arrayList.CopyTo(array);
			return array;
		}

		public static List<List<TextSnippet>> WordwrapStringSmart(string text, Color c, SpriteFont font, int maxWidth, int maxLines)
		{
			TextSnippet[] array = ChatManager.ParseMessage(text, c);
			List<List<TextSnippet>> list = new List<List<TextSnippet>>();
			List<TextSnippet> list2 = new List<TextSnippet>();
			for (int i = 0; i < array.Length; i++)
			{
				TextSnippet textSnippet = array[i];
				string[] array2 = textSnippet.Text.Split(new char[]
				{
					'\n'
				});
				for (int j = 0; j < array2.Length - 1; j++)
				{
					list2.Add(textSnippet.CopyMorph(array2[j]));
					list.Add(list2);
					list2 = new List<TextSnippet>();
				}
				list2.Add(textSnippet.CopyMorph(array2[array2.Length - 1]));
			}
			list.Add(list2);
			if (maxWidth != -1)
			{
				for (int k = 0; k < list.Count; k++)
				{
					List<TextSnippet> list3 = list[k];
					float num = 0f;
					for (int l = 0; l < list3.Count; l++)
					{
						float stringLength = list3[l].GetStringLength(font);
						if (stringLength + num > (float)maxWidth)
						{
							int num2 = maxWidth - (int)num;
							if (num > 0f)
							{
								num2 -= 16;
							}
							int num3 = Math.Min(list3[l].Text.Length, num2 / 8);
							if (num3 < 0)
							{
								num3 = 0;
							}
							string[] array3 = list3[l].Text.Split(new char[]
							{
								' '
							});
							int num4 = num3;
							if (array3.Length > 1)
							{
								num4 = 0;
								int num5 = 0;
								while (num5 < array3.Length && num4 + array3[num5].Length <= num3)
								{
									num4 += array3[num5].Length + 1;
									num5++;
								}
								if (num4 > num3)
								{
									num4 = num3;
								}
							}
							string newText = list3[l].Text.Substring(0, num4);
							string newText2 = list3[l].Text.Substring(num4);
							list2 = new List<TextSnippet>
							{
								list3[l].CopyMorph(newText2)
							};
							for (int m = l + 1; m < list3.Count; m++)
							{
								list2.Add(list3[m]);
							}
							list3[l] = list3[l].CopyMorph(newText);
							list[k] = list[k].Take(l + 1).ToList<TextSnippet>();
							list.Insert(k + 1, list2);
							break;
						}
						num += stringLength;
					}
				}
			}
			if (maxLines != -1)
			{
				while (list.Count > 10)
				{
					list.RemoveAt(10);
				}
			}
			return list;
		}

		public static string[] WordwrapString(string text, SpriteFont font, int maxWidth, int maxLines, out int lineAmount)
		{
			string[] array = new string[maxLines];
			int num = 0;
			List<string> list = new List<string>(text.Split(new char[]
			{
				'\n'
			}));
			List<string> list2 = new List<string>(list[0].Split(new char[]
			{
				' '
			}));
			for (int i = 1; i < list.Count; i++)
			{
				list2.Add("\n");
				list2.AddRange(list[i].Split(new char[]
				{
					' '
				}));
			}
			bool flag = true;
			while (list2.Count > 0)
			{
				string text2 = list2[0];
				string str = " ";
				if (list2.Count == 1)
				{
					str = "";
				}
				if (text2 == "\n")
				{
					string[] array2;
					IntPtr intPtr;
					(array2 = array)[(int)(intPtr = (IntPtr)(num++))] = array2[(int)intPtr] + text2;
					if (num >= maxLines)
					{
						break;
					}
					list2.RemoveAt(0);
				}
				else if (flag)
				{
					if (font.MeasureString(text2).X > (float)maxWidth)
					{
						string text3 = string.Concat(text2[0]);
						int num2 = 1;
						while (font.MeasureString(text3 + text2[num2] + '-').X <= (float)maxWidth)
						{
							text3 += text2[num2++];
						}
						text3 += '-';
						array[num++] = text3 + " ";
						if (num >= maxLines)
						{
							break;
						}
						list2.RemoveAt(0);
						list2.Insert(0, text2.Substring(num2));
					}
					else
					{
						string[] array3;
						IntPtr intPtr2;
						(array3 = array)[(int)(intPtr2 = (IntPtr)num)] = array3[(int)intPtr2] + text2 + str;
						flag = false;
						list2.RemoveAt(0);
					}
				}
				else if (font.MeasureString(array[num] + text2).X > (float)maxWidth)
				{
					num++;
					if (num >= maxLines)
					{
						break;
					}
					flag = true;
				}
				else
				{
					string[] array4;
					IntPtr intPtr3;
					(array4 = array)[(int)(intPtr3 = (IntPtr)num)] = array4[(int)intPtr3] + text2 + str;
					flag = false;
					list2.RemoveAt(0);
				}
			}
			lineAmount = num;
			if (lineAmount == maxLines)
			{
				lineAmount--;
			}
			return array;
		}

		public static Rectangle CenteredRectangle(Vector2 center, Vector2 size)
		{
			return new Rectangle((int)(center.X - size.X / 2f), (int)(center.Y - size.Y / 2f), (int)size.X, (int)size.Y);
		}

		public static Vector2 Vector2FromElipse(Vector2 angleVector, Vector2 elipseSizes)
		{
			if (elipseSizes == Vector2.Zero)
			{
				return Vector2.Zero;
			}
			if (angleVector == Vector2.Zero)
			{
				return Vector2.Zero;
			}
			angleVector.Normalize();
			Vector2 value = Vector2.Normalize(elipseSizes);
			value = Vector2.One / value;
			angleVector *= value;
			angleVector.Normalize();
			return angleVector * elipseSizes / 2f;
		}

		public static bool FloatIntersect(float r1StartX, float r1StartY, float r1Width, float r1Height, float r2StartX, float r2StartY, float r2Width, float r2Height)
		{
			return r1StartX <= r2StartX + r2Width && r1StartY <= r2StartY + r2Height && r1StartX + r1Width >= r2StartX && r1StartY + r1Height >= r2StartY;
		}

		public static long CoinsCount(out bool overFlowing, Item[] inv, params int[] ignoreSlots)
		{
			List<int> list = new List<int>(ignoreSlots);
			long num = 0L;
			for (int i = 0; i < inv.Length; i++)
			{
				if (!list.Contains(i))
				{
					switch (inv[i].type)
					{
					case 71:
						num += (long)inv[i].stack;
						break;
					case 72:
						num += (long)(inv[i].stack * 100);
						break;
					case 73:
						num += (long)(inv[i].stack * 10000);
						break;
					case 74:
						num += (long)(inv[i].stack * 1000000);
						break;
					}
					if (num >= 999999999L)
					{
						overFlowing = true;
						return 999999999L;
					}
				}
			}
			overFlowing = false;
			return num;
		}

		public static int[] CoinsSplit(long count)
		{
			int[] array = new int[4];
			long num = 0L;
			long num2 = 1000000L;
			for (int i = 3; i >= 0; i--)
			{
				array[i] = (int)((count - num) / num2);
				num += (long)array[i] * num2;
				num2 /= 100L;
			}
			return array;
		}

		public static long CoinsCombineStacks(out bool overFlowing, params long[] coinCounts)
		{
			long num = 0L;
			for (int i = 0; i < coinCounts.Length; i++)
			{
				long num2 = coinCounts[i];
				num += num2;
				if (num >= 999999999L)
				{
					overFlowing = true;
					return 999999999L;
				}
			}
			overFlowing = false;
			return num;
		}

		public static void PoofOfSmoke(Vector2 position)
		{
			int num = Main.rand.Next(3, 7);
			for (int i = 0; i < num; i++)
			{
				int num2 = Gore.NewGore(position, (Main.rand.NextFloat() * 6.28318548f).ToRotationVector2() * new Vector2(2f, 0.7f) * 0.7f, Main.rand.Next(11, 14), 1f);
				Main.gore[num2].scale = 0.7f;
			}
			for (int j = 0; j < 10; j++)
			{
				Dust dust = Main.dust[Dust.NewDust(position, 14, 14, 16, 0f, 0f, 100, default(Color), 1.5f)];
				dust.position += new Vector2(5f);
				dust.velocity = (Main.rand.NextFloat() * 6.28318548f).ToRotationVector2() * new Vector2(2f, 0.7f) * 0.7f * (0.5f + 0.5f * Main.rand.NextFloat());
			}
		}

		public static byte[] ToByteArray(this string str)
		{
			byte[] array = new byte[str.Length * 2];
			Buffer.BlockCopy(str.ToCharArray(), 0, array, 0, array.Length);
			return array;
		}

		public static float NextFloat(this UnifiedRandom r)
		{
			return (float)r.NextDouble();
		}

		public static float NextFloatDirection(this UnifiedRandom r)
		{
			return (float)r.NextDouble() * 2f - 1f;
		}

		public static Vector2 NextVector2Square(this UnifiedRandom r, float min, float max)
		{
			return new Vector2((max - min) * (float)r.NextDouble() + min, (max - min) * (float)r.NextDouble() + min);
		}

		public static Vector2 NextVector2Unit(this UnifiedRandom r, float startRotation = 0f, float rotationRange = 6.28318548f)
		{
			return (startRotation + rotationRange * r.NextFloat()).ToRotationVector2();
		}

		public static Vector2 NextVector2Circular(this UnifiedRandom r, float circleHalfWidth, float circleHalfHeight)
		{
			return r.NextVector2Unit(0f, 6.28318548f) * new Vector2(circleHalfWidth, circleHalfHeight) * r.NextFloat();
		}

		public static Vector2 NextVector2CircularEdge(this UnifiedRandom r, float circleHalfWidth, float circleHalfHeight)
		{
			return r.NextVector2Unit(0f, 6.28318548f) * new Vector2(circleHalfWidth, circleHalfHeight);
		}

		public static Rectangle Frame(this Texture2D tex, int horizontalFrames = 1, int verticalFrames = 1, int frameX = 0, int frameY = 0)
		{
			int num = tex.Width / horizontalFrames;
			int num2 = tex.Height / verticalFrames;
			return new Rectangle(num * frameX, num2 * frameY, num, num2);
		}

		public static Vector2 OriginFlip(this Rectangle rect, Vector2 origin, SpriteEffects effects)
		{
			if (effects.HasFlag(SpriteEffects.FlipHorizontally))
			{
				origin.X = (float)rect.Width - origin.X;
			}
			if (effects.HasFlag(SpriteEffects.FlipVertically))
			{
				origin.Y = (float)rect.Height - origin.Y;
			}
			return origin;
		}

		public static Vector2 Size(this Texture2D tex)
		{
			return new Vector2((float)tex.Width, (float)tex.Height);
		}

		public static void WriteRGB(this BinaryWriter bb, Color c)
		{
			bb.Write(c.R);
			bb.Write(c.G);
			bb.Write(c.B);
		}

		public static void WriteVector2(this BinaryWriter bb, Vector2 v)
		{
			bb.Write(v.X);
			bb.Write(v.Y);
		}

		public static void WritePackedVector2(this BinaryWriter bb, Vector2 v)
		{
			HalfVector2 halfVector = new HalfVector2(v.X, v.Y);
			bb.Write(halfVector.PackedValue);
		}

		public static Color ReadRGB(this BinaryReader bb)
		{
			return new Color((int)bb.ReadByte(), (int)bb.ReadByte(), (int)bb.ReadByte());
		}

		public static Vector2 ReadVector2(this BinaryReader bb)
		{
			return new Vector2(bb.ReadSingle(), bb.ReadSingle());
		}

		public static Vector2 ReadPackedVector2(this BinaryReader bb)
		{
			HalfVector2 halfVector = default(HalfVector2);
			halfVector.PackedValue = bb.ReadUInt32();
			return halfVector.ToVector2();
		}

		public static Vector2 Left(this Rectangle r)
		{
			return new Vector2((float)r.X, (float)(r.Y + r.Height / 2));
		}

		public static Vector2 Right(this Rectangle r)
		{
			return new Vector2((float)(r.X + r.Width), (float)(r.Y + r.Height / 2));
		}

		public static Vector2 Top(this Rectangle r)
		{
			return new Vector2((float)(r.X + r.Width / 2), (float)r.Y);
		}

		public static Vector2 Bottom(this Rectangle r)
		{
			return new Vector2((float)(r.X + r.Width / 2), (float)(r.Y + r.Height));
		}

		public static Vector2 TopLeft(this Rectangle r)
		{
			return new Vector2((float)r.X, (float)r.Y);
		}

		public static Vector2 TopRight(this Rectangle r)
		{
			return new Vector2((float)(r.X + r.Width), (float)r.Y);
		}

		public static Vector2 BottomLeft(this Rectangle r)
		{
			return new Vector2((float)r.X, (float)(r.Y + r.Height));
		}

		public static Vector2 BottomRight(this Rectangle r)
		{
			return new Vector2((float)(r.X + r.Width), (float)(r.Y + r.Height));
		}

		public static Vector2 Center(this Rectangle r)
		{
			return new Vector2((float)(r.X + r.Width / 2), (float)(r.Y + r.Height / 2));
		}

		public static Vector2 Size(this Rectangle r)
		{
			return new Vector2((float)r.Width, (float)r.Height);
		}

		public static float Distance(this Rectangle r, Vector2 point)
		{
			if (Utils.FloatIntersect((float)r.Left, (float)r.Top, (float)r.Width, (float)r.Height, point.X, point.Y, 0f, 0f))
			{
				return 0f;
			}
			if (point.X >= (float)r.Left && point.X <= (float)r.Right)
			{
				if (point.Y < (float)r.Top)
				{
					return (float)r.Top - point.Y;
				}
				return point.Y - (float)r.Bottom;
			}
			else if (point.Y >= (float)r.Top && point.Y <= (float)r.Bottom)
			{
				if (point.X < (float)r.Left)
				{
					return (float)r.Left - point.X;
				}
				return point.X - (float)r.Right;
			}
			else if (point.X < (float)r.Left)
			{
				if (point.Y < (float)r.Top)
				{
					return Vector2.Distance(point, r.TopLeft());
				}
				return Vector2.Distance(point, r.BottomLeft());
			}
			else
			{
				if (point.Y < (float)r.Top)
				{
					return Vector2.Distance(point, r.TopRight());
				}
				return Vector2.Distance(point, r.BottomRight());
			}
		}

		public static float ToRotation(this Vector2 v)
		{
			return (float)Math.Atan2((double)v.Y, (double)v.X);
		}

		public static Vector2 ToRotationVector2(this float f)
		{
			return new Vector2((float)Math.Cos((double)f), (float)Math.Sin((double)f));
		}

		public static Vector2 RotatedBy(this Vector2 spinningpoint, double radians, Vector2 center = default(Vector2))
		{
			float num = (float)Math.Cos(radians);
			float num2 = (float)Math.Sin(radians);
			Vector2 vector = spinningpoint - center;
			Vector2 result = center;
			result.X += vector.X * num - vector.Y * num2;
			result.Y += vector.X * num2 + vector.Y * num;
			return result;
		}

		public static Vector2 RotatedByRandom(this Vector2 spinninpoint, double maxRadians)
		{
			return spinninpoint.RotatedBy(Main.rand.NextDouble() * maxRadians - Main.rand.NextDouble() * maxRadians, default(Vector2));
		}

		public static Vector2 Floor(this Vector2 vec)
		{
			vec.X = (float)((int)vec.X);
			vec.Y = (float)((int)vec.Y);
			return vec;
		}

		public static bool HasNaNs(this Vector2 vec)
		{
			return float.IsNaN(vec.X) || float.IsNaN(vec.Y);
		}

		public static bool Between(this Vector2 vec, Vector2 minimum, Vector2 maximum)
		{
			return vec.X >= minimum.X && vec.X <= maximum.X && vec.Y >= minimum.Y && vec.Y <= maximum.Y;
		}

		public static Vector2 ToVector2(this Point p)
		{
			return new Vector2((float)p.X, (float)p.Y);
		}

		public static Vector2 ToWorldCoordinates(this Point p, float autoAddX = 8f, float autoAddY = 8f)
		{
			return p.ToVector2() * 16f + new Vector2(autoAddX, autoAddY);
		}

		public static Point16 ToTileCoordinates16(this Vector2 vec)
		{
			return new Point16((int)vec.X >> 4, (int)vec.Y >> 4);
		}

		public static Point ToTileCoordinates(this Vector2 vec)
		{
			return new Point((int)vec.X >> 4, (int)vec.Y >> 4);
		}

		public static Point ToPoint(this Vector2 v)
		{
			return new Point((int)v.X, (int)v.Y);
		}

		public static Vector2 SafeNormalize(this Vector2 v, Vector2 defaultValue)
		{
			if (v == Vector2.Zero)
			{
				return defaultValue;
			}
			return Vector2.Normalize(v);
		}

		public static Vector2 ClosestPointOnLine(this Vector2 P, Vector2 A, Vector2 B)
		{
			Vector2 value = P - A;
			Vector2 vector = B - A;
			float num = vector.LengthSquared();
			float num2 = Vector2.Dot(value, vector);
			float num3 = num2 / num;
			if (num3 < 0f)
			{
				return A;
			}
			if (num3 > 1f)
			{
				return B;
			}
			return A + vector * num3;
		}

		public static bool RectangleLineCollision(Vector2 rectTopLeft, Vector2 rectBottomRight, Vector2 lineStart, Vector2 lineEnd)
		{
			if (lineStart.Between(rectTopLeft, rectBottomRight) || lineEnd.Between(rectTopLeft, rectBottomRight))
			{
				return true;
			}
			Vector2 p = new Vector2(rectBottomRight.X, rectTopLeft.Y);
			Vector2 vector = new Vector2(rectTopLeft.X, rectBottomRight.Y);
			Vector2[] array = new Vector2[]
			{
				rectTopLeft.ClosestPointOnLine(lineStart, lineEnd),
				p.ClosestPointOnLine(lineStart, lineEnd),
				vector.ClosestPointOnLine(lineStart, lineEnd),
				rectBottomRight.ClosestPointOnLine(lineStart, lineEnd)
			};
			for (int i = 0; i < array.Length; i++)
			{
				if (array[0].Between(rectTopLeft, vector))
				{
					return true;
				}
			}
			return false;
		}

		public static Vector2 RotateRandom(this Vector2 spinninpoint, double maxRadians)
		{
			return spinninpoint.RotatedBy(Main.rand.NextDouble() * maxRadians - Main.rand.NextDouble() * maxRadians, default(Vector2));
		}

		public static Vector2 XY(this Vector4 vec)
		{
			return new Vector2(vec.X, vec.Y);
		}

		public static Vector2 ZW(this Vector4 vec)
		{
			return new Vector2(vec.Z, vec.W);
		}

		public static Vector3 XZW(this Vector4 vec)
		{
			return new Vector3(vec.X, vec.Z, vec.W);
		}

		public static Vector3 YZW(this Vector4 vec)
		{
			return new Vector3(vec.Y, vec.Z, vec.W);
		}

		public static Color MultiplyRGB(this Color firstColor, Color secondColor)
		{
			return new Color((int)((byte)((float)(firstColor.R * secondColor.R) / 255f)), (int)((byte)((float)(firstColor.G * secondColor.G) / 255f)), (int)((byte)((float)(firstColor.B * secondColor.B) / 255f)));
		}

		public static Color MultiplyRGBA(this Color firstColor, Color secondColor)
		{
			return new Color((int)((byte)((float)(firstColor.R * secondColor.R) / 255f)), (int)((byte)((float)(firstColor.G * secondColor.G) / 255f)), (int)((byte)((float)(firstColor.B * secondColor.B) / 255f)), (int)((byte)((float)(firstColor.A * secondColor.A) / 255f)));
		}

		public static string Hex3(this Color color)
		{
			return (color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2")).ToLower();
		}

		public static string Hex4(this Color color)
		{
			return (color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2") + color.A.ToString("X2")).ToLower();
		}

		public static int ToDirectionInt(this bool value)
		{
			if (!value)
			{
				return -1;
			}
			return 1;
		}

		public static int ToInt(this bool value)
		{
			if (!value)
			{
				return 0;
			}
			return 1;
		}

		public static float AngleLerp(this float curAngle, float targetAngle, float amount)
		{
			float angle;
			if (targetAngle < curAngle)
			{
				float num = targetAngle + 6.28318548f;
				angle = ((num - curAngle > curAngle - targetAngle) ? MathHelper.Lerp(curAngle, targetAngle, amount) : MathHelper.Lerp(curAngle, num, amount));
			}
			else
			{
				if (targetAngle <= curAngle)
				{
					return curAngle;
				}
				float num = targetAngle - 6.28318548f;
				angle = ((targetAngle - curAngle > curAngle - num) ? MathHelper.Lerp(curAngle, num, amount) : MathHelper.Lerp(curAngle, targetAngle, amount));
			}
			return MathHelper.WrapAngle(angle);
		}

		public static float AngleTowards(this float curAngle, float targetAngle, float maxChange)
		{
			curAngle = MathHelper.WrapAngle(curAngle);
			targetAngle = MathHelper.WrapAngle(targetAngle);
			if (curAngle < targetAngle)
			{
				if (targetAngle - curAngle > 3.14159274f)
				{
					curAngle += 6.28318548f;
				}
			}
			else if (curAngle - targetAngle > 3.14159274f)
			{
				curAngle -= 6.28318548f;
			}
			curAngle += MathHelper.Clamp(targetAngle - curAngle, -maxChange, maxChange);
			return MathHelper.WrapAngle(curAngle);
		}

		public static bool deepCompare(this int[] firstArray, int[] secondArray)
		{
			if (firstArray == null && secondArray == null)
			{
				return true;
			}
			if (firstArray == null || secondArray == null)
			{
				return false;
			}
			if (firstArray.Length != secondArray.Length)
			{
				return false;
			}
			for (int i = 0; i < firstArray.Length; i++)
			{
				if (firstArray[i] != secondArray[i])
				{
					return false;
				}
			}
			return true;
		}

		public static bool PlotLine(Point16 p0, Point16 p1, Utils.PerLinePoint plot, bool jump = true)
		{
			return Utils.PlotLine((int)p0.X, (int)p0.Y, (int)p1.X, (int)p1.Y, plot, jump);
		}

		public static bool PlotLine(Point p0, Point p1, Utils.PerLinePoint plot, bool jump = true)
		{
			return Utils.PlotLine(p0.X, p0.Y, p1.X, p1.Y, plot, jump);
		}

		private static bool PlotLine(int x0, int y0, int x1, int y1, Utils.PerLinePoint plot, bool jump = true)
		{
			if (x0 == x1 && y0 == y1)
			{
				return plot(x0, y0);
			}
			bool flag = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
			if (flag)
			{
				Utils.Swap<int>(ref x0, ref y0);
				Utils.Swap<int>(ref x1, ref y1);
			}
			int num = Math.Abs(x1 - x0);
			int num2 = Math.Abs(y1 - y0);
			int num3 = num / 2;
			int num4 = y0;
			int num5 = (x0 < x1) ? 1 : -1;
			int num6 = (y0 < y1) ? 1 : -1;
			for (int num7 = x0; num7 != x1; num7 += num5)
			{
				if (flag)
				{
					if (!plot(num4, num7))
					{
						return false;
					}
				}
				else if (!plot(num7, num4))
				{
					return false;
				}
				num3 -= num2;
				if (num3 < 0)
				{
					num4 += num6;
					if (!jump)
					{
						if (flag)
						{
							if (!plot(num4, num7))
							{
								return false;
							}
						}
						else if (!plot(num7, num4))
						{
							return false;
						}
					}
					num3 += num;
				}
			}
			return true;
		}

		public static int RandomNext(ref ulong seed, int bits)
		{
			seed = Utils.RandomNextSeed(seed);
			return (int)(seed >> 48 - bits);
		}

		public static ulong RandomNextSeed(ulong seed)
		{
			return seed * 25214903917uL + 11uL & 281474976710655uL;
		}

		public static float RandomFloat(ref ulong seed)
		{
			return (float)Utils.RandomNext(ref seed, 24) / 16777216f;
		}

		public static int RandomInt(ref ulong seed, int max)
		{
			if ((max & -max) == max)
			{
				return (int)((long)max * (long)Utils.RandomNext(ref seed, 31) >> 31);
			}
			int num;
			int num2;
			do
			{
				num = Utils.RandomNext(ref seed, 31);
				num2 = num % max;
			}
			while (num - num2 + (max - 1) < 0);
			return num2;
		}

		public static int RandomInt(ref ulong seed, int min, int max)
		{
			return Utils.RandomInt(ref seed, max - min) + min;
		}

		public static bool PlotTileLine(Vector2 start, Vector2 end, float width, Utils.PerLinePoint plot)
		{
			float scaleFactor = width / 2f;
			Vector2 value = end - start;
			Vector2 vector = value / value.Length();
			Vector2 value2 = new Vector2(-vector.Y, vector.X) * scaleFactor;
			Point point = (start - value2).ToTileCoordinates();
			Point point2 = (start + value2).ToTileCoordinates();
			Point point3 = start.ToTileCoordinates();
			Point point4 = end.ToTileCoordinates();
			Point lineMinOffset = new Point(point.X - point3.X, point.Y - point3.Y);
			Point lineMaxOffset = new Point(point2.X - point3.X, point2.Y - point3.Y);
			return Utils.PlotLine(point3.X, point3.Y, point4.X, point4.Y, (int x, int y) => Utils.PlotLine(x + lineMinOffset.X, y + lineMinOffset.Y, x + lineMaxOffset.X, y + lineMaxOffset.Y, plot, false), true);
		}

		public static bool PlotTileTale(Vector2 start, Vector2 end, float width, Utils.PerLinePoint plot)
		{
			float halfWidth = width / 2f;
			Vector2 value = end - start;
			Vector2 vector = value / value.Length();
			Vector2 perpOffset = new Vector2(-vector.Y, vector.X);
			Point pointStart = start.ToTileCoordinates();
			Point point = end.ToTileCoordinates();
			int length = 0;
			Utils.PlotLine(pointStart.X, pointStart.Y, point.X, point.Y, delegate(int x, int y)
			{
				length++;
				return true;
			}, true);
			length--;
			int curLength = 0;
			return Utils.PlotLine(pointStart.X, pointStart.Y, point.X, point.Y, delegate(int x, int y)
			{
				float scaleFactor = 1f - (float)curLength / (float)length;
				curLength++;
				Point point2 = (start - perpOffset * halfWidth * scaleFactor).ToTileCoordinates();
				Point point3 = (start + perpOffset * halfWidth * scaleFactor).ToTileCoordinates();
				Point point4 = new Point(point2.X - pointStart.X, point2.Y - pointStart.Y);
				Point point5 = new Point(point3.X - pointStart.X, point3.Y - pointStart.Y);
				return Utils.PlotLine(x + point4.X, y + point4.Y, x + point5.X, y + point5.Y, plot, false);
			}, true);
		}

		public static int RandomConsecutive(double random, int odds)
		{
			return (int)Math.Log(1.0 - random, 1.0 / (double)odds);
		}

		public static Vector2 RandomVector2(UnifiedRandom random, float min, float max)
		{
			return new Vector2((max - min) * (float)random.NextDouble() + min, (max - min) * (float)random.NextDouble() + min);
		}

		public static bool IndexInRange<T>(this T[] t, int index)
		{
			return index >= 0 && index < t.Length;
		}

		public static bool IndexInRange<T>(this List<T> t, int index)
		{
			return index >= 0 && index < t.Count;
		}

		public static T SelectRandom<T>(UnifiedRandom random, params T[] choices)
		{
			return choices[random.Next(choices.Length)];
		}

		public static void DrawBorderStringFourWay(SpriteBatch sb, SpriteFont font, string text, float x, float y, Color textColor, Color borderColor, Vector2 origin, float scale = 1f)
		{
			Color color = borderColor;
			Vector2 zero = Vector2.Zero;
			int i = 0;
			while (i < 5)
			{
				switch (i)
				{
				case 0:
					zero.X = x - 2f;
					zero.Y = y;
					break;
				case 1:
					zero.X = x + 2f;
					zero.Y = y;
					break;
				case 2:
					zero.X = x;
					zero.Y = y - 2f;
					break;
				case 3:
					zero.X = x;
					zero.Y = y + 2f;
					break;
				case 4:
					goto IL_92;
				default:
					goto IL_92;
				}
				IL_A6:
				sb.DrawString(font, text, zero, color, 0f, origin, scale, SpriteEffects.None, 0f);
				i++;
				continue;
				IL_92:
				zero.X = x;
				zero.Y = y;
				color = textColor;
				goto IL_A6;
			}
		}

		public static Vector2 DrawBorderString(SpriteBatch sb, string text, Vector2 pos, Color color, float scale = 1f, float anchorx = 0f, float anchory = 0f, int stringLimit = -1)
		{
			if (stringLimit != -1 && text.Length > stringLimit)
			{
				text.Substring(0, stringLimit);
			}
			SpriteFont fontMouseText = Main.fontMouseText;
			for (int i = -1; i < 2; i++)
			{
				for (int j = -1; j < 2; j++)
				{
					sb.DrawString(fontMouseText, text, pos + new Vector2((float)i, (float)j), Color.Black, 0f, new Vector2(anchorx, anchory) * fontMouseText.MeasureString(text), scale, SpriteEffects.None, 0f);
				}
			}
			sb.DrawString(fontMouseText, text, pos, color, 0f, new Vector2(anchorx, anchory) * fontMouseText.MeasureString(text), scale, SpriteEffects.None, 0f);
			return fontMouseText.MeasureString(text) * scale;
		}

		public static Vector2 DrawBorderStringBig(SpriteBatch sb, string text, Vector2 pos, Color color, float scale = 1f, float anchorx = 0f, float anchory = 0f, int stringLimit = -1)
		{
			if (stringLimit != -1 && text.Length > stringLimit)
			{
				text.Substring(0, stringLimit);
			}
			SpriteFont fontDeathText = Main.fontDeathText;
			for (int i = -1; i < 2; i++)
			{
				for (int j = -1; j < 2; j++)
				{
					sb.DrawString(fontDeathText, text, pos + new Vector2((float)i, (float)j), Color.Black, 0f, new Vector2(anchorx, anchory) * fontDeathText.MeasureString(text), scale, SpriteEffects.None, 0f);
				}
			}
			sb.DrawString(fontDeathText, text, pos, color, 0f, new Vector2(anchorx, anchory) * fontDeathText.MeasureString(text), scale, SpriteEffects.None, 0f);
			return fontDeathText.MeasureString(text) * scale;
		}

		public static void DrawInvBG(SpriteBatch sb, Rectangle R, Color c = default(Color))
		{
			Utils.DrawInvBG(sb, R.X, R.Y, R.Width, R.Height, c);
		}

		public static void DrawInvBG(SpriteBatch sb, float x, float y, float w, float h, Color c = default(Color))
		{
			Utils.DrawInvBG(sb, (int)x, (int)y, (int)w, (int)h, c);
		}

		public static void DrawInvBG(SpriteBatch sb, int x, int y, int w, int h, Color c = default(Color))
		{
			if (c == default(Color))
			{
				c = new Color(63, 65, 151, 255) * 0.785f;
			}
			Texture2D inventoryBack13Texture = Main.inventoryBack13Texture;
			if (w < 20)
			{
				w = 20;
			}
			if (h < 20)
			{
				h = 20;
			}
			sb.Draw(inventoryBack13Texture, new Rectangle(x, y, 10, 10), new Rectangle?(new Rectangle(0, 0, 10, 10)), c);
			sb.Draw(inventoryBack13Texture, new Rectangle(x + 10, y, w - 20, 10), new Rectangle?(new Rectangle(10, 0, 10, 10)), c);
			sb.Draw(inventoryBack13Texture, new Rectangle(x + w - 10, y, 10, 10), new Rectangle?(new Rectangle(inventoryBack13Texture.Width - 10, 0, 10, 10)), c);
			sb.Draw(inventoryBack13Texture, new Rectangle(x, y + 10, 10, h - 20), new Rectangle?(new Rectangle(0, 10, 10, 10)), c);
			sb.Draw(inventoryBack13Texture, new Rectangle(x + 10, y + 10, w - 20, h - 20), new Rectangle?(new Rectangle(10, 10, 10, 10)), c);
			sb.Draw(inventoryBack13Texture, new Rectangle(x + w - 10, y + 10, 10, h - 20), new Rectangle?(new Rectangle(inventoryBack13Texture.Width - 10, 10, 10, 10)), c);
			sb.Draw(inventoryBack13Texture, new Rectangle(x, y + h - 10, 10, 10), new Rectangle?(new Rectangle(0, inventoryBack13Texture.Height - 10, 10, 10)), c);
			sb.Draw(inventoryBack13Texture, new Rectangle(x + 10, y + h - 10, w - 20, 10), new Rectangle?(new Rectangle(10, inventoryBack13Texture.Height - 10, 10, 10)), c);
			sb.Draw(inventoryBack13Texture, new Rectangle(x + w - 10, y + h - 10, 10, 10), new Rectangle?(new Rectangle(inventoryBack13Texture.Width - 10, inventoryBack13Texture.Height - 10, 10, 10)), c);
		}

		public static void DrawSettingsPanel(SpriteBatch spriteBatch, Vector2 position, float width, Color color)
		{
			Utils.DrawPanel(Main.settingsPanelTexture, 2, 0, spriteBatch, position, width, color);
		}

		public static void DrawSettings2Panel(SpriteBatch spriteBatch, Vector2 position, float width, Color color)
		{
			Utils.DrawPanel(Main.settingsPanelTexture, 2, 0, spriteBatch, position, width, color);
		}

		public static void DrawPanel(Texture2D texture, int edgeWidth, int edgeShove, SpriteBatch spriteBatch, Vector2 position, float width, Color color)
		{
			spriteBatch.Draw(texture, position, new Rectangle?(new Rectangle(0, 0, edgeWidth, texture.Height)), color);
			spriteBatch.Draw(texture, new Vector2(position.X + (float)edgeWidth, position.Y), new Rectangle?(new Rectangle(edgeWidth + edgeShove, 0, texture.Width - (edgeWidth + edgeShove) * 2, texture.Height)), color, 0f, Vector2.Zero, new Vector2((width - (float)(edgeWidth * 2)) / (float)(texture.Width - (edgeWidth + edgeShove) * 2), 1f), SpriteEffects.None, 0f);
			spriteBatch.Draw(texture, new Vector2(position.X + width - (float)edgeWidth, position.Y), new Rectangle?(new Rectangle(texture.Width - edgeWidth, 0, edgeWidth, texture.Height)), color);
		}

		public static void DrawRectangle(SpriteBatch sb, Vector2 start, Vector2 end, Color colorStart, Color colorEnd, float width)
		{
			Utils.DrawLine(sb, start, new Vector2(start.X, end.Y), colorStart, colorEnd, width);
			Utils.DrawLine(sb, start, new Vector2(end.X, start.Y), colorStart, colorEnd, width);
			Utils.DrawLine(sb, end, new Vector2(start.X, end.Y), colorStart, colorEnd, width);
			Utils.DrawLine(sb, end, new Vector2(end.X, start.Y), colorStart, colorEnd, width);
		}

		public static void DrawLaser(SpriteBatch sb, Texture2D tex, Vector2 start, Vector2 end, Vector2 scale, Utils.LaserLineFraming framing)
		{
			Vector2 vector = Vector2.Normalize(end - start);
			float num = (end - start).Length();
			float rotation = vector.ToRotation() - 1.57079637f;
			if (vector.HasNaNs())
			{
				return;
			}
			float num2;
			Rectangle rectangle;
			Vector2 origin;
			Color color;
			framing(0, start, num, default(Rectangle), out num2, out rectangle, out origin, out color);
			sb.Draw(tex, start, new Rectangle?(rectangle), color, rotation, rectangle.Size() / 2f, scale, SpriteEffects.None, 0f);
			num -= num2 * scale.Y;
			Vector2 vector2 = start + vector * ((float)rectangle.Height - origin.Y) * scale.Y;
			if (num > 0f)
			{
				float num3 = 0f;
				while (num3 + 1f < num)
				{
					framing(1, vector2, num - num3, rectangle, out num2, out rectangle, out origin, out color);
					if (num - num3 < (float)rectangle.Height)
					{
						num2 *= (num - num3) / (float)rectangle.Height;
						rectangle.Height = (int)(num - num3);
					}
					sb.Draw(tex, vector2, new Rectangle?(rectangle), color, rotation, origin, scale, SpriteEffects.None, 0f);
					num3 += num2 * scale.Y;
					vector2 += vector * num2 * scale.Y;
				}
			}
			framing(2, vector2, num, default(Rectangle), out num2, out rectangle, out origin, out color);
			sb.Draw(tex, vector2, new Rectangle?(rectangle), color, rotation, origin, scale, SpriteEffects.None, 0f);
		}

		public static void DrawLine(SpriteBatch spriteBatch, Point start, Point end, Color color)
		{
			Utils.DrawLine(spriteBatch, new Vector2((float)(start.X << 4), (float)(start.Y << 4)), new Vector2((float)(end.X << 4), (float)(end.Y << 4)), color);
		}

		public static void DrawLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color)
		{
			float num = Vector2.Distance(start, end);
			Vector2 vector = (end - start) / num;
			Vector2 value = start;
			Vector2 screenPosition = Main.screenPosition;
			float rotation = vector.ToRotation();
			for (float num2 = 0f; num2 <= num; num2 += 4f)
			{
				float num3 = num2 / num;
				spriteBatch.Draw(Main.blackTileTexture, value - screenPosition, null, new Color(new Vector4(num3, num3, num3, 1f) * color.ToVector4()), rotation, Vector2.Zero, 0.25f, SpriteEffects.None, 0f);
				value = start + num2 * vector;
			}
		}

		public static void DrawLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color colorStart, Color colorEnd, float width)
		{
			float num = Vector2.Distance(start, end);
			Vector2 vector = (end - start) / num;
			Vector2 value = start;
			Vector2 screenPosition = Main.screenPosition;
			float rotation = vector.ToRotation();
			float scale = width / 16f;
			for (float num2 = 0f; num2 <= num; num2 += width)
			{
				float amount = num2 / num;
				spriteBatch.Draw(Main.blackTileTexture, value - screenPosition, null, Color.Lerp(colorStart, colorEnd, amount), rotation, Vector2.Zero, scale, SpriteEffects.None, 0f);
				value = start + num2 * vector;
			}
		}

		public static void DrawRect(SpriteBatch spriteBatch, Rectangle rect, Color color)
		{
			Utils.DrawRect(spriteBatch, new Point(rect.X, rect.Y), new Point(rect.X + rect.Width, rect.Y + rect.Height), color);
		}

		public static void DrawRect(SpriteBatch spriteBatch, Point start, Point end, Color color)
		{
			Utils.DrawRect(spriteBatch, new Vector2((float)(start.X << 4), (float)(start.Y << 4)), new Vector2((float)((end.X << 4) - 4), (float)((end.Y << 4) - 4)), color);
		}

		public static void DrawRect(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color)
		{
			Utils.DrawLine(spriteBatch, start, new Vector2(start.X, end.Y), color);
			Utils.DrawLine(spriteBatch, start, new Vector2(end.X, start.Y), color);
			Utils.DrawLine(spriteBatch, end, new Vector2(start.X, end.Y), color);
			Utils.DrawLine(spriteBatch, end, new Vector2(end.X, start.Y), color);
		}

		public static void DrawRect(SpriteBatch spriteBatch, Vector2 topLeft, Vector2 topRight, Vector2 bottomRight, Vector2 bottomLeft, Color color)
		{
			Utils.DrawLine(spriteBatch, topLeft, topRight, color);
			Utils.DrawLine(spriteBatch, topRight, bottomRight, color);
			Utils.DrawLine(spriteBatch, bottomRight, bottomLeft, color);
			Utils.DrawLine(spriteBatch, bottomLeft, topLeft, color);
		}

		public static void DrawCursorSingle(SpriteBatch sb, Color color, float rot = float.NaN, float scale = 1f, Vector2 manualPosition = default(Vector2), int cursorSlot = 0, int specialMode = 0)
		{
			bool flag = false;
			bool flag2 = true;
			bool flag3 = true;
			Vector2 zero = Vector2.Zero;
			Vector2 value = new Vector2((float)Main.mouseX, (float)Main.mouseY);
			if (manualPosition != Vector2.Zero)
			{
				value = manualPosition;
			}
			if (float.IsNaN(rot))
			{
				rot = 0f;
			}
			else
			{
				flag = true;
				rot -= 2.3561945f;
			}
			if (cursorSlot == 4 || cursorSlot == 5)
			{
				flag2 = false;
				zero = new Vector2(8f);
				if (flag && specialMode == 0)
				{
					float num = rot;
					if (num < 0f)
					{
						num += 6.28318548f;
					}
					for (float num2 = 0f; num2 < 4f; num2 += 1f)
					{
						if (Math.Abs(num - 1.57079637f * num2) <= 0.7853982f)
						{
							rot = 1.57079637f * num2;
							break;
						}
					}
				}
			}
			Vector2 value2 = Vector2.One;
			if ((Main.ThickMouse && cursorSlot == 0) || cursorSlot == 1)
			{
				value2 = Main.DrawThickCursor(cursorSlot == 1);
			}
			if (flag2)
			{
				sb.Draw(Main.cursorTextures[cursorSlot], value + value2 + Vector2.One, null, color.MultiplyRGB(new Color(0.2f, 0.2f, 0.2f, 0.5f)), rot, zero, scale * 1.1f, SpriteEffects.None, 0f);
			}
			if (flag3)
			{
				sb.Draw(Main.cursorTextures[cursorSlot], value + value2, null, color, rot, zero, scale, SpriteEffects.None, 0f);
			}
		}
	}
}
