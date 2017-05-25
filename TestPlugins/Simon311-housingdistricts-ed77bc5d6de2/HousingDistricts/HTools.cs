using System;
using System.Text.RegularExpressions;
using TShockAPI;
using TShockAPI.DB;

namespace HousingDistricts
{
	class HTools
	{
		public static House GetHouseByName(string name)
		{
			if (String.IsNullOrEmpty(name))
				return null;

			var I = HousingDistricts.Houses.Count;
			for (int i = 0; i < I; i++)
			{
				var house = HousingDistricts.Houses[i];
				if (house.Name == name)
					return house;
			}
			return null;
		}

		public static void BroadcastToHouse(House house, string text, string playername)
		{
			var I = HousingDistricts.HPlayers.Count;
			for (int i = 0; i < I; i++)
			{
				var player = HousingDistricts.HPlayers[i];
				if (house.HouseArea.Intersects(new Rectangle(player.TSPlayer.TileX, player.TSPlayer.TileY, 1, 1)))
					player.TSPlayer.SendMessage("<House> <" + playername + ">: " + text, Color.LightSkyBlue);
			}
		}

		public static string InAreaHouseName(int x, int y)
		{
			var I = HousingDistricts.Houses.Count;
			for (int i = 0; i < I; i++)
			{
				var house = HousingDistricts.Houses[i];
				if (x >= house.HouseArea.Left && x < house.HouseArea.Right &&
					y >= house.HouseArea.Top && y < house.HouseArea.Bottom)
					return house.Name;
			}
			return null;
		}

		public static void BroadcastToHouseOwners(string housename, string text)
		{
			BroadcastToHouseOwners(HTools.GetHouseByName(housename), text);
		}

		public static void BroadcastToHouseOwners(House house, string text)
		{
			foreach (TSPlayer player in TShock.Players)
				if (player != null && player.User != null && house.Owners.Contains(player.User.ID.ToString()))
						player.SendMessage(text, Color.LightSeaGreen);
		}

		public static bool OwnsHouse(User U, string housename)
		{
			if (U == null)
				return false;
			return OwnsHouse(U.ID.ToString(), housename);
		}

		public static bool OwnsHouse(User U, House house)
		{
			if (U == null)
				return false;
			return OwnsHouse(U.ID.ToString(), house);
		}

		public static bool OwnsHouse(string UserID, string housename)
		{
			if (String.IsNullOrWhiteSpace(UserID) || UserID == "0" || String.IsNullOrEmpty(housename)) return false;
			House H = HTools.GetHouseByName(housename);
			if (H == null) return false;
			return OwnsHouse(UserID, H);
		}

		public static bool OwnsHouse(string UserID, House house)
		{
			bool isAdmin = false;
			try { isAdmin = TShock.Groups.GetGroupByName(TShock.Users.GetUserByID(Convert.ToInt32(UserID)).Group).HasPermission("house.root"); }
			catch {}
			if (!String.IsNullOrEmpty(UserID) && UserID != "0" && house != null)
			{
				try
				{
					if (house.Owners.Contains(UserID) || isAdmin) return true;
					else return false;
				}
				catch (Exception ex)
				{
					TShock.Log.Error(ex.ToString());
					return false;
				}
			}
			return false;
		}

		public static bool CanVisitHouse(string UserID, House house)
		{
			return (!String.IsNullOrEmpty(UserID) && UserID != "0") && (house.Visitors.Contains(UserID) || house.Owners.Contains(UserID)); 
		}

		public static bool CanVisitHouse(User U, House house)
		{
			return (U != null && U.ID != 0) && (house.Visitors.Contains(U.ID.ToString()) || house.Owners.Contains(U.ID.ToString()));
		}

		public static HPlayer GetPlayerByID(int id)
		{
			var I = HousingDistricts.HPlayers.Count;
			for (int i = 0; i < I; i++)
			{
				var player = HousingDistricts.HPlayers[i];
				if (player.Index == id) return player;
			}

			return new HPlayer();
		}

		public static int MaxSize(TSPlayer ply)
		{
			var I = ply.Group.permissions.Count;
			for (int i = 0; i < I; i++)
			{
				var perm = ply.Group.permissions[i];
				Match Match = Regex.Match(perm, @"^house\.size\.(\d{1,9})$");
				if (Match.Success)
					return Convert.ToInt32(Match.Groups[1].Value);
			}
			return HConfigFile.Config.MaxHouseSize;
		}

		public static int MaxCount(TSPlayer ply)
		{
			var I = ply.Group.permissions.Count;
			for (int i = 0; i < I; i++)
			{
				var perm = ply.Group.permissions[i];
				Match Match = Regex.Match(perm, @"^house\.count\.(\d{1,9})$");
				if (Match.Success)
					return Convert.ToInt32(Match.Groups[1].Value);
			}
			return HConfigFile.Config.MaxHousesByUsername;
		}
	}
}
