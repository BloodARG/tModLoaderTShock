﻿/*
TShock, a server mod for Terraria
Copyright (C) 2011-2016 Nyx Studios (fka. The TShock Team)

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;

namespace TShockAPI.DB
{
    /// <summary>
    /// Class that manages bans.
    /// </summary>
	public class BanManager
	{
		private IDbConnection database;

        /// <summary>
        /// Initializes a new instance of the <see cref="TShockAPI.DB.BanManager"/> class.
        /// </summary>
        /// <param name="db">A valid connection to the TShock database</param>
		public BanManager(IDbConnection db)
		{
			database = db;

			var table = new SqlTable("Bans",
			                         new SqlColumn("IP", MySqlDbType.String, 16) {Primary = true},
			                         new SqlColumn("Name", MySqlDbType.Text),
									 new SqlColumn("UUID", MySqlDbType.Text),
			                         new SqlColumn("Reason", MySqlDbType.Text),
                                     new SqlColumn("BanningUser", MySqlDbType.Text),
                                     new SqlColumn("Date", MySqlDbType.Text),
                                     new SqlColumn("Expiration", MySqlDbType.Text)
				);
			var creator = new SqlTableCreator(db,
			                                  db.GetSqlType() == SqlType.Sqlite
			                                  	? (IQueryBuilder) new SqliteQueryCreator()
			                                  	: new MysqlQueryCreator());
			try
			{
				creator.EnsureTableStructure(table);
			}
			catch (DllNotFoundException)
			{
				System.Console.WriteLine("Possible problem with your database - is Sqlite3.dll present?");
				throw new Exception("Could not find a database library (probably Sqlite3.dll)");
			}
		}

		/// <summary>
		/// Gets a ban by IP.
		/// </summary>
		/// <param name="ip">The IP.</param>
		/// <returns>The ban.</returns>
		public Ban GetBanByIp(string ip)
		{
			try
			{
				using (var reader = database.QueryReader("SELECT * FROM Bans WHERE IP=@0", ip))
				{
					if (reader.Read())
						return new Ban(reader.Get<string>("IP"), reader.Get<string>("Name"), reader.Get<string>("UUID"), reader.Get<string>("Reason"), reader.Get<string>("BanningUser"), reader.Get<string>("Date"), reader.Get<string>("Expiration"));
				}
			}
			catch (Exception ex)
			{
				TShock.Log.Error(ex.ToString());
			}
			return null;
		}

		/// <summary>
		/// Gets a list of bans.
		/// </summary>
		public List<Ban> GetBans()
		{
			List<Ban> banlist = new List<Ban>();
			try
			{
				using (var reader = database.QueryReader("SELECT * FROM Bans"))
				{
					while (reader.Read())
					{
						banlist.Add(new Ban(reader.Get<string>("IP"), reader.Get<string>("Name"), reader.Get<string>("UUID"), reader.Get<string>("Reason"), reader.Get<string>("BanningUser"), reader.Get<string>("Date"), reader.Get<string>("Expiration")));					
					}
					return banlist;
				}
			}
			catch (Exception ex)
			{
				TShock.Log.Error(ex.ToString());
				Console.WriteLine(ex.StackTrace);
			}
			return null;
		}

		/// <summary>
		/// Gets a ban by name.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="casesensitive">Whether to check with case sensitivity.</param>
		/// <returns>The ban.</returns>
		public Ban GetBanByName(string name, bool casesensitive = true)
		{
			try
			{
				var namecol = casesensitive ? "Name" : "UPPER(Name)";
				if (!casesensitive)
					name = name.ToUpper();
				using (var reader = database.QueryReader("SELECT * FROM Bans WHERE " + namecol + "=@0", name))
				{
					if (reader.Read())
						return new Ban(reader.Get<string>("IP"), reader.Get<string>("Name"), reader.Get<string>("UUID"), reader.Get<string>("Reason"), reader.Get<string>("BanningUser"), reader.Get<string>("Date"), reader.Get<string>("Expiration"));
				}
			}
			catch (Exception ex)
			{
				TShock.Log.Error(ex.ToString());
			}
			return null;
		}

		/// <summary>
		/// Gets a ban by UUID.
		/// </summary>
		/// <param name="uuid">The UUID.</param>
		/// <returns>The ban.</returns>
		public Ban GetBanByUUID(string uuid)
		{
			try
			{
				using (var reader = database.QueryReader("SELECT * FROM Bans WHERE UUID=@0", uuid))
				{
					if (reader.Read())
						return new Ban(reader.Get<string>("IP"), reader.Get<string>("Name"), reader.Get<string>("UUID"), reader.Get<string>("Reason"), reader.Get<string>("BanningUser"), reader.Get<string>("Date"), reader.Get<string>("Expiration"));
				}
			}
			catch (Exception ex)
			{
				TShock.Log.Error(ex.ToString());
			}
			return null;
		}

        /// <summary>
        /// Adds a ban.
        /// </summary>
        /// <returns><c>true</c>, if ban was added, <c>false</c> otherwise.</returns>
        /// <param name="ip">Ip.</param>
        /// <param name="name">Name.</param>
        /// <param name="uuid">UUID.</param>
        /// <param name="reason">Reason.</param>
        /// <param name="exceptions">If set to <c>true</c> enable throwing exceptions.</param>
        /// <param name="banner">Banner.</param>
        /// <param name="expiration">Expiration date.</param>
		public bool AddBan(string ip, string name = "", string uuid = "", string reason = "", bool exceptions = false, string banner = "", string expiration = "")
		{
			try
			{
                /*
                 * If the ban already exists, update its entry with the new date
                 * and expiration details.
                 */
                if (GetBanByIp(ip) != null)
                {
                    return database.Query("UPDATE Bans SET Date = @0, Expiration = @1 WHERE IP = @2", DateTime.UtcNow.ToString("s"), expiration, ip) == 1;
                }
                else
                {
                    return database.Query("INSERT INTO Bans (IP, Name, UUID, Reason, BanningUser, Date, Expiration) VALUES (@0, @1, @2, @3, @4, @5, @6);", ip, name, uuid, reason, banner, DateTime.UtcNow.ToString("s"), expiration) != 0;
                }
			}
			catch (Exception ex)
			{
				if (exceptions)
					throw ex;
				TShock.Log.Error(ex.ToString());
			}
			return false;
		}

        /// <summary>
        /// Removes a ban.
        /// </summary>
        /// <returns><c>true</c>, if ban was removed, <c>false</c> otherwise.</returns>
        /// <param name="match">Match.</param>
        /// <param name="byName">If set to <c>true</c> by name.</param>
        /// <param name="casesensitive">If set to <c>true</c> casesensitive.</param>
        /// <param name="exceptions">If set to <c>true</c> exceptions.</param>
		public bool RemoveBan(string match, bool byName = false, bool casesensitive = true, bool exceptions = false)
		{
			try
			{
				if (!byName)
					return database.Query("DELETE FROM Bans WHERE IP=@0", match) != 0;

				var namecol = casesensitive ? "Name" : "UPPER(Name)";
				return database.Query("DELETE FROM Bans WHERE " + namecol + "=@0", casesensitive ? match : match.ToUpper()) != 0;
			}
			catch (Exception ex)
			{
				if (exceptions)
					throw ex;
				TShock.Log.Error(ex.ToString());
			}
			return false;
		}

        /// <summary>
        /// Clears bans.
        /// </summary>
        /// <returns><c>true</c>, if bans were cleared, <c>false</c> otherwise.</returns>
		public bool ClearBans()
		{
			try
			{
				return database.Query("DELETE FROM Bans") != 0;
			}
			catch (Exception ex)
			{
				TShock.Log.Error(ex.ToString());
			}
			return false;
		}
	}

    /// <summary>
    /// Model class that represents a ban entry in the TShock database.
    /// </summary>
	public class Ban
	{
        /// <summary>
        /// Gets or sets the IP address of the ban entry.
        /// </summary>
        /// <value>The IP Address</value>
		public string IP { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
		public string Name { get; set; }

        /// <summary>
        /// Gets or sets the Client UUID of the ban
        /// </summary>
        /// <value>The UUID</value>
		public string UUID { get; set; }

        /// <summary>
        /// Gets or sets the ban reason.
        /// </summary>
        /// <value>The ban reason.</value>
		public string Reason { get; set; }

        /// <summary>
        /// Gets or sets the name of the user who added this ban entry.
        /// </summary>
        /// <value>The banning user.</value>
        public string BanningUser { get; set; }

        /// <summary>
        /// Gets or sets the UTC date of when the ban is to take effect
        /// </summary>
        /// <value>The date, which must be in UTC</value>
        public string Date { get; set; }

        /// <summary>
        /// Gets or sets the expiration date, in which the ban shall be lifted
        /// </summary>
        /// <value>The expiration.</value>
        public string Expiration { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TShockAPI.DB.Ban"/> class.
        /// </summary>
        /// <param name="ip">Ip.</param>
        /// <param name="name">Name.</param>
        /// <param name="uuid">UUID.</param>
        /// <param name="reason">Reason.</param>
        /// <param name="banner">Banner.</param>
        /// <param name="date">UTC ban date.</param>
        /// <param name="exp">Expiration time</param>
		public Ban(string ip, string name, string uuid, string reason, string banner, string date, string exp)
		{
			IP = ip;
			Name = name;
			UUID = uuid;
			Reason = reason;
		    BanningUser = banner;
		    Date = date;
		    Expiration = exp;
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="TShockAPI.DB.Ban"/> class.
        /// </summary>
		public Ban()
		{
			IP = string.Empty;
			Name = string.Empty;
			UUID = string.Empty;
			Reason = string.Empty;
		    BanningUser = "";
		    Date = "";
		    Expiration = "";
		}
	}
}
