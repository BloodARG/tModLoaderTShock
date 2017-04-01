using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.Utilities;

namespace Terraria
{
	public class HitTile
	{
		public class HitTileObject
		{
			public int X;
			public int Y;
			public int damage;
			public int type;
			public int timeToLive;
			public int crackStyle;
			public int animationTimeElapsed;
			public Vector2 animationDirection;

			public HitTileObject()
			{
				this.Clear();
			}

			public void Clear()
			{
				this.X = 0;
				this.Y = 0;
				this.damage = 0;
				this.type = 0;
				this.timeToLive = 0;
				if (HitTile.rand == null)
				{
					HitTile.rand = new UnifiedRandom((int)DateTime.Now.Ticks);
				}
				this.crackStyle = HitTile.rand.Next(4);
				while (this.crackStyle == HitTile.lastCrack)
				{
					this.crackStyle = HitTile.rand.Next(4);
				}
				HitTile.lastCrack = this.crackStyle;
			}
		}

		internal const int UNUSED = 0;
		internal const int TILE = 1;
		internal const int WALL = 2;
		internal const int MAX_HITTILES = 20;
		internal const int TIMETOLIVE = 60;
		private static UnifiedRandom rand;
		private static int lastCrack = -1;
		public HitTile.HitTileObject[] data;
		private int[] order;
		private int bufferLocation;

		public HitTile()
		{
			HitTile.rand = new UnifiedRandom();
			this.data = new HitTile.HitTileObject[21];
			this.order = new int[21];
			for (int i = 0; i <= 20; i++)
			{
				this.data[i] = new HitTile.HitTileObject();
				this.order[i] = i;
			}
			this.bufferLocation = 0;
		}

		public int HitObject(int x, int y, int hitType)
		{
			HitTile.HitTileObject hitTileObject;
			for (int i = 0; i <= 20; i++)
			{
				int num = this.order[i];
				hitTileObject = this.data[num];
				if (hitTileObject.type == hitType)
				{
					if (hitTileObject.X == x && hitTileObject.Y == y)
					{
						return num;
					}
				}
				else if (i != 0 && hitTileObject.type == 0)
				{
					break;
				}
			}
			hitTileObject = this.data[this.bufferLocation];
			hitTileObject.X = x;
			hitTileObject.Y = y;
			hitTileObject.type = hitType;
			return this.bufferLocation;
		}

		public void UpdatePosition(int tileId, int x, int y)
		{
			if (tileId < 0 || tileId > 20)
			{
				return;
			}
			HitTile.HitTileObject hitTileObject = this.data[tileId];
			hitTileObject.X = x;
			hitTileObject.Y = y;
		}

		public int AddDamage(int tileId, int damageAmount, bool updateAmount = true)
		{
			if (tileId < 0 || tileId > 20)
			{
				return 0;
			}
			if (tileId == this.bufferLocation && damageAmount == 0)
			{
				return 0;
			}
			HitTile.HitTileObject hitTileObject = this.data[tileId];
			if (!updateAmount)
			{
				return hitTileObject.damage + damageAmount;
			}
			hitTileObject.damage += damageAmount;
			hitTileObject.timeToLive = 60;
			hitTileObject.animationTimeElapsed = 0;
			hitTileObject.animationDirection = (Main.rand.NextFloat() * 6.28318548f).ToRotationVector2() * 2f;
			if (tileId != this.bufferLocation)
			{
				for (int i = 0; i <= 20 + 1; i++)
				{
					if (i == 20 + 1 || this.order[i] == tileId)
					{
						while (i > 1)
						{
							int num = this.order[i - 1];
							this.order[i - 1] = this.order[i];
							this.order[i] = num;
							i--;
						}
						this.order[1] = tileId;
						goto IL_11A;
					}
				}
			}
			this.bufferLocation = this.order[20];
			this.data[this.bufferLocation].Clear();
			for (int i = 20; i > 0; i--)
			{
				this.order[i] = this.order[i - 1];
			}
			this.order[0] = this.bufferLocation;
			IL_11A:
			return hitTileObject.damage;
		}

		public void Clear(int tileId)
		{
			if (tileId < 0 || tileId > 20)
			{
				return;
			}
			this.data[tileId].Clear();
			for (int i = 0; i < 20 + 1; i++)
			{
				if (i == 20 || this.order[i] == tileId)
				{
					while (i < 20)
					{
						this.order[i] = this.order[i + 1];
						i++;
					}
					this.order[20] = tileId;
					return;
				}
			}
		}

		public void Prune()
		{
			bool flag = false;
			for (int i = 0; i <= 20; i++)
			{
				HitTile.HitTileObject hitTileObject = this.data[i];
				if (hitTileObject.type != 0)
				{
					Tile tile = Main.tile[hitTileObject.X, hitTileObject.Y];
					if (hitTileObject.timeToLive <= 1)
					{
						hitTileObject.Clear();
						flag = true;
					}
					else
					{
						hitTileObject.timeToLive--;
						if ((double)hitTileObject.timeToLive < 12.0)
						{
							hitTileObject.damage -= 10;
						}
						else if ((double)hitTileObject.timeToLive < 24.0)
						{
							hitTileObject.damage -= 7;
						}
						else if ((double)hitTileObject.timeToLive < 36.0)
						{
							hitTileObject.damage -= 5;
						}
						else if ((double)hitTileObject.timeToLive < 48.0)
						{
							hitTileObject.damage -= 2;
						}
						if (hitTileObject.damage < 0)
						{
							hitTileObject.Clear();
							flag = true;
						}
						else if (hitTileObject.type == 1)
						{
							if (!tile.active())
							{
								hitTileObject.Clear();
								flag = true;
							}
						}
						else if (tile.wall == 0)
						{
							hitTileObject.Clear();
							flag = true;
						}
					}
				}
			}
			if (!flag)
			{
				return;
			}
			int num = 1;
			while (flag)
			{
				flag = false;
				for (int j = num; j < 20; j++)
				{
					if (this.data[this.order[j]].type == 0 && this.data[this.order[j + 1]].type != 0)
					{
						int num2 = this.order[j];
						this.order[j] = this.order[j + 1];
						this.order[j + 1] = num2;
						flag = true;
					}
				}
			}
		}

