using System.Collections.Generic;
using MacroKey.Keyboard;

namespace MacroKey
{
    public class Macros
    {
        public string Name { get; }
        public ObservablePropertyCollection<KeyData> Macro { get; }
        public ObservablePropertyCollection<KeyData> Sequence { get; }

        public Macros()
        {
            Name = string.Empty;
            Sequence = new ObservablePropertyCollection<KeyData>();
            Macro = new ObservablePropertyCollection<KeyData>();
        }

        public Macros(string name, IEnumerable<KeyData> sequence, IEnumerable<KeyData> macros)
        {
            Name = name;
            Sequence = new ObservablePropertyCollection<KeyData>(sequence);
            Macro = new ObservablePropertyCollection<KeyData>(macros);
        }
    }
}
