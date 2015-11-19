using System;
using System.Collections.ObjectModel;
using MacroKey.InputData;
using MacroKey.Machine;
using MacroKeyMVVM.Model.InputData;

namespace MacroKey
{
    [Serializable]
    public class AppSettings
    {
        public Tree<Input> TreeRoot { get; set; }
        public Tree<Input> TreeSequence { get; set; }
        public ObservablePropertyCollection<Input> SequenceGUI { get; set; }
        public ObservablePropertyCollection<Input> SequenceMacrosMode { get; set; }
        public ObservableCollection<Macros> MacrosCollection { get; set; }
        public ObservableCollection<Input> Sequence { get; set; }
        public ObservableCollection<InputDelay> Macro { get; set; }

        public AppSettings()
        {
            TreeRoot = new Tree<Input>();
            TreeSequence = new Tree<Input>();

            SequenceGUI = new ObservablePropertyCollection<Input>();
            SequenceMacrosMode = new ObservablePropertyCollection<Input>();
            Sequence = new ObservableCollection<Input>();
            Macro = new ObservableCollection<InputDelay>();
            MacrosCollection = new ObservableCollection<Macros>();
        }

        public AppSettings(InputEqualityComparer equalityComparer) : this()
        {
            TreeRoot = new Tree<Input>(equalityComparer);
            TreeSequence = new Tree<Input>(equalityComparer);
        }
    }
}
 