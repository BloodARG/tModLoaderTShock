# TShock for Terraria

This is the rolling changelog for TShock for Terraria. Use past tense when adding new entries; sign your name off when you add or change something. This should primarily be things like user changes, not necessarily codebase changes unless it's really relevant or large.

## Upcoming Changes

## TShock 4.3.21
* Compatibility with Terraria 1.3.4.3 (@Patrikkk, @Zaicon).
* API: Version tick 1.26.
* API: Deprecated PlayerDamage and PlayerKillMe packets (now uses PlayerHurtV2 and PlayerDeathV2).
* API: Main.rand now uses UnifiedRandom instead of Random. This WILL break any existing plugin that uses Main.rand.
* Fixed HealOtherPlayer packet exploit (@Simon311).
* Added associated config option for HealOtherPlayer exploit prevention (@Simon311).
* Added `/accountinfo` command to get account information for a given TShock account (@Simon311).
* Removed TShock color parsing from MOTDs (@WhiteXZ).
* Fixed butterfly statues spawning catchable butterflies (@DogooFalchion).
* Implemented some missing balance changes lost in prior version patches (@DogooFalchion).
* Added alias for server shutdown command: stop (@nicatronTg).
* Removed the old REST model. This includes the following endpoints:
 * `/status`
 * `/v2/players/read`
 * `/v2/server/rawcmd` (@WhiteXZ).
* Fixed `/user group` always giving an unhelpful error messaging telling you to check the console, even if we knew exactly why it failed (@nicatronTg).
* Removed _all obsolete methods in TShock marked obsolete prior to this version (all of them)_ (@nicatronTg).
* Fixed issue where registration + login would fail because KnownIps had 0 items and .Last() doesn't work on collections with 0 items (@DogooFalchion, @nicatronTg, @Simon311).
* Added `/uploadssc [player]` which allows someone to upload SSC data for [player] and store it on the server. Adds `tshock.ssc.upload` and `tshock.ssc.upload.others` permission nodes to match (@DogooFalchion).
* Added hardened stone to the whitelist of tiles editable by players (@DogooFalchion).
* Added conversion system to send convert old MOTD format into smart text, while preserving initial line starting values to keep byte optimization for background colors Thanks to (@WhiteXZ, @Simon311, and especially @DogooFalchion) for the hard work on this issue.

## TShock 4.3.20
* Security improvement: The auth system is now automatically disabled if a superadmin exists in the database (@Enerdy).
* Removed the `auth-verify` command since `auth` now serves its purpose when necessary (@Enerdy).
* Security: `/"` exploit can no longer break chat mute filters (@Simon311).
* Fixed an issue where sometimes players could connect briefly during server shutdown, leading to errors (@Simon311).
* Fixed wyverns despawning & not behaving like normal (@WhiteXZ).
* Fixed major security issue where InvokeClientConnect could be exploited to do terrible, terrible things (@Simon311, @nicatronTg, @popstarfreas, @ProfessorXZ, @WhiteXZ).

## TShock 4.3.19
* Compatibility with Terraria 1.3.3.3 (@Simon311)
* API: Version tick 1.25
* API: Resolved some issues with the ItemForceIntoChest hook (@WhiteXZ, @Patrikkk)
* API: Resolved some shonky code that caused Vitamins and other Ankh Shield related items to drop at strange rates or not at all (@ProfessorXZ, @WhiteXZ, @nicatronTg)
* Fixed magical ice blocks not working correctly (@ProfessorXZ)

## TShock 4.3.18

