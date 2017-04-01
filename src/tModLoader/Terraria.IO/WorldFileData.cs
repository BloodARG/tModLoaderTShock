using System;
using System.IO;
using Terraria.Localization;
using Terraria.ModLoader.IO;
using Terraria.Utilities;

namespace Terraria.IO
{
	public class WorldFileData : FileData
	{
		private const ulong GUID_IN_WORLD_FILE_VERSION = 777389080577uL;
		public DateTime CreationTime;
		public int WorldSizeX;
		public int WorldSizeY;
		public ulong WorldGeneratorVersion;
		private string _seedText = "";
		private int _seed;
		public bool IsValid = true;
		public Guid UniqueId;
		public LocalizedText _worldSizeName;
		public bool IsExpertMode;
		public bool HasCorruption = true;
		public bool IsHardMode;

		public string SeedText
		{
			get
			{
				return this._seedText;
			}
		}

		public int Seed
		{
			get
			{
				return this._seed;
			}
		}

		public string WorldSizeName
		{
			get
			{
				return this._worldSizeName.Value;
			}
		}

		public bool HasCrimson
		{
			get
			{
				return !this.HasCorruption;
			}
			set
			{
				this.HasCorruption = !value;
			}
		}

		public bool HasValidSeed
		{
			get
			{
				return this.WorldGeneratorVersion != 0uL;
			}
		}

		public bool UseGuidAsMapName
		{
			get
			{
				return this.WorldGeneratorVersion >= 777389080577uL;
			}
		}

		public WorldFileData()
			: base("World")
		{
		}

		public WorldFileData(string path, bool cloudSave)
			: base("World", path, cloudSave)
		{
		}

		public override void SetAsActive()
		{
			Main.ActiveWorldFileData = this;
		}

		public void SetWorldSize(int x, int y)
		{
			this.WorldSizeX = x;
			this.WorldSizeY = y;
			if (x == 4200)
			{
				this._worldSizeName = Language.GetText("UI.WorldSizeSmall");
				return;
			}
			if (x == 6400)
			{
				this._worldSizeName = Language.GetText("UI.WorldSizeMedium");
				return;
			}
			if (x != 8400)
			{
				this._worldSizeName = Language.GetText("UI.WorldSizeUnknown");
				return;
			}
			this._worldSizeName = Language.GetText("UI.WorldSizeLarge");
		}

		public static WorldFileData FromInvalidWorld(string path, bool cloudSave)
		{
			WorldFileData worldFileData = new WorldFileData(path, cloudSave);
			worldFileData.IsExpertMode = false;
			worldFileData.SetSeedToEmpty();
			worldFileData.WorldGeneratorVersion = 0uL;
			worldFileData.Metadata = FileMetadata.FromCurrentSettings(FileType.World);
			worldFileData.SetWorldSize(1, 1);
			worldFileData.HasCorruption = true;
			worldFileData.IsHardMode = false;
			worldFileData.IsValid = false;
			worldFileData.Name = FileUtilities.GetFileName(path, false);
			worldFileData.UniqueId = Guid.Empty;
			if (!cloudSave)
			{
				worldFileData.CreationTime = File.GetCreationTime(path);
			}
			else
			{
				worldFileData.CreationTime = DateTime.Now;
			}
			return worldFileData;
		}

		public void SetSeedToEmpty()
		{
			this.SetSeed("");
		}

		public void SetSeed(string seedText)
		{
			this._seedText = seedText;
			if (!int.TryParse(seedText, out this._seed))
			{
				this._seed = seedText.GetHashCode();
			}
			this._seed = Math.Abs(this._seed);
		}

		public void SetSeedToRandom()
		{
			this.SetSeed(new UnifiedRandom().Next().ToString());
		}

		public override void MoveToCloud()
		{
			if (base.IsCloudSave)
			{
				return;
			}
			string worldPathFromName = Main.GetWorldPathFromName(this.Name, true);
			if (FileUtilities.MoveToCloud(base.Path, worldPathFromName))
			{
				Main.LocalFavoriteData.ClearEntry(this);
				this._isCloudSave = true;
				this._path = worldPathFromName;
				Main.CloudFavoritesData.SaveFavorite(this);
				WorldIO.MoveToCloud(base.Path, worldPathFromName);
			}
		}

		public override void MoveToLocal()
		{
			if (!base.IsCloudSave)
			{
				return;
			}
			string worldPathFromName = Main.GetWorldPathFromName(this.Name, false);
			if (FileUtilities.MoveToLocal(base.Path, worldPathFromName))
			{
				Main.CloudFavoritesData.ClearEntry(this);
				this._isCloudSave = false;
				this._path = worldPathFromName;
				Main.LocalFavoriteData.SaveFavorite(this);
				WorldIO.MoveToLocal(base.Path, worldPathFromName);
			}
		}
	}
}
