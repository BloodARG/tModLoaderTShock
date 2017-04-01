using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.Graphics;
using Terraria.Graphics.Effects;
using Terraria.Utilities;

namespace Terraria.GameContent.Skies
{
	public class VortexSky : CustomSky
	{
		private struct Bolt
		{
			public Vector2 Position;
			public float Depth;
			public int Life;
			public bool IsAlive;
		}

		private UnifiedRandom _random = new UnifiedRandom();
		private Texture2D _planetTexture;
		private Texture2D _bgTexture;
		private Texture2D _boltTexture;
		private Texture2D _flashTexture;
		private bool _isActive;
		private int _ticksUntilNextBolt;
		private float _fadeOpacity;
		private VortexSky.Bolt[] _bolts;

		public override void OnLoad()
		{
			this._planetTexture = TextureManager.Load("Images/Misc/VortexSky/Planet");
			this._bgTexture = TextureManager.Load("Images/Misc/VortexSky/Background");
			this._boltTexture = TextureManager.Load("Images/Misc/VortexSky/Bolt");
			this._flashTexture = TextureManager.Load("Images/Misc/VortexSky/Flash");
		}

		public override void Update(GameTime gameTime)
		{
			if (this._isActive)
			{
				this._fadeOpacity = Math.Min(1f, 0.01f + this._fadeOpacity);
			}
			else
			{
				this._fadeOpacity = Math.Max(0f, this._fadeOpacity - 0.01f);
			}
			if (this._ticksUntilNextBolt <= 0)
			{
				this._ticksUntilNextBolt = this._random.Next(1, 5);
				int num = 0;
				while (this._bolts[num].IsAlive && num != this._bolts.Length - 1)
				{
					num++;
				}
				this._bolts[num].IsAlive = true;
				this._bolts[num].Position.X = this._random.NextFloat() * ((float)Main.maxTilesX * 16f + 4000f) - 2000f;
				this._bolts[num].Position.Y = this._random.NextFloat() * 500f;
				this._bolts[num].Depth = this._random.NextFloat() * 8f + 2f;
				this._bolts[num].Life = 30;
			}
			this._ticksUntilNextBolt--;
			for (int i = 0; i < this._bolts.Length; i++)
			{
				if (this._bolts[i].IsAlive)
				{
					VortexSky.Bolt[] expr_168_cp_0 = this._bolts;
					int expr_168_cp_1 = i;
					expr_168_cp_0[expr_168_cp_1].Life = expr_168_cp_0[expr_168_cp_1].Life - 1;
					if (this._bolts[i].Life <= 0)
					{
						this._bolts[i].IsAlive = false;
					}
				}
			}
		}

		public override Color OnTileColor(Color inColor)
		{
			Vector4 value = inColor.ToVector4();
			return new Color(Vector4.Lerp(value, Vector4.One, this._fadeOpacity * 0.5f));
		}

		public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
		{
			if (maxDepth >= 3.40282347E+38f && minDepth < 3.40282347E+38f)
			{
				spriteBatch.Draw(Main.blackTileTexture, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Black * this._fadeOpacity);
				spriteBatch.Draw(this._bgTexture, new Rectangle(0, Math.Max(0, (int)((Main.worldSurface * 16.0 - (double)Main.screenPosition.Y - 2400.0) * 0.10000000149011612)), Main.screenWidth, Main.screenHeight), Color.White * Math.Min(1f, (Main.screenPosition.Y - 800f) / 1000f) * this._fadeOpacity);
				Vector2 value = new Vector2((float)(Main.screenWidth >> 1), (float)(Main.screenHeight >> 1));
				Vector2 value2 = 0.01f * (new Vector2((float)Main.maxTilesX * 8f, (float)Main.worldSurface / 2f) - Main.screenPosition);
				spriteBatch.Draw(this._planetTexture, value + new Vector2(-200f, -200f) + value2, null, Color.White * 0.9f * this._fadeOpacity, 0f, new Vector2((float)(this._planetTexture.Width >> 1), (float)(this._planetTexture.Height >> 1)), 1f, SpriteEffects.None, 1f);
			}
			float scale = Math.Min(1f, (Main.screenPosition.Y - 1000f) / 1000f);
			Vector2 value3 = Main.screenPosition + new Vector2((float)(Main.screenWidth >> 1), (float)(Main.screenHeight >> 1));
			Rectangle rectangle = new Rectangle(-1000, -1000, 4000, 4000);
			for (int i = 0; i < this._bolts.Length; i++)
			{
				if (this._bolts[i].IsAlive && this._bolts[i].Depth > minDepth && this._bolts[i].Depth < maxDepth)
				{
					Vector2 value4 = new Vector2(1f / this._bolts[i].Depth, 0.9f / this._bolts[i].Depth);
					Vector2 position = (this._bolts[i].Position - value3) * value4 + value3 - Main.screenPosition;
					if (rectangle.Contains((int)position.X, (int)position.Y))
					{
						Texture2D texture = this._boltTexture;
						int life = this._bolts[i].Life;
						if (life > 26 && life % 2 == 0)
						{
							texture = this._flashTexture;
						}
						float scale2 = (float)life / 30f;
						spriteBatch.Draw(texture, position, null, Color.White * scale * scale2 * this._fadeOpacity, 0f, Vector2.Zero, value4.X * 5f, SpriteEffects.None, 0f);
					}
				}
			}
		}

		public override float GetCloudAlpha()
		{
			return (1f - this._fadeOpacity) * 0.3f + 0.7f;
		}

		public override void Activate(Vector2 position, params object[] args)
		{
			this._fadeOpacity = 0.002f;
			this._isActive = true;
			this._bolts = new VortexSky.Bolt[500];
			for (int i = 0; i < this._bolts.Length; i++)
			{
				this._bolts[i].IsAlive = false;
			}
		}

		public override void Deactivate(params object[] args)
		{
			this._isActive = false;
		}

		public override void Reset()
		{
			this._isActive = false;
		}

		public override bool IsActive()
		{
			return this._isActive || this._fadeOpacity > 0.001f;
		}
	}
}
