using System.Collections.Generic;
using MacroKey.InputData;
using MacroKey.LowLevelApi.Hook;
using MacroKey.LowLevelApi.HookReader;

namespace MacroKeyMVVM.Model.LowLevelApi.HookRead
{
    public class HookNotRepeatReader : HookReader
    {
        private IInput mPrewInput = new KeyboardData(0, 0);

        public HookNotRepeatReader(IHooker hooker) : base(hooker) { }

        public HookNotRepeatReader(IHooker hooker, IList<IInput> sequence) : base(hooker, sequence) { }

        protected override bool RecordSequence(IInput e)
        {
            if (mPrewInput.Key != e.Key && mPrewInput.State != e.State)
                ReadSequence.Add(e);
            return true;
        }
    }
}
