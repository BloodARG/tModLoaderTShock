using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria.GameContent.UI.Elements;

namespace Terraria.UI
{
	public class UIElement : IComparable
	{
		public delegate void MouseEvent(UIMouseEvent evt, UIElement listeningElement);

		public delegate void ScrollWheelEvent(UIScrollWheelEvent evt, UIElement listeningElement);

		public string Id = "";
		public UIElement Parent;
		protected List<UIElement> Elements = new List<UIElement>();
		public StyleDimension Top;
		public StyleDimension Left;
		public StyleDimension Width;
		public StyleDimension Height;
		public StyleDimension MaxWidth = StyleDimension.Fill;
		public StyleDimension MaxHeight = StyleDimension.Fill;
		public StyleDimension MinWidth = StyleDimension.Empty;
		public StyleDimension MinHeight = StyleDimension.Empty;
		private bool _isInitialized;
		public bool OverflowHidden;
		public float PaddingTop;
		public float PaddingLeft;
		public float PaddingRight;
		public float PaddingBottom;
		public float MarginTop;
		public float MarginLeft;
		public float MarginRight;
		public float MarginBottom;
		public float HAlign;
		public float VAlign;
		private CalculatedStyle _innerDimensions;
		private CalculatedStyle _dimensions;
		private CalculatedStyle _outerDimensions;
		private static RasterizerState _overflowHiddenRasterizerState;
		protected bool _useImmediateMode;
		private SnapPoint _snapPoint;
		private bool _isMouseHovering;

		public event UIElement.MouseEvent OnMouseDown;
		public event UIElement.MouseEvent OnMouseUp;
		public event UIElement.MouseEvent OnClick;
		public event UIElement.MouseEvent OnMouseOver;
		public event UIElement.MouseEvent OnMouseOut;
		public event UIElement.MouseEvent OnDoubleClick;
		public event UIElement.MouseEvent OnRightMouseDown;
		public event UIElement.MouseEvent OnRightMouseUp;
		public event UIElement.MouseEvent OnRightClick;
		public event UIElement.MouseEvent OnRightDoubleClick;
		public event UIElement.ScrollWheelEvent OnScrollWheel;

		public bool IsMouseHovering
		{
			get
			{
				return this._isMouseHovering;
			}
		}

		public UIElement()
		{
			if (UIElement._overflowHiddenRasterizerState == null)
			{
				UIElement._overflowHiddenRasterizerState = new RasterizerState
				{
					CullMode = CullMode.None,
					ScissorTestEnable = true
				};
			}
		}

		public void SetSnapPoint(string name, int id, Vector2? anchor = null, Vector2? offset = null)
		{
			if (!anchor.HasValue)
			{
				anchor = new Vector2?(new Vector2(0.5f));
			}
			if (!offset.HasValue)
			{
				offset = new Vector2?(Vector2.Zero);
			}
			this._snapPoint = new SnapPoint(name, id, anchor.Value, offset.Value);
		}

		public bool GetSnapPoint(out SnapPoint point)
		{
			point = this._snapPoint;
			if (this._snapPoint != null)
			{
				this._snapPoint.Calculate(this);
			}
			return this._snapPoint != null;
		}

		protected virtual void DrawSelf(SpriteBatch spriteBatch)
		{
		}

		protected virtual void DrawChildren(SpriteBatch spriteBatch)
		{
			foreach (UIElement current in this.Elements)
			{
				current.Draw(spriteBatch);
			}
		}

		public void Append(UIElement element)
		{
			element.Remove();
			element.Parent = this;
			this.Elements.Add(element);
			element.Recalculate();
		}

		public void Remove()
		{
			if (this.Parent != null)
			{
				this.Parent.RemoveChild(this);
			}
		}

		public void RemoveChild(UIElement child)
		{
			this.Elements.Remove(child);
			child.Parent = null;
		}

		public void RemoveAllChildren()
		{
			foreach (UIElement current in this.Elements)
			{
				current.Parent = null;
			}
			this.Elements.Clear();
		}

		public bool HasChild(UIElement child)
		{
			return Elements.Contains(child);
		}