* Compatibility with Terraria 1.3.3.2
* API: Version tick 1.24
* API: Fixed chat line breaks when using chat tags and long strings of text (@ProfessorXZ)
* API: Added ItemForceIntoChest hook (@WhiteXZ)
* API: Included the player's registration date in REST's players/read endpoints (@ProfessorXZ)
* The setdungeon command correctly uses tshock.world.setdungeon as its permission (@OnsenManju)
* Fixed clients being able to "Catch" and remove NPCs (@ProfessorXZ)
* Fixed clients being able to remove other players' portals (@ProfessorXZ)
* Fixed possible client crashes caused by invalid item netIDs (@ProfessorXZ)
* Fixed players being able to bypass permission checks when placing Tile Entities (@ProfessorXZ)
* Fixed players being able to bypass permission checks when placing items in Item Frames (@ProfessorXZ)
* Fixed a bug involving Item Frames which allowed players to duplicate items (@ProfessorXZ)
* Fixed an issue allowing clients to teleport NPCs to arbitrary locations (@ProfessorXZ)
* Fixed a bug where players would get teleported to their previous location after dismounting the Unicorn Mount (@ProfessorXZ)
* Players can no longer quick stack items into region protected chests (@ProfessorXZ)
* Rope placement is no longer blocked by range checks (@ProfessorXZ)
* The Drill Containment Unit breaks blocks properly now (@ProfessorXZ)
* Fixed item duplications caused by range checks and invalid netIDs (@ProfessorXZ)
* Fixed Expert mode coin duplication (@ProfessorXZ)
* Players are no longer able to place liquids using LoadNetModule packet (@ProfessorXZ)
* Explosives are no longer blocked by range checks (@ProfessorXZ)
* Players can no longer bypass tile checks by using the Tile packet (@ProfessorXZ)
* Fixed a bug where players couldn't hammer a Junction Box without "allowclientsideworldedit" permission (@Patrikkk)
* Fixed the client's UI not being draw when setting wind speed to abnormal values (@ProfessorXZ)
* Added a command to start and stop sandstorms (@WhiteXZ)

## TShock 4.3.17

* Compatibility with Terraria 1.3.2.1
* Updated superadmin behaviour to conform to expected behaviour (@WhiteXZ, @Patrikk)
* Fixed a crash involving teleporters and dressers (@WhiteXZ)
* Fixed pressure plates (@Enerdy, @Patrikk)
* Fixed a deadlock in wiring (@Wolfje)
* Fixed a crash in wiring (@Patrikk)
* Improved network syncing on client joins (@Patrikk)
* The Presserator can now place actuators (@ProfessorXZ)
* Resolved a region error when removing unlisted users from regions (@WhiteXZ)
* Added a `SetDungeon` command to set the dungeon position (@webmilio)
* The currently running world name is now part of the server application's title (@webmilio)
* Gem locks can now be region protected (@mistzzt)
* Players can now place sensors (@mistzzt)
* Repackaged GeoIP with TShock so that GeoIP works (@Enerdy)
* Added permissions to use sundials and start/stop parties (@Patrikk)
* Added an announcement box hook (@mistzzt)
* Added the ability to choose what type of world (crimson/corruption) you generate (@NoNiMad)

## TShock 4.3.16

* Terraria 1.3.1 wiring bugfixes
* Terraria 1.3.1.1 compatibility

## TShock 4.3.15

* This release is actually 4.3.14, but was ticked extra due to a version issue on gen-dev prior to master push.
* Update to 1.3.1

## TShock 4.3.13

* Fixed an issue preventing TShock from starting on certain mono versions (@Wolfje)
* Fixed a deadlock in Wiring (@Wolfje)
* Fixed character styles/gender not being saved properly on first login while SSC is on (@WhiteXZ)
* Added a PlayerPermission hook fired whenever a permission check involving said player occurs (when the new TSPlayer.HasPermission method is called) (@Enerdy)
* Resolved an issue where martian invasions and eclipses would have empty messages if AnonymousBossInvasions was set to true (@WhiteXZ)
* Added an optional `slime` parameter to the `rain` command, allowing slime rain to be started and stopped. New syntax is `rain [slime] <start/stop>` (@WhiteXZ)
* Fixed performance issues due to concurrent dictionary access in TSPlayer (@CoderCow)
* Added an ID property to Regions (@WhiteXZ)
* Fixed an issue where region sizes were calculated incorrectly (@WhiteXZ)
* Fixed a bug in RegionManager preventing regions adding correctly (@pink_panther)
* Fixed another bug in RegionManager preventing regions adding correctly (@WhiteXZ)
* Fixed a routing issue with the `/v2/token/create` REST endpoint
* Removed the `/token/create` REST endpoint. `/v2/token/create` should be used instead.

## TShock 4.3.12

