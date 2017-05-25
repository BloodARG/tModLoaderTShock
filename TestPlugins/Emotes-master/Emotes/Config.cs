using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Emotes
{
	public class EmoteRegex
	{
		private Regex RegexPatternRegex = new Regex(@"\(\?\<[a-zA-Z][\S]+\>");
		private Regex RegexEscaper = new Regex(@"([^a-zA-Z0-9_|])");
        public string UniqueName;
		public string RegexPattern;
		public string ChatCommands;
		public int EmoteID;

		public EmoteRegex(string name, string pattern, string commands, int id)
		{
			UniqueName = name;
			RegexPattern = pattern;
			ChatCommands = commands;
			EmoteID = id;
		}

		public override string ToString()
		{
			string ret = RegexPattern;
			if (!RegexPatternRegex.IsMatch(ret))
			{
				//Make sure the pattern has a grouping
				ret = $"(?<{UniqueName}>{RegexPattern})";
			}
			
			//Push the two regexes together
			string commands = ChatCommands.Replace(',', '|');
			//Escape symbols such as ?, !, and >
			commands = RegexEscaper.Replace(commands, @"\$1");

			ret = $@"{ret}|(?>\:(?<spoken>{commands})\:)";


			return ret;
		}
	}

	public class Config
	{
		public List<EmoteRegex> Emotes = new List<EmoteRegex>();
		public string[] HelpText = new string[]
		{
			"Emote plugin: say phrases to trigger emotes!"
		};

		public void Write(string path)
		{
			using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write))
			{
				Write(fs);
			}
		}

		public void Write(Stream stream)
		{
			string str = JsonConvert.SerializeObject(this, Formatting.Indented);
			using (StreamWriter sw = new StreamWriter(stream))
			{
				sw.Write(str);
			}
		}

		public void Read(string path)
		{
			if (!File.Exists(path))
				return;
			using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				Read(fs);
			}
		}

		public void Read(Stream stream)
		{
			using (StreamReader sr = new StreamReader(stream))
			{
				Config c = JsonConvert.DeserializeObject<Config>(sr.ReadToEnd());
				Emotes = c.Emotes;
			}
		}
	}
}
