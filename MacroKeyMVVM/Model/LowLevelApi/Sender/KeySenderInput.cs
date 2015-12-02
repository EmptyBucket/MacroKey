using System;
using System.Collections.Generic;
using System.Linq;
using MacroKey.InputData;
using MacroKey.LowLevelApi;
using MacroKeyMVVM.Model.InputData.Keyboard;

namespace MacroKeyMVVM.Model.LowLevelApi.Sender
{
    [Serializable]
    public class KeySenderInput : SenderInput
    {
        private INPUT BuildKeyInput(KeyboardData keyData)
        {
            INPUT input = new INPUT();
            input.mType = SENDINPUTEVENTTYPE.INPUT_KEYBOARD;
            input.mInputUnion.ki = new KEYBDINPUT
            {
                wVk = (short)keyData.Key.Code,
                dwFlags = (KeyState)keyData.State == KeyState.KeyUp ? KEYEVENTF.KEYUP : KEYEVENTF.NONE,
            };
            return input;
        }

        public override void SendInput(IEnumerable<IInput> inputEnum)
        {
            INPUT[] inputArray = inputEnum
                .Select(item => BuildKeyInput((KeyboardData)item))
                .ToArray();
            SendInput((uint)inputArray.Length, inputArray, INPUT.Size);
        }
    }
}
