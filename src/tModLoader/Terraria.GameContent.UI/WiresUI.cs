using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.GameInput;

namespace Terraria.GameContent.UI
{
	public class WiresUI
	{
		public static class Settings
		{
			[Flags]
			public enum MultiToolMode
			{
				Red = 1,
				Green = 2,
				Blue = 4,
				Yellow = 8,
				Actuator = 16,
				Cutter = 32
			}

			public static WiresUI.Settings.MultiToolMode ToolMode = WiresUI.Settings.MultiToolMode.Red;
			private static int _lastActuatorEnabled = 0;

			public static bool DrawWires
			{
				get
				{
					return Main.player[Main.myPlayer].inventory[Main.player[Main.myPlayer].selectedItem].mech || (Main.player[Main.myPlayer].InfoAccMechShowWires && Main.player[Main.myPlayer].builderAccStatus[8] == 0);
				}
			}

			public static bool HideWires
			{
				get
				{
					return Main.player[Main.myPlayer].inventory[Main.player[Main.myPlayer].selectedItem].type == 3620;
				}
			}

			public static bool DrawToolModeUI
			{
				get
				{
					int type = Main.player[Main.myPlayer].inventory[Main.player[Main.myPlayer].selectedItem].type;
					return type == 3611 || type == 3625;
				}
			}

			public static bool DrawToolAllowActuators
			{
				get
				{
					int type = Main.player[Main.myPlayer].inventory[Main.player[Main.myPlayer].selectedItem].type;
					if (type == 3611)
					{
						WiresUI.Settings._lastActuatorEnabled = 2;
					}
					if (type == 3625)
					{
						WiresUI.Settings._lastActuatorEnabled = 1;
					}
					return WiresUI.Settings._lastActuatorEnabled == 2;
				}
			}
		}

		public class WiresRadial
		{
			public Vector2 position;
			public bool active;
			public bool OnWiresMenu;
			private float _lineOpacity;

			public void Update()
			{
				this.FlowerUpdate();
				this.LineUpdate();
			}

			private void LineUpdate()
			{
				bool value = true;
				float min = 0.75f;
				Player player = Main.player[Main.myPlayer];
				if (!WiresUI.Settings.DrawToolModeUI || Main.drawingPlayerChat)
				{
					value = false;
					min = 0f;
				}
				if (player.dead || Main.mouseItem.type > 0)
				{
					this._lineOpacity = 0f;
					return;
				}
				if (player.showItemIcon && player.showItemIcon2 != 0 && player.showItemIcon2 != 3625)
				{
					this._lineOpacity = 0f;
					return;
				}
				if ((!player.showItemIcon && ((!PlayerInput.UsingGamepad && !WiresUI.Settings.DrawToolAllowActuators) || player.mouseInterface || player.lastMouseInterface)) || Main.ingameOptionsWindow || Main.InGameUI.IsVisible)
				{
					this._lineOpacity = 0f;
					return;
				}
				float num = Utils.Clamp<float>(this._lineOpacity + 0.05f * (float)value.ToDirectionInt(), min, 1f);
				this._lineOpacity += 0.05f * (float)Math.Sign(num - this._lineOpacity);
				if (Math.Abs(this._lineOpacity - num) < 0.05f)
				{
					this._lineOpacity = num;
				}
			}

			private void FlowerUpdate()
			{
				Player player = Main.player[Main.myPlayer];
				if (!WiresUI.Settings.DrawToolModeUI)
				{
					this.active = false;
					return;
				}
				if ((player.mouseInterface || player.lastMouseInterface) && !this.OnWiresMenu)
				{
					this.active = false;
					return;
				}
				if (player.dead || Main.mouseItem.type > 0)
				{
					this.active = false;
					this.OnWiresMenu = false;
					return;
				}
				this.OnWiresMenu = false;
				if (Main.mouseRight && Main.mouseRightRelease && !PlayerInput.LockTileUseButton && player.noThrow == 0 && !Main.HoveringOverAnNPC && player.talkNPC == -1)
				{
					if (this.active)
					{
						this.active = false;
						return;
					}
					if (!Main.SmartInteractShowingGenuine)
					{
						this.active = true;
						this.position = Main.MouseScreen;
						if (PlayerInput.UsingGamepad && Main.SmartCursorEnabled)
						{
							this.position = new Vector2((float)Main.screenWidth, (float)Main.screenHeight) / 2f;
						}
					}
				}
			}

