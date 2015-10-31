using System.Collections.Generic;
using System.Linq;

namespace MacroKey
{
    class Machine<KeyTypeTransition>
    {
        public State<KeyTypeTransition> StartState { get; }
             
        public Machine(State<KeyTypeTransition> startState)
        {
            StartState = startState;
        }

        public void RemoveBranchFromStart(KeyTypeTransition key)
        {
            StartState.RemoveState(key);
        }

        public void RemoveBranchFromCurrent(KeyTypeTransition key, State<KeyTypeTransition> currentState)
        {
            currentState.RemoveState(key);
        }

        public void RemoveBranchFromStart(State<KeyTypeTransition> state)
        {
            List<KeyTypeTransition> toRemove = StartState.NextState.Keys.Where(key => state.NextState.Keys.Contains(key)).ToList();

            foreach (var item in toRemove)
                StartState.RemoveState(item);
        }

        public void RemoveBranchFromCurrent(State<KeyTypeTransition> state, State<KeyTypeTransition> currentState)
        {
            List<KeyTypeTransition> toRemove = currentState.NextState.Keys.Where(key => state.NextState.Keys.Contains(key)).ToList();

            foreach (var item in toRemove)
                currentState.RemoveState(item);
        }

        public void ClearBranchFromCurrent(State<KeyTypeTransition> currentState)
        {
            RemoveBranchFromCurrent(currentState, currentState);
        }

        public void ClearBranchFromStartState()
        {
            RemoveBranchFromStart(StartState);
        }

        public void AddBranchToStart(State<KeyTypeTransition> state)
        {
            AddMachineState(state, StartState);
        }

        public void AddBranchToCurrent(State<KeyTypeTransition> state, State<KeyTypeTransition> currentState)
        {
            AddMachineState(state, currentState);
        }

        private void AddMachineState(State<KeyTypeTransition> state, State<KeyTypeTransition> oldState)
        {
            foreach (var key in state.NextState.Keys)
            {
                if (oldState.NextState.ContainsKey(key))
                {
                    AddMachineState(state.NextState[key], oldState.NextState[key]);
                }
                else
                {
                    oldState.AddNextState(key, state.NextState[key]);
                }
            }
        }
    }
}
