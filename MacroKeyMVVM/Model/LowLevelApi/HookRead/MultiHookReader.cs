using System.Collections.Generic;
using MacroKey.InputData;
using MacroKey.LowLevelApi.Hook;
using MacroKey.LowLevelApi.HookReader;

namespace MacroKeyMVVM.Model.LowLevelApi.HookRead
{
    public class MultiHookReader : IHookReader
    {
        public bool IsRecord { get; private set; }

        private IEnumerable<IHooker> mHookerEnum;

        public IList<Input> ReadSequence { get; }

        private HookEventHandler hookEventHandler;

        public MultiHookReader(IEnumerable<IHooker> hookerEnum)
        {
            mHookerEnum = hookerEnum;
            hookEventHandler = RecordSequence;
            ReadSequence = new List<Input>();
        }

        public MultiHookReader(IEnumerable<IHooker> hookerEnum, IList<Input> sequence) : this(hookerEnum)
        {
            ReadSequence = sequence;
        }

        protected virtual bool RecordSequence(Input e)
        {
            ReadSequence.Add(e);
            return true;
        }

        public void Clear()
        {
            ReadSequence.Clear();
        }

        public void StartNewRecord()
        {
            Clear();
            StartRecord();
        }

        public void StartRecord()
        {
            IsRecord = true;
            foreach (var item in mHookerEnum)
                item.Hooked += hookEventHandler;
        }

        public void StopRecord()
        {
            IsRecord = false;
            foreach (var item in mHookerEnum)
                item.Hooked -= hookEventHandler;
        }
    }
}
