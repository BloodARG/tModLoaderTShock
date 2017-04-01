using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.DataStructures;
using Terraria.GameContent.Liquid;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.ID;

namespace Terraria.GameContent.Shaders
{
	public class WaterShaderData : ScreenShaderData
	{
		private struct Ripple
		{
			private static readonly Rectangle[] RIPPLE_SHAPE_SOURCE_RECTS = new Rectangle[]
			{
				new Rectangle(0, 0, 0, 0),
				new Rectangle(1, 1, 62, 62),
				new Rectangle(1, 65, 62, 62)
			};
			public readonly Vector2 Position;
			public readonly Color WaveData;
			public readonly Vector2 Size;
			public readonly RippleShape Shape;
			public readonly float Rotation;

			public Rectangle SourceRectangle
			{
				get
				{
					return WaterShaderData.Ripple.RIPPLE_SHAPE_SOURCE_RECTS[(int)this.Shape];
				}
			}

			public Ripple(Vector2 position, Color waveData, Vector2 size, RippleShape shape, float rotation)
			{
				this.Position = position;
				this.WaveData = waveData;
				this.Size = size;
				this.Shape = shape;
				this.Rotation = rotation;
			}
		}

		private const float DISTORTION_BUFFER_SCALE = 0.25f;
		private const float WAVE_FRAMERATE = 0.0166666675f;
		private const int MAX_RIPPLES_QUEUED = 200;
		private const int MAX_QUEUED_STEPS = 2;
		public bool _useViscosityFilter = true;
		private RenderTarget2D _distortionTarget;
		private RenderTarget2D _distortionTargetSwap;
		private bool _usingRenderTargets;
		private Vector2 _lastDistortionDrawOffset = Vector2.Zero;
		private float _progress;
		private WaterShaderData.Ripple[] _rippleQueue = new WaterShaderData.Ripple[200];
		private int _rippleQueueCount;
		private int _lastScreenWidth;
		private int _lastScreenHeight;
		public bool _useProjectileWaves = true;
		private bool _useNPCWaves = true;
		private bool _usePlayerWaves = true;
		private bool _useRippleWaves = true;
		private bool _useCustomWaves = true;
		private bool _clearNextFrame = true;
		private Texture2D[] _waveMaskChain = new Texture2D[3];
		private int _activeWaveMask;
		private Texture2D _rippleShapeTexture;
		private bool _isWaveBufferDirty = true;
		private int _queuedSteps;

		public event Action<TileBatch> OnWaveDraw;

		public WaterShaderData(string passName)
			: base(passName)
		{
			for (int i = 0; i < this._waveMaskChain.Length; i++)
			{
				this._waveMaskChain[i] = new Texture2D(Main.instance.GraphicsDevice, 200, 200, false, SurfaceFormat.Color);
			}
			Main.OnRenderTargetsInitialized += new ResolutionChangeEvent(this.InitRenderTargets);
			Main.OnRenderTargetsReleased += new Action(this.ReleaseRenderTargets);
			this._rippleShapeTexture = Main.instance.OurLoad<Texture2D>("Images/Misc/Ripples");
			Main.OnPreDraw += new Action<GameTime>(this.PreDraw);
		}

		public override void Update(GameTime gameTime)
		{
			this._useViscosityFilter = (Main.WaveQuality >= 3);
			this._useProjectileWaves = (Main.WaveQuality >= 3);
			this._usePlayerWaves = (Main.WaveQuality >= 2);
			this._useRippleWaves = (Main.WaveQuality >= 2);
			this._useCustomWaves = (Main.WaveQuality >= 2);
			if (Main.gamePaused || !Main.hasFocus)
			{
				return;
			}
			this._progress += (float)gameTime.ElapsedGameTime.TotalSeconds * base.Intensity * 0.75f;
			this._progress %= 86400f;
			if (this._useProjectileWaves || this._useRippleWaves || this._useCustomWaves || this._usePlayerWaves)
			{
				this._queuedSteps++;
			}
			base.Update(gameTime);
		}

