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
        private static ObservableCollection<KeyData> m_sequenceCollection = new ObservableCollection<KeyData>();
        private static ObservableCollection<KeyData> m_macroCollection = new ObservableCollection<KeyData>();
        private static ObservableCollection<Macros> m_macrosList = new ObservableCollection<Macros>();

        private static Machine<KeyData> mMachine;
        private static short mKeyExecuteGUI;
        private static short mKeyMacrosMode;
        private HookerKeys mHookerKey;
        private SenderKeyInput mSenderKey;
        private MachineWalker<KeyData> mMachineWalker;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += Page_Loaded;
        }

        public bool m_hookSequence(HookerKeys.KeyHookEventArgs e)
        {
            KeyData keyData = new KeyData(e.VirtualKeyCode, e.ScanCode, e.Flags, e.KeyboardMassage);
            State <KeyData> state = mMachineWalker.WalkMachine(keyData);
            if (state != null)
            {
                if (state.ActionState != null)
                {
                    state.ActionState(state.ActionArg);
                    mHookerKey.HookedKey -= m_hookSequence;
                }
                if (e.VirtualKeyCode == mKeyMacrosMode)
                    mHookerKey.HookedKey -= m_hookSequence;
            }
            else
                mHookerKey.HookedKey -= m_hookSequence;
            return true;
        }

        void Page_Loaded(object sender, RoutedEventArgs e)
        {
            mMachine = new Machine<KeyData>(new State<KeyData>());
            mHookerKey = new HookerKeys();
            mSenderKey = new SenderKeyInput();
            mHookerKey.SetHook();

            listSequenceKey.DataContext = m_sequenceCollection;
            listMacro.DataContext = m_macroCollection;
            listMacros.DataContext = m_macrosList;

            bool goToHookSequence = false;
            mHookerKey.HookedKey += (args) =>
            {
                if (args.VirtualKeyCode == mKeyExecuteGUI && args.KeyboardMassage == (int)KeyData.KeyboardMessage.WM_KEYDOWM)
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
                    mMachineWalker = new MachineWalker<KeyData>(mMachine);
                    mHookerKey.HookedKey += new HookerKeys.KeyHookHandler(m_hookSequence);
                }
                if (args.VirtualKeyCode == mKeyMacrosMode && args.KeyboardMassage == (int)KeyData.KeyboardMessage.WM_KEYDOWM)
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
                mHookerKey.HookedKey += new HookerKeys.KeyHookHandler(m_recordSequence);
                buttonSender.Content = "Stop";
                recordMacroButton.Content = "Record";
                mHookerKey.HookedKey -= new HookerKeys.KeyHookHandler(m_recordMacro);
            }
            else
            {
                mHookerKey.HookedKey -= new HookerKeys.KeyHookHandler(m_recordSequence);
                buttonSender.Content = "Record";
            }
        }

        private void StartRecordMacros_Click(object sender, RoutedEventArgs e)
        {
            Button buttonSender = (Button)sender;
            if (buttonSender.Content.ToString() == "Record")
            {
                mHookerKey.HookedKey += new HookerKeys.KeyHookHandler(m_recordMacro);
                buttonSender.Content = "Stop";
                recordSequenceButton.Content = "Record";
                mHookerKey.HookedKey -= new HookerKeys.KeyHookHandler(m_recordSequence);
            }
            else
            {
                mHookerKey.HookedKey -= new HookerKeys.KeyHookHandler(m_recordMacro);
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
            mMachine.AddBranch(saveStartState);

            m_macrosList.Add(new Macros(new List<KeyData>(m_sequenceCollection), new List<KeyData>(m_macroCollection)));
            m_sequenceCollection.Clear();
            m_macroCollection.Clear();
        }

        private void GUIHotkeyBox_KeyDown(object sender, KeyEventArgs e)
        {
            mKeyExecuteGUI = (short)KeyInterop.VirtualKeyFromKey(e.Key);
            TextBox tb = (TextBox)sender;
            tb.Text = e.Key.ToString();
            tb.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }

        private void macrosModeBox_KeyDown(object sender, KeyEventArgs e)
        {
            mKeyMacrosMode = (short)KeyInterop.VirtualKeyFromKey(e.Key);
            TextBox tb = (TextBox)sender;
            tb.Text = e.Key.ToString();
            tb.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }

        private void Element_GotFocus(object sender, RoutedEventArgs e)
        {
            recordSequenceButton.Content = "Record";
            mHookerKey.HookedKey -= new HookerKeys.KeyHookHandler(m_recordSequence);
            recordMacroButton.Content = "Record";
            mHookerKey.HookedKey -= new HookerKeys.KeyHookHandler(m_recordMacro);
        }

        private childItem FindVisualChild<childItem>(DependencyObject obj)
        where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is childItem)
                    return (childItem)child;
                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }

        private void ListElement_GotFocus(object sender, RoutedEventArgs e)
        {
            ListBoxItem item = (ListBoxItem)sender;
            ContentPresenter contentPresenter = FindVisualChild<ContentPresenter>(item);
            DataTemplate dataTemplate = contentPresenter.ContentTemplate;
            StackPanel stackPanel = (StackPanel)dataTemplate.FindName("listSequenceKey", contentPresenter);
            Button removeElementButton = new Button();
            removeElementButton.Click += (send, arg) =>
            {
                Panel parent = (Panel)VisualTreeHelper.GetParent(stackPanel);
                parent.Children.Remove(stackPanel);
            };
            stackPanel.Children.Add(removeElementButton);
        }

        private void ListElement_LostFocus(object sender, RoutedEventArgs e)
        {
            StackPanel stackPanel = (StackPanel)sender;
            foreach (UIElement item in stackPanel.Children)
                if (item.GetType() == typeof(Button))
                    stackPanel.Children.Remove(item);
        }
    }
}
