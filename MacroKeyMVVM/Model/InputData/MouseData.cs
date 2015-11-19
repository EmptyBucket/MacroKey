using System;
using System.Windows;
using MacroKey.InputData;

namespace MacroKeyMVVM.Model.InputData
{
    public enum MouseMessage { MK_CONTROL = 0x0008, MK_LBUTTON = 0x0001, MK_MBUTTON = 0x0010, MK_RBUTTON = 0x0002, MK_SHIFT = 0x0004, MK_XBUTTON1 = 0x0020, MK_XBUTTON2 = 0x0040 }

    [Serializable]
    public class MouseData : Input
    {

        public int VirtualCode { get; }
        public int Time { get; }
        public int Message { get; }
        public Point Coord { get; }

        public MouseData(Point coord, int mouseData, int mouseMessage, int time)
        {
            Coord = coord;
            VirtualCode = mouseData;
            Time = time;
            Message = mouseMessage;
        }
    }
}
