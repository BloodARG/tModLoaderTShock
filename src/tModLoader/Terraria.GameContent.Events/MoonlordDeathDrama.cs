using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria.Graphics;
using Terraria.Utilities;

namespace Terraria.GameContent.Events
{
	public class MoonlordDeathDrama
	{
		public class MoonlordPiece
		{
			private Texture2D _texture;
			private Vector2 _position;
			private Vector2 _velocity;
			private Vector2 _origin;
			private float _rotation;
			private float _rotationVelocity;

			public bool Dead
			{
				get
				{
					return this._position.Y > (float)(Main.maxTilesY * 16) - 480f || this._position.X < 480f || this._position.X >= (float)(Main.maxTilesX * 16) - 480f;
				}
			}

			public MoonlordPiece(Texture2D pieceTexture, Vector2 textureOrigin, Vector2 centerPos, Vector2 velocity, float rot, float angularVelocity)
			{
				this._texture = pieceTexture;
				this._origin = textureOrigin;
				this._position = centerPos;
				this._velocity = velocity;
				this._rotation = rot;
				this._rotationVelocity = angularVelocity;
			}

			public void Update()
			{
				this._velocity.Y = this._velocity.Y + 0.3f;
				this._rotation += this._rotationVelocity;
				this._rotationVelocity *= 0.99f;
				this._position += this._velocity;
			}

			public void Draw(SpriteBatch sp)
			{
				Color light = this.GetLight();
				sp.Draw(this._texture, this._position - Main.screenPosition, null, light, this._rotation, this._origin, 1f, SpriteEffects.None, 0f);
			}

			public bool InDrawRange(Rectangle playerScreen)
			{
				return playerScreen.Contains(this._position.ToPoint());
			}

			public Color GetLight()
			{
				Vector3 vector = Vector3.Zero;
				float num = 0f;
				int num2 = 5;
				Point point = this._position.ToTileCoordinates();
				for (int i = point.X - num2; i <= point.X + num2; i++)
				{
					for (int j = point.Y - num2; j <= point.Y + num2; j++)
					{
						vector += Lighting.GetColor(i, j).ToVector3();
						num += 1f;
					}
				}
				if (num == 0f)
				{
					return Color.White;
				}
				return new Color(vector / num);
			}
		}

		public class MoonlordExplosion
		{
			private Texture2D _texture;
			private Vector2 _position;
			private Vector2 _origin;
			private Rectangle _frame;
			private int _frameCounter;
			private int _frameSpeed;

			public bool Dead
			{
				get
				{
					return this._position.Y > (float)(Main.maxTilesY * 16) - 480f || this._position.X < 480f || this._position.X >= (float)(Main.maxTilesX * 16) - 480f || this._frameCounter >= this._frameSpeed * 7;
				}
			}

			public MoonlordExplosion(Texture2D pieceTexture, Vector2 centerPos, int frameSpeed)
			{
				this._texture = pieceTexture;
				this._position = centerPos;
				this._frameSpeed = frameSpeed;
				this._frameCounter = 0;
				this._frame = this._texture.Frame(1, 7, 0, 0);
				this._origin = this._frame.Size() / 2f;
			}

			public void Update()
			{
				this._frameCounter++;
				this._frame = this._texture.Frame(1, 7, 0, this._frameCounter / this._frameSpeed);
			}

			public void Draw(SpriteBatch sp)
			{
				Color light = this.GetLight();
				sp.Draw(this._texture, this._position - Main.screenPosition, new Rectangle?(this._frame), light, 0f, this._origin, 1f, SpriteEffects.None, 0f);
			}

			public bool InDrawRange(Rectangle playerScreen)
			{
				return playerScreen.Contains(this._position.ToPoint());
			}

			public Color GetLight()
			{
				return new Color(255, 255, 255, 127);
			}
		}

		private static List<MoonlordDeathDrama.MoonlordPiece> _pieces = new List<MoonlordDeathDrama.MoonlordPiece>();
		private static List<MoonlordDeathDrama.MoonlordExplosion> _explosions = new List<MoonlordDeathDrama.MoonlordExplosion>();
		private static List<Vector2> _lightSources = new List<Vector2>();
		private static float whitening = 0f;
		private static float requestedLight = 0f;

		public static void Update()
		{
			for (int i = 0; i < MoonlordDeathDrama._pieces.Count; i++)
			{
				MoonlordDeathDrama.MoonlordPiece moonlordPiece = MoonlordDeathDrama._pieces[i];
				moonlordPiece.Update();
				if (moonlordPiece.Dead)
				{
					MoonlordDeathDrama._pieces.Remove(moonlordPiece);
					i--;
				}
			}
			for (int j = 0; j < MoonlordDeathDrama._explosions.Count; j++)
			{
				MoonlordDeathDrama.MoonlordExplosion moonlordExplosion = MoonlordDeathDrama._explosions[j];
				moonlordExplosion.Update();
				if (moonlordExplosion.Dead)
				{
					MoonlordDeathDrama._explosions.Remove(moonlordExplosion);
					j--;
				}
			}
			bool flag = false;
			for (int k = 0; k < MoonlordDeathDrama._lightSources.Count; k++)
			{
				if (Main.player[Main.myPlayer].Distance(MoonlordDeathDrama._lightSources[k]) < 2000f)
				{
					flag = true;
					break;
				}
			}
			MoonlordDeathDrama._lightSources.Clear();
			if (!flag)
			{
				MoonlordDeathDrama.requestedLight = 0f;
			}
			if (MoonlordDeathDrama.requestedLight != MoonlordDeathDrama.whitening)
			{
				if (Math.Abs(MoonlordDeathDrama.requestedLight - MoonlordDeathDrama.whitening) < 0.02f)
				{
					MoonlordDeathDrama.whitening = MoonlordDeathDrama.requestedLight;
				}
				else
				{
					MoonlordDeathDrama.whitening += (float)Math.Sign(MoonlordDeathDrama.requestedLight - MoonlordDeathDrama.whitening) * 0.02f;
				}
			}
			MoonlordDeathDrama.requestedLight = 0f;
		}

