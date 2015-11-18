using System;
using System.Collections.Generic;
using MacroKey.InputData;
using MacroKey.LowLevelApi;
using MacroKey.Machine;

namespace MacroKey.Model.Machine
{
    [Serializable]
    public class SendKeyFuctionState<T> : FunctionState<T>
    {
        private ISenderInput mSender { get; }

        public SendKeyFuctionState(ISenderInput sender, object arg, IEqualityComparer<T> equalityComparer = null) : base(arg, equalityComparer)
        {
            mSender = sender;
        }

        public override object Function(object arg)
        {
            mSender.SendImput((IEnumerable<Input>)arg);
            return false;
        }
    }
}
