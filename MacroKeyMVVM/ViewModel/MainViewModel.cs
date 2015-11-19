using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using MacroKey.InputData;
using MacroKey.LowLevelApi;
using MacroKey.LowLevelApi.Hook;
using MacroKey.Machine;
using MacroKey.Model.Machine;
using MacroKey.Properties;
using MacroKeyMVVM.Model.InputData;
using MacroKeyMVVM.Model.LowLevelApi.HookRead;

namespace MacroKey.ViewModel
{
    public class MainViewModel : ViewModelBase, IDisposable
    {
        private HookerMouse mHookerMouse { get; }
        private SenderInput mSednerInput { get; }
        private HookerKey mHookerKey { get; }

        private InputEqualityComparer mInputEqualityComparer { get; } = new InputEqualityComparer();
        private Tree<Input> mTreeRoot { get; }
        private Tree<Input> mTreeSequence { get; }
        private Branch<Input> mExecuteGUIBranch { get; set; }
        private Branch<Input> mMacrosModeBranch { get; set; }

        private DontStuckHookReader mSequenceReader { get; }
        private DontStuckHookReader mExecuteGUIReader { get; }
        private DontStuckHookReader mMacrosModeReader { get; }
        private DontStuckMultiHookReader mMacroReader { get; }

        public string MacrosName { get; set; }
        public ObservableCollection<Macros> MacrosCollection { get; }

        public string DelayDefault { get; set; }

        public RelayCommand RecordSequenceCommand { get; }
        public RelayCommand RecordMacroCommand { get; }
        public RelayCommand CreateMacrosCommand { get; }
        public RelayCommand CleanRowsSequenceCommand { get; }
        public RelayCommand CleanRowsMacroCommand { get; }
        public RelayCommand CleanRowsMacrosCommand { get; }
        public RelayCommand<Input> DeleteRowSequenceCommand { get; }
        public RelayCommand<InputDelay> DeleteRowMacroCommand { get; }
        public RelayCommand<Macros> DeleteRowMacrosCommand { get; }
        public RelayCommand StopRecordExecuteGUICommand { get; }
        public RelayCommand StartRecordExecuteGUICommand { get; }
        public RelayCommand StartRecordMacrosModeCommand { get; }
        public RelayCommand StopRecordMacrosModeCommand { get; }
        public RelayCommand StopAllRecordCommand { get; }
        public RelayCommand SetDefaultDelayCommand { get; }

        public ObservablePropertyCollection<Input> GUICollection { get; }
        public ObservablePropertyCollection<Input> MacrosModeCollection { get; }
        public ObservableCollection<Input> SequenceCollection { get; }
        public ObservableCollection<InputDelay> MacroCollection { get; }

