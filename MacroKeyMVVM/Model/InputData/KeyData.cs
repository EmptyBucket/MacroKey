using System;

namespace MacroKey.InputData
{
    public enum KeyMessage { WM_KEYDOWM = 0x100, WM_KEYUP = 0x101, WM_CHAR = 0x102, WM_DEADCHAR = 0x103, WM_SYSKEYDOWN = 0x104, WM_SYSKEYUP = 0x105, WM_SYSCHAR = 0x0106 }

    [Serializable]
    public class KeyData : Input
    {
        public int VirtualCode { get; }
        public int Time { get; }
        public int Message { get; }

        public KeyData(int virtualKeyCode, int keyMessage, int time)
        {
            VirtualCode = virtualKeyCode;
            Time = time;
            Message = keyMessage;
        }
    }
}
