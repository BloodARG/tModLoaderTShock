using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.Graphics;
using Terraria.Initializers;
using Terraria.Localization;
using Terraria.UI;
using Terraria.UI.Gamepad;

namespace Terraria.GameContent.UI.States
{
	public class UIManageControls : UIState
	{
		private const float PanelTextureHeight = 30f;
		public static int ForceMoveTo = -1;
		private static List<string> _BindingsFullLine = new List<string>
		{
			"Throw",
			"Inventory",
			"RadialHotbar",
			"RadialQuickbar",
			"LockOn",
			"sp3",
			"sp4",
			"sp5",
			"sp6",
			"sp7",
			"sp8",
			"sp18",
			"sp19",
			"sp9",
			"sp10",
			"sp11",
			"sp12",
			"sp13",
			"ResetModHotkeys"
		};
		private static List<string> _BindingsHalfSingleLine = new List<string>
		{
			"sp9",
			"sp10",
			"sp11",
			"sp12",
			"sp13",
			"ResetModHotkeys"
		};
		private bool OnKeyboard = true;
		private bool OnGameplay = true;
		private List<UIElement> _bindsKeyboard = new List<UIElement>();
		private List<UIElement> _bindsGamepad = new List<UIElement>();
		private List<UIElement> _bindsKeyboardUI = new List<UIElement>();
		private List<UIElement> _bindsGamepadUI = new List<UIElement>();
		private UIElement _outerContainer;
		private UIList _uilist;
		private UIImageFramed _buttonKeyboard;
		private UIImageFramed _buttonGamepad;
		private UIImageFramed _buttonBorder1;
		private UIImageFramed _buttonBorder2;
		private UIKeybindingSimpleListItem _buttonProfile;
		private UIElement _buttonBack;
		private UIImageFramed _buttonVs1;
		private UIImageFramed _buttonVs2;
		private UIImageFramed _buttonBorderVs1;
		private UIImageFramed _buttonBorderVs2;
		private Texture2D _KeyboardGamepadTexture;
		private Texture2D _keyboardGamepadBorderTexture;
		private Texture2D _GameplayVsUITexture;
		private Texture2D _GameplayVsUIBorderTexture;
		private static int SnapPointIndex = 0;

		public override void OnInitialize()
		{
			this._KeyboardGamepadTexture = TextureManager.Load("Images/UI/Settings_Inputs");
			this._keyboardGamepadBorderTexture = TextureManager.Load("Images/UI/Settings_Inputs_Border");
			this._GameplayVsUITexture = TextureManager.Load("Images/UI/Settings_Inputs_2");
			this._GameplayVsUIBorderTexture = TextureManager.Load("Images/UI/Settings_Inputs_2_Border");
			UIElement uIElement = new UIElement();
			uIElement.Width.Set(0f, 0.8f);
			uIElement.MaxWidth.Set(600f, 0f);
			uIElement.Top.Set(220f, 0f);
			uIElement.Height.Set(-200f, 1f);
			uIElement.HAlign = 0.5f;
			this._outerContainer = uIElement;
			UIPanel uIPanel = new UIPanel();
			uIPanel.Width.Set(0f, 1f);
			uIPanel.Height.Set(-110f, 1f);
			uIPanel.BackgroundColor = new Color(33, 43, 79) * 0.8f;
			uIElement.Append(uIPanel);
			this._buttonKeyboard = new UIImageFramed(this._KeyboardGamepadTexture, this._KeyboardGamepadTexture.Frame(2, 2, 0, 0));
			this._buttonKeyboard.VAlign = 0f;
			this._buttonKeyboard.HAlign = 0f;
			this._buttonKeyboard.Left.Set(0f, 0f);
			this._buttonKeyboard.Top.Set(8f, 0f);
			this._buttonKeyboard.OnClick += new UIElement.MouseEvent(this.KeyboardButtonClick);
			this._buttonKeyboard.OnMouseOver += new UIElement.MouseEvent(this.ManageBorderKeyboardOn);
			this._buttonKeyboard.OnMouseOut += new UIElement.MouseEvent(this.ManageBorderKeyboardOff);
			uIPanel.Append(this._buttonKeyboard);
			this._buttonGamepad = new UIImageFramed(this._KeyboardGamepadTexture, this._KeyboardGamepadTexture.Frame(2, 2, 1, 1));
			this._buttonGamepad.VAlign = 0f;
			this._buttonGamepad.HAlign = 0f;
			this._buttonGamepad.Left.Set(76f, 0f);
			this._buttonGamepad.Top.Set(8f, 0f);
			this._buttonGamepad.OnClick += new UIElement.MouseEvent(this.GamepadButtonClick);
			this._buttonGamepad.OnMouseOver += new UIElement.MouseEvent(this.ManageBorderGamepadOn);
			this._buttonGamepad.OnMouseOut += new UIElement.MouseEvent(this.ManageBorderGamepadOff);
			uIPanel.Append(this._buttonGamepad);
			this._buttonBorder1 = new UIImageFramed(this._keyboardGamepadBorderTexture, this._keyboardGamepadBorderTexture.Frame(1, 1, 0, 0));
			this._buttonBorder1.VAlign = 0f;
			this._buttonBorder1.HAlign = 0f;
			this._buttonBorder1.Left.Set(0f, 0f);
			this._buttonBorder1.Top.Set(8f, 0f);
			this._buttonBorder1.Color = Color.Silver;
			uIPanel.Append(this._buttonBorder1);
			this._buttonBorder2 = new UIImageFramed(this._keyboardGamepadBorderTexture, this._keyboardGamepadBorderTexture.Frame(1, 1, 0, 0));
			this._buttonBorder2.VAlign = 0f;
			this._buttonBorder2.HAlign = 0f;
			this._buttonBorder2.Left.Set(76f, 0f);
			this._buttonBorder2.Top.Set(8f, 0f);
			this._buttonBorder2.Color = Color.Transparent;
			uIPanel.Append(this._buttonBorder2);
			this._buttonVs1 = new UIImageFramed(this._GameplayVsUITexture, this._GameplayVsUITexture.Frame(2, 2, 0, 0));
			this._buttonVs1.VAlign = 0f;
			this._buttonVs1.HAlign = 0f;
			this._buttonVs1.Left.Set(172f, 0f);
			this._buttonVs1.Top.Set(8f, 0f);
			this._buttonVs1.OnClick += new UIElement.MouseEvent(this.VsGameplayButtonClick);
			this._buttonVs1.OnMouseOver += new UIElement.MouseEvent(this.ManageBorderGameplayOn);
			this._buttonVs1.OnMouseOut += new UIElement.MouseEvent(this.ManageBorderGameplayOff);
			uIPanel.Append(this._buttonVs1);
			this._buttonVs2 = new UIImageFramed(this._GameplayVsUITexture, this._GameplayVsUITexture.Frame(2, 2, 1, 1));
			this._buttonVs2.VAlign = 0f;
			this._buttonVs2.HAlign = 0f;
			this._buttonVs2.Left.Set(212f, 0f);
			this._buttonVs2.Top.Set(8f, 0f);
			this._buttonVs2.OnClick += new UIElement.MouseEvent(this.VsMenuButtonClick);
			this._buttonVs2.OnMouseOver += new UIElement.MouseEvent(this.ManageBorderMenuOn);
			this._buttonVs2.OnMouseOut += new UIElement.MouseEvent(this.ManageBorderMenuOff);
			uIPanel.Append(this._buttonVs2);
			this._buttonBorderVs1 = new UIImageFramed(this._GameplayVsUIBorderTexture, this._GameplayVsUIBorderTexture.Frame(1, 1, 0, 0));
			this._buttonBorderVs1.VAlign = 0f;
			this._buttonBorderVs1.HAlign = 0f;
			this._buttonBorderVs1.Left.Set(172f, 0f);
			this._buttonBorderVs1.Top.Set(8f, 0f);
			this._buttonBorderVs1.Color = Color.Silver;
			uIPanel.Append(this._buttonBorderVs1);
			this._buttonBorderVs2 = new UIImageFramed(this._GameplayVsUIBorderTexture, this._GameplayVsUIBorderTexture.Frame(1, 1, 0, 0));
			this._buttonBorderVs2.VAlign = 0f;
			this._buttonBorderVs2.HAlign = 0f;
			this._buttonBorderVs2.Left.Set(212f, 0f);
			this._buttonBorderVs2.Top.Set(8f, 0f);
			this._buttonBorderVs2.Color = Color.Transparent;
			uIPanel.Append(this._buttonBorderVs2);
			this._buttonProfile = new UIKeybindingSimpleListItem(() => PlayerInput.CurrentProfile.Name, new Color(73, 94, 171, 255) * 0.9f);
			this._buttonProfile.VAlign = 0f;
			this._buttonProfile.HAlign = 1f;
			this._buttonProfile.Width.Set(180f, 0f);
			this._buttonProfile.Height.Set(30f, 0f);
			this._buttonProfile.MarginRight = 30f;
			this._buttonProfile.Left.Set(0f, 0f);
			this._buttonProfile.Top.Set(8f, 0f);
			this._buttonProfile.OnClick += new UIElement.MouseEvent(this.profileButtonClick);
			uIPanel.Append(this._buttonProfile);
			this._uilist = new UIList();
			this._uilist.Width.Set(-25f, 1f);
			this._uilist.Height.Set(-50f, 1f);
			this._uilist.VAlign = 1f;
			this._uilist.PaddingBottom = 5f;
			this._uilist.ListPadding = 20f;
			uIPanel.Append(this._uilist);
			this.AssembleBindPanels();
			this.FillList();
			UIScrollbar uIScrollbar = new UIScrollbar();
			uIScrollbar.SetView(100f, 1000f);
			uIScrollbar.Height.Set(-67f, 1f);
			uIScrollbar.HAlign = 1f;
			uIScrollbar.VAlign = 1f;
			uIScrollbar.MarginBottom = 11f;
			uIPanel.Append(uIScrollbar);
			this._uilist.SetScrollbar(uIScrollbar);
			UITextPanel<LocalizedText> uITextPanel = new UITextPanel<LocalizedText>(Language.GetText("UI.Keybindings"), 0.7f, true);
			uITextPanel.HAlign = 0.5f;
			uITextPanel.Top.Set(-45f, 0f);
			uITextPanel.Left.Set(-10f, 0f);
			uITextPanel.SetPadding(15f);
			uITextPanel.BackgroundColor = new Color(73, 94, 171);
			uIElement.Append(uITextPanel);
			UITextPanel<LocalizedText> uITextPanel2 = new UITextPanel<LocalizedText>(Language.GetText("UI.Back"), 0.7f, true);
			uITextPanel2.Width.Set(-10f, 0.5f);
			uITextPanel2.Height.Set(50f, 0f);
			uITextPanel2.VAlign = 1f;
			uITextPanel2.HAlign = 0.5f;
			uITextPanel2.Top.Set(-45f, 0f);
			uITextPanel2.OnMouseOver += new UIElement.MouseEvent(this.FadedMouseOver);
			uITextPanel2.OnMouseOut += new UIElement.MouseEvent(this.FadedMouseOut);
			uITextPanel2.OnClick += new UIElement.MouseEvent(this.GoBackClick);
			uIElement.Append(uITextPanel2);
			this._buttonBack = uITextPanel2;
			base.Append(uIElement);
		}