		public void DrawFreshAnimations(SpriteBatch spriteBatch)
		{
			for (int i = 0; i < this.data.Length; i++)
			{
				this.data[i].animationTimeElapsed++;
			}
			if (!Main.SettingsEnabled_MinersWobble)
			{
				return;
			}
			int num = 1;
			Vector2 zero = new Vector2((float)Main.offScreenRange);
			if (Main.drawToScreen)
			{
				zero = Vector2.Zero;
			}
			zero = Vector2.Zero;
			for (int j = 0; j < this.data.Length; j++)
			{
				if (this.data[j].type == num)
				{
					int damage = this.data[j].damage;
					if (damage >= 20)
					{
						int x = this.data[j].X;
						int y = this.data[j].Y;
						if (WorldGen.InWorld(x, y, 0))
						{
							bool flag = Main.tile[x, y] != null;
							if (flag && num == 1)
							{
								flag = (flag && Main.tile[x, y].active() && Main.tileSolid[(int)Main.tile[x, y].type]);
							}
							if (flag && num == 2)
							{
								flag = (flag && Main.tile[x, y].wall != 0);
							}
							if (flag)
							{
								bool flag2 = false;
								bool flag3 = false;
								if (Main.tile[x, y].type == 10)
								{
									flag2 = false;
								}
								else if (Main.tileSolid[(int)Main.tile[x, y].type] && !Main.tileSolidTop[(int)Main.tile[x, y].type])
								{
									flag2 = true;
								}
								else if (Main.tile[x, y].type == 5)
								{
									flag3 = true;
									int num2 = (int)(Main.tile[x, y].frameX / 22);
									int num3 = (int)(Main.tile[x, y].frameY / 22);
									if (num3 < 9)
									{
										flag2 = (((num2 != 1 && num2 != 2) || num3 < 6 || num3 > 8) && (num2 != 3 || num3 > 2) && (num2 != 4 || num3 < 3 || num3 > 5) && (num2 != 5 || num3 < 6 || num3 > 8));
									}
								}
								else if (Main.tile[x, y].type == 72)
								{
									flag3 = true;
									if (Main.tile[x, y].frameX <= 34)
									{
										flag2 = true;
									}
								}
								if (flag2 && Main.tile[x, y].slope() == 0 && !Main.tile[x, y].halfBrick())
								{
									int num4 = 0;
									if (damage >= 80)
									{
										num4 = 3;
									}
									else if (damage >= 60)
									{
										num4 = 2;
									}
									else if (damage >= 40)
									{
										num4 = 1;
									}
									else if (damage >= 20)
									{
										num4 = 0;
									}
									Rectangle value = new Rectangle(this.data[j].crackStyle * 18, num4 * 18, 16, 16);
									value.Inflate(-2, -2);
									if (flag3)
									{
										value.X = (4 + this.data[j].crackStyle / 2) * 18;
									}
									int animationTimeElapsed = this.data[j].animationTimeElapsed;
									if ((float)animationTimeElapsed < 10f)
									{
										float num5 = (float)animationTimeElapsed / 10f;
										Color color = Lighting.GetColor(x, y);
										float rotation = 0f;
										Vector2 zero2 = Vector2.Zero;
										float num6 = num5;
										float num7 = 0.5f;
										float num8 = num6 % num7;
										num8 *= 1f / num7;
										if ((int)(num6 / num7) % 2 == 1)
										{
											num8 = 1f - num8;
										}
										float scaleFactor = num8 * 0.45f + 1f;
										Tile tileSafely = Framing.GetTileSafely(x, y);
										Tile tile = tileSafely;
										Texture2D texture;
										if (Main.canDrawColorTile(tileSafely.type, (int)tileSafely.color()))
										{
											texture = Main.tileAltTexture[(int)tileSafely.type, (int)tileSafely.color()];
										}
										else
										{
											texture = Main.tileTexture[(int)tileSafely.type];
										}
										Vector2 vector = new Vector2(8f);
										Vector2 value2 = new Vector2(1f);
										scaleFactor = num8 * 0.2f + 1f;
										float num9 = 1f - num8;
										num9 = 1f;
										color *= num9 * num9 * 0.8f;
										Vector2 scale = scaleFactor * value2;
										Vector2 position = (new Vector2((float)(x * 16 - (int)Main.screenPosition.X), (float)(y * 16 - (int)Main.screenPosition.Y)) + zero + vector + zero2).Floor();
										spriteBatch.Draw(texture, position, new Rectangle?(new Rectangle((int)tile.frameX, (int)tile.frameY, 16, 16)), color, rotation, vector, scale, SpriteEffects.None, 0f);
										color.A = 180;
										spriteBatch.Draw(Main.tileCrackTexture, position, new Rectangle?(value), color, rotation, vector, scale, SpriteEffects.None, 0f);
									}
								}
							}
						}
					}
				}
			}
		}
	}
}
