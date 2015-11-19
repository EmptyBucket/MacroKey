using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MacroKey.InputData;
using MacroKey.LowLevelApi;
using MacroKey.Machine;
using MacroKeyMVVM.Model.InputData;

namespace MacroKey.Model.Machine
{
    [Serializable]
    public class SendKeyDelayFuctionState<TypeTransition> : FunctionState<TypeTransition>
    {
        private ISenderInput mSender { get; }

        public SendKeyDelayFuctionState(ISenderInput sender, IEnumerable<InputDelay> arg, IEqualityComparer<TypeTransition> equalityComparer = null) : base(arg, equalityComparer)
        {
            mSender = sender;
        }

        protected override object Function(object arg)
        {
            IEnumerable<InputDelay> inputDelay = (IEnumerable<InputDelay>)arg;
            Task.Run(() =>
            {
                foreach (var item in inputDelay)
                {
                    Task.WaitAll(Task.Delay(item.Delay));
                    mSender.SendInput(new Input[] { item.Data });
                }
            });
            return false;
        }
    }
}
