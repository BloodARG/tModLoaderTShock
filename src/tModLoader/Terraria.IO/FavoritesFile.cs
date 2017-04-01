using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Terraria.Utilities;

namespace Terraria.IO
{
	public class FavoritesFile
	{
		public readonly string Path;
		public readonly bool IsCloudSave;
		private Dictionary<string, Dictionary<string, bool>> _data = new Dictionary<string, Dictionary<string, bool>>();

		public FavoritesFile(string path, bool isCloud)
		{
			this.Path = path;
			this.IsCloudSave = isCloud;
		}

		public void SaveFavorite(FileData fileData)
		{
			if (!this._data.ContainsKey(fileData.Type))
			{
				this._data.Add(fileData.Type, new Dictionary<string, bool>());
			}
			this._data[fileData.Type][fileData.GetFileName(true)] = fileData.IsFavorite;
			this.Save();
		}

		public void ClearEntry(FileData fileData)
		{
			if (!this._data.ContainsKey(fileData.Type))
			{
				return;
			}
			this._data[fileData.Type].Remove(fileData.GetFileName(true));
			this.Save();
		}

		public bool IsFavorite(FileData fileData)
		{
			if (!this._data.ContainsKey(fileData.Type))
			{
				return false;
			}
			string fileName = fileData.GetFileName(true);
			bool flag;
			return this._data[fileData.Type].TryGetValue(fileName, out flag) && flag;
		}

		public void Save()
		{
			string s = JsonConvert.SerializeObject(this._data, Formatting.Indented);
			byte[] bytes = Encoding.ASCII.GetBytes(s);
			FileUtilities.WriteAllBytes(this.Path, bytes, this.IsCloudSave);
		}

		public void Load()
		{
			if (!FileUtilities.Exists(this.Path, this.IsCloudSave))
			{
				this._data.Clear();
				return;
			}
			byte[] bytes = FileUtilities.ReadAllBytes(this.Path, this.IsCloudSave);
			string @string = Encoding.ASCII.GetString(bytes);
			this._data = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, bool>>>(@string);
			if (this._data == null)
			{
				this._data = new Dictionary<string, Dictionary<string, bool>>();
			}
		}
	}
}
