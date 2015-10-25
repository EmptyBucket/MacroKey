using System;
using System.Collections.Generic;

namespace MacroKey
{
    class State<KeyTypeTransition>
    {
        public Dictionary<KeyTypeTransition, State<KeyTypeTransition>> NextState { get; }
        public Action<object> ActionState { get; set; }
        public object ActionArg { get; set; }

        public State(Action<object> action = null)
        {
            ActionState = action;
            NextState = new Dictionary<KeyTypeTransition, State<KeyTypeTransition>>();
        }

        public void AddNextState(KeyTypeTransition value, State<KeyTypeTransition> state)
        {
            NextState.Add(value, state);
        }

        public static State<KeyTypeTransition> CreateBranch(IEnumerable<KeyTypeTransition> keys, State<KeyTypeTransition> currentState = null)
        {
            if (currentState == null)
                currentState = new State<KeyTypeTransition>();
            foreach (var item in keys)
            {
                State<KeyTypeTransition> newState = new State<KeyTypeTransition>();
                currentState.AddNextState(item, newState);
                currentState = newState;
            }
            return currentState;
        }
    }
}
