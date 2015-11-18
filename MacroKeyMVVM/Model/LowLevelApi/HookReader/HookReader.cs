using System.Collections.Generic;
using MacroKey.LowLevelApi.Hook;

namespace MacroKey.LowLevelApi.HookReader
{
    public abstract class HookReader<T> : IHookReader<T>
    {
        private IHooker mHooker;

        public bool IsRecord { get; private set; } = false;

        private HookEventHandler hookEventHandler;

        public IList<T> ReadSequence { get; }

        public HookReader(IHooker hooker)
        {
            mHooker = hooker;
            hookEventHandler = RecordSequence;
            ReadSequence = new List<T>();
        }

        public HookReader(IHooker hooker, IList<T> sequence) : this(hooker)
        {
            ReadSequence = sequence;
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
