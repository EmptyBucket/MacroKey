using System;
using System.Collections.Generic;
using MacroKey.Keyboard;
using MacroKey.Machine;

namespace MacroKey
{
    [Serializable]
    public class AppSettings
    {
        public Tree<KeyData> TreeRoot { get; set; }
        public Tree<KeyData> TreeSequence { get; set; }
        public Branch<KeyData> GUIBranch { get; set; }
        public IEnumerable<KeyData> SequenceGUI { get; set; }
        public Branch<KeyData> MacrosModeBranch { get; set; }
        public IEnumerable<KeyData> SequenceMacrosMode { get; set;}
        public IEnumerable<Macros> MacrosEnumerable { get; set; }
    }
}
