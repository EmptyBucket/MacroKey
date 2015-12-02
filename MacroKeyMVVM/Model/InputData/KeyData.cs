using System;
using MacroKeyMVVM.Model.InputData;

namespace MacroKey.InputData
{
    [Serializable]
    public class KeyData : Input
    {
        public ICode VirtualCode { get; }
        public InputMessage Message { get; }

        public KeyData(int virtualCode, int keyMessage)
        {
            VirtualCode = new KeyCode(virtualCode);
            Message = (InputMessage)keyMessage;
        }
    }
}
