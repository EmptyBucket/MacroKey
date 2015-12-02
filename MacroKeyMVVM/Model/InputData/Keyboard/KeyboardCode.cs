using System;

namespace MacroKeyMVVM.Model.InputData
{
    [Serializable]
    public struct KeyboardCode : ICode
    {
        public int Code { get; }

        public KeyboardCode(int keyCode)
        {
            Code = keyCode;
        }
    }
}
