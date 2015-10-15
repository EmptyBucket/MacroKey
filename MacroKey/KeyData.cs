using System;
using System.Windows.Input;

namespace MacroKey
{
    public enum KeyState { WM_KEYDOWM = 0x100, WM_KEYUP = 0x101, WM_CHAR = 0x102, WM_DEADCHAR = 0x103, WM_SYSKEYDOWN = 0x104, WM_SYSKEYUP = 0x105, WM_SYSCHAR = 0x0106 }
    public class KeyData : IEquatable<KeyData>
    {
        public string KeyValue { get; }
        public KeyState KeyState { get; }

        public override bool Equals(object other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (this.GetType() != other.GetType())
                return false;

            return Equals((KeyData)other);
        }

        public bool Equals(KeyData other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return this.KeyValue.Equals(other.KeyValue) && this.KeyState.Equals(other.KeyState);
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public KeyData(Key key, KeyState keyState)
        {
            KeyValue = key.ToString();
            KeyState = keyState;
        }
    }
}
