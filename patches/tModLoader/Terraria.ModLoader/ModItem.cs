using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;

namespace Terraria.ModLoader
{
	/// <summary>
	/// This class serves as a place for you to place all your properties and hooks for each item. Create instances of ModItem (preferably overriding this class) to pass as parameters to Mod.AddItem.
	/// </summary>
	public class ModItem
	{
		//add modItem property to Terraria.Item (internal set)
		//set modItem to null at beginning of Terraria.Item.ResetStats		
		/// <summary>
		/// The item object that this ModItem controls.
		/// </summary>
		/// <value>
		/// The item.
		/// </value>
		public Item item
		{
			get;
			internal set;
		}

		/// <summary>
		/// Gets the mod.
		/// </summary>
		/// <value>
		/// The mod that added this ModItem.
		/// </value>
		public Mod mod
		{
			get;
			internal set;
		}

		internal string texture;
		internal string flameTexture = "";
		/// <summary>
		/// Setting this to true makes it so that this weapon can shoot projectiles only at the beginning of its animation. Set this to true if you want a sword and its projectile creation to be in sync (for example, the Terra Blade). Defaults to false.
		/// </summary>
		public bool projOnSwing = false;
		/// <summary>
		/// The type of NPC that drops this boss bag. Used to determine how many coins this boss bag contains. Defaults to 0, which means this isn't a boss bag.
		/// </summary>
		public int bossBagNPC;

		public ModItem()
		{
			item = new Item {modItem = this};
		}

		/// <summary>
		/// Adds a line of text to this item's first group of tooltips.
		/// </summary>
		/// <param name="tooltip">The tooltip.</param>
		public void AddTooltip(string tooltip)
		{
			if (string.IsNullOrEmpty(item.toolTip))
			{
				item.toolTip = tooltip;
			}
			else
			{
				item.toolTip += Environment.NewLine + tooltip;
			}
		}

		/// <summary>
		/// Adds a line of text to this item's second group of tooltips.
		/// </summary>
		/// <param name="tooltip">The tooltip.</param>
		public void AddTooltip2(string tooltip)
		{
			if (string.IsNullOrEmpty(item.toolTip2))
			{
				item.toolTip2 = tooltip;
			}
			else
			{
				item.toolTip2 += Environment.NewLine + tooltip;
			}
		}

		/// <summary>
		/// Allows you to automatically load an item instead of using Mod.AddItem. Return true to allow autoloading; by default returns the mod's autoload property. Name is initialized to the overriding class name, texture is initialized to the namespace and overriding class name with periods replaced with slashes, and equip is initialized to an empty list. Use this method to either force or stop an autoload, change the default display name and texture path, and to allow for autoloading equipment textures.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="texture">The texture.</param>
		/// <param name="equips">The equips.</param>
		/// <returns></returns>
		public virtual bool Autoload(ref string name, ref string texture, IList<EquipType> equips)
		{
			return mod.Properties.Autoload;
		}

		/// <summary>
		/// This method is called when Autoload adds an equipment type. This allows you to specify equipment texture paths that differ from the defaults. Texture will be initialized to the item texture followed by an underscore and equip.ToString(), armTexture will be initialized to the item texture followed by "_Arms", and femaleTexture will be initialized to the item texture followed by "_FemaleBody".
		/// </summary>
		/// <param name="equip">The equip.</param>
		/// <param name="texture">The texture.</param>
		/// <param name="armTexture">The arm texture.</param>
		/// <param name="femaleTexture">The female texture.</param>
		public virtual void AutoloadEquip(EquipType equip, ref string texture, ref string armTexture, ref string femaleTexture)
		{
		}

		/// <summary>
		/// Allows you to specify a texture path to the flame texture for this item when it is autoloaded. The flame texture is used when the player is holding this item and its "flame" field is set to true. At the moment torches are the only use of flame textures. By default the parameter will be set to the item texture followed by "_Flame". If the texture does not exist, then this item will not be given a flame texture.
		/// </summary>
		/// <param name="texture">The texture.</param>
		public virtual void AutoloadFlame(ref string texture)
		{
		}

		/// <summary>
		/// Override this method to create an animation for your item. In general you will return a new Terraria.DataStructures.DrawAnimationVertical(int frameDuration, int frameCount).
		/// </summary>
		/// <returns></returns>
		public virtual DrawAnimation GetAnimation()
		{
			return null;
		}

		/// <summary>
		/// This is where you set all your item's properties, such as width, damage, shootSpeed, defense, etc. For those that are familiar with tAPI, this has the same function as .json files.
		/// </summary>
		public virtual void SetDefaults()
		{
		}

