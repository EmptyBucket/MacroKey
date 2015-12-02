using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MacroKey.InputData;
using MacroKey.Machine;
using MacroKeyMVVM.Model.InputData;
using MacroKeyMVVM.Model.LowLevelApi.Sender;

namespace MacroKey.Model.Machine
{
    [Serializable]
    public class SendKeyDelayFuctionState<TypeTransition> : FunctionState<TypeTransition>
    {
        private readonly KeySenderInput mKeySender;
        private readonly MouseSenderInput mMouseSender;

        public SendKeyDelayFuctionState(KeySenderInput keySender, MouseSenderInput mouseSender, IEnumerable<InputDelay> arg, IEqualityComparer<TypeTransition> equalityComparer = null) : base(arg, equalityComparer)
        {
            mKeySender = keySender;
            mMouseSender = mouseSender;
        }

        protected override object Function(object arg)
        {
            IEnumerable<InputDelay> inputDelay = (IEnumerable<InputDelay>)arg;
            Task.Run(() =>
            {
                foreach (var item in inputDelay)
                {
                    Task.WaitAll(Task.Delay(item.Delay));
                    Input input = item.Data;
                    if (input is KeyData)
                        mKeySender.SendInput(new Input[] { input });
                    else
                        mMouseSender.SendInput(new Input[] { input });
                }
            });
            return false;
        }
    }
}
