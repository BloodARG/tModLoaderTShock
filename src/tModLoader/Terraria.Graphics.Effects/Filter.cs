using Microsoft.Xna.Framework;
using System;
using Terraria.Graphics.Shaders;

namespace Terraria.Graphics.Effects
{
	public class Filter : GameEffect
	{
		public bool Active;
		private ScreenShaderData _shader;
		public bool IsHidden;

		public Filter(ScreenShaderData shader, EffectPriority priority = EffectPriority.VeryLow)
		{
			this._shader = shader;
			this._priority = priority;
		}

		public void Update(GameTime gameTime)
		{
			this._shader.UseGlobalOpacity(this.Opacity);
			this._shader.Update(gameTime);
		}

		public void Apply()
		{
			this._shader.Apply();
		}

		public ScreenShaderData GetShader()
		{
			return this._shader;
		}

		public override void Activate(Vector2 position, params object[] args)
		{
			this._shader.UseGlobalOpacity(this.Opacity);
			this._shader.UseTargetPosition(position);
			this.Active = true;
		}

		public override void Deactivate(params object[] args)
		{
			this.Active = false;
		}

		public bool IsInUse()
		{
			return this.Active || this.Opacity != 0f;
		}

		public bool IsActive()
		{
			return this.Active;
		}

		public override bool IsVisible()
		{
			return this.GetShader().CombinedOpacity > 0f && !this.IsHidden;
		}
	}
}
