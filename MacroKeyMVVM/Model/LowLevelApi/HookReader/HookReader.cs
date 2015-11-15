using MacroKey.LowLevelApi.Hook;

namespace MacroKey.LowLevelApi.HookReader
{
    public abstract class HookReader<T> : IHookReader<T>
    {
        private IHooker mHooker;

        public bool IsRecord { get; private set; } = false;

        private HookEventHandler hookEventHandler;

        public ObservablePropertyCollection<T> ReadSequence { get; } = new ObservablePropertyCollection<T>();

        public HookReader(IHooker hooker)
        {
            mHooker = hooker;
            hookEventHandler = RecordSequence;
        }

        protected abstract bool RecordSequence(HookEventArgs e);

        public void StartNewRecord()
        {
            IsRecord = true;
            Clear();
            mHooker.Hooked += hookEventHandler;
        }

        public void StartRecord()
        {
            IsRecord = true;
            mHooker.Hooked += hookEventHandler;
        }

        public void StopRecord()
        {
            IsRecord = false;
            mHooker.Hooked -= hookEventHandler;
        }

        public void Clear()
        {
            ReadSequence.Clear();
        }
    }
}
