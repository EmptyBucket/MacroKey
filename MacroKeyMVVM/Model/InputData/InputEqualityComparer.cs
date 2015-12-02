using System;
using System.Collections.Generic;

namespace MacroKey.InputData
{
    [Serializable]
    public class InputEqualityComparer : IEqualityComparer<IInput>
    {
        public bool Equals(IInput x, IInput y)
        {
            if (ReferenceEquals(null, x)) return false;
            if (ReferenceEquals(null, y)) return false;
            if (ReferenceEquals(x, y)) return true;
            return x.Key.Equals(y.Key) && x.State.Equals(y.State);
        }

        public int GetHashCode(IInput obj)
        {
            return 0;
        }
    }
}
