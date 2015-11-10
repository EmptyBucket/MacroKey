using System.Collections.Generic;
using System.Linq;

namespace MacroKey.Machine
{
    public class Tree<KeyTypeTransition>
    {
        private List<State<KeyTypeTransition>> mStateCollection = new List<State<KeyTypeTransition>>();
        public State<KeyTypeTransition> StartStateTree { get; private set; }

        public Tree(IEqualityComparer<KeyTypeTransition> equalityComparer = null)
        {
            StartStateTree = new State<KeyTypeTransition>(equalityComparer);
        }

        public Tree(IEnumerable<State<KeyTypeTransition>> stateEnumerable, IEqualityComparer<KeyTypeTransition> equalityComparer = null) : this(equalityComparer)
        {
            SetPart(stateEnumerable);
        }

        public void SetPart(IEnumerable<State<KeyTypeTransition>> stateEnumerable)
        {
            mStateCollection = stateEnumerable.ToList();
            SetPat();
        }

        public void SetPart(IEnumerable<Branch<KeyTypeTransition>> branchEnumerable)
        {
            mStateCollection = branchEnumerable.Select(branch => branch.StartBranchState).ToList();
            SetPat();
        }

        private void SetPat()
        {
            StartStateTree.ClearNextStates();
            AddRangePart(mStateCollection);
        }

        public void AddPart(State<KeyTypeTransition> state)
        {
            mStateCollection.Add(state);
            StartStateTree.AddNextState(state);
        }

        public void AddPart(Branch<KeyTypeTransition> branch)
        {
            mStateCollection.Add(branch.StartBranchState);
            StartStateTree.AddNextState(branch.StartBranchState);
        }

        public void AddRangePart(IEnumerable<State<KeyTypeTransition>> stateEnumerable)
        {
            mStateCollection.AddRange(stateEnumerable);
            foreach (var item in stateEnumerable)
                StartStateTree.AddNextState(item);
        }

        public void AddRangePart(IEnumerable<Branch<KeyTypeTransition>> branchEnumerable)
        {
            IEnumerable<State<KeyTypeTransition>> stateEnumerabe = branchEnumerable.Select(branch => branch.StartBranchState);
            mStateCollection.AddRange(stateEnumerabe);
            foreach (var item in stateEnumerabe)
                StartStateTree.AddNextState(item);
        }

        public void RemoveState(State<KeyTypeTransition> state)
        {
            if(!mStateCollection.Remove(state))
                throw new BranchNotExistTreeException("Tree does not exist , this branch");
            SetPart(mStateCollection);
        }

        public void RemovePart(Branch<KeyTypeTransition> branch)
        {
            if (!mStateCollection.Remove(branch.StartBranchState))
                throw new BranchNotExistTreeException("Tree does not exist , this branch");
            SetPart(mStateCollection);
        }

        public void RemoveState(int index)
        {
            mStateCollection.RemoveAt(index);
            SetPart(mStateCollection);
        }

        public void ClearTree()
        {
            StartStateTree.ClearNextStates();
        }
    }
}
