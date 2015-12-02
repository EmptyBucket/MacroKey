using System;
using System.Windows;
using MacroKey.InputData;
using MacroKeyMVVM.Model.InputData.Mouse;

namespace MacroKeyMVVM.Model.InputData
{
    [Serializable]
    public class MouseData : IInput
    {
        public ICode Key { get; }
        public IInputState State { get; }
        public Point Coord { get; }

        public MouseData(Point coord, MouseCodes key, MouseStates mouseState)
        {
            Coord = coord;
            Key = new MouseCode(key);
            State = new MouseState(mouseState);
        }
    }
}
