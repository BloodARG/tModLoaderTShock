using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.Graphics;

namespace Terraria.Initializers
{
	public static class PlayerDataInitializer
	{
		public static void Load()
		{
			Main.playerTextures = new Texture2D[10, 15];
			PlayerDataInitializer.LoadStarterMale();
			PlayerDataInitializer.LoadStarterFemale();
			PlayerDataInitializer.LoadStickerMale();
			PlayerDataInitializer.LoadStickerFemale();
			PlayerDataInitializer.LoadGangsterMale();
			PlayerDataInitializer.LoadGangsterFemale();
			PlayerDataInitializer.LoadCoatMale();
			PlayerDataInitializer.LoadDressFemale();
			PlayerDataInitializer.LoadDressMale();
			PlayerDataInitializer.LoadCoatFemale();
		}

		private static void LoadDebugs()
		{
			PlayerDataInitializer.CopyVariant(8, 0);
			PlayerDataInitializer.CopyVariant(9, 4);
			for (int i = 8; i < 10; i++)
			{
				Main.playerTextures[i, 4] = Main.armorArmTexture[191];
				Main.playerTextures[i, 6] = Main.armorArmTexture[191];
				Main.playerTextures[i, 11] = Main.armorArmTexture[191];
				Main.playerTextures[i, 12] = Main.armorArmTexture[191];
				Main.playerTextures[i, 13] = Main.armorArmTexture[191];
				Main.playerTextures[i, 8] = Main.armorArmTexture[191];
			}
		}

		private static void LoadVariant(int ID, int[] pieceIDs)
		{
			for (int i = 0; i < pieceIDs.Length; i++)
			{
				Main.playerTextures[ID, pieceIDs[i]] = TextureManager.Load(string.Concat(new object[]
						{
							"Images/Player_",
							ID,
							"_",
							pieceIDs[i]
						}));
			}
		}

		private static void CopyVariant(int to, int from)
		{
			for (int i = 0; i < 15; i++)
			{
				Main.playerTextures[to, i] = Main.playerTextures[from, i];
			}
		}

		private static void LoadStarterMale()
		{
			PlayerDataInitializer.LoadVariant(0, new int[]
				{
					0,
					1,
					2,
					3,
					4,
					5,
					6,
					7,
					8,
					9,
					10,
					11,
					12,
					13
				});
			Main.playerTextures[0, 14] = TextureManager.BlankTexture;
		}

		private static void LoadStickerMale()
		{
			PlayerDataInitializer.CopyVariant(1, 0);
			PlayerDataInitializer.LoadVariant(1, new int[]
				{
					4,
					6,
					8,
					11,
					12,
					13
				});
		}

		private static void LoadGangsterMale()
		{
			PlayerDataInitializer.CopyVariant(2, 0);
			PlayerDataInitializer.LoadVariant(2, new int[]
				{
					4,
					6,
					8,
					11,
					12,
					13
				});
		}

		private static void LoadCoatMale()
		{
			PlayerDataInitializer.CopyVariant(3, 0);
			PlayerDataInitializer.LoadVariant(3, new int[]
				{
					4,
					6,
					8,
					11,
					12,
					13,
					14
				});
		}

		private static void LoadDressMale()
		{
			PlayerDataInitializer.CopyVariant(8, 0);
			PlayerDataInitializer.LoadVariant(8, new int[]
				{
					4,
					6,
					8,
					11,
					12,
					13,
					14
				});
		}

		private static void LoadStarterFemale()
		{
			PlayerDataInitializer.CopyVariant(4, 0);
			PlayerDataInitializer.LoadVariant(4, new int[]
				{
					3,
					4,
					5,
					6,
					7,
					8,
					9,
					10,
					11,
					12,
					13
				});
		}

		private static void LoadStickerFemale()
		{
			PlayerDataInitializer.CopyVariant(5, 4);
			PlayerDataInitializer.LoadVariant(5, new int[]
				{
					4,
					6,
					8,
					11,
					12,
					13
				});
		}

		private static void LoadGangsterFemale()
		{
			PlayerDataInitializer.CopyVariant(6, 4);
			PlayerDataInitializer.LoadVariant(6, new int[]
				{
					4,
					6,
					8,
					11,
					12,
					13
				});
		}

		private static void LoadCoatFemale()
		{
			PlayerDataInitializer.CopyVariant(7, 4);
			PlayerDataInitializer.LoadVariant(7, new int[]
				{
					4,
					6,
					8,
					11,
					12,
					13,
					14
				});
		}

		private static void LoadDressFemale()
		{
			PlayerDataInitializer.CopyVariant(9, 4);
			PlayerDataInitializer.LoadVariant(9, new int[]
				{
					4,
					6,
					8,
					11,
					12,
					13
				});
		}
	}
}
