namespace MacroKey.Machine
{
    interface IState<KeyTypeTransition>
    {
        void AddNextState(State<KeyTypeTransition> state);

        void RemoveNextState(State<KeyTypeTransition> removeState);

        void ClearNextStates();
    }
}