		/// <summary>
		/// Returns whether or not this item can be used. By default returns true.
		/// </summary>
		/// <param name="player">The player using the item.</param>
		public virtual bool CanUseItem(Player player)
		{
			return true;
		}

		/// <summary>
		/// Allows you to modify the location and rotation of this item in its use animation.
		/// </summary>
		/// <param name="player">The player.</param>
		public virtual void UseStyle(Player player)
		{
		}

		/// <summary>
		/// Allows you to modify the location and rotation of this item when the player is holding it.
		/// </summary>
		/// <param name="player">The player.</param>
		public virtual void HoldStyle(Player player)
		{
		}

		/// <summary>
		/// Allows you to make things happen when the player is holding this item (for example, torches make light and water candles increase spawn rate).
		/// </summary>
		/// <param name="player">The player.</param>
		public virtual void HoldItem(Player player)
		{
		}

		/// <summary>
		/// Allows you to change the effective useTime of this item.
		/// </summary>
		/// <param name="player"></param>
		/// <returns>The multiplier on the usage speed. 1f by default.</returns>
		public virtual float UseTimeMultiplier(Player player)
		{
			return 1f;
		}

		/// <summary>
		/// Allows you to change the effective useAnimation of this item.
		/// </summary>
		/// <param name="player"></param>
		/// <returns>The multiplier on the animation speed. 1f by default.</returns>
		public virtual float MeleeSpeedMultiplier(Player player)
		{
			return 1f;
		}

		/// <summary>
		/// Allows you to temporarily modify this weapon's damage based on player buffs, etc. This is useful for creating new classes of damage, or for making subclasses of damage (for example, Shroomite armor set boosts).
		/// </summary>
		/// <param name="player">The player.</param>
		/// <param name="damage">The damage.</param>
		public virtual void GetWeaponDamage(Player player, ref int damage)
		{
		}

		/// <summary>
		/// Allows you to temporarily modify this weapon's knockback based on player buffs, etc. This allows you to customize knockback beyond the Player class's limited fields.
		/// </summary>
		/// <param name="player">The player.</param>
		/// <param name="knockback">The knockback.</param>
		public virtual void GetWeaponKnockback(Player player, ref float knockback)
		{
		}

		/// <summary>
		/// Allows you to modify the projectile created by a weapon based on the ammo it is using. This hook is called on the ammo.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="type"></param>
		/// <param name="speed"></param>
		/// <param name="damage"></param>
		/// <param name="knockback"></param>
		public virtual void PickAmmo(Player player, ref int type, ref float speed, ref int damage, ref float knockback)
		{
		}

		/// <summary>
		/// Whether or not ammo will be consumed upon usage. Called both by the gun and by the ammo; if at least one returns false then the ammo will not be used. By default returns true.
		/// </summary>
		/// <param name="player">The player.</param>
		/// <returns></returns>
		public virtual bool ConsumeAmmo(Player player)
		{
			return true;
		}

		/// <summary>
		/// This is called before the weapon creates a projectile. You can use it to create special effects, such as changing the speed, changing the initial position, and/or firing multiple projectiles. Return false to stop the game from shooting the default projectile (do this if you manually spawn your own projectile). Returns true by default.
		/// </summary>
		/// <param name="player">The player.</param>
		/// <param name="position">The shoot spawn position.</param>
		/// <param name="speedX">The speed x calculated from shootSpeed and mouse position.</param>
		/// <param name="speedY">The speed y calculated from shootSpeed and mouse position.</param>
		/// <param name="type">The projectile type choosen by ammo and weapon.</param>
		/// <param name="damage">The projectile damage.</param>
		/// <param name="knockBack">The projectile knock back.</param>
		/// <returns></returns>
		public virtual bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			return true;
		}

		/// <summary>
		/// Changes the hitbox of this melee weapon when it is used.
		/// </summary>
		/// <param name="player">The player.</param>
		/// <param name="hitbox">The hitbox.</param>
		/// <param name="noHitbox">if set to <c>true</c> [no hitbox].</param>
		public virtual void UseItemHitbox(Player player, ref Rectangle hitbox, ref bool noHitbox)
		{
		}

		/// <summary>
		/// Allows you to give this melee weapon special effects, such as creating light or dust.
		/// </summary>
		/// <param name="player">The player.</param>
		/// <param name="hitbox">The hitbox.</param>
		public virtual void MeleeEffects(Player player, Rectangle hitbox)
		{
		}

