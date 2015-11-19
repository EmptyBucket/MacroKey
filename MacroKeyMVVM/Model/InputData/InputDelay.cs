using System;
using MacroKey.InputData;

namespace MacroKeyMVVM.Model.InputData
{
    [Serializable]
    public class InputDelay
    {
        public Input Data { get; }

        public int Delay { get; set; }

        public InputDelay(Input input, int delay)
        {
            Data = input;
            Delay = delay;
        }
    }
}
