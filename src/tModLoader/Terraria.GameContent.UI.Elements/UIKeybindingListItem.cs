using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria.GameContent.UI.Chat;
using Terraria.GameInput;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Terraria.GameContent.UI.Elements
{
	public class UIKeybindingListItem : UIElement
	{
		private InputMode _inputmode;
		private Color _color;
		private string _keybind;

		public UIKeybindingListItem(string bind, InputMode mode, Color color)
		{
			this._keybind = bind;
			this._inputmode = mode;
			this._color = color;
			base.OnClick += new UIElement.MouseEvent(this.OnClickMethod);
		}

		public void OnClickMethod(UIMouseEvent evt, UIElement listeningElement)
		{
			if (PlayerInput.ListeningTrigger != this._keybind)
			{
				if (PlayerInput.CurrentProfile.AllowEditting)
				{
					PlayerInput.ListenFor(this._keybind, this._inputmode);
					return;
				}
				PlayerInput.ListenFor(null, this._inputmode);
			}
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			float num = 6f;
			base.DrawSelf(spriteBatch);
			CalculatedStyle dimensions = base.GetDimensions();
			float num2 = dimensions.Width + 1f;
			Vector2 vector = new Vector2(dimensions.X, dimensions.Y);
			bool flag = PlayerInput.ListeningTrigger == this._keybind;
			Vector2 baseScale = new Vector2(0.8f);
			Color color = flag ? Color.Gold : (base.IsMouseHovering ? Color.White : Color.Silver);
			color = Color.Lerp(color, Color.White, base.IsMouseHovering ? 0.5f : 0f);
			Color color2 = base.IsMouseHovering ? this._color : this._color.MultiplyRGBA(new Color(180, 180, 180));
			Vector2 vector2 = vector;
			Utils.DrawSettingsPanel(spriteBatch, vector2, num2, color2);
			vector2.X += 8f;
			vector2.Y += 2f + num;
			ChatManager.DrawColorCodedStringWithShadow(spriteBatch, Main.fontItemStack, this.GetFriendlyName(), vector2, color, 0f, Vector2.Zero, baseScale, num2, 2f);
			vector2.X -= 17f;
			List<string> list = PlayerInput.CurrentProfile.InputModes[this._inputmode].KeyStatus[this._keybind];
			string text = this.GenInput(list);
			if (string.IsNullOrEmpty(text))
			{
				text = Lang.menu[195];
				if (!flag)
				{
					color = new Color(80, 80, 80);
				}
			}
			Vector2 stringSize = ChatManager.GetStringSize(Main.fontItemStack, text, baseScale, -1f);
			vector2 = new Vector2(dimensions.X + dimensions.Width - stringSize.X - 10f, dimensions.Y + 2f + num);
			if (this._inputmode == InputMode.XBoxGamepad || this._inputmode == InputMode.XBoxGamepadUI)
			{
				vector2 += new Vector2(0f, -3f);
			}
			GlyphTagHandler.GlyphsScale = 0.85f;
			ChatManager.DrawColorCodedStringWithShadow(spriteBatch, Main.fontItemStack, text, vector2, color, 0f, Vector2.Zero, baseScale, num2, 2f);
			GlyphTagHandler.GlyphsScale = 1f;
		}

		private string GenInput(List<string> list)
		{
			if (list.Count == 0)
			{
				return "";
			}
			string text = "";
			switch (this._inputmode)
			{
				case InputMode.Keyboard:
				case InputMode.KeyboardUI:
				case InputMode.Mouse:
					text = list[0];
					for (int i = 1; i < list.Count; i++)
					{
						text = text + "/" + list[i];
					}
					break;
				case InputMode.XBoxGamepad:
				case InputMode.XBoxGamepadUI:
					text = GlyphTagHandler.GenerateTag(list[0]);
					for (int j = 1; j < list.Count; j++)
					{
						text = text + "/" + GlyphTagHandler.GenerateTag(list[j]);
					}
					break;
			}
			return text;
		}

		private string GetFriendlyName()
		{
			string keybind;
			switch (keybind = this._keybind)
			{
				case "MouseLeft":
					return Lang.menu[162];
				case "MouseRight":
					return Lang.menu[163];
				case "Up":
					return Lang.menu[148];
				case "Down":
					return Lang.menu[149];
				case "Left":
					return Lang.menu[150];
				case "Right":
					return Lang.menu[151];
				case "Jump":
					return Lang.menu[152];
				case "Throw":
					return Lang.menu[153];
				case "Inventory":
					return Lang.menu[154];
				case "Grapple":
					return Lang.menu[155];
				case "SmartSelect":
					return Lang.menu[160];
				case "SmartCursor":
					return Lang.menu[161];
				case "QuickMount":
					return Lang.menu[158];
				case "QuickHeal":
					return Lang.menu[159];
				case "QuickMana":
					return Lang.menu[156];
				case "QuickBuff":
					return Lang.menu[157];
				case "MapZoomIn":
					return Lang.menu[168];
				case "MapZoomOut":
					return Lang.menu[169];
				case "MapAlphaUp":
					return Lang.menu[171];
				case "MapAlphaDown":
					return Lang.menu[170];
				case "MapFull":
					return Lang.menu[173];
				case "MapStyle":
					return Lang.menu[172];
				case "Hotbar1":
					return Lang.menu[176];
				case "Hotbar2":
					return Lang.menu[177];
				case "Hotbar3":
					return Lang.menu[178];
				case "Hotbar4":
					return Lang.menu[179];
				case "Hotbar5":
					return Lang.menu[180];
				case "Hotbar6":
					return Lang.menu[181];
				case "Hotbar7":
					return Lang.menu[182];
				case "Hotbar8":
					return Lang.menu[183];
				case "Hotbar9":
					return Lang.menu[184];
				case "Hotbar10":
					return Lang.menu[185];
				case "HotbarMinus":
					return Lang.menu[174];
				case "HotbarPlus":
					return Lang.menu[175];
				case "DpadRadial1":
					return Lang.menu[186];
				case "DpadRadial2":
					return Lang.menu[187];
				case "DpadRadial3":
					return Lang.menu[188];
				case "DpadRadial4":
					return Lang.menu[189];
				case "RadialHotbar":
					return Lang.menu[190];
				case "RadialQuickbar":
					return Lang.menu[244];
				case "DpadSnap1":
					return Lang.menu[191];
				case "DpadSnap2":
					return Lang.menu[192];
				case "DpadSnap3":
					return Lang.menu[193];
				case "DpadSnap4":
					return Lang.menu[194];
				case "LockOn":
					return Lang.menu[231];
			}
			return this._keybind;
		}
	}
}
