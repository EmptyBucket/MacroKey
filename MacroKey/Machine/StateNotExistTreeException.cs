using System;

namespace MacroKey.Machine
{
    class StateNotExistTreeException : Exception
    {
        public StateNotExistTreeException() { }

        public StateNotExistTreeException(string message) : base(message) { }
    }
}
