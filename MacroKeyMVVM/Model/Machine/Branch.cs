using System;
using System.Collections.Generic;
using System.Linq;

namespace MacroKey.Machine
{
    [Serializable]
    public class Branch<T> : State<T>
    {
        private State<T> mPenultimateState;
        private State<T> mLastBranchState;
        public State<T> LastBranchState
        {
            get
            {
                return mLastBranchState;
            }
            set
            {
                mLastBranchState = value;

                T key = mPenultimateState.NextState.Keys.FirstOrDefault();
                mPenultimateState.NextState = key != null
                ? new Dictionary<T, State<T>>(mEqualityComparer)
                {
                    [key] = LastBranchState
                }
                : NextState;
            }
        }
        private IEqualityComparer<T> mEqualityComparer;

        public Branch(IEqualityComparer<T> equalityComparer = null) : base(equalityComparer)
        {
            mPenultimateState = this;
            LastBranchState = this;
            mEqualityComparer = equalityComparer;
        }

        public Branch(IEnumerable<T> keys, IEqualityComparer<T> equalityComparer = null) : this(equalityComparer)
        {
            foreach (var item in keys)
            {
                mPenultimateState = LastBranchState;
                State<T> newState = new State<T>(equalityComparer);
                LastBranchState.NextState = new Dictionary<T, State<T>>(equalityComparer)
                {
                    [item] = newState
                };
                LastBranchState = newState;
            }
        }

        public Branch(IEnumerable<T> keys, IEnumerable<State<T>> states, IEqualityComparer<T> equalityComparer = null) : this(equalityComparer)
        {
            var keysAndStates = keys.Zip(states, (key, state) => new { Key = key, FunctionState = state });
            foreach (var item in keysAndStates)
            {
                mPenultimateState = LastBranchState;
                State<T> newState = item.FunctionState;
                LastBranchState.NextState = new Dictionary<T, State<T>>(equalityComparer)
                {
                    [item.Key] = newState
                };
                LastBranchState = newState;
            }
        }
    }
}
