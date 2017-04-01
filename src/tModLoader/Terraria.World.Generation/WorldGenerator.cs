using System;
using System.Collections.Generic;
using System.Diagnostics;
using Terraria.GameContent.UI.States;
using Terraria.Utilities;

namespace Terraria.World.Generation
{
	public class WorldGenerator
	{
		internal List<GenPass> _passes = new List<GenPass>();
		internal float _totalLoadWeight;
		internal int _seed;

		public WorldGenerator(int seed)
		{
			this._seed = seed;
		}

		public void Append(GenPass pass)
		{
			this._passes.Add(pass);
			this._totalLoadWeight += pass.Weight;
		}

		public void GenerateWorld(GenerationProgress progress = null)
		{
			Stopwatch stopwatch = new Stopwatch();
			float num = 0f;
			foreach (GenPass current in this._passes)
			{
				num += current.Weight;
			}
			if (progress == null)
			{
				progress = new GenerationProgress();
			}
			progress.TotalWeight = num;
			Main.menuMode = 888;
			Main.MenuUI.SetState(new UIWorldLoad(progress));
			foreach (GenPass current2 in this._passes)
			{
				WorldGen._genRand = new UnifiedRandom(this._seed);
				Main.rand = new UnifiedRandom(this._seed);
				stopwatch.Start();
				progress.Start(current2.Weight);
				current2.Apply(progress);
				progress.End();
				stopwatch.Reset();
			}
		}
	}
}
