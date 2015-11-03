using System.Collections.Generic;
using System.Linq;

namespace MacroKey.Machine
{
    class Tree<KeyTypeTransition>
    {
        private List<Branch<KeyTypeTransition>> mBranchCollection = new List<Branch<KeyTypeTransition>>();
        public State<KeyTypeTransition> StartStateTree { get; private set; } = new State<KeyTypeTransition>();

        public Tree() { }

        public Tree(IEnumerable<Branch<KeyTypeTransition>> branch)
        {
            mBranchCollection = branch.ToList();
            AdditionTree(mBranchCollection);
        }

        public void SetBranch(IEnumerable<Branch<KeyTypeTransition>> branch)
        {
            mBranchCollection = branch.ToList();
            AdditionTree(mBranchCollection);
        }

        public void AddBranch(Branch<KeyTypeTransition> branch)
        {
            mBranchCollection.Add(branch);
            StartStateTree.AddNextState(branch.StartBranchState);
        }

        public void RemoveBranch(Branch<KeyTypeTransition> branch)
        {
            if(!mBranchCollection.Remove(branch))
                throw new BranchNotExistTreeException("Tree does not exist , this branch");
            StartStateTree.ClearNextStates();
            AdditionTree(mBranchCollection);
        }

        public void RemoveBranch(int index)
        {
            mBranchCollection.RemoveAt(index);
            StartStateTree.ClearNextStates();
            AdditionTree(mBranchCollection);
        }

        private void AdditionTree(IEnumerable<Branch<KeyTypeTransition>> branch)
        {
            foreach (var item in mBranchCollection)
                StartStateTree.AddNextState(item.StartBranchState);
        }

        public void ClearTree()
        {
            StartStateTree.ClearNextStates();
        }
    }
}
