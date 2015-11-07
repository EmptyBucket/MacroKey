using MacroKey.Keyboard;
using MacroKey.LowLevelApi.Hook;

namespace MacroKey.LowLevelApi
{
    class HookSequenceReader
    {
        private HookerKeyboard mHooker;
        private KeyboardData mPrewKeyData = new KeyboardData(0, 0, 0);

        public ObservableKeyboardDataCollection ReadSequence { get; } = new ObservableKeyboardDataCollection();

        public HookSequenceReader(HookerKeyboard hooker)
        {
            mHooker = hooker;
            hookEventHandler = RecordSequence;
        }

        private HookEventHandler hookEventHandler;

        private bool RecordSequence(HookEventArgs e)
        {
            var arg = (KeyboardHookEventArgs)e;
            KeyboardData keyboardData = new KeyboardData(arg.VirtualKeyCode, arg.KeyboardMassage, arg.Time);
            if(keyboardData.KeyMessage != mPrewKeyData.KeyMessage || keyboardData.VirtualKeyCode != mPrewKeyData.VirtualKeyCode)
                ReadSequence.Add(keyboardData);
            mPrewKeyData = keyboardData;
            return true;
        }

        public void Clear()
        {
            ReadSequence.Clear();
        }

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
