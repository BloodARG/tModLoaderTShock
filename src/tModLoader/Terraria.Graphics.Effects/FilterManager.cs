using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria.IO;

namespace Terraria.Graphics.Effects
{
	public class FilterManager : EffectManager<Filter>
	{
		private const float OPACITY_RATE = 1f;
		private LinkedList<Filter> _activeFilters = new LinkedList<Filter>();
		private int _filterLimit = 16;
		private EffectPriority _priorityThreshold;
		private int _activeFilterCount;
		private bool _captureThisFrame;

		public event Action OnPostDraw;

		public FilterManager()
		{
			Main.Configuration.OnLoad += delegate(Preferences preferences)
			{
				this._filterLimit = preferences.Get<int>("FilterLimit", 16);
				string value = preferences.Get<string>("FilterPriorityThreshold", "VeryLow");
				EffectPriority priorityThreshold;
				if (Enum.TryParse<EffectPriority>(value, out priorityThreshold))
				{
					this._priorityThreshold = priorityThreshold;
				}
			};
			Main.Configuration.OnSave += delegate(Preferences preferences)
			{
				preferences.Put("FilterLimit", this._filterLimit);
				preferences.Put("FilterPriorityThreshold", Enum.GetName(typeof(EffectPriority), this._priorityThreshold));
			};
		}

		public override void OnActivate(Filter effect, Vector2 position)
		{
			if (this._activeFilters.Contains(effect))
			{
				if (effect.Active)
				{
					return;
				}
				if (effect.Priority >= this._priorityThreshold)
				{
					this._activeFilterCount--;
				}
				this._activeFilters.Remove(effect);
			}
			else
			{
				effect.Opacity = 0f;
			}
			if (effect.Priority >= this._priorityThreshold)
			{
				this._activeFilterCount++;
			}
			if (this._activeFilters.Count == 0)
			{
				this._activeFilters.AddLast(effect);
				return;
			}
			for (LinkedListNode<Filter> linkedListNode = this._activeFilters.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				Filter value = linkedListNode.Value;
				if (effect.Priority <= value.Priority)
				{
					this._activeFilters.AddAfter(linkedListNode, effect);
					return;
				}
			}
			this._activeFilters.AddLast(effect);
		}

		public void BeginCapture()
		{
			if (this._activeFilterCount == 0 && this.OnPostDraw == null)
			{
				this._captureThisFrame = false;
				return;
			}
			this._captureThisFrame = true;
			Main.instance.GraphicsDevice.SetRenderTarget(Main.screenTarget);
			Main.instance.GraphicsDevice.Clear(Color.Black);
		}

		public void Update(GameTime gameTime)
		{
			LinkedListNode<Filter> linkedListNode = this._activeFilters.First;
			int arg_17_0 = this._activeFilters.Count;
			int num = 0;
			while (linkedListNode != null)
			{
				Filter value = linkedListNode.Value;
				LinkedListNode<Filter> next = linkedListNode.Next;
				bool flag = false;
				if (value.Priority >= this._priorityThreshold)
				{
					num++;
					if (num > this._activeFilterCount - this._filterLimit)
					{
						value.Update(gameTime);
						flag = true;
					}
				}
				if (value.Active && flag)
				{
					value.Opacity = Math.Min(value.Opacity + (float)gameTime.ElapsedGameTime.TotalSeconds * 1f, 1f);
				}
				else
				{
					value.Opacity = Math.Max(value.Opacity - (float)gameTime.ElapsedGameTime.TotalSeconds * 1f, 0f);
				}
				if (!value.Active && value.Opacity == 0f)
				{
					if (value.Priority >= this._priorityThreshold)
					{
						this._activeFilterCount--;
					}
					this._activeFilters.Remove(linkedListNode);
				}
				linkedListNode = next;
			}
		}

		public void EndCapture()
		{
			if (!this._captureThisFrame)
			{
				return;
			}
			LinkedListNode<Filter> linkedListNode = this._activeFilters.First;
			int arg_20_0 = this._activeFilters.Count;
			Filter filter = null;
			RenderTarget2D renderTarget2D = Main.screenTarget;
			GraphicsDevice graphicsDevice = Main.instance.GraphicsDevice;
			int num = 0;
			if (Main.player[Main.myPlayer].gravDir == -1f)
			{
				RenderTarget2D renderTarget = Main.screenTargetSwap;
				graphicsDevice.SetRenderTarget(renderTarget);
				graphicsDevice.Clear(Color.Black);
				Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Matrix.Invert(Main.Transform));
				Main.spriteBatch.Draw(renderTarget2D, Vector2.Zero, Color.White);
				Main.spriteBatch.End();
				renderTarget2D = Main.screenTargetSwap;
			}
			while (linkedListNode != null)
			{
				Filter value = linkedListNode.Value;
				LinkedListNode<Filter> next = linkedListNode.Next;
				if (value.Priority >= this._priorityThreshold)
				{
					num++;
					if (num > this._activeFilterCount - this._filterLimit && value.IsVisible())
					{
						if (filter != null)
						{
							RenderTarget2D renderTarget;
							if (renderTarget2D == Main.screenTarget)
							{
								renderTarget = Main.screenTargetSwap;
							}
							else
							{
								renderTarget = Main.screenTarget;
							}
							graphicsDevice.SetRenderTarget(renderTarget);
							graphicsDevice.Clear(Color.Black);
							Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
							filter.Apply();
							Main.spriteBatch.Draw(renderTarget2D, Vector2.Zero, Main.bgColor);
							Main.spriteBatch.End();
							if (renderTarget2D == Main.screenTarget)
							{
								renderTarget2D = Main.screenTargetSwap;
							}
							else
							{
								renderTarget2D = Main.screenTarget;
							}
						}
						filter = value;
					}
				}
				linkedListNode = next;
			}
			graphicsDevice.SetRenderTarget(null);
			graphicsDevice.Clear(Color.Black);
			if (Main.player[Main.myPlayer].gravDir == -1f)
			{
				Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.Transform);
			}
			else
			{
				Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
			}
			if (filter != null)
			{
				filter.Apply();
				Main.spriteBatch.Draw(renderTarget2D, Vector2.Zero, Main.bgColor);
			}
			else
			{
				Main.spriteBatch.Draw(renderTarget2D, Vector2.Zero, Color.White);
			}
			Main.spriteBatch.End();
			for (int i = 0; i < 8; i++)
			{
				graphicsDevice.Textures[i] = null;
			}
			if (this.OnPostDraw != null)
			{
				this.OnPostDraw();
			}
		}

		public bool HasActiveFilter()
		{
			return this._activeFilters.Count != 0;
		}

		public bool CanCapture()
		{
			return this.HasActiveFilter() || this.OnPostDraw != null;
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