		private void StepLiquids()
		{
			this._isWaveBufferDirty = true;
			Vector2 value = Main.drawToScreen ? Vector2.Zero : new Vector2((float)Main.offScreenRange, (float)Main.offScreenRange);
			Vector2 vector = value - Main.screenPosition;
			TileBatch tileBatch = Main.tileBatch;
			GraphicsDevice graphicsDevice = Main.instance.GraphicsDevice;
			graphicsDevice.SetRenderTarget(this._distortionTarget);
			if (this._clearNextFrame)
			{
				graphicsDevice.Clear(new Color(0.5f, 0.5f, 0f, 1f));
				this._clearNextFrame = false;
			}
			this.DrawWaves();
			graphicsDevice.SetRenderTarget(this._distortionTargetSwap);
			graphicsDevice.Clear(new Color(0.5f, 0.5f, 0.5f, 1f));
			Main.tileBatch.Begin();
			vector *= 0.25f;
			vector.X = (float)Math.Floor((double)vector.X);
			vector.Y = (float)Math.Floor((double)vector.Y);
			Vector2 vector2 = vector - this._lastDistortionDrawOffset;
			this._lastDistortionDrawOffset = vector;
			tileBatch.Draw(this._distortionTarget, new Vector4(vector2.X, vector2.Y, (float)this._distortionTarget.Width, (float)this._distortionTarget.Height), new VertexColors(Color.White));
			GameShaders.Misc["WaterProcessor"].Apply(new DrawData?(new DrawData(this._distortionTarget, Vector2.Zero, Color.White)));
			tileBatch.End();
			RenderTarget2D distortionTarget = this._distortionTarget;
			this._distortionTarget = this._distortionTargetSwap;
			this._distortionTargetSwap = distortionTarget;
			if (this._useViscosityFilter)
			{
				LiquidRenderer.Instance.SetWaveMaskData(this._waveMaskChain[this._activeWaveMask]);
				tileBatch.Begin();
				Rectangle cachedDrawArea = LiquidRenderer.Instance.GetCachedDrawArea();
				Rectangle value2 = new Rectangle(0, 0, cachedDrawArea.Width, cachedDrawArea.Height);
				Vector4 vector3 = new Vector4((float)cachedDrawArea.X, (float)cachedDrawArea.Y, (float)cachedDrawArea.Width, (float)cachedDrawArea.Height);
				vector3 *= 16f;
				vector3.X -= value.X;
				vector3.Y -= value.Y;
				vector3 *= 0.25f;
				vector3.X += vector.X;
				vector3.Y += vector.Y;
				graphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
				tileBatch.Draw(this._waveMaskChain[this._activeWaveMask], vector3, new Rectangle?(value2), new VertexColors(Color.White));
				tileBatch.End();
				this._activeWaveMask++;
				this._activeWaveMask %= this._waveMaskChain.Length;
			}
			graphicsDevice.SetRenderTarget(null);
		}

