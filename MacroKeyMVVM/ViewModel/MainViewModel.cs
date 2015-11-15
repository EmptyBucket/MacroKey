using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using MacroKey;
using MacroKey.Keyboard;
using MacroKey.LowLevelApi;
using MacroKey.LowLevelApi.Hook;
using MacroKey.LowLevelApi.HookReader;
using MacroKey.Machine;

namespace MacroKeyMVVM.ViewModel
{
    public class MainViewModel : ViewModelBase, IDisposable
    {
        public IHooker mHookerKey { get; }
        public ISenderInput mSenderKey { get; }
        public KeyDataEqualityComparer mKeyDataEqualityComparer { get; } = new KeyDataEqualityComparer();
        public Tree<KeyData> mTreeRoot { get; }
        public Tree<KeyData> mTreeSequence { get; }
        public Branch<KeyData> mHotkeyExecuteGUIBranch { get; private set; }
        public Branch<KeyData> mHotkeyMacrosModeBranch { get; private set; }
        public string MacrosName { get; set; }
        public string DelayDefault { get; set; }
        public HookKeyDataReader SequenceReader { get; }
        public HookKeyDataDelayReader MacroReader { get; }
        public HookKeyDataReader ExecuteGUIReader { get; }
        public HookKeyDataReader MacrosModeReader { get; }
        public ObservableCollection<Macros> MacrosCollection { get; } = new ObservableCollection<Macros>();

        public RelayCommand RecordSequenceCommand { get; }
        public RelayCommand RecordMacroCommand { get; }
        public RelayCommand CreateMacrosCommand { get; }
        public RelayCommand CleanRowsSequenceCommand { get; }
        public RelayCommand CleanRowsMacroCommand { get; }
        public RelayCommand CleanRowsMacrosCommand { get; }
        public RelayCommand<KeyData> DeleteRowSequenceCommand { get; }
        public RelayCommand<KeyDataDelay> DeleteRowMacroCommand { get; }
        public RelayCommand<Macros> DeleteRowMacrosCommand { get; }
        public RelayCommand StopRecordExecuteGUICommand { get; }
        public RelayCommand StartRecordExecuteGUICommand { get; }
        public RelayCommand StartRecordMacrosModeCommand { get; }
        public RelayCommand StopRecordMacrosModeCommand { get; }
        public RelayCommand StopAllRecordCommand { get; }
        public RelayCommand SetDefaultDelayCommand { get; }

        public MainViewModel(IHooker hooker, ISenderInput sender)
        {
            mHookerKey = hooker;
            mSenderKey = sender;
            mTreeRoot = new Tree<KeyData>(mKeyDataEqualityComparer);
            mTreeSequence = new Tree<KeyData>(mKeyDataEqualityComparer);
            mHotkeyExecuteGUIBranch = new Branch<KeyData>(mKeyDataEqualityComparer);
            mHotkeyMacrosModeBranch = new Branch<KeyData>(mKeyDataEqualityComparer);
            SequenceReader = new HookKeyDataReader(mHookerKey);
            MacroReader = new HookKeyDataDelayReader(mHookerKey);
            ExecuteGUIReader = new HookKeyDataReader(mHookerKey);
            MacrosModeReader = new HookKeyDataReader(mHookerKey);
            MacroReader.ReadSequence.CollectionChanged += ReadMacro_CollectionChanged;

            RecordSequenceCommand = new RelayCommand(RecordSequence);
            RecordMacroCommand = new RelayCommand(RecordMacro);
            CreateMacrosCommand = new RelayCommand(CreateMacros);
            CleanRowsSequenceCommand = new RelayCommand(CleanRowsSequence);
            CleanRowsMacroCommand = new RelayCommand(CleanRowsMacro);
            CleanRowsMacrosCommand = new RelayCommand(CleanRowsMacros);
            DeleteRowSequenceCommand = new RelayCommand<KeyData>(DeleteRowSequence);
            DeleteRowMacroCommand = new RelayCommand<KeyDataDelay>(DeleteRowMacro);
            DeleteRowMacrosCommand = new RelayCommand<Macros>(DeleteRowMacros);
            StopRecordExecuteGUICommand = new RelayCommand(StopRecordExecuteGUI);
            StartRecordExecuteGUICommand = new RelayCommand(StartRecordExecuteGUI);
            StartRecordMacrosModeCommand = new RelayCommand(StartRecordMacrosMode);
            StopRecordMacrosModeCommand = new RelayCommand(StopRecordMacrosMode);
            SetDefaultDelayCommand = new RelayCommand(SetDefaultDelay);
            StopAllRecordCommand = new RelayCommand(StopAllRecord);

            mHookerKey.SetHook();

            StateWalker<KeyData> machineWalker = new StateWalker<KeyData>(mTreeRoot);
            mHookerKey.Hooked += (obj) =>
            {
                var arg = (KeyHookEventArgs)obj;
                State<KeyData> currentState = machineWalker.WalkStates(new KeyData(arg.VirtualKeyCode, arg.KeyMassage, arg.Time));
                return currentState is FunctionalState<KeyData> ? (bool)((FunctionalState<KeyData>)currentState).ExecuteFunction() : true;
            };
        }

