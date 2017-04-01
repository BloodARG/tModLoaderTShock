﻿using System.Linq;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace Terraria.ModLoader.Default
{
	internal class HelpCommand : ModCommand
	{
		public override string Command => "help";
		public override string Usage => "/help [name]";
		public override CommandType Type => CommandType.Chat | CommandType.Server;
		public override void Action(CommandCaller caller, string input, string[] args)
		{
			if (args.Length > 0)
			{
				ModCommand mc;
				if (!CommandManager.GetCommand(caller, args[0], out mc))
				{
					throw new UsageException("Unknown command: "+args[0], Color.Red);
				}
				if (mc != null)
				{
					caller.Reply(mc.Usage);
					if (!string.IsNullOrEmpty(mc.Description))
						caller.Reply(mc.Description);
				}
				return;
			}

			var help = CommandManager.GetHelp(caller.CommandType);
			caller.Reply(caller.CommandType+" Commands:", Color.Yellow);

			foreach (var entry in help)
				caller.Reply(entry.Item1 + "   " + entry.Item2);

			if (Main.netMode == 1)
			{
				//send the command to the server
				NetMessage.SendData(MessageID.ChatText, -1, -1, input);
			}
		}
	}
}
