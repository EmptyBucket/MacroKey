using System;
using System.Runtime.InteropServices;
using System.Windows;
using MacroKeyMVVM.Model.InputData;
using MacroKeyMVVM.Model.InputData.Mouse;

namespace MacroKey.LowLevelApi.Hook
{
    public class MouseHooker : Hooker
    {
        private enum MouseMessage
        {
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

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MouseHookedStruck
        {
            public POINT pt;
            public int mouseData;
            public int flags;
            public int time;
            public UIntPtr dwExtraInfo;
        }

        private const int WH_MOUSE_LL = 14;

        private LowLevelKeyProcDelegate mCallbackMouse;

        private IntPtr LowLevelMouseHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                MouseHookedStruck mouseHookedStruct = (MouseHookedStruck)Marshal.PtrToStructure(lParam, typeof(MouseHookedStruck));
                MouseMessage mouseMessage = (MouseMessage)wParam;

                if (mouseMessage == MouseMessage.WM_MOUSEMOVE)
                    return CallNextHookEx(mMouseHook, nCode, wParam, lParam);
                else
                {
                    MouseStates state;
                    MouseCodes keyMouse;
                    switch (mouseMessage)
                    {
                        case MouseMessage.WM_LBUTTONDOWN:
                            keyMouse = MouseCodes.LeftMouse;
                            state = MouseStates.MouseDown;
                            break;
                        case MouseMessage.WM_RBUTTONDOWN:
                            keyMouse = MouseCodes.RightMouse;
                            state = MouseStates.MouseDown;
                            break;
                        case MouseMessage.WM_MBUTTONDOWN:
                            keyMouse = MouseCodes.MidleMouse;
                            state = MouseStates.MouseDown;
                            break;
                        case MouseMessage.WM_LBUTTONUP:
                            keyMouse = MouseCodes.LeftMouse;
                            state = MouseStates.MouseUp;
                            break;
                        case MouseMessage.WM_RBUTTONUP:
                            keyMouse = MouseCodes.RightMouse;
                            state = MouseStates.MouseUp;
                            break;
                        case MouseMessage.WM_MBUTTONUP:
                            keyMouse = MouseCodes.MidleMouse;
                            state = MouseStates.MouseUp;
                            break;
                        case MouseMessage.WM_MOUSEWHEEL:
                        case MouseMessage.WM_MOUSEHWHEEL:
                            if (mouseHookedStruct.mouseData > 0)
                                state = MouseStates.MouseWheelUp;
                            else
                                state = MouseStates.MouseWheelDown;
                            keyMouse = MouseCodes.WheelMouse;
                            break;
                        case MouseMessage.WM_MOUSEMOVE:
                        default:
                            keyMouse = MouseCodes.Quest;
                            state = MouseStates.MouseQuest;
                            break;
                    }

                    MouseData mouseData = new MouseData(new Point(mouseHookedStruct.pt.x, mouseHookedStruct.pt.y), keyMouse, state);

                    return OnHooked(mouseData) ? CallNextHookEx(mMouseHook, nCode, wParam, lParam) : new IntPtr(1);
                }
            }
            else
                return CallNextHookEx(mMouseHook, nCode, wParam, lParam);
        }

        private IntPtr mMouseHook;

        public override void SetHook()
        {
            //фильтр - колбэк, при перехвате события
            mCallbackMouse = LowLevelMouseHookProc;
            //дескриптор файла, в котором содержится процедура фильтра, в данном случае 0, чтобы получить дескриптор файла текущего процесса
            IntPtr deskriptorFileProc = GetModuleHandle(IntPtr.Zero);
            //идентификатор потока, с которым должен быть связан хук, в данном случае 0, чтобы связаться со всеми существующими потоками
            int idStream = 0;
            //перехватываем WH_MOUSE_LL - события ввода с клавиатуры
            mMouseHook = SetWindowsHookEx(WH_MOUSE_LL, mCallbackMouse, deskriptorFileProc, idStream);
        }

        public override void Unhook()
        {
            //удаляем процедуру фильтра с данным дескриптором из хука
            UnhookWindowsHookEx(mMouseHook);
        }
    }
}
