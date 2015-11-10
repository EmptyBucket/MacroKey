using System;
using System.Collections.Generic;
using System.Linq;

namespace MacroKey.Machine
{
    public class Branch<KeyTypeTransition> : State<KeyTypeTransition>
    {
        public State<KeyTypeTransition> LastBranchState { get; private set; }
        private State<KeyTypeTransition> mPenultimateState;
        private IEqualityComparer<KeyTypeTransition> mEqualityComparer;

        public Branch(IEqualityComparer<KeyTypeTransition> equalityComparer = null) : base(equalityComparer)
        {
            LastBranchState = this;
            mPenultimateState = this;
            mEqualityComparer = equalityComparer;
        }

        public Branch(IEnumerable<KeyTypeTransition> keys, IEqualityComparer<KeyTypeTransition> equalityComparer = null) : this(equalityComparer)
        {
            foreach (var item in keys)
            {
                mPenultimateState = LastBranchState;
                State<KeyTypeTransition> newState = new State<KeyTypeTransition>(equalityComparer);
                LastBranchState.SetNextState(item, newState);
                LastBranchState = newState;
            }
        }

        public Branch(IEnumerable<KeyTypeTransition> keys, Func<object, object> function, IEqualityComparer<KeyTypeTransition> equalityComparer = null) : this(keys, function, null, equalityComparer) { }

        public Branch(IEnumerable<KeyTypeTransition> keys, Func<object, object> function, object functionArg, IEqualityComparer<KeyTypeTransition> equalityComparer = null) : this(equalityComparer)
        {
            if (keys.Count() == 0)
            {
                SetFunctionBranch(function, functionArg);
            }
            else
            {
                foreach (var item in keys)
                {
                    mPenultimateState = LastBranchState;
                    State<KeyTypeTransition> newState = new State<KeyTypeTransition>(equalityComparer);
                    LastBranchState.SetNextState(item, newState);
                    LastBranchState = newState;
                }
                LastBranchState = new FunctionalState<KeyTypeTransition>(function, functionArg, equalityComparer);
                mPenultimateState.SetNextState(mPenultimateState.NextState.Keys.First(), LastBranchState);
            }
        }

        public Branch(IEnumerable<KeyTypeTransition> keys, IEnumerable<Func<object, object>> functions, IEqualityComparer<KeyTypeTransition> equalityComparer = null) : this(keys, functions, null, equalityComparer) { }

        public Branch(IEnumerable<KeyTypeTransition> keys, IEnumerable<Func<object, object>> functions, IEnumerable<object> functionsArg = null, IEqualityComparer<KeyTypeTransition> equalityComparer = null) : this(equalityComparer)
        {
            if (keys.Count() == 0)
            {
                SetFunctionBranch(functions.First(), functionsArg.First());
            }
            else
            {
                if (functionsArg == null)
                    functionsArg = Enumerable.Repeat<object>(null, functions.Count());
                var functionsAndArgs = functions.Zip(functionsArg, (function, arg) => new { Function = function, Arg = arg });
                var keysAndFunctionsAndArgs = keys.Zip(functionsAndArgs, (key, functionAndArg) => new { Key = key, Function = functionAndArg.Function, Arg = functionAndArg.Arg });
                foreach (var item in keysAndFunctionsAndArgs)
                {
                    mPenultimateState = LastBranchState;
                    FunctionalState<KeyTypeTransition> newState = new FunctionalState<KeyTypeTransition>(item.Function, functionsArg, equalityComparer);
                    LastBranchState.SetNextState(item.Key, newState);
                    LastBranchState = newState;
                }
            }
        }

        public void AdditionBranch(Branch<KeyTypeTransition> branch)
        {
            mPenultimateState.SetNextState(mPenultimateState.NextState.Keys.First(), branch);
            LastBranchState = branch.LastBranchState;
        }

        public void SetFunctionBranch(Func<object, object> function, object functionArg = null)
        {
            LastBranchState = new FunctionalState<KeyTypeTransition>(function, functionArg, mEqualityComparer);
            mPenultimateState.SetNextState(mPenultimateState.NextState.Keys.First(), LastBranchState);
        }

        public void AddState(State<KeyTypeTransition> state)
        {
            KeyTypeTransition key = mPenultimateState.NextState.Keys.First();
            mPenultimateState.NextState[key] = state;
        }
    }
}
