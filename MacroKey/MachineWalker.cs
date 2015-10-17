using System.Collections.Generic;

namespace MacroKey
{
    class MachineWalker<KeyTypeTransition>
    {
        private Machine<KeyTypeTransition> m_machine;
        private State<KeyTypeTransition> m_currentState;

        public MachineWalker(Machine<KeyTypeTransition> machine)
        {
            m_machine = machine;
            m_currentState = machine.StartState;
        }

        public State<KeyTypeTransition> WalkMachine(KeyTypeTransition key)
        {
            try
            {
                m_currentState = m_currentState.NextState[key];
                return m_currentState;
            }
            catch (KeyNotFoundException)
            {
                return null;            }
        }

        public State<KeyTypeTransition> WalkMachine(KeyTypeTransition[] keys)
        {
            try
            {
                foreach (var key in keys)
                {
                    m_currentState = m_currentState.NextState[key];
                }
                return m_currentState;
            }
            catch (KeyNotFoundException)
            {
                return null;
            }
        }
    }
}
