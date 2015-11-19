using System;
using System.Collections.Generic;

namespace MacroKey.InputData
{
    [Serializable]
    public class InputEqualityComparer : IEqualityComparer<Input>
    {
        public bool Equals(Input x, Input y)
        {
            if (ReferenceEquals(null, x)) return false;
            if (ReferenceEquals(null, y)) return false;
            if (ReferenceEquals(x, y)) return true;
            return x.VirtualCode.Equals(y.VirtualCode) && x.Message.Equals(y.Message);
        }

        public int GetHashCode(Input obj)
        {
            return 0;
        }
    }
}
