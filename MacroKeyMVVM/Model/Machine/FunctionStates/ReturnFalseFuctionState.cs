using System;
using System.Collections.Generic;
using MacroKey.Machine;

namespace MacroKey.Model.Machine
{
    [Serializable]
    public class ReturnFalseFuctionState<T> : FunctionState<T>
    {
        public ReturnFalseFuctionState(IEqualityComparer<T> equalityComparer) : base(equalityComparer) { }

        protected override object Function(object arg) => false;
    }
}