		public static void DrawPieces(SpriteBatch spriteBatch)
		{
			Rectangle playerScreen = Utils.CenteredRectangle(Main.screenPosition + new Vector2((float)Main.screenWidth, (float)Main.screenHeight) * 0.5f, new Vector2((float)(Main.screenWidth + 1000), (float)(Main.screenHeight + 1000)));
			for (int i = 0; i < MoonlordDeathDrama._pieces.Count; i++)
			{
				if (MoonlordDeathDrama._pieces[i].InDrawRange(playerScreen))
				{
					MoonlordDeathDrama._pieces[i].Draw(spriteBatch);
				}
			}
		}

		public static void DrawExplosions(SpriteBatch spriteBatch)
		{
			Rectangle playerScreen = Utils.CenteredRectangle(Main.screenPosition + new Vector2((float)Main.screenWidth, (float)Main.screenHeight) * 0.5f, new Vector2((float)(Main.screenWidth + 1000), (float)(Main.screenHeight + 1000)));
			for (int i = 0; i < MoonlordDeathDrama._explosions.Count; i++)
			{
				if (MoonlordDeathDrama._explosions[i].InDrawRange(playerScreen))
				{
					MoonlordDeathDrama._explosions[i].Draw(spriteBatch);
				}
			}
		}

		public static void DrawWhite(SpriteBatch spriteBatch)
		{
			if (MoonlordDeathDrama.whitening == 0f)
			{
				return;
			}
			Color color = Color.White * MoonlordDeathDrama.whitening;
			spriteBatch.Draw(Main.magicPixel, new Rectangle(-2, -2, Main.screenWidth + 4, Main.screenHeight + 4), new Rectangle?(new Rectangle(0, 0, 1, 1)), color);
		}

		public static void ThrowPieces(Vector2 MoonlordCoreCenter, int DramaSeed)
		{
			UnifiedRandom r = new UnifiedRandom(DramaSeed);
			Vector2 value = Vector2.UnitY.RotatedBy((double)(r.NextFloat() * 1.57079637f - 0.7853982f + 3.14159274f), default(Vector2));
			MoonlordDeathDrama._pieces.Add(new MoonlordDeathDrama.MoonlordPiece(TextureManager.Load("Images/Misc/MoonExplosion/Spine"), new Vector2(64f, 150f), MoonlordCoreCenter + new Vector2(0f, 50f), value * 6f, 0f, r.NextFloat() * 0.1f - 0.05f));
			value = Vector2.UnitY.RotatedBy((double)(r.NextFloat() * 1.57079637f - 0.7853982f + 3.14159274f), default(Vector2));
			MoonlordDeathDrama._pieces.Add(new MoonlordDeathDrama.MoonlordPiece(TextureManager.Load("Images/Misc/MoonExplosion/Shoulder"), new Vector2(40f, 120f), MoonlordCoreCenter + new Vector2(50f, -120f), value * 10f, 0f, r.NextFloat() * 0.1f - 0.05f));
			value = Vector2.UnitY.RotatedBy((double)(r.NextFloat() * 1.57079637f - 0.7853982f + 3.14159274f), default(Vector2));
			MoonlordDeathDrama._pieces.Add(new MoonlordDeathDrama.MoonlordPiece(TextureManager.Load("Images/Misc/MoonExplosion/Torso"), new Vector2(192f, 252f), MoonlordCoreCenter, value * 8f, 0f, r.NextFloat() * 0.1f - 0.05f));
			value = Vector2.UnitY.RotatedBy((double)(r.NextFloat() * 1.57079637f - 0.7853982f + 3.14159274f), default(Vector2));
			MoonlordDeathDrama._pieces.Add(new MoonlordDeathDrama.MoonlordPiece(TextureManager.Load("Images/Misc/MoonExplosion/Head"), new Vector2(138f, 185f), MoonlordCoreCenter - new Vector2(0f, 200f), value * 12f, 0f, r.NextFloat() * 0.1f - 0.05f));
		}

		public static void AddExplosion(Vector2 spot)
		{
			MoonlordDeathDrama._explosions.Add(new MoonlordDeathDrama.MoonlordExplosion(TextureManager.Load("Images/Misc/MoonExplosion/Explosion"), spot, Main.rand.Next(2, 4)));
		}

		public static void RequestLight(float light, Vector2 spot)
		{
			MoonlordDeathDrama._lightSources.Add(spot);
			if (light > 1f)
			{
				light = 1f;
			}
			if (MoonlordDeathDrama.requestedLight < light)
			{
				MoonlordDeathDrama.requestedLight = light;
			}
		}
	}
}
