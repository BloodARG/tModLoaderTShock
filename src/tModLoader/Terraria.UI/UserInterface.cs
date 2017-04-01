using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria.GameInput;

namespace Terraria.UI
{
	public class UserInterface
	{
		private const double DOUBLE_CLICK_TIME = 500.0;
		private const double STATE_CHANGE_CLICK_DISABLE_TIME = 200.0;
		private const int MAX_HISTORY_SIZE = 32;
		private const int HISTORY_PRUNE_SIZE = 4;
		public static UserInterface ActiveInstance = new UserInterface();
		private List<UIState> _history = new List<UIState>();
		public Vector2 MousePosition;
		private bool _wasMouseDown;
		private bool _wasMouseRightDown;
		private UIElement _lastElementHover;
		private UIElement _lastElementDown;
		private UIElement _lastElementRightDown;
		private UIElement _lastElementClicked;
		private UIElement _lastElementRightClicked;
		private double _lastMouseDownTime;
		private double _lastMouseRightDownTime;
		private double _clickDisabledTimeRemaining;
		public bool IsVisible;
		private UIState _currentState;

		public UIState CurrentState
		{
			get
			{
				return this._currentState;
			}
		}

		public void ResetLasts()
		{
			this._lastElementHover = null;
			this._lastElementDown = null;
			this._lastElementRightDown = null;
			this._lastElementClicked = null;
			this._lastElementRightClicked = null;
		}

		public UserInterface()
		{
			UserInterface.ActiveInstance = this;
		}

		public void Use()
		{
			if (UserInterface.ActiveInstance != this)
			{
				UserInterface.ActiveInstance = this;
				this.Recalculate();
				return;
			}
			UserInterface.ActiveInstance = this;
		}

		private void ResetState()
		{
#if CLIENT
			this.MousePosition = new Vector2((float)Main.mouseX, (float)Main.mouseY);
			this._wasMouseDown = Main.mouseLeft;
			this._wasMouseRightDown = Main.mouseRight;
			if (this._lastElementHover != null)
			{
				this._lastElementHover.MouseOut(new UIMouseEvent(this._lastElementHover, this.MousePosition));
			}
#endif
			this._lastElementHover = null;
			this._lastElementDown = null;
			this._lastElementRightDown = null;
			this._lastElementClicked = null;
			this._lastElementRightClicked = null;
			this._lastMouseDownTime = 0.0;
			this._lastMouseRightDownTime = 0.0;
			this._clickDisabledTimeRemaining = Math.Max(this._clickDisabledTimeRemaining, 200.0);
		}

