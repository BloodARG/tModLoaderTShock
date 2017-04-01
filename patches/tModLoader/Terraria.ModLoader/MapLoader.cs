using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Map;

namespace Terraria.ModLoader
{
	//todo: further documentation
	internal static class MapLoader
	{
		internal static bool initialized = false;
		internal static readonly IDictionary<ushort, IList<MapEntry>> tileEntries = new Dictionary<ushort, IList<MapEntry>>();
		internal static readonly IDictionary<ushort, IList<MapEntry>> wallEntries = new Dictionary<ushort, IList<MapEntry>>();
		internal static readonly IDictionary<ushort, Func<string, int, int, string>> nameFuncs =
			new Dictionary<ushort, Func<string, int, int, string>>();
		internal static readonly IDictionary<ushort, ushort> entryToTile = new Dictionary<ushort, ushort>();
		internal static readonly IDictionary<ushort, ushort> entryToWall = new Dictionary<ushort, ushort>();

		internal static int modTileOptions(ushort type)
		{
			return tileEntries[type].Count;
		}

		internal static int modWallOptions(ushort type)
		{
			return wallEntries[type].Count;
		}
		//make Terraria.Map.MapHelper.colorLookup internal
		//add internal modPosition field to Terraria.Map.MapHelper
		//near end of Terraria.Map.MapHelper.Initialize set modPosition to num11 + 1
		//in Terraria.Map.MapHelper.SaveMap add mod-type-check to darkness check
		internal static void SetupModMap()
		{
			if (Main.dedServ)
			{
				return;
			}
			Array.Resize(ref MapHelper.tileLookup, TileLoader.TileCount);
			Array.Resize(ref MapHelper.wallLookup, WallLoader.WallCount);
			IList<Color> colors = new List<Color>();
			IList<string> names = new List<string>();
			foreach (ushort type in tileEntries.Keys)
			{
				MapHelper.tileLookup[type] = (ushort)(MapHelper.modPosition + colors.Count);
				foreach (MapEntry entry in tileEntries[type])
				{
					ushort mapType = (ushort)(MapHelper.modPosition + colors.Count);
					entryToTile[mapType] = type;
					nameFuncs[mapType] = entry.getName;
					colors.Add(entry.color);
					names.Add(entry.name);
				}
			}
			foreach (ushort type in wallEntries.Keys)
			{
				MapHelper.wallLookup[type] = (ushort)(MapHelper.modPosition + colors.Count);
				foreach (MapEntry entry in wallEntries[type])
				{
					ushort mapType = (ushort)(MapHelper.modPosition + colors.Count);
					entryToWall[mapType] = type;
					nameFuncs[mapType] = entry.getName;
					colors.Add(entry.color);
					names.Add(entry.name);
				}
			}
			Array.Resize(ref MapHelper.colorLookup, MapHelper.modPosition + colors.Count);
			Lang.mapLegend.Resize(MapHelper.modPosition + names.Count);
			for (int k = 0; k < colors.Count; k++)
			{
				MapHelper.colorLookup[MapHelper.modPosition + k] = colors[k];
				Lang.mapLegend[MapHelper.modPosition + k] = names[k];
			}
			initialized = true;
		}

		internal static void UnloadModMap()
		{
			tileEntries.Clear();
			wallEntries.Clear();
			if (Main.dedServ)
			{
				return;
			}
			nameFuncs.Clear();
			entryToTile.Clear();
			entryToWall.Clear();
			Array.Resize(ref MapHelper.tileLookup, TileID.Count);
			Array.Resize(ref MapHelper.wallLookup, WallID.Count);
			Array.Resize(ref MapHelper.colorLookup, MapHelper.modPosition);
			Lang.mapLegend.Resize(MapHelper.modPosition);
			initialized = false;
		}
		//at end of Terraria.Map.MapHelper.CreateMapTile before returning call
		//  MapLoader.ModMapOption(ref num16, i, j);
		internal static void ModMapOption(ref int mapType, int i, int j)
		{
			if (entryToTile.ContainsKey((ushort)mapType))
			{
				ModTile tile = TileLoader.GetTile(entryToTile[(ushort)mapType]);
				ushort option = tile.GetMapOption(i, j);
				if (option < 0 || option >= modTileOptions(tile.Type))
				{
					throw new ArgumentOutOfRangeException("Bad map option for tile " + tile.Name + " from mod " + tile.mod.Name);
				}
				mapType += option;
			}
			else if (entryToWall.ContainsKey((ushort)mapType))
			{
				ModWall wall = WallLoader.GetWall(entryToWall[(ushort)mapType]);
				ushort option = wall.GetMapOption(i, j);
				if (option < 0 || option >= modWallOptions(wall.Type))
				{
					throw new ArgumentOutOfRangeException("Bad map option for wall " + wall.Name + " from mod " + wall.mod.Name);
				}
				mapType += option;
			}
		}
	}
}
