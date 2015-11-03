using System;

namespace MacroKey.Machine
{
    class BranchNotExistTreeException : Exception
    {
        public BranchNotExistTreeException() { }

        public BranchNotExistTreeException(string message) : base(message) { }
    }
}
