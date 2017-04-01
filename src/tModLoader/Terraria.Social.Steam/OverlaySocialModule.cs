using Steamworks;
using System;
using Terraria.Social.Base;

namespace Terraria.Social.Steam
{
	public class OverlaySocialModule : Terraria.Social.Base.OverlaySocialModule
	{
		private Callback<GamepadTextInputDismissed_t> _gamepadTextInputDismissed;
		private bool _gamepadTextInputActive;

		public override void Initialize()
		{
			this._gamepadTextInputDismissed = Callback<GamepadTextInputDismissed_t>.Create(new Callback<GamepadTextInputDismissed_t>.DispatchDelegate(this.OnGamepadTextInputDismissed));
		}

		public override void Shutdown()
		{
		}

		public override bool IsGamepadTextInputActive()
		{
			return this._gamepadTextInputActive;
		}

		public override bool ShowGamepadTextInput(string description, uint maxLength, bool multiLine = false, string existingText = "", bool password = false)
		{
			if (this._gamepadTextInputActive)
			{
				return false;
			}
			bool flag = SteamUtils.ShowGamepadTextInput(password ? EGamepadTextInputMode.k_EGamepadTextInputModePassword : EGamepadTextInputMode.k_EGamepadTextInputModeNormal, multiLine ? EGamepadTextInputLineMode.k_EGamepadTextInputLineModeMultipleLines : EGamepadTextInputLineMode.k_EGamepadTextInputLineModeSingleLine, description, maxLength, existingText);
			if (flag)
			{
				this._gamepadTextInputActive = true;
			}
			return flag;
		}

		public override string GetGamepadText()
		{
			uint enteredGamepadTextLength = SteamUtils.GetEnteredGamepadTextLength();
			string result;
			SteamUtils.GetEnteredGamepadTextInput(out result, enteredGamepadTextLength);
			return result;
		}

		private void OnGamepadTextInputDismissed(GamepadTextInputDismissed_t result)
		{
			this._gamepadTextInputActive = false;
		}
	}
}
