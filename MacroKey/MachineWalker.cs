using System.Collections.Generic;

namespace MacroKey
{
    class MachineWalker<KeyTypeTransition>
    {
        private Machine<KeyTypeTransition> mMachine;
        private State<KeyTypeTransition> mStartState;
        public State<KeyTypeTransition> StartState
        {
            set
            {
                mStartState = value;
                mCurrentState = mStartState;
            }
            get
            {
                return mStartState;
            }
        }
        private State<KeyTypeTransition> mCurrentState { get; set; }

        public MachineWalker(Machine<KeyTypeTransition> machine)
        {
            mMachine = machine;
            StartState = machine.StartState;
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
