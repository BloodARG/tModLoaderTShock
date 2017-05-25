using System;
using System.Collections.Generic;
using System.Timers;
using System.Linq;
using Terraria;
using TerrariaApi.Server;
using MySql.Data.MySqlClient;
using TShockAPI;
using TShockAPI.DB;
using System.IO;
using System.Reflection;

namespace HousingDistricts
{
    [ApiVersion(1, 24)]
    public class HousingDistricts : TerrariaPlugin
    {
		public static HConfigFile HConfig { get { return HConfigFile.Config; } }
		public static List<House> Houses = new List<House>();
		public static List<HPlayer> HPlayers = new List<HPlayer>();

		public override string Name
		{
			get { return "HousingDistricts"; }
		}

		public override string Author
		{
			get { return "Twitchy, Dingo, radishes, CoderCow and Simon311"; }
		}

		public override string Description
		{
			get { return "Housing Districts v." + Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
		}

		public override Version Version
		{
			get { return Assembly.GetExecutingAssembly().GetName().Version; }
		}

		// Note: Do NOT replace for, its faster for Lists than Foreach (or Linq, huh). Yes, there are studies proving that. No, there is no such difference for arrays.

		internal static bool URunning = false;
		public static bool ULock = false;
		public const int UpdateTimeout = 800;
		static readonly Timer Update = new System.Timers.Timer(500);

		public override void Initialize()
		{
			HConfigFile.ForceLoad();

			ServerApi.Hooks.GameInitialize.Register(this, OnInitialize, -5);
			ServerApi.Hooks.ServerChat.Register(this, OnChat, 5);
			ServerApi.Hooks.NetGreetPlayer.Register(this, OnGreetPlayer, -5);
			ServerApi.Hooks.ServerLeave.Register(this, OnLeave, 5);
			ServerApi.Hooks.NetGetData.Register(this, GetData, 10);
			ServerApi.Hooks.GamePostInitialize.Register(this, PostInitialize, -5);
			GetDataHandlers.InitGetDataHandler();
			if (!HConfig.DisableUpdateTimer)
			{
				Update.Elapsed += OnUpdate;
				Update.Start();
				URunning = true;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				ServerApi.Hooks.GameInitialize.Deregister(this, OnInitialize);
				ServerApi.Hooks.ServerChat.Deregister(this, OnChat);
				ServerApi.Hooks.NetGreetPlayer.Deregister(this, OnGreetPlayer);
				ServerApi.Hooks.ServerLeave.Deregister(this, OnLeave);
				ServerApi.Hooks.NetGetData.Deregister(this, GetData);
				Update.Elapsed -= OnUpdate;
				Update.Stop();
				URunning = false;
			}

			base.Dispose(disposing);
		}

		public HousingDistricts(Main game) : base(game)
		{
			Order = 5;
		}

		public void OnInitialize(EventArgs e)
		{
			#region Setup
			bool permspresent = false;

			foreach (Group group in TShock.Groups.groups)
			{
				if (group.Name == "superadmin") continue;
				permspresent = group.HasPermission("house.use") || group.HasPermission("house.edit") || group.HasPermission("house.enterlocked") ||
						group.HasPermission("house.admin") || group.HasPermission("house.bypasscount") || group.HasPermission("house.bypasssize") ||
						group.HasPermission("house.lock");
				if (permspresent) break;
			}

			List<string> trustedperm = new List<string>();
			List<string> defaultperm = new List<string>();

			if (!permspresent)
			{
				defaultperm.Add("house.use");
				trustedperm.Add("house.edit");
				trustedperm.Add("house.enterlocked");
				trustedperm.Add("house.admin");
				trustedperm.Add("house.bypasscount");
				trustedperm.Add("house.bypasssize");
				defaultperm.Add("house.lock");

				TShock.Groups.AddPermissions("trustedadmin", trustedperm);
				TShock.Groups.AddPermissions("default", defaultperm);
			}

			var table = new SqlTable("HousingDistrict",
				new SqlColumn("ID", MySqlDbType.Int32) { Primary = true, AutoIncrement = true },
				new SqlColumn("Name", MySqlDbType.VarChar, 255) { Unique = true },
				new SqlColumn("TopX", MySqlDbType.Int32),
				new SqlColumn("TopY", MySqlDbType.Int32),
				new SqlColumn("BottomX", MySqlDbType.Int32),
				new SqlColumn("BottomY", MySqlDbType.Int32),
				new SqlColumn("Owners", MySqlDbType.Text),
				new SqlColumn("WorldID", MySqlDbType.Text),
				new SqlColumn("Locked", MySqlDbType.Int32),
				new SqlColumn("ChatEnabled", MySqlDbType.Int32),
				new SqlColumn("Visitors", MySqlDbType.Text)
			);

			var SQLWriter = new SqlTableCreator(TShock.DB, TShock.DB.GetSqlType() == SqlType.Sqlite ? (IQueryBuilder)new SqliteQueryCreator() : new MysqlQueryCreator());
			SQLWriter.EnsureTableStructure(table);
			
			#endregion

			List<string> perms = new List<string>();
			perms.Add("house.use");
			perms.Add("house.lock");
			perms.Add("house.root");

			#region Commands
			Commands.ChatCommands.Add(new Command(perms, HCommands.House, "house"));
			Commands.ChatCommands.Add(new Command("tshock.canchat", HCommands.TellAll, "all"));
			Commands.ChatCommands.Add(new Command("house.root", HCommands.HouseReload, "housereload"));
			Commands.ChatCommands.Add(new Command("house.root", HCommands.HouseWipe, "housewipe"));
			#endregion
		}

		public void PostInitialize(EventArgs e)
		{
			var reader = TShock.DB.QueryReader("Select * from HousingDistrict");
			while (reader.Read())
			{
				if (reader.Get<string>("WorldID") != Main.worldID.ToString())
					continue;

				int id = reader.Get<int>("ID");
				List<string> owners = reader.Get<string>("Owners").Split(',').ToList();
				int locked = reader.Get<int>("Locked");
				int chatenabled = reader.Get<int>("ChatEnabled") == 1 ? 1 : 0;
				List<string> visitors = reader.Get<string>("Visitors").Split(',').ToList();
				Houses.Add(new House(new Rectangle(reader.Get<int>("TopX"), reader.Get<int>("TopY"), reader.Get<int>("BottomX"), reader.Get<int>("BottomY")),
					owners, id, reader.Get<string>("Name"), locked, chatenabled, visitors));
			}
		}

		public void OnUpdate(object sender, ElapsedEventArgs e)
		{
			if (Main.worldID == 0) return;
			if (ULock) return;
			ULock = true;
			var Start = DateTime.Now;
			if (Main.rand == null) Main.rand = new Random();
			lock (HPlayers)
			{
				var I = HousingDistricts.HPlayers.Count;
				for (int i = 0; i < I; i++)
				{
					if (UTimeout(Start)) return;
					var player = HousingDistricts.HPlayers[i];
					List<string> NewCurHouses = new List<string>(player.CurHouses);
					int HousesNotIn = 0;
					try
					{
						House.UpdateAction((house) =>
						{
							if (UTimeout(Start)) return;
							try
							{
								if (house.HouseArea.Intersects(new Rectangle(player.TSPlayer.TileX, player.TSPlayer.TileY, 1, 1)))
								{
									if (house.Locked == 1 && !player.TSPlayer.Group.HasPermission("house.enterlocked"))
									{
										if (!HTools.CanVisitHouse(player.TSPlayer.User, house))
										{
											player.TSPlayer.Teleport((int)player.LastTilePos.X * 16, (int)player.LastTilePos.Y * 16);
											player.TSPlayer.SendMessage("House: '" + house.Name + "' Is locked", Color.LightSeaGreen);
										}
										else
										{
											if (!player.CurHouses.Contains(house.Name) && HConfig.NotifyOnEntry)
											{
												NewCurHouses.Add(house.Name);
												if (HTools.OwnsHouse(player.TSPlayer.User, house) && HConfig.NotifySelf)
													player.TSPlayer.SendMessage(HConfig.NotifyOnOwnHouseEntryString.Replace("$HOUSE_NAME", house.Name), Color.LightSeaGreen);
												else
												{
													if (HConfig.NotifyVisitor)
														player.TSPlayer.SendMessage(HConfig.NotifyOnEntryString.Replace("$HOUSE_NAME", house.Name), Color.LightSeaGreen);

													if (HConfig.NotifyOwner)
														HTools.BroadcastToHouseOwners(house.Name, HConfig.NotifyOnOtherEntryString.Replace("$PLAYER_NAME", player.TSPlayer.Name).Replace("$HOUSE_NAME", house.Name));
												}
											}
										}
									}
									else
									{
										if (!player.CurHouses.Contains(house.Name) && HConfig.NotifyOnEntry)
										{
											NewCurHouses.Add(house.Name);
											if (HTools.OwnsHouse(player.TSPlayer.User, house) && HConfig.NotifySelf)
													player.TSPlayer.SendMessage(HConfig.NotifyOnOwnHouseEntryString.Replace("$HOUSE_NAME", house.Name), Color.LightSeaGreen);
											else
											{
												if (HConfig.NotifyVisitor)
													player.TSPlayer.SendMessage(HConfig.NotifyOnEntryString.Replace("$HOUSE_NAME", house.Name), Color.LightSeaGreen);

												if (HConfig.NotifyOwner)
													HTools.BroadcastToHouseOwners(house.Name, HConfig.NotifyOnOtherEntryString.Replace("$PLAYER_NAME", player.TSPlayer.Name).Replace("$HOUSE_NAME", house.Name));
											}
										}
									}
								}
								else
								{
									 NewCurHouses.Remove(house.Name);
									 HousesNotIn++;
								}
							}
							catch (Exception ex)
							{
								TShock.Log.Error(ex.ToString());
							}
						});
					}
					catch (Exception ex)
					{
						TShock.Log.Error(ex.ToString());
						continue;
					}

					if (HConfig.NotifyOnExit)
					{
						{
							var K = player.CurHouses.Count;
							for (int k = 0; k < K; k++)
							{
								if (UTimeout(Start)) return;
								var cHouse = player.CurHouses[k];
								if (!NewCurHouses.Contains(cHouse))
								{
									if (HTools.OwnsHouse(player.TSPlayer.User, cHouse))
									{
										if (HConfig.NotifySelf)
											player.TSPlayer.SendMessage(HConfig.NotifyOnOwnHouseExitString.Replace("$HOUSE_NAME", cHouse), Color.LightSeaGreen);
									}
									else
									{
										if (HConfig.NotifyVisitor)
											player.TSPlayer.SendMessage(HConfig.NotifyOnExitString.Replace("$HOUSE_NAME", cHouse), Color.LightSeaGreen);

										if (HConfig.NotifyOwner)
											HTools.BroadcastToHouseOwners(cHouse, HConfig.NotifyOnOtherExitString.Replace("$PLAYER_NAME", player.TSPlayer.Name).Replace("$HOUSE_NAME", cHouse));
									}
								}
							}
						}
						
					 }

					player.CurHouses = NewCurHouses;
					player.LastTilePos = new Vector2(player.TSPlayer.TileX, player.TSPlayer.TileY);
				}
			}
			ULock = false;
		}

		public void OnChat(ServerChatEventArgs e)
		{
			var Start = DateTime.Now;
			var msg = e.Buffer;
			var ply = e.Who;
			var tsplr = TShock.Players[e.Who];
			var text = e.Text;

			if (e.Handled) return;

			if (text.StartsWith("/grow"))
			{
				if (!tsplr.Group.HasPermission(Permissions.grow)) return;
				var I = Houses.Count;

				for (int i = 0; i < I; i++)
				{
					if (!HTools.OwnsHouse(tsplr.User, Houses[i]) && Houses[i].HouseArea.Intersects(new Rectangle(tsplr.TileX, tsplr.TileY, 1, 1)))
					{
						e.Handled = true;
						tsplr.SendErrorMessage("You can't build here!");
						return;
					}
				}
				return;
			}

			if (HConfig.HouseChatEnabled)
			{
				if (text[0] == '/')
					return;

				var I = HousingDistricts.Houses.Count;
				for (int i = 0; i < I; i++)
				{
					if (Timeout(Start)) return;
					House house;
					try { house = HousingDistricts.Houses[i]; }
					catch { continue; }
					if (house.ChatEnabled == 1 && house.HouseArea.Intersects(new Rectangle(tsplr.TileX, tsplr.TileY, 1, 1)))
					{
						HTools.BroadcastToHouse(house, text, tsplr.Name);
						e.Handled = true;
					}
				}
			}
		}

		public void OnGreetPlayer(GreetPlayerEventArgs e)
		{
			lock (HPlayers)
				HPlayers.Add(new HPlayer(e.Who, new Vector2(TShock.Players[e.Who].TileX, TShock.Players[e.Who].TileY)));
		}

		public void OnLeave(LeaveEventArgs args)
		{
			var Start = DateTime.Now;
			lock (HPlayers)
			{
				var I = HPlayers.Count;
				for (int i = 0; i < I; i++)
				{
					if (Timeout(Start)) return;
					if (HPlayers[i].Index == args.Who)
					{
						HPlayers.RemoveAt(i);
						break;
					}
				}
			}
		}

		private void GetData(GetDataEventArgs e)
		{
			PacketTypes type = e.MsgID;
			var player = TShock.Players[e.Msg.whoAmI];
			if (player == null || !player.ConnectionAlive)
			{
				e.Handled = true;
				return;
			}

			using (var data = new MemoryStream(e.Msg.readBuffer, e.Index, e.Length))
			{
				try
				{
					if (GetDataHandlers.HandlerGetData(type, player, data))
						e.Handled = true;
				}
				catch (Exception ex)
				{
					TShock.Log.Error(ex.ToString());
				}
			}
		}

		public static bool UTimeout(DateTime Start, bool warn = true)
		{
			bool ret = (DateTime.Now - Start).TotalMilliseconds >= UpdateTimeout;
			if (ret)
			{
				ULock = false;
				if (warn)
					TShock.Log.ConsoleInfo("Update thread timeout detected in HousingDistricts. You might want to report this.");
			}
			return ret;
		}

		public static bool Timeout(DateTime Start, int ms = 600, bool warn = true)
		{
			bool ret = (DateTime.Now - Start).TotalMilliseconds >= ms;
			if (ret && warn) 
				TShock.Log.ConsoleInfo("Hook timeout detected in HousingDistricts. You might want to report this.");

			return ret;
		}
    }
}
