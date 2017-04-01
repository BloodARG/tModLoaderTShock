using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using Terraria.ModLoader.Exceptions;
using Terraria.ModLoader.IO;
using Terraria.Social;
using Terraria.Utilities;

namespace Terraria.IO
{
	public class PlayerFileData : FileData
	{
		private Player _player;
		private TimeSpan _playTime = TimeSpan.Zero;
		private Stopwatch _timer = new Stopwatch();
		private bool _isTimerActive;
        public CustomModDataException customDataFail = null;

		public Player Player
		{
			get
			{
				return this._player;
			}
			set
			{
				this._player = value;
				if (value != null)
				{
					this.Name = this._player.name;
				}
			}
		}

		public PlayerFileData()
			: base("Player")
		{
		}

		public PlayerFileData(string path, bool cloudSave)
			: base("Player", path, cloudSave)
		{
		}

		public static PlayerFileData CreateAndSave(Player player)
		{
			PlayerFileData playerFileData = new PlayerFileData();
			playerFileData.Metadata = FileMetadata.FromCurrentSettings(FileType.Player);
			playerFileData.Player = player;
			playerFileData._isCloudSave = (SocialAPI.Cloud != null && SocialAPI.Cloud.EnabledByDefault);
			playerFileData._path = Main.GetPlayerPathFromName(player.name, playerFileData.IsCloudSave);
			(playerFileData.IsCloudSave ? Main.CloudFavoritesData : Main.LocalFavoriteData).ClearEntry(playerFileData);
			Player.SavePlayer(playerFileData, true);
			return playerFileData;
		}

		public override void SetAsActive()
		{
			Main.ActivePlayerFileData = this;
			Main.player[Main.myPlayer] = this.Player;
		}

		public override void MoveToCloud()
		{
			if (base.IsCloudSave || SocialAPI.Cloud == null)
			{
				return;
			}
			string playerPathFromName = Main.GetPlayerPathFromName(this.Name, true);
			if (FileUtilities.MoveToCloud(base.Path, playerPathFromName))
			{
				string fileName = base.GetFileName(false);
				string path = string.Concat(new object[]
					{
						Main.PlayerPath,
						System.IO.Path.DirectorySeparatorChar,
						fileName,
						System.IO.Path.DirectorySeparatorChar
					});
				if (Directory.Exists(path))
				{
					string[] files = Directory.GetFiles(path);
					for (int i = 0; i < files.Length; i++)
					{
						string cloudPath = string.Concat(new string[]
							{
								Main.CloudPlayerPath,
								"/",
								fileName,
								"/",
								FileUtilities.GetFileName(files[i], true)
							});
						FileUtilities.MoveToCloud(files[i], cloudPath);
					}
				}
				Main.LocalFavoriteData.ClearEntry(this);
				this._isCloudSave = true;
				this._path = playerPathFromName;
				Main.CloudFavoritesData.SaveFavorite(this);
				PlayerIO.MoveToCloud(base.Path, playerPathFromName);
			}
		}

		public override void MoveToLocal()
		{
			if (!base.IsCloudSave || SocialAPI.Cloud == null)
			{
				return;
			}
			string playerPathFromName = Main.GetPlayerPathFromName(this.Name, false);
			if (FileUtilities.MoveToLocal(base.Path, playerPathFromName))
			{
				string fileName = base.GetFileName(false);
				char directorySeparatorChar = System.IO.Path.DirectorySeparatorChar;
				string matchPattern = Regex.Escape(Main.CloudPlayerPath) + "/" + Regex.Escape(fileName) + "/.+\\.map";
				List<string> files = SocialAPI.Cloud.GetFiles(matchPattern);
				matchPattern = Regex.Escape(Main.CloudPlayerPath) + "/" + Regex.Escape(fileName) + "/.+\\.tmap";
				files.AddRange(SocialAPI.Cloud.GetFiles(matchPattern));
				for (int i = 0; i < files.Count; i++)
				{
					string localPath = string.Concat(new object[]
						{
							Main.PlayerPath,
							directorySeparatorChar,
							fileName,
							directorySeparatorChar,
							FileUtilities.GetFileName(files[i], true)
						});
					FileUtilities.MoveToLocal(files[i], localPath);
				}
				Main.CloudFavoritesData.ClearEntry(this);
				this._isCloudSave = false;
				this._path = playerPathFromName;
				Main.LocalFavoriteData.SaveFavorite(this);
				PlayerIO.MoveToLocal(base.Path, playerPathFromName);
			}
		}

		public void UpdatePlayTimer()
		{
			if (Main.instance.IsActive && !Main.gamePaused && Main.hasFocus && this._isTimerActive)
			{
				this.StartPlayTimer();
				return;
			}
			this.PausePlayTimer();
		}

		public void StartPlayTimer()
		{
			this._isTimerActive = true;
			if (!this._timer.IsRunning)
			{
				this._timer.Start();
			}
		}

		public void PausePlayTimer()
		{
			if (this._timer.IsRunning)
			{
				this._timer.Stop();
			}
		}

		public TimeSpan GetPlayTime()
		{
			if (this._timer.IsRunning)
			{
				return this._playTime + this._timer.Elapsed;
			}
			return this._playTime;
		}

		public void StopPlayTimer()
		{
			this._isTimerActive = false;
			if (this._timer.IsRunning)
			{
				this._playTime += this._timer.Elapsed;
				this._timer.Reset();
			}
		}

		public void SetPlayTime(TimeSpan time)
		{
			this._playTime = time;
		}
	}
}