		public virtual void Draw(SpriteBatch spriteBatch)
		{
			bool overflowHidden = this.OverflowHidden;
			bool useImmediateMode = this._useImmediateMode;
			RasterizerState rasterizerState = spriteBatch.GraphicsDevice.RasterizerState;
			Rectangle scissorRectangle = spriteBatch.GraphicsDevice.ScissorRectangle;
			if (useImmediateMode)
			{
				spriteBatch.End();
				spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, UIElement._overflowHiddenRasterizerState);
				this.DrawSelf(spriteBatch);
				spriteBatch.End();
				spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, UIElement._overflowHiddenRasterizerState);
			}
			else
			{
				this.DrawSelf(spriteBatch);
			}
			if (overflowHidden)
			{
				spriteBatch.End();
				Rectangle clippingRectangle = this.GetClippingRectangle(spriteBatch);
				spriteBatch.GraphicsDevice.ScissorRectangle = clippingRectangle;
				spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, UIElement._overflowHiddenRasterizerState);
			}
			this.DrawChildren(spriteBatch);
			if (overflowHidden)
			{
				spriteBatch.End();
				spriteBatch.GraphicsDevice.ScissorRectangle = scissorRectangle;
				spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, rasterizerState);
			}
		}

		public virtual void Update(GameTime gameTime)
		{
			foreach (UIElement current in this.Elements)
			{
				current.Update(gameTime);
			}
		}

		public Rectangle GetClippingRectangle(SpriteBatch spriteBatch)
		{
			Rectangle result = new Rectangle((int)this._innerDimensions.X, (int)this._innerDimensions.Y, (int)this._innerDimensions.Width, (int)this._innerDimensions.Height);
			int width = spriteBatch.GraphicsDevice.Viewport.Width;
			int height = spriteBatch.GraphicsDevice.Viewport.Height;
			result.X = Utils.Clamp<int>(result.X, 0, width);
			result.Y = Utils.Clamp<int>(result.Y, 0, height);
			result.Width = Utils.Clamp<int>(result.Width, 0, width - result.X);
			result.Height = Utils.Clamp<int>(result.Height, 0, height - result.Y);
			return result;
		}

		public virtual List<SnapPoint> GetSnapPoints()
		{
			List<SnapPoint> list = new List<SnapPoint>();
			SnapPoint item;
			if (this.GetSnapPoint(out item))
			{
				list.Add(item);
			}
			foreach (UIElement current in this.Elements)
			{
				list.AddRange(current.GetSnapPoints());
			}
			return list;
		}

		public virtual void Recalculate()
		{
			CalculatedStyle calculatedStyle;
			if (this.Parent != null)
			{
				calculatedStyle = this.Parent.GetInnerDimensions();
			}
			else
			{
				calculatedStyle = UserInterface.ActiveInstance.GetDimensions();
			}
			if (this.Parent != null && this.Parent is UIList)
			{
				calculatedStyle.Height = 3.40282347E+38f;
			}
			CalculatedStyle calculatedStyle2;
			calculatedStyle2.X = this.Left.GetValue(calculatedStyle.Width) + calculatedStyle.X;
			calculatedStyle2.Y = this.Top.GetValue(calculatedStyle.Height) + calculatedStyle.Y;
			float value = this.MinWidth.GetValue(calculatedStyle.Width);
			float value2 = this.MaxWidth.GetValue(calculatedStyle.Width);
			float value3 = this.MinHeight.GetValue(calculatedStyle.Height);
			float value4 = this.MaxHeight.GetValue(calculatedStyle.Height);
			calculatedStyle2.Width = MathHelper.Clamp(this.Width.GetValue(calculatedStyle.Width), value, value2);
			calculatedStyle2.Height = MathHelper.Clamp(this.Height.GetValue(calculatedStyle.Height), value3, value4);
			calculatedStyle2.Width += this.MarginLeft + this.MarginRight;
			calculatedStyle2.Height += this.MarginTop + this.MarginBottom;
			calculatedStyle2.X += calculatedStyle.Width * this.HAlign - calculatedStyle2.Width * this.HAlign;
			calculatedStyle2.Y += calculatedStyle.Height * this.VAlign - calculatedStyle2.Height * this.VAlign;
			this._outerDimensions = calculatedStyle2;
			calculatedStyle2.X += this.MarginLeft;
			calculatedStyle2.Y += this.MarginTop;
			calculatedStyle2.Width -= this.MarginLeft + this.MarginRight;
			calculatedStyle2.Height -= this.MarginTop + this.MarginBottom;
			this._dimensions = calculatedStyle2;
			calculatedStyle2.X += this.PaddingLeft;
			calculatedStyle2.Y += this.PaddingTop;
			calculatedStyle2.Width -= this.PaddingLeft + this.PaddingRight;
			calculatedStyle2.Height -= this.PaddingTop + this.PaddingBottom;
			this._innerDimensions = calculatedStyle2;
			this.RecalculateChildren();
		}

		public UIElement GetElementAt(Vector2 point)
		{
			UIElement uIElement = null;
			foreach (UIElement current in this.Elements)
			{
				if (current.ContainsPoint(point))
				{
					uIElement = current;
					break;
				}
			}
			if (uIElement != null)
			{
				return uIElement.GetElementAt(point);
			}
			if (this.ContainsPoint(point))
			{
				return this;
			}
			return null;
		}

		public virtual bool ContainsPoint(Vector2 point)
		{
			return point.X > this._dimensions.X && point.Y > this._dimensions.Y && point.X < this._dimensions.X + this._dimensions.Width && point.Y < this._dimensions.Y + this._dimensions.Height;
		}

		public void SetPadding(float pixels)
		{
			this.PaddingBottom = pixels;
			this.PaddingLeft = pixels;
			this.PaddingRight = pixels;
			this.PaddingTop = pixels;
		}

		public virtual void RecalculateChildren()
		{
			foreach (UIElement current in this.Elements)
			{
				current.Recalculate();
			}
		}

		public CalculatedStyle GetInnerDimensions()
		{
			return this._innerDimensions;
		}

		public CalculatedStyle GetDimensions()
		{
			return this._dimensions;
		}

		public CalculatedStyle GetOuterDimensions()
		{
			return this._outerDimensions;
		}

		public void CopyStyle(UIElement element)
		{
			this.Top = element.Top;
			this.Left = element.Left;
			this.Width = element.Width;
			this.Height = element.Height;
			this.PaddingBottom = element.PaddingBottom;
			this.PaddingLeft = element.PaddingLeft;
			this.PaddingRight = element.PaddingRight;
			this.PaddingTop = element.PaddingTop;
			this.HAlign = element.HAlign;
			this.VAlign = element.VAlign;
			this.MinWidth = element.MinWidth;
			this.MaxWidth = element.MaxWidth;
			this.MinHeight = element.MinHeight;
			this.MaxHeight = element.MaxHeight;
			this.Recalculate();
		}

		public virtual void MouseDown(UIMouseEvent evt)
		{
			if (this.OnMouseDown != null)
			{
				this.OnMouseDown(evt, this);
			}
			if (this.Parent != null)
			{
				this.Parent.MouseDown(evt);
			}
		}

		public virtual void MouseUp(UIMouseEvent evt)
		{
			if (this.OnMouseUp != null)
			{
				this.OnMouseUp(evt, this);
			}
			if (this.Parent != null)
			{
				this.Parent.MouseUp(evt);
			}
		}

		public virtual void MouseOver(UIMouseEvent evt)
		{
			this._isMouseHovering = true;
			if (this.OnMouseOver != null)
			{
				this.OnMouseOver(evt, this);
			}
			if (this.Parent != null)
			{
				this.Parent.MouseOver(evt);
			}
		}

		public virtual void MouseOut(UIMouseEvent evt)
		{
			this._isMouseHovering = false;
			if (this.OnMouseOut != null)
			{
				this.OnMouseOut(evt, this);
			}
			if (this.Parent != null)
			{
				this.Parent.MouseOut(evt);
			}
		}

		public virtual void Click(UIMouseEvent evt)
		{
			if (this.OnClick != null)
			{
				this.OnClick(evt, this);
			}
			if (this.Parent != null)
			{
				this.Parent.Click(evt);
			}
		}

		public virtual void DoubleClick(UIMouseEvent evt)
		{
			if (this.OnDoubleClick != null)
			{
				this.OnDoubleClick(evt, this);
			}
			if (this.Parent != null)
			{
				this.Parent.DoubleClick(evt);
			}
		}

		public virtual void RightMouseDown(UIMouseEvent evt)
		{
			if (this.OnRightMouseDown != null)
			{
				this.OnRightMouseDown(evt, this);
			}
			if (this.Parent != null)
			{
				this.Parent.RightMouseDown(evt);
			}
		}

		public virtual void RightMouseUp(UIMouseEvent evt)
		{
			if (this.OnRightMouseUp != null)
			{
				this.OnRightMouseUp(evt, this);
			}
			if (this.Parent != null)
			{
				this.Parent.RightMouseUp(evt);
			}
		}

		public virtual void RightClick(UIMouseEvent evt)
		{
			if (this.OnRightClick != null)
			{
				this.OnRightClick(evt, this);
			}
			if (this.Parent != null)
			{
				this.Parent.RightClick(evt);
			}
		}

		public virtual void RightDoubleClick(UIMouseEvent evt)
		{
			if (this.OnRightDoubleClick != null)
			{
				this.OnRightDoubleClick(evt, this);
			}
			if (this.Parent != null)
			{
				this.Parent.RightDoubleClick(evt);
			}
		}

		public virtual void ScrollWheel(UIScrollWheelEvent evt)
		{
			if (this.OnScrollWheel != null)
			{
				this.OnScrollWheel(evt, this);
			}
			if (this.Parent != null)
			{
				this.Parent.ScrollWheel(evt);
			}
		}

		public void Activate()
		{
			if (!this._isInitialized)
			{
				this.Initialize();
			}
			this.OnActivate();
			foreach (UIElement current in this.Elements)
			{
				current.Activate();
			}
		}

		public virtual void OnActivate()
		{
		}

		public void Deactivate()
		{
			this.OnDeactivate();
			foreach (UIElement current in this.Elements)
			{
				current.Deactivate();
			}
		}

		public virtual void OnDeactivate()
		{
		}

		public void Initialize()
		{
			this.OnInitialize();
			this._isInitialized = true;
		}

		public virtual void OnInitialize()
		{
		}

		public virtual int CompareTo(object obj)
		{
			return 0;
		}

		public virtual bool PassFilters()
		{
			throw new NotImplementedException();
		}
	}
}
