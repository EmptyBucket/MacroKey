using System;
using System.Collections.Generic;
using System.Linq;

namespace MacroKey.Machine
{
    public class Branch<KeyTypeTransition>
    {
        public State<KeyTypeTransition> StartBranchState { get; private set; }
        public State<KeyTypeTransition> LastBranchState { get; private set; }
        private State<KeyTypeTransition> mPenultimateState;
        private IEqualityComparer<KeyTypeTransition> mEqualityComparer;

        public Branch(IEqualityComparer<KeyTypeTransition> equalityComparer = null)
        {
            StartBranchState = new State<KeyTypeTransition>(equalityComparer);
            LastBranchState = StartBranchState;
            mPenultimateState = StartBranchState;
            mEqualityComparer = equalityComparer;
        }

        public Branch(Branch<KeyTypeTransition> branch)
        {
            StartBranchState = branch.StartBranchState;
            mPenultimateState = branch.mPenultimateState;
            LastBranchState = branch.LastBranchState;
            mEqualityComparer = branch.mEqualityComparer;
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

        public Branch(IEnumerable<KeyTypeTransition> keys, Func<object, object> function, object functionArg = null, IEqualityComparer<KeyTypeTransition> equalityComparer = null) : this(equalityComparer)
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

        public Branch(IEnumerable<KeyTypeTransition> keys, IEnumerable<Func<object, object>> functions, IEnumerable<object> functionArg = null, IEqualityComparer<KeyTypeTransition> equalityComparer = null) : this(equalityComparer)
        {
            if (functionArg == null)
                functionArg = Enumerable.Repeat<object>(null, functions.Count());
            var functionsAndArgs = functions.Zip(functionArg, (function, arg) => new { Function = function, Arg = arg });
            var keysAndFunctionsAndArgs = keys.Zip(functionsAndArgs, (key, functionAndArg) => new { Key = key, Function = functionAndArg.Function, Arg = functionAndArg.Arg });
            foreach (var item in keysAndFunctionsAndArgs)
            {
                mPenultimateState = LastBranchState;
                FunctionalState<KeyTypeTransition> newState = new FunctionalState<KeyTypeTransition>(item.Function, functionArg, equalityComparer);
                LastBranchState.SetNextState(item.Key, newState);
                LastBranchState = newState;
            }
        }

        public void SetNextState(State<KeyTypeTransition> state)
        {
            mPenultimateState.SetNextState(mPenultimateState.NextState.Keys.First(), state);
            LastBranchState = state;
        }

        public void AdditionBranch(Branch<KeyTypeTransition> branch)
        {
            mPenultimateState.SetNextState(mPenultimateState.NextState.Keys.First(), branch.StartBranchState);
            LastBranchState = branch.LastBranchState;
        }

        public void SetFunctionBranch(Func<object, object> function, object functionArg = null)
        {
            LastBranchState = new FunctionalState<KeyTypeTransition>(function, functionArg, mEqualityComparer);
            mPenultimateState.SetNextState(mPenultimateState.NextState.Keys.First(), LastBranchState);
        }

        public static Branch<KeyTypeTransition> MergeBranches(Branch<KeyTypeTransition> oneBranch, Branch<KeyTypeTransition> twoBranch)
        {
            Branch<KeyTypeTransition> newBranch = new Branch<KeyTypeTransition>(oneBranch);
            newBranch.mPenultimateState.SetNextState(newBranch.mPenultimateState.NextState.Keys.First(), twoBranch.StartBranchState);
            newBranch.LastBranchState = twoBranch.LastBranchState;

            return newBranch;
        }
    }
}
