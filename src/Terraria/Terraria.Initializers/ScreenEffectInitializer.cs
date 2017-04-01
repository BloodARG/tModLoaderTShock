using Microsoft.Xna.Framework;
using System;
using Terraria.GameContent.Shaders;
using Terraria.GameContent.Skies;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;

namespace Terraria.Initializers
{
	public static class ScreenEffectInitializer
	{
		public static void Load()
		{
			Filters.Scene["Nebula"] = new Filter(new ScreenShaderData("FilterTower").UseColor(1f, 0f, 0.9f).UseOpacity(0.5f), EffectPriority.High);
			Filters.Scene["Solar"] = new Filter(new ScreenShaderData("FilterTower").UseColor(1f, 0.7f, 0f).UseOpacity(0.3f), EffectPriority.High);
			Filters.Scene["Stardust"] = new Filter(new ScreenShaderData("FilterTower").UseColor(0f, 0.5f, 1f).UseOpacity(0.5f), EffectPriority.High);
			Filters.Scene["Vortex"] = new Filter(new ScreenShaderData("FilterTower").UseColor(0f, 0.7f, 0.7f).UseOpacity(0.5f), EffectPriority.High);
			Filters.Scene["MonolithNebula"] = new Filter(new ScreenShaderData("FilterMiniTower").UseColor(1f, 0f, 0.9f).UseOpacity(0.5f), EffectPriority.Medium);
			Filters.Scene["MonolithSolar"] = new Filter(new ScreenShaderData("FilterMiniTower").UseColor(1f, 0.7f, 0f).UseOpacity(0.3f), EffectPriority.Medium);
			Filters.Scene["MonolithStardust"] = new Filter(new ScreenShaderData("FilterMiniTower").UseColor(0f, 0.5f, 1f).UseOpacity(0.5f), EffectPriority.Medium);
			Filters.Scene["MonolithVortex"] = new Filter(new ScreenShaderData("FilterMiniTower").UseColor(0f, 0.7f, 0.7f).UseOpacity(0.5f), EffectPriority.Medium);
			Filters.Scene["MoonLord"] = new Filter(new MoonLordScreenShaderData("FilterMoonLord"), EffectPriority.VeryHigh);
			Filters.Scene["MoonLordShake"] = new Filter(new MoonLordScreenShaderData("FilterMoonLordShake"), EffectPriority.VeryHigh);
			Filters.Scene["testInvert"] = new Filter(new ScreenShaderData("FilterInvert"), EffectPriority.VeryHigh);
			Filters.Scene["BloodMoon"] = new Filter(new BloodMoonScreenShaderData("FilterBloodMoon").UseColor(2f, -0.8f, -0.6f), EffectPriority.Medium);
			Filters.Scene["Sandstorm"] = new Filter(new SandstormShaderData("FilterSandstormForeground").UseColor(1.1f, 1f, 0.5f).UseSecondaryColor(0.7f, 0.5f, 0.3f).UseImage("Images/Misc/noise", 0, null).UseIntensity(0.4f), EffectPriority.High);
			Overlays.Scene["Sandstorm"] = new SimpleOverlay("Images/Misc/noise", new SandstormShaderData("FilterSandstormBackground").UseColor(1.1f, 1f, 0.5f).UseSecondaryColor(0.7f, 0.5f, 0.3f).UseImage("Images/Misc/noise", 0, null).UseIntensity(0.4f), EffectPriority.High, RenderLayers.Landscape);
			Filters.Scene["Blizzard"] = new Filter(new BlizzardShaderData("FilterBlizzardForeground").UseColor(1f, 1f, 1f).UseSecondaryColor(0.7f, 0.7f, 1f).UseImage("Images/Misc/noise", 0, null).UseIntensity(0.4f).UseImageScale(new Vector2(3f, 0.75f)), EffectPriority.High);
			Overlays.Scene["Blizzard"] = new SimpleOverlay("Images/Misc/noise", new BlizzardShaderData("FilterBlizzardBackground").UseColor(1f, 1f, 1f).UseSecondaryColor(0.7f, 0.7f, 1f).UseImage("Images/Misc/noise", 0, null).UseIntensity(0.4f).UseImageScale(new Vector2(3f, 0.75f)), EffectPriority.High, RenderLayers.Landscape);
			Filters.Scene["HeatDistortion"] = new Filter(new ScreenShaderData("FilterHeatDistortion").UseImage("Images/Misc/noise", 0, null).UseIntensity(4f), EffectPriority.Low);
			Filters.Scene["WaterDistortion"] = new Filter(new WaterShaderData("FilterWaterDistortion").UseIntensity(1f).UseImage("Images/Misc/noise", 0, null), EffectPriority.VeryHigh);
			Filters.Scene["CrystalDestructionColor"] = new Filter(new ScreenShaderData("FilterCrystalDestructionColor").UseColor(1f, 0f, 0.75f).UseIntensity(1f).UseOpacity(0.8f), EffectPriority.VeryHigh);
			Filters.Scene["CrystalDestructionVortex"] = new Filter(new ScreenShaderData("FilterCrystalDestructionVortex").UseImage("Images/Misc/noise", 0, null), EffectPriority.VeryHigh);
			Filters.Scene["CrystalWin"] = new Filter(new ScreenShaderData("FilterCrystalWin"), EffectPriority.VeryHigh);
			Filters.Scene["Test"] = new Filter(new ScreenShaderData("FilterTest"), EffectPriority.VeryHigh);
			Filters.Scene["Test2"] = new Filter(new ScreenShaderData("FilterTest2"), EffectPriority.VeryHigh);
			Overlays.Scene.Load();
			Filters.Scene.Load();
			ScreenEffectInitializer.LoadSkies();
		}

		private static void LoadSkies()
		{
			SkyManager.Instance["Party"] = new PartySky();
			SkyManager.Instance["Martian"] = new MartianSky();
			SkyManager.Instance["Nebula"] = new NebulaSky();
			SkyManager.Instance["Stardust"] = new StardustSky();
			SkyManager.Instance["Vortex"] = new VortexSky();
			SkyManager.Instance["Solar"] = new SolarSky();
			SkyManager.Instance["Slime"] = new SlimeSky();
			SkyManager.Instance["MoonLord"] = new MoonLordSky();
			SkyManager.Instance["MonolithNebula"] = new NebulaSky();
			SkyManager.Instance["MonolithStardust"] = new StardustSky();
			SkyManager.Instance["MonolithVortex"] = new VortexSky();
			SkyManager.Instance["MonolithSolar"] = new SolarSky();
			SkyManager.Instance["Sandstorm"] = new SandstormSky();
			SkyManager.Instance["Blizzard"] = new BlizzardSky();
			SkyManager.Instance.Load();
		}
	}
}
