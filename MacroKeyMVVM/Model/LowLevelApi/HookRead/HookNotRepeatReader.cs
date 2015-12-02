using System.Collections.Generic;
using MacroKey.InputData;
using MacroKey.LowLevelApi.Hook;
using MacroKey.LowLevelApi.HookReader;

namespace MacroKeyMVVM.Model.LowLevelApi.HookRead
{
    public class HookNotRepeatReader : HookReader
    {
        private Input mPrewInput = new KeyData(0, 0);

        public HookNotRepeatReader(IHooker hooker) : base(hooker) { }

        public HookNotRepeatReader(IHooker hooker, IList<Input> sequence) : base(hooker, sequence) { }

        protected override bool RecordSequence(Input e)
        {
            if (mPrewInput.VirtualCode != e.VirtualCode && mPrewInput.Message != e.Message)
                ReadSequence.Add(e);
            return true;
        }
    }
}
