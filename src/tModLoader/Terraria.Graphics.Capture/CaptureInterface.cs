using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Terraria.GameContent.UI;
using Terraria.GameInput;
using Terraria.UI.Chat;

namespace Terraria.Graphics.Capture
{
	public class CaptureInterface
	{
		public static class Settings
		{
			public static bool PackImage = true;
			public static bool IncludeEntities = true;
			public static bool TransparentBackground = false;
			public static int BiomeChoice = 0;
			public static int ScreenAnchor = 0;
			public static Color MarkedAreaColor = new Color(0.8f, 0.8f, 0.8f, 0f) * 0.3f;
		}

		private abstract class CaptureInterfaceMode
		{
			public bool Selected;

			public abstract void Update();

			public abstract void Draw(SpriteBatch sb);

			public abstract void ToggleActive(bool tickedOn);

			public abstract bool UsingMap();
		}

		private class ModeEdgeSelection : CaptureInterface.CaptureInterfaceMode
		{
			public override void Update()
			{
				if (!this.Selected)
				{
					return;
				}
				Vector2 mouse = new Vector2((float)Main.mouseX, (float)Main.mouseY);
				this.EdgePlacement(mouse);
			}

			public override void Draw(SpriteBatch sb)
			{
				if (!this.Selected)
				{
					return;
				}
				this.DrawMarkedArea(sb);
				this.DrawCursors(sb);
			}

			public override void ToggleActive(bool tickedOn)
			{
			}

			public override bool UsingMap()
			{
				return true;
			}

			private void EdgePlacement(Vector2 mouse)
			{
				if (CaptureInterface.JustActivated)
				{
					return;
				}
				Point point;
				if (!Main.mapFullscreen)
				{
					if (Main.mouseLeft)
					{
						CaptureInterface.EdgeAPinned = true;
						CaptureInterface.EdgeA = Main.MouseWorld.ToTileCoordinates();
					}
					if (Main.mouseRight)
					{
						CaptureInterface.EdgeBPinned = true;
						CaptureInterface.EdgeB = Main.MouseWorld.ToTileCoordinates();
					}
				}
				else if (CaptureInterface.GetMapCoords((int)mouse.X, (int)mouse.Y, 0, out point))
				{
					if (Main.mouseLeft)
					{
						CaptureInterface.EdgeAPinned = true;
						CaptureInterface.EdgeA = point;
					}
					if (Main.mouseRight)
					{
						CaptureInterface.EdgeBPinned = true;
						CaptureInterface.EdgeB = point;
					}
				}
				CaptureInterface.ConstraintPoints();
			}

			private void DrawMarkedArea(SpriteBatch sb)
			{
				if (!CaptureInterface.EdgeAPinned || !CaptureInterface.EdgeBPinned)
				{
					return;
				}
				int num = Math.Min(CaptureInterface.EdgeA.X, CaptureInterface.EdgeB.X);
				int num2 = Math.Min(CaptureInterface.EdgeA.Y, CaptureInterface.EdgeB.Y);
				int num3 = Math.Abs(CaptureInterface.EdgeA.X - CaptureInterface.EdgeB.X);
				int num4 = Math.Abs(CaptureInterface.EdgeA.Y - CaptureInterface.EdgeB.Y);
				if (!Main.mapFullscreen)
				{
					Rectangle rectangle = Main.ReverseGravitySupport(new Rectangle(num * 16, num2 * 16, (num3 + 1) * 16, (num4 + 1) * 16));
					Rectangle rectangle2 = Main.ReverseGravitySupport(new Rectangle((int)Main.screenPosition.X, (int)Main.screenPosition.Y, Main.screenWidth + 1, Main.screenHeight + 1));
					Rectangle destinationRectangle;
					Rectangle.Intersect(ref rectangle2, ref rectangle, out destinationRectangle);
					if (destinationRectangle.Width == 0 || destinationRectangle.Height == 0)
					{
						return;
					}
					destinationRectangle.Offset(-rectangle2.X, -rectangle2.Y);
					sb.Draw(Main.magicPixel, destinationRectangle, CaptureInterface.Settings.MarkedAreaColor);
					for (int i = 0; i < 2; i++)
					{
						sb.Draw(Main.magicPixel, new Rectangle(destinationRectangle.X, destinationRectangle.Y + ((i == 1) ? destinationRectangle.Height : -2), destinationRectangle.Width, 2), Color.White);
						sb.Draw(Main.magicPixel, new Rectangle(destinationRectangle.X + ((i == 1) ? destinationRectangle.Width : -2), destinationRectangle.Y, 2, destinationRectangle.Height), Color.White);
					}
					return;
				}
				else
				{
					Point point;
					CaptureInterface.GetMapCoords(num, num2, 1, out point);
					Point point2;
					CaptureInterface.GetMapCoords(num + num3 + 1, num2 + num4 + 1, 1, out point2);
					Rectangle rectangle3 = new Rectangle(point.X, point.Y, point2.X - point.X, point2.Y - point.Y);
					Rectangle rectangle4 = new Rectangle(0, 0, Main.screenWidth + 1, Main.screenHeight + 1);
					Rectangle destinationRectangle2;
					Rectangle.Intersect(ref rectangle4, ref rectangle3, out destinationRectangle2);
					if (destinationRectangle2.Width == 0 || destinationRectangle2.Height == 0)
					{
						return;
					}
					destinationRectangle2.Offset(-rectangle4.X, -rectangle4.Y);
					sb.Draw(Main.magicPixel, destinationRectangle2, CaptureInterface.Settings.MarkedAreaColor);
					for (int j = 0; j < 2; j++)
					{
						sb.Draw(Main.magicPixel, new Rectangle(destinationRectangle2.X, destinationRectangle2.Y + ((j == 1) ? destinationRectangle2.Height : -2), destinationRectangle2.Width, 2), Color.White);
						sb.Draw(Main.magicPixel, new Rectangle(destinationRectangle2.X + ((j == 1) ? destinationRectangle2.Width : -2), destinationRectangle2.Y, 2, destinationRectangle2.Height), Color.White);
					}
					return;
				}
			}

