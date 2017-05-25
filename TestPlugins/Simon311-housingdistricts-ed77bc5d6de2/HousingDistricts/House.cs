using System;
using System.Collections.Generic;

namespace HousingDistricts
{
	public class House
	{
		public Rectangle HouseArea { get; set; }
		public List<string> Owners { get; set; }
		public int ID { get; set; }
		public string Name { get; set; }
		//public string WorldID { get; set; }
		public int Locked { get; set; }
		public int ChatEnabled { get; set; }
		public List<string> Visitors { get; set; }

		//public House(Rectangle housearea, List<string> owners, int id, string name, string worldid, int locked, int chatenabled, List<string> visitors)
		public House(Rectangle housearea, List<string> owners, int id, string name, int locked, int chatenabled, List<string> visitors)
		{
			HouseArea = housearea;
			Owners = owners;
			ID = id;
			Name = name;
			//WorldID = worldid;
			Locked = locked;
			ChatEnabled = chatenabled;
			Visitors = visitors;
		}

		public static bool HandlerAction(Func<House, bool> F)
		{
			int I = HousingDistricts.Houses.Count;
			bool Return = false;
			for (int i = 0; i < I; i++)
			{
				Return = F(HousingDistricts.Houses[i]);
				if (Return) return true;
			}
			return false;
		}

		public static void UpdateAction(Action<House> F)
		{
			int I = HousingDistricts.Houses.Count;
			for (int i = 0; i < I; i++)
				F(HousingDistricts.Houses[i]);
		}
	}
}
