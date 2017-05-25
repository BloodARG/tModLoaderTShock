using System.Collections.Generic;
using TShockAPI;

namespace HousingDistricts
{
	public class HPlayer
	{
		public int Index { get; set; }
		public TSPlayer TSPlayer { get { return TShock.Players[Index]; } }
		public List<string> CurHouses { get; set; }
		public Vector2 LastTilePos { get; set; }
		public bool AwaitingHouseName { get; set; }

		public HPlayer() : this(-1, Vector2.Zero) { }

		public HPlayer(int index, Vector2 lasttilepos)
		{
			Index = index;
			CurHouses = new List<string>();
			LastTilePos = lasttilepos;
		}
	}
}