			private void DrawCursors(SpriteBatch sb)
			{
				float num = 1f / Main.cursorScale;
				float num2 = 0.8f / num;
				Vector2 vector = Main.screenPosition + new Vector2(30f);
				Vector2 vector2 = vector + new Vector2((float)Main.screenWidth, (float)Main.screenHeight) - new Vector2(60f);
				if (Main.mapFullscreen)
				{
					vector -= Main.screenPosition;
					vector2 -= Main.screenPosition;
				}
				Vector3 vector3 = Main.rgbToHsl(Main.cursorColor);
				Color color = Main.hslToRgb((vector3.X + 0.33f) % 1f, vector3.Y, vector3.Z);
				Color color2 = Main.hslToRgb((vector3.X - 0.33f) % 1f, vector3.Y, vector3.Z);
				color2 = (color = Color.White);
				bool flag = Main.player[Main.myPlayer].gravDir == -1f;
				if (!CaptureInterface.EdgeAPinned)
				{
					Utils.DrawCursorSingle(sb, color, 3.926991f, Main.cursorScale * num * num2, new Vector2((float)Main.mouseX - 5f + 12f, (float)Main.mouseY + 2.5f + 12f), 4, 0);
				}
				else
				{
					int specialMode = 0;
					float num3 = 0f;
					Vector2 vector4 = Vector2.Zero;
					if (!Main.mapFullscreen)
					{
						Vector2 vector5 = CaptureInterface.EdgeA.ToVector2() * 16f;
						if (!CaptureInterface.EdgeBPinned)
						{
							specialMode = 1;
							vector5 += Vector2.One * 8f;
							num3 = (-vector5 + Main.ReverseGravitySupport(new Vector2((float)Main.mouseX, (float)Main.mouseY), 0f) + Main.screenPosition).ToRotation();
							if (flag)
							{
								num3 = -num3;
							}
							vector4 = Vector2.Clamp(vector5, vector, vector2);
							if (vector4 != vector5)
							{
								num3 = (vector5 - vector4).ToRotation();
							}
						}
						else
						{
							Vector2 value = new Vector2((float)((CaptureInterface.EdgeA.X > CaptureInterface.EdgeB.X).ToInt() * 16), (float)((CaptureInterface.EdgeA.Y > CaptureInterface.EdgeB.Y).ToInt() * 16));
							vector5 += value;
							vector4 = Vector2.Clamp(vector5, vector, vector2);
							num3 = (CaptureInterface.EdgeB.ToVector2() * 16f + new Vector2(16f) - value - vector4).ToRotation();
							if (vector4 != vector5)
							{
								num3 = (vector5 - vector4).ToRotation();
								specialMode = 1;
							}
							if (flag)
							{
								num3 *= -1f;
							}
						}
						Utils.DrawCursorSingle(sb, color, num3 - 1.57079637f, Main.cursorScale * num, Main.ReverseGravitySupport(vector4 - Main.screenPosition, 0f), 4, specialMode);
					}
					else
					{
						Point edgeA = CaptureInterface.EdgeA;
						if (CaptureInterface.EdgeBPinned)
						{
							int num4 = (CaptureInterface.EdgeA.X > CaptureInterface.EdgeB.X).ToInt();
							int num5 = (CaptureInterface.EdgeA.Y > CaptureInterface.EdgeB.Y).ToInt();
							edgeA.X += num4;
							edgeA.Y += num5;
							CaptureInterface.GetMapCoords(edgeA.X, edgeA.Y, 1, out edgeA);
							Point edgeB = CaptureInterface.EdgeB;
							edgeB.X += 1 - num4;
							edgeB.Y += 1 - num5;
							CaptureInterface.GetMapCoords(edgeB.X, edgeB.Y, 1, out edgeB);
							vector4 = edgeA.ToVector2();
							vector4 = Vector2.Clamp(vector4, vector, vector2);
							num3 = (edgeB.ToVector2() - vector4).ToRotation();
						}
						else
						{
							CaptureInterface.GetMapCoords(edgeA.X, edgeA.Y, 1, out edgeA);
						}
						Utils.DrawCursorSingle(sb, color, num3 - 1.57079637f, Main.cursorScale * num, edgeA.ToVector2(), 4, 0);
					}
				}
				if (!CaptureInterface.EdgeBPinned)
				{
					Utils.DrawCursorSingle(sb, color2, 0.7853982f, Main.cursorScale * num * num2, new Vector2((float)Main.mouseX + 2.5f + 12f, (float)Main.mouseY - 5f + 12f), 5, 0);
					return;
				}
				int specialMode2 = 0;
				float num6 = 0f;
				Vector2 vector6 = Vector2.Zero;
				if (!Main.mapFullscreen)
				{
					Vector2 vector7 = CaptureInterface.EdgeB.ToVector2() * 16f;
					if (!CaptureInterface.EdgeAPinned)
					{
						specialMode2 = 1;
						vector7 += Vector2.One * 8f;
						num6 = (-vector7 + Main.ReverseGravitySupport(new Vector2((float)Main.mouseX, (float)Main.mouseY), 0f) + Main.screenPosition).ToRotation();
						if (flag)
						{
							num6 = -num6;
						}
						vector6 = Vector2.Clamp(vector7, vector, vector2);
						if (vector6 != vector7)
						{
							num6 = (vector7 - vector6).ToRotation();
						}
					}
					else
					{
						Vector2 value2 = new Vector2((float)((CaptureInterface.EdgeB.X >= CaptureInterface.EdgeA.X).ToInt() * 16), (float)((CaptureInterface.EdgeB.Y >= CaptureInterface.EdgeA.Y).ToInt() * 16));
						vector7 += value2;
						vector6 = Vector2.Clamp(vector7, vector, vector2);
						num6 = (CaptureInterface.EdgeA.ToVector2() * 16f + new Vector2(16f) - value2 - vector6).ToRotation();
						if (vector6 != vector7)
						{
							num6 = (vector7 - vector6).ToRotation();
							specialMode2 = 1;
						}
						if (flag)
						{
							num6 *= -1f;
						}
					}
					Utils.DrawCursorSingle(sb, color2, num6 - 1.57079637f, Main.cursorScale * num, Main.ReverseGravitySupport(vector6 - Main.screenPosition, 0f), 5, specialMode2);
					return;
				}
				Point edgeB2 = CaptureInterface.EdgeB;
				if (CaptureInterface.EdgeAPinned)
				{
					int num7 = (CaptureInterface.EdgeB.X >= CaptureInterface.EdgeA.X).ToInt();
					int num8 = (CaptureInterface.EdgeB.Y >= CaptureInterface.EdgeA.Y).ToInt();
					edgeB2.X += num7;
					edgeB2.Y += num8;
					CaptureInterface.GetMapCoords(edgeB2.X, edgeB2.Y, 1, out edgeB2);
					Point edgeA2 = CaptureInterface.EdgeA;
					edgeA2.X += 1 - num7;
					edgeA2.Y += 1 - num8;
					CaptureInterface.GetMapCoords(edgeA2.X, edgeA2.Y, 1, out edgeA2);
					vector6 = edgeB2.ToVector2();
					vector6 = Vector2.Clamp(vector6, vector, vector2);
					num6 = (edgeA2.ToVector2() - vector6).ToRotation();
				}
				else
				{
					CaptureInterface.GetMapCoords(edgeB2.X, edgeB2.Y, 1, out edgeB2);
				}
				Utils.DrawCursorSingle(sb, color2, num6 - 1.57079637f, Main.cursorScale * num, edgeB2.ToVector2(), 5, 0);
			}
		}