		private void DrawWaves()
		{
			Vector2 screenPosition = Main.screenPosition;
			Vector2 value = Main.drawToScreen ? Vector2.Zero : new Vector2((float)Main.offScreenRange, (float)Main.offScreenRange);
			Vector2 value2 = -this._lastDistortionDrawOffset / 0.25f + value;
			TileBatch tileBatch = Main.tileBatch;
			GraphicsDevice arg_52_0 = Main.instance.GraphicsDevice;
			Vector2 dimensions = new Vector2((float)Main.screenWidth, (float)Main.screenHeight);
			Vector2 value3 = new Vector2(16f, 16f);
			tileBatch.Begin();
			GameShaders.Misc["WaterDistortionObject"].Apply(null);
			if (this._useNPCWaves)
			{
				for (int i = 0; i < 200; i++)
				{
					if (Main.npc[i] != null && Main.npc[i].active && (Main.npc[i].wet || Main.npc[i].wetCount != 0) && Collision.CheckAABBvAABBCollision(screenPosition, dimensions, Main.npc[i].position - value3, Main.npc[i].Size + value3))
					{
						NPC nPC = Main.npc[i];
						Vector2 value4 = nPC.Center - value2;
						Vector2 vector = nPC.velocity.RotatedBy((double)(-(double)nPC.rotation), default(Vector2)) / new Vector2((float)nPC.height, (float)nPC.width);
						float num = vector.LengthSquared();
						num = num * 0.3f + 0.7f * num * (1024f / (float)(nPC.height * nPC.width));
						num = Math.Min(num, 0.08f);
						num += (nPC.velocity - nPC.oldVelocity).Length() * 0.5f;
						vector.Normalize();
						Vector2 velocity = nPC.velocity;
						velocity.Normalize();
						value4 -= velocity * 10f;
						if (!this._useViscosityFilter && (nPC.honeyWet || nPC.lavaWet))
						{
							num *= 0.3f;
						}
						if (nPC.wet)
						{
							tileBatch.Draw(Main.magicPixel, new Vector4(value4.X, value4.Y, (float)nPC.width * 2f, (float)nPC.height * 2f) * 0.25f, null, new VertexColors(new Color(vector.X * 0.5f + 0.5f, vector.Y * 0.5f + 0.5f, 0.5f * num)), new Vector2((float)Main.magicPixel.Width / 2f, (float)Main.magicPixel.Height / 2f), SpriteEffects.None, nPC.rotation);
						}
						if (nPC.wetCount != 0)
						{
							num = nPC.velocity.Length();
							num = 0.195f * (float)Math.Sqrt((double)num);
							float scaleFactor = 5f;
							if (!nPC.wet)
							{
								scaleFactor = -20f;
							}
							this.QueueRipple(nPC.Center + velocity * scaleFactor, new Color(0.5f, (nPC.wet ? num : (-num)) * 0.5f + 0.5f, 0f, 1f) * 0.5f, new Vector2((float)nPC.width, (float)nPC.height * ((float)nPC.wetCount / 9f)) * MathHelper.Clamp(num * 10f, 0f, 1f), RippleShape.Circle, 0f);
						}
					}
				}
			}
			if (this._usePlayerWaves)
			{
				for (int j = 0; j < 255; j++)
				{
					if (Main.player[j] != null && Main.player[j].active && (Main.player[j].wet || Main.player[j].wetCount != 0) && Collision.CheckAABBvAABBCollision(screenPosition, dimensions, Main.player[j].position - value3, Main.player[j].Size + value3))
					{
						Player player = Main.player[j];
						Vector2 value5 = player.Center - value2;
						float num2 = player.velocity.Length();
						num2 = 0.05f * (float)Math.Sqrt((double)num2);
						Vector2 velocity2 = player.velocity;
						velocity2.Normalize();
						value5 -= velocity2 * 10f;
						if (!this._useViscosityFilter && (player.honeyWet || player.lavaWet))
						{
							num2 *= 0.3f;
						}
						if (player.wet)
						{
							tileBatch.Draw(Main.magicPixel, new Vector4(value5.X - (float)player.width * 2f * 0.5f, value5.Y - (float)player.height * 2f * 0.5f, (float)player.width * 2f, (float)player.height * 2f) * 0.25f, new VertexColors(new Color(velocity2.X * 0.5f + 0.5f, velocity2.Y * 0.5f + 0.5f, 0.5f * num2)));
						}
						if (player.wetCount != 0)
						{
							float scaleFactor2 = 5f;
							if (!player.wet)
							{
								scaleFactor2 = -20f;
							}
							num2 *= 3f;
							this.QueueRipple(player.Center + velocity2 * scaleFactor2, player.wet ? num2 : (-num2), new Vector2((float)player.width, (float)player.height * ((float)player.wetCount / 9f)) * MathHelper.Clamp(num2 * 10f, 0f, 1f), RippleShape.Circle, 0f);
						}
					}
				}
			}
			if (this._useProjectileWaves)
			{
				for (int k = 0; k < 1000; k++)
				{
					Projectile projectile = Main.projectile[k];
					bool arg_686_0 = projectile.wet && !projectile.lavaWet && !projectile.honeyWet;
					bool flag = projectile.lavaWet;
					bool flag2 = projectile.honeyWet;
					bool flag3 = projectile.wet;
					if (projectile.ignoreWater)
					{
						flag3 = true;
					}
					if (projectile != null && projectile.active && ProjectileID.Sets.CanDistortWater[projectile.type] && flag3 && !ProjectileID.Sets.NoLiquidDistortion[projectile.type] && Collision.CheckAABBvAABBCollision(screenPosition, dimensions, projectile.position - value3, projectile.Size + value3))
					{
						if (projectile.ignoreWater)
						{
							bool flag4 = Collision.LavaCollision(projectile.position, projectile.width, projectile.height);
							flag = Collision.WetCollision(projectile.position, projectile.width, projectile.height);
							flag2 = Collision.honey;
							flag3 = (flag4 || flag || flag2);
							if (!flag3)
							{
								goto IL_87C;
							}
						}
						Vector2 vector2 = projectile.Center - value2;
						float num3 = projectile.velocity.Length();
						num3 = 2f * (float)Math.Sqrt((double)(0.05f * num3));
						Vector2 velocity3 = projectile.velocity;
						velocity3.Normalize();
						if (!this._useViscosityFilter && (flag2 || flag))
						{
							num3 *= 0.3f;
						}
						float num4 = Math.Max(12f, (float)projectile.width * 0.75f);
						float num5 = Math.Max(12f, (float)projectile.height * 0.75f);
						tileBatch.Draw(Main.magicPixel, new Vector4(vector2.X - num4 * 0.5f, vector2.Y - num5 * 0.5f, num4, num5) * 0.25f, new VertexColors(new Color(velocity3.X * 0.5f + 0.5f, velocity3.Y * 0.5f + 0.5f, num3 * 0.5f)));
					}
					IL_87C:
					;
				}
			}
			tileBatch.End();
			if (this._useRippleWaves)
			{
				tileBatch.Begin();
				for (int l = 0; l < this._rippleQueueCount; l++)
				{
					Vector2 vector3 = this._rippleQueue[l].Position - value2;
					Vector2 size = this._rippleQueue[l].Size;
					Rectangle sourceRectangle = this._rippleQueue[l].SourceRectangle;
					Texture2D rippleShapeTexture = this._rippleShapeTexture;
					tileBatch.Draw(rippleShapeTexture, new Vector4(vector3.X, vector3.Y, size.X, size.Y) * 0.25f, new Rectangle?(sourceRectangle), new VertexColors(this._rippleQueue[l].WaveData), new Vector2((float)(sourceRectangle.Width / 2), (float)(sourceRectangle.Height / 2)), SpriteEffects.None, this._rippleQueue[l].Rotation);
				}
				tileBatch.End();
			}
			this._rippleQueueCount = 0;
			if (this._useCustomWaves && this.OnWaveDraw != null)
			{
				tileBatch.Begin();
				this.OnWaveDraw(tileBatch);
				tileBatch.End();
			}
		}

