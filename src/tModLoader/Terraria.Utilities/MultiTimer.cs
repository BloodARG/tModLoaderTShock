using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Terraria.Utilities
{
	public class MultiTimer
	{
		private struct TimerData
		{
			public readonly double Min;
			public readonly double Max;
			public readonly double Ticks;
			public readonly double Total;

			public double Average
			{
				get
				{
					return this.Total / this.Ticks;
				}
			}

			private TimerData(double min, double max, double ticks, double total)
			{
				this.Min = min;
				this.Max = max;
				this.Ticks = ticks;
				this.Total = total;
			}

			public TimerData(double startTime)
			{
				this.Min = startTime;
				this.Max = startTime;
				this.Ticks = 1.0;
				this.Total = startTime;
			}

			public MultiTimer.TimerData AddTick(double time)
			{
				return new MultiTimer.TimerData(Math.Min(this.Min, time), Math.Max(this.Max, time), this.Ticks + 1.0, this.Total + time);
			}
		}

		private int _ticksBetweenPrint = 100;
		private int _ticksElapsedForPrint;
		private Stopwatch _timer = new Stopwatch();
		private Dictionary<string, MultiTimer.TimerData> _timerDataMap = new Dictionary<string, MultiTimer.TimerData>();

		public MultiTimer()
		{
		}

		public MultiTimer(int ticksBetweenPrint)
		{
			this._ticksBetweenPrint = ticksBetweenPrint;
		}

		public void Start()
		{
			this._timer.Reset();
			this._timer.Start();
		}

		public void Record(string key)
		{
			double totalMilliseconds = this._timer.Elapsed.TotalMilliseconds;
			MultiTimer.TimerData timerData;
			if (!this._timerDataMap.TryGetValue(key, out timerData))
			{
				this._timerDataMap.Add(key, new MultiTimer.TimerData(totalMilliseconds));
			}
			else
			{
				this._timerDataMap[key] = timerData.AddTick(totalMilliseconds);
			}
			this._timer.Restart();
		}

		public bool StopAndPrint()
		{
			this._ticksElapsedForPrint++;
			if (this._ticksElapsedForPrint == this._ticksBetweenPrint)
			{
				this._ticksElapsedForPrint = 0;
				Console.WriteLine("Average elapsed time: ");
				double num = 0.0;
				int num2 = 0;
				foreach (KeyValuePair<string, MultiTimer.TimerData> current in this._timerDataMap)
				{
					num2 = Math.Max(current.Key.Length, num2);
				}
				foreach (KeyValuePair<string, MultiTimer.TimerData> current2 in this._timerDataMap)
				{
					MultiTimer.TimerData value = current2.Value;
					string text = new string(' ', num2 - current2.Key.Length);
					Console.WriteLine(string.Concat(new object[]
							{
								current2.Key,
								text,
								" : (Average: ",
								value.Average.ToString("F4"),
								" Min: ",
								value.Min.ToString("F4"),
								" Max: ",
								value.Max.ToString("F4"),
								" from ",
								(int)value.Ticks,
								" records)"
							}));
					num += value.Total;
				}
				this._timerDataMap.Clear();
				Console.WriteLine("Total : " + (float)num / (float)this._ticksBetweenPrint + "ms");
				return true;
			}
			return false;
		}
	}
}
