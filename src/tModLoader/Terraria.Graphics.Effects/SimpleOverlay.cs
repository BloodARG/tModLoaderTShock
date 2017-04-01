using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.Graphics.Shaders;

namespace Terraria.Graphics.Effects
{
	public class SimpleOverlay : Overlay
	{
		private Ref<Texture2D> _texture;
		private ScreenShaderData _shader;
		public Vector2 TargetPosition = Vector2.Zero;

		public SimpleOverlay(string textureName, ScreenShaderData shader, EffectPriority priority = EffectPriority.VeryLow, RenderLayers layer = RenderLayers.All)
			: base(priority, layer)
		{
			this._texture = TextureManager.Retrieve((textureName == null) ? "" : textureName);
			this._shader = shader;
		}

		public SimpleOverlay(string textureName, string shaderName = "Default", EffectPriority priority = EffectPriority.VeryLow, RenderLayers layer = RenderLayers.All)
			: base(priority, layer)
		{
			this._texture = TextureManager.Retrieve((textureName == null) ? "" : textureName);
			this._shader = new ScreenShaderData(Main.ScreenShaderRef, shaderName);
		}

		public ScreenShaderData GetShader()
		{
			return this._shader;
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			this._shader.UseGlobalOpacity(this.Opacity);
			this._shader.UseTargetPosition(this.TargetPosition);
			this._shader.Apply();
			spriteBatch.Draw(this._texture.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Main.bgColor);
		}

		public override void Update(GameTime gameTime)
		{
			this._shader.Update(gameTime);
		}

		public override void Activate(Vector2 position, params object[] args)
		{
			this.TargetPosition = position;
			this.Mode = OverlayMode.FadeIn;
		}

		public override void Deactivate(params object[] args)
		{
			this.Mode = OverlayMode.FadeOut;
		}

		public override bool IsVisible()
		{
			return this._shader.CombinedOpacity > 0f;
		}
	}
}
