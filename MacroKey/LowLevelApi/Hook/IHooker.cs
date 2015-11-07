namespace MacroKey.LowLevelApi.Hook
{
    public interface IHooker
    {
        void SetHook();

        void Unhook();

        event HookEventHandler Hooked;
    }
}