		/// <summary>
		/// Allows you to determine whether this melee weapon can hit the given NPC when swung. Return true to allow hitting the target, return false to block this weapon from hitting the target, and return null to use the vanilla code for whether the target can be hit. Returns null by default.
		/// </summary>
		/// <param name="player">The player.</param>
		/// <param name="target">The target.</param>
		/// <returns></returns>
		public virtual bool? CanHitNPC(Player player, NPC target)
		{
			return null;
		}

		/// <summary>
		/// Allows you to modify the damage, knockback, etc., that this melee weapon does to an NPC.
		/// </summary>
		/// <param name="player">The player.</param>
		/// <param name="target">The target.</param>
		/// <param name="damage">The damage.</param>
		/// <param name="knockBack">The knock back.</param>
		/// <param name="crit">if set to <c>true</c> [crit].</param>
		public virtual void ModifyHitNPC(Player player, NPC target, ref int damage, ref float knockBack, ref bool crit)
		{
		}

		/// <summary>
		/// Allows you to create special effects when this melee weapon hits an NPC (for example how the Pumpkin Sword creates pumpkin heads).
		/// </summary>
		/// <param name="player">The player.</param>
		/// <param name="target">The target.</param>
		/// <param name="damage">The damage.</param>
		/// <param name="knockBack">The knock back.</param>
		/// <param name="crit">if set to <c>true</c> [crit].</param>
		public virtual void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
		{
		}

		/// <summary>
		/// Allows you to determine whether this melee weapon can hit the given opponent player when swung. Return false to block this weapon from hitting the target. Returns true by default.
		/// </summary>
		/// <param name="player">The player.</param>
		/// <param name="target">The target.</param>
		/// <returns>
		///   <c>true</c> if this instance [can hit PVP] the specified player; otherwise, <c>false</c>.
		/// </returns>
		public virtual bool CanHitPvp(Player player, Player target)
		{
			return true;
		}

		/// <summary>
		/// Allows you to modify the damage, etc., that this melee weapon does to a player.
		/// </summary>
		/// <param name="player">The player.</param>
		/// <param name="target">The target.</param>
		/// <param name="damage">The damage.</param>
		/// <param name="crit">if set to <c>true</c> [crit].</param>
		public virtual void ModifyHitPvp(Player player, Player target, ref int damage, ref bool crit)
		{
		}

		/// <summary>
		/// Allows you to create special effects when this melee weapon hits a player.
		/// </summary>
		/// <param name="player">The player.</param>
		/// <param name="target">The target.</param>
		/// <param name="damage">The damage.</param>
		/// <param name="crit">if set to <c>true</c> [crit].</param>
		public virtual void OnHitPvp(Player player, Player target, int damage, bool crit)
		{
		}

		/// <summary>
		/// Allows you to make things happen when this item is used. Return true if using this item actually does stuff. Returns false by default.
		/// </summary>
		/// <param name="player">The player.</param>
		/// <returns></returns>
		public virtual bool UseItem(Player player)
		{
			return false;
		}

		/// <summary>
		/// If this item is consumable and this returns true, then this item will be consumed upon usage. Returns true by default.
		/// </summary>
		/// <param name="player">The player.</param>
		/// <returns></returns>
		public virtual bool ConsumeItem(Player player)
		{
			return true;
		}

		/// <summary>
		/// Allows you to modify the player's animation when this item is being used. Return true if you modify the player's animation. Returns false by default.
		/// </summary>
		/// <param name="player">The player.</param>
		/// <returns></returns>
		public virtual bool UseItemFrame(Player player)
		{
			return false;
		}

		/// <summary>
		/// Allows you to modify the player's animation when the player is holding this item. Return true if you modify the player's animation. Returns false by default.
		/// </summary>
		/// <param name="player">The player.</param>
		/// <returns></returns>
		public virtual bool HoldItemFrame(Player player)
		{
			return false;
		}

		/// <summary>
		/// Allows you to make this item usable by right-clicking. Returns false by default. When this item is used by right-clicking, player.altFunctionUse will be set to 2.
		/// </summary>
		/// <param name="player">The player.</param>
		/// <returns></returns>
		public virtual bool AltFunctionUse(Player player)
		{
			return false;
		}

		/// <summary>
		/// Allows you to make things happen when this item is in the player's inventory (for example, how the cell phone makes information display).
		/// </summary>
		/// <param name="player">The player.</param>
		public virtual void UpdateInventory(Player player)
		{
		}

		/// <summary>
		/// Allows you to give effects to this armor or accessory, such as increased damage.
		/// </summary>
		/// <param name="player">The player.</param>
		public virtual void UpdateEquip(Player player)
		{
		}

