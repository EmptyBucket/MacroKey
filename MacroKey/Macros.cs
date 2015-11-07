using System.Collections.Generic;
using MacroKey.Keyboard;

namespace MacroKey
{
    public class Macros
    {
        public string Name { get; }
        public ObservableKeyboardDataCollection Macro { get; } = new ObservableKeyboardDataCollection();
        public ObservableKeyboardDataCollection Sequence { get; } = new ObservableKeyboardDataCollection();

        public Macros(string name, IEnumerable<KeyboardData> sequence, IEnumerable<KeyboardData> macros)
        {
            Name = name;
            Sequence = new ObservableKeyboardDataCollection(sequence);
            Macro = new ObservableKeyboardDataCollection(macros);
        }
    }
}
