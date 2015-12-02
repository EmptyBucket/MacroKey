using System;
using System.Windows;
using MacroKey.InputData;

namespace MacroKeyMVVM.Model.InputData
{
    [Serializable]
    public class MouseData : Input
    {

        public ICode VirtualCode { get; }
        public InputMessage Message { get; }
        public Point Coord { get; }

        public MouseData(Point coord, int virtualCode, int mouseMessage)
        {
            Coord = coord;
            VirtualCode = new MouseCode(virtualCode);
            Message = (InputMessage)mouseMessage;
        }
    }
}
