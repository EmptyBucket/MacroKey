using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using MacroKey.InputData;
using MacroKey.LowLevelApi.Hook;
using MacroKey.Machine;
using MacroKey.Model.Machine;
using MacroKey.Properties;
using MacroKeyMVVM.Model.InputData;
using MacroKeyMVVM.Model.LowLevelApi.HookRead;
using MacroKeyMVVM.Model.LowLevelApi.Sender;

namespace MacroKey.ViewModel
{
    public class MainViewModel : ViewModelBase, IDisposable
    {
        private readonly KeyHooker mHookerKey;
        private readonly MouseHooker mHookerMouse;
        private readonly MouseSenderInput mMouseSenderInput;
        private readonly KeySenderInput mKeySenderInput;
        
        private readonly InputEqualityComparer mInputEqualityComparer = new InputEqualityComparer();
        private readonly Tree<IInput> mTreeRoot;
        private readonly Tree<IInput> mTreeSequence;
        private Branch<IInput> mExecuteGUIBranch;
        private Branch<IInput> mMacrosModeBranch;

        private readonly HookNotRepeatReader mSequenceReader;
        private readonly HookNotRepeatReader mExecuteGUIReader;
        private readonly HookNotRepeatReader mMacrosModeReader;
        private readonly MultiHookNotRepeatReader mMacroReader;

        public string MacrosName { get; set; }
        public ObservableCollection<Macros> MacrosCollection { get; }

        public string DelayDefault { get; set; }

        public RelayCommand RecordSequenceCommand { get; }
        public RelayCommand RecordMacroCommand { get; }
        public RelayCommand CreateMacrosCommand { get; }
        public RelayCommand CleanRowsSequenceCommand { get; }
        public RelayCommand CleanRowsMacroCommand { get; }
        public RelayCommand CleanRowsMacrosCommand { get; }
        public RelayCommand<IInput> DeleteRowSequenceCommand { get; }
        public RelayCommand<InputDelay> DeleteRowMacroCommand { get; }
        public RelayCommand<Macros> DeleteRowMacrosCommand { get; }
        public RelayCommand StopRecordExecuteGUICommand { get; }
        public RelayCommand StartRecordExecuteGUICommand { get; }
        public RelayCommand StartRecordMacrosModeCommand { get; }
        public RelayCommand StopRecordMacrosModeCommand { get; }
        public RelayCommand StopAllRecordCommand { get; }
        public RelayCommand SetDefaultDelayCommand { get; }

        public ObservablePropertyCollection<IInput> GUICollection { get; }
        public ObservablePropertyCollection<IInput> MacrosModeCollection { get; }
        public ObservableCollection<IInput> SequenceCollection { get; }
        public ObservableCollection<InputDelay> MacroCollection { get; }

        public MainViewModel(KeyHooker hookerKey, MouseHooker hookerMouse, KeySenderInput keySenderInput, MouseSenderInput mouseSenderInput)
        {
            mHookerKey = hookerKey;
            mHookerKey.SetHook();
            mHookerMouse = hookerMouse;
            mHookerMouse.SetHook();

            mKeySenderInput = keySenderInput;
            mMouseSenderInput = mouseSenderInput;

            mExecuteGUIBranch = new Branch<IInput>(mInputEqualityComparer);
            mMacrosModeBranch = new Branch<IInput>(mInputEqualityComparer);

            if (Settings.Default.Setting == null)
                Settings.Default.Setting = new AppSettings(mInputEqualityComparer);
            AppSettings settings = Settings.Default.Setting;
            mTreeRoot = settings.TreeRoot;
            mTreeSequence = settings.TreeSequence;
            MacrosCollection = settings.MacrosCollection;

            GUICollection = settings.SequenceGUI;
            MacrosModeCollection = settings.SequenceMacrosMode;
            SequenceCollection = settings.Sequence;
            MacroCollection = settings.Macro;

            mSequenceReader = new HookNotRepeatReader(mHookerKey, SequenceCollection);
            ObservableCollection<IInput> tempCollection = new ObservableCollection<IInput>(MacroCollection.Cast<IInput>());
            tempCollection.CollectionChanged += ReadMacro_CollectionChanged;
            mMacroReader = new MultiHookNotRepeatReader(new List<IHooker> { mHookerKey, mHookerMouse }, tempCollection);
            mExecuteGUIReader = new HookNotRepeatReader(mHookerKey, GUICollection);
            mMacrosModeReader = new HookNotRepeatReader(mHookerKey, MacrosModeCollection);

            RecordSequenceCommand = new RelayCommand(RecordSequence);
            RecordMacroCommand = new RelayCommand(RecordMacro);
            CreateMacrosCommand = new RelayCommand(CreateMacros);
            CleanRowsSequenceCommand = new RelayCommand(CleanSequence);
            CleanRowsMacroCommand = new RelayCommand(CleanMacro);
            CleanRowsMacrosCommand = new RelayCommand(CleanMacros);
            DeleteRowSequenceCommand = new RelayCommand<IInput>(RemoveSequence);
            DeleteRowMacroCommand = new RelayCommand<InputDelay>(RemoveMacro);
            DeleteRowMacrosCommand = new RelayCommand<Macros>(RemoveMacros);
            StopRecordExecuteGUICommand = new RelayCommand(StopRecordExecuteGUI);
            StartRecordExecuteGUICommand = new RelayCommand(StartRecordExecuteGUI);
            StartRecordMacrosModeCommand = new RelayCommand(StartRecordMacrosMode);
            StopRecordMacrosModeCommand = new RelayCommand(StopRecordMacrosMode);
            SetDefaultDelayCommand = new RelayCommand(SetDefaultDelay);
            StopAllRecordCommand = new RelayCommand(StopAllRecord);

            StateWalker<IInput> machineWalker = new StateWalker<IInput>(mTreeRoot);
            mHookerKey.Hooked += arg =>
            {
                State<IInput> currentState = machineWalker.WalkStates(arg);
                return currentState is FunctionState<IInput> ? (bool)((FunctionState<IInput>)currentState).ExecuteFunction() : true;
            };
        }