			public void Draw(SpriteBatch spriteBatch)
			{
				this.DrawFlower(spriteBatch);
				this.DrawCursorArea(spriteBatch);
			}

			private void DrawLine(SpriteBatch spriteBatch)
			{
				if (this.active || this._lineOpacity == 0f)
				{
					return;
				}
				Vector2 vector = Main.MouseScreen;
				Vector2 vector2 = new Vector2((float)(Main.screenWidth / 2), (float)(Main.screenHeight - 70));
				if (PlayerInput.UsingGamepad)
				{
					vector = Vector2.Zero;
				}
				Vector2 vector3 = vector - vector2;
				Vector2.Dot(Vector2.Normalize(vector3), Vector2.UnitX);
				Vector2.Dot(Vector2.Normalize(vector3), Vector2.UnitY);
				vector3.ToRotation();
				vector3.Length();
				bool flag = false;
				bool drawToolAllowActuators = WiresUI.Settings.DrawToolAllowActuators;
				for (int i = 0; i < 6; i++)
				{
					if (drawToolAllowActuators || i != 5)
					{
						bool flag2 = WiresUI.Settings.ToolMode.HasFlag((WiresUI.Settings.MultiToolMode)(1 << i));
						if (i == 5)
						{
							flag2 = WiresUI.Settings.ToolMode.HasFlag(WiresUI.Settings.MultiToolMode.Actuator);
						}
						Vector2 vector4 = vector2 + Vector2.UnitX * (45f * ((float)i - 1.5f));
						int num = i;
						if (i == 0)
						{
							num = 3;
						}
						if (i == 3)
						{
							num = 0;
						}
						switch (num)
						{
							case 0:
							case 1:
								vector4 = vector2 + new Vector2((45f + (float)(drawToolAllowActuators ? 15 : 0)) * (float)(2 - num), 0f) * this._lineOpacity;
								break;
							case 2:
							case 3:
								vector4 = vector2 + new Vector2(-(45f + (float)(drawToolAllowActuators ? 15 : 0)) * (float)(num - 1), 0f) * this._lineOpacity;
								break;
							case 4:
								flag2 = false;
								vector4 = vector2 - new Vector2(0f, drawToolAllowActuators ? 22f : 0f) * this._lineOpacity;
								break;
							case 5:
								vector4 = vector2 + new Vector2(0f, 22f) * this._lineOpacity;
								break;
						}
						bool flag3 = false;
						if (!PlayerInput.UsingGamepad)
						{
							flag3 = (Vector2.Distance(vector4, vector) < 19f * this._lineOpacity);
						}
						if (flag)
						{
							flag3 = false;
						}
						if (flag3)
						{
							flag = true;
						}
						Texture2D texture2D = Main.wireUITexture[(WiresUI.Settings.ToolMode.HasFlag(WiresUI.Settings.MultiToolMode.Cutter) ? 8 : 0) + (flag3 ? 1 : 0)];
						Texture2D texture2D2 = null;
						switch (i)
						{
							case 0:
							case 1:
							case 2:
							case 3:
								texture2D2 = Main.wireUITexture[2 + i];
								break;
							case 4:
								texture2D2 = Main.wireUITexture[WiresUI.Settings.ToolMode.HasFlag(WiresUI.Settings.MultiToolMode.Cutter) ? 7 : 6];
								break;
							case 5:
								texture2D2 = Main.wireUITexture[10];
								break;
						}
						Color white = Color.White;
						Color white2 = Color.White;
						if (!flag2 && i != 4)
						{
							if (flag3)
							{
								white2 = new Color(100, 100, 100);
								white2 = new Color(120, 120, 120);
								white = new Color(200, 200, 200);
							}
							else
							{
								white2 = new Color(150, 150, 150);
								white2 = new Color(80, 80, 80);
								white = new Color(100, 100, 100);
							}
						}
						Utils.CenteredRectangle(vector4, new Vector2(40f));
						if (flag3)
						{
							if (Main.mouseLeft && Main.mouseLeftRelease)
							{
								switch (i)
								{
									case 0:
										WiresUI.Settings.ToolMode ^= WiresUI.Settings.MultiToolMode.Red;
										break;
									case 1:
										WiresUI.Settings.ToolMode ^= WiresUI.Settings.MultiToolMode.Green;
										break;
									case 2:
										WiresUI.Settings.ToolMode ^= WiresUI.Settings.MultiToolMode.Blue;
										break;
									case 3:
										WiresUI.Settings.ToolMode ^= WiresUI.Settings.MultiToolMode.Yellow;
										break;
									case 4:
										WiresUI.Settings.ToolMode ^= WiresUI.Settings.MultiToolMode.Cutter;
										break;
									case 5:
										WiresUI.Settings.ToolMode ^= WiresUI.Settings.MultiToolMode.Actuator;
										break;
								}
							}
							if (!Main.mouseLeft || Main.player[Main.myPlayer].mouseInterface)
							{
								Main.player[Main.myPlayer].mouseInterface = true;
							}
							this.OnWiresMenu = true;
						}
						spriteBatch.Draw(texture2D, vector4, null, white * this._lineOpacity, 0f, texture2D.Size() / 2f, this._lineOpacity, SpriteEffects.None, 0f);
						spriteBatch.Draw(texture2D2, vector4, null, white2 * this._lineOpacity, 0f, texture2D2.Size() / 2f, this._lineOpacity, SpriteEffects.None, 0f);
					}
				}
				if (Main.mouseLeft && Main.mouseLeftRelease && !flag)
				{
					this.active = false;
				}
			}

