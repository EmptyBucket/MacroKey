using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using MacroKey.InputData;

namespace MacroKey.LowLevelApi
{
    [Serializable]
    public class SenderInput : ISenderInput
    {
        [DllImport("user32.dll")]
        protected static extern uint SendInput(uint nInputs, [MarshalAs(UnmanagedType.LPArray), In] INPUT[] pInputs, int cbSize);

        public virtual void SendInput(IEnumerable<Input> inputEnum)
        {
            INPUT[] inputArray = inputEnum.Select(item =>
            {
                INPUT input;
                if(item is KeyData)
                {
                    input = new INPUT()
                    {
                        mType = SENDINPUTEVENTTYPE.INPUT_KEYBOARD,
                        mInputUnion = new InputUnion()
                        {
                            ki = new KEYBDINPUT
                            {
                                wVk = (short)item.VirtualCode,
                                dwFlags = item.Message == (int)KeyMessage.WM_KEYUP ? KEYEVENTF.KEYUP : KEYEVENTF.NONE,
                                time = item.Time
                            }
                        }
                    };
                }
                else
                {
                    input = new INPUT()
                    {
                        mType = SENDINPUTEVENTTYPE.INPUT_MOUSE,
                        mInputUnion = new InputUnion()
                        {
                            mi = new MOUSEINPUT
                            {
                                mouseData = item.VirtualCode,
                                time = item.Time,
                            }
                        }
                    };
                }
                return input;
            }).ToArray();
            SendInput((uint)inputArray.Length, inputArray, INPUT.Size);
        }

        [StructLayout(LayoutKind.Sequential)]
        protected struct INPUT
        {
            internal SENDINPUTEVENTTYPE mType;
            internal InputUnion mInputUnion;
            internal static int Size
            {
                get { return Marshal.SizeOf(typeof(INPUT)); }
            }
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

        [Flags]
        protected enum MOUSEEVENTF : uint
        {
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
            internal IntPtr dwExtraInfo;
        }

        protected enum SENDINPUTEVENTTYPE : uint
        {
            INPUT_MOUSE = 0x0000,
            INPUT_KEYBOARD = 0x0001,
            INPUT_HARDWARE = 0x0002
        }

        [Flags]
        protected enum KEYEVENTF : uint
        {
            NONE = 0x0000,
            EXTENDEDKEY = 0x0001,
            KEYUP = 0x0002,
            SCANCODE = 0x0008,
            UNICODE = 0x0004
        }
    }
}