		private void PreDraw(GameTime gameTime)
		{
			this.ValidateRenderTargets();
			if (!this._usingRenderTargets || !Main.IsGraphicsDeviceAvailable)
			{
				return;
			}
			if (this._useProjectileWaves || this._useRippleWaves || this._useCustomWaves || this._usePlayerWaves)
			{
				for (int i = 0; i < Math.Min(this._queuedSteps, 2); i++)
				{
					this.StepLiquids();
				}
			}
			else if (this._isWaveBufferDirty || this._clearNextFrame)
			{
				GraphicsDevice graphicsDevice = Main.instance.GraphicsDevice;
				graphicsDevice.SetRenderTarget(this._distortionTarget);
				graphicsDevice.Clear(new Color(0.5f, 0.5f, 0f, 1f));
				this._clearNextFrame = false;
				this._isWaveBufferDirty = false;
				graphicsDevice.SetRenderTarget(null);
			}
			this._queuedSteps = 0;
		}

		public override void Apply()
		{
			if (!this._usingRenderTargets || !Main.IsGraphicsDeviceAvailable)
			{
				return;
			}
			base.UseProgress(this._progress);
			Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
			Vector2 value = (Main.drawToScreen ? Vector2.Zero : new Vector2((float)Main.offScreenRange, (float)Main.offScreenRange)) - Main.screenPosition;
			base.UseImage(this._distortionTarget, 1, null);
			base.UseImage(Main.waterTarget, 2, SamplerState.PointClamp);
			base.UseTargetPosition(Main.screenPosition - Main.sceneWaterPos + new Vector2((float)Main.offScreenRange, (float)Main.offScreenRange));
			base.UseImageOffset(-(value * 0.25f - this._lastDistortionDrawOffset) / new Vector2((float)this._distortionTarget.Width, (float)this._distortionTarget.Height));
			base.Apply();
		}