		private void AssembleBindPanels()
		{
			List<string> bindings = new List<string>
			{
				"MouseLeft",
				"MouseRight",
				"Up",
				"Down",
				"Left",
				"Right",
				"Jump",
				"Grapple",
				"SmartSelect",
				"SmartCursor",
				"QuickMount",
				"QuickHeal",
				"QuickMana",
				"QuickBuff",
				"Throw",
				"Inventory",
				"sp9"
			};
			List<string> bindings2 = new List<string>
			{
				"MouseLeft",
				"MouseRight",
				"Up",
				"Down",
				"Left",
				"Right",
				"Jump",
				"Grapple",
				"SmartSelect",
				"SmartCursor",
				"QuickMount",
				"QuickHeal",
				"QuickMana",
				"QuickBuff",
				"LockOn",
				"Throw",
				"Inventory",
				"sp9"
			};
			List<string> bindings3 = new List<string>
			{
				"HotbarMinus",
				"HotbarPlus",
				"Hotbar1",
				"Hotbar2",
				"Hotbar3",
				"Hotbar4",
				"Hotbar5",
				"Hotbar6",
				"Hotbar7",
				"Hotbar8",
				"Hotbar9",
				"Hotbar10",
				"sp10"
			};
			List<string> bindings4 = new List<string>
			{
				"MapZoomIn",
				"MapZoomOut",
				"MapAlphaUp",
				"MapAlphaDown",
				"MapFull",
				"MapStyle",
				"sp11"
			};
			List<string> bindings5 = new List<string>
			{
				"sp1",
				"sp2",
				"RadialHotbar",
				"RadialQuickbar",
				"sp12"
			};
			List<string> bindings6 = new List<string>
			{
				"sp3",
				"sp4",
				"sp5",
				"sp6",
				"sp7",
				"sp8",
				"sp14",
				"sp15",
				"sp16",
				"sp17",
				"sp18",
				"sp19",
				"sp13"
			};
			_BindingsFullLine.RemoveAll(x => x.Contains(":"));
			List<string> modBindings = new List<string>();
			foreach (var hotkey in ModLoader.ModLoader.modHotKeys)
			{
				modBindings.Add(hotkey.Value.displayName);
				_BindingsFullLine.Add(hotkey.Value.displayName);
			}
			modBindings.Add("ResetModHotkeys");

			InputMode currentInputMode = InputMode.Keyboard;
			this._bindsKeyboard.Add(this.CreateBindingGroup(0, bindings, currentInputMode));
			this._bindsKeyboard.Add(this.CreateBindingGroup(1, bindings4, currentInputMode));
			this._bindsKeyboard.Add(this.CreateBindingGroup(2, bindings3, currentInputMode));
			this._bindsKeyboard.Add(this.CreateBindingGroup(5, modBindings, currentInputMode));
			currentInputMode = InputMode.XBoxGamepad;
			this._bindsGamepad.Add(this.CreateBindingGroup(0, bindings2, currentInputMode));
			this._bindsGamepad.Add(this.CreateBindingGroup(1, bindings4, currentInputMode));
			this._bindsGamepad.Add(this.CreateBindingGroup(2, bindings3, currentInputMode));
			this._bindsGamepad.Add(this.CreateBindingGroup(3, bindings5, currentInputMode));
			this._bindsGamepad.Add(this.CreateBindingGroup(4, bindings6, currentInputMode));
			this._bindsGamepad.Add(this.CreateBindingGroup(5, modBindings, currentInputMode));
			currentInputMode = InputMode.KeyboardUI;
			this._bindsKeyboardUI.Add(this.CreateBindingGroup(0, bindings, currentInputMode));
			this._bindsKeyboardUI.Add(this.CreateBindingGroup(1, bindings4, currentInputMode));
			this._bindsKeyboardUI.Add(this.CreateBindingGroup(2, bindings3, currentInputMode));
			this._bindsKeyboardUI.Add(this.CreateBindingGroup(5, modBindings, currentInputMode));
			currentInputMode = InputMode.XBoxGamepadUI;
			this._bindsGamepadUI.Add(this.CreateBindingGroup(0, bindings2, currentInputMode));
			this._bindsGamepadUI.Add(this.CreateBindingGroup(1, bindings4, currentInputMode));
			this._bindsGamepadUI.Add(this.CreateBindingGroup(2, bindings3, currentInputMode));
			this._bindsGamepadUI.Add(this.CreateBindingGroup(3, bindings5, currentInputMode));
			this._bindsGamepadUI.Add(this.CreateBindingGroup(4, bindings6, currentInputMode));
			this._bindsGamepadUI.Add(this.CreateBindingGroup(5, modBindings, currentInputMode));
		}

