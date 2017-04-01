using Microsoft.Xna.Framework;
using System;
using Terraria.GameContent;
using Terraria.GameContent.Shaders;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.Utilities;
using Terraria.ModLoader;

namespace Terraria
{
	public class Gore
	{
		public ModGore modGore
		{
			get;
			internal set;
		}
		internal int realType = 0;

		public static int goreTime = 600;
		public Vector2 position;
		public Vector2 velocity;
		public float rotation;
		public float scale;
		public int alpha;
		public int type;
		public float light;
		public bool active;
		public bool sticky = true;
		public int timeLeft = Gore.goreTime;
		public bool behindTiles;
		public byte frame;
		public byte frameCounter;
		public byte numFrames = 1;

		public void Update()
		{
			if (Main.netMode == 2)
			{
				return;
			}
			if (this.active)
			{
				if (this.modGore != null && !this.modGore.Update(this))
				{
					return;
				}
				ModGore.SetupUpdateType(this);
				bool flag = this.type >= 1024 && this.type <= 1026;
				if (this.type >= 276 && this.type <= 282)
				{
					this.velocity.X = this.velocity.X * 0.98f;
					this.velocity.Y = this.velocity.Y * 0.98f;
					if (this.velocity.Y < this.scale)
					{
						this.velocity.Y = this.velocity.Y + 0.05f;
					}
					if ((double)this.velocity.Y > 0.1)
					{
						if (this.velocity.X > 0f)
						{
							this.rotation += 0.01f;
						}
						else
						{
							this.rotation -= 0.01f;
						}
					}
				}
				if (this.type >= 570 && this.type <= 572)
				{
					this.scale -= 0.001f;
					if ((double)this.scale <= 0.01)
					{
						this.scale = 0.01f;
						Gore.goreTime = 0;
					}
					this.sticky = false;
					this.rotation = this.velocity.X * 0.1f;
				}
				else if ((this.type >= 706 && this.type <= 717) || this.type == 943)
				{
					if ((double)this.position.Y < Main.worldSurface * 16.0 + 8.0)
					{
						this.alpha = 0;
					}
					else
					{
						this.alpha = 100;
					}
					int num = 4;
					this.frameCounter += 1;
					if (this.frame <= 4)
					{
						int num2 = (int)(this.position.X / 16f);
						int num3 = (int)(this.position.Y / 16f) - 1;
						if (WorldGen.InWorld(num2, num3, 0) && !Main.tile[num2, num3].active())
						{
							this.active = false;
						}
						if (this.frame == 0)
						{
							num = 24 + Main.rand.Next(256);
						}
						if (this.frame == 1)
						{
							num = 24 + Main.rand.Next(256);
						}
						if (this.frame == 2)
						{
							num = 24 + Main.rand.Next(256);
						}
						if (this.frame == 3)
						{
							num = 24 + Main.rand.Next(96);
						}
						if (this.frame == 5)
						{
							num = 16 + Main.rand.Next(64);
						}
						if (this.type == 716)
						{
							num *= 2;
						}
						if (this.type == 717)
						{
							num *= 4;
						}
						if (this.type == 943 && this.frame < 6)
						{
							num = 4;
						}
						if ((int)this.frameCounter >= num)
						{
							this.frameCounter = 0;
							this.frame += 1;
							if (this.frame == 5)
							{
								int num4 = Gore.NewGore(this.position, this.velocity, this.type, 1f);
								Main.gore[num4].frame = 9;
								Main.gore[num4].velocity *= 0f;
							}
							if (this.type == 943 && this.frame > 4)
							{
								if (Main.rand.Next(2) == 0)
								{
									Gore gore = Main.gore[Gore.NewGore(this.position, this.velocity, this.type, this.scale)];
									gore.frameCounter = 0;
									gore.frame = 7;
									gore.velocity = Vector2.UnitY * 1f;
								}
								if (Main.rand.Next(2) == 0)
								{
									Gore gore = Main.gore[Gore.NewGore(this.position, this.velocity, this.type, this.scale)];
									gore.frameCounter = 0;
									gore.frame = 7;
									gore.velocity = Vector2.UnitY * 2f;
								}
							}
						}
					}
					else if (this.frame <= 6)
					{
						num = 8;
						if (this.type == 716)
						{
							num *= 2;
						}
						if (this.type == 717)
						{
							num *= 3;
						}
						if ((int)this.frameCounter >= num)
						{
							this.frameCounter = 0;
							this.frame += 1;
							if (this.frame == 7)
							{
								this.active = false;
							}
						}
					}
					else if (this.frame <= 9)
					{
						num = 6;
						if (this.type == 716)
						{
							num = (int)((double)num * 1.5);
							this.velocity.Y = this.velocity.Y + 0.175f;
						}
						else if (this.type == 717)
						{
							num *= 2;
							this.velocity.Y = this.velocity.Y + 0.15f;
						}
						else if (this.type == 943)
						{
							num = (int)((double)num * 1.5);
							this.velocity.Y = this.velocity.Y + 0.2f;
						}
						else
						{
							this.velocity.Y = this.velocity.Y + 0.2f;
						}
						if ((double)this.velocity.Y < 0.5)
						{
							this.velocity.Y = 0.5f;
						}
						if (this.velocity.Y > 12f)
						{
							this.velocity.Y = 12f;
						}
						if ((int)this.frameCounter >= num)
						{
							this.frameCounter = 0;
							this.frame += 1;
						}
						if (this.frame > 9)
						{
							this.frame = 7;
						}
					}
					else
					{
						if (this.type == 716)
						{
							num *= 2;
						}
						else if (this.type == 717)
						{
							num *= 6;
						}
						this.velocity.Y = this.velocity.Y + 0.1f;
						if ((int)this.frameCounter >= num)
						{
							this.frameCounter = 0;
							this.frame += 1;
						}
						this.velocity *= 0f;
						if (this.frame > 14)
						{
							this.active = false;
						}
					}
				}
				else if (this.type == 11 || this.type == 12 || this.type == 13 || this.type == 61 || this.type == 62 || this.type == 63 || this.type == 99 || this.type == 220 || this.type == 221 || this.type == 222 || (this.type >= 375 && this.type <= 377) || (this.type >= 435 && this.type <= 437) || (this.type >= 861 && this.type <= 862))
				{
					this.velocity.Y = this.velocity.Y * 0.98f;
					this.velocity.X = this.velocity.X * 0.98f;
					this.scale -= 0.007f;
					if ((double)this.scale < 0.1)
					{
						this.scale = 0.1f;
						this.alpha = 255;
					}
				}
				else if (this.type == 16 || this.type == 17)
				{
					this.velocity.Y = this.velocity.Y * 0.98f;
					this.velocity.X = this.velocity.X * 0.98f;
					this.scale -= 0.01f;
					if ((double)this.scale < 0.1)
					{
						this.scale = 0.1f;
						this.alpha = 255;
					}
				}
				else if (this.type == 331)
				{
					this.alpha += 5;
					this.velocity.Y = this.velocity.Y * 0.95f;
					this.velocity.X = this.velocity.X * 0.95f;
					this.rotation = this.velocity.X * 0.1f;
				}
				else if (GoreID.Sets.SpecialAI[this.type] == 3)
				{
					if ((this.frameCounter += 1) >= 8 && this.velocity.Y > 0.2f)
					{
						this.frameCounter = 0;
						int num5 = (int)(this.frame / 4);
						if ((int)(this.frame += 1) >= 4 + num5 * 4)
						{
							this.frame = (byte)(num5 * 4);
						}
					}
				}
				else if (GoreID.Sets.SpecialAI[this.type] != 1 && GoreID.Sets.SpecialAI[this.type] != 2)
				{
					if (this.type >= 907 && this.type <= 909)
					{
						this.rotation = 0f;
						this.velocity.X = this.velocity.X * 0.98f;
						if (this.velocity.Y > 0f && this.velocity.Y < 0.001f)
						{
							this.velocity.Y = -0.5f + Main.rand.NextFloat() * -3f;
						}
						if (this.velocity.Y > -1f)
						{
							this.velocity.Y = this.velocity.Y - 0.1f;
						}
						if (this.scale < 1f)
						{
							this.scale += 0.1f;
						}
						if ((this.frameCounter += 1) >= 8)
						{
							this.frameCounter = 0;
							if ((this.frame += 1) >= 3)
							{
								this.frame = 0;
							}
						}
					}
					else if (this.type < 411 || this.type > 430)
					{
						this.velocity.Y = this.velocity.Y + 0.2f;
					}
				}
				this.rotation += this.velocity.X * 0.1f;
				if (this.type >= 580 && this.type <= 582)
				{
					this.rotation = 0f;
					this.velocity.X = this.velocity.X * 0.95f;
				}
				if (GoreID.Sets.SpecialAI[this.type] == 2)
				{
					if (this.timeLeft < 60)
					{
						this.alpha += Main.rand.Next(1, 7);
					}
					else if (this.alpha > 100)
					{
						this.alpha -= Main.rand.Next(1, 4);
					}
					if (this.alpha < 0)
					{
						this.alpha = 0;
					}
					if (this.alpha > 255)
					{
						this.timeLeft = 0;
					}
					this.velocity.X = (this.velocity.X * 50f + Main.windSpeed * 2f + (float)Main.rand.Next(-10, 11) * 0.1f) / 51f;
					float num6 = 0f;
					if (this.velocity.X < 0f)
					{
						num6 = this.velocity.X * 0.2f;
					}
					this.velocity.Y = (this.velocity.Y * 50f + -0.35f + num6 + (float)Main.rand.Next(-10, 11) * 0.2f) / 51f;
					this.rotation = this.velocity.X * 0.6f;
					float num7 = -1f;
					if (Main.goreLoaded[this.type])
					{
						Rectangle rectangle = new Rectangle((int)this.position.X, (int)this.position.Y, (int)((float)Main.goreTexture[this.type].Width * this.scale), (int)((float)Main.goreTexture[this.type].Height * this.scale));
						for (int i = 0; i < 255; i++)
						{
							if (Main.player[i].active && !Main.player[i].dead)
							{
								Rectangle value = new Rectangle((int)Main.player[i].position.X, (int)Main.player[i].position.Y, Main.player[i].width, Main.player[i].height);
								if (rectangle.Intersects(value))
								{
									this.timeLeft = 0;
									num7 = Main.player[i].velocity.Length();
									break;
								}
							}
						}
					}
					if (this.timeLeft > 0)
					{
						if (Main.rand.Next(2) == 0)
						{
							this.timeLeft--;
						}
						if (Main.rand.Next(50) == 0)
						{
							this.timeLeft -= 5;
						}
						if (Main.rand.Next(100) == 0)
						{
							this.timeLeft -= 10;
						}
					}
					else
					{
						this.alpha = 255;
						if (Main.goreLoaded[this.type] && num7 != -1f)
						{
							float num8 = (float)Main.goreTexture[this.type].Width * this.scale * 0.8f;
							float x = this.position.X;
							float y = this.position.Y;
							float num9 = (float)Main.goreTexture[this.type].Width * this.scale;
							float num10 = (float)Main.goreTexture[this.type].Height * this.scale;
							int num11 = 31;
							int num12 = 0;
							while ((float)num12 < num8)
							{
								int num13 = Dust.NewDust(new Vector2(x, y), (int)num9, (int)num10, num11, 0f, 0f, 0, default(Color), 1f);
								Main.dust[num13].velocity *= (1f + num7) / 3f;
								Main.dust[num13].noGravity = true;
								Main.dust[num13].alpha = 100;
								Main.dust[num13].scale = this.scale;
								num12++;
							}
						}
					}
				}
				if (this.type >= 411 && this.type <= 430)
				{
					this.alpha = 50;
					this.velocity.X = (this.velocity.X * 50f + Main.windSpeed * 2f + (float)Main.rand.Next(-10, 11) * 0.1f) / 51f;
					this.velocity.Y = (this.velocity.Y * 50f + -0.25f + (float)Main.rand.Next(-10, 11) * 0.2f) / 51f;
					this.rotation = this.velocity.X * 0.3f;
					if (Main.goreLoaded[this.type])
					{
						Rectangle rectangle2 = new Rectangle((int)this.position.X, (int)this.position.Y, (int)((float)Main.goreTexture[this.type].Width * this.scale), (int)((float)Main.goreTexture[this.type].Height * this.scale));
						for (int j = 0; j < 255; j++)
						{
							if (Main.player[j].active && !Main.player[j].dead)
							{
								Rectangle value2 = new Rectangle((int)Main.player[j].position.X, (int)Main.player[j].position.Y, Main.player[j].width, Main.player[j].height);
								if (rectangle2.Intersects(value2))
								{
									this.timeLeft = 0;
								}
							}
						}
						if (Collision.SolidCollision(this.position, (int)((float)Main.goreTexture[this.type].Width * this.scale), (int)((float)Main.goreTexture[this.type].Height * this.scale)))
						{
							this.timeLeft = 0;
						}
					}
					if (this.timeLeft > 0)
					{
						if (Main.rand.Next(2) == 0)
						{
							this.timeLeft--;
						}
						if (Main.rand.Next(50) == 0)
						{
							this.timeLeft -= 5;
						}
						if (Main.rand.Next(100) == 0)
						{
							this.timeLeft -= 10;
						}
					}
					else
					{
						this.alpha = 255;
						if (Main.goreLoaded[this.type])
						{
							float num14 = (float)Main.goreTexture[this.type].Width * this.scale * 0.8f;
							float x2 = this.position.X;
							float y2 = this.position.Y;
							float num15 = (float)Main.goreTexture[this.type].Width * this.scale;
							float num16 = (float)Main.goreTexture[this.type].Height * this.scale;
							int num17 = 176;
							if (this.type >= 416 && this.type <= 420)
							{
								num17 = 177;
							}
							if (this.type >= 421 && this.type <= 425)
							{
								num17 = 178;
							}
							if (this.type >= 426 && this.type <= 430)
							{
								num17 = 179;
							}
							int num18 = 0;
							while ((float)num18 < num14)
							{
								int num19 = Dust.NewDust(new Vector2(x2, y2), (int)num15, (int)num16, num17, 0f, 0f, 0, default(Color), 1f);
								Main.dust[num19].noGravity = true;
								Main.dust[num19].alpha = 100;
								Main.dust[num19].scale = this.scale;
								num18++;
							}
						}
					}
				}
				else if (GoreID.Sets.SpecialAI[this.type] != 3 && GoreID.Sets.SpecialAI[this.type] != 1)
				{
					if ((this.type >= 706 && this.type <= 717) || this.type == 943)
					{
						if (this.type == 716)
						{
							float num20 = 0.6f;
							if (this.frame == 0)
							{
								num20 *= 0.1f;
							}
							else if (this.frame == 1)
							{
								num20 *= 0.2f;
							}
							else if (this.frame == 2)
							{
								num20 *= 0.3f;
							}
							else if (this.frame == 3)
							{
								num20 *= 0.4f;
							}
							else if (this.frame == 4)
							{
								num20 *= 0.5f;
							}
							else if (this.frame == 5)
							{
								num20 *= 0.4f;
							}
							else if (this.frame == 6)
							{
								num20 *= 0.2f;
							}
							else if (this.frame <= 9)
							{
								num20 *= 0.5f;
							}
							else if (this.frame == 10)
							{
								num20 *= 0.5f;
							}
							else if (this.frame == 11)
							{
								num20 *= 0.4f;
							}
							else if (this.frame == 12)
							{
								num20 *= 0.3f;
							}
							else if (this.frame == 13)
							{
								num20 *= 0.2f;
							}
							else if (this.frame == 14)
							{
								num20 *= 0.1f;
							}
							else
							{
								num20 = 0f;
							}
							float r = 1f * num20;
							float g = 0.5f * num20;
							float b = 0.1f * num20;
							Lighting.AddLight(this.position + new Vector2(8f, 8f), r, g, b);
						}
						Vector2 value3 = this.velocity;
						this.velocity = Collision.TileCollision(this.position, this.velocity, 16, 14, false, false, 1);
						if (this.velocity != value3)
						{
							if (this.frame < 10)
							{
								this.frame = 10;
								this.frameCounter = 0;
								if (this.type != 716 && this.type != 717 && this.type != 943)
								{
									Main.PlaySound(39, (int)this.position.X + 8, (int)this.position.Y + 8, Main.rand.Next(2), 1f, 0f);
								}
							}
						}
						else if (Collision.WetCollision(this.position + this.velocity, 16, 14))
						{
							if (this.frame < 10)
							{
								this.frame = 10;
								this.frameCounter = 0;
								if (this.type != 716 && this.type != 717 && this.type != 943)
								{
									Main.PlaySound(39, (int)this.position.X + 8, (int)this.position.Y + 8, 2, 1f, 0f);
								}
								((WaterShaderData)Filters.Scene["WaterDistortion"].GetShader()).QueueRipple(this.position + new Vector2(8f, 8f), 1f, RippleShape.Square, 0f);
							}
							int num21 = (int)(this.position.X + 8f) / 16;
							int num22 = (int)(this.position.Y + 14f) / 16;
							if (Main.tile[num21, num22] != null && Main.tile[num21, num22].liquid > 0)
							{
								this.velocity *= 0f;
								this.position.Y = (float)(num22 * 16 - (int)(Main.tile[num21, num22].liquid / 16));
							}
						}
					}
					else if (this.sticky)
					{
						int num23 = 32;
						if (Main.goreLoaded[this.type])
						{
							num23 = Main.goreTexture[this.type].Width;
							if (Main.goreTexture[this.type].Height < num23)
							{
								num23 = Main.goreTexture[this.type].Height;
							}
						}
						if (flag)
						{
							num23 = 4;
						}
						num23 = (int)((float)num23 * 0.9f);
						this.velocity = Collision.TileCollision(this.position, this.velocity, (int)((float)num23 * this.scale), (int)((float)num23 * this.scale), false, false, 1);
						if (this.velocity.Y == 0f)
						{
							if (flag)
							{
								this.velocity.X = this.velocity.X * 0.94f;
							}
							else
							{
								this.velocity.X = this.velocity.X * 0.97f;
							}
							if ((double)this.velocity.X > -0.01 && (double)this.velocity.X < 0.01)
							{
								this.velocity.X = 0f;
							}
						}
						if (this.timeLeft > 0)
						{
							this.timeLeft -= GoreID.Sets.DisappearSpeed[this.type];
						}
						else
						{
							this.alpha += GoreID.Sets.DisappearSpeedAlpha[this.type];
						}
					}
					else
					{
						this.alpha += 2 * GoreID.Sets.DisappearSpeedAlpha[this.type];
					}
				}
				if (this.type >= 907 && this.type <= 909)
				{
					int num24 = 32;
					if (Main.goreLoaded[this.type])
					{
						num24 = Main.goreTexture[this.type].Width;
						if (Main.goreTexture[this.type].Height < num24)
						{
							num24 = Main.goreTexture[this.type].Height;
						}
					}
					num24 = (int)((float)num24 * 0.9f);
					Vector4 vector = Collision.SlopeCollision(this.position, this.velocity, num24, num24, 0f, true);
					this.position.X = vector.X;
					this.position.Y = vector.Y;
					this.velocity.X = vector.Z;
					this.velocity.Y = vector.W;
				}
				if (GoreID.Sets.SpecialAI[this.type] == 1)
				{
					if (this.velocity.Y < 0f)
					{
						Vector2 vector2 = new Vector2(this.velocity.X, 0.6f);
						int num25 = 32;
						if (Main.goreLoaded[this.type])
						{
							num25 = Main.goreTexture[this.type].Width;
							if (Main.goreTexture[this.type].Height < num25)
							{
								num25 = Main.goreTexture[this.type].Height;
							}
						}
						num25 = (int)((float)num25 * 0.9f);
						vector2 = Collision.TileCollision(this.position, vector2, (int)((float)num25 * this.scale), (int)((float)num25 * this.scale), false, false, 1);
						vector2.X *= 0.97f;
						if ((double)vector2.X > -0.01 && (double)vector2.X < 0.01)
						{
							vector2.X = 0f;
						}
						if (this.timeLeft > 0)
						{
							this.timeLeft--;
						}
						else
						{
							this.alpha++;
						}
						this.velocity.X = vector2.X;
					}
					else
					{
						this.velocity.Y = this.velocity.Y + 0.05235988f;
						Vector2 vector3 = new Vector2(Vector2.UnitY.RotatedBy((double)this.velocity.Y, default(Vector2)).X * 2f, Math.Abs(Vector2.UnitY.RotatedBy((double)this.velocity.Y, default(Vector2)).Y) * 3f);
						vector3 *= 2f;
						int num26 = 32;
						if (Main.goreLoaded[this.type])
						{
							num26 = Main.goreTexture[this.type].Width;
							if (Main.goreTexture[this.type].Height < num26)
							{
								num26 = Main.goreTexture[this.type].Height;
							}
						}
						Vector2 value4 = vector3;
						vector3 = Collision.TileCollision(this.position, vector3, (int)((float)num26 * this.scale), (int)((float)num26 * this.scale), false, false, 1);
						if (vector3 != value4)
						{
							this.velocity.Y = -1f;
						}
						this.position += vector3;
						this.rotation = vector3.ToRotation() + 3.14159274f;
						if (this.timeLeft > 0)
						{
							this.timeLeft--;
						}
						else
						{
							this.alpha++;
						}
					}
				}
				else if (GoreID.Sets.SpecialAI[this.type] == 3)
				{
					if (this.velocity.Y < 0f)
					{
						Vector2 vector4 = new Vector2(this.velocity.X, -0.2f);
						int num27 = 8;
						if (Main.goreLoaded[this.type])
						{
							num27 = Main.goreTexture[this.type].Width;
							if (Main.goreTexture[this.type].Height < num27)
							{
								num27 = Main.goreTexture[this.type].Height;
							}
						}
						num27 = (int)((float)num27 * 0.9f);
						vector4 = Collision.TileCollision(this.position, vector4, (int)((float)num27 * this.scale), (int)((float)num27 * this.scale), false, false, 1);
						vector4.X *= 0.94f;
						if ((double)vector4.X > -0.01 && (double)vector4.X < 0.01)
						{
							vector4.X = 0f;
						}
						if (this.timeLeft > 0)
						{
							this.timeLeft -= GoreID.Sets.DisappearSpeed[this.type];
						}
						else
						{
							this.alpha += GoreID.Sets.DisappearSpeedAlpha[this.type];
						}
						this.velocity.X = vector4.X;
					}
					else
					{
						this.velocity.Y = this.velocity.Y + 0.0174532924f;
						Vector2 vector5 = new Vector2(Vector2.UnitY.RotatedBy((double)this.velocity.Y, default(Vector2)).X * 1f, Math.Abs(Vector2.UnitY.RotatedBy((double)this.velocity.Y, default(Vector2)).Y) * 1f);
						int num28 = 8;
						if (Main.goreLoaded[this.type])
						{
							num28 = Main.goreTexture[this.type].Width;
							if (Main.goreTexture[this.type].Height < num28)
							{
								num28 = Main.goreTexture[this.type].Height;
							}
						}
						Vector2 value5 = vector5;
						vector5 = Collision.TileCollision(this.position, vector5, (int)((float)num28 * this.scale), (int)((float)num28 * this.scale), false, false, 1);
						if (vector5 != value5)
						{
							this.velocity.Y = -1f;
						}
						this.position += vector5;
						this.rotation = vector5.ToRotation() + 1.57079637f;
						if (this.timeLeft > 0)
						{
							this.timeLeft -= GoreID.Sets.DisappearSpeed[this.type];
						}
						else
						{
							this.alpha += GoreID.Sets.DisappearSpeedAlpha[this.type];
						}
					}
				}
				else
				{
					this.position += this.velocity;
				}
				if (this.alpha >= 255)
				{
					this.active = false;
				}
				if (this.light > 0f)
				{
					float num29 = this.light * this.scale;
					float num30 = this.light * this.scale;
					float num31 = this.light * this.scale;
					if (this.type == 16)
					{
						num31 *= 0.3f;
						num30 *= 0.8f;
					}
					else if (this.type == 17)
					{
						num30 *= 0.6f;
						num29 *= 0.3f;
					}
					if (Main.goreLoaded[this.type])
					{
						Lighting.AddLight((int)((this.position.X + (float)Main.goreTexture[this.type].Width * this.scale / 2f) / 16f), (int)((this.position.Y + (float)Main.goreTexture[this.type].Height * this.scale / 2f) / 16f), num29, num30, num31);
						return;
					}
					Lighting.AddLight((int)((this.position.X + 32f * this.scale / 2f) / 16f), (int)((this.position.Y + 32f * this.scale / 2f) / 16f), num29, num30, num31);
				}
				ModGore.TakeDownUpdateType(this);
			}
		}

