using System.Collections.Generic;

namespace MacroKey
{
    public class Macros
    {
        public string Name { get; }
        public ObservableKeyDataCollection Macro { get; } = new ObservableKeyDataCollection();
        public ObservableKeyDataCollection Sequence { get; } = new ObservableKeyDataCollection();

        public Macros(string name, IEnumerable<KeyData> sequence, IEnumerable<KeyData> macros)
        {
            Name = name;
            Sequence = new ObservableKeyDataCollection(sequence);
            Macro = new ObservableKeyDataCollection(macros);
        }
    }
}
