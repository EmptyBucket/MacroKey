using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using MacroKey.InputData;

namespace MacroKey.LowLevelApi
{
    [Serializable]
    public abstract class SenderInput : ISenderInput
    {
        [DllImport("user32.dll")]
        protected static extern uint SendInput(uint nInputs, [MarshalAs(UnmanagedType.LPArray), In] INPUT[] pInputs, int cbSize);

        public abstract void SendInput(IEnumerable<Input> input);

        [StructLayout(LayoutKind.Sequential)]
        protected struct INPUT
        {
            internal SENDINPUTEVENTTYPE mType;
            internal InputUnion mInputUnion;
            internal static int Size => Marshal.SizeOf(typeof(INPUT));
        }

        [StructLayout(LayoutKind.Explicit)]
        protected struct InputUnion
        {
            [FieldOffset(0)]
            internal MOUSEINPUT mi;
            [FieldOffset(0)]
            internal KEYBDINPUT ki;
            [FieldOffset(0)]
            internal HARDWAREINPUT hi;
        }

        [StructLayout(LayoutKind.Sequential)]
        protected struct MOUSEINPUT
        {
            internal int dx;
            internal int dy;
            internal int mouseData;
            internal MOUSEEVENTF dwFlags;
            internal int time;
            internal UIntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        protected struct HARDWAREINPUT
        {
            internal int uMsg;
            internal short wParamL;
            internal short wParamH;
        }

        [StructLayout(LayoutKind.Sequential)]
        protected struct KEYBDINPUT
        {
            internal short wVk;
            internal short wScan;
            internal KEYEVENTF dwFlags;
            internal int time;
            internal UIntPtr dwExtraInfo;
        }

        protected enum SENDINPUTEVENTTYPE
        {
            INPUT_MOUSE = 0x0000,
            INPUT_KEYBOARD = 0x0001,
            INPUT_HARDWARE = 0x0002
        }

        [Flags]
        protected enum KEYEVENTF
        {
            NONE = 0x0000,
            EXTENDEDKEY = 0x0001,
            KEYUP = 0x0002,
            SCANCODE = 0x0008,
            UNICODE = 0x0004
        }

        [Flags]
        protected enum MOUSEEVENTF
        {
            NONE = 0x0000,
            ABSOLUTE = 0x8000,
            HWHEEL = 0x01000,
            MOVE = 0x0001,
            MOVE_NOCOALESCE = 0x2000,
            LEFTDOWN = 0x0002,
            LEFTUP = 0x0004,
            RIGHTDOWN = 0x0008,
            RIGHTUP = 0x0010,
            MIDDLEDOWN = 0x0020,
            MIDDLEUP = 0x0040,
            VIRTUALDESK = 0x4000,
            WHEEL = 0x0800,
            XDOWN = 0x0080,
            XUP = 0x0100
        }
    }
}
