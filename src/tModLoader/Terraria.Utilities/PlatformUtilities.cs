#if !WINDOWS
using SDL2;
#endif
using System;
using System.IO;

#if WINDOWS
using System.Threading;
using System.Windows.Forms;

#else
using System.Runtime.InteropServices;
#endif
namespace Terraria.Utilities
{
	public static class PlatformUtilities
	{
		#if !WINDOWS
		private struct SDL_Surface
		{
			private uint flags;
			public IntPtr format;
			public int w;
			public int h;
			private int pitch;
			public IntPtr pixels;
			private IntPtr userdata;
			private int locked;
			private IntPtr lock_data;
			private SDL.SDL_Rect clip_rect;
			private IntPtr map;
			private int refcount;
		}
		#endif
		#if WINDOWS
		public const bool IsXNA = true;
		public const bool IsFNA = false;
		public const bool IsWindows = true;

#else
		public const bool IsXNA = false;
		public const bool IsFNA = true;
		public const bool IsWindows = false;
		#endif
		#if MAC
		public const bool IsOSX = true;

#else
		public const bool IsOSX = false;
		#endif
		#if LINUX
		public const bool IsLinux = true;

#else
		public const bool IsLinux = false;
		#endif

		public static string GetClipboard()
		{
#if WINDOWS
			string clipboardText = "";
			Thread thread = new Thread((ThreadStart)delegate
			{
				clipboardText = Clipboard.GetText();
			});
			thread.SetApartmentState(ApartmentState.STA);
			thread.Start();
			thread.Join();
			char[] array = new char[clipboardText.Length];
			int length = 0;
			for (int i = 0; i < clipboardText.Length; i++)
			{
				if (clipboardText[i] >= ' ' && clipboardText[i] != '\u007f')
				{
					array[length++] = clipboardText[i];
				}
			}
#else
			string empty = string.Empty;
			char[] array = new char[empty.Length];
			int length = 0;
			for (int i = 0; i < empty.Length; i++)
			{
				if (empty[i] >= ' ' && empty[i] != '\u007f')
				{
					array[length++] = empty[i];
				}
			}
#endif
			return new string(array, 0, length);
		}

		public static void SetClipboard(string text)
		{
#if WINDOWS
			Thread thread = new Thread((ThreadStart)delegate
			{
				if (text.Length > 0)
				{
					Clipboard.SetText(text);
				}
			});
			thread.SetApartmentState(ApartmentState.STA);
			thread.Start();
			thread.Join();
#endif
		}

		public static string GetStoragePath()
		{
#if WINDOWS
			string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "My Games");
			path = Path.Combine(path, "Terraria");
			return Path.Combine(path, "ModLoader");
#endif
#if MAC
			string text = Environment.GetEnvironmentVariable("HOME");
			if (string.IsNullOrEmpty(text))
			{
				return ".";
			}
			text += "/Library/Application Support";
			text = Path.Combine(text, "Terraria");
			return Path.Combine(text, "ModLoader");
#endif
#if LINUX
			string text = Environment.GetEnvironmentVariable("XDG_DATA_HOME");
 			if (string.IsNullOrEmpty(text))
 			{
				text = Environment.GetEnvironmentVariable("HOME");
				if (string.IsNullOrEmpty(text))
				{
					return ".";
				}
				text += "/.local/share";
 			}
 			text = Path.Combine(text, "Terraria");
			return Path.Combine(text, "ModLoader");
#endif
		}
		#if !WINDOWS
		public unsafe static void SavePng(Stream stream, int width, int height, int imgWidth, int imgHeight, byte[] data)
		{
			IntPtr intPtr = SDL.SDL_CreateRGBSurface(0u, imgWidth, imgHeight, 32, 255u, 65280u, 16711680u, 4278190080u);
			SDL.SDL_LockSurface(intPtr);
			PlatformUtilities.SDL_Surface* ptr = (PlatformUtilities.SDL_Surface*)((void*)intPtr);
			Marshal.Copy(data, 0, ptr->pixels, width * height * 4);
			SDL.SDL_UnlockSurface(intPtr);
			data = null;
			if (width != imgWidth || height != imgHeight)
			{
				IntPtr intPtr2 = SDL.SDL_CreateRGBSurface(0u, width, height, 32, 255u, 65280u, 16711680u, 4278190080u);
				SDL.SDL_BlitScaled(intPtr, IntPtr.Zero, intPtr2, IntPtr.Zero);
				SDL.SDL_FreeSurface(intPtr);
				intPtr = intPtr2;
			}
			byte[] array = new byte[width * height * 4 + 41 + 57 + 256];
			IntPtr dst = SDL.SDL_RWFromMem(array, array.Length);
			SDL_image.IMG_SavePNG_RW(intPtr, dst, 1);
			SDL.SDL_FreeSurface(intPtr);
			int count = ((int)array[33] << 24 | (int)array[34] << 16 | (int)array[35] << 8 | (int)array[36]) + 41 + 57;
			stream.Write(array, 0, count);
		}
		#endif
	}
}
