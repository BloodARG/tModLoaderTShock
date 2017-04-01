using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.GameContent.UI;
using Terraria.GameContent.UI.Chat;
using Terraria.GameContent.UI.States;
using Terraria.ID;
using Terraria.IO;
using Terraria.Social;
using Terraria.UI;
using Terraria.UI.Gamepad;

namespace Terraria.GameInput
{
	public class PlayerInput
	{
		public class MiscSettingsTEMP
		{
			public static bool HotbarRadialShouldBeUsed = true;
		}

		public static TriggersPack Triggers = new TriggersPack();

		public static List<string> KnownTriggers = new List<string>
		{
			"MouseLeft",
			"MouseRight",
			"Up",
			"Down",
			"Left",
			"Right",
			"Jump",
			"Throw",
			"Inventory",
			"Grapple",
			"SmartSelect",
			"SmartCursor",
			"QuickMount",
			"QuickHeal",
			"QuickMana",
			"QuickBuff",
			"MapZoomIn",
			"MapZoomOut",
			"MapAlphaUp",
			"MapAlphaDown",
			"MapFull",
			"MapStyle",
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
			"HotbarMinus",
			"HotbarPlus",
			"DpadRadial1",
			"DpadRadial2",
			"DpadRadial3",
			"DpadRadial4",
			"RadialHotbar",
			"RadialQuickbar",
			"DpadSnap1",
			"DpadSnap2",
			"DpadSnap3",
			"DpadSnap4",
			"MenuUp",
			"MenuDown",
			"MenuLeft",
			"MenuRight",
			"LockOn"
		};

		private static bool _canReleaseRebindingLock = true;

		private static int _memoOfLastPoint = -1;

		public static int NavigatorRebindingLock = 0;

		public static string BlockedKey = "";

		private static string _listeningTrigger;

		private static InputMode _listeningInputMode;

		public static Dictionary<string, PlayerInputProfile> Profiles = new Dictionary<string, PlayerInputProfile>();

		public static Dictionary<string, PlayerInputProfile> OriginalProfiles = new Dictionary<string, PlayerInputProfile>();

		private static string _selectedProfile;

		private static PlayerInputProfile _currentProfile;

		public static InputMode CurrentInputMode = InputMode.Keyboard;

		private static Buttons[] ButtonsGamepad = (Buttons[])Enum.GetValues(typeof(Buttons));

		public static bool GrappleAndInteractAreShared = false;

		private static string _invalidatorCheck = "";

		private static bool _lastActivityState = false;

		public static MouseState MouseInfo;

		public static MouseState MouseInfoOld;

		public static int MouseX;

		public static int MouseY;

		public static bool LockTileUseButton = false;

		public static List<string> MouseKeys = new List<string>();

		public static int PreUIX = 0;

		public static int PreUIY = 0;

		public static int PreLockOnX = 0;

		public static int PreLockOnY = 0;

		public static int ScrollWheelValue;

		public static int ScrollWheelValueOld;

		public static int ScrollWheelDelta;

		public static int ScrollWheelDeltaForUI;

		public static bool GamepadAllowScrolling;

		public static int GamepadScrollValue;

		public static Vector2 GamepadThumbstickLeft = Vector2.Zero;

		public static Vector2 GamepadThumbstickRight = Vector2.Zero;

		private static bool _InBuildingMode = false;

		private static int _UIPointForBuildingMode = -1;

		public static bool WritingText = false;

		private static int[] DpadSnapCooldown;

		public static string ListeningTrigger
		{
			get
			{
				return PlayerInput._listeningTrigger;
			}
		}

		public static bool CurrentlyRebinding
		{
			get
			{
				return PlayerInput._listeningTrigger != null;
			}
		}

		public static bool InvisibleGamepadInMenus
		{
			get
			{
				bool flag = Main.gameMenu || Main.ingameOptionsWindow || Main.playerInventory || Main.player[Main.myPlayer].talkNPC != -1 || Main.player[Main.myPlayer].sign != -1;
				return (flag && !PlayerInput._InBuildingMode && Main.InvisibleCursorForGamepad) || (PlayerInput.CursorIsBusy && !PlayerInput._InBuildingMode);
			}
		}

		public static PlayerInputProfile CurrentProfile
		{
			get
			{
				return PlayerInput._currentProfile;
			}
		}

		public static KeyConfiguration ProfileGamepadUI
		{
			get
			{
				return PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepadUI];
			}
		}

		public static bool UsingGamepad
		{
			get
			{
				return PlayerInput.CurrentInputMode == InputMode.XBoxGamepad || PlayerInput.CurrentInputMode == InputMode.XBoxGamepadUI;
			}
		}

		public static bool UsingGamepadUI
		{
			get
			{
				return PlayerInput.CurrentInputMode == InputMode.XBoxGamepadUI;
			}
		}

		public static bool IgnoreMouseInterface
		{
			get
			{
				return PlayerInput.UsingGamepad && !UILinkPointNavigator.Available;
			}
		}

		public static bool InBuildingMode
		{
			get
			{
				return PlayerInput._InBuildingMode;
			}
		}

		public static bool CursorIsBusy
		{
			get
			{
				return ItemSlot.CircularRadialOpacity > 0f || ItemSlot.QuicksRadialOpacity > 0f;
			}
		}

		public static void ListenFor(string triggerName, InputMode inputmode)
		{
			PlayerInput._listeningTrigger = triggerName;
			PlayerInput._listeningInputMode = inputmode;
		}

		private static bool InvalidateKeyboardSwap()
		{
			if (PlayerInput._invalidatorCheck.Length == 0)
			{
				return false;
			}
			string text = "";
			Keys[] pressedKeys = Main.keyState.GetPressedKeys();
			Keys[] array = pressedKeys;
			for (int i = 0; i < array.Length; i++)
			{
				Keys keys = array[i];
				text = text + keys.ToString() + ", ";
			}
			if (text == PlayerInput._invalidatorCheck)
			{
				return true;
			}
			PlayerInput._invalidatorCheck = "";
			return false;
		}

		public static void ResetInputsOnActiveStateChange()
		{
			bool isActive = Main.instance.IsActive;
			if (PlayerInput._lastActivityState != isActive)
			{
				PlayerInput.MouseInfo = default(MouseState);
				PlayerInput.MouseInfoOld = default(MouseState);
				Main.keyState = Keyboard.GetState();
				Main.inputText = Keyboard.GetState();
				Main.oldInputText = Keyboard.GetState();
				Main.keyCount = 0;
				PlayerInput.Triggers.Reset();
				PlayerInput.Triggers.Reset();
				string text = "";
				Keys[] pressedKeys = Main.keyState.GetPressedKeys();
				Keys[] array = pressedKeys;
				for (int i = 0; i < array.Length; i++)
				{
					Keys keys = array[i];
					text = text + keys.ToString() + ", ";
				}
				PlayerInput._invalidatorCheck = text;
			}
			PlayerInput._lastActivityState = isActive;
		}

		public static void EnterBuildingMode()
		{
			PlayerInput._InBuildingMode = true;
			PlayerInput._UIPointForBuildingMode = UILinkPointNavigator.CurrentPoint;
			Main.SmartCursorEnabled = true;
			if (Main.mouseItem.stack <= 0)
			{
				int uIPointForBuildingMode = PlayerInput._UIPointForBuildingMode;
				if (uIPointForBuildingMode < 50 && uIPointForBuildingMode >= 0 && Main.player[Main.myPlayer].inventory[uIPointForBuildingMode].stack > 0)
				{
					Utils.Swap<Item>(ref Main.mouseItem, ref Main.player[Main.myPlayer].inventory[uIPointForBuildingMode]);
				}
			}
		}

		public static void ExitBuildingMode()
		{
			PlayerInput._InBuildingMode = false;
			UILinkPointNavigator.ChangePoint(PlayerInput._UIPointForBuildingMode);
			if (Main.mouseItem.stack > 0 && Main.player[Main.myPlayer].itemAnimation == 0)
			{
				int uIPointForBuildingMode = PlayerInput._UIPointForBuildingMode;
				if (uIPointForBuildingMode < 50 && uIPointForBuildingMode >= 0 && Main.player[Main.myPlayer].inventory[uIPointForBuildingMode].stack <= 0)
				{
					Utils.Swap<Item>(ref Main.mouseItem, ref Main.player[Main.myPlayer].inventory[uIPointForBuildingMode]);
				}
			}
			PlayerInput._UIPointForBuildingMode = -1;
		}

		public static void VerifyBuildingMode()
		{
			if (PlayerInput._InBuildingMode)
			{
				Player player = Main.player[Main.myPlayer];
				bool flag = false;
				if (Main.mouseItem.stack <= 0)
				{
					flag = true;
				}
				if (player.dead)
				{
					flag = true;
				}
				if (flag)
				{
					PlayerInput.ExitBuildingMode();
				}
			}
		}

		public static void SetSelectedProfile(string name)
		{
			if (PlayerInput.Profiles.ContainsKey(name))
			{
				PlayerInput._selectedProfile = name;
				PlayerInput._currentProfile = PlayerInput.Profiles[PlayerInput._selectedProfile];
			}
		}

		public static void Initialize()
		{
			Main.InputProfiles.OnProcessText += new Preferences.TextProcessAction(PlayerInput.PrettyPrintProfiles);
			Player.Hooks.OnEnterWorld += new Action<Player>(PlayerInput.Hook_OnEnterWorld);
			PlayerInputProfile playerInputProfile = new PlayerInputProfile("Redigit's Pick");
			playerInputProfile.Initialize(PresetProfiles.Redigit);
			PlayerInput.Profiles.Add(playerInputProfile.Name, playerInputProfile);
			playerInputProfile = new PlayerInputProfile("Yoraiz0r's Pick");
			playerInputProfile.Initialize(PresetProfiles.Yoraiz0r);
			PlayerInput.Profiles.Add(playerInputProfile.Name, playerInputProfile);
			playerInputProfile = new PlayerInputProfile("Console (Playstation)");
			playerInputProfile.Initialize(PresetProfiles.ConsolePS);
			PlayerInput.Profiles.Add(playerInputProfile.Name, playerInputProfile);
			playerInputProfile = new PlayerInputProfile("Console (Xbox)");
			playerInputProfile.Initialize(PresetProfiles.ConsoleXBox);
			PlayerInput.Profiles.Add(playerInputProfile.Name, playerInputProfile);
			playerInputProfile = new PlayerInputProfile("Custom");
			playerInputProfile.Initialize(PresetProfiles.Redigit);
			PlayerInput.Profiles.Add(playerInputProfile.Name, playerInputProfile);
			playerInputProfile = new PlayerInputProfile("Redigit's Pick");
			playerInputProfile.Initialize(PresetProfiles.Redigit);
			PlayerInput.OriginalProfiles.Add(playerInputProfile.Name, playerInputProfile);
			playerInputProfile = new PlayerInputProfile("Yoraiz0r's Pick");
			playerInputProfile.Initialize(PresetProfiles.Yoraiz0r);
			PlayerInput.OriginalProfiles.Add(playerInputProfile.Name, playerInputProfile);
			playerInputProfile = new PlayerInputProfile("Console (Playstation)");
			playerInputProfile.Initialize(PresetProfiles.ConsolePS);
			PlayerInput.OriginalProfiles.Add(playerInputProfile.Name, playerInputProfile);
			playerInputProfile = new PlayerInputProfile("Console (Xbox)");
			playerInputProfile.Initialize(PresetProfiles.ConsoleXBox);
			PlayerInput.OriginalProfiles.Add(playerInputProfile.Name, playerInputProfile);
			PlayerInput.SetSelectedProfile("Custom");
			PlayerInput.Triggers.Initialize();
		}

