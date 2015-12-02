using System;
using System.Collections.Generic;
using System.Linq;
using MacroKey.InputData;
using MacroKey.LowLevelApi;
using MacroKeyMVVM.Model.InputData;

namespace MacroKeyMVVM.Model.LowLevelApi.Sender
{
    [Serializable]
    public class MouseSenderInput : SenderInput
    {
        private INPUT BuildMouseInput(MouseData mouseData)
        {
            INPUT input = new INPUT();
            MOUSEEVENTF flags = MOUSEEVENTF.NONE;
            switch (mouseData.Message)
            {
                case InputMessage.WM_RBUTTONUP:
                    input.mInputUnion.mi.dwFlags = MOUSEEVENTF.RIGHTUP;
                    break;
                case InputMessage.WM_RBUTTONDOWN:
                    flags = MOUSEEVENTF.RIGHTDOWN;
                    break;
                case InputMessage.WM_LBUTTONUP:
                    flags = MOUSEEVENTF.LEFTUP;
                    break;
                case InputMessage.WM_LBUTTONDOWN:
                    flags = MOUSEEVENTF.LEFTDOWN;
                    break;
                case InputMessage.WM_MOUSEWHEEL:
                    flags = MOUSEEVENTF.WHEEL;
                    break;
            }
            input.mType = SENDINPUTEVENTTYPE.INPUT_MOUSE;
            input.mInputUnion.mi = new MOUSEINPUT() { dwFlags = flags };
            return input;
        }

        public override void SendInput(IEnumerable<Input> inputEnum)
        {
            INPUT[] inputArray = inputEnum
                .Select(item => BuildMouseInput((MouseData)item))
                .ToArray();
            SendInput((uint)inputArray.Length, inputArray, INPUT.Size);
        }
    }
}