		public static Gore NewGorePerfect(Vector2 Position, Vector2 Velocity, int Type, float Scale = 1f)
		{
			Gore gore = Gore.NewGoreDirect(Position, Velocity, Type, Scale);
			gore.position = Position;
			gore.velocity = Velocity;
			return gore;
		}

		public static Gore NewGoreDirect(Vector2 Position, Vector2 Velocity, int Type, float Scale = 1f)
		{
			return Main.gore[Gore.NewGore(Position, Velocity, Type, Scale)];
		}

		public static int NewGore(Vector2 Position, Vector2 Velocity, int Type, float Scale = 1f)
		{
			if (Main.netMode == 2)
			{
				return 500;
			}
			if (Main.gamePaused)
			{
				return 500;
			}
			if (Main.rand == null)
			{
				Main.rand = new UnifiedRandom();
			}
			int num = 500;
			for (int i = 0; i < 500; i++)
			{
				if (!Main.gore[i].active)
				{
					num = i;
					break;
				}
			}
			if (num == 500)
			{
				return num;
			}
			Main.gore[num].numFrames = 1;
			Main.gore[num].frame = 0;
			Main.gore[num].frameCounter = 0;
			Main.gore[num].behindTiles = false;
			Main.gore[num].light = 0f;
			Main.gore[num].position = Position;
			Main.gore[num].velocity = Velocity;
			Gore expr_C9_cp_0 = Main.gore[num];
			expr_C9_cp_0.velocity.Y = expr_C9_cp_0.velocity.Y - (float)Main.rand.Next(10, 31) * 0.1f;
			Gore expr_F6_cp_0 = Main.gore[num];
			expr_F6_cp_0.velocity.X = expr_F6_cp_0.velocity.X + (float)Main.rand.Next(-20, 21) * 0.1f;
			Main.gore[num].type = Type;
			Main.gore[num].active = true;
			Main.gore[num].alpha = 0;
			Main.gore[num].rotation = 0f;
			Main.gore[num].scale = Scale;
			ModGore.SetupGore(Main.gore[num]);
			if (!ChildSafety.Disabled && ChildSafety.DangerousGore(Type))
			{
				Main.gore[num].type = Main.rand.Next(11, 14);
				Main.gore[num].scale = Main.rand.NextFloat() * 0.5f + 0.5f;
				Main.gore[num].velocity /= 2f;
			}
			if (Gore.goreTime == 0 || Type == 11 || Type == 12 || Type == 13 || Type == 16 || Type == 17 || Type == 61 || Type == 62 || Type == 63 || Type == 99 || Type == 220 || Type == 221 || Type == 222 || Type == 435 || Type == 436 || Type == 437 || (Type >= 861 && Type <= 862))
			{
				Main.gore[num].sticky = false;
			}
			else if (Type >= 375 && Type <= 377)
			{
				Main.gore[num].sticky = false;
				Main.gore[num].alpha = 100;
			}
			else
			{
				Main.gore[num].sticky = true;
				Main.gore[num].timeLeft = Gore.goreTime;
			}
			if ((Type >= 706 && Type <= 717) || Type == 943)
			{
				Main.gore[num].numFrames = 15;
				Main.gore[num].behindTiles = true;
				Main.gore[num].timeLeft = Gore.goreTime * 3;
			}
			if (Type == 16 || Type == 17)
			{
				Main.gore[num].alpha = 100;
				Main.gore[num].scale = 0.7f;
				Main.gore[num].light = 1f;
			}
			if (Type >= 570 && Type <= 572)
			{
				Main.gore[num].velocity = Velocity;
			}
			if (GoreID.Sets.SpecialAI[Type] == 3)
			{
				Main.gore[num].velocity = new Vector2((Main.rand.NextFloat() - 0.5f) * 1f, Main.rand.NextFloat() * 6.28318548f);
				Main.gore[num].numFrames = 8;
				Main.gore[num].frame = (byte)Main.rand.Next(8);
				Main.gore[num].frameCounter = (byte)Main.rand.Next(8);
			}
			if (GoreID.Sets.SpecialAI[Type] == 1)
			{
				Main.gore[num].velocity = new Vector2((Main.rand.NextFloat() - 0.5f) * 3f, Main.rand.NextFloat() * 6.28318548f);
			}
			if (Type >= 411 && Type <= 430 && Main.goreLoaded[Type])
			{
				Main.gore[num].position.X = Position.X - (float)(Main.goreTexture[Type].Width / 2) * Scale;
				Main.gore[num].position.Y = Position.Y - (float)Main.goreTexture[Type].Height * Scale;
				Gore expr_46D_cp_0 = Main.gore[num];
				expr_46D_cp_0.velocity.Y = expr_46D_cp_0.velocity.Y * ((float)Main.rand.Next(90, 150) * 0.01f);
				Gore expr_49D_cp_0 = Main.gore[num];
				expr_49D_cp_0.velocity.X = expr_49D_cp_0.velocity.X * ((float)Main.rand.Next(40, 90) * 0.01f);
				int num2 = Main.rand.Next(4) * 5;
				Main.gore[num].type += num2;
				Main.gore[num].timeLeft = Main.rand.Next(Gore.goreTime / 2, Gore.goreTime * 2);
				Main.gore[num].sticky = true;
				if (Gore.goreTime == 0)
				{
					Main.gore[num].timeLeft = Main.rand.Next(150, 600);
				}
			}
			if (Type >= 907 && Type <= 909)
			{
				Main.gore[num].sticky = true;
				Main.gore[num].numFrames = 3;
				Main.gore[num].frame = (byte)Main.rand.Next(3);
				Main.gore[num].frameCounter = (byte)Main.rand.Next(5);
				Main.gore[num].rotation = 0f;
			}
			if (GoreID.Sets.SpecialAI[Type] == 2)
			{
				Main.gore[num].sticky = false;
				if (Main.goreLoaded[Type])
				{
					Main.gore[num].alpha = 150;
					Main.gore[num].velocity = Velocity;
					Main.gore[num].position.X = Position.X - (float)(Main.goreTexture[Type].Width / 2) * Scale;
					Main.gore[num].position.Y = Position.Y - (float)Main.goreTexture[Type].Height * Scale / 2f;
					Main.gore[num].timeLeft = Main.rand.Next(Gore.goreTime / 2, Gore.goreTime + 1);
				}
			}
			return num;
		}

