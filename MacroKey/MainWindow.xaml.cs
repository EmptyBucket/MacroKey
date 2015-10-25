using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        private MachineWalker<KeyData> mMachineWalker = new MachineWalker<KeyData>(mMachine);

        private SequenceReader mHotkeyExecuteGUI = new SequenceReader(mHookerKey);
        private SequenceReader mHotkeyMacrosMode = new SequenceReader(mHookerKey);
        private SequenceReader mSequenceCollection = new SequenceReader(mHookerKey);
        private SequenceReader mMacroCollection = new SequenceReader(mHookerKey);
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
                State<KeyData> state = machineWalker.WalkMachine(new KeyData(obj.VirtualKeyCode, obj.ScanCode, obj.Flags, obj.KeyboardMassage));
                if (state.ActionState != null)
                    state.ActionState(state.ActionArg);

                return true;
            };

            listSequenceKey.DataContext = mSequenceCollection;
            listMacroKey.DataContext = mMacroCollection;
            listMacros.DataContext = mMacrosCollection;

            executeGUIHotkeyBox.DataContext = mHotkeyExecuteGUI;
            macrosModHotkeyBox.DataContext = mHotkeyMacrosMode;

            EventManager.RegisterClassHandler(typeof(UIElement), AccessKeyManager.AccessKeyPressedEvent, new AccessKeyPressedEventHandler(OnAccessKeyPressed));
        }

        private static void OnAccessKeyPressed(object sender, AccessKeyPressedEventArgs e)
        {
            if (!e.Handled && e.Scope == null && (e.Target == null || e.Target.GetType() == typeof(Label)))
            {
                if ((Keyboard.Modifiers & ModifierKeys.Alt) != ModifierKeys.Alt)
                {
                    e.Target = null;
                    e.Handled = true;
                }
            }
        }

        private void StartRecordSequence_Click(object sender, RoutedEventArgs e)
        {
            Button buttonSender = (Button)sender;
            if (buttonSender.Content.ToString() == "Record _Sequence")
            {
                mSequenceCollection.StartRecord();
                buttonSender.Content = "Stop _Sequence";
                recordMacroButton.Content = "Record _Macro";
                mMacroCollection.StopRecord();
            }
            else
            {
                mSequenceCollection.StopRecord();
                buttonSender.Content = "Record _Sequence";
            }
        }

        private void StartRecordMacros_Click(object sender, RoutedEventArgs e)
        {
            Button buttonSender = (Button)sender;
            if (buttonSender.Content.ToString() == "Record _Macro")
            {
                mMacroCollection.StartRecord();
                buttonSender.Content = "Stop _Macro";
                recordSequenceButton.Content = "Record _Sequence";
                mSequenceCollection.StopRecord();
            }
            else
            {
                mMacroCollection.StopRecord();
                buttonSender.Content = "Record _Macro";
            }
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void CreateMacros_Click(object sender, RoutedEventArgs e)
        {
            recordSequenceButton.Content = "Record _Sequence";
            mSequenceCollection.StopRecord();
            recordMacroButton.Content = "Record _Macro";
            mMacroCollection.StopRecord();

            if (mSequenceCollection.ReadSequence.Count == 0)
            {
                MessageBox.Show("Sequence is empty", "Error", MessageBoxButton.OK);
                return;
            }
            if(mMacroCollection.ReadSequence.Count == 0)
            {
                MessageBox.Show("Macros is empty", "Error", MessageBoxButton.OK);
                return;
            }

            State<KeyData> startState = new State<KeyData>();
            State<KeyData> endState = State<KeyData>.CreateBranch(mSequenceCollection.ReadSequence, startState);
            endState.ActionArg = new List<KeyData>(mMacroCollection.ReadSequence);
            endState.ActionState = obj => mSenderKey.SendKeyPress((IEnumerable<KeyData>)obj);
            mMachine.AddBranchToCurrent(startState, mMachineWalker.StartState);

            mMacrosCollection.Add(new Macros(textBoxMacrosName.Text, mSequenceCollection.ReadSequence, mMacroCollection.ReadSequence));
            textBoxMacrosName.Clear();
            mSequenceCollection.Clear();
            mMacroCollection.Clear();
        }

        private void HotkeyBoxExecuteGUI_GotFocus(object sender, RoutedEventArgs e)
        {
            recordSequenceButton.Content = "Record _Sequence";
            mSequenceCollection.StopRecord();
            recordMacroButton.Content = "Record _Macro";
            mMacroCollection.StopRecord();

            FrameworkElement element = (FrameworkElement)sender;
            SequenceReader reader = (SequenceReader)element.DataContext;
            reader.Clear();
            reader.StartRecord();
        }

        private void HotkeyBoxExecuteGUI_LostFocus(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            SequenceReader reader = (SequenceReader)element.DataContext;
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
            };
            mMachine.AddBranchToStart(startState);
        }

        private void HotkeyBoxMacrosMode_GotFocus(object sender, RoutedEventArgs e)
        {
            recordSequenceButton.Content = "Record _Sequence";
            mSequenceCollection.StopRecord();
            recordMacroButton.Content = "Record _Macro";
            mMacroCollection.StopRecord();

            FrameworkElement element = (FrameworkElement)sender;
            SequenceReader reader = (SequenceReader)element.DataContext;
            reader.Clear();
            reader.StartRecord();
        }

        private void HotkeyBoxMacrosMode_LostFocus(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            SequenceReader reader = (SequenceReader)element.DataContext;
            reader.StopRecord();

            State<KeyData> startState = new State<KeyData>();
            State<KeyData> endState = State<KeyData>.CreateBranch(reader.ReadSequence, startState);
            mMachine.AddBranchToStart(startState);

            mMachineWalker.StartState = endState;
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

        private void DeleteRowButton_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            ListView listView = FindParent<ListView>(element);
            object listViewDataContext = listView.DataContext;
            Type typeDataContext = listViewDataContext.GetType();
            typeDataContext.GetMethod("Remove").Invoke(listViewDataContext, new object[] { element.DataContext });
        }

        private void CleanRowsButton_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            ListView listView = FindParent<ListView>(element);
            object listViewDataContext = listView.DataContext;
            Type typeDataContext = listViewDataContext.GetType();
            typeDataContext.GetMethod("Clear").Invoke(listViewDataContext, new object[] { });
        }
    }
}
