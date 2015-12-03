using System.Collections.Generic;
using System.Threading;
using MacroKey.Machine;

namespace MacroKeyMVVM.Model.Machine.CancellationFunctionalStates
{
    public class CancelState<T> : FunctionState<T>
    {
        private CancellationTokenSource mCancellationTokenSource;
        public CancellationToken CancelToken => mCancellationTokenSource.Token;

        public CancelState(IEqualityComparer<T> equalityComparer) : base(equalityComparer)
        {
            mCancellationTokenSource = new CancellationTokenSource();
        }

        protected override object Function(object arg)
        {
            mCancellationTokenSource.Cancel();
            mCancellationTokenSource = new CancellationTokenSource();
            return false;
        }
    }
}
