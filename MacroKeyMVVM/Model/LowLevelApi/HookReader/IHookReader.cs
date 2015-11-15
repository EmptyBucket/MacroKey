namespace MacroKey.LowLevelApi.HookReader
{
    interface IHookReader<T>
    {
        void Clear();

        void StartNewRecord();

        void StartRecord();

        void StopRecord();
    }
}
