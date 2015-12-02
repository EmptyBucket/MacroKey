using System;

namespace MacroKeyMVVM.Model.InputData
{
    [Serializable]
    public struct MouseCode : ICode
    {
        public int Code { get; }

        public MouseCode(int mouseCode)
        {
            Code = mouseCode;
        }

        public const int XBUTTON1 = 0x0001;
        public const int XBUTTON2 = 0x0002;
    }
}