		/// <summary>
		/// Allows you to give effects to this accessory. The hideVisual parameter is whether the player has marked the accessory slot to be hidden from being drawn on the player.
		/// </summary>
		/// <param name="player">The player.</param>
		/// <param name="hideVisual">if set to <c>true</c> the accessory is hidden.</param>
		public virtual void UpdateAccessory(Player player, bool hideVisual)
		{
		}

		/// <summary>
		/// Allows you to create special effects (such as dust) when this item's equipment texture of the given equipment type is displayed on the player. Note that this hook is only ever called through this item's associated equipment texture.
		/// </summary>
		/// <param name="player">The player.</param>
		/// <param name="type">The type.</param>
		public virtual void UpdateVanity(Player player, EquipType type)
		{
		}

		/// <summary>
		/// Returns whether or not the head armor, body armor, and leg armor make up a set. If this returns true, then this item's UpdateArmorSet method will be called. Returns false by default.
		/// </summary>
		/// <param name="head">The head.</param>
		/// <param name="body">The body.</param>
		/// <param name="legs">The legs.</param>
		public virtual bool IsArmorSet(Item head, Item body, Item legs)
		{
			return false;
		}

		/// <summary>
		/// Allows you to give set bonuses to the armor set that this armor is in.
		/// </summary>
		/// <param name="player">The player.</param>
		public virtual void UpdateArmorSet(Player player)
		{
		}

		/// <summary>
		/// Returns whether or not the head armor, body armor, and leg armor textures make up a set. This hook is used for the PreUpdateVanitySet, UpdateVanitySet, and ArmorSetShadow hooks. By default, this will return the same value as the IsArmorSet hook (passing the equipment textures' associated items as parameters), so you will not have to use this hook unless you want vanity effects to be entirely separate from armor sets. Note that this hook is only ever called through this item's associated equipment texture.
		/// </summary>
		/// <param name="head">The head.</param>
		/// <param name="body">The body.</param>
		/// <param name="legs">The legs.</param>
		public virtual bool IsVanitySet(int head, int body, int legs)
		{
			Item headItem = new Item();
			if (head >= 0)
			{
				headItem.SetDefaults(Item.headType[head], true);
			}
			Item bodyItem = new Item();
			if (body >= 0)
			{
				bodyItem.SetDefaults(Item.bodyType[body], true);
			}
			Item legItem = new Item();
			if (legs >= 0)
			{
				legItem.SetDefaults(Item.legType[legs], true);
			}
			return IsArmorSet(headItem, bodyItem, legItem);
		}

		/// <summary>
		/// Allows you to create special effects (such as the necro armor's hurt noise) when the player wears this item's vanity set. This hook is called regardless of whether the player is frozen in any way. Note that this hook is only ever called through this item's associated equipment texture.
		/// </summary>
		/// <param name="player">The player.</param>
		public virtual void PreUpdateVanitySet(Player player)
		{
		}

		/// <summary>
		/// Allows you to create special effects (such as dust) when the player wears this item's vanity set. This hook will only be called if the player is not frozen in any way. Note that this hook is only ever called through this item's associated equipment texture.
		/// </summary>
		/// <param name="player">The player.</param>
		public virtual void UpdateVanitySet(Player player)
		{
		}

		/// <summary>
		/// Allows you to determine special visual effects this vanity set has on the player without having to code them yourself. Note that this hook is only ever called through this item's associated equipment texture. Use the player.armorEffectDraw bools to activate the desired effects.
		/// </summary>
		/// <example><code>player.armorEffectDrawShadow = true;</code></example>
		/// <param name="player">The player.</param>
		public virtual void ArmorSetShadows(Player player)
		{
		}

		/// <summary>
		/// Allows you to modify the equipment that the player appears to be wearing. This hook will only be called for body armor and leg armor. Note that equipSlot is not the same as the item type of the armor the player will appear to be wearing. Worn equipment has a separate set of IDs. You can find the vanilla equipment IDs by looking at the headSlot, bodySlot, and legSlot fields for items, and modded equipment IDs by looking at EquipLoader.
		/// If this hook is called on body armor, equipSlot allows you to modify the leg armor the player appears to be wearing. If you modify it, make sure to set robes to true. If this hook is called on leg armor, equipSlot allows you to modify the leg armor the player appears to be wearing, and the robes parameter is useless.
		/// Note that this hook is only ever called through this item's associated equipment texture.
		/// </summary>
		/// <param name="male">if set to <c>true</c> [male].</param>
		/// <param name="equipSlot">The equip slot.</param>
		/// <param name="robes">if set to <c>true</c> [robes].</param>
		public virtual void SetMatch(bool male, ref int equipSlot, ref bool robes)
		{
		}

