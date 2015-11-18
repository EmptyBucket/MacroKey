using System;
using System.Collections.Generic;

namespace MacroKey.InputData
{
    [Serializable]
    public class KeyDataEqualityComparer : IEqualityComparer<KeyData>
    {
        public bool Equals(KeyData x, KeyData y)
        {
            if (ReferenceEquals(null, x)) return false;
            if (ReferenceEquals(null, y)) return false;
            if (ReferenceEquals(x, y)) return true;
            return x.VirtualKeyCode.Equals(y.VirtualKeyCode) && x.Message.Equals(y.Message);
        }

        public int GetHashCode(KeyData obj)
        {
            return 0;
        }
    }
}