			private void DrawFlower(SpriteBatch spriteBatch)
			{
				if (!this.active)
				{
					return;
				}
				Vector2 vector = Main.MouseScreen;
				Vector2 vector2 = this.position;
				if (PlayerInput.UsingGamepad && Main.SmartCursorEnabled)
				{
					if (PlayerInput.GamepadThumbstickRight != Vector2.Zero)
					{
						vector = this.position + PlayerInput.GamepadThumbstickRight * 40f;
					}
					else if (PlayerInput.GamepadThumbstickLeft != Vector2.Zero)
					{
						vector = this.position + PlayerInput.GamepadThumbstickLeft * 40f;
					}
					else
					{
						vector = this.position;
					}
				}
				Vector2 vector3 = vector - vector2;
				Vector2.Dot(Vector2.Normalize(vector3), Vector2.UnitX);
				Vector2.Dot(Vector2.Normalize(vector3), Vector2.UnitY);
				float num = vector3.ToRotation();
				float num2 = vector3.Length();
				bool flag = false;
				bool drawToolAllowActuators = WiresUI.Settings.DrawToolAllowActuators;
				float num3 = (float)(4 + drawToolAllowActuators.ToInt());
				float num4 = drawToolAllowActuators ? 11f : -0.5f;
				for (int i = 0; i < 6; i++)
				{
					if (drawToolAllowActuators || i != 5)
					{
						bool flag2 = WiresUI.Settings.ToolMode.HasFlag((WiresUI.Settings.MultiToolMode)(1 << i));
						if (i == 5)
						{
							flag2 = WiresUI.Settings.ToolMode.HasFlag(WiresUI.Settings.MultiToolMode.Actuator);
						}
						Vector2 vector4 = vector2 + Vector2.UnitX * (45f * ((float)i - 1.5f));
						switch (i)
						{
							case 0:
							case 1:
							case 2:
							case 3:
								{
									float num5 = (float)i;
									if (i == 0)
									{
										num5 = 3f;
									}
									if (i == 3)
									{
										num5 = 0f;
									}
									vector4 = vector2 + Vector2.UnitX.RotatedBy((double)(num5 * 6.28318548f / num3 - 3.14159274f / num4), default(Vector2)) * 45f;
									break;
								}
							case 4:
								flag2 = false;
								vector4 = vector2;
								break;
							case 5:
								vector4 = vector2 + Vector2.UnitX.RotatedBy((double)((float)(i - 1) * 6.28318548f / num3 - 3.14159274f / num4), default(Vector2)) * 45f;
								break;
						}
						bool flag3 = false;
						if (i == 4)
						{
							flag3 = (num2 < 20f);
						}
						switch (i)
						{
							case 0:
							case 1:
							case 2:
							case 3:
							case 5:
								{
									float curAngle = (vector4 - vector2).ToRotation();
									float value = curAngle.AngleTowards(num, 6.28318548f / (num3 * 2f)) - num;
									if (num2 >= 20f && Math.Abs(value) < 0.01f)
									{
										flag3 = true;
									}
									break;
								}
							case 4:
								flag3 = (num2 < 20f);
								break;
						}
						if (!PlayerInput.UsingGamepad)
						{
							flag3 = (Vector2.Distance(vector4, vector) < 19f);
						}
						if (flag)
						{
							flag3 = false;
						}
						if (flag3)
						{
							flag = true;
						}
						Texture2D texture2D = Main.wireUITexture[(WiresUI.Settings.ToolMode.HasFlag(WiresUI.Settings.MultiToolMode.Cutter) ? 8 : 0) + (flag3 ? 1 : 0)];
						Texture2D texture2D2 = null;
						switch (i)
						{
							case 0:
							case 1:
							case 2:
							case 3:
								texture2D2 = Main.wireUITexture[2 + i];
								break;
							case 4:
								texture2D2 = Main.wireUITexture[WiresUI.Settings.ToolMode.HasFlag(WiresUI.Settings.MultiToolMode.Cutter) ? 7 : 6];
								break;
							case 5:
								texture2D2 = Main.wireUITexture[10];
								break;
						}
						Color white = Color.White;
						Color white2 = Color.White;
						if (!flag2 && i != 4)
						{
							if (flag3)
							{
								white2 = new Color(100, 100, 100);
								white2 = new Color(120, 120, 120);
								white = new Color(200, 200, 200);
							}
							else
							{
								white2 = new Color(150, 150, 150);
								white2 = new Color(80, 80, 80);
								white = new Color(100, 100, 100);
							}
						}
						Utils.CenteredRectangle(vector4, new Vector2(40f));
						if (flag3)
						{
							if (Main.mouseLeft && Main.mouseLeftRelease)
							{
								switch (i)
								{
									case 0:
										WiresUI.Settings.ToolMode ^= WiresUI.Settings.MultiToolMode.Red;
										break;
									case 1:
										WiresUI.Settings.ToolMode ^= WiresUI.Settings.MultiToolMode.Green;
										break;
									case 2:
										WiresUI.Settings.ToolMode ^= WiresUI.Settings.MultiToolMode.Blue;
										break;
									case 3:
										WiresUI.Settings.ToolMode ^= WiresUI.Settings.MultiToolMode.Yellow;
										break;
									case 4:
										WiresUI.Settings.ToolMode ^= WiresUI.Settings.MultiToolMode.Cutter;
										break;
									case 5:
										WiresUI.Settings.ToolMode ^= WiresUI.Settings.MultiToolMode.Actuator;
										break;
								}
							}
							Main.player[Main.myPlayer].mouseInterface = true;
							this.OnWiresMenu = true;
						}
						spriteBatch.Draw(texture2D, vector4, null, white, 0f, texture2D.Size() / 2f, 1f, SpriteEffects.None, 0f);
						spriteBatch.Draw(texture2D2, vector4, null, white2, 0f, texture2D2.Size() / 2f, 1f, SpriteEffects.None, 0f);
					}
				}
				if (Main.mouseLeft && Main.mouseLeftRelease && !flag)
				{
					this.active = false;
				}
			}

