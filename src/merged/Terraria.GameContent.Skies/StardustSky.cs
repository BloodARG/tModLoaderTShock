using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.Graphics;
using Terraria.Graphics.Effects;
using Terraria.Utilities;

namespace Terraria.GameContent.Skies
{
	public class StardustSky : CustomSky
	{
		private struct Star
		{
			public Vector2 Position;

			public float Depth;

			public int TextureIndex;

			public float SinOffset;

			public float AlphaFrequency;

			public float AlphaAmplitude;
		}

		private UnifiedRandom _random = new UnifiedRandom();

		private Texture2D _planetTexture;

		private Texture2D _bgTexture;

		private Texture2D[] _starTextures;

		private bool _isActive;

		private StardustSky.Star[] _stars;

		private float _fadeOpacity;

		public override void OnLoad()
		{
			this._planetTexture = TextureManager.Load("Images/Misc/StarDustSky/Planet");
			this._bgTexture = TextureManager.Load("Images/Misc/StarDustSky/Background");
			this._starTextures = new Texture2D[2];
			for (int i = 0; i < this._starTextures.Length; i++)
			{
				this._starTextures[i] = TextureManager.Load("Images/Misc/StarDustSky/Star " + i);
			}
		}

		public override void Update(GameTime gameTime)
		{
			if (this._isActive)
			{
				this._fadeOpacity = Math.Min(1f, 0.01f + this._fadeOpacity);
				return;
			}
			this._fadeOpacity = Math.Max(0f, this._fadeOpacity - 0.01f);
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
				spriteBatch.Draw(this._bgTexture, new Rectangle(0, Math.Max(0, (int)((Main.worldSurface * 16.0 - (double)Main.screenPosition.Y - 2400.0) * 0.10000000149011612)), Main.screenWidth, Main.screenHeight), Color.White * Math.Min(1f, (Main.screenPosition.Y - 800f) / 1000f * this._fadeOpacity));
				Vector2 value = new Vector2((float)(Main.screenWidth >> 1), (float)(Main.screenHeight >> 1));
				Vector2 value2 = 0.01f * (new Vector2((float)Main.maxTilesX * 8f, (float)Main.worldSurface / 2f) - Main.screenPosition);
				spriteBatch.Draw(this._planetTexture, value + new Vector2(-200f, -200f) + value2, null, Color.White * 0.9f * this._fadeOpacity, 0f, new Vector2((float)(this._planetTexture.Width >> 1), (float)(this._planetTexture.Height >> 1)), 1f, SpriteEffects.None, 1f);
			}
			int num = -1;
			int num2 = 0;
			for (int i = 0; i < this._stars.Length; i++)
			{
				float depth = this._stars[i].Depth;
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
			float scale = Math.Min(1f, (Main.screenPosition.Y - 1000f) / 1000f);
			Vector2 value3 = Main.screenPosition + new Vector2((float)(Main.screenWidth >> 1), (float)(Main.screenHeight >> 1));
			Rectangle rectangle = new Rectangle(-1000, -1000, 4000, 4000);
			for (int j = num; j < num2; j++)
			{
				Vector2 value4 = new Vector2(1f / this._stars[j].Depth, 1.1f / this._stars[j].Depth);
				Vector2 position = (this._stars[j].Position - value3) * value4 + value3 - Main.screenPosition;
				if (rectangle.Contains((int)position.X, (int)position.Y))
				{
					float num3 = (float)Math.Sin((double)(this._stars[j].AlphaFrequency * Main.GlobalTime + this._stars[j].SinOffset)) * this._stars[j].AlphaAmplitude + this._stars[j].AlphaAmplitude;
					float num4 = (float)Math.Sin((double)(this._stars[j].AlphaFrequency * Main.GlobalTime * 5f + this._stars[j].SinOffset)) * 0.1f - 0.1f;
					num3 = MathHelper.Clamp(num3, 0f, 1f);
					Texture2D texture2D = this._starTextures[this._stars[j].TextureIndex];
					spriteBatch.Draw(texture2D, position, null, Color.White * scale * num3 * 0.8f * (1f - num4) * this._fadeOpacity, 0f, new Vector2((float)(texture2D.Width >> 1), (float)(texture2D.Height >> 1)), (value4.X * 0.5f + 0.5f) * (num3 * 0.3f + 0.7f), SpriteEffects.None, 0f);
				}
			}
		}

		public override float GetCloudAlpha()
		{
			return (1f - this._fadeOpacity) * 0.3f + 0.7f;
		}

		internal override void Activate(Vector2 position, params object[] args)
		{
			this._fadeOpacity = 0.002f;
			this._isActive = true;
			int num = 200;
			int num2 = 10;
			this._stars = new StardustSky.Star[num * num2];
			int num3 = 0;
			for (int i = 0; i < num; i++)
			{
				float num4 = (float)i / (float)num;
				for (int j = 0; j < num2; j++)
				{
					float num5 = (float)j / (float)num2;
					this._stars[num3].Position.X = num4 * (float)Main.maxTilesX * 16f;
					this._stars[num3].Position.Y = num5 * ((float)Main.worldSurface * 16f + 2000f) - 1000f;
					this._stars[num3].Depth = this._random.NextFloat() * 8f + 1.5f;
					this._stars[num3].TextureIndex = this._random.Next(this._starTextures.Length);
					this._stars[num3].SinOffset = this._random.NextFloat() * 6.28f;
					this._stars[num3].AlphaAmplitude = this._random.NextFloat() * 5f;
					this._stars[num3].AlphaFrequency = this._random.NextFloat() + 1f;
					num3++;
				}
			}
			Array.Sort<StardustSky.Star>(this._stars, new Comparison<StardustSky.Star>(this.SortMethod));
		}

		private int SortMethod(StardustSky.Star meteor1, StardustSky.Star meteor2)
		{
			return meteor2.Depth.CompareTo(meteor1.Depth);
		}

		internal override void Deactivate(params object[] args)
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
