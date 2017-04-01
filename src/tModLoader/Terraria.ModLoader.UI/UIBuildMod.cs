using System;
using System.IO;
using Terraria.UI;

namespace Terraria.ModLoader.UI
{
	internal class UIBuildMod : UIState, ModCompile.IBuildStatus
	{
		private UILoadProgress loadProgress;

		public override void OnInitialize()
		{
			loadProgress = new UILoadProgress();
			loadProgress.Width.Set(0f, 0.8f);
			loadProgress.MaxWidth.Set(600f, 0f);
			loadProgress.Height.Set(150f, 0f);
			loadProgress.HAlign = 0.5f;
			loadProgress.VAlign = 0.5f;
			loadProgress.Top.Set(10f, 0f);
			base.Append(loadProgress);
		}

		public void SetProgress(int num, int max)
		{
			loadProgress.SetProgress((float)num / (float)max);
		}

		public void SetStatus(string msg)
		{
			loadProgress.SetText(msg);
		}
	}
}
