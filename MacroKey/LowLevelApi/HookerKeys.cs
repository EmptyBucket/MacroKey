using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MacroKey
{
    public class HookerKeys
    {
        public class KeyHookEventArgs : EventArgs
        {
            public short VirtualKeyCode { get; set; }
            public int Time { get; set; }
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


        private LowLevelKeyboardProcDelegate mCallback;

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

                KeyHookEventArgs keyHookEventArgs = new KeyHookEventArgs()
                {
                    VirtualKeyCode = keyStruct.VirtualKeyCode,
                    KeyboardMassage = keyboardMessage,
                    Time = keyStruct.Time
                };

                Debug.Print(keyHookEventArgs.Time.ToString());

                return OnHookedKey(keyHookEventArgs)
                    ? CallNextHookEx(mHook, nCode, wParam, lParam)
                    : new IntPtr(1);
            }
            else
            {
                return CallNextHookEx(mHook, nCode, wParam, lParam);
            }
        }

        protected virtual bool OnHookedKey(KeyHookEventArgs e)
        {
            var handler = HookedKey;
            if (handler != null)
                return handler(e);
            else
                return true;
        }

        private IntPtr mHook;

        public delegate bool KeyHookHandler(KeyHookEventArgs e);

        public event KeyHookHandler HookedKey;

        public void SetHook()
        {
            //фильтр - колбэк, при перехвате события
            mCallback = LowLevelKeyboardHookProc;
            //дескриптор файла, в котором содержится процедура фильтра, в данном случае 0, чтобы получить дескриптор файла текущего процесса
            IntPtr deskriptorFileProc = GetModuleHandle(IntPtr.Zero);
            //идентификатор потока, с которым должен быть связан хук, в данном случае 0, чтобы связаться со всеми существующими потоками
            int idStream = 0;
            //перехватываем WH_KEYBOARD_LL - события ввода с клавиатуры
            mHook = SetWindowsHookEx(WH_KEYBOARD_LL,
                mCallback, deskriptorFileProc, idStream);
        }

        public void Unhook()
        {
            //удаляем процедуру фильтра с данным дескриптором из хука
            UnhookWindowsHookEx(mHook);
        }
    }
}
