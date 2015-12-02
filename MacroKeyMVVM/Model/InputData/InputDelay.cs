using System;
using MacroKey.InputData;

namespace MacroKeyMVVM.Model.InputData
{
    [Serializable]
    public class InputDelay
    {
        public IInput Data { get; }

        public int Delay { get; set; }

        public InputDelay(IInput input, int delay)
        {
            Data = input;
            Delay = delay;
        }
    }
}
