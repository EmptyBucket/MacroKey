﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace MacroKey.Machine
{
    [Serializable]
    public class State<KeyTypeTransition> : IState<KeyTypeTransition>
    {
        public Dictionary<KeyTypeTransition, State<KeyTypeTransition>> NextState { get; private set; }

        public State(IEqualityComparer<KeyTypeTransition> equalityComparer = null)
        {
            if (equalityComparer == null)
                equalityComparer = EqualityComparer<KeyTypeTransition>.Default;
            NextState = new Dictionary<KeyTypeTransition, State<KeyTypeTransition>>(equalityComparer);
        }

        public State(State<KeyTypeTransition> state)
        {
            State<KeyTypeTransition> copyState = new State<KeyTypeTransition>(state.NextState.Comparer);
            state.MemberwiseClone();
        }

        public void SetNextState(KeyTypeTransition key, State<KeyTypeTransition> state)
        {
            NextState[key] = state;
        }

        public void SetNextState(Dictionary<KeyTypeTransition, State<KeyTypeTransition>> nextStates)
        {
            NextState = nextStates;
        }

        public void AddNextState(State<KeyTypeTransition> addState)
        {
            var keys = addState.NextState.Keys.ToList();
            foreach (var key in keys)
                if (this.NextState.ContainsKey(key))
                    this.NextState[key].AddNextState(addState.NextState[key]);
                else
                    this.NextState.Add(key, addState.NextState[key]);
        }

        public void RemoveNextState(State<KeyTypeTransition> removeState)
        {
            if (this.NextState.Count != 0 && removeState.NextState.Count == 0)
            {
                return;
            }
            foreach (var key in removeState.NextState.Keys.ToArray())
            {
                State<KeyTypeTransition> NextRemoveState = removeState.NextState[key];
                State<KeyTypeTransition> NextTreeState = this.NextState[key];
                if (NextTreeState.NextState.Count == 0 && NextRemoveState.NextState.Count == 0)
                    this.NextState.Remove(key);
                else
                {
                    NextTreeState.RemoveNextState(NextRemoveState);
                    if (NextTreeState.NextState.Count == 0)
                        this.NextState.Remove(key);
                }
            }
        }

        public void ClearNextStates()
        {
            NextState.Clear();
        }
    }
}
