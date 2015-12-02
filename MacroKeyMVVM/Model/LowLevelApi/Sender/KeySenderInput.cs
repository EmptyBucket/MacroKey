using System;
using System.Collections.Generic;
using System.Linq;
using MacroKey.InputData;
using MacroKey.LowLevelApi;
using MacroKeyMVVM.Model.InputData;

namespace MacroKeyMVVM.Model.LowLevelApi.Sender
{
    [Serializable]
    public class KeySenderInput : SenderInput
    {
        private INPUT BuildKeyInput(KeyData keyData)
        {
            INPUT input = new INPUT();
            input.mType = SENDINPUTEVENTTYPE.INPUT_KEYBOARD;
            input.mInputUnion.ki = new KEYBDINPUT
            {
                wVk = (short)keyData.VirtualCode.Code,
                dwFlags = keyData.Message == InputMessage.WM_KEYUP ? KEYEVENTF.KEYUP : KEYEVENTF.NONE,
            };
            return input;
        }

        public override void SendInput(IEnumerable<Input> inputEnum)
        {
            INPUT[] inputArray = inputEnum
                .Select(item => BuildKeyInput((KeyData)item))
                .ToArray();
            SendInput((uint)inputArray.Length, inputArray, INPUT.Size);
        }
    }
}
