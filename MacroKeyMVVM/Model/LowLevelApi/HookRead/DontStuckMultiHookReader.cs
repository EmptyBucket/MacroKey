using System.Collections.Generic;
using MacroKey.InputData;
using MacroKey.LowLevelApi.Hook;

namespace MacroKeyMVVM.Model.LowLevelApi.HookRead
{
    public class DontStuckMultiHookReader : MultiHookReader
    {
        private Input mPrewInput = new KeyData(0, 0, 0);

        public DontStuckMultiHookReader(IEnumerable<IHooker> hookerEnum) : base(hookerEnum) { }

        public DontStuckMultiHookReader(IEnumerable<IHooker> hookerEnum, IList<Input> sequence) : base(hookerEnum, sequence) { }

        protected override bool RecordSequence(Input e)
        {
            if (mPrewInput.VirtualCode != e.VirtualCode && mPrewInput.Message != e.Message)
                ReadSequence.Add(e);
            return true;
        }
    }
}
