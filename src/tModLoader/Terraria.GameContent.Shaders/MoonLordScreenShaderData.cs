using System;
using Terraria.Graphics.Shaders;

namespace Terraria.GameContent.Shaders
{
	public class MoonLordScreenShaderData : ScreenShaderData
	{
		private int _moonLordIndex = -1;

		public MoonLordScreenShaderData(string passName)
			: base(passName)
		{
		}

		private void UpdateMoonLordIndex()
		{
			if (this._moonLordIndex >= 0 && Main.npc[this._moonLordIndex].active && Main.npc[this._moonLordIndex].type == 398)
			{
				return;
			}
			int moonLordIndex = -1;
			for (int i = 0; i < Main.npc.Length; i++)
			{
				if (Main.npc[i].active && Main.npc[i].type == 398)
				{
					moonLordIndex = i;
					break;
				}
			}
			this._moonLordIndex = moonLordIndex;
		}

		public override void Apply()
		{
			this.UpdateMoonLordIndex();
			if (this._moonLordIndex != -1)
			{
				base.UseTargetPosition(Main.npc[this._moonLordIndex].Center);
			}
			base.Apply();
		}
	}
}
