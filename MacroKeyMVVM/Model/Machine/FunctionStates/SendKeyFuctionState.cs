using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MacroKey.InputData;
using MacroKeyMVVM.Model.InputData;
using MacroKeyMVVM.Model.LowLevelApi.Sender;
using MacroKeyMVVM.Model.Machine;

namespace MacroKey.Model.Machine
{
    [Serializable]
    public class SendKeyDelayFuctionState<TypeTransition> : CancellationFunctionState<TypeTransition>
    {
        private readonly KeySenderInput mKeySender;
        private readonly MouseSenderInput mMouseSender;

        public SendKeyDelayFuctionState(KeySenderInput keySender, MouseSenderInput mouseSender, IEnumerable<InputDelay> arg, IEqualityComparer<TypeTransition> equalityComparer = null) : base(arg, equalityComparer)
        {
            mKeySender = keySender;
            mMouseSender = mouseSender;
        }

        protected override object Function(object arg, CancellationToken cancellationToken)
        {
            IEnumerable<InputDelay> inputDelay = (IEnumerable<InputDelay>)arg;
            Task.Run(() =>
            {
                try
                {
                    foreach (var item in inputDelay)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        Task.WaitAll(Task.Delay(item.Delay));
                        cancellationToken.ThrowIfCancellationRequested();
                        IInput input = item.Data;
                        if (input is KeyboardData)
                            mKeySender.SendInput(new IInput[] { input });
                        else
                            mMouseSender.SendInput(new IInput[] { input });
                    }
                }
                catch (OperationCanceledException) { }
            });
            return false;
        }
    }
}