		private UISortableElement CreateBindingGroup(int elementIndex, List<string> bindings, InputMode currentInputMode)
		{
			UISortableElement uISortableElement = new UISortableElement(elementIndex);
			uISortableElement.HAlign = 0.5f;
			uISortableElement.Width.Set(0f, 1f);
			uISortableElement.Height.Set(2000f, 0f);
			UIPanel uIPanel = new UIPanel();
			uIPanel.Width.Set(0f, 1f);
			uIPanel.Height.Set(-16f, 1f);
			uIPanel.VAlign = 1f;
			uIPanel.BackgroundColor = new Color(33, 43, 79) * 0.8f;
			uISortableElement.Append(uIPanel);
			UIList uIList = new UIList();
			uIList.OverflowHidden = false;
			uIList.Width.Set(0f, 1f);
			uIList.Height.Set(-8f, 1f);
			uIList.VAlign = 1f;
			uIList.ListPadding = 5f;
			uIPanel.Append(uIList);
			Color arg_F3_0 = uIPanel.BackgroundColor;
			switch (elementIndex)
			{
				case 0:
					uIPanel.BackgroundColor = Color.Lerp(uIPanel.BackgroundColor, Color.Green, 0.18f);
					break;
				case 1:
					uIPanel.BackgroundColor = Color.Lerp(uIPanel.BackgroundColor, Color.Goldenrod, 0.18f);
					break;
				case 2:
					uIPanel.BackgroundColor = Color.Lerp(uIPanel.BackgroundColor, Color.HotPink, 0.18f);
					break;
				case 3:
					uIPanel.BackgroundColor = Color.Lerp(uIPanel.BackgroundColor, Color.Indigo, 0.18f);
					break;
				case 4:
					uIPanel.BackgroundColor = Color.Lerp(uIPanel.BackgroundColor, Color.Turquoise, 0.18f);
					break;
				case 5:
					uIPanel.BackgroundColor = Color.Lerp(uIPanel.BackgroundColor, Color.Yellow, 0.18f);
					break;
			}
			this.CreateElementGroup(uIList, bindings, currentInputMode, uIPanel.BackgroundColor);
			uIPanel.BackgroundColor = uIPanel.BackgroundColor.MultiplyRGBA(new Color(111, 111, 111));
			string text = "";
			switch (elementIndex)
			{
				case 0:
					text = Lang.menu[(currentInputMode == InputMode.Keyboard || currentInputMode == InputMode.XBoxGamepad) ? 164 : 243];
					break;
				case 1:
					text = Lang.menu[165];
					break;
				case 2:
					text = Lang.menu[166];
					break;
				case 3:
					text = Lang.menu[167];
					break;
				case 4:
					text = Lang.menu[198];
					break;
				case 5:
					text = "Mod Controls";
					break;
			}
			uISortableElement.Append(new UITextPanel<string>(text, 0.7f, false)
				{
					VAlign = 0f,
					HAlign = 0.5f
				});
			uISortableElement.Recalculate();
			float totalHeight = uIList.GetTotalHeight();
			uISortableElement.Width.Set(0f, 1f);
			uISortableElement.Height.Set(totalHeight + 30f + 16f, 0f);
			return uISortableElement;
		}

		private void CreateElementGroup(UIList parent, List<string> bindings, InputMode currentInputMode, Color color)
		{
			for (int i = 0; i < bindings.Count; i++)
			{
				string arg_0E_0 = bindings[i];
				UISortableElement uISortableElement = new UISortableElement(i);
				uISortableElement.Width.Set(0f, 1f);
				uISortableElement.Height.Set(30f, 0f);
				uISortableElement.HAlign = 0.5f;
				parent.Add(uISortableElement);
				if (UIManageControls._BindingsHalfSingleLine.Contains(bindings[i]))
				{
					UIElement uIElement = this.CreatePanel(bindings[i], currentInputMode, color);
					uIElement.Width.Set(0f, 0.5f);
					uIElement.HAlign = 0.5f;
					uIElement.Height.Set(0f, 1f);
					uIElement.SetSnapPoint("Wide", UIManageControls.SnapPointIndex++, null, null);
					uISortableElement.Append(uIElement);
				}
				else if (UIManageControls._BindingsFullLine.Contains(bindings[i]))
				{
					UIElement uIElement2 = this.CreatePanel(bindings[i], currentInputMode, color);
					uIElement2.Width.Set(0f, 1f);
					uIElement2.Height.Set(0f, 1f);
					uIElement2.SetSnapPoint("Wide", UIManageControls.SnapPointIndex++, null, null);
					uISortableElement.Append(uIElement2);
				}
				else
				{
					UIElement uIElement3 = this.CreatePanel(bindings[i], currentInputMode, color);
					uIElement3.Width.Set(-5f, 0.5f);
					uIElement3.Height.Set(0f, 1f);
					uIElement3.SetSnapPoint("Thin", UIManageControls.SnapPointIndex++, null, null);
					uISortableElement.Append(uIElement3);
					i++;
					if (i < bindings.Count)
					{
						uIElement3 = this.CreatePanel(bindings[i], currentInputMode, color);
						uIElement3.Width.Set(-5f, 0.5f);
						uIElement3.Height.Set(0f, 1f);
						uIElement3.HAlign = 1f;
						uIElement3.SetSnapPoint("Thin", UIManageControls.SnapPointIndex++, null, null);
						uISortableElement.Append(uIElement3);
					}
				}
			}
		}

