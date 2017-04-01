using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria.GameInput;

namespace Terraria
{
	public class TestHighFPSIssues
	{
		private static List<double> _tapUpdates = new List<double>();
		private static List<double> _tapUpdateEnds = new List<double>();
		private static List<double> _tapDraws = new List<double>();
		private static int conU = 0;
		private static int conUH = 0;
		private static int conD = 0;
		private static int conDH = 0;
		private static int race = 0;

		public static void TapUpdate(GameTime gt)
		{
			TestHighFPSIssues._tapUpdates.Add(gt.TotalGameTime.TotalMilliseconds);
			TestHighFPSIssues.conD = 0;
			TestHighFPSIssues.race--;
			if (++TestHighFPSIssues.conU > TestHighFPSIssues.conUH)
			{
				TestHighFPSIssues.conUH = TestHighFPSIssues.conU;
			}
		}

		public static void TapUpdateEnd(GameTime gt)
		{
			TestHighFPSIssues._tapUpdateEnds.Add(gt.TotalGameTime.TotalMilliseconds);
		}

		public static void TapDraw(GameTime gt)
		{
			TestHighFPSIssues._tapDraws.Add(gt.TotalGameTime.TotalMilliseconds);
			TestHighFPSIssues.conU = 0;
			TestHighFPSIssues.race++;
			if (++TestHighFPSIssues.conD > TestHighFPSIssues.conDH)
			{
				TestHighFPSIssues.conDH = TestHighFPSIssues.conD;
			}
		}

		public static void Update(GameTime gt)
		{
			if (PlayerInput.Triggers.Current.Down)
			{
				TestHighFPSIssues.race = (TestHighFPSIssues.conUH = (TestHighFPSIssues.conDH = 0));
			}
			double totalMilliseconds = gt.TotalGameTime.TotalMilliseconds;
			double num = totalMilliseconds - 5000.0;
			while (TestHighFPSIssues._tapUpdates.Count >= 0)
			{
				if (TestHighFPSIssues._tapUpdates.Count == 0 || TestHighFPSIssues._tapUpdates[0] >= num)
				{
					while (TestHighFPSIssues._tapDraws.Count >= 0)
					{
						if (TestHighFPSIssues._tapDraws.Count == 0 || TestHighFPSIssues._tapDraws[0] >= num)
						{
							while (TestHighFPSIssues._tapUpdateEnds.Count > 0 && TestHighFPSIssues._tapUpdateEnds[0] < num)
							{
								TestHighFPSIssues._tapUpdateEnds.RemoveAt(0);
							}
							Main.versionNumber = string.Concat(new object[]
								{
									"total (u/d)   ",
									TestHighFPSIssues._tapUpdates.Count,
									" ",
									TestHighFPSIssues._tapUpdateEnds.Count,
									"  ",
									TestHighFPSIssues.race,
									" ",
									TestHighFPSIssues.conUH,
									" ",
									TestHighFPSIssues.conDH
								});
							Main.NewText(Main.versionNumber, 255, 255, 255, false);
							return;
						}
						TestHighFPSIssues._tapDraws.RemoveAt(0);
					}
				}
				TestHighFPSIssues._tapUpdates.RemoveAt(0);
			}
		}
	}
}
