using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.Graphics;
using Terraria.Localization;
using Terraria.UI;
using Terraria.UI.Gamepad;

namespace Terraria.GameContent.UI.States
{
	public class UIVirtualKeyboard : UIState
	{
		public delegate void KeyboardSubmitEvent(string text);

		public enum KeyState
		{
			Default,
			Symbol,
			Shift
		}

		private const string DEFAULT_KEYS = "1234567890qwertyuiopasdfghjkl'zxcvbnm,.?";
		private const string SHIFT_KEYS = "1234567890QWERTYUIOPASDFGHJKL'ZXCVBNM,.?";
		private const string SYMBOL_KEYS = "1234567890!@#$%^&*()-_+=/\\{}[]<>;:\"`|~£¥";
		private const float KEY_SPACING = 4f;
		private const float KEY_WIDTH = 48f;
		private const float KEY_HEIGHT = 37f;
		private static UIVirtualKeyboard _currentInstance;
		private static string _cancelCacheSign = "";
		private static string _cancelCacheChest = "";
		private UITextPanel<object>[] _keyList = new UITextPanel<object>[50];
		private UITextPanel<object> _shiftButton;
		private UITextPanel<object> _symbolButton;
		private UITextBox _textBox;
		private UITextPanel<LocalizedText> _submitButton;
		private UITextPanel<LocalizedText> _cancelButton;
		private UIText _label;
		private UITextPanel<object> _enterButton;
		private UITextPanel<object> _spacebarButton;
		private UITextPanel<object> _restoreButton;
		private Texture2D _textureShift;
		private Texture2D _textureBackspace;
		private Color _internalBorderColor = new Color(89, 116, 213);
		private Color _internalBorderColorSelected = Main.OurFavoriteColor;
		private UITextPanel<LocalizedText> _submitButton2;
		private UITextPanel<LocalizedText> _cancelButton2;
		private UIElement outerLayer1;
		private UIElement outerLayer2;
		private bool _allowEmpty;
		private UIVirtualKeyboard.KeyState _keyState;
		private UIVirtualKeyboard.KeyboardSubmitEvent _submitAction;
		private Action _cancelAction;
		private int _lastOffsetDown;
		public static int OffsetDown = 0;
		private int _keyboardContext;
		private bool _edittingSign;
		private bool _edittingChest;
		private float _textBoxHeight;
		private float _labelHeight;
		private bool _canSubmit;

		public string Text
		{
			get
			{
				return this._textBox.Text;
			}
			set
			{
				this._textBox.SetText(value);
				this.ValidateText();
			}
		}

		public static bool CanSubmit
		{
			get
			{
				return UIVirtualKeyboard._currentInstance != null && UIVirtualKeyboard._currentInstance._canSubmit;
			}
		}

		public static int KeyboardContext
		{
			get
			{
				if (UIVirtualKeyboard._currentInstance == null)
				{
					return -1;
				}
				return UIVirtualKeyboard._currentInstance._keyboardContext;
			}
		}

