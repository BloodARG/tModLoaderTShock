using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.Graphics.Effects;
using Terraria.Utilities;

namespace Terraria.GameContent.Skies
{
	public class MartianSky : CustomSky
	{
		private abstract class IUfoController
		{
			public abstract void InitializeUfo(ref MartianSky.Ufo ufo);

			public abstract bool Update(ref MartianSky.Ufo ufo);
		}

		private class ZipBehavior : MartianSky.IUfoController
		{
			private Vector2 _speed;
			private int _ticks;
			private int _maxTicks;

			public override void InitializeUfo(ref MartianSky.Ufo ufo)
			{
				ufo.Position.X = (float)(MartianSky.Ufo.Random.NextDouble() * (double)(Main.maxTilesX << 4));
				ufo.Position.Y = (float)(MartianSky.Ufo.Random.NextDouble() * 5000.0);
				ufo.Opacity = 0f;
				float num = (float)MartianSky.Ufo.Random.NextDouble() * 5f + 10f;
				double num2 = MartianSky.Ufo.Random.NextDouble() * 0.60000002384185791 - 0.30000001192092896;
				ufo.Rotation = (float)num2;
				if (MartianSky.Ufo.Random.Next(2) == 0)
				{
					num2 += 3.1415927410125732;
				}
				this._speed = new Vector2((float)Math.Cos(num2) * num, (float)Math.Sin(num2) * num);
				this._ticks = 0;
				this._maxTicks = MartianSky.Ufo.Random.Next(400, 500);
			}

			public override bool Update(ref MartianSky.Ufo ufo)
			{
				if (this._ticks < 10)
				{
					ufo.Opacity += 0.1f;
				}
				else if (this._ticks > this._maxTicks - 10)
				{
					ufo.Opacity -= 0.1f;
				}
				ufo.Position += this._speed;
				if (this._ticks == this._maxTicks)
				{
					return false;
				}
				this._ticks++;
				return true;
			}
		}

		private class HoverBehavior : MartianSky.IUfoController
		{
			private int _ticks;
			private int _maxTicks;

			public override void InitializeUfo(ref MartianSky.Ufo ufo)
			{
				ufo.Position.X = (float)(MartianSky.Ufo.Random.NextDouble() * (double)(Main.maxTilesX << 4));
				ufo.Position.Y = (float)(MartianSky.Ufo.Random.NextDouble() * 5000.0);
				ufo.Opacity = 0f;
				ufo.Rotation = 0f;
				this._ticks = 0;
				this._maxTicks = MartianSky.Ufo.Random.Next(120, 240);
			}

			public override bool Update(ref MartianSky.Ufo ufo)
			{
				if (this._ticks < 10)
				{
					ufo.Opacity += 0.1f;
				}
				else if (this._ticks > this._maxTicks - 10)
				{
					ufo.Opacity -= 0.1f;
				}
				if (this._ticks == this._maxTicks)
				{
					return false;
				}
				this._ticks++;
				return true;
			}
		}

		private struct Ufo
		{
			private const int MAX_FRAMES = 3;
			private const int FRAME_RATE = 4;
			public static UnifiedRandom Random = new UnifiedRandom();
			private int _frame;
			private Texture2D _texture;
			private MartianSky.IUfoController _controller;
			public Texture2D GlowTexture;
			public Vector2 Position;
			public int FrameHeight;
			public int FrameWidth;
			public float Depth;
			public float Scale;
			public float Opacity;
			public bool IsActive;
			public float Rotation;

			public int Frame
			{
				get
				{
					return this._frame;
				}
				set
				{
					this._frame = value % 12;
				}
			}

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
					this.FrameHeight = value.Height / 3;
				}
			}

			public MartianSky.IUfoController Controller
			{
				get
				{
					return this._controller;
				}
				set
				{
					this._controller = value;
					value.InitializeUfo(ref this);
				}
			}

			public Ufo(Texture2D texture, float depth = 1f)
			{
				this._frame = 0;
				this.Position = Vector2.Zero;
				this._texture = texture;
				this.Depth = depth;
				this.Scale = 1f;
				this.FrameWidth = texture.Width;
				this.FrameHeight = texture.Height / 3;
				this.GlowTexture = null;
				this.Opacity = 0f;
				this.Rotation = 0f;
				this.IsActive = false;
				this._controller = null;
			}

			public Rectangle GetSourceRectangle()
			{
				return new Rectangle(0, this._frame / 4 * this.FrameHeight, this.FrameWidth, this.FrameHeight);
			}

			public bool Update()
			{
				return this.Controller.Update(ref this);
			}

