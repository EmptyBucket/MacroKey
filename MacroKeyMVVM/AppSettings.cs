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
        public Tree<IInput> TreeRoot { get; set; }
        public Tree<IInput> TreeSequence { get; set; }
        public ObservablePropertyCollection<IInput> SequenceGUI { get; set; }
        public ObservablePropertyCollection<IInput> SequenceMacrosMode { get; set; }
        public ObservableCollection<Macros> MacrosCollection { get; set; }
        public ObservableCollection<IInput> Sequence { get; set; }
        public ObservableCollection<InputDelay> Macro { get; set; }

        public AppSettings()
        {
            TreeRoot = new Tree<IInput>();
            TreeSequence = new Tree<IInput>();

            SequenceGUI = new ObservablePropertyCollection<IInput>();
            SequenceMacrosMode = new ObservablePropertyCollection<IInput>();
            Sequence = new ObservableCollection<IInput>();
            Macro = new ObservableCollection<InputDelay>();
            MacrosCollection = new ObservableCollection<Macros>();
        }

        public AppSettings(InputEqualityComparer equalityComparer) : this()
        {
            TreeRoot = new Tree<IInput>(equalityComparer);
            TreeSequence = new Tree<IInput>(equalityComparer);
        }
    }
}
 