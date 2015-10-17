using System;
using System.Windows.Input;

namespace MacroKey
{

    public class KeyData : IEquatable<KeyData>
    {
        public enum KeyboardMessage { WM_KEYDOWM = 0x100, WM_KEYUP = 0x101, WM_CHAR = 0x102, WM_DEADCHAR = 0x103, WM_SYSKEYDOWN = 0x104, WM_SYSKEYUP = 0x105, WM_SYSCHAR = 0x0106 }

        public short VirtualKeyCode { get; }
        public short ScanCode { get; }
        public int Flags { get; }
        public KeyboardMessage KeyMessage { get; }
        public string KeyValue
        {
            get
            {
                return KeyInterop.KeyFromVirtualKey(VirtualKeyCode).ToString();
            }
        }

        public override bool Equals(object other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (GetType() != other.GetType())
                return false;

            return Equals((KeyData)other);
        }

        public bool Equals(KeyData other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return VirtualKeyCode.Equals(other.VirtualKeyCode) && KeyMessage.Equals(other.KeyMessage);
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public KeyData(short virtualKeyCode, short scanCode, int flags, int keyboardMessage)
        {
            VirtualKeyCode = virtualKeyCode;
            ScanCode = scanCode;
            Flags = flags;
            KeyMessage = (KeyboardMessage)keyboardMessage;
        }
    }
}
