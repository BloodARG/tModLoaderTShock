using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Terraria.GameInput
{
	public class TriggersSet
	{
		public Dictionary<string, bool> KeyStatus = new Dictionary<string, bool>();

		public bool UsedMovementKey = true;

		public int HotbarScrollCD;

		public int HotbarHoldTime;

		public bool MouseLeft
		{
			get
			{
				return this.KeyStatus["MouseLeft"];
			}
			set
			{
				this.KeyStatus["MouseLeft"] = value;
			}
		}

		public bool MouseRight
		{
			get
			{
				return this.KeyStatus["MouseRight"];
			}
			set
			{
				this.KeyStatus["MouseRight"] = value;
			}
		}

		public bool Up
		{
			get
			{
				return this.KeyStatus["Up"];
			}
			set
			{
				this.KeyStatus["Up"] = value;
			}
		}

		public bool Down
		{
			get
			{
				return this.KeyStatus["Down"];
			}
			set
			{
				this.KeyStatus["Down"] = value;
			}
		}

		public bool Left
		{
			get
			{
				return this.KeyStatus["Left"];
			}
			set
			{
				this.KeyStatus["Left"] = value;
			}
		}

		public bool Right
		{
			get
			{
				return this.KeyStatus["Right"];
			}
			set
			{
				this.KeyStatus["Right"] = value;
			}
		}

		public bool Jump
		{
			get
			{
				return this.KeyStatus["Jump"];
			}
			set
			{
				this.KeyStatus["Jump"] = value;
			}
		}

		public bool Throw
		{
			get
			{
				return this.KeyStatus["Throw"];
			}
			set
			{
				this.KeyStatus["Throw"] = value;
			}
		}

		public bool Inventory
		{
			get
			{
				return this.KeyStatus["Inventory"];
			}
			set
			{
				this.KeyStatus["Inventory"] = value;
			}
		}

		public bool Grapple
		{
			get
			{
				return this.KeyStatus["Grapple"];
			}
			set
			{
				this.KeyStatus["Grapple"] = value;
			}
		}

		public bool SmartSelect
		{
			get
			{
				return this.KeyStatus["SmartSelect"];
			}
			set
			{
				this.KeyStatus["SmartSelect"] = value;
			}
		}

		public bool SmartCursor
		{
			get
			{
				return this.KeyStatus["SmartCursor"];
			}
			set
			{
				this.KeyStatus["SmartCursor"] = value;
			}
		}

		public bool QuickMount
		{
			get
			{
				return this.KeyStatus["QuickMount"];
			}
			set
			{
				this.KeyStatus["QuickMount"] = value;
			}
		}

		public bool QuickHeal
		{
			get
			{
				return this.KeyStatus["QuickHeal"];
			}
			set
			{
				this.KeyStatus["QuickHeal"] = value;
			}
		}

		public bool QuickMana
		{
			get
			{
				return this.KeyStatus["QuickMana"];
			}
			set
			{
				this.KeyStatus["QuickMana"] = value;
			}
		}

		public bool QuickBuff
		{
			get
			{
				return this.KeyStatus["QuickBuff"];
			}
			set
			{
				this.KeyStatus["QuickBuff"] = value;
			}
		}

		public bool MapZoomIn
		{
			get
			{
				return this.KeyStatus["MapZoomIn"];
			}
			set
			{
				this.KeyStatus["MapZoomIn"] = value;
			}
		}

		public bool MapZoomOut
		{
			get
			{
				return this.KeyStatus["MapZoomOut"];
			}
			set
			{
				this.KeyStatus["MapZoomOut"] = value;
			}
		}

		public bool MapAlphaUp
		{
			get
			{
				return this.KeyStatus["MapAlphaUp"];
			}
			set
			{
				this.KeyStatus["MapAlphaUp"] = value;
			}
		}

		public bool MapAlphaDown
		{
			get
			{
				return this.KeyStatus["MapAlphaDown"];
			}
			set
			{
				this.KeyStatus["MapAlphaDown"] = value;
			}
		}

		public bool MapFull
		{
			get
			{
				return this.KeyStatus["MapFull"];
			}
			set
			{
				this.KeyStatus["MapFull"] = value;
			}
		}

		public bool MapStyle
		{
			get
			{
				return this.KeyStatus["MapStyle"];
			}
			set
			{
				this.KeyStatus["MapStyle"] = value;
			}
		}

		public bool Hotbar1
		{
			get
			{
				return this.KeyStatus["Hotbar1"];
			}
			set
			{
				this.KeyStatus["Hotbar1"] = value;
			}
		}

		public bool Hotbar2
		{
			get
			{
				return this.KeyStatus["Hotbar2"];
			}
			set
			{
				this.KeyStatus["Hotbar2"] = value;
			}
		}

		public bool Hotbar3
		{
			get
			{
				return this.KeyStatus["Hotbar3"];
			}
			set
			{
				this.KeyStatus["Hotbar3"] = value;
			}
		}

		public bool Hotbar4
		{
			get
			{
				return this.KeyStatus["Hotbar4"];
			}
			set
			{
				this.KeyStatus["Hotbar4"] = value;
			}
		}

		public bool Hotbar5
		{
			get
			{
				return this.KeyStatus["Hotbar5"];
			}
			set
			{
				this.KeyStatus["Hotbar5"] = value;
			}
		}

		public bool Hotbar6
		{
			get
			{
				return this.KeyStatus["Hotbar6"];
			}
			set
			{
				this.KeyStatus["Hotbar6"] = value;
			}
		}

		public bool Hotbar7
		{
			get
			{
				return this.KeyStatus["Hotbar7"];
			}
			set
			{
				this.KeyStatus["Hotbar7"] = value;
			}
		}

		public bool Hotbar8
		{
			get
			{
				return this.KeyStatus["Hotbar8"];
			}
			set
			{
				this.KeyStatus["Hotbar8"] = value;
			}
		}

		public bool Hotbar9
		{
			get
			{
				return this.KeyStatus["Hotbar9"];
			}
			set
			{
				this.KeyStatus["Hotbar9"] = value;
			}
		}

		public bool Hotbar10
		{
			get
			{
				return this.KeyStatus["Hotbar10"];
			}
			set
			{
				this.KeyStatus["Hotbar10"] = value;
			}
		}

		public bool HotbarMinus
		{
			get
			{
				return this.KeyStatus["HotbarMinus"];
			}
			set
			{
				this.KeyStatus["HotbarMinus"] = value;
			}
		}

		public bool HotbarPlus
		{
			get
			{
				return this.KeyStatus["HotbarPlus"];
			}
			set
			{
				this.KeyStatus["HotbarPlus"] = value;
			}
		}

		public bool DpadRadial1
		{
			get
			{
				return this.KeyStatus["DpadRadial1"];
			}
			set
			{
				this.KeyStatus["DpadRadial1"] = value;
			}
		}

		public bool DpadRadial2
		{
			get
			{
				return this.KeyStatus["DpadRadial2"];
			}
			set
			{
				this.KeyStatus["DpadRadial2"] = value;
			}
		}

		public bool DpadRadial3
		{
			get
			{
				return this.KeyStatus["DpadRadial3"];
			}
			set
			{
				this.KeyStatus["DpadRadial3"] = value;
			}
		}

		public bool DpadRadial4
		{
			get
			{
				return this.KeyStatus["DpadRadial4"];
			}
			set
			{
				this.KeyStatus["DpadRadial4"] = value;
			}
		}

		public bool RadialHotbar
		{
			get
			{
				return this.KeyStatus["RadialHotbar"];
			}
			set
			{
				this.KeyStatus["RadialHotbar"] = value;
			}
		}

		public bool RadialQuickbar
		{
			get
			{
				return this.KeyStatus["RadialQuickbar"];
			}
			set
			{
				this.KeyStatus["RadialQuickbar"] = value;
			}
		}

		public bool DpadMouseSnap1
		{
			get
			{
				return this.KeyStatus["DpadSnap1"];
			}
			set
			{
				this.KeyStatus["DpadSnap1"] = value;
			}
		}

		public bool DpadMouseSnap2
		{
			get
			{
				return this.KeyStatus["DpadSnap2"];
			}
			set
			{
				this.KeyStatus["DpadSnap2"] = value;
			}
		}

		public bool DpadMouseSnap3
		{
			get
			{
				return this.KeyStatus["DpadSnap3"];
			}
			set
			{
				this.KeyStatus["DpadSnap3"] = value;
			}
		}

		public bool DpadMouseSnap4
		{
			get
			{
				return this.KeyStatus["DpadSnap4"];
			}
			set
			{
				this.KeyStatus["DpadSnap4"] = value;
			}
		}

		public bool MenuUp
		{
			get
			{
				return this.KeyStatus["MenuUp"];
			}
			set
			{
				this.KeyStatus["MenuUp"] = value;
			}
		}

		public bool MenuDown
		{
			get
			{
				return this.KeyStatus["MenuDown"];
			}
			set
			{
				this.KeyStatus["MenuDown"] = value;
			}
		}

		public bool MenuLeft
		{
			get
			{
				return this.KeyStatus["MenuLeft"];
			}
			set
			{
				this.KeyStatus["MenuLeft"] = value;
			}
		}

		public bool MenuRight
		{
			get
			{
				return this.KeyStatus["MenuRight"];
			}
			set
			{
				this.KeyStatus["MenuRight"] = value;
			}
		}

		public bool LockOn
		{
			get
			{
				return this.KeyStatus["LockOn"];
			}
			set
			{
				this.KeyStatus["LockOn"] = value;
			}
		}

		public Vector2 DirectionsRaw
		{
			get
			{
				return new Vector2((float)(this.Right.ToInt() - this.Left.ToInt()), (float)(this.Down.ToInt() - this.Up.ToInt()));
			}
		}

		public void Reset()
		{
			string[] array = this.KeyStatus.Keys.ToArray<string>();
			for (int i = 0; i < array.Length; i++)
			{
				this.KeyStatus[array[i]] = false;
			}
		}

		public TriggersSet Clone()
		{
			TriggersSet triggersSet = new TriggersSet();
			foreach (string current in this.KeyStatus.Keys)
			{
				triggersSet.KeyStatus.Add(current, this.KeyStatus[current]);
			}
			triggersSet.UsedMovementKey = this.UsedMovementKey;
			triggersSet.HotbarScrollCD = this.HotbarScrollCD;
			triggersSet.HotbarHoldTime = this.HotbarHoldTime;
			return triggersSet;
		}

		public void SetupKeys()
		{
			this.KeyStatus.Clear();
			foreach (string current in PlayerInput.KnownTriggers)
			{
				this.KeyStatus.Add(current, false);
			}
		}

		public Vector2 GetNavigatorDirections()
		{
			bool flag = Main.gameMenu || Main.ingameOptionsWindow || Main.editChest || Main.editSign || (Main.playerInventory && PlayerInput.CurrentProfile.UsingDpadMovekeys());
			bool value = this.Up || (flag && this.MenuUp);
			bool value2 = this.Right || (flag && this.MenuRight);
			bool value3 = this.Down || (flag && this.MenuDown);
			bool value4 = this.Left || (flag && this.MenuLeft);
			return new Vector2((float)(value2.ToInt() - value4.ToInt()), (float)(value3.ToInt() - value.ToInt()));
		}

		public void CopyInto(Player p)
		{
			if (PlayerInput.CurrentInputMode != InputMode.XBoxGamepadUI && !PlayerInput.CursorIsBusy)
			{
				p.controlUp = this.Up;
				p.controlDown = this.Down;
				p.controlLeft = this.Left;
				p.controlRight = this.Right;
				p.controlJump = this.Jump;
				p.controlHook = this.Grapple;
				p.controlTorch = this.SmartSelect;
				p.controlSmart = this.SmartCursor;
				p.controlMount = this.QuickMount;
				p.controlQuickHeal = this.QuickHeal;
				p.controlQuickMana = this.QuickMana;
				if (this.QuickBuff)
				{
					p.QuickBuff();
				}
			}
			p.controlInv = this.Inventory;
			p.controlThrow = this.Throw;
			p.mapZoomIn = this.MapZoomIn;
			p.mapZoomOut = this.MapZoomOut;
			p.mapAlphaUp = this.MapAlphaUp;
			p.mapAlphaDown = this.MapAlphaDown;
			p.mapFullScreen = this.MapFull;
			p.mapStyle = this.MapStyle;
			if (this.MouseLeft)
			{
				if (!Main.blockMouse && !p.mouseInterface)
				{
					p.controlUseItem = true;
				}
			}
			else
			{
				Main.blockMouse = false;
			}
			if (!this.MouseRight && !Main.playerInventory)
			{
				PlayerInput.LockTileUseButton = false;
			}
			if (this.MouseRight && !p.mouseInterface && (!Main.blockMouse & !PlayerInput.LockTileUseButton) && !PlayerInput.InBuildingMode)
			{
				p.controlUseTile = true;
			}
			if (PlayerInput.InBuildingMode && this.MouseRight)
			{
				p.controlInv = true;
			}
			bool flag = PlayerInput.Triggers.Current.HotbarPlus || PlayerInput.Triggers.Current.HotbarMinus;
			if (flag)
			{
				this.HotbarHoldTime++;
			}
			else
			{
				this.HotbarHoldTime = 0;
			}
			if (this.HotbarScrollCD > 0 && (this.HotbarScrollCD != 1 || !flag || PlayerInput.CurrentProfile.HotbarRadialHoldTimeRequired <= 0))
			{
				this.HotbarScrollCD--;
			}
		}

		public void CopyIntoDuringChat(Player p)
		{
			if (this.MouseLeft)
			{
				if (!Main.blockMouse && !p.mouseInterface)
				{
					p.controlUseItem = true;
				}
			}
			else
			{
				Main.blockMouse = false;
			}
			if (!this.MouseRight && !Main.playerInventory)
			{
				PlayerInput.LockTileUseButton = false;
			}
			if (this.MouseRight && !p.mouseInterface && (!Main.blockMouse & !PlayerInput.LockTileUseButton) && !PlayerInput.InBuildingMode)
			{
				p.controlUseTile = true;
			}
			bool flag = PlayerInput.Triggers.Current.HotbarPlus || PlayerInput.Triggers.Current.HotbarMinus;
			if (flag)
			{
				this.HotbarHoldTime++;
			}
			else
			{
				this.HotbarHoldTime = 0;
			}
			if (this.HotbarScrollCD > 0 && (this.HotbarScrollCD != 1 || !flag || PlayerInput.CurrentProfile.HotbarRadialHoldTimeRequired <= 0))
			{
				this.HotbarScrollCD--;
			}
		}
	}
}
