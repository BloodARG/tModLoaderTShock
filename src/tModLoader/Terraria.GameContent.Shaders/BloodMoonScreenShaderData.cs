using System;
using Terraria.Graphics.Shaders;

namespace Terraria.GameContent.Shaders
{
	public class BloodMoonScreenShaderData : ScreenShaderData
	{
		public BloodMoonScreenShaderData(string passName)
			: base(passName)
		{
		}

		public override void Apply()
		{
			float num = 1f - Utils.SmoothStep((float)Main.worldSurface + 50f, (float)Main.rockLayer + 100f, (Main.screenPosition.Y + (float)(Main.screenHeight / 2)) / 16f);
			base.UseOpacity(num * 0.75f);
			base.Apply();
		}
	}
}
