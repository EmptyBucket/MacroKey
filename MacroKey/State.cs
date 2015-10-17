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
    }
}
