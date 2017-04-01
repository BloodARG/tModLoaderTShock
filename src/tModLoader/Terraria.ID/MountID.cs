using System;

namespace Terraria.ID
{
	public static class MountID
	{
		public static class Sets
		{
			public static SetFactory Factory = new SetFactory(MountID.Count);
			public static bool[] Cart = MountID.Sets.Factory.CreateBoolSet(new int[]
				{
					6,
					11,
					13
				});
		}

		public const int Count = 15;
		public const int Rudolph = 0;
		public const int Bunny = 1;
		public const int Pigron = 2;
		public const int Slime = 3;
		public const int Turtle = 4;
		public const int Bee = 5;
		public const int MineCart = 6;
		public const int Ufo = 7;
		public const int DrillContainmentUnit = 8;
		public const int Scutlix = 9;
		public const int Unicorn = 10;
		public const int MineCartMech = 11;
		public const int CuteFishron = 12;
		public const int MineCartWood = 13;
		public const int Basilisk = 14;
	}
}
