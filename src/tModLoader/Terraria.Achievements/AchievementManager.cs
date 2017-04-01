using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Terraria.Social;
using Terraria.Utilities;

namespace Terraria.Achievements
{
	public class AchievementManager
	{
		private class StoredAchievement
		{
			public Dictionary<string, JObject> Conditions;
		}

		private string _savePath;
		private bool _isCloudSave;
		private Dictionary<string, Achievement> _achievements = new Dictionary<string, Achievement>();
		private readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings();
		private byte[] _cryptoKey;
		private Dictionary<string, int> _achievementIconIndexes = new Dictionary<string, int>();
		private static object _ioLock = new object();

		public event Achievement.AchievementCompleted OnAchievementCompleted;

		public AchievementManager()
		{
			if (SocialAPI.Achievements != null)
			{
				this._savePath = SocialAPI.Achievements.GetSavePath();
				this._isCloudSave = true;
				this._cryptoKey = SocialAPI.Achievements.GetEncryptionKey();
				return;
			}
			this._savePath = Main.SavePath + Path.DirectorySeparatorChar + "achievements.dat";
			this._isCloudSave = false;
			this._cryptoKey = Encoding.ASCII.GetBytes("RELOGIC-TERRARIA");
		}

		public void Save()
		{
			this.Save(this._savePath, this._isCloudSave);
		}

		private void Save(string path, bool cloud)
		{
			lock (AchievementManager._ioLock)
			{
				if (SocialAPI.Achievements != null)
				{
					SocialAPI.Achievements.StoreStats();
				}
				try
				{
					using (MemoryStream memoryStream = new MemoryStream())
					{
						using (CryptoStream cryptoStream = new CryptoStream(memoryStream, new RijndaelManaged().CreateEncryptor(this._cryptoKey, this._cryptoKey), CryptoStreamMode.Write))
						{
							using (BsonWriter bsonWriter = new BsonWriter(cryptoStream))
							{
								JsonSerializer jsonSerializer = JsonSerializer.Create(this._serializerSettings);
								jsonSerializer.Serialize(bsonWriter, this._achievements);
								bsonWriter.Flush();
								cryptoStream.FlushFinalBlock();
								FileUtilities.WriteAllBytes(path, memoryStream.ToArray(), cloud);
							}
						}
					}
				}
				catch (Exception)
				{
				}
			}
		}

		public List<Achievement> CreateAchievementsList()
		{
			return this._achievements.Values.ToList<Achievement>();
		}

		public void Load()
		{
			this.Load(this._savePath, this._isCloudSave);
		}

		private void Load(string path, bool cloud)
		{
			bool flag = false;
			lock (AchievementManager._ioLock)
			{
				if (!FileUtilities.Exists(path, cloud))
				{
					return;
				}
				byte[] buffer = FileUtilities.ReadAllBytes(path, cloud);
				Dictionary<string, AchievementManager.StoredAchievement> dictionary = null;
				try
				{
					using (MemoryStream memoryStream = new MemoryStream(buffer))
					{
						using (CryptoStream cryptoStream = new CryptoStream(memoryStream, new RijndaelManaged().CreateDecryptor(this._cryptoKey, this._cryptoKey), CryptoStreamMode.Read))
						{
							using (BsonReader bsonReader = new BsonReader(cryptoStream))
							{
								JsonSerializer jsonSerializer = JsonSerializer.Create(this._serializerSettings);
								dictionary = jsonSerializer.Deserialize<Dictionary<string, AchievementManager.StoredAchievement>>(bsonReader);
							}
						}
					}
				}
				catch (Exception)
				{
					FileUtilities.Delete(path, cloud);
					return;
				}
				if (dictionary == null)
				{
					return;
				}
				foreach (KeyValuePair<string, AchievementManager.StoredAchievement> current in dictionary)
				{
					if (this._achievements.ContainsKey(current.Key))
					{
						this._achievements[current.Key].Load(current.Value.Conditions);
					}
				}
				if (SocialAPI.Achievements != null)
				{
					foreach (KeyValuePair<string, Achievement> current2 in this._achievements)
					{
						if (current2.Value.IsCompleted && !SocialAPI.Achievements.IsAchievementCompleted(current2.Key))
						{
							flag = true;
							current2.Value.ClearProgress();
						}
					}
				}
			}
			if (flag)
			{
				this.Save();
			}
		}

		private void AchievementCompleted(Achievement achievement)
		{
			this.Save();
			if (this.OnAchievementCompleted != null)
			{
				this.OnAchievementCompleted(achievement);
			}
		}

		public void Register(Achievement achievement)
		{
			this._achievements.Add(achievement.Name, achievement);
			achievement.OnCompleted += new Achievement.AchievementCompleted(this.AchievementCompleted);
		}

		public void RegisterIconIndex(string achievementName, int iconIndex)
		{
			this._achievementIconIndexes.Add(achievementName, iconIndex);
		}

		public void RegisterAchievementCategory(string achievementName, AchievementCategory category)
		{
			this._achievements[achievementName].SetCategory(category);
		}

		public Achievement GetAchievement(string achievementName)
		{
			Achievement result;
			if (this._achievements.TryGetValue(achievementName, out result))
			{
				return result;
			}
			return null;
		}

		public T GetCondition<T>(string achievementName, string conditionName) where T : AchievementCondition
		{
			return this.GetCondition(achievementName, conditionName) as T;
		}

		public AchievementCondition GetCondition(string achievementName, string conditionName)
		{
			Achievement achievement;
			if (this._achievements.TryGetValue(achievementName, out achievement))
			{
				return achievement.GetCondition(conditionName);
			}
			return null;
		}

		public int GetIconIndex(string achievementName)
		{
			int result;
			if (this._achievementIconIndexes.TryGetValue(achievementName, out result))
			{
				return result;
			}
			return 0;
		}
	}
}