		public UIVirtualKeyboard(string labelText, string startingText, UIVirtualKeyboard.KeyboardSubmitEvent submitAction, Action cancelAction, int inputMode = 0, bool allowEmpty = false)
		{
			this._keyboardContext = inputMode;
			this._allowEmpty = allowEmpty;
			UIVirtualKeyboard.OffsetDown = 0;
			this._lastOffsetDown = 0;
			this._edittingSign = (this._keyboardContext == 1);
			this._edittingChest = (this._keyboardContext == 2);
			UIVirtualKeyboard._currentInstance = this;
			this._submitAction = submitAction;
			this._cancelAction = cancelAction;
			this._textureShift = TextureManager.Load("Images/UI/VK_Shift");
			this._textureBackspace = TextureManager.Load("Images/UI/VK_Backspace");
			this.Top.Pixels = (float)this._lastOffsetDown;
			float num = (float)(-5000 * this._edittingSign.ToInt());
			float num2 = 255f;
			float precent = 0f;
			float num3 = 516f;
			UIElement uIElement = new UIElement();
			uIElement.Width.Pixels = num3 + 8f + 16f;
			uIElement.Top.Precent = precent;
			uIElement.Top.Pixels = num2;
			uIElement.Height.Pixels = 266f;
			uIElement.HAlign = 0.5f;
			uIElement.SetPadding(0f);
			this.outerLayer1 = uIElement;
			UIElement uIElement2 = new UIElement();
			uIElement2.Width.Pixels = num3 + 8f + 16f;
			uIElement2.Top.Precent = precent;
			uIElement2.Top.Pixels = num2;
			uIElement2.Height.Pixels = 266f;
			uIElement2.HAlign = 0.5f;
			uIElement2.SetPadding(0f);
			this.outerLayer2 = uIElement2;
			UIPanel uIPanel = new UIPanel();
			uIPanel.Width.Precent = 1f;
			uIPanel.Height.Pixels = 225f;
			uIPanel.BackgroundColor = new Color(23, 33, 69) * 0.7f;
			uIElement.Append(uIPanel);
			float num4 = -49f;
			this._textBox = new UITextBox("", 0.78f, true);
			this._textBox.BackgroundColor = Color.Transparent;
			this._textBox.BorderColor = Color.Transparent;
			this._textBox.HAlign = 0.5f;
			this._textBox.Width.Pixels = num3;
			this._textBox.Top.Pixels = num4 + num2 - 10f + num;
			this._textBox.Top.Precent = precent;
			this._textBox.Height.Pixels = 37f;
			base.Append(this._textBox);
			for (int i = 0; i < 10; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					int index = j * 10 + i;
					UITextPanel<object> uITextPanel = this.CreateKeyboardButton("1234567890qwertyuiopasdfghjkl'zxcvbnm,.?"[index].ToString(), i, j, 1, true);
					uITextPanel.OnClick += new UIElement.MouseEvent(this.TypeText);
					uIPanel.Append(uITextPanel);
				}
			}
			this._shiftButton = this.CreateKeyboardButton("", 0, 4, 1, false);
			this._shiftButton.PaddingLeft = 0f;
			this._shiftButton.PaddingRight = 0f;
			this._shiftButton.PaddingBottom = (this._shiftButton.PaddingTop = 0f);
			this._shiftButton.BackgroundColor = new Color(63, 82, 151) * 0.7f;
			this._shiftButton.BorderColor = this._internalBorderColor * 0.7f;
			this._shiftButton.OnMouseOver += delegate(UIMouseEvent evt, UIElement listeningElement)
			{
				this._shiftButton.BorderColor = this._internalBorderColorSelected;
				if (this._keyState != UIVirtualKeyboard.KeyState.Shift)
				{
					this._shiftButton.BackgroundColor = new Color(73, 94, 171);
				}
			};
			this._shiftButton.OnMouseOut += delegate(UIMouseEvent evt, UIElement listeningElement)
			{
				this._shiftButton.BorderColor = this._internalBorderColor * 0.7f;
				if (this._keyState != UIVirtualKeyboard.KeyState.Shift)
				{
					this._shiftButton.BackgroundColor = new Color(63, 82, 151) * 0.7f;
				}
			};
			this._shiftButton.OnClick += delegate(UIMouseEvent evt, UIElement listeningElement)
			{
				Main.PlaySound(12, -1, -1, 1, 1f, 0f);
				this.SetKeyState((this._keyState == UIVirtualKeyboard.KeyState.Shift) ? UIVirtualKeyboard.KeyState.Default : UIVirtualKeyboard.KeyState.Shift);
			};
			UIImage uIImage = new UIImage(this._textureShift);
			uIImage.HAlign = 0.5f;
			uIImage.VAlign = 0.5f;
			uIImage.ImageScale = 0.85f;
			this._shiftButton.Append(uIImage);
			uIPanel.Append(this._shiftButton);
			this._symbolButton = this.CreateKeyboardButton("@%", 1, 4, 1, false);
			this._symbolButton.PaddingLeft = 0f;
			this._symbolButton.PaddingRight = 0f;
			this._symbolButton.BackgroundColor = new Color(63, 82, 151) * 0.7f;
			this._symbolButton.BorderColor = this._internalBorderColor * 0.7f;
			this._symbolButton.OnMouseOver += delegate(UIMouseEvent evt, UIElement listeningElement)
			{
				this._symbolButton.BorderColor = this._internalBorderColorSelected;
				if (this._keyState != UIVirtualKeyboard.KeyState.Symbol)
				{
					this._symbolButton.BackgroundColor = new Color(73, 94, 171);
				}
			};
			this._symbolButton.OnMouseOut += delegate(UIMouseEvent evt, UIElement listeningElement)
			{
				this._symbolButton.BorderColor = this._internalBorderColor * 0.7f;
				if (this._keyState != UIVirtualKeyboard.KeyState.Symbol)
				{
					this._symbolButton.BackgroundColor = new Color(63, 82, 151) * 0.7f;
				}
			};
			this._symbolButton.OnClick += delegate(UIMouseEvent evt, UIElement listeningElement)
			{
				Main.PlaySound(12, -1, -1, 1, 1f, 0f);
				this.SetKeyState((this._keyState == UIVirtualKeyboard.KeyState.Symbol) ? UIVirtualKeyboard.KeyState.Default : UIVirtualKeyboard.KeyState.Symbol);
			};
			uIPanel.Append(this._symbolButton);
			this.BuildSpaceBarArea(uIPanel);
			this._submitButton = new UITextPanel<LocalizedText>((this._edittingSign || this._edittingChest) ? Language.GetText("UI.Save") : Language.GetText("UI.Submit"), 0.4f, true);
			this._submitButton.Height.Pixels = 37f;
			this._submitButton.Width.Precent = 0.4f;
			this._submitButton.HAlign = 1f;
			this._submitButton.VAlign = 1f;
			this._submitButton.PaddingLeft = 0f;
			this._submitButton.PaddingRight = 0f;
			this.ValidateText();
			this._submitButton.OnMouseOver += delegate(UIMouseEvent evt, UIElement listeningElement)
			{
				this.ValidateText();
			};
			this._submitButton.OnMouseOut += delegate(UIMouseEvent evt, UIElement listeningElement)
			{
				this.ValidateText();
			};
			this._submitButton.OnClick += delegate(UIMouseEvent evt, UIElement listeningElement)
			{
				string text = this.Text.Trim();
				if (text.Length > 0 || this._edittingSign || this._edittingChest || this._allowEmpty)
				{
					Main.PlaySound(10, -1, -1, 1, 1f, 0f);
					this._submitAction(text);
				}
			};
			uIElement.Append(this._submitButton);
			this._cancelButton = new UITextPanel<LocalizedText>(Language.GetText("UI.Cancel"), 0.4f, true);
			this.StyleKey<LocalizedText>(this._cancelButton, true);
			this._cancelButton.Height.Pixels = 37f;
			this._cancelButton.Width.Precent = 0.4f;
			this._cancelButton.VAlign = 1f;
			this._cancelButton.OnClick += delegate(UIMouseEvent evt, UIElement listeningElement)
			{
				Main.PlaySound(11, -1, -1, 1, 1f, 0f);
				this._cancelAction();
			};
			uIElement.Append(this._cancelButton);
			this._submitButton2 = new UITextPanel<LocalizedText>((this._edittingSign || this._edittingChest) ? Language.GetText("UI.Save") : Language.GetText("UI.Submit"), 0.72f, true);
			this._submitButton2.TextColor = Color.Silver;
			this._submitButton2.DrawPanel = false;
			this._submitButton2.Height.Pixels = 60f;
			this._submitButton2.Width.Precent = 0.4f;
			this._submitButton2.HAlign = 0.5f;
			this._submitButton2.VAlign = 0f;
			this._submitButton2.OnMouseOver += delegate(UIMouseEvent a, UIElement b)
			{
				((UITextPanel<LocalizedText>)b).TextScale = 0.85f;
				((UITextPanel<LocalizedText>)b).TextColor = Color.White;
			};
			this._submitButton2.OnMouseOut += delegate(UIMouseEvent a, UIElement b)
			{
				((UITextPanel<LocalizedText>)b).TextScale = 0.72f;
				((UITextPanel<LocalizedText>)b).TextColor = Color.Silver;
			};
			this._submitButton2.Top.Pixels = 50f;
			this._submitButton2.PaddingLeft = 0f;
			this._submitButton2.PaddingRight = 0f;
			this.ValidateText();
			this._submitButton2.OnMouseOver += delegate(UIMouseEvent evt, UIElement listeningElement)
			{
				this.ValidateText();
			};
			this._submitButton2.OnMouseOut += delegate(UIMouseEvent evt, UIElement listeningElement)
			{
				this.ValidateText();
			};
			this._submitButton2.OnClick += delegate(UIMouseEvent evt, UIElement listeningElement)
			{
				string text = this.Text.Trim();
				if (text.Length > 0 || this._edittingSign || this._edittingChest || this._allowEmpty)
				{
					Main.PlaySound(10, -1, -1, 1, 1f, 0f);
					this._submitAction(text);
				}
			};
			this.outerLayer2.Append(this._submitButton2);
			this._cancelButton2 = new UITextPanel<LocalizedText>(Language.GetText("UI.Cancel"), 0.72f, true);
			this._cancelButton2.TextColor = Color.Silver;
			this._cancelButton2.DrawPanel = false;
			this._cancelButton2.OnMouseOver += delegate(UIMouseEvent a, UIElement b)
			{
				((UITextPanel<LocalizedText>)b).TextScale = 0.85f;
				((UITextPanel<LocalizedText>)b).TextColor = Color.White;
			};
			this._cancelButton2.OnMouseOut += delegate(UIMouseEvent a, UIElement b)
			{
				((UITextPanel<LocalizedText>)b).TextScale = 0.72f;
				((UITextPanel<LocalizedText>)b).TextColor = Color.Silver;
			};
			this._cancelButton2.Height.Pixels = 60f;
			this._cancelButton2.Width.Precent = 0.4f;
			this._cancelButton2.Top.Pixels = 114f;
			this._cancelButton2.VAlign = 0f;
			this._cancelButton2.HAlign = 0.5f;
			this._cancelButton2.OnClick += delegate(UIMouseEvent evt, UIElement listeningElement)
			{
				Main.PlaySound(11, -1, -1, 1, 1f, 0f);
				this._cancelAction();
			};
			this.outerLayer2.Append(this._cancelButton2);
			UITextPanel<object> uITextPanel2 = this.CreateKeyboardButton("", 8, 4, 2, true);
			uITextPanel2.OnClick += delegate(UIMouseEvent evt, UIElement listeningElement)
			{
				Main.PlaySound(12, -1, -1, 1, 1f, 0f);
				this._textBox.Backspace();
				this.ValidateText();
			};
			uITextPanel2.PaddingLeft = 0f;
			uITextPanel2.PaddingRight = 0f;
			uITextPanel2.PaddingBottom = (uITextPanel2.PaddingTop = 0f);
			uITextPanel2.Append(new UIImage(this._textureBackspace)
				{
					HAlign = 0.5f,
					VAlign = 0.5f,
					ImageScale = 0.92f
				});
			uIPanel.Append(uITextPanel2);
			UIText uIText = new UIText(labelText, 0.75f, true);
			uIText.HAlign = 0.5f;
			uIText.Width.Pixels = num3;
			uIText.Top.Pixels = num4 - 37f - 4f + num2 + num;
			uIText.Top.Precent = precent;
			uIText.Height.Pixels = 37f;
			base.Append(uIText);
			this._label = uIText;
			base.Append(uIElement);
			this._textBox.SetTextMaxLength(this._edittingSign ? 99999 : 20);
			this.Text = startingText;
			if (this.Text.Length == 0)
			{
				this.SetKeyState(UIVirtualKeyboard.KeyState.Shift);
			}
			UIVirtualKeyboard.OffsetDown = 9999;
			this.UpdateOffsetDown();
		}

