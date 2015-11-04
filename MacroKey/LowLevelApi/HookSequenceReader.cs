using MacroKey.Key;

namespace MacroKey.LowLevelApi
{
    class HookSequenceReader
    {
        private HookerKeys mHooker;
        private KeyData mPrewKeyData;

        public ObservableKeyDataCollection ReadSequence { get; } = new ObservableKeyDataCollection();

        public HookSequenceReader(HookerKeys hooker)
        {
            mHooker = hooker;
        }

        private bool RecordSequence(HookerKeys.KeyHookEventArgs e)
        {
            KeyData keyData = new KeyData(e.VirtualKeyCode, e.KeyboardMassage, e.Time);
            if(!keyData.Equals(mPrewKeyData))
                ReadSequence.Add(keyData);
            mPrewKeyData = keyData;
            return true;
        }

        public void Clear()
        {
            ReadSequence.Clear();
        }

        public void StartNewRecord()
        {
            Clear();
            mHooker.HookedKey += RecordSequence;
        }

        public void StartRecord()
        {
            mHooker.HookedKey += RecordSequence;
        }

        public void StopRecord()
        {
            mHooker.HookedKey -= RecordSequence;
        }
    }
}
