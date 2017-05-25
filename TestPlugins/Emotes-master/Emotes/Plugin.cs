using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Terraria;
using Terraria.GameContent.UI;

using TerrariaApi.Server;
using TShockAPI;

namespace Emotes
{
	[ApiVersion(1, 25)]
	public class Plugin : TerrariaPlugin
	{
		internal string SavePath { get { return Path.Combine(TShock.SavePath, "Emotes.json"); } }
		internal Config config = new Config();
		internal Dictionary<Regex, EmoteRegex> Regexes = new Dictionary<Regex, EmoteRegex>();

		private Command _emoteCmd;

		public override string Author
		{
			get
			{
				return "White";
			}
		}

		public override string Description
		{
			get
			{
				return "Displays Terraria emotes when specific text is sent";
			}
		}

		public override string Name
		{
			get
			{
				return "Emotes";
			}
		}

		public override Version Version
		{
			get
			{
				return new Version(1, 0);
			}
		}

		public Plugin(Main game) : base(game)
		{
		}

		public override void Initialize()
		{
			if (!File.Exists(SavePath))
			{
				AddDefaultsToConfig();
				config.Write(SavePath);
			}
			config.Read(SavePath);

			foreach (EmoteRegex regex in config.Emotes)
			{
				Regexes.Add(new Regex(regex.ToString()), regex);
			}

			_emoteCmd = new Command("", EmoteCallback, "emotes")
			{
				HelpDesc = config.HelpText
			};

			ServerApi.Hooks.ServerChat.Register(this, OnChat, 6);
			TShockAPI.Hooks.GeneralHooks.ReloadEvent += OnReload;
            Commands.ChatCommands.Add(_emoteCmd);
		}

		private void EmoteCallback(CommandArgs e)
		{
			foreach (string line in config.HelpText)
			{
				e.Player.SendInfoMessage(line);
			}
		}

		private void OnReload(TShockAPI.Hooks.ReloadEventArgs e)
		{
			if (!File.Exists(SavePath))
			{
				AddDefaultsToConfig();
				config.Write(SavePath);
			}
			config.Read(SavePath);

			Regexes.Clear();

			foreach (EmoteRegex regex in config.Emotes)
			{
				Regexes.Add(new Regex(regex.ToString()), regex);
			}

			Commands.ChatCommands.Remove(_emoteCmd);
			_emoteCmd.HelpDesc = config.HelpText;
			Commands.ChatCommands.Add(_emoteCmd);
		}

		private void AddDefaultsToConfig()
		{
			config.Emotes.Add(
				new EmoteRegex("Happy", @"[:;=][,.'^o-]?[)D]", "happy,smile,laugh,lol", EmoteID.EmoteLaugh));

			config.Emotes.Add(
				new EmoteRegex("Heart", @"<3", "love,<3", EmoteID.EmotionLove));

			config.Emotes.Add(
				new EmoteRegex("Kissy", @"[:;=][,.'^o-]?\*", "kiss", EmoteID.EmoteKiss));

			config.Emotes.Add(
				new EmoteRegex("Sleepy", @"[zZ]{3,5}", "sleep,sleepy,zzz", EmoteID.EmoteSleep));

			config.Emotes.Add(
				new EmoteRegex("Confused", @"\?{2,5}", "confused,???", EmoteID.EmoteConfused));

			config.Emotes.Add(
				new EmoteRegex("Angry", @"[D>\]][:;=][,.'^0-]?[\(<\/]", "angry,grumpy,anger", EmoteID.EmotionAnger));

			config.Emotes.Add(
				new EmoteRegex("Cry", @"(?>[:=][,'][\(\/])|(?>[T;][-_][T;])", "cry,sad", EmoteID.EmotionCry));

			config.Emotes.Add(
				new EmoteRegex("Rip", @"^[Rr][Ii][Pp]$", "rip", EmoteID.ItemTombstone));
		}

		private void OnChat(ServerChatEventArgs args)
		{
			TSPlayer player = TShock.Players[args.Who];
			if (player == null)
			{
				return;
			}

			//Check if the provided text matches any of the defined regexes
			Regex regex = Regexes.Keys.FirstOrDefault(r => r.IsMatch(args.Text));

			if (regex == null)
			{
				return;
			}

			Match match = regex.Match(args.Text);

			int ID = EmoteBubble.AssignNewID();

			if (!String.IsNullOrEmpty(match.Groups["spoken"].Value))
			{
				args.Handled = true;
			}

			NetMessage.SendData(91, -1, -1, "", ID, 1, args.Who, 600, Regexes[regex].EmoteID);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				ServerApi.Hooks.ServerChat.Deregister(this, OnChat);
				TShockAPI.Hooks.GeneralHooks.ReloadEvent -= OnReload;
			}
			base.Dispose(disposing);
		}
	}
}
