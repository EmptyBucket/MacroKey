using System.Collections.Generic;

namespace MacroKey.Machine
{
    interface IState<KeyTypeTransition>
    {
        Dictionary<KeyTypeTransition, State<KeyTypeTransition>> NextState { get; set; }

        void AddNextState(State<KeyTypeTransition> state);

        void RemoveNextState(State<KeyTypeTransition> removeState);

        void ClearNextStates();
    }
}
