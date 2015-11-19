using System;
using System.Collections.Generic;
using MacroKey.InputData;

namespace MacroKey
{
    [Serializable]
    public class Macros
    {
        public string Name { get; }
        public IEnumerable<Input> Macro { get; }
        public IEnumerable<Input> Sequence { get; }

        public Macros()
        {
            Name = string.Empty;
            Sequence = new List<Input>();
            Macro = new List<Input>();
        }

        public Macros(string name, IEnumerable<Input> sequence, IEnumerable<Input> macro)
        {
            Name = name;
            Sequence = new List<Input>(sequence);
            Macro = new List<Input>(macro);
        }
    }
}