		/// <summary>
		/// Returns whether or not this item does something when it is right-clicked in the inventory. Returns false by default.
		/// </summary>
		public virtual bool CanRightClick()
		{
			return false;
		}

		/// <summary>
		/// Allows you to make things happen when this item is right-clicked in the inventory. Useful for goodie bags.
		/// </summary>
		/// <param name="player">The player.</param>
		public virtual void RightClick(Player player)
		{
		}

		/// <summary>
		/// Allows you to give items to the given player when this item is right-clicked in the inventory if the bossBagNPC field has been set to a positive number. This ignores the CanRightClick and RightClick hooks.
		/// </summary>
		/// <param name="player">The player.</param>
		public virtual void OpenBossBag(Player player)
		{
		}

		/// <summary>
		/// This hooks gets called immediately before an item gets reforged by the Goblin Tinkerer. Useful for storing custom data, since reforging erases custom data. Note that, because the ModItem instance will change, the data must be backed up elsewhere, such as in static fields.
		/// </summary>
		public virtual void PreReforge()
		{
		}

		/// <summary>
		/// This hook gets called immediately after an item gets reforged by the Goblin Tinkerer. Useful for restoring custom data that you saved in PreReforge.
		/// </summary>
		public virtual void PostReforge()
		{
		}

		/// <summary>
		/// Allows you to determine whether the skin/shirt on the player's arms and hands are drawn when this body armor is worn. By default both flags will be false. Note that if drawHands is false, the arms will not be drawn either. Also note that this hook is only ever called through this item's associated equipment texture.
		/// </summary>
		/// <param name="drawHands">if set to <c>true</c> [draw hands].</param>
		/// <param name="drawArms">if set to <c>true</c> [draw arms].</param>
		public virtual void DrawHands(ref bool drawHands, ref bool drawArms)
		{
		}

		/// <summary>
		/// Allows you to determine whether the player's hair or alt (hat) hair draws when this head armor is worn. By default both flags will be false. Note that this hook is only ever called through this item's associated equipment texture.
		/// </summary>
		/// <param name="drawHair">if set to <c>true</c> [draw hair].</param>
		/// <param name="drawAltHair">if set to <c>true</c> [draw alt hair].</param>
		public virtual void DrawHair(ref bool drawHair, ref bool drawAltHair)
		{
		}

		/// <summary>
		/// Return false to hide the player's head when this head armor is worn. Returns true by default. Note that this hook is only ever called through this item's associated equipment texture.
		/// </summary>
		/// <returns></returns>
		public virtual bool DrawHead()
		{
			return true;
		}

		/// <summary>
		/// Return false to hide the player's body when this body armor is worn. Returns true by default. Note that this hook is only ever called through this item's associated equipment texture.
		/// </summary>
		/// <returns></returns>
		public virtual bool DrawBody()
		{
			return true;
		}

		/// <summary>
		/// Return false to hide the player's legs when this leg armor or shoe accessory is worn. Returns true by default. Note that this hook is only ever called through this item's associated equipment texture.
		/// </summary>
		/// <returns></returns>
		public virtual bool DrawLegs()
		{
			return true;
		}

		/// <summary>
		/// Allows you to modify the colors in which this armor and surrounding accessories are drawn, in addition to which glow mask and in what color is drawn. Note that this hook is only ever called through this item's associated equipment texture.
		/// </summary>
		/// <param name="drawPlayer">The draw player.</param>
		/// <param name="shadow">The shadow.</param>
		/// <param name="color">The color.</param>
		/// <param name="glowMask">The glow mask.</param>
		/// <param name="glowMaskColor">Color of the glow mask.</param>
		public virtual void DrawArmorColor(Player drawPlayer, float shadow, ref Color color, ref int glowMask, ref Color glowMaskColor)
		{
		}

		/// <summary>
		/// Allows you to modify which glow mask and in what color is drawn on the player's arms. Note that this is only called for body armor. Also note that this hook is only ever called through this item's associated equipment texture.
		/// </summary>
		/// <param name="drawPlayer">The draw player.</param>
		/// <param name="shadow">The shadow.</param>
		/// <param name="glowMask">The glow mask.</param>
		/// <param name="color">The color.</param>
		public virtual void ArmorArmGlowMask(Player drawPlayer, float shadow, ref int glowMask, ref Color color)
		{
		}