		private class ModeDragBounds : CaptureInterface.CaptureInterfaceMode
		{
			public int currentAim = -1;
			private bool dragging;
			private int caughtEdge = -1;
			private bool inMap;

			public override void Update()
			{
				if (!this.Selected)
				{
					return;
				}
				if (CaptureInterface.JustActivated)
				{
					return;
				}
				Vector2 mouse = new Vector2((float)Main.mouseX, (float)Main.mouseY);
				this.DragBounds(mouse);
			}

			public override void Draw(SpriteBatch sb)
			{
				if (!this.Selected)
				{
					return;
				}
				this.DrawMarkedArea(sb);
			}

			public override void ToggleActive(bool tickedOn)
			{
				if (!tickedOn)
				{
					this.currentAim = -1;
				}
			}

			public override bool UsingMap()
			{
				return this.caughtEdge != -1;
			}

			private void DragBounds(Vector2 mouse)
			{
				if (!CaptureInterface.EdgeAPinned || !CaptureInterface.EdgeBPinned)
				{
					bool flag = false;
					if (Main.mouseLeft)
					{
						flag = true;
					}
					if (flag)
					{
						bool flag2 = true;
						Point point;
						if (!Main.mapFullscreen)
						{
							point = (Main.screenPosition + mouse).ToTileCoordinates();
						}
						else
						{
							flag2 = CaptureInterface.GetMapCoords((int)mouse.X, (int)mouse.Y, 0, out point);
						}
						if (flag2)
						{
							if (!CaptureInterface.EdgeAPinned)
							{
								CaptureInterface.EdgeAPinned = true;
								CaptureInterface.EdgeA = point;
							}
							if (!CaptureInterface.EdgeBPinned)
							{
								CaptureInterface.EdgeBPinned = true;
								CaptureInterface.EdgeB = point;
							}
						}
						this.currentAim = 3;
						this.caughtEdge = 1;
					}
				}
				int num = Math.Min(CaptureInterface.EdgeA.X, CaptureInterface.EdgeB.X);
				int num2 = Math.Min(CaptureInterface.EdgeA.Y, CaptureInterface.EdgeB.Y);
				int num3 = Math.Abs(CaptureInterface.EdgeA.X - CaptureInterface.EdgeB.X);
				int num4 = Math.Abs(CaptureInterface.EdgeA.Y - CaptureInterface.EdgeB.Y);
				bool value = Main.player[Main.myPlayer].gravDir == -1f;
				int num5 = 1 - value.ToInt();
				int num6 = value.ToInt();
				Rectangle rectangle;
				Rectangle rectangle2;
				if (!Main.mapFullscreen)
				{
					rectangle = Main.ReverseGravitySupport(new Rectangle(num * 16, num2 * 16, (num3 + 1) * 16, (num4 + 1) * 16));
					rectangle2 = Main.ReverseGravitySupport(new Rectangle((int)Main.screenPosition.X, (int)Main.screenPosition.Y, Main.screenWidth + 1, Main.screenHeight + 1));
					Rectangle rectangle3;
					Rectangle.Intersect(ref rectangle2, ref rectangle, out rectangle3);
					if (rectangle3.Width == 0 || rectangle3.Height == 0)
					{
						return;
					}
					rectangle3.Offset(-rectangle2.X, -rectangle2.Y);
				}
				else
				{
					Point point2;
					CaptureInterface.GetMapCoords(num, num2, 1, out point2);
					Point point3;
					CaptureInterface.GetMapCoords(num + num3 + 1, num2 + num4 + 1, 1, out point3);
					rectangle = new Rectangle(point2.X, point2.Y, point3.X - point2.X, point3.Y - point2.Y);
					rectangle2 = new Rectangle(0, 0, Main.screenWidth + 1, Main.screenHeight + 1);
					Rectangle rectangle3;
					Rectangle.Intersect(ref rectangle2, ref rectangle, out rectangle3);
					if (rectangle3.Width == 0 || rectangle3.Height == 0)
					{
						return;
					}
					rectangle3.Offset(-rectangle2.X, -rectangle2.Y);
				}
				this.dragging = false;
				if (!Main.mouseLeft)
				{
					this.currentAim = -1;
				}
				if (this.currentAim != -1)
				{
					this.dragging = true;
					Point point4 = default(Point);
					if (!Main.mapFullscreen)
					{
						point4 = Main.MouseWorld.ToTileCoordinates();
					}
					else
					{
						Point point5;
						if (!CaptureInterface.GetMapCoords((int)mouse.X, (int)mouse.Y, 0, out point5))
						{
							return;
						}
						point4 = point5;
					}
					switch (this.currentAim)
					{
						case 0:
						case 1:
							if (this.caughtEdge == 0)
							{
								CaptureInterface.EdgeA.Y = point4.Y;
							}
							if (this.caughtEdge == 1)
							{
								CaptureInterface.EdgeB.Y = point4.Y;
							}
							break;
						case 2:
						case 3:
							if (this.caughtEdge == 0)
							{
								CaptureInterface.EdgeA.X = point4.X;
							}
							if (this.caughtEdge == 1)
							{
								CaptureInterface.EdgeB.X = point4.X;
							}
							break;
					}
				}
				else
				{
					this.caughtEdge = -1;
					Rectangle drawbox = rectangle;
					drawbox.Offset(-rectangle2.X, -rectangle2.Y);
					this.inMap = drawbox.Contains(mouse.ToPoint());
					int i = 0;
					while (i < 4)
					{
						Rectangle bound = this.GetBound(drawbox, i);
						bound.Inflate(8, 8);
						if (bound.Contains(mouse.ToPoint()))
						{
							this.currentAim = i;
							if (i == 0)
							{
								if (CaptureInterface.EdgeA.Y < CaptureInterface.EdgeB.Y)
								{
									this.caughtEdge = num6;
									break;
								}
								this.caughtEdge = num5;
								break;
							}
							else if (i == 1)
							{
								if (CaptureInterface.EdgeA.Y >= CaptureInterface.EdgeB.Y)
								{
									this.caughtEdge = num6;
									break;
								}
								this.caughtEdge = num5;
								break;
							}
							else if (i == 2)
							{
								if (CaptureInterface.EdgeA.X < CaptureInterface.EdgeB.X)
								{
									this.caughtEdge = 0;
									break;
								}
								this.caughtEdge = 1;
								break;
							}
							else
							{
								if (i != 3)
								{
									break;
								}
								if (CaptureInterface.EdgeA.X >= CaptureInterface.EdgeB.X)
								{
									this.caughtEdge = 0;
									break;
								}
								this.caughtEdge = 1;
								break;
							}
						}
						else
						{
							i++;
						}
					}
				}
				CaptureInterface.ConstraintPoints();
			}

