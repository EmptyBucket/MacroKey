using MacroKey.Keyboard;
using MacroKey.LowLevelApi.Hook;

namespace MacroKey.LowLevelApi.HookReader
{
    public class HookKeyDataReader : HookReader<KeyData>
    {
        private KeyData mPrewKeyData = new KeyData(0, 0, 0);

        public HookKeyDataReader(IHooker hooker) : base(hooker) { }

        protected override bool RecordSequence(HookEventArgs e)
        {
            var arg = (KeyHookEventArgs)e;
            KeyData keyData = new KeyData(arg.VirtualKeyCode, arg.KeyMassage, arg.Time);
            if(keyData.Message != mPrewKeyData.Message || keyData.VirtualKeyCode != mPrewKeyData.VirtualKeyCode)
                ReadSequence.Add(keyData);
            mPrewKeyData = keyData;
            return true;
        }
    }
}
