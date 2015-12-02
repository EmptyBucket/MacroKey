using System;

namespace MacroKeyMVVM.Model.InputData
{
    public enum MouseCodes
    {
        LeftMouse,
        RightMouse,
        MidleMouse,
        WheelMouse,
        Quest
    }

    [Serializable]
    public struct MouseCode : ICode
    {
        public int Code { get; }

        public MouseCode(MouseCodes mouseCode)
        {
            Code = (int)mouseCode;
        }
    }
}
