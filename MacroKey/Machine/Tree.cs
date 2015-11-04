using System.Collections.Generic;
using System.Linq;

namespace MacroKey.Machine
{
    class Tree<KeyTypeTransition>
    {
        private List<Branch<KeyTypeTransition>> mBranchCollection = new List<Branch<KeyTypeTransition>>();
        public State<KeyTypeTransition> StartStateTree { get; private set; }

        public Tree(IEqualityComparer<KeyTypeTransition> equalityComparer = null)
        {
            StartStateTree = new State<KeyTypeTransition>(equalityComparer);
        }

        public Tree(IEnumerable<Branch<KeyTypeTransition>> branch, IEqualityComparer<KeyTypeTransition> equalityComparer = null) : this(equalityComparer)
        {
            SetBranch(branch);
        }

        public void SetBranch(IEnumerable<Branch<KeyTypeTransition>> branch)
        {
            mBranchCollection = branch.ToList();
            StartStateTree.ClearNextStates();
            AddBranch(branch);
        }

        public void AddBranch(Branch<KeyTypeTransition> branch)
        {
            mBranchCollection.Add(branch);
            StartStateTree.AddNextState(branch.StartBranchState);
        }

        public void AddBranch(IEnumerable<Branch<KeyTypeTransition>> branch)
        {
            mBranchCollection.AddRange(branch);
            foreach (var item in branch)
                StartStateTree.AddNextState(item.StartBranchState);
        }

        public void RemoveBranch(Branch<KeyTypeTransition> branch)
        {
            if(!mBranchCollection.Remove(branch))
                throw new BranchNotExistTreeException("Tree does not exist , this branch");
            SetBranch(mBranchCollection);
        }

        public void RemoveBranch(int index)
        {
            mBranchCollection.RemoveAt(index);
            SetBranch(mBranchCollection);
        }

        public void ClearTree()
        {
            StartStateTree.ClearNextStates();
        }
    }
}
