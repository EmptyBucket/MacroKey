using System.Collections.Generic;

namespace MacroKey
{
    class MachineWalker<KeyTypeTransition>
    {
        private Machine<KeyTypeTransition> mMachine;
        private State<KeyTypeTransition> mStartState;
        private State<KeyTypeTransition> mCurrentState;

        public MachineWalker(Machine<KeyTypeTransition> machine)
        {
            mMachine = machine;
            mStartState = machine.StartState;
            mCurrentState = mStartState;
        }

        public State<KeyTypeTransition> WalkMachine(KeyTypeTransition key)
        {
            try
            {
                mCurrentState = mCurrentState.NextState[key];
            }
            catch (KeyNotFoundException)
            {
                mCurrentState = mStartState;
            }
            return mCurrentState;
        }

        public State<KeyTypeTransition> WalkMachine(KeyTypeTransition[] keys)
        {
            try
            {
                foreach (var key in keys)
                {
                    mCurrentState = mCurrentState.NextState[key];
                }
            }
            catch (KeyNotFoundException)
            {
                mCurrentState = mStartState;
            }
            return mCurrentState;
        }
    }
}