		private void ValidateRenderTargets()
		{
			int backBufferWidth = Main.instance.GraphicsDevice.PresentationParameters.BackBufferWidth;
			int backBufferHeight = Main.instance.GraphicsDevice.PresentationParameters.BackBufferHeight;
			bool flag = !Main.drawToScreen;
			if (this._usingRenderTargets && !flag)
			{
				this.ReleaseRenderTargets();
				return;
			}
			if (!this._usingRenderTargets && flag)
			{
				this.InitRenderTargets(backBufferWidth, backBufferHeight);
				return;
			}
			if (this._usingRenderTargets && flag && (this._distortionTarget.IsContentLost || this._distortionTargetSwap.IsContentLost))
			{
				this._clearNextFrame = true;
			}
		}

		private void InitRenderTargets(int width, int height)
		{
			this._lastScreenWidth = width;
			this._lastScreenHeight = height;
			width = (int)((float)width * 0.25f);
			height = (int)((float)height * 0.25f);
			try
			{
				this._distortionTarget = new RenderTarget2D(Main.instance.GraphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
				this._distortionTargetSwap = new RenderTarget2D(Main.instance.GraphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
				this._usingRenderTargets = true;
				this._clearNextFrame = true;
			}
			catch (Exception ex)
			{
				Lighting.lightMode = 2;
				this._usingRenderTargets = false;
				Console.WriteLine("Failed to create water distortion render targets. " + ex.ToString());
			}
		}

		private void ReleaseRenderTargets()
		{
			try
			{
				if (this._distortionTarget != null)
				{
					this._distortionTarget.Dispose();
				}
				if (this._distortionTargetSwap != null)
				{
					this._distortionTargetSwap.Dispose();
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error disposing of water distortion render targets. " + ex.ToString());
			}
			this._distortionTarget = null;
			this._distortionTargetSwap = null;
			this._usingRenderTargets = false;
		}

		public void QueueRipple(Vector2 position, float strength = 1f, RippleShape shape = RippleShape.Square, float rotation = 0f)
		{
			float g = strength * 0.5f + 0.5f;
			float scale = Math.Min(Math.Abs(strength), 1f);
			this.QueueRipple(position, new Color(0.5f, g, 0f, 1f) * scale, new Vector2(4f * Math.Max(Math.Abs(strength), 1f)), shape, rotation);
		}

		public void QueueRipple(Vector2 position, float strength, Vector2 size, RippleShape shape = RippleShape.Square, float rotation = 0f)
		{
			float g = strength * 0.5f + 0.5f;
			float scale = Math.Min(Math.Abs(strength), 1f);
			this.QueueRipple(position, new Color(0.5f, g, 0f, 1f) * scale, size, shape, rotation);
		}

		public void QueueRipple(Vector2 position, Color waveData, Vector2 size, RippleShape shape = RippleShape.Square, float rotation = 0f)
		{
			if (!this._useRippleWaves || Main.drawToScreen)
			{
				this._rippleQueueCount = 0;
				return;
			}
			if (this._rippleQueueCount < this._rippleQueue.Length)
			{
				this._rippleQueue[this._rippleQueueCount++] = new WaterShaderData.Ripple(position, waveData, size, shape, rotation);
			}
		}
	}
}
