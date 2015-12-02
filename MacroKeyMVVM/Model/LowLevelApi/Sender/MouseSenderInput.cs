using System;
using System.Collections.Generic;
using System.Linq;
using MacroKey.InputData;
using MacroKey.LowLevelApi;
using MacroKeyMVVM.Model.InputData;
using MacroKeyMVVM.Model.InputData.Mouse;

namespace MacroKeyMVVM.Model.LowLevelApi.Sender
{
    [Serializable]
    public class MouseSenderInput : SenderInput
    {
        private INPUT BuildMouseInput(MouseData mouseData)
        {
            INPUT input = new INPUT();
            input.mType = SENDINPUTEVENTTYPE.INPUT_MOUSE;
            MouseState mouseState = (MouseState)mouseData.State;
            switch ((MouseCodes)mouseData.Key.Code)
            {
                case MouseCodes.LeftMouse:
                    input.mInputUnion.mi.dwFlags = mouseState == MouseState.MouseDown ? MOUSEEVENTF.LEFTDOWN : MOUSEEVENTF.LEFTUP;
                    break;
                case MouseCodes.RightMouse:
                    input.mInputUnion.mi.dwFlags = mouseState == MouseState.MouseDown ? MOUSEEVENTF.RIGHTDOWN : MOUSEEVENTF.RIGHTUP;
                    break;
                case MouseCodes.MidleMouse:
                    input.mInputUnion.mi.dwFlags = mouseState == MouseState.MouseDown ? MOUSEEVENTF.MIDDLEDOWN : MOUSEEVENTF.MIDDLEUP;
                    break;
                case MouseCodes.WheelMouse:
                    input.mInputUnion.mi.dwFlags = MOUSEEVENTF.WHEEL;
                    input.mInputUnion.mi.mouseData = mouseState == MouseState.MouseWheelDown ? -100 : 100;
                    break;
            }
            return input;
        }

        public override void SendInput(IEnumerable<IInput> inputEnum)
        {
            INPUT[] inputArray = inputEnum
                .Select(item => BuildMouseInput((MouseData)item))
                .ToArray();
            SendInput((uint)inputArray.Length, inputArray, INPUT.Size);
        }
    }
}