		/// <summary>
		/// Obsolete: Use the overloaded method with the player parameter.
		/// </summary>
		[method: Obsolete("Use the overloaded method with the player parameter.")]
		public virtual void VerticalWingSpeeds(ref float ascentWhenFalling, ref float ascentWhenRising,
			ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
		{
		}

		/// <summary>
		/// Allows you to modify the speeds at which you rise and fall when these wings are equipped.
		/// </summary>
		/// <param name="player">The player.</param>
		/// <param name="ascentWhenFalling">The ascent when falling.</param>
		/// <param name="ascentWhenRising">The ascent when rising.</param>
		/// <param name="maxCanAscendMultiplier">The maximum can ascend multiplier.</param>
		/// <param name="maxAscentMultiplier">The maximum ascent multiplier.</param>
		/// <param name="constantAscend">The constant ascend.</param>
		public virtual void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising,
	ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
		{
			VerticalWingSpeeds(ref ascentWhenFalling, ref ascentWhenRising, ref maxCanAscendMultiplier, ref maxAscentMultiplier, ref constantAscend);
		}

		/// <summary>
		/// Obsolete: Use the overloaded method with the player parameter.
		/// </summary>
		[method: Obsolete("Use the overloaded method with the player parameter.")]
		public virtual void HorizontalWingSpeeds(ref float speed, ref float acceleration)
		{
		}

		/// <summary>
		/// Allows you to modify these wing's horizontal flight speed and acceleration.
		/// </summary>
		/// <param name="player">The player.</param>
		/// <param name="speed">The speed.</param>
		/// <param name="acceleration">The acceleration.</param>
		public virtual void HorizontalWingSpeeds(Player player, ref float speed, ref float acceleration)
		{
			HorizontalWingSpeeds(ref speed, ref acceleration);
		}

		/// <summary>
		/// Obsolete: WingUpdate will return a bool value later. (Use NewWingUpdate in the meantime.)
		/// </summary>
		[method: Obsolete("WingUpdate will return a bool value later. (Use NewWingUpdate in the meantime.)")]
		public virtual void WingUpdate(Player player, bool inUse)
		{
		}

		/// <summary>
		/// Allows for Wings to do various things while in use. "inUse" is whether or not the jump button is currently pressed. Called when these wings visually appear on the player. Use to animate wings, create dusts, invoke sounds, and create lights. Note that this hook is only ever called through this item's associated equipment texture. False will keep everything the same. True, you need to handle all animations in your own code.
		/// </summary>
		/// <param name="player">The player.</param>
		/// <param name="inUse">if set to <c>true</c> [in use].</param>
		/// <returns></returns>
		public virtual bool NewWingUpdate(Player player, bool inUse)
		{
			return false;
		}

		/// <summary>
		/// Allows you to customize this item's movement when lying in the world. Note that this will not be called if this item is currently being grabbed by a player.
		/// </summary>
		/// <param name="gravity">The gravity.</param>
		/// <param name="maxFallSpeed">The maximum fall speed.</param>
		public virtual void Update(ref float gravity, ref float maxFallSpeed)
		{
		}

		/// <summary>
		/// Allows you to make things happen when this item is lying in the world. This will always be called, even when it is being grabbed by a player. This hook should be used for adding light, or for increasing the age of less valuable items.
		/// </summary>
		public virtual void PostUpdate()
		{
		}

		/// <summary>
		/// Allows you to modify how close this item must be to the player in order to move towards the player.
		/// </summary>
		/// <param name="player">The player.</param>
		/// <param name="grabRange">The grab range.</param>
		public virtual void GrabRange(Player player, ref int grabRange)
		{
		}

		/// <summary>
		/// Allows you to modify the way this item moves towards the player. Return true if you override this hook; returning false will allow the vanilla grab style to take place. Returns false by default.
		/// </summary>
		/// <param name="player">The player.</param>
		/// <returns></returns>
		public virtual bool GrabStyle(Player player)
		{
			return false;
		}

		/// <summary>
		/// Allows you to determine whether or not the item can be picked up
		/// </summary>
		/// <param name="player">The player.</param>
		public virtual bool CanPickup(Player player)
		{
			return true;
		}

		/// <summary>
		/// Allows you to make special things happen when the player picks up this item. Return false to stop the item from being added to the player's inventory; returns true by default.
		/// </summary>
		/// <param name="player">The player.</param>
		/// <returns></returns>
		public virtual bool OnPickup(Player player)
		{
			return true;
		}

		/// <summary>
		/// Return true to specify that the item can be picked up despite not having enough room in inventory. Useful for something like hearts or experience items. Use in conjunction with OnPickup to actually consume the item and handle it.
		/// </summary>
		/// <param name="player">The player.</param>
		/// <returns></returns>
		public virtual bool ExtraPickupSpace(Player player)
		{
			return false;
		}

		/// <summary>
		/// Allows you to determine the color and transparency in which this item is drawn. Return null to use the default color (normally light color). Returns null by default.
		/// </summary>
		/// <param name="lightColor">Color of the light.</param>
		/// <returns></returns>
		public virtual Color? GetAlpha(Color lightColor)
		{
			return null;
		}

		/// <summary>
		/// Allows you to draw things behind this item, or to modify the way this item is drawn in the world. Return false to stop the game from drawing the item (useful if you're manually drawing the item). Returns true by default.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch.</param>
		/// <param name="lightColor">Color of the light.</param>
		/// <param name="alphaColor">Color of the alpha.</param>
		/// <param name="rotation">The rotation.</param>
		/// <param name="scale">The scale.</param>
		/// <param name="whoAmI">The who am i.</param>
		/// <returns></returns>
		public virtual bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
		{
			return true;
		}

		/// <summary>
		/// Allows you to draw things in front of this item. This method is called even if PreDrawInWorld returns false.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch.</param>
		/// <param name="lightColor">Color of the light.</param>
		/// <param name="alphaColor">Color of the alpha.</param>
		/// <param name="rotation">The rotation.</param>
		/// <param name="scale">The scale.</param>
		/// <param name="whoAmI">The who am i.</param>
		public virtual void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
		}

