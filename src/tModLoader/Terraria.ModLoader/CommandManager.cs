﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Terraria.ModLoader
{
	//todo: further documentation
	/// <summary>
	/// This serves as the central class from which ModCommand functions are supported and carried out.
	/// </summary>
	public static class CommandManager
	{
		internal static readonly IDictionary<string, List<ModCommand>> Commands = new Dictionary<string, List<ModCommand>>(StringComparer.OrdinalIgnoreCase);

		public static bool Matches(CommandType commandType, CommandType callerType) {
			if ((commandType & CommandType.World) != 0)
				if (Main.netMode == 2)
					commandType |= CommandType.Server;
				else if (Main.netMode == 0)
					commandType |= CommandType.Chat;

			return (callerType & commandType) != 0;
		}

		internal static void Add(ModCommand cmd)
		{
			List<ModCommand> cmdList;
			if (!Commands.TryGetValue(cmd.Command, out cmdList))
				Commands.Add(cmd.Command, cmdList = new List<ModCommand>());

			cmdList.Add(cmd);
		}

		internal static void Unload()
		{
			Commands.Clear();
		}
		/// <summary>
		/// Finds a command by name. Handles mod prefixing. Replies with error messages.
		/// </summary>
		/// <param name="mc">The found command, or null if an error was encountered.</param>
		/// <returns>True if a ModCommand was found, or an error message was replied. False if the command is unrecognised.</returns>
		internal static bool GetCommand(CommandCaller caller, string name, out ModCommand mc)
		{
			string modName = null;
			if (name.Contains(':'))
			{
				var split = name.Split(':');
				modName = split[0];
				name = split[1];
			}

			mc = null;

			List<ModCommand> cmdList;
			if (!Commands.TryGetValue(name, out cmdList))
				return false;

			cmdList = cmdList.Where(c => Matches(c.Type, caller.CommandType)).ToList();
			if (cmdList.Count == 0)
				return false;

			if (modName != null)
			{
				Mod mod = ModLoader.GetMod(modName);
				if (mod == null)
				{
					caller.Reply("Unknown Mod: " + modName, Color.Red);
				}
				else
				{
					mc = cmdList.SingleOrDefault(c => c.mod == mod);
					if (mc == null)
						caller.Reply("Mod: " + modName + " does not have a " + name + " command.", Color.Red);
				}
			}
			else if (cmdList.Count > 1)
			{
				caller.Reply("Multiple definitions of command /" + name + ". Try:", Color.Red);
				foreach (var c in cmdList)
					caller.Reply(c.mod.Name + ":" + c.Command, Color.LawnGreen);
			}
			else
			{
				mc = cmdList[0];
			}
			return true;
		}

		internal static bool HandleCommand(string input, CommandCaller caller)
		{
			var args = input.TrimEnd().Split(' ');
			var name = args[0];
			args = args.Skip(1).ToArray();

			if (caller.CommandType != CommandType.Console)
			{
				if (name[0] != '/')
					return false;

				name = name.Substring(1);
			}

			ModCommand mc;
			if (!GetCommand(caller, name, out mc))
				return false;

			if (mc == null)//error in command name (multiple commands or missing mod etc)
				return true;

			try
			{
				mc.Action(caller, input, args);
			}
			catch (Exception e)
			{
				var ue = e as UsageException;
				if (ue?.msg != null)
					caller.Reply(ue.msg, ue.color);
				else
					caller.Reply("Usage: "+mc.Usage, Color.Red);
			}
			return true;
		}

		public static List<Tuple<string, string>> GetHelp(CommandType type)
		{
			var list = new List<Tuple<string, string>>();
			foreach (var entry in Commands)
			{
				var cmdList = entry.Value.Where(mc => Matches(mc.Type, type)).ToList();
				foreach (var mc in cmdList)
				{
					string cmd = mc.Command;
					if (cmdList.Count > 1)
						cmd = mc.mod.Name + ":" + cmd;

					list.Add(new Tuple<string, string>(cmd, mc.Description));
				}
			}
			return list;
		}
	}
}