			private Rectangle GetBound(Rectangle drawbox, int boundIndex)
			{
				if (boundIndex == 0)
				{
					return new Rectangle(drawbox.X, drawbox.Y - 2, drawbox.Width, 2);
				}
				if (boundIndex == 1)
				{
					return new Rectangle(drawbox.X, drawbox.Y + drawbox.Height, drawbox.Width, 2);
				}
				if (boundIndex == 2)
				{
					return new Rectangle(drawbox.X - 2, drawbox.Y, 2, drawbox.Height);
				}
				if (boundIndex == 3)
				{
					return new Rectangle(drawbox.X + drawbox.Width, drawbox.Y, 2, drawbox.Height);
				}
				return Rectangle.Empty;
			}

			public void DrawMarkedArea(SpriteBatch sb)
			{
				if (!CaptureInterface.EdgeAPinned || !CaptureInterface.EdgeBPinned)
				{
					return;
				}
				int num = Math.Min(CaptureInterface.EdgeA.X, CaptureInterface.EdgeB.X);
				int num2 = Math.Min(CaptureInterface.EdgeA.Y, CaptureInterface.EdgeB.Y);
				int num3 = Math.Abs(CaptureInterface.EdgeA.X - CaptureInterface.EdgeB.X);
				int num4 = Math.Abs(CaptureInterface.EdgeA.Y - CaptureInterface.EdgeB.Y);
				Rectangle destinationRectangle;
				if (!Main.mapFullscreen)
				{
					Rectangle rectangle = Main.ReverseGravitySupport(new Rectangle(num * 16, num2 * 16, (num3 + 1) * 16, (num4 + 1) * 16));
					Rectangle rectangle2 = Main.ReverseGravitySupport(new Rectangle((int)Main.screenPosition.X, (int)Main.screenPosition.Y, Main.screenWidth + 1, Main.screenHeight + 1));
					Rectangle.Intersect(ref rectangle2, ref rectangle, out destinationRectangle);
					if (destinationRectangle.Width == 0 || destinationRectangle.Height == 0)
					{
						return;
					}
					destinationRectangle.Offset(-rectangle2.X, -rectangle2.Y);
				}
				else
				{
					Point point;
					CaptureInterface.GetMapCoords(num, num2, 1, out point);
					Point point2;
					CaptureInterface.GetMapCoords(num + num3 + 1, num2 + num4 + 1, 1, out point2);
					Rectangle rectangle = new Rectangle(point.X, point.Y, point2.X - point.X, point2.Y - point.Y);
					Rectangle rectangle2 = new Rectangle(0, 0, Main.screenWidth + 1, Main.screenHeight + 1);
					Rectangle.Intersect(ref rectangle2, ref rectangle, out destinationRectangle);
					if (destinationRectangle.Width == 0 || destinationRectangle.Height == 0)
					{
						return;
					}
					destinationRectangle.Offset(-rectangle2.X, -rectangle2.Y);
				}
				sb.Draw(Main.magicPixel, destinationRectangle, CaptureInterface.Settings.MarkedAreaColor);
				Rectangle empty = Rectangle.Empty;
				for (int i = 0; i < 2; i++)
				{
					if (this.currentAim != i)
					{
						this.DrawBound(sb, new Rectangle(destinationRectangle.X, destinationRectangle.Y + ((i == 1) ? destinationRectangle.Height : -2), destinationRectangle.Width, 2), 0);
					}
					else
					{
						empty = new Rectangle(destinationRectangle.X, destinationRectangle.Y + ((i == 1) ? destinationRectangle.Height : -2), destinationRectangle.Width, 2);
					}
					if (this.currentAim != i + 2)
					{
						this.DrawBound(sb, new Rectangle(destinationRectangle.X + ((i == 1) ? destinationRectangle.Width : -2), destinationRectangle.Y, 2, destinationRectangle.Height), 0);
					}
					else
					{
						empty = new Rectangle(destinationRectangle.X + ((i == 1) ? destinationRectangle.Width : -2), destinationRectangle.Y, 2, destinationRectangle.Height);
					}
				}
				if (empty != Rectangle.Empty)
				{
					this.DrawBound(sb, empty, 1 + this.dragging.ToInt());
				}
			}

			private void DrawBound(SpriteBatch sb, Rectangle r, int mode)
			{
				if (mode == 0)
				{
					sb.Draw(Main.magicPixel, r, Color.Silver);
					return;
				}
				if (mode == 1)
				{
					Rectangle destinationRectangle = new Rectangle(r.X - 2, r.Y, r.Width + 4, r.Height);
					sb.Draw(Main.magicPixel, destinationRectangle, Color.White);
					destinationRectangle = new Rectangle(r.X, r.Y - 2, r.Width, r.Height + 4);
					sb.Draw(Main.magicPixel, destinationRectangle, Color.White);
					sb.Draw(Main.magicPixel, r, Color.White);
					return;
				}
				if (mode == 2)
				{
					Rectangle destinationRectangle2 = new Rectangle(r.X - 2, r.Y, r.Width + 4, r.Height);
					sb.Draw(Main.magicPixel, destinationRectangle2, Color.Gold);
					destinationRectangle2 = new Rectangle(r.X, r.Y - 2, r.Width, r.Height + 4);
					sb.Draw(Main.magicPixel, destinationRectangle2, Color.Gold);
					sb.Draw(Main.magicPixel, r, Color.Gold);
				}
			}
		}

		private class ModeChangeSettings : CaptureInterface.CaptureInterfaceMode
		{
			private const int ButtonsCount = 7;
			private int hoveredButton = -1;
			private bool inUI;

			private Rectangle GetRect()
			{
				Rectangle result = new Rectangle(0, 0, 224, 170);
				int screenAnchor = CaptureInterface.Settings.ScreenAnchor;
				if (screenAnchor == 0)
				{
					result.X = 227 - result.Width / 2;
					result.Y = 80;
					int num = 0;
					Player player = Main.player[Main.myPlayer];
					while (num < player.buffTime.Length && player.buffTime[num] > 0)
					{
						num++;
					}
					int num2 = num / 11;
					num2 += ((num % 11 >= 3) ? 1 : 0);
					result.Y += 48 * num2;
				}
				return result;
			}

			private void ButtonDraw(int button, ref string key, ref string value)
			{
				switch (button)
				{
					case 0:
						key = Lang.inter[74];
						value = Lang.inter[73 - CaptureInterface.Settings.PackImage.ToInt()];
						return;
					case 1:
						key = Lang.inter[75];
						value = Lang.inter[73 - CaptureInterface.Settings.IncludeEntities.ToInt()];
						return;
					case 2:
						key = Lang.inter[76];
						value = Lang.inter[73 - (!CaptureInterface.Settings.TransparentBackground).ToInt()];
						return;
					case 3:
					case 4:
					case 5:
						break;
					case 6:
						key = "      " + Lang.menu[86];
						value = "";
						break;
					default:
						return;
				}
			}

