using System;
using MacroKeyMVVM.Model.InputData;
using MacroKeyMVVM.Model.InputData.Keyboard;

namespace MacroKey.InputData
{
    [Serializable]
    public class KeyboardData : IInput
    {
        public ICode Key { get; }
        public IInputState State { get; }

        public KeyboardData(int virtualCode, KeyStates keyState)
        {
            Key = new KeyboardCode(virtualCode);
            State = new KeyState(keyState);
        }
    }
}