		public static void Hook_OnEnterWorld(Player player)
		{
			if (PlayerInput.UsingGamepad && player.whoAmI == Main.myPlayer)
			{
				Main.SmartCursorEnabled = true;
			}
		}

		public static bool Save()
		{
			Main.InputProfiles.Clear();
			Main.InputProfiles.Put("Selected Profile", PlayerInput._selectedProfile);
			foreach (KeyValuePair<string, PlayerInputProfile> current in PlayerInput.Profiles)
			{
				Main.InputProfiles.Put(current.Value.Name, current.Value.Save());
			}
			return Main.InputProfiles.Save(true);
		}

		public static void Load()
		{
			Main.InputProfiles.Load();
			Dictionary<string, PlayerInputProfile> dictionary = new Dictionary<string, PlayerInputProfile>();
			string text = null;
			Main.InputProfiles.Get<string>("Selected Profile", ref text);
			List<string> allKeys = Main.InputProfiles.GetAllKeys();
			for (int i = 0; i < allKeys.Count; i++)
			{
				string text2 = allKeys[i];
				if (!(text2 == "Selected Profile") && !string.IsNullOrEmpty(text2))
				{
					Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
					Main.InputProfiles.Get<Dictionary<string, object>>(text2, ref dictionary2);
					if (dictionary2.Count > 0)
					{
						PlayerInputProfile playerInputProfile = new PlayerInputProfile(text2);
						playerInputProfile.Initialize(PresetProfiles.None);
						if (playerInputProfile.Load(dictionary2))
						{
							dictionary.Add(text2, playerInputProfile);
						}
					}
				}
			}
			if (dictionary.Count > 0)
			{
				PlayerInput.Profiles = dictionary;
				if (!string.IsNullOrEmpty(text) && PlayerInput.Profiles.ContainsKey(text))
				{
					PlayerInput.SetSelectedProfile(text);
					return;
				}
				PlayerInput.SetSelectedProfile(PlayerInput.Profiles.Keys.First<string>());
			}
		}

		public static void ManageVersion_1_3()
		{
			PlayerInputProfile playerInputProfile = PlayerInput.Profiles["Custom"];
			string[,] array = new string[20, 2];
			array[0, 0] = "KeyUp";
			array[0, 1] = "Up";
			array[1, 0] = "KeyDown";
			array[1, 1] = "Down";
			array[2, 0] = "KeyLeft";
			array[2, 1] = "Left";
			array[3, 0] = "KeyRight";
			array[3, 1] = "Right";
			array[4, 0] = "KeyJump";
			array[4, 1] = "Jump";
			array[5, 0] = "KeyThrowItem";
			array[5, 1] = "Throw";
			array[6, 0] = "KeyInventory";
			array[6, 1] = "Inventory";
			array[7, 0] = "KeyQuickHeal";
			array[7, 1] = "QuickHeal";
			array[8, 0] = "KeyQuickMana";
			array[8, 1] = "QuickMana";
			array[9, 0] = "KeyQuickBuff";
			array[9, 1] = "QuickBuff";
			array[10, 0] = "KeyUseHook";
			array[10, 1] = "Grapple";
			array[11, 0] = "KeyAutoSelect";
			array[11, 1] = "SmartSelect";
			array[12, 0] = "KeySmartCursor";
			array[12, 1] = "SmartCursor";
			array[13, 0] = "KeyMount";
			array[13, 1] = "QuickMount";
			array[14, 0] = "KeyMapStyle";
			array[14, 1] = "MapStyle";
			array[15, 0] = "KeyFullscreenMap";
			array[15, 1] = "MapFull";
			array[16, 0] = "KeyMapZoomIn";
			array[16, 1] = "MapZoomIn";
			array[17, 0] = "KeyMapZoomOut";
			array[17, 1] = "MapZoomOut";
			array[18, 0] = "KeyMapAlphaUp";
			array[18, 1] = "MapAlphaUp";
			array[19, 0] = "KeyMapAlphaDown";
			array[19, 1] = "MapAlphaDown";
			string[,] array2 = array;
			for (int i = 0; i < array2.GetLength(0); i++)
			{
				string text = null;
				Main.Configuration.Get<string>(array2[i, 0], ref text);
				if (text != null)
				{
					playerInputProfile.InputModes[InputMode.Keyboard].KeyStatus[array2[i, 1]] = new List<string>
					{
						text
					};
					playerInputProfile.InputModes[InputMode.KeyboardUI].KeyStatus[array2[i, 1]] = new List<string>
					{
						text
					};
				}
			}
		}