			private void PressButton(int button)
			{
				switch (button)
				{
					case 0:
						CaptureInterface.Settings.PackImage = !CaptureInterface.Settings.PackImage;
						return;
					case 1:
						CaptureInterface.Settings.IncludeEntities = !CaptureInterface.Settings.IncludeEntities;
						return;
					case 2:
						CaptureInterface.Settings.TransparentBackground = !CaptureInterface.Settings.TransparentBackground;
						return;
					case 3:
					case 4:
					case 5:
						break;
					case 6:
						CaptureInterface.Settings.PackImage = false;
						CaptureInterface.Settings.IncludeEntities = true;
						CaptureInterface.Settings.TransparentBackground = false;
						CaptureInterface.Settings.BiomeChoice = 0;
						break;
					default:
						return;
				}
			}

			private void DrawWaterChoices(SpriteBatch spritebatch, Point start, Point mouse)
			{
				Rectangle r = new Rectangle(0, 0, 20, 20);
				for (int i = 0; i < 2; i++)
				{
					for (int j = 0; j < 5; j++)
					{
						if (i != 1 || j != 3)
						{
							int index = j + i * 5;
							r.X = start.X + 24 * j + 12 * i;
							r.Y = start.Y + 24 * i;
							if (i == 1 && j == 4)
							{
								r.X -= 24;
							}
							int num = 0;
							if (r.Contains(mouse))
							{
								if (Main.mouseLeft && Main.mouseLeftRelease)
								{
									CaptureInterface.Settings.BiomeChoice = this.BiomeWater(index);
								}
								num++;
							}
							if (CaptureInterface.Settings.BiomeChoice == this.BiomeWater(index))
							{
								num += 2;
							}
							Texture2D texture = Main.liquidTexture[this.BiomeWater(index)];
							int x = (int)Main.wFrame * 18;
							Color arg_CF_0 = Color.White;
							float num2 = 1f;
							if (num < 2)
							{
								num2 *= 0.5f;
							}
							if (num % 2 == 1)
							{
								spritebatch.Draw(Main.magicPixel, r.TopLeft(), new Rectangle?(new Rectangle(0, 0, 1, 1)), Color.Gold, 0f, Vector2.Zero, new Vector2(20f), SpriteEffects.None, 0f);
							}
							else
							{
								spritebatch.Draw(Main.magicPixel, r.TopLeft(), new Rectangle?(new Rectangle(0, 0, 1, 1)), Color.White * num2, 0f, Vector2.Zero, new Vector2(20f), SpriteEffects.None, 0f);
							}
							spritebatch.Draw(texture, r.TopLeft() + new Vector2(2f), new Rectangle?(new Rectangle(x, 0, 16, 16)), Color.White * num2);
						}
					}
				}
			}

			private int BiomeWater(int index)
			{
				switch (index)
				{
					case 0:
						return 0;
					case 1:
						return 2;
					case 2:
						return 3;
					case 3:
						return 4;
					case 4:
						return 5;
					case 5:
						return 6;
					case 6:
						return 7;
					case 7:
						return 8;
					case 8:
						return 9;
					case 9:
						return 10;
					default:
						return 0;
				}
			}

			public override void Update()
			{
				if (!this.Selected)
				{
					return;
				}
				if (CaptureInterface.JustActivated)
				{
					return;
				}
				Point value = new Point(Main.mouseX, Main.mouseY);
				this.hoveredButton = -1;
				Rectangle rect = this.GetRect();
				this.inUI = rect.Contains(value);
				rect.Inflate(-20, -20);
				rect.Height = 16;
				int y = rect.Y;
				for (int i = 0; i < 7; i++)
				{
					rect.Y = y + i * 20;
					if (rect.Contains(value))
					{
						this.hoveredButton = i;
						break;
					}
				}
				if (Main.mouseLeft && Main.mouseLeftRelease && this.hoveredButton != -1)
				{
					this.PressButton(this.hoveredButton);
				}
			}

			public override void Draw(SpriteBatch sb)
			{
				if (!this.Selected)
				{
					return;
				}
				((CaptureInterface.ModeDragBounds)CaptureInterface.Modes[1]).currentAim = -1;
				((CaptureInterface.ModeDragBounds)CaptureInterface.Modes[1]).DrawMarkedArea(sb);
				Rectangle rect = this.GetRect();
				Utils.DrawInvBG(sb, rect, new Color(63, 65, 151, 255) * 0.485f);
				for (int i = 0; i < 7; i++)
				{
					string text = "";
					string text2 = "";
					this.ButtonDraw(i, ref text, ref text2);
					Color baseColor = Color.White;
					if (i == this.hoveredButton)
					{
						baseColor = Color.Gold;
					}
					ChatManager.DrawColorCodedStringWithShadow(sb, Main.fontItemStack, text, rect.TopLeft() + new Vector2(20f, (float)(20 + 20 * i)), baseColor, 0f, Vector2.Zero, Vector2.One, -1f, 2f);
					ChatManager.DrawColorCodedStringWithShadow(sb, Main.fontItemStack, text2, rect.TopRight() + new Vector2(-20f, (float)(20 + 20 * i)), baseColor, 0f, Main.fontItemStack.MeasureString(text2) * Vector2.UnitX, Vector2.One, -1f, 2f);
				}
				this.DrawWaterChoices(sb, (rect.TopLeft() + new Vector2((float)(rect.Width / 2 - 58), 90f)).ToPoint(), Main.MouseScreen.ToPoint());
			}

			public override void ToggleActive(bool tickedOn)
			{
				if (tickedOn)
				{
					this.hoveredButton = -1;
				}
			}

			public override bool UsingMap()
			{
				return this.inUI;
			}
		}

		private const Keys KeyToggleActive = Keys.F1;
		private const float CameraMaxFrame = 5f;
		private const float CameraMaxWait = 60f;
		private static Dictionary<int, CaptureInterface.CaptureInterfaceMode> Modes = CaptureInterface.FillModes();
		public bool Active;
		public static bool JustActivated = false;
		private bool KeyToggleActiveHeld;
		public int SelectedMode;
		public int HoveredMode;
		public static bool EdgeAPinned = false;
		public static bool EdgeBPinned = false;
		public static Point EdgeA = default(Point);
		public static Point EdgeB = default(Point);
		public static bool CameraLock = false;
		private static float CameraFrame = 0f;
		private static float CameraWaiting = 0f;
		private static CaptureSettings CameraSettings;

