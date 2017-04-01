using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using Terraria.Utilities;

namespace Terraria
{
	public class FrameSkipTest
	{
		private const int SamplesCount = 5;
		private static int LastRecordedSecondNumber;
		private static float CallsThisSecond = 0f;
		private static float DeltasThisSecond = 0f;
		private static List<float> DeltaSamples = new List<float>();
		private static MultiTimer serverFramerateTest = new MultiTimer(60);

		public static void Update(GameTime gameTime)
		{
			float num = 60f;
			float num2 = 1f / num;
			float num3 = (float)gameTime.ElapsedGameTime.TotalSeconds;
			float num4 = num2 - num3;
			num4 *= 1000f;
			num4 = MathHelper.Clamp(num4 + 1f, 0f, 1000f);
			Thread.Sleep((int)num4);
		}

		public static void CheckReset(GameTime gameTime)
		{
			if (FrameSkipTest.LastRecordedSecondNumber != gameTime.TotalGameTime.Seconds)
			{
				FrameSkipTest.DeltaSamples.Add(FrameSkipTest.DeltasThisSecond / FrameSkipTest.CallsThisSecond);
				if (FrameSkipTest.DeltaSamples.Count > 5)
				{
					FrameSkipTest.DeltaSamples.RemoveAt(0);
				}
				FrameSkipTest.CallsThisSecond = 0f;
				FrameSkipTest.DeltasThisSecond = 0f;
				FrameSkipTest.LastRecordedSecondNumber = gameTime.TotalGameTime.Seconds;
			}
		}

		public static void UpdateServerTest()
		{
			FrameSkipTest.serverFramerateTest.Record("frame time");
			FrameSkipTest.serverFramerateTest.StopAndPrint();
			FrameSkipTest.serverFramerateTest.Start();
		}
	}
}
