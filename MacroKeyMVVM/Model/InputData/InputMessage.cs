using System;

namespace MacroKeyMVVM.Model.InputData
{
    [Serializable]
    public enum InputMessage
    {
        WM_KEYDOWM = 0x100,
        WM_KEYUP = 0x101,
        WM_CHAR = 0x102,
        WM_DEADCHAR = 0x103,
        WM_SYSKEYDOWN = 0x104,
        WM_SYSKEYUP = 0x105,
        WM_SYSCHAR = 0x0106,

        WM_MOUSEMOVE = 0x0200,
        WM_LBUTTONDOWN = 0x0201,
        WM_LBUTTONUP = 0x0202,
        WM_RBUTTONDOWN = 0x0204,
        WM_RBUTTONUP = 0x0205,
        WM_MBUTTONDOWN = 0x207,
        WM_MBUTTONUP = 0x208,
        WM_MOUSEWHEEL = 0x020A,
        WM_MOUSEHWHEEL = 0x020E
    }
}