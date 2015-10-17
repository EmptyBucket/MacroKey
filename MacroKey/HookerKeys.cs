using System;
using System.Runtime.InteropServices;

namespace MacroKey
{
    public class HookerKeys
    {
        public class KeyHookEventArgs : EventArgs
        {
            public short VirtualKeyCode { get; set; }
            public short ScanCode { get; set; }
            public int Flags { get; set; }
            public int KeyboardMassage { get; set; }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct KeyHookedStruct
        {
            internal short VirtualKeyCode;
            internal short ScanCode;
            internal int Flags;
            internal int Time;
            internal UIntPtr ExtraInfo;
        }


        private LowLevelKeyboardProcDelegate m_callback;

        private const int WH_KEYBOARD_LL = 13;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(
            int idHook,
            LowLevelKeyboardProcDelegate lpfn,
            IntPtr hMod, int dwThreadId);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("Kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetModuleHandle(IntPtr lpModuleName);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr CallNextHookEx(
            IntPtr hhk,
            int nCode, IntPtr wParam, IntPtr lParam);

        private delegate IntPtr LowLevelKeyboardProcDelegate(
            int nCode, IntPtr wParam, IntPtr lParam);

        private IntPtr LowLevelKeyboardHookProc(
            int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                KeyHookedStruct keyStruct = (KeyHookedStruct)Marshal.PtrToStructure(
                    lParam, typeof(KeyHookedStruct));
                int keyboardMessage = (int)wParam;

                return HookedKey(new KeyHookEventArgs() { VirtualKeyCode = keyStruct.VirtualKeyCode, ScanCode = keyStruct.ScanCode, Flags = keyStruct.Flags, KeyboardMassage = keyboardMessage })
                    ? CallNextHookEx(m_hHook, nCode, wParam, lParam)
                    : new IntPtr(1);
            }
            else
            {
                return CallNextHookEx(m_hHook, nCode, wParam, lParam);
            }
        }

        private IntPtr m_hHook;

        public delegate bool KeyHookHandler(KeyHookEventArgs e);

        public event KeyHookHandler HookedKey;

        public void SetHook()
        {
            //фильтр - колбэк, при перехвате события
            m_callback = LowLevelKeyboardHookProc;
            //дескриптор файла, в котором содержится процедура фильтра, в данном случае 0, чтобы получить дескриптор файла текущего процесса
            IntPtr deskriptorFileProc = GetModuleHandle(IntPtr.Zero);
            //идентификатор потока, с которым должен быть связан хук, в данном случае 0, чтобы связаться со всеми существующими потоками
            int idStream = 0;
            //перехватываем WH_KEYBOARD_LL - события ввода с клавиатуры
            m_hHook = SetWindowsHookEx(WH_KEYBOARD_LL,
                m_callback, deskriptorFileProc, idStream);
        }

        public void Unhook()
        {
            //удаляем процедуру фильтра с данным дескриптором из хука
            UnhookWindowsHookEx(m_hHook);
        }
    }
}
