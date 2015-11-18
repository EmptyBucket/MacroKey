using System;
using System.Collections.Generic;
using MacroKey.Machine;

namespace MacroKey.Model.Machine
{
    [Serializable]
    public class ReturnFalseFuctionState<T> : FunctionState<T>
    {
        public ReturnFalseFuctionState(IEqualityComparer<T> equalityComparer) : base(equalityComparer) { }

        public ReturnFalseFuctionState(object arg, IEqualityComparer<T> equalityComparer = null) : base(arg, equalityComparer) { } 

        public override object Function(object arg)
        {
            return false;
        }
    }
}
