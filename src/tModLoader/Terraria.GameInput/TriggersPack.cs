using System;
using System.Linq;

namespace Terraria.GameInput
{
	public class TriggersPack
	{
		public TriggersSet Current = new TriggersSet();
		public TriggersSet Old = new TriggersSet();
		public TriggersSet JustPressed = new TriggersSet();
		public TriggersSet JustReleased = new TriggersSet();

		public void Initialize()
		{
			this.Current.SetupKeys();
			this.Old.SetupKeys();
			this.JustPressed.SetupKeys();
			this.JustReleased.SetupKeys();
		}

		public void Reset()
		{
			this.Old = this.Current.Clone();
			this.Current.Reset();
		}

		public void Update()
		{
			this.CompareDiffs(this.JustPressed, this.Old, this.Current);
			this.CompareDiffs(this.JustReleased, this.Current, this.Old);
		}

		public void CompareDiffs(TriggersSet Bearer, TriggersSet oldset, TriggersSet newset)
		{
			Bearer.Reset();
			foreach (string current in Bearer.KeyStatus.Keys.ToList<string>())
			{
				Bearer.KeyStatus[current] = (newset.KeyStatus[current] && !oldset.KeyStatus[current]);
			}
		}
	}
}
