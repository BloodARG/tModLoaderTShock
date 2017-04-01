using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Terraria.Graphics.Effects
{
	public class SkyManager : EffectManager<CustomSky>
	{
		public static SkyManager Instance = new SkyManager();
		private float _lastDepth;
		private LinkedList<CustomSky> _activeSkies = new LinkedList<CustomSky>();

		public void Reset()
		{
			foreach (CustomSky current in this._effects.Values)
			{
				current.Reset();
			}
			this._activeSkies.Clear();
		}

		public void Update(GameTime gameTime)
		{
			LinkedListNode<CustomSky> next;
			for (LinkedListNode<CustomSky> linkedListNode = this._activeSkies.First; linkedListNode != null; linkedListNode = next)
			{
				CustomSky value = linkedListNode.Value;
				next = linkedListNode.Next;
				value.Update(gameTime);
				if (!value.IsActive())
				{
					this._activeSkies.Remove(linkedListNode);
				}
			}
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			this.DrawDepthRange(spriteBatch, -3.40282347E+38f, 3.40282347E+38f);
		}

		public void DrawToDepth(SpriteBatch spriteBatch, float minDepth)
		{
			if (this._lastDepth <= minDepth)
			{
				return;
			}
			this.DrawDepthRange(spriteBatch, minDepth, this._lastDepth);
			this._lastDepth = minDepth;
		}

		public void DrawDepthRange(SpriteBatch spriteBatch, float minDepth, float maxDepth)
		{
			foreach (CustomSky current in this._activeSkies)
			{
				current.Draw(spriteBatch, minDepth, maxDepth);
			}
		}

		public void DrawRemainingDepth(SpriteBatch spriteBatch)
		{
			this.DrawDepthRange(spriteBatch, -3.40282347E+38f, this._lastDepth);
			this._lastDepth = -3.40282347E+38f;
		}

		public void ResetDepthTracker()
		{
			this._lastDepth = 3.40282347E+38f;
		}

		public void SetStartingDepth(float depth)
		{
			this._lastDepth = depth;
		}

		public override void OnActivate(CustomSky effect, Vector2 position)
		{
			this._activeSkies.Remove(effect);
			this._activeSkies.AddLast(effect);
		}

		public Color ProcessTileColor(Color color)
		{
			foreach (CustomSky current in this._activeSkies)
			{
				color = current.OnTileColor(color);
			}
			return color;
		}

		public float ProcessCloudAlpha()
		{
			float num = 1f;
			foreach (CustomSky current in this._activeSkies)
			{
				num *= current.GetCloudAlpha();
			}
			return MathHelper.Clamp(num, 0f, 1f);
		}

		internal void DeactivateAll()
		{
			foreach (string key in this._effects.Keys)
			{
				if (this[key].IsActive())
				{
					this[key].Deactivate(new object[0]);
				}
			}
		}
	}
}
