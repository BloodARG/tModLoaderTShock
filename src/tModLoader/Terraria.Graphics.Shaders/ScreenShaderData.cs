using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Terraria.Graphics.Shaders
{
	public class ScreenShaderData : ShaderData
	{
		private Vector3 _uColor = Vector3.One;
		private Vector3 _uSecondaryColor = Vector3.One;
		private float _uOpacity = 1f;
		private float _globalOpacity = 1f;
		private float _uIntensity = 1f;
		private Vector2 _uTargetPosition = Vector2.One;
		private Vector2 _uDirection = new Vector2(0f, 1f);
		private float _uProgress;
		private Vector2 _imageScale = Vector2.One;
		private Vector2 _uImageOffset = Vector2.Zero;
		private Ref<Texture2D>[] _uImages = new Ref<Texture2D>[3];
		private SamplerState[] _samplerStates = new SamplerState[3];

		public float Intensity
		{
			get
			{
				return this._uIntensity;
			}
		}

		public float CombinedOpacity
		{
			get
			{
				return this._uOpacity * this._globalOpacity;
			}
		}

		public ScreenShaderData(string passName)
			: base(Main.ScreenShaderRef, passName)
		{
		}

		public ScreenShaderData(Ref<Effect> shader, string passName)
			: base(shader, passName)
		{
		}

		public virtual void Update(GameTime gameTime)
		{
		}

		public new virtual void Apply()
		{
			Vector2 value = new Vector2((float)Main.offScreenRange, (float)Main.offScreenRange);
			Vector2 value2 = new Vector2((float)Main.screenWidth, (float)Main.screenHeight);
			base.Shader.Parameters["uColor"].SetValue(this._uColor);
			base.Shader.Parameters["uOpacity"].SetValue(this.CombinedOpacity);
			base.Shader.Parameters["uSecondaryColor"].SetValue(this._uSecondaryColor);
			base.Shader.Parameters["uTime"].SetValue(Main.GlobalTime);
			base.Shader.Parameters["uScreenResolution"].SetValue(value2);
			base.Shader.Parameters["uScreenPosition"].SetValue(Main.screenPosition - value);
			base.Shader.Parameters["uTargetPosition"].SetValue(this._uTargetPosition - value);
			base.Shader.Parameters["uImageOffset"].SetValue(this._uImageOffset);
			base.Shader.Parameters["uIntensity"].SetValue(this._uIntensity);
			base.Shader.Parameters["uProgress"].SetValue(this._uProgress);
			base.Shader.Parameters["uDirection"].SetValue(this._uDirection);
			for (int i = 0; i < this._uImages.Length; i++)
			{
				if (this._uImages[i] != null && this._uImages[i].Value != null)
				{
					Main.graphics.GraphicsDevice.Textures[i + 1] = this._uImages[i].Value;
					int width = this._uImages[i].Value.Width;
					int height = this._uImages[i].Value.Height;
					if (this._samplerStates[i] != null)
					{
						Main.graphics.GraphicsDevice.SamplerStates[i + 1] = this._samplerStates[i];
					}
					else if (Utils.IsPowerOfTwo(width) && Utils.IsPowerOfTwo(height))
					{
						Main.graphics.GraphicsDevice.SamplerStates[i + 1] = SamplerState.LinearWrap;
					}
					else
					{
						Main.graphics.GraphicsDevice.SamplerStates[i + 1] = SamplerState.AnisotropicClamp;
					}
					base.Shader.Parameters["uImageSize" + (i + 1)].SetValue(new Vector2((float)width, (float)height) * this._imageScale);
				}
			}
			base.Apply();
		}

		public ScreenShaderData UseImageOffset(Vector2 offset)
		{
			this._uImageOffset = offset;
			return this;
		}

		public ScreenShaderData UseIntensity(float intensity)
		{
			this._uIntensity = intensity;
			return this;
		}

		public ScreenShaderData UseColor(float r, float g, float b)
		{
			return this.UseColor(new Vector3(r, g, b));
		}

		public ScreenShaderData UseProgress(float progress)
		{
			this._uProgress = progress;
			return this;
		}

		public ScreenShaderData UseImage(Texture2D image, int index = 0, SamplerState samplerState = null)
		{
			this._samplerStates[index] = samplerState;
			if (this._uImages[index] == null)
			{
				this._uImages[index] = new Ref<Texture2D>(image);
			}
			else
			{
				this._uImages[index].Value = image;
			}
			return this;
		}

		public ScreenShaderData UseImage(string path, int index = 0, SamplerState samplerState = null)
		{
			this._uImages[index] = TextureManager.Retrieve(path);
			this._samplerStates[index] = samplerState;
			return this;
		}

		public ScreenShaderData UseColor(Color color)
		{
			return this.UseColor(color.ToVector3());
		}

		public ScreenShaderData UseColor(Vector3 color)
		{
			this._uColor = color;
			return this;
		}

		public ScreenShaderData UseDirection(Vector2 direction)
		{
			this._uDirection = direction;
			return this;
		}

		public ScreenShaderData UseGlobalOpacity(float opacity)
		{
			this._globalOpacity = opacity;
			return this;
		}

		public ScreenShaderData UseTargetPosition(Vector2 position)
		{
			this._uTargetPosition = position;
			return this;
		}

		public ScreenShaderData UseSecondaryColor(float r, float g, float b)
		{
			return this.UseSecondaryColor(new Vector3(r, g, b));
		}

		public ScreenShaderData UseSecondaryColor(Color color)
		{
			return this.UseSecondaryColor(color.ToVector3());
		}

		public ScreenShaderData UseSecondaryColor(Vector3 color)
		{
			this._uSecondaryColor = color;
			return this;
		}

		public ScreenShaderData UseOpacity(float opacity)
		{
			this._uOpacity = opacity;
			return this;
		}

		public ScreenShaderData UseImageScale(Vector2 scale)
		{
			this._imageScale = scale;
			return this;
		}

		public virtual ScreenShaderData GetSecondaryShader(Player player)
		{
			return this;
		}
	}
}
