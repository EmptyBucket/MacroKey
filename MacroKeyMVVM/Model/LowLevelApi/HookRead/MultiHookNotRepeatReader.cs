using System.Collections.Generic;
using MacroKey.InputData;
using MacroKey.LowLevelApi.Hook;

namespace MacroKeyMVVM.Model.LowLevelApi.HookRead
{
    public class MultiHookNotRepeatReader : MultiHookReader
    {
        private IInput mPrewInput = new KeyboardData(0, 0);

        public MultiHookNotRepeatReader(IEnumerable<IHooker> hookerEnum) : base(hookerEnum) { }

        public MultiHookNotRepeatReader(IEnumerable<IHooker> hookerEnum, IList<IInput> sequence) : base(hookerEnum, sequence) { }

        protected override bool RecordSequence(IInput e)
        {
            if (mPrewInput.Key != e.Key && mPrewInput.State != e.State)
                ReadSequence.Add(e);
            return true;
        }
    }
}