		public static void UpdateInput()
		{
			PlayerInput.Triggers.Reset();
			PlayerInput.ScrollWheelValueOld = PlayerInput.ScrollWheelValue;
			PlayerInput.ScrollWheelValue = 0;
			PlayerInput.GamepadThumbstickLeft = Vector2.Zero;
			PlayerInput.GamepadThumbstickRight = Vector2.Zero;
			PlayerInput.GrappleAndInteractAreShared = (PlayerInput.UsingGamepad && PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepad].DoGrappleAndInteractShareTheSameKey);
			if (PlayerInput.InBuildingMode && !PlayerInput.UsingGamepad)
			{
				PlayerInput.ExitBuildingMode();
			}
			if (PlayerInput._canReleaseRebindingLock && PlayerInput.NavigatorRebindingLock > 0)
			{
				PlayerInput.NavigatorRebindingLock--;
				PlayerInput.Triggers.Current.UsedMovementKey = false;
				if (PlayerInput.NavigatorRebindingLock == 0 && PlayerInput._memoOfLastPoint != -1)
				{
					UIManageControls.ForceMoveTo = PlayerInput._memoOfLastPoint;
					PlayerInput._memoOfLastPoint = -1;
				}
			}
			PlayerInput._canReleaseRebindingLock = true;
			PlayerInput.VerifyBuildingMode();
			PlayerInput.MouseInput();
			PlayerInput.KeyboardInput();
			PlayerInput.GamePadInput();
			PlayerInput.Triggers.Update();
			PlayerInput.PostInput();
			PlayerInput.ScrollWheelDelta = PlayerInput.ScrollWheelValue - PlayerInput.ScrollWheelValueOld;
			PlayerInput.ScrollWheelDeltaForUI = PlayerInput.ScrollWheelDelta;
			PlayerInput.WritingText = false;
			PlayerInput.UpdateMainMouse();
			Main.mouseLeft = PlayerInput.Triggers.Current.MouseLeft;
			Main.mouseRight = PlayerInput.Triggers.Current.MouseRight;
		}

		public static void UpdateMainMouse()
		{
			Main.lastMouseX = Main.mouseX;
			Main.lastMouseY = Main.mouseY;
			Main.mouseX = PlayerInput.MouseX;
			Main.mouseY = PlayerInput.MouseY;
		}

		private static void GamePadInput()
		{
			bool flag = false;
			PlayerInput.ScrollWheelValue += PlayerInput.GamepadScrollValue;
			GamePadState gamePadState = default(GamePadState);
			bool flag2 = false;
			for (int i = 0; i < 4; i++)
			{
				GamePadState state = GamePad.GetState((PlayerIndex)i);
				if (state.IsConnected)
				{
					flag2 = true;
					gamePadState = state;
					break;
				}
			}
			if (!flag2)
			{
				return;
			}
			if (!Main.instance.IsActive && !Main.AllowUnfocusedInputOnGamepad)
			{
				return;
			}
			Player player = Main.player[Main.myPlayer];
			bool flag3 = UILinkPointNavigator.Available && !PlayerInput.InBuildingMode;
			InputMode inputMode = InputMode.XBoxGamepad;
			if (Main.gameMenu || flag3 || player.talkNPC != -1 || player.sign != -1 || IngameFancyUI.CanCover())
			{
				inputMode = InputMode.XBoxGamepadUI;
			}
			if (!Main.gameMenu && PlayerInput.InBuildingMode)
			{
				inputMode = InputMode.XBoxGamepad;
			}
			if (PlayerInput.CurrentInputMode == InputMode.XBoxGamepad && inputMode == InputMode.XBoxGamepadUI)
			{
				flag = true;
			}
			if (PlayerInput.CurrentInputMode == InputMode.XBoxGamepadUI && inputMode == InputMode.XBoxGamepad)
			{
				flag = true;
			}
			if (flag)
			{
				PlayerInput.CurrentInputMode = inputMode;
			}
			KeyConfiguration keyConfiguration = PlayerInput.CurrentProfile.InputModes[inputMode];
			int num = 2145386496;
			for (int j = 0; j < PlayerInput.ButtonsGamepad.Length; j++)
			{
				if ((num & (int)PlayerInput.ButtonsGamepad[j]) <= 0 && gamePadState.IsButtonDown(PlayerInput.ButtonsGamepad[j]))
				{
					if (PlayerInput.CheckRebindingProcessGamepad(PlayerInput.ButtonsGamepad[j].ToString()))
					{
						return;
					}
					keyConfiguration.Processkey(PlayerInput.Triggers.Current, PlayerInput.ButtonsGamepad[j].ToString());
					flag = true;
				}
			}
			PlayerInput.GamepadThumbstickLeft = gamePadState.ThumbSticks.Left * new Vector2(1f, -1f) * new Vector2((float)(PlayerInput.CurrentProfile.LeftThumbstickInvertX.ToDirectionInt() * -1), (float)(PlayerInput.CurrentProfile.LeftThumbstickInvertY.ToDirectionInt() * -1));
			PlayerInput.GamepadThumbstickRight = gamePadState.ThumbSticks.Right * new Vector2(1f, -1f) * new Vector2((float)(PlayerInput.CurrentProfile.RightThumbstickInvertX.ToDirectionInt() * -1), (float)(PlayerInput.CurrentProfile.RightThumbstickInvertY.ToDirectionInt() * -1));
			Vector2 gamepadThumbstickRight = PlayerInput.GamepadThumbstickRight;
			Vector2 gamepadThumbstickLeft = PlayerInput.GamepadThumbstickLeft;
			Vector2 vector = gamepadThumbstickRight;
			if (vector != Vector2.Zero)
			{
				vector.Normalize();
			}
			Vector2 vector2 = gamepadThumbstickLeft;
			if (vector2 != Vector2.Zero)
			{
				vector2.Normalize();
			}
			float num2 = 0.6f;
			float triggersDeadzone = PlayerInput.CurrentProfile.TriggersDeadzone;
			if (inputMode == InputMode.XBoxGamepadUI)
			{
				num2 = 0.4f;
				if (PlayerInput.GamepadAllowScrolling)
				{
					PlayerInput.GamepadScrollValue -= (int)(gamepadThumbstickRight.Y * 16f);
				}
				PlayerInput.GamepadAllowScrolling = false;
			}
			if (Vector2.Dot(-Vector2.UnitX, vector2) >= num2 && gamepadThumbstickLeft.X < -PlayerInput.CurrentProfile.LeftThumbstickDeadzoneX)
			{
				if (PlayerInput.CheckRebindingProcessGamepad(Buttons.LeftThumbstickLeft.ToString()))
				{
					return;
				}
				keyConfiguration.Processkey(PlayerInput.Triggers.Current, Buttons.LeftThumbstickLeft.ToString());
				flag = true;
			}
			if (Vector2.Dot(Vector2.UnitX, vector2) >= num2 && gamepadThumbstickLeft.X > PlayerInput.CurrentProfile.LeftThumbstickDeadzoneX)
			{
				if (PlayerInput.CheckRebindingProcessGamepad(Buttons.LeftThumbstickRight.ToString()))
				{
					return;
				}
				keyConfiguration.Processkey(PlayerInput.Triggers.Current, Buttons.LeftThumbstickRight.ToString());
				flag = true;
			}
			if (Vector2.Dot(-Vector2.UnitY, vector2) >= num2 && gamepadThumbstickLeft.Y < -PlayerInput.CurrentProfile.LeftThumbstickDeadzoneY)
			{
				if (PlayerInput.CheckRebindingProcessGamepad(Buttons.LeftThumbstickUp.ToString()))
				{
					return;
				}
				keyConfiguration.Processkey(PlayerInput.Triggers.Current, Buttons.LeftThumbstickUp.ToString());
				flag = true;
			}
			if (Vector2.Dot(Vector2.UnitY, vector2) >= num2 && gamepadThumbstickLeft.Y > PlayerInput.CurrentProfile.LeftThumbstickDeadzoneY)
			{
				if (PlayerInput.CheckRebindingProcessGamepad(Buttons.LeftThumbstickDown.ToString()))
				{
					return;
				}
				keyConfiguration.Processkey(PlayerInput.Triggers.Current, Buttons.LeftThumbstickDown.ToString());
				flag = true;
			}
			if (Vector2.Dot(-Vector2.UnitX, vector) >= num2 && gamepadThumbstickRight.X < -PlayerInput.CurrentProfile.RightThumbstickDeadzoneX)
			{
				if (PlayerInput.CheckRebindingProcessGamepad(Buttons.RightThumbstickLeft.ToString()))
				{
					return;
				}
				keyConfiguration.Processkey(PlayerInput.Triggers.Current, Buttons.RightThumbstickLeft.ToString());
				flag = true;
			}
			if (Vector2.Dot(Vector2.UnitX, vector) >= num2 && gamepadThumbstickRight.X > PlayerInput.CurrentProfile.RightThumbstickDeadzoneX)
			{
				if (PlayerInput.CheckRebindingProcessGamepad(Buttons.RightThumbstickRight.ToString()))
				{
					return;
				}
				keyConfiguration.Processkey(PlayerInput.Triggers.Current, Buttons.RightThumbstickRight.ToString());
				flag = true;
			}
			if (Vector2.Dot(-Vector2.UnitY, vector) >= num2 && gamepadThumbstickRight.Y < -PlayerInput.CurrentProfile.RightThumbstickDeadzoneY)
			{
				if (PlayerInput.CheckRebindingProcessGamepad(Buttons.RightThumbstickUp.ToString()))
				{
					return;
				}
				keyConfiguration.Processkey(PlayerInput.Triggers.Current, Buttons.RightThumbstickUp.ToString());
				flag = true;
			}
			if (Vector2.Dot(Vector2.UnitY, vector) >= num2 && gamepadThumbstickRight.Y > PlayerInput.CurrentProfile.RightThumbstickDeadzoneY)
			{
				if (PlayerInput.CheckRebindingProcessGamepad(Buttons.RightThumbstickDown.ToString()))
				{
					return;
				}
				keyConfiguration.Processkey(PlayerInput.Triggers.Current, Buttons.RightThumbstickDown.ToString());
				flag = true;
			}
			if (gamePadState.Triggers.Left > triggersDeadzone)
			{
				if (PlayerInput.CheckRebindingProcessGamepad(Buttons.LeftTrigger.ToString()))
				{
					return;
				}
				keyConfiguration.Processkey(PlayerInput.Triggers.Current, Buttons.LeftTrigger.ToString());
				flag = true;
			}
			if (gamePadState.Triggers.Right > triggersDeadzone)
			{
				if (PlayerInput.CheckRebindingProcessGamepad(Buttons.RightTrigger.ToString()))
				{
					return;
				}
				keyConfiguration.Processkey(PlayerInput.Triggers.Current, Buttons.RightTrigger.ToString());
				flag = true;
			}
			bool flag4 = ItemID.Sets.GamepadWholeScreenUseRange[player.inventory[player.selectedItem].type] || player.scope;
			int num3 = player.inventory[player.selectedItem].tileBoost + ItemID.Sets.GamepadExtraRange[player.inventory[player.selectedItem].type];
			if (player.yoyoString && ItemID.Sets.Yoyo[player.inventory[player.selectedItem].type])
			{
				num3 += 5;
			}
			else if (player.inventory[player.selectedItem].createTile < 0 && player.inventory[player.selectedItem].createWall <= 0 && player.inventory[player.selectedItem].shoot > 0)
			{
				num3 += 10;
			}
			else if (player.controlTorch)
			{
				num3++;
			}
			if (flag4)
			{
				num3 += 30;
			}
			if (player.mount.Active && player.mount.Type == 8)
			{
				num3 = 10;
			}
			bool flag5 = false;
			bool flag6 = !Main.gameMenu && !flag3 && Main.SmartCursorEnabled;
			if (!PlayerInput.CursorIsBusy)
			{
				bool flag7 = Main.mapFullscreen || (!Main.gameMenu && !flag3);
				int num4 = Main.screenWidth / 2;
				int num5 = Main.screenHeight / 2;
				if (!Main.mapFullscreen && flag7 && !flag4)
				{
					Point point = Main.ReverseGravitySupport(player.Center - Main.screenPosition, 0f).ToPoint();
					num4 = point.X;
					num5 = point.Y;
				}
				if (player.velocity == Vector2.Zero && gamepadThumbstickLeft == Vector2.Zero && gamepadThumbstickRight == Vector2.Zero && flag6)
				{
					num4 += player.direction * 10;
				}
				if (gamepadThumbstickRight != Vector2.Zero && flag7)
				{
					Vector2 vector3 = new Vector2(8f);
					if (!Main.gameMenu && Main.mapFullscreen)
					{
						vector3 = new Vector2(16f);
					}
					if (flag6)
					{
						vector3 = new Vector2((float)(Player.tileRangeX * 16), (float)(Player.tileRangeY * 16));
						if (num3 != 0)
						{
							vector3 += new Vector2((float)(num3 * 16), (float)(num3 * 16));
						}
						if (flag4)
						{
							vector3 = new Vector2((float)(Math.Max(Main.screenWidth, Main.screenHeight) / 2));
						}
					}
					else if (!Main.mapFullscreen)
					{
						if (player.inventory[player.selectedItem].mech)
						{
							vector3 += Vector2.Zero;
						}
						else
						{
							vector3 += new Vector2((float)num3) / 4f;
						}
					}
					Vector2 vector4 = gamepadThumbstickRight * vector3;
					int num6 = PlayerInput.MouseX - num4;
					int num7 = PlayerInput.MouseY - num5;
					if (flag6)
					{
						num6 = 0;
						num7 = 0;
					}
					num6 += (int)vector4.X;
					num7 += (int)vector4.Y;
					PlayerInput.MouseX = num6 + num4;
					PlayerInput.MouseY = num7 + num5;
					flag = true;
					flag5 = true;
				}
				if (gamepadThumbstickLeft != Vector2.Zero && flag7)
				{
					float scaleFactor = 8f;
					if (!Main.gameMenu && Main.mapFullscreen)
					{
						scaleFactor = 3f;
					}
					if (Main.mapFullscreen)
					{
						Vector2 value = gamepadThumbstickLeft * scaleFactor;
						Main.mapFullscreenPos += value * scaleFactor * (1f / Main.mapFullscreenScale);
					}
					else if (!flag5 && Main.SmartCursorEnabled)
					{
						Vector2 vector5 = gamepadThumbstickLeft * new Vector2((float)(Player.tileRangeX * 16), (float)(Player.tileRangeY * 16));
						if (num3 != 0)
						{
							vector5 = gamepadThumbstickLeft * new Vector2((float)((Player.tileRangeX + num3) * 16), (float)((Player.tileRangeY + num3) * 16));
						}
						if (flag4)
						{
							vector5 = new Vector2((float)(Math.Max(Main.screenWidth, Main.screenHeight) / 2)) * gamepadThumbstickLeft;
						}
						int num8 = (int)vector5.X;
						int num9 = (int)vector5.Y;
						PlayerInput.MouseX = num8 + num4;
						PlayerInput.MouseY = num9 + num5;
					}
					flag = true;
				}
				if (PlayerInput.CurrentInputMode == InputMode.XBoxGamepad)
				{
					PlayerInput.HandleDpadSnap();
					int num10 = PlayerInput.MouseX - num4;
					int num11 = PlayerInput.MouseY - num5;
					if (!Main.gameMenu && !flag3)
					{
						if (flag4 && !Main.mapFullscreen)
						{
							int num12 = Main.screenWidth / 2;
							int num13 = Main.screenHeight / 2;
							num10 = Utils.Clamp<int>(num10, -num12, num12);
							num11 = Utils.Clamp<int>(num11, -num13, num13);
						}
						else
						{
							num10 = Utils.Clamp<int>(num10, -(Player.tileRangeX + num3) * 16, (Player.tileRangeX + num3) * 16);
							num11 = Utils.Clamp<int>(num11, -(Player.tileRangeY + num3) * 16, (Player.tileRangeY + num3) * 16);
						}
						if (flag6 && (!flag || flag4))
						{
							float num14 = 0.81f;
							if (flag4)
							{
								num14 = 0.95f;
							}
							num10 = (int)((float)num10 * num14);
							num11 = (int)((float)num11 * num14);
						}
					}
					else
					{
						num10 = Utils.Clamp<int>(num10, -num4 + 10, num4 - 10);
						num11 = Utils.Clamp<int>(num11, -num5 + 10, num5 - 10);
					}
					PlayerInput.MouseX = num10 + num4;
					PlayerInput.MouseY = num11 + num5;
				}
			}
			if (flag)
			{
				PlayerInput.CurrentInputMode = inputMode;
			}
			if (PlayerInput.CurrentInputMode == InputMode.XBoxGamepad)
			{
				Main.SetCameraGamepadLerp(0.1f);
			}
		}

		private static void MouseInput()
		{
			bool flag = false;
			PlayerInput.MouseInfoOld = PlayerInput.MouseInfo;
			PlayerInput.MouseInfo = Mouse.GetState();
			PlayerInput.ScrollWheelValue += PlayerInput.MouseInfo.ScrollWheelValue;
			int num = PlayerInput.MouseInfo.X - PlayerInput.MouseInfoOld.X;
			int num2 = PlayerInput.MouseInfo.Y - PlayerInput.MouseInfoOld.Y;
			if (num != 0 || num2 != 0 || PlayerInput.MouseInfo.ScrollWheelValue != PlayerInput.MouseInfoOld.ScrollWheelValue)
			{
				PlayerInput.MouseX = PlayerInput.MouseInfo.X;
				PlayerInput.MouseY = PlayerInput.MouseInfo.Y;
				flag = true;
			}
			PlayerInput.MouseKeys.Clear();
			if (Main.instance.IsActive)
			{
				if (PlayerInput.MouseInfo.LeftButton == ButtonState.Pressed)
				{
					PlayerInput.MouseKeys.Add("Mouse1");
					flag = true;
				}
				if (PlayerInput.MouseInfo.RightButton == ButtonState.Pressed)
				{
					PlayerInput.MouseKeys.Add("Mouse2");
					flag = true;
				}
				if (PlayerInput.MouseInfo.MiddleButton == ButtonState.Pressed)
				{
					PlayerInput.MouseKeys.Add("Mouse3");
					flag = true;
				}
				if (PlayerInput.MouseInfo.XButton1 == ButtonState.Pressed)
				{
					PlayerInput.MouseKeys.Add("Mouse4");
					flag = true;
				}
				if (PlayerInput.MouseInfo.XButton2 == ButtonState.Pressed)
				{
					PlayerInput.MouseKeys.Add("Mouse5");
					flag = true;
				}
			}
			if (flag)
			{
				PlayerInput.CurrentInputMode = InputMode.Mouse;
				PlayerInput.Triggers.Current.UsedMovementKey = false;
			}
		}

		private static void KeyboardInput()
		{
			bool flag = false;
			bool flag2 = false;
			Keys[] pressedKeys = Main.keyState.GetPressedKeys();
			if (PlayerInput.InvalidateKeyboardSwap() && PlayerInput.MouseKeys.Count == 0)
			{
				return;
			}
			for (int i = 0; i < pressedKeys.Length; i++)
			{
				if (pressedKeys[i] == Keys.LeftShift || pressedKeys[i] == Keys.RightShift)
				{
					flag = true;
				}
				else if (pressedKeys[i] == Keys.LeftAlt || pressedKeys[i] == Keys.RightAlt)
				{
					flag2 = true;
				}
			}
			if (Main.blockKey != Keys.None.ToString())
			{
				bool flag3 = false;
				for (int j = 0; j < pressedKeys.Length; j++)
				{
					if (pressedKeys[j].ToString() == Main.blockKey)
					{
						pressedKeys[j] = Keys.None;
						flag3 = true;
					}
				}
				if (!flag3)
				{
					Main.blockKey = Keys.None.ToString();
				}
			}
			KeyConfiguration keyConfiguration = PlayerInput.CurrentProfile.InputModes[InputMode.Keyboard];
			if (Main.gameMenu && !PlayerInput.WritingText)
			{
				keyConfiguration = PlayerInput.CurrentProfile.InputModes[InputMode.KeyboardUI];
			}
			List<string> list = new List<string>(pressedKeys.Length);
			for (int k = 0; k < pressedKeys.Length; k++)
			{
				list.Add(pressedKeys[k].ToString());
			}
			if (PlayerInput.WritingText)
			{
				list.Clear();
			}
			int count = list.Count;
			list.AddRange(PlayerInput.MouseKeys);
			bool flag4 = false;
			for (int l = 0; l < list.Count; l++)
			{
				string newKey = list[l].ToString();
				if (!(list[l] == Keys.Tab.ToString()) || ((!flag || SocialAPI.Mode != SocialMode.Steam) && !flag2))
				{
					if (PlayerInput.CheckRebindingProcessKeyboard(newKey))
					{
						return;
					}
					KeyboardState arg_19B_0 = Main.oldKeyState;
					if (l >= count || !Main.oldKeyState.IsKeyDown(pressedKeys[l]))
					{
						keyConfiguration.Processkey(PlayerInput.Triggers.Current, newKey);
					}
					else
					{
						keyConfiguration.CopyKeyState(PlayerInput.Triggers.Old, PlayerInput.Triggers.Current, newKey);
					}
					flag4 = true;
				}
			}
			if (flag4)
			{
				PlayerInput.CurrentInputMode = InputMode.Keyboard;
			}
		}

		private static void FixDerpedRebinds()
		{
			List<string> list = new List<string>
			{
				"MouseLeft",
				"MouseRight",
				"Inventory"
			};
			foreach (InputMode inputMode in Enum.GetValues(typeof(InputMode)))
			{
				if (inputMode != InputMode.Mouse)
				{
					foreach (string current in list)
					{
						if (PlayerInput.CurrentProfile.InputModes[inputMode].KeyStatus[current].Count < 1)
						{
							string key = "Redigit's Pick";
							if (PlayerInput.OriginalProfiles.ContainsKey(PlayerInput._selectedProfile))
							{
								key = PlayerInput._selectedProfile;
							}
							PlayerInput.CurrentProfile.InputModes[inputMode].KeyStatus[current].AddRange(PlayerInput.OriginalProfiles[key].InputModes[inputMode].KeyStatus[current]);
						}
					}
				}
			}
		}

		private static bool CheckRebindingProcessGamepad(string newKey)
		{
			PlayerInput._canReleaseRebindingLock = false;
			if (PlayerInput.CurrentlyRebinding && PlayerInput._listeningInputMode == InputMode.XBoxGamepad)
			{
				PlayerInput.NavigatorRebindingLock = 3;
				PlayerInput._memoOfLastPoint = UILinkPointNavigator.CurrentPoint;
				Main.PlaySound(12, -1, -1, 1, 1f, 0f);
				if (PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepad].KeyStatus[PlayerInput.ListeningTrigger].Contains(newKey))
				{
					PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepad].KeyStatus[PlayerInput.ListeningTrigger].Remove(newKey);
				}
				else
				{
					PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepad].KeyStatus[PlayerInput.ListeningTrigger] = new List<string>
					{
						newKey
					};
				}
				PlayerInput.ListenFor(null, InputMode.XBoxGamepad);
			}
			if (PlayerInput.CurrentlyRebinding && PlayerInput._listeningInputMode == InputMode.XBoxGamepadUI)
			{
				PlayerInput.NavigatorRebindingLock = 3;
				PlayerInput._memoOfLastPoint = UILinkPointNavigator.CurrentPoint;
				Main.PlaySound(12, -1, -1, 1, 1f, 0f);
				if (PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepadUI].KeyStatus[PlayerInput.ListeningTrigger].Contains(newKey))
				{
					PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepadUI].KeyStatus[PlayerInput.ListeningTrigger].Remove(newKey);
				}
				else
				{
					PlayerInput.CurrentProfile.InputModes[InputMode.XBoxGamepadUI].KeyStatus[PlayerInput.ListeningTrigger] = new List<string>
					{
						newKey
					};
				}
				PlayerInput.ListenFor(null, InputMode.XBoxGamepadUI);
			}
			PlayerInput.FixDerpedRebinds();
			return PlayerInput.NavigatorRebindingLock > 0;
		}

		private static bool CheckRebindingProcessKeyboard(string newKey)
		{
			PlayerInput._canReleaseRebindingLock = false;
			if (PlayerInput.CurrentlyRebinding && PlayerInput._listeningInputMode == InputMode.Keyboard)
			{
				PlayerInput.NavigatorRebindingLock = 3;
				PlayerInput._memoOfLastPoint = UILinkPointNavigator.CurrentPoint;
				Main.PlaySound(12, -1, -1, 1, 1f, 0f);
				if (PlayerInput.CurrentProfile.InputModes[InputMode.Keyboard].KeyStatus[PlayerInput.ListeningTrigger].Contains(newKey))
				{
					PlayerInput.CurrentProfile.InputModes[InputMode.Keyboard].KeyStatus[PlayerInput.ListeningTrigger].Remove(newKey);
				}
				else
				{
					PlayerInput.CurrentProfile.InputModes[InputMode.Keyboard].KeyStatus[PlayerInput.ListeningTrigger] = new List<string>
					{
						newKey
					};
				}
				PlayerInput.ListenFor(null, InputMode.Keyboard);
				Main.blockKey = newKey;
				Main.blockInput = false;
			}
			if (PlayerInput.CurrentlyRebinding && PlayerInput._listeningInputMode == InputMode.KeyboardUI)
			{
				PlayerInput.NavigatorRebindingLock = 3;
				PlayerInput._memoOfLastPoint = UILinkPointNavigator.CurrentPoint;
				Main.PlaySound(12, -1, -1, 1, 1f, 0f);
				if (PlayerInput.CurrentProfile.InputModes[InputMode.KeyboardUI].KeyStatus[PlayerInput.ListeningTrigger].Contains(newKey))
				{
					PlayerInput.CurrentProfile.InputModes[InputMode.KeyboardUI].KeyStatus[PlayerInput.ListeningTrigger].Remove(newKey);
				}
				else
				{
					PlayerInput.CurrentProfile.InputModes[InputMode.KeyboardUI].KeyStatus[PlayerInput.ListeningTrigger] = new List<string>
					{
						newKey
					};
				}
				PlayerInput.ListenFor(null, InputMode.KeyboardUI);
				Main.blockKey = newKey;
				Main.blockInput = false;
			}
			PlayerInput.FixDerpedRebinds();
			return PlayerInput.NavigatorRebindingLock > 0;
		}

		private static void PostInput()
		{
			Main.GamepadCursorAlpha = MathHelper.Clamp(Main.GamepadCursorAlpha + ((Main.SmartCursorEnabled && !UILinkPointNavigator.Available && PlayerInput.GamepadThumbstickLeft == Vector2.Zero && PlayerInput.GamepadThumbstickRight == Vector2.Zero) ? -0.05f : 0.05f), 0f, 1f);
			if (PlayerInput.CurrentProfile.HotbarAllowsRadial)
			{
				int num = PlayerInput.Triggers.Current.HotbarPlus.ToInt() - PlayerInput.Triggers.Current.HotbarMinus.ToInt();
				if (PlayerInput.MiscSettingsTEMP.HotbarRadialShouldBeUsed)
				{
					if (num == 1)
					{
						PlayerInput.Triggers.Current.RadialHotbar = true;
						PlayerInput.Triggers.JustReleased.RadialHotbar = false;
					}
					else if (num == -1)
					{
						PlayerInput.Triggers.Current.RadialQuickbar = true;
						PlayerInput.Triggers.JustReleased.RadialQuickbar = false;
					}
				}
			}
			PlayerInput.MiscSettingsTEMP.HotbarRadialShouldBeUsed = false;
		}

		private static void HandleDpadSnap()
		{
			Vector2 value = Vector2.Zero;
			Player player = Main.player[Main.myPlayer];
			for (int i = 0; i < 4; i++)
			{
				bool flag = false;
				Vector2 value2 = Vector2.Zero;
				if (Main.gameMenu || (UILinkPointNavigator.Available && !PlayerInput.InBuildingMode))
				{
					return;
				}
				switch (i)
				{
				case 0:
					flag = PlayerInput.Triggers.Current.DpadMouseSnap1;
					value2 = -Vector2.UnitY;
					break;
				case 1:
					flag = PlayerInput.Triggers.Current.DpadMouseSnap2;
					value2 = Vector2.UnitX;
					break;
				case 2:
					flag = PlayerInput.Triggers.Current.DpadMouseSnap3;
					value2 = Vector2.UnitY;
					break;
				case 3:
					flag = PlayerInput.Triggers.Current.DpadMouseSnap4;
					value2 = -Vector2.UnitX;
					break;
				}
				if (PlayerInput.DpadSnapCooldown[i] > 0)
				{
					PlayerInput.DpadSnapCooldown[i]--;
				}
				if (flag)
				{
					if (PlayerInput.DpadSnapCooldown[i] == 0)
					{
						int num = 6;
						if (ItemSlot.IsABuildingItem(player.inventory[player.selectedItem]))
						{
							num = player.inventory[player.selectedItem].useTime;
						}
						PlayerInput.DpadSnapCooldown[i] = num;
						value += value2;
					}
				}
				else
				{
					PlayerInput.DpadSnapCooldown[i] = 0;
				}
			}
			if (value != Vector2.Zero)
			{
				Main.SmartCursorEnabled = false;
				Vector2 vec = Main.MouseScreen + Main.screenPosition + value * new Vector2(16f);
				Point point = vec.ToTileCoordinates();
				PlayerInput.MouseX = point.X * 16 + 8 - (int)Main.screenPosition.X;
				PlayerInput.MouseY = point.Y * 16 + 8 - (int)Main.screenPosition.Y;
			}
		}

		public static string ComposeInstructionsForGamepad()
		{
			string text = "";
			if (!PlayerInput.UsingGamepad)
			{
				return text;
			}
			InputMode inputMode = InputMode.XBoxGamepad;
			if (Main.gameMenu || UILinkPointNavigator.Available)
			{
				inputMode = InputMode.XBoxGamepadUI;
			}
			if (PlayerInput.InBuildingMode && !Main.gameMenu)
			{
				inputMode = InputMode.XBoxGamepad;
			}
			KeyConfiguration keyConfiguration = PlayerInput.CurrentProfile.InputModes[inputMode];
			if (Main.mapFullscreen && !Main.gameMenu)
			{
				text += "          ";
				text += PlayerInput.BuildCommand(Lang.misc[56], false, new List<string>[]
				{
					PlayerInput.ProfileGamepadUI.KeyStatus["Inventory"]
				});
				text += PlayerInput.BuildCommand(Lang.inter[118], false, new List<string>[]
				{
					PlayerInput.ProfileGamepadUI.KeyStatus["HotbarMinus"]
				});
				text += PlayerInput.BuildCommand(Lang.inter[119], false, new List<string>[]
				{
					PlayerInput.ProfileGamepadUI.KeyStatus["HotbarPlus"]
				});
				if (Main.netMode == 1 && Main.player[Main.myPlayer].HasItem(2997))
				{
					text += PlayerInput.BuildCommand(Lang.inter[120], false, new List<string>[]
					{
						PlayerInput.ProfileGamepadUI.KeyStatus["MouseRight"]
					});
				}
			}
			else if (inputMode == InputMode.XBoxGamepadUI && !PlayerInput.InBuildingMode)
			{
				text = UILinkPointNavigator.GetInstructions();
			}
			else
			{
				if (!PlayerInput.GrappleAndInteractAreShared || (!WiresUI.Settings.DrawToolModeUI && (!Main.SmartInteractShowingGenuine || (Main.SmartInteractNPC == -1 && (Main.SmartInteractX == -1 || Main.SmartInteractY == -1)))))
				{
					text += PlayerInput.BuildCommand(Lang.misc[57], false, new List<string>[]
					{
						keyConfiguration.KeyStatus["Grapple"]
					});
				}
				text += PlayerInput.BuildCommand(Lang.misc[58], false, new List<string>[]
				{
					keyConfiguration.KeyStatus["Jump"]
				});
				text += PlayerInput.BuildCommand(Lang.misc[59], false, new List<string>[]
				{
					keyConfiguration.KeyStatus["HotbarMinus"],
					keyConfiguration.KeyStatus["HotbarPlus"]
				});
				if (PlayerInput.InBuildingMode)
				{
					text += PlayerInput.BuildCommand(Lang.menu[6], false, new List<string>[]
					{
						keyConfiguration.KeyStatus["Inventory"],
						keyConfiguration.KeyStatus["MouseRight"]
					});
				}
				if (WiresUI.Open)
				{
					text += PlayerInput.BuildCommand(Lang.misc[53], false, new List<string>[]
					{
						keyConfiguration.KeyStatus["MouseLeft"]
					});
					text += PlayerInput.BuildCommand(Lang.misc[56], false, new List<string>[]
					{
						keyConfiguration.KeyStatus["MouseRight"]
					});
				}
				else
				{
					Item item = Main.player[Main.myPlayer].inventory[Main.player[Main.myPlayer].selectedItem];
					if (item.damage > 0 && item.ammo == 0)
					{
						text += PlayerInput.BuildCommand(Lang.misc[60], false, new List<string>[]
						{
							keyConfiguration.KeyStatus["MouseLeft"]
						});
					}
					else if (item.createTile >= 0 || item.createWall > 0)
					{
						text += PlayerInput.BuildCommand(Lang.misc[61], false, new List<string>[]
						{
							keyConfiguration.KeyStatus["MouseLeft"]
						});
					}
					else
					{
						text += PlayerInput.BuildCommand(Lang.misc[63], false, new List<string>[]
						{
							keyConfiguration.KeyStatus["MouseLeft"]
						});
					}
					if (Main.SmartInteractShowingGenuine)
					{
						if (Main.SmartInteractNPC != -1)
						{
							text += PlayerInput.BuildCommand(Lang.misc[80], false, new List<string>[]
							{
								keyConfiguration.KeyStatus["MouseRight"]
							});
						}
						else if (Main.SmartInteractX != -1 && Main.SmartInteractY != -1)
						{
							Tile tile = Main.tile[Main.SmartInteractX, Main.SmartInteractY];
							if (TileID.Sets.TileInteractRead[(int)tile.type])
							{
								text += PlayerInput.BuildCommand(Lang.misc[81], false, new List<string>[]
								{
									keyConfiguration.KeyStatus["MouseRight"]
								});
							}
							else
							{
								text += PlayerInput.BuildCommand(Lang.misc[79], false, new List<string>[]
								{
									keyConfiguration.KeyStatus["MouseRight"]
								});
							}
						}
					}
					else if (WiresUI.Settings.DrawToolModeUI)
					{
						text += PlayerInput.BuildCommand(Lang.misc[89], false, new List<string>[]
						{
							keyConfiguration.KeyStatus["MouseRight"]
						});
					}
				}
			}
			return text;
		}

		public static string BuildCommand(string CommandText, bool Last, params List<string>[] Bindings)
		{
			string text = "";
			if (Bindings.Length == 0)
			{
				return text;
			}
			text += PlayerInput.GenInput(Bindings[0]);
			for (int i = 1; i < Bindings.Length; i++)
			{
				string text2 = PlayerInput.GenInput(Bindings[i]);
				if (text2.Length > 0)
				{
					text = text + "/" + text2;
				}
			}
			if (text.Length > 0)
			{
				text = text + ": " + CommandText;
				if (!Last)
				{
					text += "   ";
				}
			}
			return text;
		}

		private static string GenInput(List<string> list)
		{
			if (list.Count == 0)
			{
				return "";
			}
			string text = GlyphTagHandler.GenerateTag(list[0]);
			for (int i = 1; i < list.Count; i++)
			{
				text = text + "/" + GlyphTagHandler.GenerateTag(list[i]);
			}
			return text;
		}

		public static void NavigatorCachePosition()
		{
			PlayerInput.PreUIX = PlayerInput.MouseX;
			PlayerInput.PreUIY = PlayerInput.MouseY;
		}

		public static void NavigatorUnCachePosition()
		{
			PlayerInput.MouseX = PlayerInput.PreUIX;
			PlayerInput.MouseY = PlayerInput.PreUIY;
		}

		public static void LockOnCachePosition()
		{
			PlayerInput.PreLockOnX = PlayerInput.MouseX;
			PlayerInput.PreLockOnY = PlayerInput.MouseY;
		}

		public static void LockOnUnCachePosition()
		{
			PlayerInput.MouseX = PlayerInput.PreLockOnX;
			PlayerInput.MouseY = PlayerInput.PreLockOnY;
		}

		public static void PrettyPrintProfiles(ref string text)
		{
			string[] array = text.Split(new string[]
			{
				"\r\n"
			}, StringSplitOptions.None);
			for (int i = 0; i < array.Length; i++)
			{
				string text2 = array[i];
				if (text2.Contains(": {"))
				{
					string str = text2.Substring(0, text2.IndexOf('"'));
					string text3 = text2 + "\r\n  ";
					string newValue = text3.Replace(": {\r\n  ", ": \r\n" + str + "{\r\n  ");
					text = text.Replace(text3, newValue);
				}
			}
			text = text.Replace("[\r\n        ", "[");
			text = text.Replace("[\r\n      ", "[");
			text = text.Replace("\"\r\n      ", "\"");
			text = text.Replace("\",\r\n        ", "\", ");
			text = text.Replace("\",\r\n      ", "\", ");
			text = text.Replace("\r\n    ]", "]");
		}

		public static void PrettyPrintProfilesOld(ref string text)
		{
			text = text.Replace(": {\r\n  ", ": \r\n  {\r\n  ");
			text = text.Replace("[\r\n      ", "[");
			text = text.Replace("\"\r\n      ", "\"");
			text = text.Replace("\",\r\n      ", "\", ");
			text = text.Replace("\r\n    ]", "]");
		}

		public static void Reset(KeyConfiguration c, PresetProfiles style, InputMode mode)
		{
			switch (style)
			{
			case PresetProfiles.Redigit:
				switch (mode)
				{
				case InputMode.Keyboard:
					c.KeyStatus["MouseLeft"].Add("Mouse1");
					c.KeyStatus["MouseRight"].Add("Mouse2");
					c.KeyStatus["Up"].Add("W");
					c.KeyStatus["Down"].Add("S");
					c.KeyStatus["Left"].Add("A");
					c.KeyStatus["Right"].Add("D");
					c.KeyStatus["Jump"].Add("Space");
					c.KeyStatus["Inventory"].Add("Escape");
					c.KeyStatus["Grapple"].Add("E");
					c.KeyStatus["SmartSelect"].Add("LeftShift");
					c.KeyStatus["SmartCursor"].Add("LeftControl");
					c.KeyStatus["QuickMount"].Add("R");
					c.KeyStatus["QuickHeal"].Add("H");
					c.KeyStatus["QuickMana"].Add("J");
					c.KeyStatus["QuickBuff"].Add("B");
					c.KeyStatus["MapStyle"].Add("Tab");
					c.KeyStatus["MapFull"].Add("M");
					c.KeyStatus["MapZoomIn"].Add("Add");
					c.KeyStatus["MapZoomOut"].Add("Subtract");
					c.KeyStatus["MapAlphaUp"].Add("PageUp");
					c.KeyStatus["MapAlphaDown"].Add("PageDown");
					c.KeyStatus["Hotbar1"].Add("D1");
					c.KeyStatus["Hotbar2"].Add("D2");
					c.KeyStatus["Hotbar3"].Add("D3");
					c.KeyStatus["Hotbar4"].Add("D4");
					c.KeyStatus["Hotbar5"].Add("D5");
					c.KeyStatus["Hotbar6"].Add("D6");
					c.KeyStatus["Hotbar7"].Add("D7");
					c.KeyStatus["Hotbar8"].Add("D8");
					c.KeyStatus["Hotbar9"].Add("D9");
					c.KeyStatus["Hotbar10"].Add("D0");
					return;
				case InputMode.KeyboardUI:
					c.KeyStatus["MouseLeft"].Add("Mouse1");
					c.KeyStatus["MouseLeft"].Add("Space");
					c.KeyStatus["MouseRight"].Add("Mouse2");
					c.KeyStatus["Up"].Add("W");
					c.KeyStatus["Up"].Add("Up");
					c.KeyStatus["Down"].Add("S");
					c.KeyStatus["Down"].Add("Down");
					c.KeyStatus["Left"].Add("A");
					c.KeyStatus["Left"].Add("Left");
					c.KeyStatus["Right"].Add("D");
					c.KeyStatus["Right"].Add("Right");
					c.KeyStatus["Inventory"].Add(Keys.Escape.ToString());
					c.KeyStatus["MenuUp"].Add(string.Concat(Buttons.DPadUp));
					c.KeyStatus["MenuDown"].Add(string.Concat(Buttons.DPadDown));
					c.KeyStatus["MenuLeft"].Add(string.Concat(Buttons.DPadLeft));
					c.KeyStatus["MenuRight"].Add(string.Concat(Buttons.DPadRight));
					return;
				case InputMode.Mouse:
					break;
				case InputMode.XBoxGamepad:
					c.KeyStatus["MouseLeft"].Add(string.Concat(Buttons.RightTrigger));
					c.KeyStatus["MouseRight"].Add(string.Concat(Buttons.B));
					c.KeyStatus["Up"].Add(string.Concat(Buttons.LeftThumbstickUp));
					c.KeyStatus["Down"].Add(string.Concat(Buttons.LeftThumbstickDown));
					c.KeyStatus["Left"].Add(string.Concat(Buttons.LeftThumbstickLeft));
					c.KeyStatus["Right"].Add(string.Concat(Buttons.LeftThumbstickRight));
					c.KeyStatus["Jump"].Add(string.Concat(Buttons.LeftTrigger));
					c.KeyStatus["Inventory"].Add(string.Concat(Buttons.Y));
					c.KeyStatus["Grapple"].Add(string.Concat(Buttons.B));
					c.KeyStatus["LockOn"].Add(string.Concat(Buttons.X));
					c.KeyStatus["QuickMount"].Add(string.Concat(Buttons.A));
					c.KeyStatus["SmartSelect"].Add(string.Concat(Buttons.LeftStick));
					c.KeyStatus["SmartCursor"].Add(string.Concat(Buttons.RightStick));
					c.KeyStatus["HotbarMinus"].Add(string.Concat(Buttons.LeftShoulder));
					c.KeyStatus["HotbarPlus"].Add(string.Concat(Buttons.RightShoulder));
					c.KeyStatus["MapFull"].Add(string.Concat(Buttons.Start));
					c.KeyStatus["DpadSnap1"].Add(string.Concat(Buttons.DPadUp));
					c.KeyStatus["DpadSnap3"].Add(string.Concat(Buttons.DPadDown));
					c.KeyStatus["DpadSnap4"].Add(string.Concat(Buttons.DPadLeft));
					c.KeyStatus["DpadSnap2"].Add(string.Concat(Buttons.DPadRight));
					c.KeyStatus["MapStyle"].Add(string.Concat(Buttons.Back));
					return;
				case InputMode.XBoxGamepadUI:
					c.KeyStatus["MouseLeft"].Add(string.Concat(Buttons.A));
					c.KeyStatus["MouseRight"].Add(string.Concat(Buttons.LeftShoulder));
					c.KeyStatus["SmartCursor"].Add(string.Concat(Buttons.RightShoulder));
					c.KeyStatus["Up"].Add(string.Concat(Buttons.LeftThumbstickUp));
					c.KeyStatus["Down"].Add(string.Concat(Buttons.LeftThumbstickDown));
					c.KeyStatus["Left"].Add(string.Concat(Buttons.LeftThumbstickLeft));
					c.KeyStatus["Right"].Add(string.Concat(Buttons.LeftThumbstickRight));
					c.KeyStatus["Inventory"].Add(string.Concat(Buttons.B));
					c.KeyStatus["Inventory"].Add(string.Concat(Buttons.Y));
					c.KeyStatus["HotbarMinus"].Add(string.Concat(Buttons.LeftTrigger));
					c.KeyStatus["HotbarPlus"].Add(string.Concat(Buttons.RightTrigger));
					c.KeyStatus["Grapple"].Add(string.Concat(Buttons.X));
					c.KeyStatus["MapFull"].Add(string.Concat(Buttons.Start));
					c.KeyStatus["SmartSelect"].Add(string.Concat(Buttons.Back));
					c.KeyStatus["QuickMount"].Add(string.Concat(Buttons.RightStick));
					c.KeyStatus["DpadSnap1"].Add(string.Concat(Buttons.DPadUp));
					c.KeyStatus["DpadSnap3"].Add(string.Concat(Buttons.DPadDown));
					c.KeyStatus["DpadSnap4"].Add(string.Concat(Buttons.DPadLeft));
					c.KeyStatus["DpadSnap2"].Add(string.Concat(Buttons.DPadRight));
					c.KeyStatus["MenuUp"].Add(string.Concat(Buttons.DPadUp));
					c.KeyStatus["MenuDown"].Add(string.Concat(Buttons.DPadDown));
					c.KeyStatus["MenuLeft"].Add(string.Concat(Buttons.DPadLeft));
					c.KeyStatus["MenuRight"].Add(string.Concat(Buttons.DPadRight));
					return;
				default:
					return;
				}
				break;
			case PresetProfiles.Yoraiz0r:
				switch (mode)
				{
				case InputMode.Keyboard:
					c.KeyStatus["MouseLeft"].Add("Mouse1");
					c.KeyStatus["MouseRight"].Add("Mouse2");
					c.KeyStatus["Up"].Add("W");
					c.KeyStatus["Down"].Add("S");
					c.KeyStatus["Left"].Add("A");
					c.KeyStatus["Right"].Add("D");
					c.KeyStatus["Jump"].Add("Space");
					c.KeyStatus["Inventory"].Add("Escape");
					c.KeyStatus["Grapple"].Add("E");
					c.KeyStatus["SmartSelect"].Add("LeftShift");
					c.KeyStatus["SmartCursor"].Add("LeftControl");
					c.KeyStatus["QuickMount"].Add("R");
					c.KeyStatus["QuickHeal"].Add("H");
					c.KeyStatus["QuickMana"].Add("J");
					c.KeyStatus["QuickBuff"].Add("B");
					c.KeyStatus["MapStyle"].Add("Tab");
					c.KeyStatus["MapFull"].Add("M");
					c.KeyStatus["MapZoomIn"].Add("Add");
					c.KeyStatus["MapZoomOut"].Add("Subtract");
					c.KeyStatus["MapAlphaUp"].Add("PageUp");
					c.KeyStatus["MapAlphaDown"].Add("PageDown");
					c.KeyStatus["Hotbar1"].Add("D1");
					c.KeyStatus["Hotbar2"].Add("D2");
					c.KeyStatus["Hotbar3"].Add("D3");
					c.KeyStatus["Hotbar4"].Add("D4");
					c.KeyStatus["Hotbar5"].Add("D5");
					c.KeyStatus["Hotbar6"].Add("D6");
					c.KeyStatus["Hotbar7"].Add("D7");
					c.KeyStatus["Hotbar8"].Add("D8");
					c.KeyStatus["Hotbar9"].Add("D9");
					c.KeyStatus["Hotbar10"].Add("D0");
					return;
				case InputMode.KeyboardUI:
					c.KeyStatus["MouseLeft"].Add("Mouse1");
					c.KeyStatus["MouseLeft"].Add("Space");
					c.KeyStatus["MouseRight"].Add("Mouse2");
					c.KeyStatus["Up"].Add("W");
					c.KeyStatus["Up"].Add("Up");
					c.KeyStatus["Down"].Add("S");
					c.KeyStatus["Down"].Add("Down");
					c.KeyStatus["Left"].Add("A");
					c.KeyStatus["Left"].Add("Left");
					c.KeyStatus["Right"].Add("D");
					c.KeyStatus["Right"].Add("Right");
					c.KeyStatus["Inventory"].Add(Keys.Escape.ToString());
					c.KeyStatus["MenuUp"].Add(string.Concat(Buttons.DPadUp));
					c.KeyStatus["MenuDown"].Add(string.Concat(Buttons.DPadDown));
					c.KeyStatus["MenuLeft"].Add(string.Concat(Buttons.DPadLeft));
					c.KeyStatus["MenuRight"].Add(string.Concat(Buttons.DPadRight));
					return;
				case InputMode.Mouse:
					break;
				case InputMode.XBoxGamepad:
					c.KeyStatus["MouseLeft"].Add(string.Concat(Buttons.RightTrigger));
					c.KeyStatus["MouseRight"].Add(string.Concat(Buttons.B));
					c.KeyStatus["Up"].Add(string.Concat(Buttons.LeftThumbstickUp));
					c.KeyStatus["Down"].Add(string.Concat(Buttons.LeftThumbstickDown));
					c.KeyStatus["Left"].Add(string.Concat(Buttons.LeftThumbstickLeft));
					c.KeyStatus["Right"].Add(string.Concat(Buttons.LeftThumbstickRight));
					c.KeyStatus["Jump"].Add(string.Concat(Buttons.LeftTrigger));
					c.KeyStatus["Inventory"].Add(string.Concat(Buttons.Y));
					c.KeyStatus["Grapple"].Add(string.Concat(Buttons.LeftShoulder));
					c.KeyStatus["SmartSelect"].Add(string.Concat(Buttons.LeftStick));
					c.KeyStatus["SmartCursor"].Add(string.Concat(Buttons.RightStick));
					c.KeyStatus["QuickMount"].Add(string.Concat(Buttons.X));
					c.KeyStatus["QuickHeal"].Add(string.Concat(Buttons.A));
					c.KeyStatus["RadialHotbar"].Add(string.Concat(Buttons.RightShoulder));
					c.KeyStatus["MapFull"].Add(string.Concat(Buttons.Start));
					c.KeyStatus["DpadSnap1"].Add(string.Concat(Buttons.DPadUp));
					c.KeyStatus["DpadSnap3"].Add(string.Concat(Buttons.DPadDown));
					c.KeyStatus["DpadSnap4"].Add(string.Concat(Buttons.DPadLeft));
					c.KeyStatus["DpadSnap2"].Add(string.Concat(Buttons.DPadRight));
					c.KeyStatus["MapStyle"].Add(string.Concat(Buttons.Back));
					return;
				case InputMode.XBoxGamepadUI:
					c.KeyStatus["MouseLeft"].Add(string.Concat(Buttons.A));
					c.KeyStatus["MouseRight"].Add(string.Concat(Buttons.LeftShoulder));
					c.KeyStatus["SmartCursor"].Add(string.Concat(Buttons.RightShoulder));
					c.KeyStatus["Up"].Add(string.Concat(Buttons.LeftThumbstickUp));
					c.KeyStatus["Down"].Add(string.Concat(Buttons.LeftThumbstickDown));
					c.KeyStatus["Left"].Add(string.Concat(Buttons.LeftThumbstickLeft));
					c.KeyStatus["Right"].Add(string.Concat(Buttons.LeftThumbstickRight));
					c.KeyStatus["LockOn"].Add(string.Concat(Buttons.B));
					c.KeyStatus["Inventory"].Add(string.Concat(Buttons.Y));
					c.KeyStatus["HotbarMinus"].Add(string.Concat(Buttons.LeftTrigger));
					c.KeyStatus["HotbarPlus"].Add(string.Concat(Buttons.RightTrigger));
					c.KeyStatus["Grapple"].Add(string.Concat(Buttons.X));
					c.KeyStatus["MapFull"].Add(string.Concat(Buttons.Start));
					c.KeyStatus["SmartSelect"].Add(string.Concat(Buttons.Back));
					c.KeyStatus["QuickMount"].Add(string.Concat(Buttons.RightStick));
					c.KeyStatus["DpadSnap1"].Add(string.Concat(Buttons.DPadUp));
					c.KeyStatus["DpadSnap3"].Add(string.Concat(Buttons.DPadDown));
					c.KeyStatus["DpadSnap4"].Add(string.Concat(Buttons.DPadLeft));
					c.KeyStatus["DpadSnap2"].Add(string.Concat(Buttons.DPadRight));
					c.KeyStatus["MenuUp"].Add(string.Concat(Buttons.DPadUp));
					c.KeyStatus["MenuDown"].Add(string.Concat(Buttons.DPadDown));
					c.KeyStatus["MenuLeft"].Add(string.Concat(Buttons.DPadLeft));
					c.KeyStatus["MenuRight"].Add(string.Concat(Buttons.DPadRight));
					return;
				default:
					return;
				}
				break;
			case PresetProfiles.ConsolePS:
				switch (mode)
				{
				case InputMode.Keyboard:
					c.KeyStatus["MouseLeft"].Add("Mouse1");
					c.KeyStatus["MouseRight"].Add("Mouse2");
					c.KeyStatus["Up"].Add("W");
					c.KeyStatus["Down"].Add("S");
					c.KeyStatus["Left"].Add("A");
					c.KeyStatus["Right"].Add("D");
					c.KeyStatus["Jump"].Add("Space");
					c.KeyStatus["Inventory"].Add("Escape");
					c.KeyStatus["Grapple"].Add("E");
					c.KeyStatus["SmartSelect"].Add("LeftShift");
					c.KeyStatus["SmartCursor"].Add("LeftControl");
					c.KeyStatus["QuickMount"].Add("R");
					c.KeyStatus["QuickHeal"].Add("H");
					c.KeyStatus["QuickMana"].Add("J");
					c.KeyStatus["QuickBuff"].Add("B");
					c.KeyStatus["MapStyle"].Add("Tab");
					c.KeyStatus["MapFull"].Add("M");
					c.KeyStatus["MapZoomIn"].Add("Add");
					c.KeyStatus["MapZoomOut"].Add("Subtract");
					c.KeyStatus["MapAlphaUp"].Add("PageUp");
					c.KeyStatus["MapAlphaDown"].Add("PageDown");
					c.KeyStatus["Hotbar1"].Add("D1");
					c.KeyStatus["Hotbar2"].Add("D2");
					c.KeyStatus["Hotbar3"].Add("D3");
					c.KeyStatus["Hotbar4"].Add("D4");
					c.KeyStatus["Hotbar5"].Add("D5");
					c.KeyStatus["Hotbar6"].Add("D6");
					c.KeyStatus["Hotbar7"].Add("D7");
					c.KeyStatus["Hotbar8"].Add("D8");
					c.KeyStatus["Hotbar9"].Add("D9");
					c.KeyStatus["Hotbar10"].Add("D0");
					return;
				case InputMode.KeyboardUI:
					c.KeyStatus["MouseLeft"].Add("Mouse1");
					c.KeyStatus["MouseLeft"].Add("Space");
					c.KeyStatus["MouseRight"].Add("Mouse2");
					c.KeyStatus["Up"].Add("W");
					c.KeyStatus["Up"].Add("Up");
					c.KeyStatus["Down"].Add("S");
					c.KeyStatus["Down"].Add("Down");
					c.KeyStatus["Left"].Add("A");
					c.KeyStatus["Left"].Add("Left");
					c.KeyStatus["Right"].Add("D");
					c.KeyStatus["Right"].Add("Right");
					c.KeyStatus["MenuUp"].Add(string.Concat(Buttons.DPadUp));
					c.KeyStatus["MenuDown"].Add(string.Concat(Buttons.DPadDown));
					c.KeyStatus["MenuLeft"].Add(string.Concat(Buttons.DPadLeft));
					c.KeyStatus["MenuRight"].Add(string.Concat(Buttons.DPadRight));
					c.KeyStatus["Inventory"].Add(Keys.Escape.ToString());
					return;
				case InputMode.Mouse:
					break;
				case InputMode.XBoxGamepad:
					c.KeyStatus["MouseLeft"].Add(string.Concat(Buttons.RightShoulder));
					c.KeyStatus["MouseRight"].Add(string.Concat(Buttons.B));
					c.KeyStatus["Up"].Add(string.Concat(Buttons.LeftThumbstickUp));
					c.KeyStatus["Down"].Add(string.Concat(Buttons.LeftThumbstickDown));
					c.KeyStatus["Left"].Add(string.Concat(Buttons.LeftThumbstickLeft));
					c.KeyStatus["Right"].Add(string.Concat(Buttons.LeftThumbstickRight));
					c.KeyStatus["Jump"].Add(string.Concat(Buttons.A));
					c.KeyStatus["LockOn"].Add(string.Concat(Buttons.X));
					c.KeyStatus["Inventory"].Add(string.Concat(Buttons.Y));
					c.KeyStatus["Grapple"].Add(string.Concat(Buttons.LeftShoulder));
					c.KeyStatus["SmartSelect"].Add(string.Concat(Buttons.LeftStick));
					c.KeyStatus["SmartCursor"].Add(string.Concat(Buttons.RightStick));
					c.KeyStatus["HotbarMinus"].Add(string.Concat(Buttons.LeftTrigger));
					c.KeyStatus["HotbarPlus"].Add(string.Concat(Buttons.RightTrigger));
					c.KeyStatus["MapFull"].Add(string.Concat(Buttons.Start));
					c.KeyStatus["DpadRadial1"].Add(string.Concat(Buttons.DPadUp));
					c.KeyStatus["DpadRadial3"].Add(string.Concat(Buttons.DPadDown));
					c.KeyStatus["DpadRadial4"].Add(string.Concat(Buttons.DPadLeft));
					c.KeyStatus["DpadRadial2"].Add(string.Concat(Buttons.DPadRight));
					c.KeyStatus["QuickMount"].Add(string.Concat(Buttons.Back));
					return;
				case InputMode.XBoxGamepadUI:
					c.KeyStatus["MouseLeft"].Add(string.Concat(Buttons.A));
					c.KeyStatus["MouseRight"].Add(string.Concat(Buttons.LeftShoulder));
					c.KeyStatus["SmartCursor"].Add(string.Concat(Buttons.RightShoulder));
					c.KeyStatus["Up"].Add(string.Concat(Buttons.LeftThumbstickUp));
					c.KeyStatus["Down"].Add(string.Concat(Buttons.LeftThumbstickDown));
					c.KeyStatus["Left"].Add(string.Concat(Buttons.LeftThumbstickLeft));
					c.KeyStatus["Right"].Add(string.Concat(Buttons.LeftThumbstickRight));
					c.KeyStatus["Inventory"].Add(string.Concat(Buttons.B));
					c.KeyStatus["Inventory"].Add(string.Concat(Buttons.Y));
					c.KeyStatus["HotbarMinus"].Add(string.Concat(Buttons.LeftTrigger));
					c.KeyStatus["HotbarPlus"].Add(string.Concat(Buttons.RightTrigger));
					c.KeyStatus["Grapple"].Add(string.Concat(Buttons.X));
					c.KeyStatus["MapFull"].Add(string.Concat(Buttons.Start));
					c.KeyStatus["SmartSelect"].Add(string.Concat(Buttons.Back));
					c.KeyStatus["QuickMount"].Add(string.Concat(Buttons.RightStick));
					c.KeyStatus["DpadRadial1"].Add(string.Concat(Buttons.DPadUp));
					c.KeyStatus["DpadRadial3"].Add(string.Concat(Buttons.DPadDown));
					c.KeyStatus["DpadRadial4"].Add(string.Concat(Buttons.DPadLeft));
					c.KeyStatus["DpadRadial2"].Add(string.Concat(Buttons.DPadRight));
					c.KeyStatus["MenuUp"].Add(string.Concat(Buttons.DPadUp));
					c.KeyStatus["MenuDown"].Add(string.Concat(Buttons.DPadDown));
					c.KeyStatus["MenuLeft"].Add(string.Concat(Buttons.DPadLeft));
					c.KeyStatus["MenuRight"].Add(string.Concat(Buttons.DPadRight));
					return;
				default:
					return;
				}
				break;
			case PresetProfiles.ConsoleXBox:
				switch (mode)
				{
				case InputMode.Keyboard:
					c.KeyStatus["MouseLeft"].Add("Mouse1");
					c.KeyStatus["MouseRight"].Add("Mouse2");
					c.KeyStatus["Up"].Add("W");
					c.KeyStatus["Down"].Add("S");
					c.KeyStatus["Left"].Add("A");
					c.KeyStatus["Right"].Add("D");
					c.KeyStatus["Jump"].Add("Space");
					c.KeyStatus["Inventory"].Add("Escape");
					c.KeyStatus["Grapple"].Add("E");
					c.KeyStatus["SmartSelect"].Add("LeftShift");
					c.KeyStatus["SmartCursor"].Add("LeftControl");
					c.KeyStatus["QuickMount"].Add("R");
					c.KeyStatus["QuickHeal"].Add("H");
					c.KeyStatus["QuickMana"].Add("J");
					c.KeyStatus["QuickBuff"].Add("B");
					c.KeyStatus["MapStyle"].Add("Tab");
					c.KeyStatus["MapFull"].Add("M");
					c.KeyStatus["MapZoomIn"].Add("Add");
					c.KeyStatus["MapZoomOut"].Add("Subtract");
					c.KeyStatus["MapAlphaUp"].Add("PageUp");
					c.KeyStatus["MapAlphaDown"].Add("PageDown");
					c.KeyStatus["Hotbar1"].Add("D1");
					c.KeyStatus["Hotbar2"].Add("D2");
					c.KeyStatus["Hotbar3"].Add("D3");
					c.KeyStatus["Hotbar4"].Add("D4");
					c.KeyStatus["Hotbar5"].Add("D5");
					c.KeyStatus["Hotbar6"].Add("D6");
					c.KeyStatus["Hotbar7"].Add("D7");
					c.KeyStatus["Hotbar8"].Add("D8");
					c.KeyStatus["Hotbar9"].Add("D9");
					c.KeyStatus["Hotbar10"].Add("D0");
					return;
				case InputMode.KeyboardUI:
					c.KeyStatus["MouseLeft"].Add("Mouse1");
					c.KeyStatus["MouseLeft"].Add("Space");
					c.KeyStatus["MouseRight"].Add("Mouse2");
					c.KeyStatus["Up"].Add("W");
					c.KeyStatus["Up"].Add("Up");
					c.KeyStatus["Down"].Add("S");
					c.KeyStatus["Down"].Add("Down");
					c.KeyStatus["Left"].Add("A");
					c.KeyStatus["Left"].Add("Left");
					c.KeyStatus["Right"].Add("D");
					c.KeyStatus["Right"].Add("Right");
					c.KeyStatus["MenuUp"].Add(string.Concat(Buttons.DPadUp));
					c.KeyStatus["MenuDown"].Add(string.Concat(Buttons.DPadDown));
					c.KeyStatus["MenuLeft"].Add(string.Concat(Buttons.DPadLeft));
					c.KeyStatus["MenuRight"].Add(string.Concat(Buttons.DPadRight));
					c.KeyStatus["Inventory"].Add(Keys.Escape.ToString());
					return;
				case InputMode.Mouse:
					break;
				case InputMode.XBoxGamepad:
					c.KeyStatus["MouseLeft"].Add(string.Concat(Buttons.RightTrigger));
					c.KeyStatus["MouseRight"].Add(string.Concat(Buttons.B));
					c.KeyStatus["Up"].Add(string.Concat(Buttons.LeftThumbstickUp));
					c.KeyStatus["Down"].Add(string.Concat(Buttons.LeftThumbstickDown));
					c.KeyStatus["Left"].Add(string.Concat(Buttons.LeftThumbstickLeft));
					c.KeyStatus["Right"].Add(string.Concat(Buttons.LeftThumbstickRight));
					c.KeyStatus["Jump"].Add(string.Concat(Buttons.A));
					c.KeyStatus["LockOn"].Add(string.Concat(Buttons.X));
					c.KeyStatus["Inventory"].Add(string.Concat(Buttons.Y));
					c.KeyStatus["Grapple"].Add(string.Concat(Buttons.LeftTrigger));
					c.KeyStatus["SmartSelect"].Add(string.Concat(Buttons.LeftStick));
					c.KeyStatus["SmartCursor"].Add(string.Concat(Buttons.RightStick));
					c.KeyStatus["HotbarMinus"].Add(string.Concat(Buttons.LeftShoulder));
					c.KeyStatus["HotbarPlus"].Add(string.Concat(Buttons.RightShoulder));
					c.KeyStatus["MapFull"].Add(string.Concat(Buttons.Start));
					c.KeyStatus["DpadRadial1"].Add(string.Concat(Buttons.DPadUp));
					c.KeyStatus["DpadRadial3"].Add(string.Concat(Buttons.DPadDown));
					c.KeyStatus["DpadRadial4"].Add(string.Concat(Buttons.DPadLeft));
					c.KeyStatus["DpadRadial2"].Add(string.Concat(Buttons.DPadRight));
					c.KeyStatus["QuickMount"].Add(string.Concat(Buttons.Back));
					return;
				case InputMode.XBoxGamepadUI:
					c.KeyStatus["MouseLeft"].Add(string.Concat(Buttons.A));
					c.KeyStatus["MouseRight"].Add(string.Concat(Buttons.LeftShoulder));
					c.KeyStatus["SmartCursor"].Add(string.Concat(Buttons.RightShoulder));
					c.KeyStatus["Up"].Add(string.Concat(Buttons.LeftThumbstickUp));
					c.KeyStatus["Down"].Add(string.Concat(Buttons.LeftThumbstickDown));
					c.KeyStatus["Left"].Add(string.Concat(Buttons.LeftThumbstickLeft));
					c.KeyStatus["Right"].Add(string.Concat(Buttons.LeftThumbstickRight));
					c.KeyStatus["Inventory"].Add(string.Concat(Buttons.B));
					c.KeyStatus["Inventory"].Add(string.Concat(Buttons.Y));
					c.KeyStatus["HotbarMinus"].Add(string.Concat(Buttons.LeftTrigger));
					c.KeyStatus["HotbarPlus"].Add(string.Concat(Buttons.RightTrigger));
					c.KeyStatus["Grapple"].Add(string.Concat(Buttons.X));
					c.KeyStatus["MapFull"].Add(string.Concat(Buttons.Start));
					c.KeyStatus["SmartSelect"].Add(string.Concat(Buttons.Back));
					c.KeyStatus["QuickMount"].Add(string.Concat(Buttons.RightStick));
					c.KeyStatus["DpadRadial1"].Add(string.Concat(Buttons.DPadUp));
					c.KeyStatus["DpadRadial3"].Add(string.Concat(Buttons.DPadDown));
					c.KeyStatus["DpadRadial4"].Add(string.Concat(Buttons.DPadLeft));
					c.KeyStatus["DpadRadial2"].Add(string.Concat(Buttons.DPadRight));
					c.KeyStatus["MenuUp"].Add(string.Concat(Buttons.DPadUp));
					c.KeyStatus["MenuDown"].Add(string.Concat(Buttons.DPadDown));
					c.KeyStatus["MenuLeft"].Add(string.Concat(Buttons.DPadLeft));
					c.KeyStatus["MenuRight"].Add(string.Concat(Buttons.DPadRight));
					break;
				default:
					return;
				}
				break;
			default:
				return;
			}
		}

		static PlayerInput()
		{
			// Note: this type is marked as 'beforefieldinit'.
			int[] dpadSnapCooldown = new int[4];
			PlayerInput.DpadSnapCooldown = dpadSnapCooldown;
		}
	}
}
