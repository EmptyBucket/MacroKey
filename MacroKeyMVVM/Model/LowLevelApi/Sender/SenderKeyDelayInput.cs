using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MacroKey.InputData;

namespace MacroKey.LowLevelApi
{
    [Serializable]
    public class SenderKeyDelayInput : SenderInput
    {
        public async void SendKeyPress(IEnumerable<KeyDataDelay> keyDataDelayArray)
        {
            await Task.Run(() =>
            {
                foreach (var item in keyDataDelayArray)
                {
                    Task.WaitAll(new Task[] { Task.Delay(item.Delay) });
                    INPUT input = new INPUT()
                    {
                        mType = SENDINPUTEVENTTYPE.INPUT_KEYBOARD,
                        mInputUnion = new InputUnion()
                        {
                            ki = new KEYBDINPUT
                            {
                                wVk = item.VirtualKeyCode,
                                dwFlags = item.Message == KeyData.KeyMessage.WM_KEYUP ? KEYEVENTF.KEYUP : KEYEVENTF.NONE,
                                time = item.Time
                            }
                        }
                    };
                    SendInput(1, new INPUT[] { input }, INPUT.Size);
                }
            });
        }

        public override void SendImput(IEnumerable<Input> input)
        {
            IEnumerable<KeyDataDelay> keyDataDelay = (IEnumerable<KeyDataDelay>)input;
            SendKeyPress(keyDataDelay);
        }
    }
}
