using System;
using System.Collections.Generic;
using System.Linq;

namespace MacroKey
{
    class State<KeyTypeTransition>
    {
        public Dictionary<KeyTypeTransition, State<KeyTypeTransition>> NextState { get; }
        public Func<object, object> ActionState { get; set; }
        public object ActionArg { get; set; }

        public State(Func<object, object> action = null)
        {
            ActionState = action;
            NextState = new Dictionary<KeyTypeTransition, State<KeyTypeTransition>>();
        }

        public void AddNextState(KeyTypeTransition value, State<KeyTypeTransition> state)
        {
            NextState.Add(value, state);
        }

        public bool RemoveState(KeyTypeTransition value)
        {
            return NextState.Remove(value);
        }

        public static State<KeyTypeTransition> CreateBranch(IEnumerable<KeyTypeTransition> keys)
        {
            State<KeyTypeTransition> startState = new State<KeyTypeTransition>();
            return CreateBranch(keys, startState);
        }

        public static State<KeyTypeTransition> CreateBranch(IEnumerable<KeyTypeTransition> keys, State<KeyTypeTransition> startState)
        {
            State<KeyTypeTransition> currentState = startState;
            foreach (var item in keys)
            {
                State<KeyTypeTransition> newState = new State<KeyTypeTransition>();
                currentState.AddNextState(item, newState);
                currentState = newState;
            }
            return currentState;
        }

        public static State<KeyTypeTransition> CreateBranch(IEnumerable<KeyTypeTransition> keys, IEnumerable<Func<object, object>> functions)
        {
            State<KeyTypeTransition> startState = new State<KeyTypeTransition>();
            return CreateBranch(keys, startState, functions);
        }

        public static State<KeyTypeTransition> CreateBranch(IEnumerable<KeyTypeTransition> keys, State<KeyTypeTransition> startState, IEnumerable<Func<object, object>> functions)
        {
            State<KeyTypeTransition> currentState = startState;
            var keysAndFunctions = keys.Zip(functions, (key, function) => new { Key = key, Function = function });
            foreach (var item in keysAndFunctions)
            {
                State<KeyTypeTransition> newState = new State<KeyTypeTransition>(item.Function);
                currentState.AddNextState(item.Key, newState);
                currentState = newState;
            }
            return currentState;
        }
    }
}
