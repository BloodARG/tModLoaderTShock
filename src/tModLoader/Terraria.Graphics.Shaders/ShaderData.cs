using Microsoft.Xna.Framework.Graphics;
using System;

namespace Terraria.Graphics.Shaders
{
	public class ShaderData
	{
		protected Ref<Effect> _shader;
		protected string _passName;
		private EffectPass _effectPass;
		private Effect _lastEffect;

		public Effect Shader
		{
			get
			{
				if (this._shader != null)
				{
					return this._shader.Value;
				}
				return null;
			}
		}

		public ShaderData(Ref<Effect> shader, string passName)
		{
			this._passName = passName;
			this._shader = shader;
		}

		public void SwapProgram(string passName)
		{
			this._passName = passName;
			if (passName != null)
			{
				this._effectPass = this.Shader.CurrentTechnique.Passes[passName];
			}
		}

		protected virtual void Apply()
		{
			if (this._shader != null && this._lastEffect != this._shader.Value && this.Shader != null && this._passName != null)
			{
				this._effectPass = this.Shader.CurrentTechnique.Passes[this._passName];
			}
			this._effectPass.Apply();
		}
	}
}