		private void BuildSpaceBarArea(UIPanel mainPanel)
		{
			Action createTheseTwo = delegate
			{
				bool flag2 = this.CanRestore();
				int x = flag2 ? 4 : 5;
				bool edittingSign = this._edittingSign;
				int num = (flag2 && edittingSign) ? 2 : 3;
				UITextPanel<object> uITextPanel = this.CreateKeyboardButton(Language.GetText("UI.SpaceButton"), 2, 4, (this._edittingSign || (this._edittingChest && flag2)) ? num : 6, true);
				uITextPanel.OnClick += delegate(UIMouseEvent evt, UIElement listeningElement)
				{
					Main.PlaySound(12, -1, -1, 1, 1f, 0f);
					this._textBox.Write(" ");
					this.ValidateText();
				};
				mainPanel.Append(uITextPanel);
				this._spacebarButton = uITextPanel;
				if (edittingSign)
				{
					UITextPanel<object> uITextPanel2 = this.CreateKeyboardButton(Language.GetText("UI.EnterButton"), x, 4, num, true);
					uITextPanel2.OnClick += delegate(UIMouseEvent evt, UIElement listeningElement)
					{
						Main.PlaySound(12, -1, -1, 1, 1f, 0f);
						this._textBox.Write(string.Concat('\n'));
						this.ValidateText();
					};
					mainPanel.Append(uITextPanel2);
					this._enterButton = uITextPanel2;
				}
			};
			createTheseTwo();
			bool flag = this.CanRestore();
			if (flag)
			{
				UITextPanel<object> restoreBar = this.CreateKeyboardButton(Language.GetText("UI.RestoreButton"), 6, 4, 2, true);
				restoreBar.OnClick += delegate(UIMouseEvent evt, UIElement listeningElement)
				{
					Main.PlaySound(12, -1, -1, 1, 1f, 0f);
					this.RestoreCancelledInput(this._keyboardContext);
					this.ValidateText();
					restoreBar.Remove();
					this._enterButton.Remove();
					this._spacebarButton.Remove();
					createTheseTwo();
				};
				mainPanel.Append(restoreBar);
				this._restoreButton = restoreBar;
			}
		}

