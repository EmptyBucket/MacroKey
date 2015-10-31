using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MacroKey
{
    public partial class MainWindow : Window
    {
        private static Machine<KeyData> mMachine = new Machine<KeyData>(new State<KeyData>());
        private static HookerKeys mHookerKey = new HookerKeys();
        private static SenderKeyInput mSenderKey = new SenderKeyInput();

        private State<KeyData> mStartSequenceState = new State<KeyData>();

        private HookSequenceReader mHotkeyExecuteGUIReader = new HookSequenceReader(mHookerKey);
        private HookSequenceReader mHotkeyMacrosModeReader = new HookSequenceReader(mHookerKey);
        private HookSequenceReader mSequenceCollectionReader = new HookSequenceReader(mHookerKey);
        private HookSequenceReader mMacroCollectionReader = new HookSequenceReader(mHookerKey);
        private ObservableCollection<Macros> mMacrosCollection = new ObservableCollection<Macros>();

        public MainWindow()
        {
            InitializeComponent();
            Loaded += Page_Loaded;
        }

        void Page_Loaded(object sender, RoutedEventArgs e)
        {
            mHookerKey.SetHook();

            MachineWalker<KeyData> machineWalker = new MachineWalker<KeyData>(mMachine);
            mHookerKey.HookedKey += (obj) =>
            {
                State<KeyData> state = machineWalker.WalkMachine(new KeyData(obj.VirtualKeyCode, obj.KeyboardMassage, obj.Time));
                if (state.ActionState != null)
                    return (bool)state.ActionState(state.ActionArg);
                else
                    return true;
            };
            InitializeDataContext();
        }

        private void InitializeDataContext()
        {
            recordSequenceButton.DataContext = mSequenceCollectionReader;
            listSequenceKey.DataContext = mSequenceCollectionReader.ReadSequence;
            recordMacroButton.DataContext = mMacroCollectionReader;
            listMacroKey.DataContext = mMacroCollectionReader.ReadSequence;
            listMacros.DataContext = mMacrosCollection;

            executeGUIHotkeyBox.DataContext = mHotkeyExecuteGUIReader;
            macrosModHotkeyBox.DataContext = mHotkeyMacrosModeReader;
        }

        private void StartRecordSequence_Click(object sender, RoutedEventArgs e)
        {
            Button buttonSender = (Button)sender;
            if (buttonSender.Content.ToString() == "Record Sequence")
            {
                StopRecord();
                buttonSender.Content = "Stop Record";
                HookSequenceReader hookSequenceReader = (HookSequenceReader)buttonSender.DataContext;
                hookSequenceReader.StartRecord();
            }
            else
                StopRecord();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void StopRecord()
        {
            recordSequenceButton.Content = "Record Sequence";
            mSequenceCollectionReader.StopRecord();
            recordMacroButton.Content = "Record Sequence";
            mMacroCollectionReader.StopRecord();
        }

        private void TextBoxMacrosName_GotFocus(object sender, RoutedEventArgs e)   
        {
            StopRecord();
        }

        private void HotkeyBox_GotFocus(object sender, RoutedEventArgs e)
        {
            StopRecord();

            FrameworkElement element = (FrameworkElement)sender;
            HookSequenceReader reader = (HookSequenceReader)element.DataContext;
            reader.Clear();
            reader.StartRecord();
        }

        private void HotkeyBoxExecuteGUI_LostFocus(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            HookSequenceReader reader = (HookSequenceReader)element.DataContext;
            reader.StopRecord();

            State<KeyData> startState = new State<KeyData>();
            State<KeyData> endState = State<KeyData>.CreateBranch(reader.ReadSequence, startState);
            endState.ActionState = obj => 
            {
                if (Visibility == Visibility.Collapsed)
                {
                    Visibility = Visibility.Visible;
                    Focus();
                }
                else
                    Visibility = Visibility.Collapsed;
                return true;
            };
            mMachine.AddBranchToStart(startState);
        }

        private void HotkeyBoxMacrosMode_LostFocus(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            HookSequenceReader reader = (HookSequenceReader)element.DataContext;
            reader.StopRecord();

            State<KeyData> startState = new State<KeyData>();
            State<KeyData> endState = State<KeyData>.CreateBranch(reader.ReadSequence, startState);
            mMachine.AddBranchToStart(startState);

            mStartSequenceState = endState;
        }

        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);
            if (parentObject == null) return null;
            T parent = parentObject as T;
            if (parent != null)
                return parent;
            else
                return FindParent<T>(parentObject);
        }

        private void CreateMacros_Click(object sender, RoutedEventArgs e)
        {
            StopRecord();

            if (mSequenceCollectionReader.ReadSequence.Count == 0)
            {
                MessageBox.Show("Sequence is empty", "Error", MessageBoxButton.OK);
                return;
            }
            if(mMacroCollectionReader.ReadSequence.Count == 0)
            {
                MessageBox.Show("Macros is empty", "Error", MessageBoxButton.OK);
                return;
            }
            if (macrosModHotkeyBox.Text == string.Empty)
            {
                MessageBox.Show("Macros mode hotkey is empty", "Error", MessageBoxButton.OK);
                return;
            }

            State<KeyData> startState = new State<KeyData>();
            State<KeyData> endState = State<KeyData>.CreateBranch(mSequenceCollectionReader.ReadSequence, startState, Enumerable.Repeat(new Func<object, object>((obj) => false), mSequenceCollectionReader.ReadSequence.Count));
            endState.ActionArg = new List<KeyData>(mMacroCollectionReader.ReadSequence);
            endState.ActionState = obj =>
            {
                mSenderKey.SendKeyPress((IEnumerable<KeyData>)obj);
                return false;
            };
            mMachine.AddBranchToCurrent(startState, mStartSequenceState);

            mMacrosCollection.Add(new Macros(textBoxMacrosName.Text, mSequenceCollectionReader.ReadSequence, mMacroCollectionReader.ReadSequence));
            textBoxMacrosName.Clear();
            mSequenceCollectionReader.Clear();
            mMacroCollectionReader.Clear();
        }

        private void DeleteRowSequenceButton_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            ListView listView = FindParent<ListView>(element);
            ObservableKeyDataCollection listViewDataContext = (ObservableKeyDataCollection)listView.DataContext;
            listViewDataContext.Remove((KeyData)element.DataContext);
        }

        private void CleanRowsSequenceButton_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            ListView listView = FindParent<ListView>(element);
            ObservableKeyDataCollection listViewDataContext = (ObservableKeyDataCollection)listView.DataContext;
            listViewDataContext.Clear();
        }

        private void DeleteRowMacrosButton_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            Macros macro = (Macros)element.DataContext;
            ListView listView = FindParent<ListView>(element);
            ObservableCollection<Macros> listViewDataContext = (ObservableCollection<Macros>)listView.DataContext;
            mMachine.RemoveBranchFromCurrent(macro.Sequence[0], mStartSequenceState);
            listViewDataContext.Remove(macro);
        }

        private void CleanRowsMacrosButton_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            ListView listView = FindParent<ListView>(element);
            ObservableCollection<Macros> listViewDataContext = (ObservableCollection<Macros>)listView.DataContext;
            mMachine.ClearBranchFromCurrent(mStartSequenceState);
            listViewDataContext.Clear();
        }
    }
}