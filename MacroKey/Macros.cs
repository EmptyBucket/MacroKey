using System;
using System.Collections.Generic;
using MacroKey.Keyboard;

namespace MacroKey
{
    [Serializable]
    public class Macros
    {
        public string Name { get; }
        public ObservablePropertyCollection<KeyDataDelay> Macro { get; }
        public ObservablePropertyCollection<KeyData> Sequence { get; }

        public Macros()
        {
            Name = string.Empty;
            Sequence = new ObservablePropertyCollection<KeyData>();
            Macro = new ObservablePropertyCollection<KeyDataDelay>();
        }

        public Macros(string name, IEnumerable<KeyData> sequence, IEnumerable<KeyDataDelay> macro)
        {
            Name = name;
            Sequence = new ObservablePropertyCollection<KeyData>(sequence);
            Macro = new ObservablePropertyCollection<KeyDataDelay>(macro);
        }
    }
}
