using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Terraria.Graphics.Effects
{
	public abstract class EffectManager<T> where T : GameEffect
	{
		protected bool _isLoaded;
		protected Dictionary<string, T> _effects = new Dictionary<string, T>();

		public bool IsLoaded
		{
			get
			{
				return this._isLoaded;
			}
		}

		public T this[string key]
		{
			get
			{
				T result;
				if (this._effects.TryGetValue(key, out result))
				{
					return result;
				}
				return default(T);
			}
			set
			{
				this.Bind(key, value);
			}
		}

		public void Bind(string name, T effect)
		{
			this._effects[name] = effect;
			if (this._isLoaded)
			{
				effect.Load();
			}
		}

		public void Load()
		{
			if (this._isLoaded)
			{
				return;
			}
			this._isLoaded = true;
			foreach (T current in this._effects.Values)
			{
				current.Load();
			}
		}

		public T Activate(string name, Vector2 position = default(Vector2), params object[] args)
		{
			if (!this._effects.ContainsKey(name))
			{
				throw new MissingEffectException(string.Concat(new object[]
						{
							"Unable to find effect named: ",
							name,
							". Type: ",
							typeof(T),
							"."
						}));
			}
			T t = this._effects[name];
			this.OnActivate(t, position);
			t.Activate(position, args);
			return t;
		}

		public void Deactivate(string name, params object[] args)
		{
			if (!this._effects.ContainsKey(name))
			{
				throw new MissingEffectException(string.Concat(new object[]
						{
							"Unable to find effect named: ",
							name,
							". Type: ",
							typeof(T),
							"."
						}));
			}
			T effect = this._effects[name];
			this.OnDeactivate(effect);
			effect.Deactivate(args);
		}

		public virtual void OnActivate(T effect, Vector2 position)
		{
		}

		public virtual void OnDeactivate(T effect)
		{
		}
	}
}