		private static Dictionary<int, CaptureInterface.CaptureInterfaceMode> FillModes()
		{
			return new Dictionary<int, CaptureInterface.CaptureInterfaceMode>
			{
				{
					0,
					new CaptureInterface.ModeEdgeSelection()
				},
				{
					1,
					new CaptureInterface.ModeDragBounds()
				},
				{
					2,
					new CaptureInterface.ModeChangeSettings()
				}
			};
		}

		public static Rectangle GetArea()
		{
			int x = Math.Min(CaptureInterface.EdgeA.X, CaptureInterface.EdgeB.X);
			int y = Math.Min(CaptureInterface.EdgeA.Y, CaptureInterface.EdgeB.Y);
			int num = Math.Abs(CaptureInterface.EdgeA.X - CaptureInterface.EdgeB.X);
			int num2 = Math.Abs(CaptureInterface.EdgeA.Y - CaptureInterface.EdgeB.Y);
			return new Rectangle(x, y, num + 1, num2 + 1);
		}

		public void Update()
		{
			this.UpdateCamera();
			if (CaptureInterface.CameraLock)
			{
				return;
			}
			bool flag = Main.keyState.IsKeyDown(Keys.F1);
			if (flag && !this.KeyToggleActiveHeld && (Main.mouseItem.type == 0 || this.Active) && !Main.CaptureModeDisabled)
			{
				this.ToggleCamera(!this.Active);
			}
			this.KeyToggleActiveHeld = flag;
			if (!this.Active)
			{
				return;
			}
			Main.blockMouse = true;
			if (CaptureInterface.JustActivated && Main.mouseLeftRelease && !Main.mouseLeft)
			{
				CaptureInterface.JustActivated = false;
			}
			Vector2 mouse = new Vector2((float)Main.mouseX, (float)Main.mouseY);
			if (this.UpdateButtons(mouse) && Main.mouseLeft)
			{
				return;
			}
			foreach (KeyValuePair<int, CaptureInterface.CaptureInterfaceMode> current in CaptureInterface.Modes)
			{
				current.Value.Selected = (current.Key == this.SelectedMode);
				current.Value.Update();
			}
		}

		public void Draw(SpriteBatch sb)
		{
			if (!this.Active)
			{
				return;
			}
			foreach (CaptureInterface.CaptureInterfaceMode current in CaptureInterface.Modes.Values)
			{
				current.Draw(sb);
			}
			Main.mouseText = false;
			Main.instance.GUIBarsDraw();
			this.DrawButtons(sb);
			Main.instance.DrawMouseOver();
			Utils.DrawBorderStringBig(sb, Lang.inter[81], new Vector2((float)Main.screenWidth * 0.5f, 100f), Color.White, 1f, 0.5f, 0.5f, -1);
			Utils.DrawCursorSingle(sb, Main.cursorColor, float.NaN, Main.cursorScale, default(Vector2), 0, 0);
			this.DrawCameraLock(sb);
		}

		public void ToggleCamera(bool On = true)
		{
			if (CaptureInterface.CameraLock)
			{
				return;
			}
			bool active = this.Active;
			this.Active = (CaptureInterface.Modes.ContainsKey(this.SelectedMode) && On);
			if (active != this.Active)
			{
				Main.PlaySound(12, -1, -1, 1, 1f, 0f);
			}
			foreach (KeyValuePair<int, CaptureInterface.CaptureInterfaceMode> current in CaptureInterface.Modes)
			{
				current.Value.ToggleActive(this.Active && current.Key == this.SelectedMode);
			}
			if (On && !active)
			{
				CaptureInterface.JustActivated = true;
			}
		}

		private bool UpdateButtons(Vector2 mouse)
		{
			this.HoveredMode = -1;
			bool flag = !Main.graphics.IsFullScreen;
			int num = 9;
			for (int i = 0; i < num; i++)
			{
				Rectangle rectangle = new Rectangle(24 + 46 * i, 24, 42, 42);
				if (rectangle.Contains(mouse.ToPoint()))
				{
					this.HoveredMode = i;
					bool flag2 = Main.mouseLeft && Main.mouseLeftRelease;
					int num2 = 0;
					if (i == num2++ && flag2)
					{
						CaptureInterface.QuickScreenshot();
					}
					if (i == num2++ && flag2 && CaptureInterface.EdgeAPinned && CaptureInterface.EdgeBPinned)
					{
						CaptureInterface.StartCamera(new CaptureSettings
							{
								Area = CaptureInterface.GetArea(),
								Biome = CaptureBiome.Biomes[CaptureInterface.Settings.BiomeChoice],
								CaptureBackground = !CaptureInterface.Settings.TransparentBackground,
								CaptureEntities = CaptureInterface.Settings.IncludeEntities,
								UseScaling = CaptureInterface.Settings.PackImage,
								CaptureMech = WiresUI.Settings.DrawWires
							});
					}
					if (i == num2++ && flag2 && this.SelectedMode != 0)
					{
						this.SelectedMode = 0;
						this.ToggleCamera(true);
					}
					if (i == num2++ && flag2 && this.SelectedMode != 1)
					{
						this.SelectedMode = 1;
						this.ToggleCamera(true);
					}
					if (i == num2++ && flag2)
					{
						CaptureInterface.ResetFocus();
					}
					if (i == num2++ && flag2 && Main.mapEnabled)
					{
						Main.mapFullscreen = !Main.mapFullscreen;
					}
					if (i == num2++ && flag2 && this.SelectedMode != 2)
					{
						this.SelectedMode = 2;
						this.ToggleCamera(true);
					}
					if (i == num2++ && flag2 && flag)
					{
						string fileName = Path.Combine(Main.SavePath, "Captures");
#if LINUX
						Process.Start(new ProcessStartInfo(fileName)
						{
							FileName = "open-folder",
							Arguments = fileName,
							UseShellExecute = true,
							CreateNoWindow = true
						});
#else
						Process.Start(fileName);
#endif
					}
					if (i == num2++ && flag2)
					{
						this.ToggleCamera(false);
						Main.blockMouse = true;
						Main.mouseLeftRelease = false;
					}
					return true;
				}
			}
			return false;
		}

		public static void QuickScreenshot()
		{
			CaptureSettings captureSettings = new CaptureSettings();
			Point point = Main.screenPosition.ToTileCoordinates();
			Point point2 = (Main.screenPosition + new Vector2((float)Main.screenWidth, (float)Main.screenHeight)).ToTileCoordinates();
			captureSettings.Area = new Rectangle(point.X, point.Y, point2.X - point.X + 1, point2.Y - point.Y + 1);
			captureSettings.Biome = CaptureBiome.Biomes[CaptureInterface.Settings.BiomeChoice];
			captureSettings.CaptureBackground = !CaptureInterface.Settings.TransparentBackground;
			captureSettings.CaptureEntities = CaptureInterface.Settings.IncludeEntities;
			captureSettings.UseScaling = CaptureInterface.Settings.PackImage;
			captureSettings.CaptureMech = WiresUI.Settings.DrawWires;
			CaptureInterface.StartCamera(captureSettings);
		}