* Fixed issues with TSPlayer.SetTeam not working (@WhiteXZ)
* Fixed /butcher not killing bosses in expert difficulty (@WhiteXZ)
* API: Deprecated PacketBufferer (now obviated by SendQ) (@WhiteXZ)
* API: Building on Windows no longer breaks traps (@Wolfje)
* Fixed bombs, dynamite, and sticky bombs (@Wolfje)
* Removed spammy messages from OnSecondUpdate that confused some server owners (@Wolfje)
* Rewrote some stat tracker code to send actually relevant data to the stats server (@Cleant / George from Multiplay UK)
* Added an opt-out command line switch to disable the stat tracker (--stats-optout) (@Cleant / George from Multiplay UK)
* Added a unique provider token which can be passed to the stat tracker (--provider-token [token]) for tracking servers from the same GSP. (@Cleant / George from Multiplay UK)

## TShock 4.3.11

* This release is actually 4.3.10, but was ticked extra due to a version issue on gen-dev prior to master push.

## TShock 4.3.10

This version features a drop-in tile replacement system by @Wolfje that reduces RAM requirements
by up to 70% on all worlds and CPU requirements up to 10% in the running process.

* Large worlds: from 700MB-1GB -> ~325MB
* Medium worlds: from 500MB -> ~200MB
* Small worlds: from 400MB -> ~125MB

Other notable changes include:

* API: **Drop-in tile storage replacement system** (@Wolfje)
* API: Fixed some possible packet leaks in sendq (@Wolfje)
* API: APIVersion 1.22
* API: Added crash protection around malicious and/or invalid packets (@Wolfje)
* API: Fixed worlds not loading sometimes (@tysonstrange)
* API: Fixed living leaf walls not working as housing
* Fixed an issue preventing some players from joining when the world is saving (@Wolfje)
* Fixed an issue adding a ban on a player who has previously been banned (@Wolfje)
* Fixed /invade martian (@Wolfje)
* Fixed target dummies not working properly (@WhiteXZ)
* Added a config option (DisableSecondUpdateLogs) to prevent log spam from OnSecondUpdate() (@WhiteXZ)
* Added RESTful API login rate limiting (@George)
* Added config options (MaximumRequestsPerInterval, RequestBucketDecreaseIntervalMinutes, LimitOnlyFailedLoginRequests) for rate limiting (@George)
* **DEPRECATION**: Deprecated Disable(string, bool) and added Disable(string, DisableFlags). Please update your plugins accordingly (@WhiteXZ)
* Fixed Halloween and Christmas events not working properly (@TomyLobo)
* Fixed the demon heart's extra accessory slot not working correctly in SSC (@WhiteXZ)
* Fixed gender-changing potions not working correctly in SSC (@hastinbe)
* Fixed IP bans not working correctly (@hastinbe)
* Fixed /reload not using the correct permission (@WhiteXZ)
* Fixed TSPlayer.ActiveChest not being tracked correctly resulting in item dupes while disabled (@WhiteXZ)
* /reload now reloads tile and projectile bans

## TShock 4.3.8
* API: Update to Terraria 1.3.0.8 (@Patrikkk)
* **API: Added a crash reporter which collects memory dumps on Windows** (@Wolfje)
* API: New commandline param: `-crashdir` - Writes crash reports to the specified directory (@Wolfje)
* API: Sendq now doesn't disconnect people when it cant send a packet (@Wolfje)
* API: Fixed more crashes on disconnect in sendq (@Wolfje)
* API: Now ignores unknown server packets (@Wolfje)
* API: Potentially removed arithmetic overflows in server (@Wolfje)

### Using the Crash Reporter

TShock now has a crash reporter built in which writes crash logs to the `crashes` directory
in the event of a catastrophic failure.  **To change where TShock writes its crash logs,
specify the `-crashdir` parameter on the command line**.

1. In the event of a crash, look for a file called `crash_xxxx.zip` in the `crashes` directory
2. Upload the file somewhere, beware the crash file may be quite large (>100MB), anywhere like google drive, dropbox or mega will be fine
3. Post a link to the crash with reproduction steps in the TShock support forum

Alternatively, if you do not want to report the crash, just delete the file.

## TShock 4.3.7

* Auth system kicks players if system is disabled. (@nicatronTg)
* Fixed /login permitting multiple logins without a logout in between. (@nicatronTg)
* Allow[Hallow/Corruption/Crimson]Creep in config now work. (@WhiteXZ)
* API: Treasure bags are now named properly. (@WhiteXZ)
* API: Clients no longer close on disconnect. (@Wolfje)
* API: Add server broadcast hook. (@Patrikk)
* API: Fixed pressure plate hook triggering multiple times. (@Patrikk)
* API: Fixed issues with SendQ writes failing. (@Wolfje)
* API: Version tick to 1.21