		private bool CanRestore()
		{
			if (this._edittingSign)
			{
				return UIVirtualKeyboard._cancelCacheSign.Length > 0;
			}
			return this._edittingChest && UIVirtualKeyboard._cancelCacheChest.Length > 0;
		}

		private void TypeText(UIMouseEvent evt, UIElement listeningElement)
		{
			Main.PlaySound(12, -1, -1, 1, 1f, 0f);
			bool flag = this.Text.Length == 0;
			this._textBox.Write(((UITextPanel<object>)listeningElement).Text);
			this.ValidateText();
			if (flag && this.Text.Length > 0 && this._keyState == UIVirtualKeyboard.KeyState.Shift)
			{
				this.SetKeyState(UIVirtualKeyboard.KeyState.Default);
			}
		}

		public void SetKeyState(UIVirtualKeyboard.KeyState keyState)
		{
			UITextPanel<object> uITextPanel = null;
			switch (this._keyState)
			{
				case UIVirtualKeyboard.KeyState.Symbol:
					uITextPanel = this._symbolButton;
					break;
				case UIVirtualKeyboard.KeyState.Shift:
					uITextPanel = this._shiftButton;
					break;
			}
			if (uITextPanel != null)
			{
				if (uITextPanel.IsMouseHovering)
				{
					uITextPanel.BackgroundColor = new Color(73, 94, 171);
				}
				else
				{
					uITextPanel.BackgroundColor = new Color(63, 82, 151) * 0.7f;
				}
			}
			string text = null;
			UITextPanel<object> uITextPanel2 = null;
			switch (keyState)
			{
				case UIVirtualKeyboard.KeyState.Default:
					text = "1234567890qwertyuiopasdfghjkl'zxcvbnm,.?";
					break;
				case UIVirtualKeyboard.KeyState.Symbol:
					text = "1234567890!@#$%^&*()-_+=/\\{}[]<>;:\"`|~£¥";
					uITextPanel2 = this._symbolButton;
					break;
				case UIVirtualKeyboard.KeyState.Shift:
					text = "1234567890QWERTYUIOPASDFGHJKL'ZXCVBNM,.?";
					uITextPanel2 = this._shiftButton;
					break;
			}
			for (int i = 0; i < text.Length; i++)
			{
				this._keyList[i].SetText(text[i].ToString());
			}
			this._keyState = keyState;
			if (uITextPanel2 != null)
			{
				uITextPanel2.BackgroundColor = new Color(93, 114, 191);
			}
		}