        public MainViewModel()
        {
            mHookerKey = new HookerKey();
            mHookerMouse = new HookerMouse();
            mSednerInput = new SenderInput();

            mExecuteGUIBranch = new Branch<Input>(mInputEqualityComparer);
            mMacrosModeBranch = new Branch<Input>(mInputEqualityComparer);

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
            
            mSequenceReader = new DontStuckHookReader(mHookerKey, SequenceCollection);
            ObservableCollection<Input> tempCollection = new ObservableCollection<Input>(MacroCollection.Cast<Input>());
            tempCollection.CollectionChanged += ReadMacro_CollectionChanged;
            mMacroReader = new DontStuckMultiHookReader(new List<IHooker> { mHookerKey, mHookerMouse }, tempCollection);
            mExecuteGUIReader = new DontStuckHookReader(mHookerKey, GUICollection);
            mMacrosModeReader = new DontStuckHookReader(mHookerKey, MacrosModeCollection);

            RecordSequenceCommand = new RelayCommand(RecordSequence);
            RecordMacroCommand = new RelayCommand(RecordMacro);
            CreateMacrosCommand = new RelayCommand(CreateMacros);
            CleanRowsSequenceCommand = new RelayCommand(CleanRowsSequence);
            CleanRowsMacroCommand = new RelayCommand(CleanRowsMacro);
            CleanRowsMacrosCommand = new RelayCommand(CleanRowsMacros);
            DeleteRowSequenceCommand = new RelayCommand<Input>(DeleteRowSequence);
            DeleteRowMacroCommand = new RelayCommand<InputDelay>(DeleteRowMacro);
            DeleteRowMacrosCommand = new RelayCommand<Macros>(DeleteRowMacros);
            StopRecordExecuteGUICommand = new RelayCommand(StopRecordExecuteGUI);
            StartRecordExecuteGUICommand = new RelayCommand(StartRecordExecuteGUI);
            StartRecordMacrosModeCommand = new RelayCommand(StartRecordMacrosMode);
            StopRecordMacrosModeCommand = new RelayCommand(StopRecordMacrosMode);
            SetDefaultDelayCommand = new RelayCommand(SetDefaultDelay);
            StopAllRecordCommand = new RelayCommand(StopAllRecord);

            mHookerKey.SetHook();

            StateWalker<Input> machineWalker = new StateWalker<Input>(mTreeRoot);
            mHookerKey.Hooked += (arg) =>
            {
                State<Input> currentState = machineWalker.WalkStates(arg);
                return currentState is FunctionState<Input> ? (bool)((FunctionState<Input>)currentState).ExecuteFunction() : true;
            };
        }

        private void ReadMacro_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                int value = int.Parse(DelayDefault);
                foreach (var item in e.NewItems)
                    MacroCollection.Add(new InputDelay((Input)item, value));
            }
        }

        public override void Cleanup()
        {
            Dispose();
            Settings.Default.Save();
        }

        public void Dispose() => mHookerKey.Unhook();

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
                StopAllRecord();
        }

        public void RecordMacro()
        {
            if (!mMacroReader.IsRecord)
            {
                StopAllRecord();
                mMacroReader.StartRecord();
            }
            else
                StopAllRecord();
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

            ReturnFalseFuctionState<Input> functionState = new ReturnFalseFuctionState<Input>(mInputEqualityComparer);
            IEnumerable<ReturnFalseFuctionState<Input>> functionStateEnum = Enumerable.Repeat(functionState, SequenceCollection.Count);
            Branch<Input> branchSequence = new Branch<Input>(SequenceCollection, functionStateEnum, mInputEqualityComparer);
            branchSequence.SetFunctionBranch(new SendKeyDelayFuctionState<Input>(mSednerInput, MacroCollection.ToArray(), mInputEqualityComparer));

            mTreeSequence.AddState(branchSequence);

            Macros macro = new Macros(MacrosName, SequenceCollection, MacroCollection.Select(item => item.Data));
            MacrosCollection.Add(macro);

            MacrosName = string.Empty;
            SequenceCollection.Clear();
            MacroCollection.Clear();
        }

        public void CleanRowsSequence() => SequenceCollection.Clear();

        public void CleanRowsMacro() => MacroCollection.Clear();

        public void CleanRowsMacros()
        {
            mTreeSequence.ClearTree();
            MacrosCollection.Clear();
        }

        public void DeleteRowSequence(Input param) => SequenceCollection.Remove(param);

        public void DeleteRowMacro(InputDelay param) => MacroCollection.Remove(param);

        public void DeleteRowMacros(Macros param)
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

            mExecuteGUIBranch = new Branch<Input>(GUICollection, mInputEqualityComparer);
            mTreeRoot.SetState(new List<Branch<Input>> { mExecuteGUIBranch, mMacrosModeBranch });
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

            mMacrosModeBranch = new Branch<Input>(MacrosModeCollection, mInputEqualityComparer);
            mMacrosModeBranch.AddState(mTreeSequence);
            mTreeRoot.SetState(new List<Branch<Input>> { mExecuteGUIBranch, mMacrosModeBranch });
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