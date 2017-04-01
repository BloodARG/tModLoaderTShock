using System.IO;
using Terraria.ModLoader.IO;

namespace Terraria.ModLoader.Default
{
	public class MysteryItem : ModItem
	{
		private string modName;
		private string itemName;
		private TagCompound data;

		public override void SetDefaults()
		{
			item.name = "Unloaded Item";
			item.width = 20;
			item.height = 20;
			item.rare = 1;
		}

		internal void Setup(TagCompound tag)
		{
			this.modName = tag.GetString("mod");
			this.itemName = tag.GetString("name");
			this.data = tag;
			item.toolTip = "Mod: " + modName;
			item.toolTip2 = "Item: " + itemName;
		}

		public override TagCompound Save()
		{
			return data;
		}

		public override void Load(TagCompound tag)
		{
			Setup(tag);
			int type = ModLoader.GetMod(modName)?.ItemType(itemName) ?? 0;
			if (type > 0)
			{
				item.netDefaults(type);
				item.modItem.Load(tag.GetCompound("data"));
				ItemIO.LoadGlobals(item, tag.GetList<TagCompound>("globalData"));
			}
		}

		public override void LoadLegacy(BinaryReader reader)
		{
			string modName = reader.ReadString();
			bool hasGlobal = false;
			if (modName.Length == 0)
			{
				hasGlobal = true;
				modName = reader.ReadString();
			}
			Load(new TagCompound {
				["mod"] = modName,
				["name"] = reader.ReadString(),
				["hasGlobalSaving"] = hasGlobal,
				["legacyData"] = ItemIO.LegacyModData(int.MaxValue, reader, hasGlobal)
			});
		}

		public override ModItem Clone()
		{
			var clone = (MysteryItem)base.Clone();
			clone.data = (TagCompound) data?.Clone();
			return clone;
		}
	}
}
