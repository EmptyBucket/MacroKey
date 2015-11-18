using System.Collections.Generic;
using MacroKey.InputData;

namespace MacroKey.LowLevelApi
{
    public interface ISenderInput
    {
        void SendImput(IEnumerable<Input> input);
    }
}