		public void Update(GameTime time)
		{
			if (this._currentState != null)
			{
				this.MousePosition = new Vector2((float)Main.mouseX, (float)Main.mouseY);
				bool flag = Main.mouseLeft && Main.hasFocus;
				bool mouseRightDown = Main.mouseRight && Main.hasFocus;
				UIElement uIElement = Main.hasFocus ? this._currentState.GetElementAt(this.MousePosition) : null;
				this._clickDisabledTimeRemaining = Math.Max(0.0, this._clickDisabledTimeRemaining - time.ElapsedGameTime.TotalMilliseconds);
				bool flag2 = this._clickDisabledTimeRemaining > 0.0;
				if (uIElement != this._lastElementHover)
				{
					if (this._lastElementHover != null)
					{
						this._lastElementHover.MouseOut(new UIMouseEvent(this._lastElementHover, this.MousePosition));
					}
					if (uIElement != null)
					{
						uIElement.MouseOver(new UIMouseEvent(uIElement, this.MousePosition));
					}
					this._lastElementHover = uIElement;
				}
				if (flag && !this._wasMouseDown && uIElement != null && !flag2)
				{
					this._lastElementDown = uIElement;
					uIElement.MouseDown(new UIMouseEvent(uIElement, this.MousePosition));
					if (this._lastElementClicked == uIElement && time.TotalGameTime.TotalMilliseconds - this._lastMouseDownTime < 500.0)
					{
						uIElement.DoubleClick(new UIMouseEvent(uIElement, this.MousePosition));
						this._lastElementClicked = null;
					}
					this._lastMouseDownTime = time.TotalGameTime.TotalMilliseconds;
				}
				else if (!flag && this._wasMouseDown && this._lastElementDown != null && !flag2)
				{
					UIElement lastElementDown = this._lastElementDown;
					if (lastElementDown.ContainsPoint(this.MousePosition))
					{
						lastElementDown.Click(new UIMouseEvent(lastElementDown, this.MousePosition));
						this._lastElementClicked = this._lastElementDown;
					}
					lastElementDown.MouseUp(new UIMouseEvent(lastElementDown, this.MousePosition));
					this._lastElementDown = null;
				}
				// tModLoader added functionality, right click Events
				if (mouseRightDown && !this._wasMouseRightDown && uIElement != null && !flag2)
				{
					this._lastElementRightDown = uIElement;
					uIElement.RightMouseDown(new UIMouseEvent(uIElement, this.MousePosition));
					if (this._lastElementRightClicked == uIElement && time.TotalGameTime.TotalMilliseconds - this._lastMouseRightDownTime < 500.0)
					{
						uIElement.RightDoubleClick(new UIMouseEvent(uIElement, this.MousePosition));
						this._lastElementRightClicked = null;
					}
					this._lastMouseRightDownTime = time.TotalGameTime.TotalMilliseconds;
				}
				else if (!mouseRightDown && this._wasMouseRightDown && this._lastElementRightDown != null && !flag2)
				{
					UIElement lastElementRightDown = this._lastElementRightDown;
					if (lastElementRightDown.ContainsPoint(this.MousePosition))
					{
						lastElementRightDown.RightClick(new UIMouseEvent(lastElementRightDown, this.MousePosition));
						this._lastElementRightClicked = this._lastElementRightDown;
					}
					lastElementRightDown.RightMouseUp(new UIMouseEvent(lastElementRightDown, this.MousePosition));
					this._lastElementRightDown = null;
				}
				if (PlayerInput.ScrollWheelDeltaForUI != 0)
				{
					if (uIElement != null)
					{
						uIElement.ScrollWheel(new UIScrollWheelEvent(uIElement, this.MousePosition, PlayerInput.ScrollWheelDeltaForUI));
					}
					PlayerInput.ScrollWheelDeltaForUI = 0;
				}
				this._wasMouseDown = flag;
				this._wasMouseRightDown = mouseRightDown;
				if (this._currentState != null)
				{
					this._currentState.Update(time);
				}
			}
		}

		public void Draw(SpriteBatch spriteBatch, GameTime time)
		{
			this.Use();
			if (this._currentState != null)
			{
				this._currentState.Draw(spriteBatch);
			}
		}

		public void SetState(UIState state)
		{
			this.AddToHistory(state);
			if (this._currentState != null)
			{
				this._currentState.Deactivate();
			}
			this._currentState = state;
			this.ResetState();
			if (state != null)
			{
				state.Activate();
				state.Recalculate();
			}
		}

		public void GoBack()
		{
			if (this._history.Count < 2)
			{
				return;
			}
			UIState state = this._history[this._history.Count - 2];
			this._history.RemoveRange(this._history.Count - 2, 2);
			this.SetState(state);
		}

		private void AddToHistory(UIState state)
		{
			this._history.Add(state);
			if (this._history.Count > 32)
			{
				this._history.RemoveRange(0, 4);
			}
		}

		public void Recalculate()
		{
			if (this._currentState != null)
			{
				this._currentState.Recalculate();
			}
		}

		public CalculatedStyle GetDimensions()
		{
			return new CalculatedStyle(0f, 0f, (float)Main.screenWidth, (float)Main.screenHeight);
		}

		internal void RefreshState()
		{
			if (this._currentState != null)
			{
				this._currentState.Deactivate();
			}
			this.ResetState();
			this._currentState.Activate();
			this._currentState.Recalculate();
		}

		public bool IsElementUnderMouse()
		{
			return this.IsVisible && this._lastElementHover != null && !(this._lastElementHover is UIState);
		}
	}
}
