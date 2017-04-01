using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terraria.UI
{
	public class ItemSorting
	{
		private class ItemSortingLayer
		{
			public readonly string Name;
			public readonly Func<ItemSorting.ItemSortingLayer, Item[], List<int>, List<int>> SortingMethod;

			public ItemSortingLayer(string name, Func<ItemSorting.ItemSortingLayer, Item[], List<int>, List<int>> method)
			{
				this.Name = name;
				this.SortingMethod = method;
			}

			public void Validate(ref List<int> indexesSortable, Item[] inv)
			{
				List<int> list;
				if (ItemSorting._layerWhiteLists.TryGetValue(this.Name, out list))
				{
					indexesSortable = (from i in indexesSortable
					                   where list.Contains(inv[i].netID)
					                   select i).ToList<int>();
				}
			}

			public override string ToString()
			{
				return this.Name;
			}
		}

		private class ItemSortingLayers
		{
			public static ItemSorting.ItemSortingLayer WeaponsMelee = new ItemSorting.ItemSortingLayer("Weapons - Melee", delegate(ItemSorting.ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
				{
					List<int> list = (from i in itemsToSort
					                 where inv[i].maxStack == 1 && inv[i].damage > 0 && inv[i].ammo == 0 && inv[i].melee && inv[i].pick < 1 && inv[i].hammer < 1 && inv[i].axe < 1
					                 select i).ToList<int>();
					layer.Validate(ref list, inv);
					foreach (int current in list)
					{
						itemsToSort.Remove(current);
					}
					list.Sort(delegate(int x, int y)
						{
							int num = inv[y].rare.CompareTo(inv[x].rare);
							if (num == 0)
							{
								num = ((x == y) ? 0 : -1);
							}
							return num;
						});
					return list;
				});
			public static ItemSorting.ItemSortingLayer WeaponsRanged = new ItemSorting.ItemSortingLayer("Weapons - Ranged", delegate(ItemSorting.ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
				{
					List<int> list = (from i in itemsToSort
					                 where inv[i].maxStack == 1 && inv[i].damage > 0 && inv[i].ammo == 0 && inv[i].ranged
					                 select i).ToList<int>();
					layer.Validate(ref list, inv);
					foreach (int current in list)
					{
						itemsToSort.Remove(current);
					}
					list.Sort(delegate(int x, int y)
						{
							int num = inv[y].rare.CompareTo(inv[x].rare);
							if (num == 0)
							{
								num = ((x == y) ? 0 : -1);
							}
							return num;
						});
					return list;
				});
			public static ItemSorting.ItemSortingLayer WeaponsMagic = new ItemSorting.ItemSortingLayer("Weapons - Magic", delegate(ItemSorting.ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
				{
					List<int> list = (from i in itemsToSort
					                 where inv[i].maxStack == 1 && inv[i].damage > 0 && inv[i].ammo == 0 && inv[i].magic
					                 select i).ToList<int>();
					layer.Validate(ref list, inv);
					foreach (int current in list)
					{
						itemsToSort.Remove(current);
					}
					list.Sort(delegate(int x, int y)
						{
							int num = inv[y].rare.CompareTo(inv[x].rare);
							if (num == 0)
							{
								num = ((x == y) ? 0 : -1);
							}
							return num;
						});
					return list;
				});
			public static ItemSorting.ItemSortingLayer WeaponsMinions = new ItemSorting.ItemSortingLayer("Weapons - Minions", delegate(ItemSorting.ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
				{
					List<int> list = (from i in itemsToSort
					                 where inv[i].maxStack == 1 && inv[i].damage > 0 && inv[i].summon
					                 select i).ToList<int>();
					layer.Validate(ref list, inv);
					foreach (int current in list)
					{
						itemsToSort.Remove(current);
					}
					list.Sort(delegate(int x, int y)
						{
							int num = inv[y].rare.CompareTo(inv[x].rare);
							if (num == 0)
							{
								num = ((x == y) ? 0 : -1);
							}
							return num;
						});
					return list;
				});
			public static ItemSorting.ItemSortingLayer WeaponsThrown = new ItemSorting.ItemSortingLayer("Weapons - Thrown", delegate(ItemSorting.ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
				{
					List<int> list = (from i in itemsToSort
					                 where inv[i].damage > 0 && (inv[i].ammo == 0 || inv[i].notAmmo) && inv[i].shoot > 0 && inv[i].thrown
					                 select i).ToList<int>();
					layer.Validate(ref list, inv);
					foreach (int current in list)
					{
						itemsToSort.Remove(current);
					}
					list.Sort(delegate(int x, int y)
						{
							int num = inv[y].rare.CompareTo(inv[x].rare);
							if (num == 0)
							{
								num = ((x == y) ? 0 : -1);
							}
							return num;
						});
					return list;
				});
			public static ItemSorting.ItemSortingLayer WeaponsAssorted = new ItemSorting.ItemSortingLayer("Weapons - Assorted", delegate(ItemSorting.ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
				{
					List<int> list = (from i in itemsToSort
					                 where inv[i].damage > 0 && inv[i].ammo == 0 && inv[i].pick == 0 && inv[i].axe == 0 && inv[i].hammer == 0
					                 select i).ToList<int>();
					layer.Validate(ref list, inv);
					foreach (int current in list)
					{
						itemsToSort.Remove(current);
					}
					list.Sort(delegate(int x, int y)
						{
							int num = inv[y].rare.CompareTo(inv[x].rare);
							if (num == 0)
							{
								num = ((x == y) ? 0 : -1);
							}
							return num;
						});
					return list;
				});
			public static ItemSorting.ItemSortingLayer WeaponsAmmo = new ItemSorting.ItemSortingLayer("Weapons - Ammo", delegate(ItemSorting.ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
				{
					List<int> list = (from i in itemsToSort
					                 where inv[i].ammo > 0 && inv[i].damage > 0
					                 select i).ToList<int>();
					layer.Validate(ref list, inv);
					foreach (int current in list)
					{
						itemsToSort.Remove(current);
					}
					list.Sort(delegate(int x, int y)
						{
							int num = inv[y].rare.CompareTo(inv[x].rare);
							if (num == 0)
							{
								num = ((x == y) ? 0 : -1);
							}
							return num;
						});
					return list;
				});
			public static ItemSorting.ItemSortingLayer ToolsPicksaws = new ItemSorting.ItemSortingLayer("Tools - Picksaws", delegate(ItemSorting.ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
				{
					List<int> list = (from i in itemsToSort
					                 where inv[i].pick > 0 && inv[i].axe > 0
					                 select i).ToList<int>();
					layer.Validate(ref list, inv);
					foreach (int current in list)
					{
						itemsToSort.Remove(current);
					}
					list.Sort((int x, int y) => inv[x].pick.CompareTo(inv[y].pick));
					return list;
				});
			public static ItemSorting.ItemSortingLayer ToolsHamaxes = new ItemSorting.ItemSortingLayer("Tools - Hamaxes", delegate(ItemSorting.ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
				{
					List<int> list = (from i in itemsToSort
					                 where inv[i].hammer > 0 && inv[i].axe > 0
					                 select i).ToList<int>();
					layer.Validate(ref list, inv);
					foreach (int current in list)
					{
						itemsToSort.Remove(current);
					}
					list.Sort((int x, int y) => inv[x].axe.CompareTo(inv[y].axe));
					return list;
				});
			public static ItemSorting.ItemSortingLayer ToolsPickaxes = new ItemSorting.ItemSortingLayer("Tools - Pickaxes", delegate(ItemSorting.ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
				{
					List<int> list = (from i in itemsToSort
					                 where inv[i].pick > 0
					                 select i).ToList<int>();
					layer.Validate(ref list, inv);
					foreach (int current in list)
					{
						itemsToSort.Remove(current);
					}
					list.Sort((int x, int y) => inv[x].pick.CompareTo(inv[y].pick));
					return list;
				});
			public static ItemSorting.ItemSortingLayer ToolsAxes = new ItemSorting.ItemSortingLayer("Tools - Axes", delegate(ItemSorting.ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
				{
					List<int> list = (from i in itemsToSort
					                 where inv[i].pick > 0
					                 select i).ToList<int>();
					layer.Validate(ref list, inv);
					foreach (int current in list)
					{
						itemsToSort.Remove(current);
					}
					list.Sort((int x, int y) => inv[x].axe.CompareTo(inv[y].axe));
					return list;
				});
			public static ItemSorting.ItemSortingLayer ToolsHammers = new ItemSorting.ItemSortingLayer("Tools - Hammers", delegate(ItemSorting.ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
				{
					List<int> list = (from i in itemsToSort
					                 where inv[i].hammer > 0
					                 select i).ToList<int>();
					layer.Validate(ref list, inv);
					foreach (int current in list)
					{
						itemsToSort.Remove(current);
					}
					list.Sort((int x, int y) => inv[x].hammer.CompareTo(inv[y].hammer));
					return list;
				});
			public static ItemSorting.ItemSortingLayer ToolsTerraforming = new ItemSorting.ItemSortingLayer("Tools - Terraforming", delegate(ItemSorting.ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
				{
					List<int> list = (from i in itemsToSort
					                 where inv[i].netID > 0 && ItemID.Sets.SortingPriorityTerraforming[inv[i].netID] > -1
					                 select i).ToList<int>();
					layer.Validate(ref list, inv);
					foreach (int current in list)
					{
						itemsToSort.Remove(current);
					}
					list.Sort(delegate(int x, int y)
						{
							int num = ItemID.Sets.SortingPriorityTerraforming[inv[x].netID].CompareTo(ItemID.Sets.SortingPriorityTerraforming[inv[y].netID]);
							if (num == 0)
							{
								num = inv[y].stack.CompareTo(inv[x].stack);
							}
							if (num == 0)
							{
								num = ((x == y) ? 0 : -1);
							}
							return num;
						});
					return list;
				});
			public static ItemSorting.ItemSortingLayer ToolsAmmoLeftovers = new ItemSorting.ItemSortingLayer("Weapons - Ammo Leftovers", delegate(ItemSorting.ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
				{
					List<int> list = (from i in itemsToSort
					                 where inv[i].ammo > 0
					                 select i).ToList<int>();
					layer.Validate(ref list, inv);
					foreach (int current in list)
					{
						itemsToSort.Remove(current);
					}
					list.Sort(delegate(int x, int y)
						{
							int num = inv[y].rare.CompareTo(inv[x].rare);
							if (num == 0)
							{
								num = ((x == y) ? 0 : -1);
							}
							return num;
						});
					return list;
				});
			public static ItemSorting.ItemSortingLayer ArmorCombat = new ItemSorting.ItemSortingLayer("Armor - Combat", delegate(ItemSorting.ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
				{
					List<int> list = (from i in itemsToSort
					                 where (inv[i].bodySlot >= 0 || inv[i].headSlot >= 0 || inv[i].legSlot >= 0) && !inv[i].vanity
					                 select i).ToList<int>();
					layer.Validate(ref list, inv);
					foreach (int current in list)
					{
						itemsToSort.Remove(current);
					}
					list.Sort(delegate(int x, int y)
						{
							int num = inv[y].rare.CompareTo(inv[x].rare);
							if (num == 0)
							{
								num = inv[x].netID.CompareTo(inv[y].netID);
							}
							return num;
						});
					return list;
				});
			public static ItemSorting.ItemSortingLayer ArmorVanity = new ItemSorting.ItemSortingLayer("Armor - Vanity", delegate(ItemSorting.ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
				{
					List<int> list = (from i in itemsToSort
					                 where (inv[i].bodySlot >= 0 || inv[i].headSlot >= 0 || inv[i].legSlot >= 0) && inv[i].vanity
					                 select i).ToList<int>();
					layer.Validate(ref list, inv);
					foreach (int current in list)
					{
						itemsToSort.Remove(current);
					}
					list.Sort(delegate(int x, int y)
						{
							int num = inv[y].rare.CompareTo(inv[x].rare);
							if (num == 0)
							{
								num = inv[x].netID.CompareTo(inv[y].netID);
							}
							return num;
						});
					return list;
				});
			public static ItemSorting.ItemSortingLayer ArmorAccessories = new ItemSorting.ItemSortingLayer("Armor - Accessories", delegate(ItemSorting.ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
				{
					List<int> list = (from i in itemsToSort
					                 where inv[i].accessory
					                 select i).ToList<int>();
					layer.Validate(ref list, inv);
					foreach (int current in list)
					{
						itemsToSort.Remove(current);
					}
					list.Sort(delegate(int x, int y)
						{
							int num = inv[x].vanity.CompareTo(inv[y].vanity);
							if (num == 0)
							{
								num = inv[y].rare.CompareTo(inv[x].rare);
							}
							if (num == 0)
							{
								num = inv[x].netID.CompareTo(inv[y].netID);
							}
							return num;
						});
					return list;
				});
			public static ItemSorting.ItemSortingLayer EquipGrapple = new ItemSorting.ItemSortingLayer("Equip - Grapple", delegate(ItemSorting.ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
				{
					List<int> list = (from i in itemsToSort
					                 where Main.projHook[inv[i].shoot]
					                 select i).ToList<int>();
					layer.Validate(ref list, inv);
					foreach (int current in list)
					{
						itemsToSort.Remove(current);
					}
					list.Sort(delegate(int x, int y)
						{
							int num = inv[y].rare.CompareTo(inv[x].rare);
							if (num == 0)
							{
								num = inv[x].netID.CompareTo(inv[y].netID);
							}
							if (num == 0)
							{
								num = ((x == y) ? 0 : -1);
							}
							return num;
						});
					return list;
				});
			public static ItemSorting.ItemSortingLayer EquipMount = new ItemSorting.ItemSortingLayer("Equip - Mount", delegate(ItemSorting.ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
				{
					List<int> list = (from i in itemsToSort
					                 where inv[i].mountType != -1 && !MountID.Sets.Cart[inv[i].mountType]
					                 select i).ToList<int>();
					layer.Validate(ref list, inv);
					foreach (int current in list)
					{
						itemsToSort.Remove(current);
					}
					list.Sort(delegate(int x, int y)
						{
							int num = inv[y].rare.CompareTo(inv[x].rare);
							if (num == 0)
							{
								num = inv[x].netID.CompareTo(inv[y].netID);
							}
							if (num == 0)
							{
								num = ((x == y) ? 0 : -1);
							}
							return num;
						});
					return list;
				});
			public static ItemSorting.ItemSortingLayer EquipCart = new ItemSorting.ItemSortingLayer("Equip - Cart", delegate(ItemSorting.ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
				{
					List<int> list = (from i in itemsToSort
					                 where inv[i].mountType != -1 && MountID.Sets.Cart[inv[i].mountType]
					                 select i).ToList<int>();
					layer.Validate(ref list, inv);
					foreach (int current in list)
					{
						itemsToSort.Remove(current);
					}
					list.Sort(delegate(int x, int y)
						{
							int num = inv[y].rare.CompareTo(inv[x].rare);
							if (num == 0)
							{
								num = inv[x].netID.CompareTo(inv[y].netID);
							}
							if (num == 0)
							{
								num = ((x == y) ? 0 : -1);
							}
							return num;
						});
					return list;
				});
			public static ItemSorting.ItemSortingLayer EquipLightPet = new ItemSorting.ItemSortingLayer("Equip - Light Pet", delegate(ItemSorting.ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
				{
					List<int> list = (from i in itemsToSort
					                 where inv[i].buffType > 0 && Main.lightPet[inv[i].buffType]
					                 select i).ToList<int>();
					layer.Validate(ref list, inv);
					foreach (int current in list)
					{
						itemsToSort.Remove(current);
					}
					list.Sort(delegate(int x, int y)
						{
							int num = inv[y].rare.CompareTo(inv[x].rare);
							if (num == 0)
							{
								num = inv[x].netID.CompareTo(inv[y].netID);
							}
							if (num == 0)
							{
								num = ((x == y) ? 0 : -1);
							}
							return num;
						});
					return list;
				});
			public static ItemSorting.ItemSortingLayer EquipVanityPet = new ItemSorting.ItemSortingLayer("Equip - Vanity Pet", delegate(ItemSorting.ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
				{
					List<int> list = (from i in itemsToSort
					                 where inv[i].buffType > 0 && Main.vanityPet[inv[i].buffType]
					                 select i).ToList<int>();
					layer.Validate(ref list, inv);
					foreach (int current in list)
					{
						itemsToSort.Remove(current);
					}
					list.Sort(delegate(int x, int y)
						{
							int num = inv[y].rare.CompareTo(inv[x].rare);
							if (num == 0)
							{
								num = inv[x].netID.CompareTo(inv[y].netID);
							}
							if (num == 0)
							{
								num = ((x == y) ? 0 : -1);
							}
							return num;
						});
					return list;
				});
			public static ItemSorting.ItemSortingLayer PotionsLife = new ItemSorting.ItemSortingLayer("Potions - Life", delegate(ItemSorting.ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
				{
					List<int> list = (from i in itemsToSort
					                 where inv[i].consumable && inv[i].healLife > 0 && inv[i].healMana < 1
					                 select i).ToList<int>();
					layer.Validate(ref list, inv);
					foreach (int current in list)
					{
						itemsToSort.Remove(current);
					}
					list.Sort((int x, int y) => inv[y].healLife.CompareTo(inv[x].healLife));
					return list;
				});
			public static ItemSorting.ItemSortingLayer PotionsMana = new ItemSorting.ItemSortingLayer("Potions - Mana", delegate(ItemSorting.ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
				{
					List<int> list = (from i in itemsToSort
					                 where inv[i].consumable && inv[i].healLife < 1 && inv[i].healMana > 0
					                 select i).ToList<int>();
					layer.Validate(ref list, inv);
					foreach (int current in list)
					{
						itemsToSort.Remove(current);
					}
					list.Sort((int x, int y) => inv[y].healMana.CompareTo(inv[x].healMana));
					return list;
				});
			public static ItemSorting.ItemSortingLayer PotionsElixirs = new ItemSorting.ItemSortingLayer("Potions - Elixirs", delegate(ItemSorting.ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
				{
					List<int> list = (from i in itemsToSort
					                 where inv[i].consumable && inv[i].healLife > 0 && inv[i].healMana > 0
					                 select i).ToList<int>();
					layer.Validate(ref list, inv);
					foreach (int current in list)
					{
						itemsToSort.Remove(current);
					}
					list.Sort((int x, int y) => inv[y].healLife.CompareTo(inv[x].healLife));
					return list;
				});
			public static ItemSorting.ItemSortingLayer PotionsBuffs = new ItemSorting.ItemSortingLayer("Potions - Buffs", delegate(ItemSorting.ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
				{
					List<int> list = (from i in itemsToSort
					                 where inv[i].consumable && inv[i].buffType > 0
					                 select i).ToList<int>();
					layer.Validate(ref list, inv);
					foreach (int current in list)
					{
						itemsToSort.Remove(current);
					}
					list.Sort(delegate(int x, int y)
						{
							int num = inv[y].rare.CompareTo(inv[x].rare);
							if (num == 0)
							{
								num = inv[x].netID.CompareTo(inv[y].netID);
							}
							if (num == 0)
							{
								num = inv[y].stack.CompareTo(inv[x].stack);
							}
							if (num == 0)
							{
								num = ((x == y) ? 0 : -1);
							}
							return num;
						});
					return list;
				});
			public static ItemSorting.ItemSortingLayer PotionsDyes = new ItemSorting.ItemSortingLayer("Potions - Dyes", delegate(ItemSorting.ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
				{
					List<int> list = (from i in itemsToSort
					                 where inv[i].dye > 0
					                 select i).ToList<int>();
					layer.Validate(ref list, inv);
					foreach (int current in list)
					{
						itemsToSort.Remove(current);
					}
					list.Sort(delegate(int x, int y)
						{
							int num = inv[y].rare.CompareTo(inv[x].rare);
							if (num == 0)
							{
								num = inv[y].dye.CompareTo(inv[x].dye);
							}
							if (num == 0)
							{
								num = inv[y].stack.CompareTo(inv[x].stack);
							}
							if (num == 0)
							{
								num = ((x == y) ? 0 : -1);
							}
							return num;
						});
					return list;
				});
			public static ItemSorting.ItemSortingLayer PotionsHairDyes = new ItemSorting.ItemSortingLayer("Potions - Hair Dyes", delegate(ItemSorting.ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
				{
					List<int> list = (from i in itemsToSort
					                 where inv[i].hairDye >= 0
					                 select i).ToList<int>();
					layer.Validate(ref list, inv);
					foreach (int current in list)
					{
						itemsToSort.Remove(current);
					}
					list.Sort(delegate(int x, int y)
						{
							int num = inv[y].rare.CompareTo(inv[x].rare);
							if (num == 0)
							{
								num = inv[y].hairDye.CompareTo(inv[x].hairDye);
							}
							if (num == 0)
							{
								num = inv[y].stack.CompareTo(inv[x].stack);
							}
							if (num == 0)
							{
								num = ((x == y) ? 0 : -1);
							}
							return num;
						});
					return list;
				});
			public static ItemSorting.ItemSortingLayer MiscValuables = new ItemSorting.ItemSortingLayer("Misc - Importants", delegate(ItemSorting.ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
				{
					List<int> list = (from i in itemsToSort
					                 where inv[i].netID > 0 && ItemID.Sets.SortingPriorityBossSpawns[inv[i].netID] > -1
					                 select i).ToList<int>();
					layer.Validate(ref list, inv);
					foreach (int current in list)
					{
						itemsToSort.Remove(current);
					}
					list.Sort(delegate(int x, int y)
						{
							int num = ItemID.Sets.SortingPriorityBossSpawns[inv[x].netID].CompareTo(ItemID.Sets.SortingPriorityBossSpawns[inv[y].netID]);
							if (num == 0)
							{
								num = inv[y].stack.CompareTo(inv[x].stack);
							}
							if (num == 0)
							{
								num = ((x == y) ? 0 : -1);
							}
							return num;
						});
					return list;
				});
			public static ItemSorting.ItemSortingLayer MiscWiring = new ItemSorting.ItemSortingLayer("Misc - Wiring", delegate(ItemSorting.ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
				{
					List<int> list = (from i in itemsToSort
					                 where (inv[i].netID > 0 && ItemID.Sets.SortingPriorityWiring[inv[i].netID] > -1) || inv[i].mech
					                 select i).ToList<int>();
					layer.Validate(ref list, inv);
					foreach (int current in list)
					{
						itemsToSort.Remove(current);
					}
					list.Sort(delegate(int x, int y)
						{
							int num = ItemID.Sets.SortingPriorityWiring[inv[y].netID].CompareTo(ItemID.Sets.SortingPriorityWiring[inv[x].netID]);
							if (num == 0)
							{
								num = inv[y].rare.CompareTo(inv[x].rare);
							}
							if (num == 0)
							{
								num = inv[y].netID.CompareTo(inv[x].netID);
							}
							if (num == 0)
							{
								num = inv[y].stack.CompareTo(inv[x].stack);
							}
							if (num == 0)
							{
								num = ((x == y) ? 0 : -1);
							}
							return num;
						});
					return list;
				});
			public static ItemSorting.ItemSortingLayer MiscMaterials = new ItemSorting.ItemSortingLayer("Misc - Materials", delegate(ItemSorting.ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
				{
					List<int> list = (from i in itemsToSort
					                 where inv[i].netID > 0 && ItemID.Sets.SortingPriorityMaterials[inv[i].netID] > -1
					                 select i).ToList<int>();
					layer.Validate(ref list, inv);
					foreach (int current in list)
					{
						itemsToSort.Remove(current);
					}
					list.Sort((int x, int y) => ItemID.Sets.SortingPriorityMaterials[inv[y].netID].CompareTo(ItemID.Sets.SortingPriorityMaterials[inv[x].netID]));
					return list;
				});
			public static ItemSorting.ItemSortingLayer MiscExtractinator = new ItemSorting.ItemSortingLayer("Misc - Extractinator", delegate(ItemSorting.ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
				{
					List<int> list = (from i in itemsToSort
					                 where inv[i].netID > 0 && ItemID.Sets.SortingPriorityExtractibles[inv[i].netID] > -1
					                 select i).ToList<int>();
					layer.Validate(ref list, inv);
					foreach (int current in list)
					{
						itemsToSort.Remove(current);
					}
					list.Sort((int x, int y) => ItemID.Sets.SortingPriorityExtractibles[inv[y].netID].CompareTo(ItemID.Sets.SortingPriorityExtractibles[inv[x].netID]));
					return list;
				});
			public static ItemSorting.ItemSortingLayer MiscPainting = new ItemSorting.ItemSortingLayer("Misc - Painting", delegate(ItemSorting.ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
				{
					List<int> list = (from i in itemsToSort
					                 where (inv[i].netID > 0 && ItemID.Sets.SortingPriorityPainting[inv[i].netID] > -1) || inv[i].paint > 0
					                 select i).ToList<int>();
					layer.Validate(ref list, inv);
					foreach (int current in list)
					{
						itemsToSort.Remove(current);
					}
					list.Sort(delegate(int x, int y)
						{
							int num = ItemID.Sets.SortingPriorityPainting[inv[y].netID].CompareTo(ItemID.Sets.SortingPriorityPainting[inv[x].netID]);
							if (num == 0)
							{
								num = inv[x].paint.CompareTo(inv[y].paint);
							}
							if (num == 0)
							{
								num = inv[y].stack.CompareTo(inv[x].stack);
							}
							if (num == 0)
							{
								num = ((x == y) ? 0 : -1);
							}
							return num;
						});
					return list;
				});
			public static ItemSorting.ItemSortingLayer MiscRopes = new ItemSorting.ItemSortingLayer("Misc - Ropes", delegate(ItemSorting.ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
				{
					List<int> list = (from i in itemsToSort
					                 where inv[i].netID > 0 && ItemID.Sets.SortingPriorityRopes[inv[i].netID] > -1
					                 select i).ToList<int>();
					layer.Validate(ref list, inv);
					foreach (int current in list)
					{
						itemsToSort.Remove(current);
					}
					list.Sort((int x, int y) => ItemID.Sets.SortingPriorityRopes[inv[y].netID].CompareTo(ItemID.Sets.SortingPriorityRopes[inv[x].netID]));
					return list;
				});
			public static ItemSorting.ItemSortingLayer LastMaterials = new ItemSorting.ItemSortingLayer("Last - Materials", delegate(ItemSorting.ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
				{
					List<int> list = (from i in itemsToSort
					                 where inv[i].createTile < 0 && inv[i].createWall < 1
					                 select i).ToList<int>();
					layer.Validate(ref list, inv);
					foreach (int current in list)
					{
						itemsToSort.Remove(current);
					}
					list.Sort(delegate(int x, int y)
						{
							int num = inv[y].rare.CompareTo(inv[x].rare);
							if (num == 0)
							{
								num = inv[y].value.CompareTo(inv[x].value);
							}
							if (num == 0)
							{
								num = inv[y].stack.CompareTo(inv[x].stack);
							}
							if (num == 0)
							{
								num = ((x == y) ? 0 : -1);
							}
							return num;
						});
					return list;
				});
			public static ItemSorting.ItemSortingLayer LastTilesImportant = new ItemSorting.ItemSortingLayer("Last - Tiles (Frame Important)", delegate(ItemSorting.ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
				{
					List<int> list = (from i in itemsToSort
					                 where inv[i].createTile >= 0 && Main.tileFrameImportant[inv[i].createTile]
					                 select i).ToList<int>();
					layer.Validate(ref list, inv);
					foreach (int current in list)
					{
						itemsToSort.Remove(current);
					}
					list.Sort(delegate(int x, int y)
						{
							int num = string.Compare(inv[x].name, inv[y].name, StringComparison.OrdinalIgnoreCase);
							if (num == 0)
							{
								num = inv[y].stack.CompareTo(inv[x].stack);
							}
							if (num == 0)
							{
								num = ((x == y) ? 0 : -1);
							}
							return num;
						});
					return list;
				});
			public static ItemSorting.ItemSortingLayer LastTilesCommon = new ItemSorting.ItemSortingLayer("Last - Tiles (Common), Walls", delegate(ItemSorting.ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
				{
					List<int> list = (from i in itemsToSort
					                 where inv[i].createWall > 0 || inv[i].createTile >= 0
					                 select i).ToList<int>();
					layer.Validate(ref list, inv);
					foreach (int current in list)
					{
						itemsToSort.Remove(current);
					}
					list.Sort(delegate(int x, int y)
						{
							int num = string.Compare(inv[x].name, inv[y].name, StringComparison.OrdinalIgnoreCase);
							if (num == 0)
							{
								num = inv[y].stack.CompareTo(inv[x].stack);
							}
							if (num == 0)
							{
								num = ((x == y) ? 0 : -1);
							}
							return num;
						});
					return list;
				});
			public static ItemSorting.ItemSortingLayer LastNotTrash = new ItemSorting.ItemSortingLayer("Last - Not Trash", delegate(ItemSorting.ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
				{
					List<int> list = (from i in itemsToSort
					                 where inv[i].rare >= 0
					                 select i).ToList<int>();
					layer.Validate(ref list, inv);
					foreach (int current in list)
					{
						itemsToSort.Remove(current);
					}
					list.Sort(delegate(int x, int y)
						{
							int num = inv[y].rare.CompareTo(inv[x].rare);
							if (num == 0)
							{
								num = string.Compare(inv[x].name, inv[y].name, StringComparison.OrdinalIgnoreCase);
							}
							if (num == 0)
							{
								num = inv[y].stack.CompareTo(inv[x].stack);
							}
							if (num == 0)
							{
								num = ((x == y) ? 0 : -1);
							}
							return num;
						});
					return list;
				});
			public static ItemSorting.ItemSortingLayer LastTrash = new ItemSorting.ItemSortingLayer("Last - Trash", delegate(ItemSorting.ItemSortingLayer layer, Item[] inv, List<int> itemsToSort)
				{
					List<int> list = new List<int>(itemsToSort);
					layer.Validate(ref list, inv);
					foreach (int current in list)
					{
						itemsToSort.Remove(current);
					}
					list.Sort(delegate(int x, int y)
						{
							int num = inv[y].value.CompareTo(inv[x].value);
							if (num == 0)
							{
								num = inv[y].stack.CompareTo(inv[x].stack);
							}
							if (num == 0)
							{
								num = ((x == y) ? 0 : -1);
							}
							return num;
						});
					return list;
				});
		}

		private static List<ItemSorting.ItemSortingLayer> _layerList = new List<ItemSorting.ItemSortingLayer>();
		private static Dictionary<string, List<int>> _layerWhiteLists = new Dictionary<string, List<int>>();

		public static void SetupWhiteLists()
		{
			ItemSorting._layerWhiteLists.Clear();
			List<ItemSorting.ItemSortingLayer> list = new List<ItemSorting.ItemSortingLayer>();
			List<Item> list2 = new List<Item>();
			List<int> list3 = new List<int>();
			list.Add(ItemSorting.ItemSortingLayers.WeaponsMelee);
			list.Add(ItemSorting.ItemSortingLayers.WeaponsRanged);
			list.Add(ItemSorting.ItemSortingLayers.WeaponsMagic);
			list.Add(ItemSorting.ItemSortingLayers.WeaponsMinions);
			list.Add(ItemSorting.ItemSortingLayers.WeaponsThrown);
			list.Add(ItemSorting.ItemSortingLayers.WeaponsAssorted);
			list.Add(ItemSorting.ItemSortingLayers.WeaponsAmmo);
			list.Add(ItemSorting.ItemSortingLayers.ToolsPicksaws);
			list.Add(ItemSorting.ItemSortingLayers.ToolsHamaxes);
			list.Add(ItemSorting.ItemSortingLayers.ToolsPickaxes);
			list.Add(ItemSorting.ItemSortingLayers.ToolsAxes);
			list.Add(ItemSorting.ItemSortingLayers.ToolsHammers);
			list.Add(ItemSorting.ItemSortingLayers.ToolsTerraforming);
			list.Add(ItemSorting.ItemSortingLayers.ToolsAmmoLeftovers);
			list.Add(ItemSorting.ItemSortingLayers.ArmorCombat);
			list.Add(ItemSorting.ItemSortingLayers.ArmorVanity);
			list.Add(ItemSorting.ItemSortingLayers.ArmorAccessories);
			list.Add(ItemSorting.ItemSortingLayers.EquipGrapple);
			list.Add(ItemSorting.ItemSortingLayers.EquipMount);
			list.Add(ItemSorting.ItemSortingLayers.EquipCart);
			list.Add(ItemSorting.ItemSortingLayers.EquipLightPet);
			list.Add(ItemSorting.ItemSortingLayers.EquipVanityPet);
			list.Add(ItemSorting.ItemSortingLayers.PotionsDyes);
			list.Add(ItemSorting.ItemSortingLayers.PotionsHairDyes);
			list.Add(ItemSorting.ItemSortingLayers.PotionsLife);
			list.Add(ItemSorting.ItemSortingLayers.PotionsMana);
			list.Add(ItemSorting.ItemSortingLayers.PotionsElixirs);
			list.Add(ItemSorting.ItemSortingLayers.PotionsBuffs);
			list.Add(ItemSorting.ItemSortingLayers.MiscValuables);
			list.Add(ItemSorting.ItemSortingLayers.MiscPainting);
			list.Add(ItemSorting.ItemSortingLayers.MiscWiring);
			list.Add(ItemSorting.ItemSortingLayers.MiscMaterials);
			list.Add(ItemSorting.ItemSortingLayers.MiscRopes);
			list.Add(ItemSorting.ItemSortingLayers.MiscExtractinator);
			list.Add(ItemSorting.ItemSortingLayers.LastMaterials);
			list.Add(ItemSorting.ItemSortingLayers.LastTilesImportant);
			list.Add(ItemSorting.ItemSortingLayers.LastTilesCommon);
			list.Add(ItemSorting.ItemSortingLayers.LastNotTrash);
			list.Add(ItemSorting.ItemSortingLayers.LastTrash);
			for (int i = -48; i < ItemLoader.ItemCount; i++)
			{
				Item item = new Item();
				item.netDefaults(i);
				list2.Add(item);
				list3.Add(i + 48);
			}
			Item[] array = list2.ToArray();
			foreach (ItemSorting.ItemSortingLayer current in list)
			{
				List<int> list4 = current.SortingMethod(current, array, list3);
				List<int> list5 = new List<int>();
				for (int j = 0; j < list4.Count; j++)
				{
					list5.Add(array[list4[j]].netID);
				}
				ItemSorting._layerWhiteLists.Add(current.Name, list5);
			}
		}

		private static void SetupSortingPriorities()
		{
			Player player = Main.player[Main.myPlayer];
			ItemSorting._layerList.Clear();
			List<float> list = new List<float>
			{
				player.meleeDamage,
				player.rangedDamage,
				player.magicDamage,
				player.minionDamage,
				player.thrownDamage
			};
			list.Sort((float x, float y) => y.CompareTo(x));
			for (int i = 0; i < 5; i++)
			{
				if (!ItemSorting._layerList.Contains(ItemSorting.ItemSortingLayers.WeaponsMelee) && player.meleeDamage == list[0])
				{
					list.RemoveAt(0);
					ItemSorting._layerList.Add(ItemSorting.ItemSortingLayers.WeaponsMelee);
				}
				if (!ItemSorting._layerList.Contains(ItemSorting.ItemSortingLayers.WeaponsRanged) && player.rangedDamage == list[0])
				{
					list.RemoveAt(0);
					ItemSorting._layerList.Add(ItemSorting.ItemSortingLayers.WeaponsRanged);
				}
				if (!ItemSorting._layerList.Contains(ItemSorting.ItemSortingLayers.WeaponsMagic) && player.magicDamage == list[0])
				{
					list.RemoveAt(0);
					ItemSorting._layerList.Add(ItemSorting.ItemSortingLayers.WeaponsMagic);
				}
				if (!ItemSorting._layerList.Contains(ItemSorting.ItemSortingLayers.WeaponsMinions) && player.minionDamage == list[0])
				{
					list.RemoveAt(0);
					ItemSorting._layerList.Add(ItemSorting.ItemSortingLayers.WeaponsMinions);
				}
				if (!ItemSorting._layerList.Contains(ItemSorting.ItemSortingLayers.WeaponsThrown) && player.thrownDamage == list[0])
				{
					list.RemoveAt(0);
					ItemSorting._layerList.Add(ItemSorting.ItemSortingLayers.WeaponsThrown);
				}
			}
			ItemSorting._layerList.Add(ItemSorting.ItemSortingLayers.WeaponsAssorted);
			ItemSorting._layerList.Add(ItemSorting.ItemSortingLayers.WeaponsAmmo);
			ItemSorting._layerList.Add(ItemSorting.ItemSortingLayers.ToolsPicksaws);
			ItemSorting._layerList.Add(ItemSorting.ItemSortingLayers.ToolsHamaxes);
			ItemSorting._layerList.Add(ItemSorting.ItemSortingLayers.ToolsPickaxes);
			ItemSorting._layerList.Add(ItemSorting.ItemSortingLayers.ToolsAxes);
			ItemSorting._layerList.Add(ItemSorting.ItemSortingLayers.ToolsHammers);
			ItemSorting._layerList.Add(ItemSorting.ItemSortingLayers.ToolsTerraforming);
			ItemSorting._layerList.Add(ItemSorting.ItemSortingLayers.ToolsAmmoLeftovers);
			ItemSorting._layerList.Add(ItemSorting.ItemSortingLayers.ArmorCombat);
			ItemSorting._layerList.Add(ItemSorting.ItemSortingLayers.ArmorVanity);
			ItemSorting._layerList.Add(ItemSorting.ItemSortingLayers.ArmorAccessories);
			ItemSorting._layerList.Add(ItemSorting.ItemSortingLayers.EquipGrapple);
			ItemSorting._layerList.Add(ItemSorting.ItemSortingLayers.EquipMount);
			ItemSorting._layerList.Add(ItemSorting.ItemSortingLayers.EquipCart);
			ItemSorting._layerList.Add(ItemSorting.ItemSortingLayers.EquipLightPet);
			ItemSorting._layerList.Add(ItemSorting.ItemSortingLayers.EquipVanityPet);
			ItemSorting._layerList.Add(ItemSorting.ItemSortingLayers.PotionsDyes);
			ItemSorting._layerList.Add(ItemSorting.ItemSortingLayers.PotionsHairDyes);
			ItemSorting._layerList.Add(ItemSorting.ItemSortingLayers.PotionsLife);
			ItemSorting._layerList.Add(ItemSorting.ItemSortingLayers.PotionsMana);
			ItemSorting._layerList.Add(ItemSorting.ItemSortingLayers.PotionsElixirs);
			ItemSorting._layerList.Add(ItemSorting.ItemSortingLayers.PotionsBuffs);
			ItemSorting._layerList.Add(ItemSorting.ItemSortingLayers.MiscValuables);
			ItemSorting._layerList.Add(ItemSorting.ItemSortingLayers.MiscPainting);
			ItemSorting._layerList.Add(ItemSorting.ItemSortingLayers.MiscWiring);
			ItemSorting._layerList.Add(ItemSorting.ItemSortingLayers.MiscMaterials);
			ItemSorting._layerList.Add(ItemSorting.ItemSortingLayers.MiscRopes);
			ItemSorting._layerList.Add(ItemSorting.ItemSortingLayers.MiscExtractinator);
			ItemSorting._layerList.Add(ItemSorting.ItemSortingLayers.LastMaterials);
			ItemSorting._layerList.Add(ItemSorting.ItemSortingLayers.LastTilesImportant);
			ItemSorting._layerList.Add(ItemSorting.ItemSortingLayers.LastTilesCommon);
			ItemSorting._layerList.Add(ItemSorting.ItemSortingLayers.LastNotTrash);
			ItemSorting._layerList.Add(ItemSorting.ItemSortingLayers.LastTrash);
		}

		private static void Sort(Item[] inv, params int[] ignoreSlots)
		{
			ItemSorting.SetupSortingPriorities();
			List<int> list = new List<int>();
			for (int i = 0; i < inv.Length; i++)
			{
				if (!ignoreSlots.Contains(i))
				{
					Item item = inv[i];
					if (item != null && item.stack != 0 && item.type != 0 && !item.favorited)
					{
						list.Add(i);
					}
				}
			}
			for (int j = 0; j < list.Count; j++)
			{
				Item item2 = inv[list[j]];
				if (item2.stack < item2.maxStack)
				{
					int num = item2.maxStack - item2.stack;
					for (int k = j; k < list.Count; k++)
					{
						if (j != k)
						{
							Item item3 = inv[list[k]];
							if (item2.type == item3.type && item3.stack != item3.maxStack)
							{
								int num2 = item3.stack;
								if (num < num2)
								{
									num2 = num;
								}
								item2.stack += num2;
								item3.stack -= num2;
								num -= num2;
								if (item3.stack == 0)
								{
									inv[list[k]] = new Item();
									list.Remove(list[k]);
									j--;
									k--;
									break;
								}
								if (num == 0)
								{
									break;
								}
							}
						}
					}
				}
			}
			List<int> list2 = new List<int>(list);
			for (int l = 0; l < inv.Length; l++)
			{
				if (!ignoreSlots.Contains(l) && !list2.Contains(l))
				{
					Item item4 = inv[l];
					if (item4 == null || item4.stack == 0 || item4.type == 0)
					{
						list2.Add(l);
					}
				}
			}
			list2.Sort();
			List<int> list3 = new List<int>();
			List<int> list4 = new List<int>();
			foreach (ItemSorting.ItemSortingLayer current in ItemSorting._layerList)
			{
				List<int> list5 = current.SortingMethod(current, inv, list);
				if (list5.Count > 0)
				{
					list4.Add(list5.Count);
				}
				list3.AddRange(list5);
			}
			list3.AddRange(list);
			List<Item> list6 = new List<Item>();
			foreach (int current2 in list3)
			{
				list6.Add(inv[current2]);
				inv[current2] = new Item();
			}
			float num3 = 1f / (float)list4.Count;
			float num4 = num3 / 2f;
			for (int m = 0; m < list6.Count; m++)
			{
				int num5 = list2[0];
				ItemSlot.SetGlow(num5, num4, Main.player[Main.myPlayer].chest != -1);
				List<int> list7;
				(list7 = list4)[0] = list7[0] - 1;
				if (list4[0] == 0)
				{
					list4.RemoveAt(0);
					num4 += num3;
				}
				inv[num5] = list6[m];
				list2.Remove(num5);
			}
		}

		public static void SortInventory()
		{
			ItemSorting.Sort(Main.player[Main.myPlayer].inventory, new int[]
				{
					0,
					1,
					2,
					3,
					4,
					5,
					6,
					7,
					8,
					9,
					50,
					51,
					52,
					53,
					54,
					55,
					56,
					57,
					58
				});
		}

		public static void SortChest()
		{
			int chest = Main.player[Main.myPlayer].chest;
			if (chest == -1)
			{
				return;
			}
			Item[] item = Main.player[Main.myPlayer].bank.item;
			if (chest == -3)
			{
				item = Main.player[Main.myPlayer].bank2.item;
			}
			if (chest == -4)
			{
				item = Main.player[Main.myPlayer].bank3.item;
			}
			if (chest > -1)
			{
				item = Main.chest[chest].item;
			}
			Tuple<int, int, int>[] array = new Tuple<int, int, int>[40];
			for (int i = 0; i < 40; i++)
			{
				array[i] = Tuple.Create<int, int, int>(item[i].netID, item[i].stack, (int)item[i].prefix);
			}
			ItemSorting.Sort(item, new int[0]);
			Tuple<int, int, int>[] array2 = new Tuple<int, int, int>[40];
			for (int j = 0; j < 40; j++)
			{
				array2[j] = Tuple.Create<int, int, int>(item[j].netID, item[j].stack, (int)item[j].prefix);
			}
			if (Main.netMode == 1 && Main.player[Main.myPlayer].chest > -1)
			{
				for (int k = 0; k < 40; k++)
				{
					if (array2[k] != array[k])
					{
						NetMessage.SendData(32, -1, -1, "", Main.player[Main.myPlayer].chest, (float)k, 0f, 0f, 0, 0, 0);
					}
				}
			}
		}
	}
}
