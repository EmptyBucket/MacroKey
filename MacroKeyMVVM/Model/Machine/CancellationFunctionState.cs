using System.Collections.Generic;
using System.Threading;
using MacroKey.Machine;

namespace MacroKeyMVVM.Model.Machine
{
    public abstract class CancellationFunctionState<T> : State<T>
    {
        private object FunctionArg { get; }

        public CancellationFunctionState(IEqualityComparer<T> equalityComparer) : base(equalityComparer) { }

        public CancellationFunctionState(object functionArg, IEqualityComparer<T> equalityComparer = null) : base(equalityComparer)
        {
            FunctionArg = functionArg;
        }

        protected abstract object Function(object arg, CancellationToken cancellationToken);

        public object ExecuteFunction(CancellationToken cancellationToken) => Function(FunctionArg, cancellationToken);
    }
}
