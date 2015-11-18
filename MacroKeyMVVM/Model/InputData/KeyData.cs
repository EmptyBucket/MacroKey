using System;

namespace MacroKey.InputData
{
    [Serializable]
    public class KeyData : Input
    {
        public enum KeyMessage { WM_KEYDOWM = 0x100, WM_KEYUP = 0x101, WM_CHAR = 0x102, WM_DEADCHAR = 0x103, WM_SYSKEYDOWN = 0x104, WM_SYSKEYUP = 0x105, WM_SYSCHAR = 0x0106 }

        public short VirtualKeyCode { get; }
        public int Time { get; }
        public KeyMessage Message { get; }

        public KeyData(short virtualKeyCode, int keyMessage, int time)
        {
            VirtualKeyCode = virtualKeyCode;
            Time = time;
            Message = (KeyMessage)keyMessage;
        }
    }
}
