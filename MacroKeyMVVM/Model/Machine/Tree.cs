using System;
using System.Collections.Generic;

namespace MacroKey.Machine
{
    [Serializable]
    public class Tree<KeyTypeTransition> : State<KeyTypeTransition>
    {
        private List<State<KeyTypeTransition>> mStateCollection = new List<State<KeyTypeTransition>>();

        public Tree(IEqualityComparer<KeyTypeTransition> equalityComparer = null) : base(equalityComparer) { }

        public Tree(IEnumerable<State<KeyTypeTransition>> stateEnumerable, IEqualityComparer<KeyTypeTransition> equalityComparer = null) : this(equalityComparer)
        {
            SetState(stateEnumerable);
        }

        public void SetState(IEnumerable<State<KeyTypeTransition>> stateEnumerable)
        {
            ClearTree();
            AddRangeState(stateEnumerable);
        }

        public void AddState(State<KeyTypeTransition> state)
        {
            mStateCollection.Add(state);
            AddNextState(state);
        }

        public void AddRangeState(IEnumerable<State<KeyTypeTransition>> stateEnumerable)
        {
            mStateCollection.AddRange(stateEnumerable);
            foreach (var item in stateEnumerable)
                AddNextState(item);
        }

        public void RemoveState(State<KeyTypeTransition> state)
        {
            if(!mStateCollection.Remove(state))
                throw new StateNotExistTreeException("Tree does not exist this state");
            SetState(mStateCollection);
        }

        public void RemoveAtState(int index)
        {
            mStateCollection.RemoveAt(index);
            SetState(mStateCollection);
        }

        public void ClearTree()
        {
            ClearNextStates();
            mStateCollection.Clear();
        }
    }
}
