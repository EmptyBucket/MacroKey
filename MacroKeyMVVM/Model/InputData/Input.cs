namespace MacroKey.InputData
{
    public interface Input
    {
        int VirtualCode { get; }
        int Time { get; }
        int Message { get; }
    }
}
