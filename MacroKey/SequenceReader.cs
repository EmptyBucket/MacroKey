using System.ComponentModel;

namespace MacroKey
{
    class SequenceReader : INotifyPropertyChanged
    {
        private HookerKeys mHooker;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableKeyDataCollection ReadSequence { get; } = new ObservableKeyDataCollection();

        public SequenceReader(HookerKeys hooker)
        {
            mHooker = hooker;
        }

        private bool mRecordSequence(HookerKeys.KeyHookEventArgs e)
        {
            KeyData keyData = new KeyData(e.VirtualKeyCode, e.ScanCode, e.Flags, e.KeyboardMassage);
            ReadSequence.Add(keyData);
            OnPropertyChanged(new PropertyChangedEventArgs("ReadSequence"));
            return true;
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, e);
        }

        public void Clear()
        {
            ReadSequence.Clear();
        }

        public void StartRecord()
        {
            mHooker.HookedKey += mRecordSequence;
        }

        public void StopRecord()
        {
            mHooker.HookedKey -= mRecordSequence;
        }
    }
}
