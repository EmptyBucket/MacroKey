namespace MacroKey
{
    class Machine<KeyTypeTransition>
    {
        public State<KeyTypeTransition> StartState { get; }

        public Machine(State<KeyTypeTransition> startState)
        {
            StartState = startState;
        }

        public void AddBranch(State<KeyTypeTransition> state)
        {
            AddMachineState(state, StartState);
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
