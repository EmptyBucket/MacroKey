namespace MacroKey
{
    class MouseData : IData
    {
        public int DX { get; }
        public int DY { get; }
        public int Flags { get; }
        public int Data { get; }

        public MouseData(int dx, int dy, int flags, int mouseData)
        {
            DX = dx;
            DY = dy;
            Flags = flags;
            Data = mouseData;
        }
    }
}