        public void Dispose() => mHookerKey.Unhook();

        private void StopAllRecord()
        {
            SequenceReader.StopRecord();
            MacroReader.StopRecord();
            MacrosModeReader.StopRecord();
            ExecuteGUIReader.StopRecord();
        }

        public void RecordSequence()
        {
            if (!SequenceReader.IsRecord)
            {
                StopAllRecord();
                SequenceReader.StartRecord();
            }
            else
                StopAllRecord();

        }

        public void RecordMacro()
        {
            if (!MacroReader.IsRecord)
            {
                StopAllRecord();
                MacroReader.StartRecord();
            }
            else
                StopAllRecord();

        }

        public void CreateMacros()
        {
            StopAllRecord();

            if (SequenceReader.ReadSequence.Count == 0)
            {
                MessageBox.Show("Sequence is empty", "Error", MessageBoxButton.OK);
                return;
            }
            if (MacroReader.ReadSequence.Count == 0)
            {
                MessageBox.Show("Macros is empty", "Error", MessageBoxButton.OK);
                return;
            }
            if (MacrosModeReader.ReadSequence.Count == 0)
            {
                MessageBox.Show("Macros mode hotkey is empty", "Error", MessageBoxButton.OK);
                return;
            }
            IEnumerable<Func<object, object>> functions = Enumerable.Repeat<Func<object, object>>(obj => false, SequenceReader.ReadSequence.Count);
            Branch<KeyData> branchSequence = new Branch<KeyData>(SequenceReader.ReadSequence, functions, mKeyDataEqualityComparer);
            branchSequence.SetFunctionBranch(obj =>
            {
                mSenderKey.SendImput((IEnumerable<KeyDataDelay>)obj);
                return false;
            }, MacroReader.ReadSequence.ToArray());

            mTreeSequence.AddState(branchSequence);

            Macros macro = new Macros(MacrosName, SequenceReader.ReadSequence, MacroReader.ReadSequence);
            MacrosCollection.Add(macro);

            MacrosName = string.Empty;
            SequenceReader.Clear();
            MacroReader.Clear();
        }

        public void CleanRowsSequence() => SequenceReader.Clear();

        public void CleanRowsMacro() => MacroReader.Clear();

        public void CleanRowsMacros()
        {
            mTreeSequence.ClearTree();
            MacrosCollection.Clear();
        }

        public void DeleteRowSequence(KeyData param) => SequenceReader.ReadSequence.Remove(param);

        public void DeleteRowMacro(KeyDataDelay param) => MacroReader.ReadSequence.Remove(param);

        public void DeleteRowMacros(Macros param)
        {
            int index = MacrosCollection.IndexOf(param);
            mTreeSequence.RemoveAtState(index);

            MacrosCollection.Remove(param);
        }

        public void StopRecordExecuteGUI()
        {
            ExecuteGUIReader.StopRecord();
            if (ExecuteGUIReader.ReadSequence.Count == 0)
                return;

            mHotkeyExecuteGUIBranch = new Branch<KeyData>(ExecuteGUIReader.ReadSequence, mKeyDataEqualityComparer);
            mTreeRoot.SetState(new List<Branch<KeyData>> { mHotkeyExecuteGUIBranch, mHotkeyMacrosModeBranch });
        }

        public void StartRecordExecuteGUI()
        {
            StopAllRecord();
            ExecuteGUIReader.StartNewRecord();
        }

        public void StopRecordMacrosMode()
        {
            MacrosModeReader.StopRecord();
            if (MacrosModeReader.ReadSequence.Count == 0)
                return;

            mHotkeyMacrosModeBranch = new Branch<KeyData>(MacrosModeReader.ReadSequence, mKeyDataEqualityComparer);
            mHotkeyMacrosModeBranch.AddState(mTreeSequence);
            mTreeRoot.SetState(new List<Branch<KeyData>> { mHotkeyExecuteGUIBranch, mHotkeyMacrosModeBranch });
        }

        public void StartRecordMacrosMode()
        {
            StopAllRecord();
            MacrosModeReader.StartNewRecord();
        }

        public void SetDefaultDelay()
        {
            int value = int.Parse(DelayDefault);
            for (int i = 0; i < MacroReader.ReadSequence.Count; i++)
            {
                var item = MacroReader.ReadSequence[i];
                MacroReader.ReadSequence[i] = new KeyDataDelay(item.VirtualKeyCode, (int)item.Message, item.Time, value);
            }
        }

        private void ReadMacro_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                int value = int.Parse(DelayDefault);
                foreach (var item in e.NewItems)
                    ((KeyDataDelay)item).Delay = value;
            }
        }
    }
}