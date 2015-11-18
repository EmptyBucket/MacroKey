using System;
using System.Collections.Generic;
using System.Linq;

namespace MacroKey.Machine
{
    [Serializable]
    public class Branch<T> : State<T>
    {
        public State<T> LastBranchState { get; private set; }
        private State<T> mPenultimateState;
        private IEqualityComparer<T> mEqualityComparer;

        public Branch(IEqualityComparer<T> equalityComparer = null) : base(equalityComparer)
        {
            LastBranchState = this;
            mPenultimateState = this;
            mEqualityComparer = equalityComparer;
        }

        public Branch(IEnumerable<T> keys, IEqualityComparer<T> equalityComparer = null) : this(equalityComparer)
        {
            foreach (var item in keys)
            {
                mPenultimateState = LastBranchState;
                State<T> newState = new State<T>(equalityComparer);
                LastBranchState.SetNextState(item, newState);
                LastBranchState = newState;
            }
        }

        public Branch(IEnumerable<T> keys, FunctionState<T> functionState, IEqualityComparer<T> equalityComparer = null) : this(equalityComparer)
        {
            if (keys.Count() == 0)
            {
                SetFunctionBranch(functionState);
            }
            else
            {
                foreach (var item in keys)
                {
                    mPenultimateState = LastBranchState;
                    State<T> newState = new State<T>(equalityComparer);
                    LastBranchState.SetNextState(item, newState);
                    LastBranchState = newState;
                }
                LastBranchState = functionState;
                mPenultimateState.SetNextState(mPenultimateState.NextState.Keys.First(), LastBranchState);
            }
        }

        public Branch(IEnumerable<T> keys, IEnumerable<FunctionState<T>> functionStateEnum, IEqualityComparer<T> equalityComparer = null) : this(equalityComparer)
        {
            if (keys.Count() == 0)
            {
                SetFunctionBranch(functionStateEnum.First());
            }
            else
            {
                var keysAndFunctionsAndArgs = keys.Zip(functionStateEnum, (key, functionState) => new { Key = key, FunctionState = functionState });
                foreach (var item in keysAndFunctionsAndArgs)
                {
                    mPenultimateState = LastBranchState;
                    FunctionState<T> newState = item.FunctionState;
                    LastBranchState.SetNextState(item.Key, newState);
                    LastBranchState = newState;
                }
            }
        }

        public void AdditionBranch(Branch<T> branch)
        {
            mPenultimateState.SetNextState(mPenultimateState.NextState.Keys.First(), branch);
            LastBranchState = branch.LastBranchState;
        }

        public void SetFunctionBranch(FunctionState<T> functionState)
        {
            LastBranchState = functionState;
            mPenultimateState.SetNextState(mPenultimateState.NextState.Keys.First(), LastBranchState);
        }

        public void AddState(State<T> state)
        {
            T key = mPenultimateState.NextState.Keys.First();
            mPenultimateState.NextState[key] = state;
        }
    }
}
