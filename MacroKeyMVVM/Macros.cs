using System;
using System.Collections.Generic;
using MacroKey.InputData;

namespace MacroKey
{
    [Serializable]
    public class Macros
    {
        public string Name { get; }
        public IEnumerable<IInput> Macro { get; }
        public IEnumerable<IInput> Sequence { get; }

        public Macros()
        {
            Name = string.Empty;
            Sequence = new List<IInput>();
            Macro = new List<IInput>();
        }

        public Macros(string name, IEnumerable<IInput> sequence, IEnumerable<IInput> macro)
        {
            Name = name;
            Sequence = new List<IInput>(sequence);
            Macro = new List<IInput>(macro);
        }
    }
}
