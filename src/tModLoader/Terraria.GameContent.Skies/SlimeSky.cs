using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.Graphics;
using Terraria.Graphics.Effects;
using Terraria.Utilities;

namespace Terraria.GameContent.Skies
{
	public class SlimeSky : CustomSky
	{
		private struct Slime
		{
			private const int MAX_FRAMES = 4;
			private const int FRAME_RATE = 6;
			private Texture2D _texture;
			public Vector2 Position;
			public float Depth;
			public int FrameHeight;
			public int FrameWidth;
			public float Speed;
			public bool Active;
			private int _frame;

			public Texture2D Texture
			{
				get
				{
					return this._texture;
				}
				set
				{
					this._texture = value;
					this.FrameWidth = value.Width;
					this.FrameHeight = value.Height / 4;
				}
			}

			public int Frame
			{
				get
				{
					return this._frame;
				}
				set
				{
					this._frame = value % 24;
				}
			}

			public Rectangle GetSourceRectangle()
			{
				return new Rectangle(0, this._frame / 6 * this.FrameHeight, this.FrameWidth, this.FrameHeight);
			}
		}

		private Texture2D[] _textures;
		private SlimeSky.Slime[] _slimes;
		private UnifiedRandom _random = new UnifiedRandom();
		private int _slimesRemaining;
		private bool _isActive;
		private bool _isLeaving;

		public override void OnLoad()
		{
			this._textures = new Texture2D[4];
			for (int i = 0; i < 4; i++)
			{
				this._textures[i] = TextureManager.Load("Images/Misc/Sky_Slime_" + (i + 1));
			}
			this.GenerateSlimes();
		}

		private void GenerateSlimes()
		{
			this._slimes = new SlimeSky.Slime[Main.maxTilesY / 6];
			for (int i = 0; i < this._slimes.Length; i++)
			{
				int num = (int)((double)Main.screenPosition.Y * 0.7 - (double)Main.screenHeight);
				int minValue = (int)((double)num - Main.worldSurface * 16.0);
				this._slimes[i].Position = new Vector2((float)(this._random.Next(0, Main.maxTilesX) * 16), (float)this._random.Next(minValue, num));
				this._slimes[i].Speed = 5f + 3f * (float)this._random.NextDouble();
				this._slimes[i].Depth = (float)i / (float)this._slimes.Length * 1.75f + 1.6f;
				this._slimes[i].Texture = this._textures[this._random.Next(2)];
				if (this._random.Next(60) == 0)
				{
					this._slimes[i].Texture = this._textures[3];
					this._slimes[i].Speed = 6f + 3f * (float)this._random.NextDouble();
					SlimeSky.Slime[] expr_157_cp_0 = this._slimes;
					int expr_157_cp_1 = i;
					expr_157_cp_0[expr_157_cp_1].Depth = expr_157_cp_0[expr_157_cp_1].Depth + 0.5f;
				}
				else if (this._random.Next(30) == 0)
				{
					this._slimes[i].Texture = this._textures[2];
					this._slimes[i].Speed = 6f + 2f * (float)this._random.NextDouble();
				}
				this._slimes[i].Active = true;
			}
			this._slimesRemaining = this._slimes.Length;
		}

