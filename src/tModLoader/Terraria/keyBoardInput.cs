#if !WINDOWS
using Microsoft.Xna.Framework.Input;
#endif
using System;

#if WINDOWS
using System.Runtime.InteropServices;
using System.Windows.Forms;

#else
using System.Threading;
#endif
namespace Terraria
{
	public class keyBoardInput
	{
		#if WINDOWS
		public class inKey : IMessageFilter
		{
			public bool PreFilterMessage(ref Message m)
			{
				if (m.Msg == 258)
				{
					char c = (char)((int)m.WParam);
					Console.WriteLine(c);
					if (keyBoardInput.newKeyEvent != null)
					{
						keyBoardInput.newKeyEvent(c);
					}
				}
				else if (m.Msg == 256)
				{
					IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(m));
					Marshal.StructureToPtr(m, intPtr, true);
					keyBoardInput.TranslateMessage(intPtr);
				}
				return false;
			}
		}
		public static bool slashToggle;
		public static event Action<char> newKeyEvent;
		[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
		public static extern bool TranslateMessage(IntPtr message);

#else
		public static event Action<char> newKeyEvent;
		#endif
		static keyBoardInput()
		{
#if WINDOWS
			keyBoardInput.slashToggle = true;
			Application.AddMessageFilter(new keyBoardInput.inKey());
#else
			TextInputEXT.TextInput += delegate(char character)
			{
				keyBoardInput.onTextInput(character);
			};
#endif
		}
		#if !WINDOWS
		private static void onTextInput(char key)
		{
			keyBoardInput.newKeyEvent(key);
		}
		#endif
	}
}