		private void DrawButtons(SpriteBatch sb)
		{
			new Vector2((float)Main.mouseX, (float)Main.mouseY);
			int num = 9;
			for (int i = 0; i < num; i++)
			{
				Texture2D texture2D = Main.inventoryBackTexture;
				float num2 = 0.8f;
				Vector2 vector = new Vector2((float)(24 + 46 * i), 24f);
				Color color = Main.inventoryBack * 0.8f;
				if (this.SelectedMode == 0 && i == 2)
				{
					texture2D = Main.inventoryBack14Texture;
				}
				else if (this.SelectedMode == 1 && i == 3)
				{
					texture2D = Main.inventoryBack14Texture;
				}
				else if (this.SelectedMode == 2 && i == 6)
				{
					texture2D = Main.inventoryBack14Texture;
				}
				else if (i >= 2 && i <= 3)
				{
					texture2D = Main.inventoryBack2Texture;
				}
				sb.Draw(texture2D, vector, null, color, 0f, default(Vector2), num2, SpriteEffects.None, 0f);
				switch (i)
				{
					case 0:
						texture2D = Main.cameraTexture[7];
						break;
					case 1:
						texture2D = Main.cameraTexture[0];
						break;
					case 2:
					case 3:
					case 4:
						texture2D = Main.cameraTexture[i];
						break;
					case 5:
						texture2D = (Main.mapFullscreen ? Main.mapIconTexture[0] : Main.mapIconTexture[4]);
						break;
					case 6:
						texture2D = Main.cameraTexture[1];
						break;
					case 7:
						texture2D = Main.cameraTexture[6];
						break;
					case 8:
						texture2D = Main.cameraTexture[5];
						break;
				}
				sb.Draw(texture2D, vector + new Vector2(26f) * num2, null, Color.White, 0f, texture2D.Size() / 2f, 1f, SpriteEffects.None, 0f);
				bool flag = false;
				int num3 = i;
				if (num3 != 1)
				{
					switch (num3)
					{
						case 5:
							if (!Main.mapEnabled)
							{
								flag = true;
							}
							break;
						case 7:
							if (Main.graphics.IsFullScreen)
							{
								flag = true;
							}
							break;
					}
				}
				else if (!CaptureInterface.EdgeAPinned || !CaptureInterface.EdgeBPinned)
				{
					flag = true;
				}
				if (flag)
				{
					sb.Draw(Main.cdTexture, vector + new Vector2(26f) * num2, null, Color.White * 0.65f, 0f, Main.cdTexture.Size() / 2f, 1f, SpriteEffects.None, 0f);
				}
			}
			string text = "";
			switch (this.HoveredMode)
			{
				case -1:
					break;
				case 0:
					text = Lang.inter[111];
					break;
				case 1:
					text = Lang.inter[67];
					break;
				case 2:
					text = Lang.inter[69];
					break;
				case 3:
					text = Lang.inter[70];
					break;
				case 4:
					text = Lang.inter[78];
					break;
				case 5:
					text = (Main.mapFullscreen ? Lang.inter[109] : Lang.inter[108]);
					break;
				case 6:
					text = Lang.inter[68];
					break;
				case 7:
					text = Lang.inter[110];
					break;
				case 8:
					text = Lang.inter[71];
					break;
				default:
					text = "???";
					break;
			}
			int hoveredMode = this.HoveredMode;
			if (hoveredMode != 1)
			{
				switch (hoveredMode)
				{
					case 5:
						if (!Main.mapEnabled)
						{
							text = text + "\n" + Lang.inter[114];
						}
						break;
					case 7:
						if (Main.graphics.IsFullScreen)
						{
							text = text + "\n" + Lang.inter[113];
						}
						break;
				}
			}
			else if (!CaptureInterface.EdgeAPinned || !CaptureInterface.EdgeBPinned)
			{
				text = text + "\n" + Lang.inter[112];
			}
			if (text != "")
			{
				Main.instance.MouseText(text, 0, 0);
			}
		}

		private static bool GetMapCoords(int PinX, int PinY, int Goal, out Point result)
		{
			if (!Main.mapFullscreen)
			{
				result = new Point(-1, -1);
				return false;
			}
			int arg_33_0 = Main.maxTilesX / Main.textureMaxWidth;
			int arg_3F_0 = Main.maxTilesY / Main.textureMaxHeight;
			float num = 10f;
			float num2 = 10f;
			float num3 = (float)(Main.maxTilesX - 10);
			float num4 = (float)(Main.maxTilesY - 10);
			float mapFullscreenScale = Main.mapFullscreenScale;
			float num5 = (float)Main.screenWidth / (float)Main.maxTilesX * 0.8f;
			if (Main.mapFullscreenScale < num5)
			{
				Main.mapFullscreenScale = num5;
			}
			if (Main.mapFullscreenScale > 16f)
			{
				Main.mapFullscreenScale = 16f;
			}
			mapFullscreenScale = Main.mapFullscreenScale;
			if (Main.mapFullscreenPos.X < num)
			{
				Main.mapFullscreenPos.X = num;
			}
			if (Main.mapFullscreenPos.X > num3)
			{
				Main.mapFullscreenPos.X = num3;
			}
			if (Main.mapFullscreenPos.Y < num2)
			{
				Main.mapFullscreenPos.Y = num2;
			}
			if (Main.mapFullscreenPos.Y > num4)
			{
				Main.mapFullscreenPos.Y = num4;
			}
			float num6 = Main.mapFullscreenPos.X;
			float num7 = Main.mapFullscreenPos.Y;
			num6 *= mapFullscreenScale;
			num7 *= mapFullscreenScale;
			float num8 = -num6 + (float)(Main.screenWidth / 2);
			float num9 = -num7 + (float)(Main.screenHeight / 2);
			num8 += num * mapFullscreenScale;
			num9 += num2 * mapFullscreenScale;
			float num10 = (float)(Main.maxTilesX / 840);
			num10 *= Main.mapFullscreenScale;
			float num11 = num8;
			float num12 = num9;
			float num13 = (float)Main.mapTexture.Width;
			float num14 = (float)Main.mapTexture.Height;
			if (Main.maxTilesX == 8400)
			{
				num10 *= 0.999f;
				num11 -= 40.6f * num10;
				num12 = num9 - 5f * num10;
				num13 -= 8.045f;
				num13 *= num10;
				num14 += 0.12f;
				num14 *= num10;
				if ((double)num10 < 1.2)
				{
					num14 += 1f;
				}
			}
			else if (Main.maxTilesX == 6400)
			{
				num10 *= 1.09f;
				num11 -= 38.8f * num10;
				num12 = num9 - 3.85f * num10;
				num13 -= 13.6f;
				num13 *= num10;
				num14 -= 6.92f;
				num14 *= num10;
				if ((double)num10 < 1.2)
				{
					num14 += 2f;
				}
			}
			else if (Main.maxTilesX == 6300)
			{
				num10 *= 1.09f;
				num11 -= 39.8f * num10;
				num12 = num9 - 4.08f * num10;
				num13 -= 26.69f;
				num13 *= num10;
				num14 -= 6.92f;
				num14 *= num10;
				if ((double)num10 < 1.2)
				{
					num14 += 2f;
				}
			}
			else if (Main.maxTilesX == 4200)
			{
				num10 *= 0.998f;
				num11 -= 37.3f * num10;
				num12 -= 1.7f * num10;
				num13 -= 16f;
				num13 *= num10;
				num14 -= 8.31f;
				num14 *= num10;
			}
			if (Goal == 0)
			{
				int num15 = (int)((-num8 + (float)PinX) / mapFullscreenScale + num);
				int num16 = (int)((-num9 + (float)PinY) / mapFullscreenScale + num2);
				bool flag = false;
				if ((float)num15 < num)
				{
					flag = true;
				}
				if ((float)num15 >= num3)
				{
					flag = true;
				}
				if ((float)num16 < num2)
				{
					flag = true;
				}
				if ((float)num16 >= num4)
				{
					flag = true;
				}
				if (!flag)
				{
					result = new Point(num15, num16);
					return true;
				}
				result = new Point(-1, -1);
				return false;
			}
			else
			{
				if (Goal == 1)
				{
					Vector2 value = new Vector2(num8, num9);
					Vector2 value2 = new Vector2((float)PinX, (float)PinY) * mapFullscreenScale - new Vector2(10f * mapFullscreenScale);
					result = (value + value2).ToPoint();
					return true;
				}
				result = new Point(-1, -1);
				return false;
			}
		}