## TShock 4.3.6

* API: NPCs shoot the right way (@WhiteXZ)
* API: The server config file works correctly with priority and port (@Patrikkk)
* API: Removed support for favorites and removed JSON dependencies. (@Enerdy)
* API: Removed support for clouds. (@Enerdy)
* API: Fixed a whole lot of bugs with wiring, and in general re-wrote some core bits that were bugged. (@WhiteXZ)
* API: Fixed projectile AI bugs. (@AndrioCelos)
* API: Fixed world saving problems. (WhiteXZ)
* API: Fixed server not accepting more connections once max slots was filled. (@WhiteXZ)
* API: Removed startup parameters and moved them to TShock. (@Cleant)
* API: Item.SetDefaults() no longer kills some tools. (@Enerdy)
* API: Restored chat bubbles. (@WhiteXZ)
* API: Updated to 1.3.0.6. (@Enerdy & @Patrikkk)
* API: Lots and I mean lots of network improvements in the SendQ department. (@tylerjwatson)
* API: Added NpcLootDrop and DropBossBag hooks. (@Patrikkk)
* API: Fixed hook: NpcTriggerPressurePlate (@Patrikkk)
* API: Fixed hook: ProjectileTriggerPressurePlate (@Patrikkk)
* API: Fixed hook: ItemSetDefaultsString (@Patrikkk)
* API: Fixed hook: ItemSetDefaultsInt (@Patrikkk)
* API: Fixed hook: ItemNetDefaults (@Patrikkk)
* API: Fixed hook: GameStatueSpawn (@Patrikkk)
* API: Fixed hook: NpcNetDefaults (@Patrikkk)
* API: Fixed hook: NpcNetSetDefaultsString (@Patrikkk)
* API: Fixed hook: NpcNetSetDefaultsInt (@Patrikkk)
* API: Fixed hook: NpcSpawn (@Patrikkk)
* API: Fixed hook: NpcTransformation (@Patrikkk)
* API: Fixed hook: NpcStrike (@Patrikkk)
* API: Updated AssemblyInfo to 1.3.0.6. (@nicatronTg)
* API: Moved to .NET Framework 4.5. (@tylerjwatson)
* API: Dedicated server input thread doesn't run if input is redirected/piped. (@tylerjwatson)
* API: Wiring.cs methods are now public. (@Stealownz)
* API: Added PlayerTriggerPressurePlate hook. (@Patrikkk)
* API: API Version Tick to 1.20.
* The config option disabling the DCU has been deprecated and will be removed in a future release. (@nicatronTg)
* Fixed bubble tile triggering noclip checks. (@Enerdy)
* Updated projectile handling in GetDataHandlers. (@WhiteXZ)
* Fixed issue #992. (@WhiteXZ)
* Teleport handler now handles wormholes. (@WhiteXZ)
* Fixed tall gates and trap doors (issue #998). (@WhiteXZ)
* Added monoliths to orientable tiles (issue #999). (@WhiteXZ)
* Fixed vortex stealth armor (issue #964). (@WhiteXZ)
* Added moon lord to spawn boss. (@WhiteXZ)
* Fixed serverpassword syntax error error message. (@JordyMoos)
* Fixed issue #1019. (@WhiteXZ)
* Fix: Region protection prevents placement of objects. (@Patrikkk)
* Moved all startup parameters to TShock. (@Cleant)
* Fix: Target dummies are no longer butchered. (@Denway)
* Added projectile 465 to the ignore list, which fixes some other issues. (@Enerdy)
* Fix: Logging out is now safe with SSC (/logout) (issue #1037). (@WhiteXZ)
* API/TShock: Removed -world parameter from TShock, put it back in the API. (@tylerjwatson)

## TShock 4.3.5

* Fix HandleSpawnBoss, and as a result the spawnboss command and boss spawning items. (@Ijwu)
* Rewrite SendQ for more network stack improvements (@tylerjwatson)
* Update to Terraria 1.3.0.5 (@Patrikkk)

## TShock 4.3.4

* Fix invasion progress messages (@WhiteXZ)
* Completely rewrite SendQ to have less deadlocks (@tylerjwatson)

## TShock 4.3.3

* Fix dihydrogen monoxide (@tylerjwatson)
* Whitelist another boss projectile (@Patrikkk, @WhiteXZ)

## TShock 4.3.2

* Fixed the issue where using the Super Absorbent Sponge would disable users (@WhiteXZ)
* Fixed an issue in NetGetData where e.Length - 1 would be -1 (@WhiteXZ)
* Fixed /who -i and /userinfo (@Enerdy)
* API: OnRegionEntered hook now returns the region entered (@Patrikkk)
* Support for Terraria 1.3.0.4 (@nicatronTg)
* Fixed dressers being unbreakable. (@nicatronTg)
* Fixed wall placement mechanics (@nicatronTg, @Ijwu, @WhiteXZ)
* Fixed Moon Lord projectiles disabling players (@k0rd, @nicatronTg)
* Fixed several potential crashes in server (@Patrikkk)
* Fixed -autocreate command line argument (@WhiteXZ, @nicatronTg)
* Added more world data to world load menu (@WhiteXZ)
* Moved server password to TShock config (@Enerdy)
* Fixed world delete in server (@benjiro)
* Fixed disappearing NPCs (@WhiteXZ)
* Added much more performant code, SendQ, to server module. Reduces downstream network overhead by at least 40% (@tylerjwatson)
* API: Updated TSPlayer.Disable to use new buffs (@Enerdy)
* Updated default max damage & projectile damage to 1,175 (based on 625 people)
* Fixed support for SSC (@WhiteXZ)

## TShock 4.3.1

* Fixed a bug where /user group failing would output no error. (@nicatronTg)
* Fixed a bug where /user group would fail. @(Enerdy)
* Added the ability to disable backup autosave messages. (@nicatronTg)
* Fixed /buff malfunctioning when entering an invalid buff name. (@Enerdy)
* Fixed projectiles 435-438 (martian invasion) freezing everyone under certain conditions. (@Enerdy)
* DisableTombstones now works properly with the new golden gravestones. (@Enerdy)
* REST module now properly catches exceptions during Start(). (@Patrikkk)
* Added /expert command to toggle expert mode. (@WhiteXZ)
* Fixed pirate invasions. (@patrik)
* Fixed worldinfo packet. (@WhiteXZ)
* Fixed server passwords. (@Enerdy)

## TShock 4.3.0.0

* API: Modifed NetItem so that it's actually useful. (@MarioE)
* Updated prebuilts (SQLite, JSON, MySQL) to latest versions. (@nicatronTg)
* Added a minimum password length to prevent blank passwords. (@nicatronTg)
* Modified item ban checks to provide which item is disabling a player in the logs. (@Enerdy)
* API: Modified TSPlayer to store a user, and deprecated calls to TSPlayer.User.ID. (@WhiteXZ)
* Modified chat color specs in config file to be int arrays rather than floats. (@nicatronTg)
* Modified verbiage for ```/auth``` and ```/auth-verify``` to make it clearer how they operate. (@nicatronTg)
* API: Added fuzzy name searching for users. (@WhiteXZ)
* API: Fixed ```OnPlayerLogout``` not being fired when a player disconnects. (@nicatronTg)
* API: Deprecated ```ValidString``` and ```SanitizeString``` methods in Utils. (@nicatronTg)
* Added BCrypt password hashing and related systems for it. BCrypt replaces the old system using non-password hashing algorithms for storing passwords. It breaks implementations of the login code that were manually recreated, but is otherwise seamless in transition. (@nicatronTg)
* API: Added ```User.VerifyPassword(string password)``` which verifies if the user's password matches their stored hash. It automatically upgrades a users' password to BCrypt if called and the password stored is not a BCrypt hash. (@nicatronTg)
* API: Deprecated ```Utils.HashPassword``` and related password hashing functions as those are no longer needed for plugin access. (@nicatronTg)
* Fixed ```UseServerName``` config option so that it correctly sends the config server name any time that Main.WorldName is used. (@Olink)
* Fixed a bug where people could ban themselves. (@nicatronTg)
* Fixed a bug where banning a player who never logged in caused problems. (@nicatronTg)
* Terraria 1.3.0.3 support.