        private void ReadMacro_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                int value = int.Parse(DelayDefault);
                foreach (var item in e.NewItems)
                    MacroCollection.Add(new InputDelay((IInput)item, value));
            }
        }

        public override void Cleanup()
        {
            Dispose();
            Settings.Default.Save();
        }

        public void Dispose()
        {
            mHookerKey.Unhook();
            mHookerMouse.Unhook();
        }

        private void StopAllRecord()
        {
            mSequenceReader.StopRecord();
            mExecuteGUIReader.StopRecord();
            mMacroReader.StopRecord();
        }

        public void RecordSequence()
        {
            if (!mSequenceReader.IsRecord)
            {
                StopAllRecord();
                mSequenceReader.StartRecord();
            }
            else
                mSequenceReader.StopRecord();
        }

        public void RecordMacro()
        {
            if (!mMacroReader.IsRecord)
            {
                StopAllRecord();
                mMacroReader.StartRecord();
            }
            else
            {
                mMacroReader.StopRecord();
                for (int i = 0; i < 2; i++)
                    MacroCollection.RemoveAt(MacroCollection.Count - 1);
            }
        }

        public void CreateMacros()
        {
            StopAllRecord();

            if (SequenceCollection.Count == 0)
            {
                MessageBox.Show("Sequence is empty", "Error", MessageBoxButton.OK);
                return;
            }
            if (MacroCollection.Count == 0)
            {
                MessageBox.Show("Macros is empty", "Error", MessageBoxButton.OK);
                return;
            }
            if (MacrosModeCollection.Count == 0)
            {
                MessageBox.Show("Macros mode hotkey is empty", "Error", MessageBoxButton.OK);
                return;
            }

            ReturnFalseFuctionState<IInput> functionState = new ReturnFalseFuctionState<IInput>(mInputEqualityComparer);
            IEnumerable<ReturnFalseFuctionState<IInput>> functionStateEnum = Enumerable.Repeat(functionState, SequenceCollection.Count);
            Branch<IInput> branchSequence = new Branch<IInput>(SequenceCollection, functionStateEnum, mInputEqualityComparer);
            branchSequence.SetFunctionBranch(new SendKeyDelayFuctionState<IInput>(mKeySenderInput, mMouseSenderInput, MacroCollection.ToArray(), mInputEqualityComparer));

            mTreeSequence.AddState(branchSequence);

            Macros macro = new Macros(MacrosName, SequenceCollection, MacroCollection.Select(item => item.Data));
            MacrosCollection.Add(macro);

            MacrosName = string.Empty;
            SequenceCollection.Clear();
            MacroCollection.Clear();
        }

        public void CleanSequence() => SequenceCollection.Clear();

        public void CleanMacro() => MacroCollection.Clear();

        public void CleanMacros()
        {
            mTreeSequence.ClearTree();
            MacrosCollection.Clear();
        }

        public void RemoveSequence(IInput param) => SequenceCollection.Remove(param);

        public void RemoveMacro(InputDelay param) => MacroCollection.Remove(param);

        public void RemoveMacros(Macros param)
        {
            int index = MacrosCollection.IndexOf(param);
            mTreeSequence.RemoveAtState(index);
            MacrosCollection.Remove(param);
        }

        public void StopRecordExecuteGUI()
        {
            mExecuteGUIReader.StopRecord();
            if (GUICollection.Count == 0)
                return;

            mExecuteGUIBranch = new Branch<IInput>(GUICollection, mInputEqualityComparer);
            mTreeRoot.SetState(new List<Branch<IInput>> { mExecuteGUIBranch, mMacrosModeBranch });
        }

        public void StartRecordExecuteGUI()
        {
            StopAllRecord();
            mExecuteGUIReader.StartNewRecord();
        }

        public void StopRecordMacrosMode()
        {
            mMacrosModeReader.StopRecord();
            if (MacrosModeCollection.Count == 0)
                return;

            mMacrosModeBranch = new Branch<IInput>(MacrosModeCollection, mInputEqualityComparer);
            mMacrosModeBranch.AddState(mTreeSequence);
            mTreeRoot.SetState(new List<Branch<IInput>> { mExecuteGUIBranch, mMacrosModeBranch });
        }

        public void StartRecordMacrosMode()
        {
            StopAllRecord();
            mMacrosModeReader.StartNewRecord();
        }

        public void SetDefaultDelay()
        {
            int value = int.Parse(DelayDefault);
            for (int i = 0; i < MacroCollection.Count; i++)
            {
                InputDelay inputDelay = MacroCollection[i];
                MacroCollection[i] = new InputDelay(inputDelay.Data, value);
            }
        }
    }
}