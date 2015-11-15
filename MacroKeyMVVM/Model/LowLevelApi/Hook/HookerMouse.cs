using System;
using System.Runtime.InteropServices;

namespace MacroKey.LowLevelApi.Hook
{
    public class MouseHookEventArgs : HookEventArgs
    {
        public int MouseData { get; set; }
        public int Time { get; set; }
        public int MouseMessage { get; set; }
    }

    public class HookerMouse : Hooker
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public long x;
            public long y;
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
                int mouseMessage = (int)wParam;

                MouseHookEventArgs mouseHookEventArgs = new MouseHookEventArgs()
                {
                    MouseData = mouseHookedStruct.mouseData,
                    MouseMessage = mouseMessage,
                    Time = mouseHookedStruct.time
                };

                return OnHooked(mouseHookEventArgs) ? CallNextHookEx(mMouseHook, nCode, wParam, lParam) : new IntPtr(1);
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