		public override void Update(GameTime gameTime)
		{
			if (Main.gamePaused || !Main.hasFocus)
			{
				return;
			}
			for (int i = 0; i < this._slimes.Length; i++)
			{
				if (this._slimes[i].Active)
				{
					SlimeSky.Slime[] expr_38_cp_0 = this._slimes;
					int expr_38_cp_1 = i;
					expr_38_cp_0[expr_38_cp_1].Frame = expr_38_cp_0[expr_38_cp_1].Frame + 1;
					SlimeSky.Slime[] expr_56_cp_0_cp_0 = this._slimes;
					int expr_56_cp_0_cp_1 = i;
					expr_56_cp_0_cp_0[expr_56_cp_0_cp_1].Position.Y = expr_56_cp_0_cp_0[expr_56_cp_0_cp_1].Position.Y + this._slimes[i].Speed;
					if ((double)this._slimes[i].Position.Y > Main.worldSurface * 16.0)
					{
						if (!this._isLeaving)
						{
							this._slimes[i].Depth = (float)i / (float)this._slimes.Length * 1.75f + 1.6f;
							this._slimes[i].Position = new Vector2((float)(this._random.Next(0, Main.maxTilesX) * 16), -100f);
							this._slimes[i].Texture = this._textures[this._random.Next(2)];
							this._slimes[i].Speed = 5f + 3f * (float)this._random.NextDouble();
							if (this._random.Next(60) == 0)
							{
								this._slimes[i].Texture = this._textures[3];
								this._slimes[i].Speed = 6f + 3f * (float)this._random.NextDouble();
								SlimeSky.Slime[] expr_1AC_cp_0 = this._slimes;
								int expr_1AC_cp_1 = i;
								expr_1AC_cp_0[expr_1AC_cp_1].Depth = expr_1AC_cp_0[expr_1AC_cp_1].Depth + 0.5f;
							}
							else if (this._random.Next(30) == 0)
							{
								this._slimes[i].Texture = this._textures[2];
								this._slimes[i].Speed = 6f + 2f * (float)this._random.NextDouble();
							}
						}
						else
						{
							this._slimes[i].Active = false;
							this._slimesRemaining--;
						}
					}
				}
			}
			if (this._slimesRemaining == 0)
			{
				this._isActive = false;
			}
		}

		public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
		{
			if (Main.screenPosition.Y > 10000f || Main.gameMenu)
			{
				return;
			}
			int num = -1;
			int num2 = 0;
			for (int i = 0; i < this._slimes.Length; i++)
			{
				float depth = this._slimes[i].Depth;
				if (num == -1 && depth < maxDepth)
				{
					num = i;
				}
				if (depth <= minDepth)
				{
					break;
				}
				num2 = i;
			}
			if (num == -1)
			{
				return;
			}
			Vector2 value = Main.screenPosition + new Vector2((float)(Main.screenWidth >> 1), (float)(Main.screenHeight >> 1));
			Rectangle rectangle = new Rectangle(-1000, -1000, 4000, 4000);
			for (int j = num; j < num2; j++)
			{
				if (this._slimes[j].Active)
				{
					Color color = new Color(Main.bgColor.ToVector4() * 0.9f + new Vector4(0.1f)) * 0.8f;
					float num3 = 1f;
					if (this._slimes[j].Depth > 3f)
					{
						num3 = 0.6f;
					}
					else if ((double)this._slimes[j].Depth > 2.5)
					{
						num3 = 0.7f;
					}
					else if (this._slimes[j].Depth > 2f)
					{
						num3 = 0.8f;
					}
					else if ((double)this._slimes[j].Depth > 1.5)
					{
						num3 = 0.9f;
					}
					num3 *= 0.8f;
					color = new Color((int)((float)color.R * num3), (int)((float)color.G * num3), (int)((float)color.B * num3), (int)((float)color.A * num3));
					Vector2 value2 = new Vector2(1f / this._slimes[j].Depth, 0.9f / this._slimes[j].Depth);
					Vector2 vector = this._slimes[j].Position;
					vector = (vector - value) * value2 + value - Main.screenPosition;
					vector.X = (vector.X + 500f) % 4000f;
					if (vector.X < 0f)
					{
						vector.X += 4000f;
					}
					vector.X -= 500f;
					if (rectangle.Contains((int)vector.X, (int)vector.Y))
					{
						spriteBatch.Draw(this._slimes[j].Texture, vector, new Rectangle?(this._slimes[j].GetSourceRectangle()), color, 0f, Vector2.Zero, value2.X * 2f, SpriteEffects.None, 0f);
					}
				}
			}
		}

		public override void Activate(Vector2 position, params object[] args)
		{
			this.GenerateSlimes();
			this._isActive = true;
			this._isLeaving = false;
		}

		public override void Deactivate(params object[] args)
		{
			this._isLeaving = true;
		}

		public override void Reset()
		{
			this._isActive = false;
		}

		public override bool IsActive()
		{
			return this._isActive;
		}
	}
}
