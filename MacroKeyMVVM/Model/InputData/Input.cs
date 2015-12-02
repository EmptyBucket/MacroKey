using MacroKeyMVVM.Model.InputData;

namespace MacroKey.InputData
{
    public interface Input
    {
        ICode VirtualCode { get; }
        InputMessage Message { get; }
    }
}
