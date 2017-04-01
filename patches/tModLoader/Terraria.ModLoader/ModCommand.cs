﻿using System;
using Microsoft.Xna.Framework;

namespace Terraria.ModLoader
{
	/// <summary>A flag enum representing context where this command operates.</summary>
	[Flags]
	public enum CommandType
	{
		/// <summary>Command can be used in Chat in SP and MP.</summary>
		Chat = 1,
		/// <summary>Command is executed by server in MP.</summary>
		Server = 2,
		/// <summary>Command can be used in server console during MP.</summary>
		Console = 4,
		/// <summary>Command can be used in Chat in SP and MP, but executes on the Server in MP. (singleplayer ? Chat : Server)</summary>
		World = 8
	}

	public interface CommandCaller
	{
		CommandType CommandType { get; }
		Player Player { get; }
		void Reply(string text, Color color = default(Color));
	}

	/// <summary>
	/// This class represents a chat or console command. Use the CommandType to specify the scope of the command.
	/// </summary>
	public abstract class ModCommand
	{
		/// <summary>The Mod this ModCommand belongs to.</summary>
		public Mod mod { get; internal set; }
		/// <summary>Internal name of this command.</summary>
		public string Name { get; internal set; }
		/// <summary>The desired text to trigger this command.</summary>
		public abstract string Command { get; }
		/// <summary>A flag enum representing context where this command operates.</summary>
		public abstract CommandType Type { get; }
		/// <summary>A short usage explanation for this command.</summary>
		public virtual string Usage => "/" + Command;
		/// <summary>A short description of this command.</summary>
		public virtual string Description => "";
		/// <summary>Autoload this command, defaults to Mod.Properties.Autoload.</summary>
		public virtual bool Autoload(ref string name) => mod.Properties.Autoload;
		/// <summary>The code that is executed when the command is triggered.</summary>
		public abstract void Action(CommandCaller caller, string input, string[] args);
	}

	public class UsageException : Exception
	{
		internal string msg;
		internal Color color = Color.Red;

		public UsageException() { }

		public UsageException(string msg)
		{
			this.msg = msg;
		}

		public UsageException(string msg, Color color)
		{
			this.msg = msg;
			this.color = color;
		}
	}

	internal class ChatCommandCaller : CommandCaller
	{
		public CommandType CommandType => CommandType.Chat;
		public Player Player => Main.player[Main.myPlayer];

		public void Reply(string text, Color color = default(Color))
		{
			if (color == default(Color))
				color = Color.White;
			foreach (var line in text.Split('\n'))
				Main.NewText(line, color.R, color.G, color.B);
		} 
	}

	internal class PlayerCommandCaller : CommandCaller
	{
		public PlayerCommandCaller(Player player)
		{
			Player = player;
		}
		public CommandType CommandType => CommandType.Server;

		public Player Player { get; }

		public void Reply(string text, Color color = default(Color))
		{
			if (color == default(Color))
				color = Color.White;
			foreach (var line in text.Split('\n'))
				NetMessage.SendData(25, Player.whoAmI, -1, line, 255, color.R, color.G, color.B);
		}
	}

	internal class ConsoleCommandCaller : CommandCaller
	{
		public CommandType CommandType => CommandType.Console;
		public Player Player => null;

		public void Reply(string text, Color color = default(Color))
		{
			foreach (var line in text.Split('\n'))
				Console.WriteLine(line);
		}
	}
}
