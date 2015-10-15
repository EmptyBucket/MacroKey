using System.Collections.Generic;

namespace MacroKey
{
    class Machine<KeyTypeTransition>
    {
        private State<KeyTypeTransition> m_startState;

        public Machine(State<KeyTypeTransition> startState)
        {
            m_startState = startState;
        }

        public void AddBranch(State<KeyTypeTransition> state)
        {
            AddMachineState(state, m_startState);
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

        public State<KeyTypeTransition> WalkState(KeyTypeTransition[] keys)
        {
            State<KeyTypeTransition> state = m_startState;
            try
            {
                foreach (var key in keys)
                {
                    state = state.NextState[key];
                }
                return state;
            }
            catch (KeyNotFoundException)
            {
                return null;
            }
        }
    }
}