		private void ValidateText()
		{
			if (this.Text.Trim().Length > 0 || this._edittingSign || this._edittingChest || this._allowEmpty)
			{
				this._canSubmit = true;
				this._submitButton.TextColor = Color.White;
				if (this._submitButton.IsMouseHovering)
				{
					this._submitButton.BackgroundColor = new Color(73, 94, 171);
					return;
				}
				this._submitButton.BackgroundColor = new Color(63, 82, 151) * 0.7f;
				return;
			}
			else
			{
				this._canSubmit = false;
				this._submitButton.TextColor = Color.Gray;
				if (this._submitButton.IsMouseHovering)
				{
					this._submitButton.BackgroundColor = new Color(180, 60, 60) * 0.85f;
					return;
				}
				this._submitButton.BackgroundColor = new Color(150, 40, 40) * 0.85f;
				return;
			}
		}

		private void StyleKey<T>(UITextPanel<T> button, bool external = false)
		{
			button.PaddingLeft = 0f;
			button.PaddingRight = 0f;
			button.BackgroundColor = new Color(63, 82, 151) * 0.7f;
			if (!external)
			{
				button.BorderColor = this._internalBorderColor * 0.7f;
			}
			button.OnMouseOver += delegate(UIMouseEvent evt, UIElement listeningElement)
			{
				((UITextPanel<T>)listeningElement).BackgroundColor = new Color(73, 94, 171) * 0.85f;
				if (!external)
				{
					((UITextPanel<T>)listeningElement).BorderColor = this._internalBorderColorSelected * 0.85f;
				}
			};
			button.OnMouseOut += delegate(UIMouseEvent evt, UIElement listeningElement)
			{
				((UITextPanel<T>)listeningElement).BackgroundColor = new Color(63, 82, 151) * 0.7f;
				if (!external)
				{
					((UITextPanel<T>)listeningElement).BorderColor = this._internalBorderColor * 0.7f;
				}
			};
		}

