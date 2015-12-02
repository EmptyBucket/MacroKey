using System.Collections.Generic;
using MacroKey.InputData;

namespace MacroKey.LowLevelApi
{
    public interface ISenderInput
    {
        void SendInput(IEnumerable<IInput> input);
    }
}
