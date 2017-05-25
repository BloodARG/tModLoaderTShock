using System.IO;
using Newtonsoft.Json;
using TShockAPI;

namespace HousingDistricts
{
	public class HConfigFile
	{
		public bool NotifyOnEntry = true;
		public string NotifyOnEntry_description = "Global setting: Enables entry notifications (see below).";
		public bool NotifyOwner = true;
		public string NotifyOwner_description = "Global setting: Notifies the owner of the house when a user enters/leaves.";
		public bool NotifyVisitor = true;
		public string NotifyVisitor_description = "Global setting: Notifies a user about entering/leaving a house.";
		public bool NotifySelf = true;
		public string NotifySelf_description = "Global setting: Notifies a user about entering/leaving his/her own house.";
		public string NotifyOnEntryString = "You have entered the house: '$HOUSE_NAME'";
		public string NotifyOnEntryString_description = "The string presented to players when they enter another player's house.";
		public string NotifyOnOwnHouseEntryString = "Entered your house: '$HOUSE_NAME'";
		public string NotifyOnOwnHouseEntryString_description = "The string presented to players when they enter their own house.";
		public string NotifyOnOtherEntryString = "$PLAYER_NAME Entered your house: '$HOUSE_NAME'";
		public string NotifyOnOtherEntryString_description = "The string presented to players when someone else enters their house.";
		public bool NotifyOnExit = true;
		public string NotifyOnExit_description = "Global setting: Enables exit notifications.";
		public string NotifyOnExitString = "You have left the house: '$HOUSE_NAME'";
		public string NotifyOnExitString_description = "The string presented to players when they leave another player's house.";
		public string NotifyOnOwnHouseExitString = "Left your house: '$HOUSE_NAME'";
		public string NotifyOnOwnHouseExitString_description = "The string presented to players when they leave their own house.";
		public string NotifyOnOtherExitString = "$PLAYER_NAME Left your house: '$HOUSE_NAME'";
		public string NotifyOnOtherExitString_description = "The string presented to players when someone else leaves their house.";
		public bool HouseChatEnabled = true;
		public string HouseChatEnabled_description = "Global setting: False completely disables house chat.";
		public int MaxHouseSize = 5000;
		public string MaxHouseSize_description = "Maximum house size (width*height).";
		public int MinHouseWidth = 10;
		public string MinHouseWidth_description = "Minimum house width, for protection from griefer use of /house.";
		public int MinHouseHeight = 5;
		public string MinHouseHeight_description = "Minimum house height, for protection from griefer use of /house.";
		public int MaxHousesByUsername = 10;
		public string MaxHousesByUsername_description = "Maximum amount of houses a user can have (unless has persmission house.bypasscount).";
		public bool OverlapHouses = false;
		public string OverlapHouses_description = "Can players create houses that overlap another players' house?";
		public bool AllowRod = true;
		public string AllowRod_description = "Can players use RoD to teleport into houses?";
		public bool DisableUpdateTimer = false;
		public string DisableUpdateTimer_description = "Will likely increase performance, however /house lock and notifications won't work.";
		public bool RequirePermissionForAllow = false;
		public string RequirePermissionForAllow_description = "Require house.allow for /house allow.";
		/*
		public bool RecursiveNumericPermissions = false;
		public string RecursiveNumericPermissions_description = "Makes house.count. and house.size permissions recursive. Might affect performance.";
		 */

		private static string HConfigPath { get { return Path.Combine(TShock.SavePath, "hconfig.json"); } }

		public static HConfigFile Config
		{
			get
			{
				if (_config == null)
					return _config = Read();
				return _config;
			}
		}
		static HConfigFile _config;

		public static void ForceLoad()
		{
			_config = Read();
		}

		static HConfigFile Read()
		{
			HConfigFile cf;

			if (!File.Exists(HConfigPath))
			{
				TShock.Log.ConsoleInfo("Housing Districts config not found, creating new one...");
				cf = new HConfigFile();
				cf.Write();
				return cf;
			}

			try
			{
				cf = JsonConvert.DeserializeObject<HConfigFile>(File.ReadAllText(HConfigPath));
				cf.Write(); // Add missing config options
			}
			catch
			{
				TShock.Log.ConsoleError("Housing Districts config file is broken. A dummy config has been loaded.");
				cf = new HConfigFile();
			}
			return cf;
		}

		public void Write()
		{
			File.WriteAllText(HConfigPath, JsonConvert.SerializeObject(this, Formatting.Indented));
		}
	}
}
