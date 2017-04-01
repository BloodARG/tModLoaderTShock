using Ionic.Zip;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Terraria
{
	public class TexturePackSupport
	{
		public static bool Enabled = false;
		public static int ReplacedTextures = 0;
		private static List<ZipFile> texturePacks = new List<ZipFile>();
		private static Dictionary<string, ZipEntry> entries = new Dictionary<string, ZipEntry>();
		private static Stopwatch test = new Stopwatch();

		public static bool FetchTexture(string path, out Texture2D tex)
		{
			ZipEntry zipEntry;
			if (TexturePackSupport.entries.TryGetValue(path, out zipEntry))
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					zipEntry.Extract(memoryStream);
					tex = TexturePackSupport.FromStreamSlow(Main.instance.GraphicsDevice, memoryStream);
					TexturePackSupport.ReplacedTextures++;
					return true;
				}
			}
			tex = null;
			return false;
		}

		public static Texture2D FromStreamSlow(GraphicsDevice graphicsDevice, Stream stream)
		{
			Texture2D texture2D = Texture2D.FromStream(graphicsDevice, stream);
			Color[] array = new Color[texture2D.Width * texture2D.Height];
			texture2D.GetData<Color>(array);
			for (int num = 0; num != array.Length; num++)
			{
				array[num] = Color.FromNonPremultiplied(array[num].ToVector4());
			}
			texture2D.SetData<Color>(array);
			return texture2D;
		}

		/*
		public static void FindTexturePack()
		{
			string savePath = Main.SavePath;
			string path = savePath + "/Texture Pack.zip";
			if (!File.Exists(path))
			{
				return;
			}
			TexturePackSupport.entries.Clear();
			TexturePackSupport.texturePack = ZipFile.Read(File.OpenRead(path));
			foreach (ZipEntry current in TexturePackSupport.texturePack.Entries)
			{
				TexturePackSupport.entries.Add(current.FileName.Replace("/", "\\"), current);
			}
		}
		*/

		// TODO: Implement a Priority System (similar to Mods UI menu), save priority? For now, alphabetical, so later in alphabet actually takes priority. Or, we can ignore entries that already exist in dictionary to keep it A to Z
		public static void FindTexturePacks()
		{
			string texturePacksPath = Path.Combine(Main.SavePath, "TexturePacks");
			if (Enabled)
			{
				Directory.CreateDirectory(texturePacksPath);
			}

			TexturePackSupport.entries.Clear();
			TexturePackSupport.texturePacks.Clear();
			if (Directory.Exists(texturePacksPath))
			{
				List<string> files = Directory.GetFiles(texturePacksPath).ToList();
				files.Sort();
				foreach (string fileName in files)
				{
					ZipFile zipFile = ZipFile.Read(File.OpenRead(fileName));
					TexturePackSupport.texturePacks.Add(zipFile);
					foreach (ZipEntry current in zipFile.Entries)
					{
						TexturePackSupport.entries.Add(current.FileName.Replace("/", "\\"), current);
					}
				}
			}
		}
	}
}