		private static void ConstraintPoints()
		{
			int offScreenTiles = Lighting.offScreenTiles;
			if (CaptureInterface.EdgeAPinned)
			{
				CaptureInterface.PointWorldClamp(ref CaptureInterface.EdgeA, offScreenTiles);
			}
			if (CaptureInterface.EdgeBPinned)
			{
				CaptureInterface.PointWorldClamp(ref CaptureInterface.EdgeB, offScreenTiles);
			}
		}

		private static void PointWorldClamp(ref Point point, int fluff)
		{
			if (point.X < fluff)
			{
				point.X = fluff;
			}
			if (point.X > Main.maxTilesX - 1 - fluff)
			{
				point.X = Main.maxTilesX - 1 - fluff;
			}
			if (point.Y < fluff)
			{
				point.Y = fluff;
			}
			if (point.Y > Main.maxTilesY - 1 - fluff)
			{
				point.Y = Main.maxTilesY - 1 - fluff;
			}
		}

		public bool UsingMap()
		{
			return CaptureInterface.CameraLock || CaptureInterface.Modes[this.SelectedMode].UsingMap();
		}

		public static void ResetFocus()
		{
			CaptureInterface.EdgeAPinned = false;
			CaptureInterface.EdgeBPinned = false;
			CaptureInterface.EdgeA = new Point(-1, -1);
			CaptureInterface.EdgeB = new Point(-1, -1);
		}

		public void Scrolling()
		{
			int num = PlayerInput.ScrollWheelDelta / 120;
			num %= 30;
			if (num < 0)
			{
				num += 30;
			}
			int selectedMode = this.SelectedMode;
			this.SelectedMode -= num;
			while (this.SelectedMode < 0)
			{
				this.SelectedMode += 2;
			}
			while (this.SelectedMode > 2)
			{
				this.SelectedMode -= 2;
			}
			if (this.SelectedMode != selectedMode)
			{
				Main.PlaySound(12, -1, -1, 1, 1f, 0f);
			}
		}

		private void UpdateCamera()
		{
			if (CaptureInterface.CameraLock && CaptureInterface.CameraFrame == 4f)
			{
				CaptureManager.Instance.Capture(CaptureInterface.CameraSettings);
			}
			CaptureInterface.CameraFrame += (float)CaptureInterface.CameraLock.ToDirectionInt();
			if (CaptureInterface.CameraFrame < 0f)
			{
				CaptureInterface.CameraFrame = 0f;
			}
			if (CaptureInterface.CameraFrame > 5f)
			{
				CaptureInterface.CameraFrame = 5f;
			}
			if (CaptureInterface.CameraFrame == 5f)
			{
				CaptureInterface.CameraWaiting += 1f;
			}
			if (CaptureInterface.CameraWaiting > 60f)
			{
				CaptureInterface.CameraWaiting = 60f;
			}
		}

		private void DrawCameraLock(SpriteBatch sb)
		{
			if (CaptureInterface.CameraFrame == 0f)
			{
				return;
			}
			sb.Draw(Main.magicPixel, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), new Rectangle?(new Rectangle(0, 0, 1, 1)), Color.Black * (CaptureInterface.CameraFrame / 5f));
			if (CaptureInterface.CameraFrame != 5f)
			{
				return;
			}
			float num = CaptureInterface.CameraWaiting - 60f + 5f;
			if (num <= 0f)
			{
				return;
			}
			num /= 5f;
			float num2 = CaptureManager.Instance.GetProgress() * 100f;
			if (num2 > 100f)
			{
				num2 = 100f;
			}
			string text = num2.ToString("##") + " ";
			string text2 = "/ 100%";
			Vector2 vector = Main.fontDeathText.MeasureString(text);
			Vector2 vector2 = Main.fontDeathText.MeasureString(text2);
			Vector2 value = new Vector2(-vector.X, -vector.Y / 2f);
			Vector2 value2 = new Vector2(0f, -vector2.Y / 2f);
			ChatManager.DrawColorCodedStringWithShadow(sb, Main.fontDeathText, text, new Vector2((float)Main.screenWidth, (float)Main.screenHeight) / 2f + value, Color.White * num, 0f, Vector2.Zero, Vector2.One, -1f, 2f);
			ChatManager.DrawColorCodedStringWithShadow(sb, Main.fontDeathText, text2, new Vector2((float)Main.screenWidth, (float)Main.screenHeight) / 2f + value2, Color.White * num, 0f, Vector2.Zero, Vector2.One, -1f, 2f);
		}

		public static void StartCamera(CaptureSettings settings)
		{
			Main.PlaySound(40, -1, -1, 1, 1f, 0f);
			CaptureInterface.CameraSettings = settings;
			CaptureInterface.CameraLock = true;
			CaptureInterface.CameraWaiting = 0f;
		}

		public static void EndCamera()
		{
			CaptureInterface.CameraLock = false;
		}
	}
}
