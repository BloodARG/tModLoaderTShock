using Microsoft.Xna.Framework;
using System;

namespace Terraria.Graphics.Effects
{
	public abstract class GameEffect
	{
		public float Opacity;
		protected bool _isLoaded;
		protected EffectPriority _priority;

		public bool IsLoaded
		{
			get
			{
				return this._isLoaded;
			}
		}

		public EffectPriority Priority
		{
			get
			{
				return this._priority;
			}
		}

		public void Load()
		{
			if (this._isLoaded)
			{
				return;
			}
			this._isLoaded = true;
			this.OnLoad();
		}

		public virtual void OnLoad()
		{
		}

		public abstract bool IsVisible();

		public abstract void Activate(Vector2 position, params object[] args);

		public abstract void Deactivate(params object[] args);
	}
}
