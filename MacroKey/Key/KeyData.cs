using System;
using System.Windows.Input;

namespace MacroKey.Key
{

    public class KeyData
    {
        public enum KeyboardMessage { WM_KEYDOWM = 0x100, WM_KEYUP = 0x101, WM_CHAR = 0x102, WM_DEADCHAR = 0x103, WM_SYSKEYDOWN = 0x104, WM_SYSKEYUP = 0x105, WM_SYSCHAR = 0x0106 }

        public short VirtualKeyCode { get; }
        public int Time { get; }
        public KeyboardMessage KeyMessage { get; }
        public string KeyValue
        {
            get
            {
                return KeyInterop.KeyFromVirtualKey(VirtualKeyCode).ToString();
            }
        }

        public KeyData(short virtualKeyCode, int keyboardMessage, int time)
        {
            VirtualKeyCode = virtualKeyCode;
            Time = time;
            KeyMessage = (KeyboardMessage)keyboardMessage;
        }
    }
}
