using System;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace MacroKey
{
    class HookerKeys
    {
        public delegate bool KeyHookHandler(Key key, KeyState keyState);

        private KeyHookHandler m_keyHookHandler;
        LowLevelKeyboardProcDelegate m_callback;

        public HookerKeys() { }

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
                Key key = KeyInterop.KeyFromVirtualKey(Marshal.ReadInt32(lParam));
                KeyState keyState = (KeyState)wParam;

                return m_keyHookHandler(key, keyState)
                    ? CallNextHookEx(m_hHook, nCode, wParam, lParam)
                    : new IntPtr(1);
            }
            else
            {
                return CallNextHookEx(m_hHook, nCode, wParam, lParam);
            }
        }

        private IntPtr m_hHook;

        public void SetHook(KeyHookHandler keyHookHandler)
        {
            m_keyHookHandler = keyHookHandler;
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
