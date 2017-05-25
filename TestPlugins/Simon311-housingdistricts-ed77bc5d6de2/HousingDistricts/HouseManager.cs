using System;
using System.Collections.Generic;
using System.Text;
using TShockAPI.DB;
using TShockAPI;
using Terraria;

namespace HousingDistricts
{
	public class HouseManager
	{
		const string cols = "Name, TopX, TopY, BottomX, BottomY, Owners, WorldID, Locked, ChatEnabled, Visitors";
		public static bool AddHouse(int tx, int ty, int width, int height, string housename, string owner, int locked, int chatenabled)
		{
			if (HTools.GetHouseByName(housename) != null)
				return false;

			try
			{
				TShock.DB.Query("INSERT INTO HousingDistrict (" + cols + ") VALUES (@0, @1, @2, @3, @4, @5, @6, @7, @8, @9);", housename, tx, ty, width, height, "", Main.worldID.ToString(), locked, chatenabled, "");
			}
			catch (Exception ex)
			{
				TShock.Log.Error(ex.ToString());
				return false;
			}

			HousingDistricts.Houses.Add(new House(new Rectangle(tx, ty, width, height), new List<string>(), (HousingDistricts.Houses.Count + 1), housename, locked, chatenabled, new List<string>()));
			return true;
		}

		public static bool AddNewUser(string houseName, string id)
		{
			var house = HTools.GetHouseByName(houseName);
			if (house == null)
				return false;
			StringBuilder sb = new StringBuilder();
			int count = 0;
			house.Owners.Add(id);
			var I = house.Owners.Count;
			for (int i = 0; i < I; i++)
			{
				var owner = house.Owners[i];
				count++;
				sb.Append(owner);
				if (count != house.Owners.Count)
					sb.Append(",");
			}

			try
			{
				string query = "UPDATE HousingDistrict SET Owners=@0 WHERE Name=@1";

				TShock.DB.Query(query, sb.ToString(), houseName);
			}
			catch (Exception ex)
			{
				TShock.Log.Error(ex.ToString());
				return false;
			}

			return true;
		}

		public static bool DeleteUser(string houseName, string id)
		{
			var house = HTools.GetHouseByName(houseName);
			if (house == null)
				return false;

			StringBuilder sb = new StringBuilder();
			int count = 0;
			house.Owners.Remove(id);
			var I = house.Owners.Count;
			for (int i = 0; i < I; i++)
			{
				var owner = house.Owners[i];
				count++;
				sb.Append(owner);
				if (count != house.Owners.Count)
					sb.Append(",");
			}

			try
			{
				string query = "UPDATE HousingDistrict SET Owners=@0 WHERE Name=@1";

				TShock.DB.Query(query, sb.ToString(), houseName);
			}
			catch (Exception ex)
			{
				TShock.Log.Error(ex.ToString());
				return false;
			}

			return true;
		}

		public static bool AddNewVisitor(House house, string id)
		{
			StringBuilder sb = new StringBuilder();
			int count = 0;
			house.Visitors.Add(id);
			var I = house.Visitors.Count;
			for (int i = 0; i < I; i++)
			{
				var visitor = house.Visitors[i];
				count++;
				sb.Append(visitor);
				if (count != house.Visitors.Count)
					sb.Append(",");
			}

			try
			{
				string query = "UPDATE HousingDistrict SET Visitors=@0 WHERE Name=@1";

				TShock.DB.Query(query, sb.ToString(), house.Name);
			}
			catch (Exception ex)
			{
				TShock.Log.Error(ex.ToString());
				return false;
			}

			return true;
		}

		public static bool DeleteVisitor(House house, string id)
		{
			StringBuilder sb = new StringBuilder();
			int count = 0;
			house.Visitors.Remove(id);
			var I = house.Visitors.Count;
			for (int i = 0; i < I; i++)
			{
				var visitor = house.Visitors[i];
				count++;
				sb.Append(visitor);
				if (count != house.Visitors.Count)
					sb.Append(",");
			}

			try
			{
				string query = "UPDATE HousingDistrict SET Visitors=@0 WHERE Name=@1";

				TShock.DB.Query(query, sb.ToString(), house.Name);
			}
			catch (Exception ex)
			{
				TShock.Log.Error(ex.ToString());
				return false;
			}

			return true;
		}

		public static bool ToggleChat(House house, int onOrOff)
		{
			house.ChatEnabled = onOrOff;

			try
			{
				string query = "UPDATE HousingDistrict SET ChatEnabled=@0 WHERE Name=@1";
				TShock.DB.Query(query, house.ChatEnabled.ToString(), house.Name);
			}
			catch (Exception ex)
			{
				TShock.Log.Error(ex.ToString());
				return false;
			}

			return true;
		}
		public static bool ChangeLock(House house)
		{
			bool locked = false;

			if (house.Locked == 0)
				locked = true;
			else
				locked = false;

			house.Locked = locked ? 1 : 0;

			try
			{
				string query = "UPDATE HousingDistrict SET Locked=@0 WHERE Name=@1";

				TShock.DB.Query(query, locked ? 1 : 0, house.Name);
			}
			catch (Exception ex)
			{
				TShock.Log.Error(ex.ToString());
				return false;
			}

			return locked;
		}
		public static bool RedefineHouse(int tx, int ty, int width, int height, string housename)
		{
			try
			{
				var house = HTools.GetHouseByName(housename);
				var houseName = house.Name;

				try
				{
					string query = "UPDATE HousingDistrict SET TopX=@0, TopY=@1, BottomX=@2, BottomY=@3, WorldID=@4 WHERE Name=@5";

					TShock.DB.Query(query, tx, ty, width, height, Main.worldID.ToString(), house.Name);
				}
				catch (Exception ex)
				{
					TShock.Log.Error(ex.ToString());
					return false;
				}

				house.HouseArea = new Rectangle(tx, ty, width, height);

				return true;
			}
			catch (Exception ex)
			{
				TShock.Log.Error("Error on redefining house: \n" + ex);
				return false;
			}
		}
	}
}