		/// <summary>
		/// Allows you to draw things behind this item in the inventory. Return false to stop the game from drawing the item (useful if you're manually drawing the item). Returns true by default.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch.</param>
		/// <param name="position">The position.</param>
		/// <param name="frame">The frame.</param>
		/// <param name="drawColor">Color of the draw.</param>
		/// <param name="itemColor">Color of the item.</param>
		/// <param name="origin">The origin.</param>
		/// <param name="scale">The scale.</param>
		/// <returns></returns>
		public virtual bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor,
			Color itemColor, Vector2 origin, float scale)
		{
			return true;
		}

		/// <summary>
		/// Allows you to draw things in front of this item in the inventory. This method is called even if PreDrawInInventory returns false.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch.</param>
		/// <param name="position">The position.</param>
		/// <param name="frame">The frame.</param>
		/// <param name="drawColor">Color of the draw.</param>
		/// <param name="itemColor">Color of the item.</param>
		/// <param name="origin">The origin.</param>
		/// <param name="scale">The scale.</param>
		public virtual void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor,
			Color itemColor, Vector2 origin, float scale)
		{
		}

		/// <summary>
		/// Allows you to determine the offset of this item's sprite when used by the player. This is only used for items with a useStyle of 5 that aren't staves. Return null to use the vanilla holdout offset; returns null by default.
		/// </summary>
		/// <returns></returns>
		public virtual Vector2? HoldoutOffset()
		{
			return null;
		}

		/// <summary>
		/// Allows you to determine the point on this item's sprite that the player holds onto when using this item. The origin is from the bottom left corner of the sprite. This is only used for staves with a useStyle of 5. Return null to use the vanilla holdout origin (zero); returns null by default.
		/// </summary>
		/// <returns></returns>
		public virtual Vector2? HoldoutOrigin()
		{
			return null;
		}

		/// <summary>
		/// Allows you to disallow the player from equipping this accessory. Return false to disallow equipping this accessory. Returns true by default.
		/// </summary>
		/// <param name="player">The player.</param>
		/// <param name="slot">The slot.</param>
		public virtual bool CanEquipAccessory(Player player, int slot)
		{
			return true;
		}

		/// <summary>
		/// Allows you to modify what item, and in what quantity, is obtained when this item is fed into the Extractinator. By default the parameters will be set to the output of feeding Silt/Slush into the Extractinator.
		/// </summary>
		/// <param name="resultType">Type of the result.</param>
		/// <param name="resultStack">The result stack.</param>
		public virtual void ExtractinatorUse(ref int resultType, ref int resultStack)
		{
		}

		/// <summary>
		/// Allows you to tell the game whether this item is a torch that cannot be placed in liquid, a torch that can be placed in liquid, or a glowstick. This information is used for when the player is holding down the auto-select hotkey.
		/// </summary>
		/// <param name="dryTorch">if set to <c>true</c> [dry torch].</param>
		/// <param name="wetTorch">if set to <c>true</c> [wet torch].</param>
		/// <param name="glowstick">if set to <c>true</c> [glowstick].</param>
		public virtual void AutoLightSelect(ref bool dryTorch, ref bool wetTorch, ref bool glowstick)
		{
		}

		/// <summary>
		/// Allows you to determine how many of this item a player obtains when the player fishes this item.
		/// </summary>
		/// <param name="stack">The stack.</param>
		public virtual void CaughtFishStack(ref int stack)
		{
		}

		/// <summary>
		/// Whether or not the Angler can ever randomly request this type of item for his daily quest. Returns false by default.
		/// </summary>
		public virtual bool IsQuestFish()
		{
			return false;
		}

		/// <summary>
		/// Whether or not specific conditions have been satisfied for the Angler to be able to request this item. (For example, Hardmode.) Returns true by default.
		/// </summary>
		public virtual bool IsAnglerQuestAvailable()
		{
			return true;
		}

		/// <summary>
		/// Allows you to set what the Angler says when he requests for this item. The description parameter is his dialogue, and catchLocation should be set to "\n(Caught at [location])".
		/// </summary>
		/// <param name="description">The description.</param>
		/// <param name="catchLocation">The catch location.</param>
		public virtual void AnglerQuestChat(ref string description, ref string catchLocation)
		{
		}

		internal void SetupItem(Item item)
		{
			SetupModItem(item);
			EquipLoader.SetSlot(item);
			item.modItem.SetDefaults();
		}

		//change Terraria.Item.Clone
		//  Item newItem = (Item)base.MemberwiseClone();
		//  if (newItem.type >= ItemID.Count)
		//  {
		//      ItemLoader.GetItem(newItem.type).SetupModItem(newItem);
		//  }
		//  return newItem;
		internal void SetupModItem(Item item)
		{
			ModItem newItem = Clone(item);
			newItem.item = item;
			item.modItem = newItem;
			newItem.mod = mod;
		}

		internal void SetupClone(Item clone)
		{
			ModItem newItem = CloneNewInstances ? Clone(clone) : (ModItem)Activator.CreateInstance(GetType());
			newItem.item = clone;
			newItem.mod = mod;
			newItem.texture = texture;
			newItem.flameTexture = flameTexture;
			newItem.projOnSwing = projOnSwing;
			newItem.bossBagNPC = bossBagNPC;
			clone.modItem = newItem;
		}

		public virtual ModItem Clone(Item item)
		{
			return Clone();
		}

		/// <summary>
		/// Returns a clone of this ModItem. Allows you to decide which fields of your ModItem class are copied over when an item stack is split or something similar happens. By default all fields that you make will be automatically copied for you. Only called if CloneNewInstances is set to true.
		/// </summary>
		/// <returns></returns>
		public virtual ModItem Clone()
		{
			return (ModItem)MemberwiseClone();
		}

		/// <summary>
		/// Whether instances of this ModItem are created through its Clone hook or its constructor. Defaults to false.
		/// </summary>
		public virtual bool CloneNewInstances => false;

		/// <summary>
		/// Allows you to save custom data for this item. Returns null by default.
		/// </summary>
		/// <returns></returns>
		public virtual TagCompound Save()
		{
			return null;
		}

		/// <summary>
		/// Allows you to load custom data that you have saved for this item.
		/// </summary>
		/// <param name="tag">The tag.</param>
		public virtual void Load(TagCompound tag)
		{
		}

		/// <summary>
		/// Allows you to load pre-v0.9 custom data that you have saved for this item.
		/// </summary>
		/// <param name="reader">The reader.</param>
		public virtual void LoadLegacy(BinaryReader reader)
		{
		}

		/// <summary>
		/// Allows you to send custom data for this item between client and server.
		/// </summary>
		/// <param name="writer">The writer.</param>
		public virtual void NetSend(BinaryWriter writer)
		{
		}

		/// <summary>
		/// Receives the custom data sent in the NetSend hook.
		/// </summary>
		/// <param name="reader">The reader.</param>
		public virtual void NetRecieve(BinaryReader reader)
		{
		}

		/// <summary>
		/// This is essentially the same as Mod.AddRecipes. Do note that this will be called for every instance of the overriding ModItem class that is added to the game. This allows you to avoid clutter in your overriding Mod class by adding recipes for which this item is the result.
		/// </summary>
		public virtual void AddRecipes()
		{
		}

		/// <summary>
		/// Allows you to make anything happen when the player crafts this item using the given recipe.
		/// </summary>
		/// <param name="recipe">The recipe that was used to craft this item.</param>
		public virtual void OnCraft(Recipe recipe)
		{
		}

		/// <summary>
		/// Allows you to modify all the tooltips that display for this item. See here for information about TooltipLine.
		/// </summary>
		/// <param name="tooltips">The tooltips.</param>
		public virtual void ModifyTooltips(List<TooltipLine> tooltips)
		{
		}
	}
}