		private UITextPanel<object> CreateKeyboardButton(object text, int x, int y, int width = 1, bool style = true)
		{
			float num = 516f;
			UITextPanel<object> uITextPanel = new UITextPanel<object>(text, 0.4f, true);
			uITextPanel.Width.Pixels = 48f * (float)width + 4f * (float)(width - 1);
			uITextPanel.Height.Pixels = 37f;
			uITextPanel.Left.Precent = 0.5f;
			uITextPanel.Left.Pixels = 52f * (float)x - num * 0.5f;
			uITextPanel.Top.Pixels = 41f * (float)y;
			if (style)
			{
				this.StyleKey<object>(uITextPanel, false);
			}
			for (int i = 0; i < width; i++)
			{
				this._keyList[y * 10 + x + i] = uITextPanel;
			}
			return uITextPanel;
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			if (Main.gameMenu)
			{
				if (PlayerInput.UsingGamepad)
				{
					this.outerLayer2.Remove();
					if (!this.Elements.Contains(this.outerLayer1))
					{
						base.Append(this.outerLayer1);
					}
					this.outerLayer1.Activate();
					this.outerLayer2.Deactivate();
					this.Recalculate();
					this.RecalculateChildren();
					if (this._labelHeight != 0f)
					{
						this._textBox.Top.Pixels = this._textBoxHeight;
						this._label.Top.Pixels = this._labelHeight;
						this._textBox.Recalculate();
						this._label.Recalculate();
						this._labelHeight = (this._textBoxHeight = 0f);
						UserInterface.ActiveInstance.ResetLasts();
					}
				}
				else
				{
					this.outerLayer1.Remove();
					if (!this.Elements.Contains(this.outerLayer2))
					{
						base.Append(this.outerLayer2);
					}
					this.outerLayer2.Activate();
					this.outerLayer1.Deactivate();
					this.Recalculate();
					this.RecalculateChildren();
					if (this._textBoxHeight == 0f)
					{
						this._textBoxHeight = this._textBox.Top.Pixels;
						this._labelHeight = this._label.Top.Pixels;
						UITextBox expr_168_cp_0 = this._textBox;
						expr_168_cp_0.Top.Pixels = expr_168_cp_0.Top.Pixels + 50f;
						UIText expr_184_cp_0 = this._label;
						expr_184_cp_0.Top.Pixels = expr_184_cp_0.Top.Pixels + 50f;
						this._textBox.Recalculate();
						this._label.Recalculate();
						UserInterface.ActiveInstance.ResetLasts();
					}
				}
			}
			if (!Main.editSign && this._edittingSign)
			{
				IngameFancyUI.Close();
				return;
			}
			if (!Main.editChest && this._edittingChest)
			{
				IngameFancyUI.Close();
				return;
			}
			base.DrawSelf(spriteBatch);
			this.UpdateOffsetDown();
			UIVirtualKeyboard.OffsetDown = 0;
			this.SetupGamepadPoints(spriteBatch);
			PlayerInput.WritingText = true;
			string text = Main.GetInputText(this.Text);
			if (this._edittingSign && Main.inputTextEnter)
			{
				text += '\n';
			}
			else
			{
				if (this._edittingChest && Main.inputTextEnter)
				{
					ChestUI.RenameChestSubmit(Main.player[Main.myPlayer]);
					IngameFancyUI.Close();
					return;
				}
				if (Main.inputTextEnter && UIVirtualKeyboard.CanSubmit)
				{
					UIVirtualKeyboard.Submit();
				}
				else if (Main.inputTextEscape)
				{
					if (this._edittingSign)
					{
						Main.InputTextSignCancel();
					}
					if (this._edittingChest)
					{
						ChestUI.RenameChestCancel();
					}
					IngameFancyUI.Close();
					return;
				}
			}
			if (IngameFancyUI.CanShowVirtualKeyboard(this._keyboardContext))
			{
				if (text != this.Text)
				{
					this.Text = text;
				}
				if (this._edittingSign)
				{
					this.CopyTextToSign();
				}
				if (this._edittingChest)
				{
					this.CopyTextToChest();
				}
			}
			byte b = (byte)((255 + Main.tileColor.R * 2) / 3);
			Color value = new Color((int)b, (int)b, (int)b, 255);
			this._textBox.TextColor = Color.Lerp(Color.White, value, 0.2f);
			this._label.TextColor = Color.Lerp(Color.White, value, 0.2f);
		}

