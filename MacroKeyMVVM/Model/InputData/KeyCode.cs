using System;

namespace MacroKeyMVVM.Model.InputData
{
    [Serializable]
    public struct KeyCode : ICode
    {
        public int Code { get; }

        public KeyCode(int keyCode)
        {
            Code = keyCode;
        }
    }
}
