using System.Collections.Generic;

namespace MacroKey.Keyboard
{
    class KeyboardDataEqualityComparer : IEqualityComparer<KeyboardData>
    {
        public bool Equals(KeyboardData x, KeyboardData y)
        {
            if (ReferenceEquals(null, x)) return false;
            if (ReferenceEquals(null, y)) return false;
            if (ReferenceEquals(x, y)) return true;
            return x.VirtualKeyCode.Equals(y.VirtualKeyCode) && x.KeyMessage.Equals(y.KeyMessage);
        }

        public int GetHashCode(KeyboardData obj)
        {
            return 0;
        }
    }
}
