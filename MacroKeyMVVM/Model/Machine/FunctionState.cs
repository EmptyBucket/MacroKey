using System;
using System.Collections.Generic;

namespace MacroKey.Machine
{
    [Serializable]
    public abstract class FunctionState<T> : State<T>
    {
        private object FunctionArg { get; }

        public FunctionState(IEqualityComparer<T> equalityComparer) : base(equalityComparer) { }

        public FunctionState(object functionArg, IEqualityComparer<T> equalityComparer = null) : base(equalityComparer)
        {
            FunctionArg = functionArg;
        }

        public abstract object Function(object arg);

        public object ExecuteFunction()
        {
            return Function(FunctionArg);
        }
    }
}
