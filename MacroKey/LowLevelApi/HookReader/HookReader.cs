﻿using MacroKey.LowLevelApi.Hook;

namespace MacroKey.LowLevelApi.HookReader
{
    abstract class HookReader<T> : ObservablePropertyCollection<T>, IHookReader<T>
    {
        private HookerKey mHooker;

        private HookEventHandler hookEventHandler;

        public HookReader(HookerKey hooker)
        {
            mHooker = hooker;
            hookEventHandler = RecordSequence;
        }

        protected abstract bool RecordSequence(HookEventArgs e);

        public void StartNewRecord()
        {
            Clear();
            mHooker.Hooked += hookEventHandler;
        }

        public void StartRecord()
        {
            mHooker.Hooked += hookEventHandler;
        }

        public void StopRecord()
        {
            mHooker.Hooked -= hookEventHandler;
        }
    }
}
