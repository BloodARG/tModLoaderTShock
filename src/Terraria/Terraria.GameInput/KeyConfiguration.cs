using System;
using System.Collections.Generic;
using System.Linq;

namespace Terraria.GameInput
{
	public class KeyConfiguration
	{
		public Dictionary<string, List<string>> KeyStatus = new Dictionary<string, List<string>>();

		public bool DoGrappleAndInteractShareTheSameKey
		{
			get
			{
				return this.KeyStatus["Grapple"].Count > 0 && this.KeyStatus["MouseRight"].Count > 0 && this.KeyStatus["MouseRight"].Contains(this.KeyStatus["Grapple"][0]);
			}
		}

		public void SetupKeys()
		{
			this.KeyStatus.Clear();
			foreach (string current in PlayerInput.KnownTriggers)
			{
				this.KeyStatus.Add(current, new List<string>());
			}
		}

		public void Processkey(TriggersSet set, string newKey)
		{
			foreach (KeyValuePair<string, List<string>> current in this.KeyStatus)
			{
				if (current.Value.Contains(newKey))
				{
					set.KeyStatus[current.Key] = true;
				}
			}
			if (set.Up || set.Down || set.Left || set.Right || set.HotbarPlus || set.HotbarMinus || ((Main.gameMenu || Main.ingameOptionsWindow) && (set.MenuUp || set.MenuDown || set.MenuLeft || set.MenuRight)))
			{
				set.UsedMovementKey = true;
			}
		}

		public void CopyKeyState(TriggersSet oldSet, TriggersSet newSet, string newKey)
		{
			foreach (KeyValuePair<string, List<string>> current in this.KeyStatus)
			{
				if (current.Value.Contains(newKey))
				{
					newSet.KeyStatus[current.Key] = oldSet.KeyStatus[current.Key];
				}
			}
		}

		public void ReadPreferences(Dictionary<string, List<string>> dict)
		{
			foreach (KeyValuePair<string, List<string>> current in dict)
			{
				if (this.KeyStatus.ContainsKey(current.Key))
				{
					this.KeyStatus[current.Key].Clear();
					foreach (string current2 in current.Value)
					{
						this.KeyStatus[current.Key].Add(current2);
					}
				}
			}
		}

		public Dictionary<string, List<string>> WritePreferences()
		{
			Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();
			foreach (KeyValuePair<string, List<string>> current in this.KeyStatus)
			{
				if (current.Value.Count > 0)
				{
					dictionary.Add(current.Key, current.Value.ToList<string>());
				}
			}
			if (!dictionary.ContainsKey("MouseLeft") || dictionary["MouseLeft"].Count == 0)
			{
				dictionary.Add("MouseLeft", new List<string>
				{
					"Mouse1"
				});
			}
			if (!dictionary.ContainsKey("Inventory") || dictionary["Inventory"].Count == 0)
			{
				dictionary.Add("Inventory", new List<string>
				{
					"Escape"
				});
			}
			return dictionary;
		}
	}
}