		private void UpdateOffsetDown()
		{
			int num = UIVirtualKeyboard.OffsetDown - this._lastOffsetDown;
			int num2 = num;
			if (Math.Abs(num) < 10)
			{
				num2 = num;
			}
			this._lastOffsetDown += num2;
			if (num2 == 0)
			{
				return;
			}
			this.Top.Pixels = this.Top.Pixels + (float)num2;
			this.Recalculate();
		}

		public override void OnActivate()
		{
			if (PlayerInput.UsingGamepadUI && this._restoreButton != null)
			{
				UILinkPointNavigator.ChangePoint(3002);
			}
		}

		public override void OnDeactivate()
		{
			base.OnDeactivate();
			PlayerInput.WritingText = false;
			UILinkPointNavigator.Shortcuts.FANCYUI_SPECIAL_INSTRUCTIONS = 0;
		}

		private void SetupGamepadPoints(SpriteBatch spriteBatch)
		{
			UILinkPointNavigator.Shortcuts.BackButtonCommand = 6;
			UILinkPointNavigator.Shortcuts.FANCYUI_SPECIAL_INSTRUCTIONS = 1;
			int num = 3002;
			UILinkPointNavigator.SetPosition(3000, this._cancelButton.GetDimensions().Center());
			UILinkPoint uILinkPoint = UILinkPointNavigator.Points[3000];
			uILinkPoint.Unlink();
			uILinkPoint.Right = 3001;
			uILinkPoint.Up = num + 40;
			UILinkPointNavigator.SetPosition(3001, this._submitButton.GetDimensions().Center());
			uILinkPoint = UILinkPointNavigator.Points[3001];
			uILinkPoint.Unlink();
			uILinkPoint.Left = 3000;
			uILinkPoint.Up = num + 49;
			for (int i = 0; i < 5; i++)
			{
				for (int j = 0; j < 10; j++)
				{
					int num2 = i * 10 + j;
					int num3 = num + num2;
					if (this._keyList[num2] != null)
					{
						UILinkPointNavigator.SetPosition(num3, this._keyList[num2].GetDimensions().Center());
						uILinkPoint = UILinkPointNavigator.Points[num3];
						uILinkPoint.Unlink();
						int num4 = j - 1;
						while (num4 >= 0 && this._keyList[i * 10 + num4] == this._keyList[num2])
						{
							num4--;
						}
						if (num4 != -1)
						{
							uILinkPoint.Left = i * 10 + num4 + num;
						}
						else
						{
							uILinkPoint.Left = i * 10 + 9 + num;
						}
						int num5 = j + 1;
						while (num5 <= 9 && this._keyList[i * 10 + num5] == this._keyList[num2])
						{
							num5++;
						}
						if (num5 != 10 && this._keyList[num2] != this._keyList[num5])
						{
							uILinkPoint.Right = i * 10 + num5 + num;
						}
						else
						{
							uILinkPoint.Right = i * 10 + num;
						}
						if (i != 0)
						{
							uILinkPoint.Up = num3 - 10;
						}
						if (i != 4)
						{
							uILinkPoint.Down = num3 + 10;
						}
						else
						{
							uILinkPoint.Down = ((j < 5) ? 3000 : 3001);
						}
					}
				}
			}
		}

