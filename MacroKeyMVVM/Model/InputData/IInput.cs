using MacroKeyMVVM.Model.InputData;

namespace MacroKey.InputData
{
    public interface IInput
    {
        ICode Key { get; }
        IInputState State { get; }
    }
}