		public UIElement CreatePanel(string bind, InputMode currentInputMode, Color color)
		{
			switch (bind)
			{
				case "sp1":
					{
						UIElement uIElement = new UIKeybindingToggleListItem(() => Lang.menu[196], () => PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepad].KeyStatus["DpadSnap1"].Contains(Buttons.DPadUp.ToString()) && PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepad].KeyStatus["DpadSnap2"].Contains(Buttons.DPadRight.ToString()) && PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepad].KeyStatus["DpadSnap3"].Contains(Buttons.DPadDown.ToString()) && PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepad].KeyStatus["DpadSnap4"].Contains(Buttons.DPadLeft.ToString()) && PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepadUI].KeyStatus["DpadSnap1"].Contains(Buttons.DPadUp.ToString()) && PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepadUI].KeyStatus["DpadSnap2"].Contains(Buttons.DPadRight.ToString()) && PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepadUI].KeyStatus["DpadSnap3"].Contains(Buttons.DPadDown.ToString()) && PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepadUI].KeyStatus["DpadSnap4"].Contains(Buttons.DPadLeft.ToString()), color);
						uIElement.OnClick += new UIElement.MouseEvent(this.SnapButtonClick);
						return uIElement;
					}
				case "sp2":
					{
						UIElement uIElement2 = new UIKeybindingToggleListItem(() => Lang.menu[197], () => PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepad].KeyStatus["DpadRadial1"].Contains(Buttons.DPadUp.ToString()) && PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepad].KeyStatus["DpadRadial2"].Contains(Buttons.DPadRight.ToString()) && PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepad].KeyStatus["DpadRadial3"].Contains(Buttons.DPadDown.ToString()) && PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepad].KeyStatus["DpadRadial4"].Contains(Buttons.DPadLeft.ToString()) && PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepadUI].KeyStatus["DpadRadial1"].Contains(Buttons.DPadUp.ToString()) && PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepadUI].KeyStatus["DpadRadial2"].Contains(Buttons.DPadRight.ToString()) && PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepadUI].KeyStatus["DpadRadial3"].Contains(Buttons.DPadDown.ToString()) && PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepadUI].KeyStatus["DpadRadial4"].Contains(Buttons.DPadLeft.ToString()), color);
						uIElement2.OnClick += new UIElement.MouseEvent(this.RadialButtonClick);
						return uIElement2;
					}
				case "sp3":
					return new UIKeybindingSliderItem(() => Lang.menu[199] + " (" + PlayerInput.CurrentProfile.TriggersDeadzone.ToString("P1") + ")", () => PlayerInput.CurrentProfile.TriggersDeadzone, delegate(float f)
						{
							PlayerInput.CurrentProfile.TriggersDeadzone = f;
						}, delegate
						{
							PlayerInput.CurrentProfile.TriggersDeadzone = UILinksInitializer.HandleSlider(PlayerInput.CurrentProfile.TriggersDeadzone, 0f, 0.95f, PlayerInput.CurrentProfile.InterfaceDeadzoneX, 0.35f);
						}, 1000, color);
				case "sp4":
					return new UIKeybindingSliderItem(() => Lang.menu[200] + " (" + PlayerInput.CurrentProfile.InterfaceDeadzoneX.ToString("P1") + ")", () => PlayerInput.CurrentProfile.InterfaceDeadzoneX, delegate(float f)
						{
							PlayerInput.CurrentProfile.InterfaceDeadzoneX = f;
						}, delegate
						{
							PlayerInput.CurrentProfile.InterfaceDeadzoneX = UILinksInitializer.HandleSlider(PlayerInput.CurrentProfile.InterfaceDeadzoneX, 0f, 0.95f, 0.35f, 0.35f);
						}, 1001, color);
				case "sp5":
					return new UIKeybindingSliderItem(() => Lang.menu[201] + " (" + PlayerInput.CurrentProfile.LeftThumbstickDeadzoneX.ToString("P1") + ")", () => PlayerInput.CurrentProfile.LeftThumbstickDeadzoneX, delegate(float f)
						{
							PlayerInput.CurrentProfile.LeftThumbstickDeadzoneX = f;
						}, delegate
						{
							PlayerInput.CurrentProfile.LeftThumbstickDeadzoneX = UILinksInitializer.HandleSlider(PlayerInput.CurrentProfile.LeftThumbstickDeadzoneX, 0f, 0.95f, PlayerInput.CurrentProfile.InterfaceDeadzoneX, 0.35f);
						}, 1002, color);
				case "sp6":
					return new UIKeybindingSliderItem(() => Lang.menu[202] + " (" + PlayerInput.CurrentProfile.LeftThumbstickDeadzoneY.ToString("P1") + ")", () => PlayerInput.CurrentProfile.LeftThumbstickDeadzoneY, delegate(float f)
						{
							PlayerInput.CurrentProfile.LeftThumbstickDeadzoneY = f;
						}, delegate
						{
							PlayerInput.CurrentProfile.LeftThumbstickDeadzoneY = UILinksInitializer.HandleSlider(PlayerInput.CurrentProfile.LeftThumbstickDeadzoneY, 0f, 0.95f, PlayerInput.CurrentProfile.InterfaceDeadzoneX, 0.35f);
						}, 1003, color);
				case "sp7":
					return new UIKeybindingSliderItem(() => Lang.menu[203] + " (" + PlayerInput.CurrentProfile.RightThumbstickDeadzoneX.ToString("P1") + ")", () => PlayerInput.CurrentProfile.RightThumbstickDeadzoneX, delegate(float f)
						{
							PlayerInput.CurrentProfile.RightThumbstickDeadzoneX = f;
						}, delegate
						{
							PlayerInput.CurrentProfile.RightThumbstickDeadzoneX = UILinksInitializer.HandleSlider(PlayerInput.CurrentProfile.RightThumbstickDeadzoneX, 0f, 0.95f, PlayerInput.CurrentProfile.InterfaceDeadzoneX, 0.35f);
						}, 1004, color);
				case "sp8":
					return new UIKeybindingSliderItem(() => Lang.menu[204] + " (" + PlayerInput.CurrentProfile.RightThumbstickDeadzoneY.ToString("P1") + ")", () => PlayerInput.CurrentProfile.RightThumbstickDeadzoneY, delegate(float f)
						{
							PlayerInput.CurrentProfile.RightThumbstickDeadzoneY = f;
						}, delegate
						{
							PlayerInput.CurrentProfile.RightThumbstickDeadzoneY = UILinksInitializer.HandleSlider(PlayerInput.CurrentProfile.RightThumbstickDeadzoneY, 0f, 0.95f, PlayerInput.CurrentProfile.InterfaceDeadzoneX, 0.35f);
						}, 1005, color);
				case "sp9":
					{
						UIElement uIElement3 = new UIKeybindingSimpleListItem(() => Lang.menu[86], color);
						uIElement3.OnClick += delegate(UIMouseEvent evt, UIElement listeningElement)
						{
							string copyableProfileName = UIManageControls.GetCopyableProfileName();
							PlayerInput.CurrentProfile.CopyGameplaySettingsFrom(PlayerInput.OriginalProfiles[copyableProfileName], currentInputMode);
						};
						return uIElement3;
					}
				case "sp10":
					{
						UIElement uIElement4 = new UIKeybindingSimpleListItem(() => Lang.menu[86], color);
						uIElement4.OnClick += delegate(UIMouseEvent evt, UIElement listeningElement)
						{
							string copyableProfileName = UIManageControls.GetCopyableProfileName();
							PlayerInput.CurrentProfile.CopyHotbarSettingsFrom(PlayerInput.OriginalProfiles[copyableProfileName], currentInputMode);
						};
						return uIElement4;
					}
				case "sp11":
					{
						UIElement uIElement5 = new UIKeybindingSimpleListItem(() => Lang.menu[86], color);
						uIElement5.OnClick += delegate(UIMouseEvent evt, UIElement listeningElement)
						{
							string copyableProfileName = UIManageControls.GetCopyableProfileName();
							PlayerInput.CurrentProfile.CopyMapSettingsFrom(PlayerInput.OriginalProfiles[copyableProfileName], currentInputMode);
						};
						return uIElement5;
					}
				case "sp12":
					{
						UIElement uIElement6 = new UIKeybindingSimpleListItem(() => Lang.menu[86], color);
						uIElement6.OnClick += delegate(UIMouseEvent evt, UIElement listeningElement)
						{
							string copyableProfileName = UIManageControls.GetCopyableProfileName();
							PlayerInput.CurrentProfile.CopyGamepadSettingsFrom(PlayerInput.OriginalProfiles[copyableProfileName], currentInputMode);
						};
						return uIElement6;
					}
				case "sp13":
					{
						UIElement uIElement7 = new UIKeybindingSimpleListItem(() => Lang.menu[86], color);
						uIElement7.OnClick += delegate(UIMouseEvent evt, UIElement listeningElement)
						{
							string copyableProfileName = UIManageControls.GetCopyableProfileName();
							PlayerInput.CurrentProfile.CopyGamepadAdvancedSettingsFrom(PlayerInput.OriginalProfiles[copyableProfileName], currentInputMode);
						};
						return uIElement7;
					}
				case "ResetModHotkeys":
					{
						UIElement uIElement7 = new UIKeybindingSimpleListItem(() => Lang.menu[86], color);
						uIElement7.OnClick += delegate(UIMouseEvent evt, UIElement listeningElement)
						{
							string copyableProfileName = UIManageControls.GetCopyableProfileName();
							PlayerInput.CurrentProfile.CopyModHotkeySettingsFrom(PlayerInput.OriginalProfiles[copyableProfileName], currentInputMode);
						};
						return uIElement7;
					}
				case "sp14":
					{
						UIElement uIElement8 = new UIKeybindingToggleListItem(() => Lang.menu[205], () => PlayerInput.CurrentProfile.LeftThumbstickInvertX, color);
						uIElement8.OnClick += delegate(UIMouseEvent evt, UIElement listeningElement)
						{
							if (PlayerInput.CurrentProfile.AllowEditting)
							{
								PlayerInput.CurrentProfile.LeftThumbstickInvertX = !PlayerInput.CurrentProfile.LeftThumbstickInvertX;
							}
						};
						return uIElement8;
					}
				case "sp15":
					{
						UIElement uIElement9 = new UIKeybindingToggleListItem(() => Lang.menu[206], () => PlayerInput.CurrentProfile.LeftThumbstickInvertY, color);
						uIElement9.OnClick += delegate(UIMouseEvent evt, UIElement listeningElement)
						{
							if (PlayerInput.CurrentProfile.AllowEditting)
							{
								PlayerInput.CurrentProfile.LeftThumbstickInvertY = !PlayerInput.CurrentProfile.LeftThumbstickInvertY;
							}
						};
						return uIElement9;
					}
				case "sp16":
					{
						UIElement uIElement10 = new UIKeybindingToggleListItem(() => Lang.menu[207], () => PlayerInput.CurrentProfile.RightThumbstickInvertX, color);
						uIElement10.OnClick += delegate(UIMouseEvent evt, UIElement listeningElement)
						{
							if (PlayerInput.CurrentProfile.AllowEditting)
							{
								PlayerInput.CurrentProfile.RightThumbstickInvertX = !PlayerInput.CurrentProfile.RightThumbstickInvertX;
							}
						};
						return uIElement10;
					}
				case "sp17":
					{
						UIElement uIElement11 = new UIKeybindingToggleListItem(() => Lang.menu[208], () => PlayerInput.CurrentProfile.RightThumbstickInvertY, color);
						uIElement11.OnClick += delegate(UIMouseEvent evt, UIElement listeningElement)
						{
							if (PlayerInput.CurrentProfile.AllowEditting)
							{
								PlayerInput.CurrentProfile.RightThumbstickInvertY = !PlayerInput.CurrentProfile.RightThumbstickInvertY;
							}
						};
						return uIElement11;
					}
				case "sp18":
					return new UIKeybindingSliderItem(delegate
						{
							int hotbarRadialHoldTimeRequired = PlayerInput.CurrentProfile.HotbarRadialHoldTimeRequired;
							if (hotbarRadialHoldTimeRequired == -1)
							{
								return Lang.menu[228];
							}
							return Lang.menu[227] + " (" + ((float)hotbarRadialHoldTimeRequired / 60f).ToString("F2") + "s)";
						}, delegate
						{
							if (PlayerInput.CurrentProfile.HotbarRadialHoldTimeRequired == -1)
							{
								return 1f;
							}
							return (float)PlayerInput.CurrentProfile.HotbarRadialHoldTimeRequired / 301f;
						}, delegate(float f)
						{
							PlayerInput.CurrentProfile.HotbarRadialHoldTimeRequired = (int)(f * 301f);
							if ((float)PlayerInput.CurrentProfile.HotbarRadialHoldTimeRequired == 301f)
							{
								PlayerInput.CurrentProfile.HotbarRadialHoldTimeRequired = -1;
							}
						}, delegate
						{
							float num2 = (PlayerInput.CurrentProfile.HotbarRadialHoldTimeRequired == -1) ? 1f : ((float)PlayerInput.CurrentProfile.HotbarRadialHoldTimeRequired / 301f);
							num2 = UILinksInitializer.HandleSlider(num2, 0f, 1f, PlayerInput.CurrentProfile.InterfaceDeadzoneX, 0.5f);
							PlayerInput.CurrentProfile.HotbarRadialHoldTimeRequired = (int)(num2 * 301f);
							if ((float)PlayerInput.CurrentProfile.HotbarRadialHoldTimeRequired == 301f)
							{
								PlayerInput.CurrentProfile.HotbarRadialHoldTimeRequired = -1;
							}
						}, 1007, color);
				case "sp19":
					return new UIKeybindingSliderItem(delegate
						{
							int inventoryMoveCD = PlayerInput.CurrentProfile.InventoryMoveCD;
							return Lang.menu[252] + " (" + ((float)inventoryMoveCD / 60f).ToString("F2") + "s)";
						}, () => Utils.InverseLerp(4f, 12f, (float)PlayerInput.CurrentProfile.InventoryMoveCD, true), delegate(float f)
						{
							PlayerInput.CurrentProfile.InventoryMoveCD = (int)Math.Round((double)MathHelper.Lerp(4f, 12f, f));
						}, delegate
						{
							if (UILinkPointNavigator.Shortcuts.INV_MOVE_OPTION_CD > 0)
							{
								UILinkPointNavigator.Shortcuts.INV_MOVE_OPTION_CD--;
							}
							if (UILinkPointNavigator.Shortcuts.INV_MOVE_OPTION_CD == 0)
							{
								float num2 = Utils.InverseLerp(4f, 12f, (float)PlayerInput.CurrentProfile.InventoryMoveCD, true);
								float num3 = UILinksInitializer.HandleSlider(num2, 0f, 1f, PlayerInput.CurrentProfile.InterfaceDeadzoneX, 0.5f);
								if (num2 != num3)
								{
									UILinkPointNavigator.Shortcuts.INV_MOVE_OPTION_CD = 8;
									int num4 = Math.Sign(num3 - num2);
									PlayerInput.CurrentProfile.InventoryMoveCD = (int)MathHelper.Clamp((float)(PlayerInput.CurrentProfile.InventoryMoveCD + num4), 4f, 12f);
								}
							}
						}, 1008, color);
			}
			return new UIKeybindingListItem(bind, currentInputMode, color);
		}

		public override void OnActivate()
		{
			// TODO, only if new mod controls -- seems to be cause of bug on reload.
			_bindsKeyboard.Clear();
			_bindsGamepad.Clear();
			_bindsKeyboardUI.Clear();
			_bindsGamepadUI.Clear();
			AssembleBindPanels();
			FillList();

			if (Main.gameMenu)
			{
				this._outerContainer.Top.Set(220f, 0f);
				this._outerContainer.Height.Set(-220f, 1f);
			}
			else
			{
				this._outerContainer.Top.Set(120f, 0f);
				this._outerContainer.Height.Set(-120f, 1f);
			}
			if (PlayerInput.UsingGamepadUI)
			{
				UILinkPointNavigator.ChangePoint(3002);
			}
		}

		private static string GetCopyableProfileName()
		{
			string result = "Redigit's Pick";
			if (PlayerInput.OriginalProfiles.ContainsKey(PlayerInput.CurrentProfile.Name))
			{
				result = PlayerInput.CurrentProfile.Name;
			}
			return result;
		}

		private void FillList()
		{
			List<UIElement> list = this._bindsKeyboard;
			if (!this.OnKeyboard)
			{
				list = this._bindsGamepad;
			}
			if (!this.OnGameplay)
			{
				list = (this.OnKeyboard ? this._bindsKeyboardUI : this._bindsGamepadUI);
			}
			this._uilist.Clear();
			foreach (UIElement current in list)
			{
				this._uilist.Add(current);
			}
		}

		private void SnapButtonClick(UIMouseEvent evt, UIElement listeningElement)
		{
			if (PlayerInput.CurrentProfile.AllowEditting)
			{
				Main.PlaySound(12, -1, -1, 1, 1f, 0f);
				if (PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepad].KeyStatus["DpadSnap1"].Contains(Buttons.DPadUp.ToString()) && PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepad].KeyStatus["DpadSnap2"].Contains(Buttons.DPadRight.ToString()) && PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepad].KeyStatus["DpadSnap3"].Contains(Buttons.DPadDown.ToString()) && PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepad].KeyStatus["DpadSnap4"].Contains(Buttons.DPadLeft.ToString()) && PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepadUI].KeyStatus["DpadSnap1"].Contains(Buttons.DPadUp.ToString()) && PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepadUI].KeyStatus["DpadSnap2"].Contains(Buttons.DPadRight.ToString()) && PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepadUI].KeyStatus["DpadSnap3"].Contains(Buttons.DPadDown.ToString()) && PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepadUI].KeyStatus["DpadSnap4"].Contains(Buttons.DPadLeft.ToString()))
				{
					PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepad].KeyStatus["DpadSnap1"].Clear();
					PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepad].KeyStatus["DpadSnap2"].Clear();
					PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepad].KeyStatus["DpadSnap3"].Clear();
					PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepad].KeyStatus["DpadSnap4"].Clear();
					PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepadUI].KeyStatus["DpadSnap1"].Clear();
					PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepadUI].KeyStatus["DpadSnap2"].Clear();
					PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepadUI].KeyStatus["DpadSnap3"].Clear();
					PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepadUI].KeyStatus["DpadSnap4"].Clear();
					return;
				}
				PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepad].KeyStatus["DpadRadial1"].Clear();
				PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepad].KeyStatus["DpadRadial2"].Clear();
				PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepad].KeyStatus["DpadRadial3"].Clear();
				PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepad].KeyStatus["DpadRadial4"].Clear();
				PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepadUI].KeyStatus["DpadRadial1"].Clear();
				PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepadUI].KeyStatus["DpadRadial2"].Clear();
				PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepadUI].KeyStatus["DpadRadial3"].Clear();
				PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepadUI].KeyStatus["DpadRadial4"].Clear();
				PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepad].KeyStatus["DpadSnap1"] = new List<string>
				{
					Buttons.DPadUp.ToString()
				};
				PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepad].KeyStatus["DpadSnap2"] = new List<string>
				{
					Buttons.DPadRight.ToString()
				};
				PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepad].KeyStatus["DpadSnap3"] = new List<string>
				{
					Buttons.DPadDown.ToString()
				};
				PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepad].KeyStatus["DpadSnap4"] = new List<string>
				{
					Buttons.DPadLeft.ToString()
				};
				PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepadUI].KeyStatus["DpadSnap1"] = new List<string>
				{
					Buttons.DPadUp.ToString()
				};
				PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepadUI].KeyStatus["DpadSnap2"] = new List<string>
				{
					Buttons.DPadRight.ToString()
				};
				PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepadUI].KeyStatus["DpadSnap3"] = new List<string>
				{
					Buttons.DPadDown.ToString()
				};
				PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepadUI].KeyStatus["DpadSnap4"] = new List<string>
				{
					Buttons.DPadLeft.ToString()
				};
			}
		}

		private void RadialButtonClick(UIMouseEvent evt, UIElement listeningElement)
		{
			if (PlayerInput.CurrentProfile.AllowEditting)
			{
				Main.PlaySound(12, -1, -1, 1, 1f, 0f);
				if (PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepad].KeyStatus["DpadRadial1"].Contains(Buttons.DPadUp.ToString()) && PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepad].KeyStatus["DpadRadial2"].Contains(Buttons.DPadRight.ToString()) && PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepad].KeyStatus["DpadRadial3"].Contains(Buttons.DPadDown.ToString()) && PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepad].KeyStatus["DpadRadial4"].Contains(Buttons.DPadLeft.ToString()) && PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepadUI].KeyStatus["DpadRadial1"].Contains(Buttons.DPadUp.ToString()) && PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepadUI].KeyStatus["DpadRadial2"].Contains(Buttons.DPadRight.ToString()) && PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepadUI].KeyStatus["DpadRadial3"].Contains(Buttons.DPadDown.ToString()) && PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepadUI].KeyStatus["DpadRadial4"].Contains(Buttons.DPadLeft.ToString()))
				{
					PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepad].KeyStatus["DpadRadial1"].Clear();
					PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepad].KeyStatus["DpadRadial2"].Clear();
					PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepad].KeyStatus["DpadRadial3"].Clear();
					PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepad].KeyStatus["DpadRadial4"].Clear();
					PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepadUI].KeyStatus["DpadRadial1"].Clear();
					PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepadUI].KeyStatus["DpadRadial2"].Clear();
					PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepadUI].KeyStatus["DpadRadial3"].Clear();
					PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepadUI].KeyStatus["DpadRadial4"].Clear();
					return;
				}
				PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepad].KeyStatus["DpadSnap1"].Clear();
				PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepad].KeyStatus["DpadSnap2"].Clear();
				PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepad].KeyStatus["DpadSnap3"].Clear();
				PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepad].KeyStatus["DpadSnap4"].Clear();
				PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepadUI].KeyStatus["DpadSnap1"].Clear();
				PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepadUI].KeyStatus["DpadSnap2"].Clear();
				PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepadUI].KeyStatus["DpadSnap3"].Clear();
				PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepadUI].KeyStatus["DpadSnap4"].Clear();
				PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepad].KeyStatus["DpadRadial1"] = new List<string>
				{
					Buttons.DPadUp.ToString()
				};
				PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepad].KeyStatus["DpadRadial2"] = new List<string>
				{
					Buttons.DPadRight.ToString()
				};
				PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepad].KeyStatus["DpadRadial3"] = new List<string>
				{
					Buttons.DPadDown.ToString()
				};
				PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepad].KeyStatus["DpadRadial4"] = new List<string>
				{
					Buttons.DPadLeft.ToString()
				};
				PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepadUI].KeyStatus["DpadRadial1"] = new List<string>
				{
					Buttons.DPadUp.ToString()
				};
				PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepadUI].KeyStatus["DpadRadial2"] = new List<string>
				{
					Buttons.DPadRight.ToString()
				};
				PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepadUI].KeyStatus["DpadRadial3"] = new List<string>
				{
					Buttons.DPadDown.ToString()
				};
				PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepadUI].KeyStatus["DpadRadial4"] = new List<string>
				{
					Buttons.DPadLeft.ToString()
				};
			}
		}

		private void KeyboardButtonClick(UIMouseEvent evt, UIElement listeningElement)
		{
			this._buttonKeyboard.SetFrame(this._KeyboardGamepadTexture.Frame(2, 2, 0, 0));
			this._buttonGamepad.SetFrame(this._KeyboardGamepadTexture.Frame(2, 2, 1, 1));
			this.OnKeyboard = true;
			this.FillList();
		}

		private void GamepadButtonClick(UIMouseEvent evt, UIElement listeningElement)
		{
			this._buttonKeyboard.SetFrame(this._KeyboardGamepadTexture.Frame(2, 2, 0, 1));
			this._buttonGamepad.SetFrame(this._KeyboardGamepadTexture.Frame(2, 2, 1, 0));
			this.OnKeyboard = false;
			this.FillList();
		}

		private void ManageBorderKeyboardOn(UIMouseEvent evt, UIElement listeningElement)
		{
			this._buttonBorder2.Color = ((!this.OnKeyboard) ? Color.Silver : Color.Black);
			this._buttonBorder1.Color = Main.OurFavoriteColor;
		}

		private void ManageBorderKeyboardOff(UIMouseEvent evt, UIElement listeningElement)
		{
			this._buttonBorder2.Color = ((!this.OnKeyboard) ? Color.Silver : Color.Black);
			this._buttonBorder1.Color = (this.OnKeyboard ? Color.Silver : Color.Black);
		}

		private void ManageBorderGamepadOn(UIMouseEvent evt, UIElement listeningElement)
		{
			this._buttonBorder1.Color = (this.OnKeyboard ? Color.Silver : Color.Black);
			this._buttonBorder2.Color = Main.OurFavoriteColor;
		}

		private void ManageBorderGamepadOff(UIMouseEvent evt, UIElement listeningElement)
		{
			this._buttonBorder1.Color = (this.OnKeyboard ? Color.Silver : Color.Black);
			this._buttonBorder2.Color = ((!this.OnKeyboard) ? Color.Silver : Color.Black);
		}

		private void VsGameplayButtonClick(UIMouseEvent evt, UIElement listeningElement)
		{
			this._buttonVs1.SetFrame(this._GameplayVsUITexture.Frame(2, 2, 0, 0));
			this._buttonVs2.SetFrame(this._GameplayVsUITexture.Frame(2, 2, 1, 1));
			this.OnGameplay = true;
			this.FillList();
		}

		private void VsMenuButtonClick(UIMouseEvent evt, UIElement listeningElement)
		{
			this._buttonVs1.SetFrame(this._GameplayVsUITexture.Frame(2, 2, 0, 1));
			this._buttonVs2.SetFrame(this._GameplayVsUITexture.Frame(2, 2, 1, 0));
			this.OnGameplay = false;
			this.FillList();
		}

		private void ManageBorderGameplayOn(UIMouseEvent evt, UIElement listeningElement)
		{
			this._buttonBorderVs2.Color = ((!this.OnGameplay) ? Color.Silver : Color.Black);
			this._buttonBorderVs1.Color = Main.OurFavoriteColor;
		}

		private void ManageBorderGameplayOff(UIMouseEvent evt, UIElement listeningElement)
		{
			this._buttonBorderVs2.Color = ((!this.OnGameplay) ? Color.Silver : Color.Black);
			this._buttonBorderVs1.Color = (this.OnGameplay ? Color.Silver : Color.Black);
		}

		private void ManageBorderMenuOn(UIMouseEvent evt, UIElement listeningElement)
		{
			this._buttonBorderVs1.Color = (this.OnGameplay ? Color.Silver : Color.Black);
			this._buttonBorderVs2.Color = Main.OurFavoriteColor;
		}

		private void ManageBorderMenuOff(UIMouseEvent evt, UIElement listeningElement)
		{
			this._buttonBorderVs1.Color = (this.OnGameplay ? Color.Silver : Color.Black);
			this._buttonBorderVs2.Color = ((!this.OnGameplay) ? Color.Silver : Color.Black);
		}

		private void profileButtonClick(UIMouseEvent evt, UIElement listeningElement)
		{
			string name = PlayerInput.CurrentProfile.Name;
			List<string> list = PlayerInput.Profiles.Keys.ToList<string>();
			int num = list.IndexOf(name);
			num++;
			if (num >= list.Count)
			{
				num -= list.Count;
			}
			PlayerInput.SetSelectedProfile(list[num]);
		}

		private void FadedMouseOver(UIMouseEvent evt, UIElement listeningElement)
		{
			Main.PlaySound(12, -1, -1, 1, 1f, 0f);
			((UIPanel)evt.Target).BackgroundColor = new Color(73, 94, 171);
		}

		private void FadedMouseOut(UIMouseEvent evt, UIElement listeningElement)
		{
			((UIPanel)evt.Target).BackgroundColor = new Color(63, 82, 151) * 0.7f;
		}

		private void GoBackClick(UIMouseEvent evt, UIElement listeningElement)
		{
			Main.menuMode = 1127;
			IngameFancyUI.Close();
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);
			this.SetupGamepadPoints(spriteBatch);
		}

		private void SetupGamepadPoints(SpriteBatch spriteBatch)
		{
			UILinkPointNavigator.Shortcuts.BackButtonCommand = 4;
			int num = 3000;
			UILinkPointNavigator.SetPosition(num, this._buttonBack.GetInnerDimensions().ToRectangle().Center.ToVector2());
			UILinkPointNavigator.SetPosition(num + 1, this._buttonKeyboard.GetInnerDimensions().ToRectangle().Center.ToVector2());
			UILinkPointNavigator.SetPosition(num + 2, this._buttonGamepad.GetInnerDimensions().ToRectangle().Center.ToVector2());
			UILinkPointNavigator.SetPosition(num + 3, this._buttonProfile.GetInnerDimensions().ToRectangle().Center.ToVector2());
			UILinkPointNavigator.SetPosition(num + 4, this._buttonVs1.GetInnerDimensions().ToRectangle().Center.ToVector2());
			UILinkPointNavigator.SetPosition(num + 5, this._buttonVs2.GetInnerDimensions().ToRectangle().Center.ToVector2());
			int num2 = num;
			UILinkPoint uILinkPoint = UILinkPointNavigator.Points[num2];
			uILinkPoint.Unlink();
			uILinkPoint.Up = num + 6;
			num2 = num + 1;
			uILinkPoint = UILinkPointNavigator.Points[num2];
			uILinkPoint.Unlink();
			uILinkPoint.Right = num + 2;
			uILinkPoint.Down = num + 6;
			num2 = num + 2;
			uILinkPoint = UILinkPointNavigator.Points[num2];
			uILinkPoint.Unlink();
			uILinkPoint.Left = num + 1;
			uILinkPoint.Right = num + 4;
			uILinkPoint.Down = num + 6;
			num2 = num + 4;
			uILinkPoint = UILinkPointNavigator.Points[num2];
			uILinkPoint.Unlink();
			uILinkPoint.Left = num + 2;
			uILinkPoint.Right = num + 5;
			uILinkPoint.Down = num + 6;
			num2 = num + 5;
			uILinkPoint = UILinkPointNavigator.Points[num2];
			uILinkPoint.Unlink();
			uILinkPoint.Left = num + 4;
			uILinkPoint.Right = num + 3;
			uILinkPoint.Down = num + 6;
			num2 = num + 3;
			uILinkPoint = UILinkPointNavigator.Points[num2];
			uILinkPoint.Unlink();
			uILinkPoint.Left = num + 5;
			uILinkPoint.Down = num + 6;
			Rectangle clippingRectangle = this._uilist.GetClippingRectangle(spriteBatch);
			Vector2 minimum = clippingRectangle.TopLeft();
			Vector2 maximum = clippingRectangle.BottomRight();
			List<SnapPoint> snapPoints = this._uilist.GetSnapPoints();
			for (int i = 0; i < snapPoints.Count; i++)
			{
				if (!snapPoints[i].Position.Between(minimum, maximum))
				{
					snapPoints.Remove(snapPoints[i]);
					i--;
				}
			}
			snapPoints.Sort((SnapPoint x, SnapPoint y) => x.ID.CompareTo(y.ID));
			for (int j = 0; j < snapPoints.Count; j++)
			{
				num2 = num + 6 + j;
				if (snapPoints[j].Name == "Thin")
				{
					uILinkPoint = UILinkPointNavigator.Points[num2];
					uILinkPoint.Unlink();
					UILinkPointNavigator.SetPosition(num2, snapPoints[j].Position);
					uILinkPoint.Right = num2 + 1;
					uILinkPoint.Down = ((j < snapPoints.Count - 2) ? (num2 + 2) : num);
					uILinkPoint.Up = ((j < 2) ? (num + 1) : ((snapPoints[j - 1].Name == "Wide") ? (num2 - 1) : (num2 - 2)));
					UILinkPointNavigator.Points[num].Up = num2;
					UILinkPointNavigator.Shortcuts.FANCYUI_HIGHEST_INDEX = num2;
					j++;
					if (j < snapPoints.Count)
					{
						num2 = num + 6 + j;
						uILinkPoint = UILinkPointNavigator.Points[num2];
						uILinkPoint.Unlink();
						UILinkPointNavigator.SetPosition(num2, snapPoints[j].Position);
						uILinkPoint.Left = num2 - 1;
						uILinkPoint.Down = ((j < snapPoints.Count - 1) ? ((snapPoints[j + 1].Name == "Wide") ? (num2 + 1) : (num2 + 2)) : num);
						uILinkPoint.Up = ((j < 2) ? (num + 1) : (num2 - 2));
						UILinkPointNavigator.Shortcuts.FANCYUI_HIGHEST_INDEX = num2;
					}
				}
				else
				{
					uILinkPoint = UILinkPointNavigator.Points[num2];
					uILinkPoint.Unlink();
					UILinkPointNavigator.SetPosition(num2, snapPoints[j].Position);
					uILinkPoint.Down = ((j < snapPoints.Count - 1) ? (num2 + 1) : num);
					uILinkPoint.Up = ((j < 1) ? (num + 1) : ((snapPoints[j - 1].Name == "Wide") ? (num2 - 1) : (num2 - 2)));
					UILinkPointNavigator.Shortcuts.FANCYUI_HIGHEST_INDEX = num2;
					UILinkPointNavigator.Points[num].Up = num2;
				}
			}
			if (UIManageControls.ForceMoveTo != -1)
			{
				UILinkPointNavigator.ChangePoint((int)MathHelper.Clamp((float)UIManageControls.ForceMoveTo, (float)num, (float)UILinkPointNavigator.Shortcuts.FANCYUI_HIGHEST_INDEX));
				UIManageControls.ForceMoveTo = -1;
			}
		}
	}
}