		public static void CycleSymbols()
		{
			if (UIVirtualKeyboard._currentInstance == null)
			{
				return;
			}
			switch (UIVirtualKeyboard._currentInstance._keyState)
			{
				case UIVirtualKeyboard.KeyState.Default:
					UIVirtualKeyboard._currentInstance.SetKeyState(UIVirtualKeyboard.KeyState.Shift);
					return;
				case UIVirtualKeyboard.KeyState.Symbol:
					UIVirtualKeyboard._currentInstance.SetKeyState(UIVirtualKeyboard.KeyState.Default);
					return;
				case UIVirtualKeyboard.KeyState.Shift:
					UIVirtualKeyboard._currentInstance.SetKeyState(UIVirtualKeyboard.KeyState.Symbol);
					return;
				default:
					return;
			}
		}

		public static void BackSpace()
		{
			if (UIVirtualKeyboard._currentInstance == null)
			{
				return;
			}
			Main.PlaySound(12, -1, -1, 1, 1f, 0f);
			UIVirtualKeyboard._currentInstance._textBox.Backspace();
			UIVirtualKeyboard._currentInstance.ValidateText();
		}

		public static void Submit()
		{
			if (UIVirtualKeyboard._currentInstance == null)
			{
				return;
			}
			string text = UIVirtualKeyboard._currentInstance.Text.Trim();
			if (text.Length > 0)
			{
				Main.PlaySound(10, -1, -1, 1, 1f, 0f);
				UIVirtualKeyboard._currentInstance._submitAction(text);
			}
		}

		public static void Cancel()
		{
			if (UIVirtualKeyboard._currentInstance == null)
			{
				return;
			}
			Main.PlaySound(11, -1, -1, 1, 1f, 0f);
			UIVirtualKeyboard._currentInstance._cancelAction();
		}

		public static void Write(string text)
		{
			if (UIVirtualKeyboard._currentInstance == null)
			{
				return;
			}
			Main.PlaySound(12, -1, -1, 1, 1f, 0f);
			bool flag = UIVirtualKeyboard._currentInstance.Text.Length == 0;
			UIVirtualKeyboard._currentInstance._textBox.Write(text);
			UIVirtualKeyboard._currentInstance.ValidateText();
			if (flag && UIVirtualKeyboard._currentInstance.Text.Length > 0 && UIVirtualKeyboard._currentInstance._keyState == UIVirtualKeyboard.KeyState.Shift)
			{
				UIVirtualKeyboard._currentInstance.SetKeyState(UIVirtualKeyboard.KeyState.Default);
			}
		}

		public static void CursorLeft()
		{
			if (UIVirtualKeyboard._currentInstance == null)
			{
				return;
			}
			Main.PlaySound(12, -1, -1, 1, 1f, 0f);
			UIVirtualKeyboard._currentInstance._textBox.CursorLeft();
		}

		public static void CursorRight()
		{
			if (UIVirtualKeyboard._currentInstance == null)
			{
				return;
			}
			Main.PlaySound(12, -1, -1, 1, 1f, 0f);
			UIVirtualKeyboard._currentInstance._textBox.CursorRight();
		}

		public static bool CanDisplay(int keyboardContext)
		{
			return keyboardContext != 1 || Main.screenHeight > 700;
		}

		public static void CacheCancelledInput(int cacheMode)
		{
			if (cacheMode != 1)
			{
				return;
			}
			UIVirtualKeyboard._cancelCacheSign = Main.npcChatText;
		}

		private void RestoreCancelledInput(int cacheMode)
		{
			if (cacheMode != 1)
			{
				return;
			}
			Main.npcChatText = UIVirtualKeyboard._cancelCacheSign;
			this.Text = Main.npcChatText;
			UIVirtualKeyboard._cancelCacheSign = "";
		}

		private void CopyTextToSign()
		{
			if (!this._edittingSign)
			{
				return;
			}
			int sign = Main.player[Main.myPlayer].sign;
			if (sign < 0 || Main.sign[sign] == null)
			{
				return;
			}
			Main.npcChatText = this.Text;
		}

		private void CopyTextToChest()
		{
			if (!this._edittingChest)
			{
				return;
			}
			Main.npcChatText = this.Text;
		}
	}
}
