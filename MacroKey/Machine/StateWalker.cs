using System.Collections.Generic;

namespace MacroKey.Machine
{
    class StateWalker<KeyTypeTransition>
    {
        private State<KeyTypeTransition> mStartState;
        private State<KeyTypeTransition> mCurrentState;

        public StateWalker(State<KeyTypeTransition> startState)
        {
            mStartState = startState;
            mCurrentState = mStartState;
        }

        public State<KeyTypeTransition> WalkStates(KeyTypeTransition key)
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

        public State<KeyTypeTransition> WalkStates(KeyTypeTransition[] keys)
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