			private void DrawCursorArea(SpriteBatch spriteBatch)
			{
				if (this.active || this._lineOpacity == 0f)
				{
					return;
				}
				Vector2 mouseScreen = Main.MouseScreen;
				Vector2 value = mouseScreen + new Vector2((float)(10 - 9 * PlayerInput.UsingGamepad.ToInt()), 25f);
				Color value2 = new Color(50, 50, 50);
				bool drawToolAllowActuators = WiresUI.Settings.DrawToolAllowActuators;
				if (!drawToolAllowActuators)
				{
					if (!PlayerInput.UsingGamepad)
					{
						value += new Vector2(-20f, 10f);
					}
					else
					{
						value += new Vector2(0f, 10f);
					}
				}
				Texture2D builderAccTexture = Main.builderAccTexture;
				Texture2D texture = builderAccTexture;
				Rectangle rectangle = new Rectangle(140, 2, 6, 6);
				Rectangle rectangle2 = new Rectangle(148, 2, 6, 6);
				Rectangle rectangle3 = new Rectangle(128, 0, 10, 10);
				float num = 1f;
				float scale = 1f;
				bool flag = false;
				if (flag && !drawToolAllowActuators)
				{
					num *= Main.cursorScale;
				}
				float num2 = this._lineOpacity;
				if (PlayerInput.UsingGamepad)
				{
					num2 *= Main.GamepadCursorAlpha;
				}
				for (int i = 0; i < 5; i++)
				{
					if (drawToolAllowActuators || i != 4)
					{
						float scale2 = num2;
						Vector2 vec = value + Vector2.UnitX * (45f * ((float)i - 1.5f));
						int num3 = i;
						if (i == 0)
						{
							num3 = 3;
						}
						if (i == 1)
						{
							num3 = 2;
						}
						if (i == 2)
						{
							num3 = 1;
						}
						if (i == 3)
						{
							num3 = 0;
						}
						if (i == 4)
						{
							num3 = 5;
						}
						int num4 = num3;
						if (num4 == 2)
						{
							num4 = 1;
						}
						else if (num4 == 1)
						{
							num4 = 2;
						}
						bool flag2 = WiresUI.Settings.ToolMode.HasFlag((WiresUI.Settings.MultiToolMode)(1 << num4));
						if (num4 == 5)
						{
							flag2 = WiresUI.Settings.ToolMode.HasFlag(WiresUI.Settings.MultiToolMode.Actuator);
						}
						Color color = Color.HotPink;
						switch (num3)
						{
							case 0:
								color = new Color(253, 58, 61);
								break;
							case 1:
								color = new Color(83, 180, 253);
								break;
							case 2:
								color = new Color(83, 253, 153);
								break;
							case 3:
								color = new Color(253, 254, 83);
								break;
							case 5:
								color = Color.WhiteSmoke;
								break;
						}
						if (!flag2)
						{
							color = Color.Lerp(color, Color.Black, 0.65f);
						}
						if (flag)
						{
							if (drawToolAllowActuators)
							{
								switch (num3)
								{
									case 0:
										vec = value + new Vector2(-12f, 0f) * num;
										break;
									case 1:
										vec = value + new Vector2(-6f, 12f) * num;
										break;
									case 2:
										vec = value + new Vector2(6f, 12f) * num;
										break;
									case 3:
										vec = value + new Vector2(12f, 0f) * num;
										break;
									case 5:
										vec = value + new Vector2(0f, 0f) * num;
										break;
								}
							}
							else
							{
								vec = value + new Vector2((float)(12 * (num3 + 1)), (float)(12 * (3 - num3))) * num;
							}
						}
						else if (drawToolAllowActuators)
						{
							switch (num3)
							{
								case 0:
									vec = value + new Vector2(-12f, 0f) * num;
									break;
								case 1:
									vec = value + new Vector2(-6f, 12f) * num;
									break;
								case 2:
									vec = value + new Vector2(6f, 12f) * num;
									break;
								case 3:
									vec = value + new Vector2(12f, 0f) * num;
									break;
								case 5:
									vec = value + new Vector2(0f, 0f) * num;
									break;
							}
						}
						else
						{
							float scaleFactor = 0.7f;
							switch (num3)
							{
								case 0:
									vec = value + new Vector2(0f, -12f) * num * scaleFactor;
									break;
								case 1:
									vec = value + new Vector2(-12f, 0f) * num * scaleFactor;
									break;
								case 2:
									vec = value + new Vector2(0f, 12f) * num * scaleFactor;
									break;
								case 3:
									vec = value + new Vector2(12f, 0f) * num * scaleFactor;
									break;
							}
						}
						vec = vec.Floor();
						spriteBatch.Draw(texture, vec, new Rectangle?(rectangle3), value2 * scale2, 0f, rectangle3.Size() / 2f, scale, SpriteEffects.None, 0f);
						spriteBatch.Draw(builderAccTexture, vec, new Rectangle?(rectangle), color * scale2, 0f, rectangle.Size() / 2f, scale, SpriteEffects.None, 0f);
						if (WiresUI.Settings.ToolMode.HasFlag(WiresUI.Settings.MultiToolMode.Cutter))
						{
							spriteBatch.Draw(builderAccTexture, vec, new Rectangle?(rectangle2), value2 * scale2, 0f, rectangle2.Size() / 2f, scale, SpriteEffects.None, 0f);
						}
					}
				}
			}
		}

		private static WiresUI.WiresRadial radial = new WiresUI.WiresRadial();

		public static bool Open
		{
			get
			{
				return WiresUI.radial.active;
			}
		}

		public static void HandleWiresUI(SpriteBatch spriteBatch)
		{
			WiresUI.radial.Update();
			WiresUI.radial.Draw(spriteBatch);
		}
	}
}
