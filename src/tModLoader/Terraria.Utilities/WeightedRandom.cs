using System;
using System.Collections.Generic;
using System.Linq;

namespace Terraria.Utilities
{
	public class WeightedRandom<T>
	{
		public readonly List<Tuple<T, double>> elements = new List<Tuple<T, double>>();
		public readonly UnifiedRandom random;
		public bool needsRefresh = true;
		private double _totalWeight;

		public WeightedRandom()
		{
			this.random = new UnifiedRandom();
		}

		public WeightedRandom(int seed)
		{
			this.random = new UnifiedRandom(seed);
		}

		public WeightedRandom(UnifiedRandom random)
		{
			this.random = random;
		}

		public WeightedRandom(params Tuple<T, double>[] theElements)
		{
			this.random = new UnifiedRandom();
			this.elements = theElements.ToList<Tuple<T, double>>();
		}

		public WeightedRandom(int seed, params Tuple<T, double>[] theElements)
		{
			this.random = new UnifiedRandom(seed);
			this.elements = theElements.ToList<Tuple<T, double>>();
		}

		public WeightedRandom(UnifiedRandom random, params Tuple<T, double>[] theElements)
		{
			this.random = random;
			this.elements = theElements.ToList<Tuple<T, double>>();
		}

		public void Add(T element, double weight = 1.0)
		{
			this.elements.Add(new Tuple<T, double>(element, weight));
			this.needsRefresh = true;
		}

		public T Get()
		{
			if (this.needsRefresh)
			{
				this.CalculateTotalWeight();
			}
			double num = this.random.NextDouble();
			num *= this._totalWeight;
			foreach (Tuple<T, double> current in this.elements)
			{
				if (num <= current.Item2)
				{
					return current.Item1;
				}
				num -= current.Item2;
			}
			return default(T);
		}

		public void CalculateTotalWeight()
		{
			this._totalWeight = 0.0;
			foreach (Tuple<T, double> current in this.elements)
			{
				this._totalWeight += current.Item2;
			}
			this.needsRefresh = false;
		}

		public void Clear()
		{
			this.elements.Clear();
		}

		public static implicit operator T(WeightedRandom<T> weightedRandom)
		{
			return weightedRandom.Get();
		}
	}
}
