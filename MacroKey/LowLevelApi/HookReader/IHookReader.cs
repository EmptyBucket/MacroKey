namespace MacroKey.LowLevelApi.HookReader
{
    interface IHookReader<T>
    {
        ObservablePropertyCollection<T> ReadSequence { get; }

        void Clear();

        void StartNewRecord();

        void StartRecord();

        void StopRecord();
    }
}
