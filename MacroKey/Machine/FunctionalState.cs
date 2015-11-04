using System;
using System.Collections.Generic;

namespace MacroKey.Machine
{
    class FunctionalState<KeyTypeTransition> : State<KeyTypeTransition>
    {
        public Func<object, object> FunctionState { get; set; }
        public object FunctionArg { get; set; }

        public FunctionalState(Func<object, object> function, object functionArg = null, IEqualityComparer<KeyTypeTransition> equalityComparer = null) : base(equalityComparer)
        {
            FunctionState = function;
            FunctionArg = functionArg;
        }
    }
}
