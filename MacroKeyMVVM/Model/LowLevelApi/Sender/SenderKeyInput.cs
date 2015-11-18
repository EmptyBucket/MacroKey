using System.Collections.Generic;
using System.Linq;
using MacroKey.InputData;

namespace MacroKey.LowLevelApi
{
    public class SenderKeyInput : SenderInput
    {
        public override void SendImput(IEnumerable<Input> input)
        {
            IEnumerable<KeyData> keyDataEnum = (IEnumerable<KeyData>)input;
            INPUT[] inputs = keyDataEnum.Select(item =>
                new INPUT()
                {
                    mType = SENDINPUTEVENTTYPE.INPUT_KEYBOARD,
                    mInputUnion = new InputUnion()
                    {
                        ki = new KEYBDINPUT()
                        {
                            wVk = item.VirtualKeyCode,
                            dwFlags = item.Message == KeyData.KeyMessage.WM_KEYUP ? KEYEVENTF.KEYUP : KEYEVENTF.NONE,
                            time = item.Time
                        }
                    }
                }).ToArray();
            SendInput((uint)inputs.Length, inputs, INPUT.Size);
        }
    }
}
