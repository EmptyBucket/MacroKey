using System;
using System.Collections.Generic;
using System.Linq;

namespace MacroKey.Machine
{
    [Serializable]
    public class State<KeyTypeTransition> : IState<KeyTypeTransition>
    {
        public Dictionary<KeyTypeTransition, State<KeyTypeTransition>> NextState { get; set; }

        public State(IEqualityComparer<KeyTypeTransition> equalityComparer = null)
        {
            NextState = new Dictionary<KeyTypeTransition, State<KeyTypeTransition>>(equalityComparer ?? EqualityComparer<KeyTypeTransition>.Default);
        }

        public State(State<KeyTypeTransition> state)
        {
            State<KeyTypeTransition> copyState = new State<KeyTypeTransition>(state.NextState.Comparer);
            state.MemberwiseClone();
        }

        public void AddNextState(State<KeyTypeTransition> addState)
        {
            var keys = addState.NextState.Keys.ToList();
            foreach (var key in keys)
                if (NextState.ContainsKey(key))
                    NextState[key].AddNextState(addState.NextState[key]);
                else
                    NextState.Add(key, addState.NextState[key]);
        }

        public void RemoveNextState(State<KeyTypeTransition> removeState)
        {
            if (NextState.Count != 0 && removeState.NextState.Count == 0)
            {
                return;
            }
            foreach (var key in removeState.NextState.Keys.ToArray())
            {
                State<KeyTypeTransition> NextRemoveState = removeState.NextState[key];
                State<KeyTypeTransition> NextTreeState = NextState[key];
                if (NextTreeState.NextState.Count == 0 && NextRemoveState.NextState.Count == 0)
                    NextState.Remove(key);
                else
                {
                    NextTreeState.RemoveNextState(NextRemoveState);
                    if (NextTreeState.NextState.Count == 0)
                        NextState.Remove(key);
                }
            }
        }

        public void ClearNextStates() => NextState.Clear();
    }
}
