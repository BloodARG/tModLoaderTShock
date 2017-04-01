using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Terraria.Graphics
{
	public static class TextureManager
	{
		private struct LoadPair
		{
			public string Path;
			public Ref<Texture2D> TextureRef;

			public LoadPair(string path, Ref<Texture2D> textureRef)
			{
				this.Path = path;
				this.TextureRef = textureRef;
			}
		}

		private static ConcurrentDictionary<string, Texture2D> _textures = new ConcurrentDictionary<string, Texture2D>();
		private static ConcurrentQueue<TextureManager.LoadPair> _loadQueue = new ConcurrentQueue<TextureManager.LoadPair>();
		private static Thread _loadThread;
		private static readonly object _loadThreadLock = new object();
		public static Texture2D BlankTexture;

		public static void Initialize()
		{
			TextureManager.BlankTexture = new Texture2D(Main.graphics.GraphicsDevice, 4, 4);
		}

		public static Texture2D Load(string name)
		{
			if (!TextureManager._textures.ContainsKey(name))
			{
				Texture2D texture2D = TextureManager.BlankTexture;
#if CLIENT
				if (name != "" && name != null)
				{
					try
					{
						texture2D = Main.instance.OurLoad<Texture2D>(name);
					}
					catch (Exception)
					{
						texture2D = TextureManager.BlankTexture;
					}
				}
#endif
				TextureManager._textures[name] = texture2D;
				return texture2D;
			}
			return TextureManager._textures[name];
		}

		public static Ref<Texture2D> Retrieve(string name)
		{
			Texture2D value = TextureManager.Load(name);
			return new Ref<Texture2D>(value);
		}

		public static void Run(object context)
		{
			bool looping = true;
			Main.instance.Exiting += delegate(object obj, EventArgs args)
			{
				looping = false;
				if (Monitor.TryEnter(TextureManager._loadThreadLock))
				{
					Monitor.Pulse(TextureManager._loadThreadLock);
					Monitor.Exit(TextureManager._loadThreadLock);
				}
			};
			Monitor.Enter(TextureManager._loadThreadLock);
			while (looping)
			{
				if (TextureManager._loadQueue.Count != 0)
				{
					TextureManager.LoadPair loadPair;
					if (TextureManager._loadQueue.TryDequeue(out loadPair))
					{
						loadPair.TextureRef.Value = TextureManager.Load(loadPair.Path);
					}
				}
				else
				{
					Monitor.Wait(TextureManager._loadThreadLock);
				}
			}
			Monitor.Exit(TextureManager._loadThreadLock);
		}
	}
}
