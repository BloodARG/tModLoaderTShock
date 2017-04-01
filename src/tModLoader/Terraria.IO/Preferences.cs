using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Terraria.Localization;

namespace Terraria.IO
{
	public class Preferences
	{
		public delegate void TextProcessAction(ref string text);

		private Dictionary<string, object> _data = new Dictionary<string, object>();
		private readonly string _path;
		private readonly JsonSerializerSettings _serializerSettings;
		public readonly bool UseBson;
		private readonly object _lock = new object();
		public bool AutoSave;

		public event Action<Preferences> OnSave;
		public event Action<Preferences> OnLoad;
		public event Preferences.TextProcessAction OnProcessText;

		public Preferences(string path, bool parseAllTypes = false, bool useBson = false)
		{
			this._path = path;
			this.UseBson = useBson;
			if (parseAllTypes)
			{
				this._serializerSettings = new JsonSerializerSettings
				{
					TypeNameHandling = TypeNameHandling.Auto,
					MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead,
					Formatting = Formatting.Indented
				};
				return;
			}
			this._serializerSettings = new JsonSerializerSettings
			{
				Formatting = Formatting.Indented
			};
		}

		public bool Load()
		{
			bool result;
			lock (this._lock)
			{
				if (!File.Exists(this._path))
				{
					result = false;
				}
				else
				{
					try
					{
						if (!this.UseBson)
						{
							string value = File.ReadAllText(this._path);
							this._data = JsonConvert.DeserializeObject<Dictionary<string, object>>(value, this._serializerSettings);
						}
						else
						{
							using (FileStream fileStream = File.OpenRead(this._path))
							{
								using (BsonReader bsonReader = new BsonReader(fileStream))
								{
									JsonSerializer jsonSerializer = JsonSerializer.Create(this._serializerSettings);
									this._data = jsonSerializer.Deserialize<Dictionary<string, object>>(bsonReader);
								}
							}
						}
						if (this._data == null)
						{
							this._data = new Dictionary<string, object>();
						}
						if (this.OnLoad != null)
						{
							this.OnLoad(this);
						}
						result = true;
					}
					catch (Exception)
					{
						result = false;
					}
				}
			}
			return result;
		}

		public bool Save(bool createFile = true)
		{
			bool result;
			lock (this._lock)
			{
				try
				{
					if (this.OnSave != null)
					{
						this.OnSave(this);
					}
					if (!createFile && !File.Exists(this._path))
					{
						result = false;
						return result;
					}
					Directory.GetParent(this._path).Create();
					if (!createFile)
					{
						File.SetAttributes(this._path, FileAttributes.Normal);
					}
					if (!this.UseBson)
					{
						string contents = JsonConvert.SerializeObject(this._data, this._serializerSettings);
						if (this.OnProcessText != null)
						{
							this.OnProcessText(ref contents);
						}
						File.WriteAllText(this._path, contents);
						File.SetAttributes(this._path, FileAttributes.Normal);
					}
					else
					{
						using (FileStream fileStream = File.Create(this._path))
						{
							using (BsonWriter bsonWriter = new BsonWriter(fileStream))
							{
								File.SetAttributes(this._path, FileAttributes.Normal);
								JsonSerializer jsonSerializer = JsonSerializer.Create(this._serializerSettings);
								jsonSerializer.Serialize(bsonWriter, this._data);
							}
						}
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine(Language.GetTextValue("Error.UnableToWritePreferences", this._path));
					Console.WriteLine(ex.ToString());
					Monitor.Exit(this._lock);
					result = false;
					return result;
				}
				result = true;
			}
			return result;
		}

		public void Clear()
		{
			this._data.Clear();
		}

		public void Put(string name, object value)
		{
			lock (this._lock)
			{
				this._data[name] = value;
				if (this.AutoSave)
				{
					this.Save(true);
				}
			}
		}

		public T Get<T>(string name, T defaultValue)
		{
			T result;
			lock (this._lock)
			{
				try
				{
					object obj;
					if (this._data.TryGetValue(name, out obj))
					{
						if (obj is T)
						{
							result = (T)((object)obj);
						}
						else if (obj is JObject)
						{
							result = JsonConvert.DeserializeObject<T>(((JObject)obj).ToString());
						}
						else
						{
							result = (T)((object)Convert.ChangeType(obj, typeof(T)));
						}
					}
					else
					{
						result = defaultValue;
					}
				}
				catch
				{
					result = defaultValue;
				}
			}
			return result;
		}

		public void Get<T>(string name, ref T currentValue)
		{
			currentValue = this.Get<T>(name, currentValue);
		}

		public List<string> GetAllKeys()
		{
			return this._data.Keys.ToList<string>();
		}
	}
}
