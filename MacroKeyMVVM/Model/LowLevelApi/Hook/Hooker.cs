using System;
using System.Runtime.InteropServices;

namespace MacroKey.LowLevelApi.Hook
{
    public class HookEventArgs : EventArgs { }

    public delegate bool HookEventHandler(HookEventArgs e);

    public abstract class Hooker : IHooker
    {
        [DllImport("user32.dll", SetLastError = true)]
        protected static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyProcDelegate lpfn, IntPtr hMod, int dwThreadId);

        [DllImport("user32.dll", SetLastError = true)]
        protected static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("Kernel32.dll", SetLastError = true)]
        protected static extern IntPtr GetModuleHandle(IntPtr lpModuleName);

        [DllImport("user32.dll", SetLastError = true)]
        protected static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        protected delegate IntPtr LowLevelKeyProcDelegate(int nCode, IntPtr wParam, IntPtr lParam);

        public abstract void SetHook();

        public abstract void Unhook();

        public event HookEventHandler Hooked;

        protected bool OnHooked(HookEventArgs e)
        {
            var handler = Hooked;
            if (handler != null)
                return handler(e);
            else
                return true;
        }
    }
}
