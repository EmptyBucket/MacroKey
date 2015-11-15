using System.Collections.Generic;
using MacroKey.Keyboard;

namespace MacroKey.LowLevelApi
{
    public interface ISenderInput
    {
        void SendImput(IEnumerable<Input> input);
    }
}