		public Color GetAlpha(Color newColor)
		{
			if (this.modGore != null)
			{
				Color? modColor = this.modGore.GetAlpha(this, newColor);
				if (modColor.HasValue)
				{
					return modColor.Value;
				}
			}
			float num = (float)(255 - this.alpha) / 255f;
			int r;
			int g;
			int b;
			if (this.type == 16 || this.type == 17)
			{
				r = (int)newColor.R;
				g = (int)newColor.G;
				b = (int)newColor.B;
			}
			else
			{
				if (this.type == 716)
				{
					return new Color(255, 255, 255, 200);
				}
				if (this.type >= 570 && this.type <= 572)
				{
					byte b2 = (byte)(255 - this.alpha);
					return new Color((int)b2, (int)b2, (int)b2, (int)(b2 / 2));
				}
				if (this.type == 331)
				{
					return new Color(255, 255, 255, 50);
				}
				r = (int)((float)newColor.R * num);
				g = (int)((float)newColor.G * num);
				b = (int)((float)newColor.B * num);
			}
			int num2 = (int)newColor.A - this.alpha;
			if (num2 < 0)
			{
				num2 = 0;
			}
			if (num2 > 255)
			{
				num2 = 255;
			}
			return new Color(r, g, b, num2);
		}
	}
}