			public void AssignNewBehavior()
			{
				switch (MartianSky.Ufo.Random.Next(2))
				{
					case 0:
						this.Controller = new MartianSky.ZipBehavior();
						return;
					case 1:
						this.Controller = new MartianSky.HoverBehavior();
						return;
					default:
						return;
				}
			}
		}

		private MartianSky.Ufo[] _ufos;
		private UnifiedRandom _random = new UnifiedRandom();
		private int _maxUfos;
		private bool _active;
		private bool _leaving;
		private int _activeUfos;

		public override void Update(GameTime gameTime)
		{
			if (Main.gamePaused || !Main.hasFocus)
			{
				return;
			}
			int num = this._activeUfos;
			for (int i = 0; i < this._ufos.Length; i++)
			{
				MartianSky.Ufo ufo = this._ufos[i];
				if (ufo.IsActive)
				{
					ufo.Frame++;
					if (!ufo.Update())
					{
						if (!this._leaving)
						{
							ufo.AssignNewBehavior();
						}
						else
						{
							ufo.IsActive = false;
							num--;
						}
					}
				}
				this._ufos[i] = ufo;
			}
			if (!this._leaving && num != this._maxUfos)
			{
				this._ufos[num].IsActive = true;
				this._ufos[num++].AssignNewBehavior();
			}
			this._active = (!this._leaving || num != 0);
			this._activeUfos = num;
		}

		public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
		{
			if (Main.screenPosition.Y > 10000f)
			{
				return;
			}
			int num = -1;
			int num2 = 0;
			for (int i = 0; i < this._ufos.Length; i++)
			{
				float depth = this._ufos[i].Depth;
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
			Color value = new Color(Main.bgColor.ToVector4() * 0.9f + new Vector4(0.1f));
			Vector2 value2 = Main.screenPosition + new Vector2((float)(Main.screenWidth >> 1), (float)(Main.screenHeight >> 1));
			Rectangle rectangle = new Rectangle(-1000, -1000, 4000, 4000);
			for (int j = num; j < num2; j++)
			{
				Vector2 value3 = new Vector2(1f / this._ufos[j].Depth, 0.9f / this._ufos[j].Depth);
				Vector2 vector = this._ufos[j].Position;
				vector = (vector - value2) * value3 + value2 - Main.screenPosition;
				if (this._ufos[j].IsActive && rectangle.Contains((int)vector.X, (int)vector.Y))
				{
					spriteBatch.Draw(this._ufos[j].Texture, vector, new Rectangle?(this._ufos[j].GetSourceRectangle()), value * this._ufos[j].Opacity, this._ufos[j].Rotation, Vector2.Zero, value3.X * 5f * this._ufos[j].Scale, SpriteEffects.None, 0f);
					if (this._ufos[j].GlowTexture != null)
					{
						spriteBatch.Draw(this._ufos[j].GlowTexture, vector, new Rectangle?(this._ufos[j].GetSourceRectangle()), Color.White * this._ufos[j].Opacity, this._ufos[j].Rotation, Vector2.Zero, value3.X * 5f * this._ufos[j].Scale, SpriteEffects.None, 0f);
					}
				}
			}
		}

		private void GenerateUfos()
		{
			float num = (float)Main.maxTilesX / 4200f;
			this._maxUfos = (int)(256f * num);
			this._ufos = new MartianSky.Ufo[this._maxUfos];
			int num2 = this._maxUfos >> 4;
			for (int i = 0; i < num2; i++)
			{
				float arg_3E_0 = (float)i / (float)num2;
				this._ufos[i] = new MartianSky.Ufo(Main.extraTexture[5], (float)Main.rand.NextDouble() * 4f + 6.6f);
				this._ufos[i].GlowTexture = Main.glowMaskTexture[90];
			}
			for (int j = num2; j < this._ufos.Length; j++)
			{
				float arg_A8_0 = (float)(j - num2) / (float)(this._ufos.Length - num2);
				this._ufos[j] = new MartianSky.Ufo(Main.extraTexture[6], (float)Main.rand.NextDouble() * 5f + 1.6f);
				this._ufos[j].Scale = 0.5f;
				this._ufos[j].GlowTexture = Main.glowMaskTexture[91];
			}
		}

		public override void Activate(Vector2 position, params object[] args)
		{
			this._activeUfos = 0;
			this.GenerateUfos();
			Array.Sort<MartianSky.Ufo>(this._ufos, (MartianSky.Ufo ufo1, MartianSky.Ufo ufo2) => ufo2.Depth.CompareTo(ufo1.Depth));
			this._active = true;
			this._leaving = false;
		}

		public override void Deactivate(params object[] args)
		{
			this._leaving = true;
		}

		public override bool IsActive()
		{
			return this._active;
		}

		public override void Reset()
		{
			this._active = false;
		}
	}
}
