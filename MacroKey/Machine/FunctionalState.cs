using System;
using System.Collections.Generic;

namespace MacroKey.Machine
{
    class FunctionalState<KeyTypeTransition> : State<KeyTypeTransition>
    {
        public Func<object, object> Function { get; set; }
        public object FunctionArg { get; set; }

        public FunctionalState(Func<object, object> function, object functionArg = null, IEqualityComparer<KeyTypeTransition> equalityComparer = null) : base(equalityComparer)
        {
            Function = function;
            FunctionArg = functionArg;
        }

        public object ExecuteFunction()
        {
            return Function(FunctionArg);
        }
    }
}
