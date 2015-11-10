namespace MacroKey.Keyboard
{
    public class KeyDataDelay : KeyData
    {
        public KeyDataDelay(short virtualKeyCode, int keyMessage, int time, int delay) : base(virtualKeyCode, keyMessage, time)
        {
            Delay = delay;
        }

        public int Delay { get; set; }
    }
}
