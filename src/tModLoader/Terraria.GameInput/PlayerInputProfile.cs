using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Terraria.GameInput
{
	public class PlayerInputProfile
	{
		public Dictionary<InputMode, KeyConfiguration> InputModes = new Dictionary<InputMode, KeyConfiguration>
		{
			{
				InputMode.Keyboard,
				new KeyConfiguration()
			},
			{
				InputMode.KeyboardUI,
				new KeyConfiguration()
			},
			{
				InputMode.XBoxGamepad,
				new KeyConfiguration()
			},
			{
				InputMode.XBoxGamepadUI,
				new KeyConfiguration()
			}
		};
		public string Name = "";
		public bool AllowEditting = true;
		public int HotbarRadialHoldTimeRequired = 16;
		public float TriggersDeadzone = 0.3f;
		public float InterfaceDeadzoneX = 0.2f;
		public float LeftThumbstickDeadzoneX = 0.25f;
		public float LeftThumbstickDeadzoneY = 0.4f;
		public float RightThumbstickDeadzoneX;
		public float RightThumbstickDeadzoneY;
		public bool LeftThumbstickInvertX;
		public bool LeftThumbstickInvertY;
		public bool RightThumbstickInvertX;
		public bool RightThumbstickInvertY;
		public int InventoryMoveCD = 6;

		public bool HotbarAllowsRadial
		{
			get
			{
				return this.HotbarRadialHoldTimeRequired != -1;
			}
		}

		public PlayerInputProfile(string name)
		{
			this.Name = name;
		}

		public void Initialize(PresetProfiles style)
		{
			foreach (KeyValuePair<InputMode, KeyConfiguration> current in this.InputModes)
			{
				current.Value.SetupKeys();
				PlayerInput.Reset(current.Value, style, current.Key);
			}
		}

		public bool Load(Dictionary<string, object> dict)
		{
			object obj;
			if (dict.TryGetValue("Last Launched Version", out obj))
			{
				long arg_15_0 = (long)obj;
			}
			if (dict.TryGetValue("Mouse And Keyboard", out obj))
			{
				this.InputModes[InputMode.Keyboard].ReadPreferences(JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(((JObject)obj).ToString()));
			}
			if (dict.TryGetValue("Gamepad", out obj))
			{
				this.InputModes[InputMode.XBoxGamepad].ReadPreferences(JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(((JObject)obj).ToString()));
			}
			if (dict.TryGetValue("Mouse And Keyboard UI", out obj))
			{
				this.InputModes[InputMode.KeyboardUI].ReadPreferences(JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(((JObject)obj).ToString()));
			}
			if (dict.TryGetValue("Gamepad UI", out obj))
			{
				this.InputModes[InputMode.XBoxGamepadUI].ReadPreferences(JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(((JObject)obj).ToString()));
			}
			if (dict.TryGetValue("Settings", out obj))
			{
				Dictionary<string, object> dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(((JObject)obj).ToString());
				if (dictionary.TryGetValue("Edittable", out obj))
				{
					this.AllowEditting = (bool)obj;
				}
				if (dictionary.TryGetValue("Gamepad - HotbarRadialHoldTime", out obj))
				{
					this.HotbarRadialHoldTimeRequired = (int)((long)obj);
				}
				if (dictionary.TryGetValue("Gamepad - LeftThumbstickDeadzoneX", out obj))
				{
					this.LeftThumbstickDeadzoneX = (float)((double)obj);
				}
				if (dictionary.TryGetValue("Gamepad - LeftThumbstickDeadzoneY", out obj))
				{
					this.LeftThumbstickDeadzoneY = (float)((double)obj);
				}
				if (dictionary.TryGetValue("Gamepad - RightThumbstickDeadzoneX", out obj))
				{
					this.RightThumbstickDeadzoneX = (float)((double)obj);
				}
				if (dictionary.TryGetValue("Gamepad - RightThumbstickDeadzoneY", out obj))
				{
					this.RightThumbstickDeadzoneY = (float)((double)obj);
				}
				if (dictionary.TryGetValue("Gamepad - LeftThumbstickInvertX", out obj))
				{
					this.LeftThumbstickInvertX = (bool)obj;
				}
				if (dictionary.TryGetValue("Gamepad - LeftThumbstickInvertY", out obj))
				{
					this.LeftThumbstickInvertY = (bool)obj;
				}
				if (dictionary.TryGetValue("Gamepad - RightThumbstickInvertX", out obj))
				{
					this.RightThumbstickInvertX = (bool)obj;
				}
				if (dictionary.TryGetValue("Gamepad - RightThumbstickInvertY", out obj))
				{
					this.RightThumbstickInvertY = (bool)obj;
				}
				if (dictionary.TryGetValue("Gamepad - TriggersDeadzone", out obj))
				{
					this.TriggersDeadzone = (float)((double)obj);
				}
				if (dictionary.TryGetValue("Gamepad - InterfaceDeadzoneX", out obj))
				{
					this.InterfaceDeadzoneX = (float)((double)obj);
				}
				if (dictionary.TryGetValue("Gamepad - InventoryMoveCD", out obj))
				{
					this.InventoryMoveCD = (int)((long)obj);
				}
			}
			return true;
		}

		public Dictionary<string, object> Save()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
			dictionary.Add("Last Launched Version", 188);
			dictionary2.Add("Edittable", this.AllowEditting);
			dictionary2.Add("Gamepad - HotbarRadialHoldTime", this.HotbarRadialHoldTimeRequired);
			dictionary2.Add("Gamepad - LeftThumbstickDeadzoneX", this.LeftThumbstickDeadzoneX);
			dictionary2.Add("Gamepad - LeftThumbstickDeadzoneY", this.LeftThumbstickDeadzoneY);
			dictionary2.Add("Gamepad - RightThumbstickDeadzoneX", this.RightThumbstickDeadzoneX);
			dictionary2.Add("Gamepad - RightThumbstickDeadzoneY", this.RightThumbstickDeadzoneY);
			dictionary2.Add("Gamepad - LeftThumbstickInvertX", this.LeftThumbstickInvertX);
			dictionary2.Add("Gamepad - LeftThumbstickInvertY", this.LeftThumbstickInvertY);
			dictionary2.Add("Gamepad - RightThumbstickInvertX", this.RightThumbstickInvertX);
			dictionary2.Add("Gamepad - RightThumbstickInvertY", this.RightThumbstickInvertY);
			dictionary2.Add("Gamepad - TriggersDeadzone", this.TriggersDeadzone);
			dictionary2.Add("Gamepad - InterfaceDeadzoneX", this.InterfaceDeadzoneX);
			dictionary2.Add("Gamepad - InventoryMoveCD", this.InventoryMoveCD);
			dictionary.Add("Settings", dictionary2);
			dictionary.Add("Mouse And Keyboard", this.InputModes[InputMode.Keyboard].WritePreferences());
			dictionary.Add("Gamepad", this.InputModes[InputMode.XBoxGamepad].WritePreferences());
			dictionary.Add("Mouse And Keyboard UI", this.InputModes[InputMode.KeyboardUI].WritePreferences());
			dictionary.Add("Gamepad UI", this.InputModes[InputMode.XBoxGamepadUI].WritePreferences());
			return dictionary;
		}

		public void ConditionalAddProfile(Dictionary<string, object> dicttouse, string k, InputMode nm, Dictionary<string, List<string>> dict)
		{
			if (PlayerInput.OriginalProfiles.ContainsKey(this.Name))
			{
				Dictionary<string, List<string>> dictionary = PlayerInput.OriginalProfiles[this.Name].InputModes[nm].WritePreferences();
				foreach (KeyValuePair<string, List<string>> current in dictionary)
				{
					bool flag = true;
					List<string> list;
					if (dict.TryGetValue(current.Key, out list))
					{
						if (list.Count != current.Value.Count)
						{
							flag = false;
						}
						if (!flag)
						{
							for (int i = 0; i < list.Count; i++)
							{
								if (list[i] != current.Value[i])
								{
									flag = false;
									break;
								}
							}
						}
					}
					else
					{
						flag = false;
					}
					if (flag)
					{
						dict.Remove(current.Key);
					}
				}
			}
			if (dict.Count > 0)
			{
				dicttouse.Add(k, dict);
			}
		}

		public void ConditionalAdd(Dictionary<string, object> dicttouse, string a, object b, Func<PlayerInputProfile, bool> check)
		{
			if (PlayerInput.OriginalProfiles.ContainsKey(this.Name) && check(PlayerInput.OriginalProfiles[this.Name]))
			{
				return;
			}
			dicttouse.Add(a, b);
		}

		public void CopyGameplaySettingsFrom(PlayerInputProfile profile, InputMode mode)
		{
			string[] keysToCopy = new string[]
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
				"Inventory"
			};
			this.CopyKeysFrom(profile, mode, keysToCopy);
		}

		public void CopyHotbarSettingsFrom(PlayerInputProfile profile, InputMode mode)
		{
			string[] keysToCopy = new string[]
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
				"Hotbar10"
			};
			this.CopyKeysFrom(profile, mode, keysToCopy);
		}

		public void CopyMapSettingsFrom(PlayerInputProfile profile, InputMode mode)
		{
			string[] keysToCopy = new string[]
			{
				"MapZoomIn",
				"MapZoomOut",
				"MapAlphaUp",
				"MapAlphaDown",
				"MapFull",
				"MapStyle"
			};
			this.CopyKeysFrom(profile, mode, keysToCopy);
		}

		public void CopyModHotkeySettingsFrom(PlayerInputProfile profile, InputMode mode)
		{
			string[] keysToCopy = ModLoader.ModLoader.modHotKeys.Select(x=>x.Value.displayName).ToArray();
			this.CopyKeysFrom(profile, mode, keysToCopy);
		}

		public void CopyGamepadSettingsFrom(PlayerInputProfile profile, InputMode mode)
		{
			string[] keysToCopy = new string[]
			{
				"RadialHotbar",
				"RadialQuickbar",
				"DpadSnap1",
				"DpadSnap2",
				"DpadSnap3",
				"DpadSnap4",
				"DpadRadial1",
				"DpadRadial2",
				"DpadRadial3",
				"DpadRadial4"
			};
			this.CopyKeysFrom(profile, InputMode.XBoxGamepad, keysToCopy);
			this.CopyKeysFrom(profile, InputMode.XBoxGamepadUI, keysToCopy);
		}

		public void CopyGamepadAdvancedSettingsFrom(PlayerInputProfile profile, InputMode mode)
		{
			this.TriggersDeadzone = profile.TriggersDeadzone;
			this.InterfaceDeadzoneX = profile.InterfaceDeadzoneX;
			this.LeftThumbstickDeadzoneX = profile.LeftThumbstickDeadzoneX;
			this.LeftThumbstickDeadzoneY = profile.LeftThumbstickDeadzoneY;
			this.RightThumbstickDeadzoneX = profile.RightThumbstickDeadzoneX;
			this.RightThumbstickDeadzoneY = profile.RightThumbstickDeadzoneY;
			this.LeftThumbstickInvertX = profile.LeftThumbstickInvertX;
			this.LeftThumbstickInvertY = profile.LeftThumbstickInvertY;
			this.RightThumbstickInvertX = profile.RightThumbstickInvertX;
			this.RightThumbstickInvertY = profile.RightThumbstickInvertY;
			this.InventoryMoveCD = profile.InventoryMoveCD;
		}

		private void CopyKeysFrom(PlayerInputProfile profile, InputMode mode, string[] keysToCopy)
		{
			for (int i = 0; i < keysToCopy.Length; i++)
			{
				List<string> collection;
				if (profile.InputModes[mode].KeyStatus.TryGetValue(keysToCopy[i], out collection))
				{
					this.InputModes[mode].KeyStatus[keysToCopy[i]].Clear();
					this.InputModes[mode].KeyStatus[keysToCopy[i]].AddRange(collection);
				}
			}
		}

		public bool UsingDpadHotbar()
		{
			return this.InputModes[InputMode.XBoxGamepad].KeyStatus["DpadRadial1"].Contains(Buttons.DPadUp.ToString()) && this.InputModes[InputMode.XBoxGamepad].KeyStatus["DpadRadial2"].Contains(Buttons.DPadRight.ToString()) && this.InputModes[InputMode.XBoxGamepad].KeyStatus["DpadRadial3"].Contains(Buttons.DPadDown.ToString()) && this.InputModes[InputMode.XBoxGamepad].KeyStatus["DpadRadial4"].Contains(Buttons.DPadLeft.ToString()) && this.InputModes[InputMode.XBoxGamepadUI].KeyStatus["DpadRadial1"].Contains(Buttons.DPadUp.ToString()) && this.InputModes[InputMode.XBoxGamepadUI].KeyStatus["DpadRadial2"].Contains(Buttons.DPadRight.ToString()) && this.InputModes[InputMode.XBoxGamepadUI].KeyStatus["DpadRadial3"].Contains(Buttons.DPadDown.ToString()) && this.InputModes[InputMode.XBoxGamepadUI].KeyStatus["DpadRadial4"].Contains(Buttons.DPadLeft.ToString());
		}

		public bool UsingDpadMovekeys()
		{
			return this.InputModes[InputMode.XBoxGamepad].KeyStatus["DpadSnap1"].Contains(Buttons.DPadUp.ToString()) && this.InputModes[InputMode.XBoxGamepad].KeyStatus["DpadSnap2"].Contains(Buttons.DPadRight.ToString()) && this.InputModes[InputMode.XBoxGamepad].KeyStatus["DpadSnap3"].Contains(Buttons.DPadDown.ToString()) && this.InputModes[InputMode.XBoxGamepad].KeyStatus["DpadSnap4"].Contains(Buttons.DPadLeft.ToString()) && this.InputModes[InputMode.XBoxGamepadUI].KeyStatus["DpadSnap1"].Contains(Buttons.DPadUp.ToString()) && this.InputModes[InputMode.XBoxGamepadUI].KeyStatus["DpadSnap2"].Contains(Buttons.DPadRight.ToString()) && this.InputModes[InputMode.XBoxGamepadUI].KeyStatus["DpadSnap3"].Contains(Buttons.DPadDown.ToString()) && this.InputModes[InputMode.XBoxGamepadUI].KeyStatus["DpadSnap4"].Contains(Buttons.DPadLeft.ToString());
		}
	}
}
