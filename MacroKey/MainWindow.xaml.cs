using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MacroKey
{
    public partial class MainWindow : Window
    {
        private static ObservableCollection<KeyData> m_sequenceCollection = new ObservableCollection<KeyData>();
        private static ObservableCollection<KeyData> m_macroCollection = new ObservableCollection<KeyData>();
        private static ObservableCollection<Macros> m_macrosList = new ObservableCollection<Macros>();

        private static Machine<KeyData> m_machine;
        private static short m_keyExecuteGUI;
        private static short m_keyMacrosMode;
        private HookerKeys m_hookerKey;
        private SenderKeyInput mSenderKey;
        private MachineWalker<KeyData> m_machineWalker;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += Page_Loaded;
        }

        public bool m_hookSequence(HookerKeys.KeyHookEventArgs e)
        {
            KeyData keyData = new KeyData(e.VirtualKeyCode, e.ScanCode, e.Flags, e.KeyboardMassage);
            State <KeyData> state = m_machineWalker.WalkMachine(keyData);
            if (state != null)
            {
                if (state.ActionState != null)
                {
                    state.ActionState(state.ActionArg);
                    m_hookerKey.HookedKey -= m_hookSequence;
                }
                if (e.VirtualKeyCode == m_keyMacrosMode)
                    m_hookerKey.HookedKey -= m_hookSequence;
            }
            else
                m_hookerKey.HookedKey -= m_hookSequence;
            return true;
        }

        void Page_Loaded(object sender, RoutedEventArgs e)
        {
            m_machine = new Machine<KeyData>(new State<KeyData>());
            m_hookerKey = new HookerKeys();
            mSenderKey = new SenderKeyInput();
            m_hookerKey.SetHook();

            listSequenceKey.DataContext = m_sequenceCollection;
            listMacro.DataContext = m_macroCollection;
            listMacros.DataContext = m_macrosList;

            bool goToHookSequence = false;
            m_hookerKey.HookedKey += (args) =>
            {
                if (args.VirtualKeyCode == m_keyExecuteGUI && args.KeyboardMassage == (int)KeyData.KeyboardMessage.WM_KEYDOWM)
                    if (IsVisible)
                        Visibility = Visibility.Collapsed;
                    else
                    {
                        Visibility = Visibility.Visible;
                        Activate();
                    }
                if(goToHookSequence)
                {
                    goToHookSequence = false;
                    m_machineWalker = new MachineWalker<KeyData>(m_machine);
                    m_hookerKey.HookedKey += new HookerKeys.KeyHookHandler(m_hookSequence);
                }
                if (args.VirtualKeyCode == m_keyMacrosMode && args.KeyboardMassage == (int)KeyData.KeyboardMessage.WM_KEYDOWM)
                    goToHookSequence = true;
                
                return true;
            };
        }

        private bool m_recordSequence(HookerKeys.KeyHookEventArgs e)
        {
            m_sequenceCollection.Add(new KeyData(e.VirtualKeyCode, e.ScanCode, e.Flags, e.KeyboardMassage));
            return true;
        }

        private bool m_recordMacro(HookerKeys.KeyHookEventArgs e)
        {
            m_macroCollection.Add(new KeyData(e.VirtualKeyCode, e.ScanCode, e.Flags, e.KeyboardMassage));
            return true;
        }

        private void StartRecordSequence_Click(object sender, RoutedEventArgs e)
        {
            Button buttonSender = (Button)sender;
            if (buttonSender.Content.ToString() == "Record")
            {
                m_hookerKey.HookedKey += new HookerKeys.KeyHookHandler(m_recordSequence);
                buttonSender.Content = "Stop";
                recordMacroButton.Content = "Record";
                m_hookerKey.HookedKey -= new HookerKeys.KeyHookHandler(m_recordMacro);
            }
            else
            {
                m_hookerKey.HookedKey -= new HookerKeys.KeyHookHandler(m_recordSequence);
                buttonSender.Content = "Record";
            }
        }

        private void StartRecordMacros_Click(object sender, RoutedEventArgs e)
        {
            Button buttonSender = (Button)sender;
            if (buttonSender.Content.ToString() == "Record")
            {
                m_hookerKey.HookedKey += new HookerKeys.KeyHookHandler(m_recordMacro);
                buttonSender.Content = "Stop";
                recordSequenceButton.Content = "Record";
                m_hookerKey.HookedKey -= new HookerKeys.KeyHookHandler(m_recordSequence);
            }
            else
            {
                m_hookerKey.HookedKey -= new HookerKeys.KeyHookHandler(m_recordMacro);
                buttonSender.Content = "Record";
            }
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void CreateMacros_Click(object sender, RoutedEventArgs e)
        {
            if(m_sequenceCollection.Count == 0)
            {
                MessageBox.Show("Sequence is empty", "Error", MessageBoxButton.OK);
                return;
            }
            if(m_macroCollection.Count == 0)
            {
                MessageBox.Show("Macros is empty", "Error", MessageBoxButton.OK);
                return;
            }

            State<KeyData> currentState = new State<KeyData>();
            State<KeyData> saveStartState = currentState;
            foreach (var item in m_sequenceCollection)
            {
                State<KeyData> newState = new State<KeyData>();
                currentState.AddNextState(item, newState);
                currentState = newState;
            }
            currentState.ActionArg = new List<KeyData>(m_macroCollection);
            currentState.ActionState = obj => mSenderKey.SendKeyPress((IEnumerable<KeyData>)obj);
            m_machine.AddBranch(saveStartState);

            m_macrosList.Add(new Macros(new List<KeyData>(m_sequenceCollection), new List<KeyData>(m_macroCollection)));
            m_sequenceCollection.Clear();
            m_macroCollection.Clear();
        }

        private void GUIHotkeyBox_KeyDown(object sender, KeyEventArgs e)
        {
            m_keyExecuteGUI = (short)KeyInterop.VirtualKeyFromKey(e.Key);
            TextBox tb = (TextBox)sender;
            tb.Text = e.Key.ToString();
            tb.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }

        private void macrosModeBox_KeyDown(object sender, KeyEventArgs e)
        {
            m_keyMacrosMode = (short)KeyInterop.VirtualKeyFromKey(e.Key);
            TextBox tb = (TextBox)sender;
            tb.Text = e.Key.ToString();
            tb.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }

        private void Element_GotFocus(object sender, RoutedEventArgs e)
        {
            recordSequenceButton.Content = "Record";
            m_hookerKey.HookedKey -= new HookerKeys.KeyHookHandler(m_recordSequence);
            recordMacroButton.Content = "Record";
            m_hookerKey.HookedKey -= new HookerKeys.KeyHookHandler(m_recordMacro);
        }
    }
}
