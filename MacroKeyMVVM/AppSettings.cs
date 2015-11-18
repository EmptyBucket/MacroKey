using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MacroKey.InputData;
using MacroKey.Machine;

namespace MacroKey
{
    [Serializable]
    public class AppSettings
    {
        public Tree<KeyData> TreeRoot { get; set; }
        public Tree<KeyData> TreeSequence { get; set; }
        public Branch<KeyData> GUIBranch { get; set; }
        public Branch<KeyData> MacrosModeBranch { get; set; }
        public IEnumerable<KeyData> SequenceGUI { get; set; }
        public IEnumerable<KeyData> SequenceMacrosMode { get; set; }
        public ObservableCollection<Macros> MacrosCollection { get; set; }
        public ObservableCollection<KeyData> Sequence { get; set; }
        public ObservableCollection<KeyDataDelay> Macro { get; set; }

        public AppSettings()
        {
            TreeRoot = new Tree<KeyData>();
            TreeSequence = new Tree<KeyData>();
            GUIBranch = new Branch<KeyData>();
            MacrosModeBranch = new Branch<KeyData>();
            SequenceGUI = new List<KeyData>();
            SequenceMacrosMode = new List<KeyData>();
            MacrosCollection = new ObservableCollection<Macros>();
            Sequence = new ObservableCollection<KeyData>();
            Macro = new ObservableCollection<KeyDataDelay>();
        }

        public AppSettings(KeyDataEqualityComparer equalityComparer) : this()
        {
            TreeRoot = new Tree<KeyData>(equalityComparer);
            TreeSequence = new Tree<KeyData>(equalityComparer);
            GUIBranch = new Branch<KeyData>(equalityComparer);
            MacrosModeBranch = new Branch<KeyData>(equalityComparer);
        }
    }
}
 