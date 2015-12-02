using System;
using System.Runtime.InteropServices;
using MacroKey.InputData;
using MacroKeyMVVM.Model.InputData.Keyboard;

namespace MacroKey.LowLevelApi.Hook
{
    public class KeyHooker : Hooker
    {
        private enum KeyMessage
        {
            WM_KEYDOWM = 0x100,
            WM_KEYUP = 0x101,
            WM_CHAR = 0x102,
            WM_DEADCHAR = 0x103,
            WM_SYSKEYDOWN = 0x104,
            WM_SYSKEYUP = 0x105,
            WM_SYSCHAR = 0x0106,
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct KeyHookedStruct
        {
            public short VirtualKeyCode;
            public short ScanCode;
            public int Flags;
            public int Time;
            public UIntPtr ExtraInfo;
        }

        private const int WH_KEYBOARD_LL = 13;

        private LowLevelKeyProcDelegate mCallbackKeyboard;

        private IntPtr LowLevelKeyboardHookProc(
            int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                KeyHookedStruct keyStruct = (KeyHookedStruct)Marshal.PtrToStructure(lParam, typeof(KeyHookedStruct));
                KeyMessage keyboardMessage = (KeyMessage)wParam;

                KeyStates state;
                switch (keyboardMessage)
                {
                    case KeyMessage.WM_KEYDOWM:
                    case KeyMessage.WM_SYSKEYDOWN:
                        state = KeyStates.KeyDown;
                        break;
                    case KeyMessage.WM_KEYUP:
                    case KeyMessage.WM_SYSKEYUP:
                        state = KeyStates.KeyUp;
                        break;
                    case KeyMessage.WM_CHAR:
                    case KeyMessage.WM_DEADCHAR:
                    case KeyMessage.WM_SYSCHAR:
                    default:
                        state = KeyStates.KeyQuest;
                        break;
                }

                KeyboardData keyData = new KeyboardData(keyStruct.VirtualKeyCode, state);

                return OnHooked(keyData) ? CallNextHookEx(mKeyboardHook, nCode, wParam, lParam) : new IntPtr(1);
            }
            else
                return CallNextHookEx(mKeyboardHook, nCode, wParam, lParam);
        }

        private IntPtr mKeyboardHook;

        public override void SetHook()
        {
            //фильтр - колбэк, при перехвате события
            mCallbackKeyboard = LowLevelKeyboardHookProc;
            //дескриптор файла, в котором содержится процедура фильтра, в данном случае 0, чтобы получить дескриптор файла текущего процесса
            IntPtr deskriptorFileProc = GetModuleHandle(IntPtr.Zero);
            //идентификатор потока, с которым должен быть связан хук, в данном случае 0, чтобы связаться со всеми существующими потоками
            int idStream = 0;
            //перехватываем WH_KEYBOARD_LL - события ввода с клавиатуры
            mKeyboardHook = SetWindowsHookEx(WH_KEYBOARD_LL, mCallbackKeyboard, deskriptorFileProc, idStream);
        }

        public override void Unhook()
        {
            //удаляем процедуру фильтра с данным дескриптором из хука
            UnhookWindowsHookEx(mKeyboardHook);
        }
    }
}
