using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MacroKey.Machine;
using MacroKey.Key;

namespace MacroKey
{
    public partial class MainWindow : Window
    {
        private static HookerKeys mHookerKey = new HookerKeys();
        private static SenderKeyInput mSenderKey = new SenderKeyInput();

        private Tree<KeyData> mTree = new Tree<KeyData>();

        private Branch<KeyData> mHotkeyExecuteGUIBranch;
        private Branch<KeyData> mHotkeyMacrosModeBranch;

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

            TreeWalker<KeyData> machineWalker = new TreeWalker<KeyData>(mTree);
            mHookerKey.HookedKey += (obj) =>
            {
                State<KeyData> currentState = machineWalker.WalkStates(new KeyData(obj.VirtualKeyCode, obj.KeyboardMassage, obj.Time));
                if (currentState is FunctionalState<KeyData>)
                {
                    FunctionalState<KeyData> functionalState = (FunctionalState<KeyData>)currentState;
                    return (bool)functionalState.FunctionState(functionalState.FunctionArg);
                }
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
                StopAllRecord();
                buttonSender.Content = "Stop Record";
                HookSequenceReader hookSequenceReader = (HookSequenceReader)buttonSender.DataContext;
                hookSequenceReader.StartRecord();
            }
            else
                StopAllRecord();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void StopAllRecord()
        {
            recordSequenceButton.Content = "Record Sequence";
            mSequenceCollectionReader.StopRecord();
            recordMacroButton.Content = "Record Sequence";
            mMacroCollectionReader.StopRecord();
        }

        private void TextBoxMacrosName_GotFocus(object sender, RoutedEventArgs e)   
        {
            StopAllRecord();
        }

        private void HotkeyBox_GotFocus(object sender, RoutedEventArgs e)
        {
            StopAllRecord();

            FrameworkElement element = (FrameworkElement)sender;
            HookSequenceReader reader = (HookSequenceReader)element.DataContext;
            reader.StartNewRecord();
        }

        private void HotkeyBoxExecuteGUI_LostFocus(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            HookSequenceReader reader = (HookSequenceReader)element.DataContext;
            reader.StopRecord();

            Branch<KeyData> hotkeyExecureGUIBranch = new Branch<KeyData>(
                reader.ReadSequence,
                obj =>
                {
                    if (Visibility == Visibility.Collapsed)
                    {
                        Visibility = Visibility.Visible;
                        Focus();
                    }
                    else
                        Visibility = Visibility.Collapsed;
                    return true;
                });
            mHotkeyExecuteGUIBranch = hotkeyExecureGUIBranch;
        }

        private void HotkeyBoxMacrosMode_LostFocus(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            HookSequenceReader reader = (HookSequenceReader)element.DataContext;
            reader.StopRecord();

            Branch<KeyData> hotkeyMacrosModeBranch = new Branch<KeyData>(reader.ReadSequence);
            mHotkeyMacrosModeBranch = hotkeyMacrosModeBranch;
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
            StopAllRecord();

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
            Branch<KeyData> branchSequence = new Branch<KeyData>(
                mSequenceCollectionReader.ReadSequence,
                obj =>
                {
                    mSenderKey.SendKeyPress((KeyData[])obj);
                    return false;
                },
                mMacroCollectionReader.ReadSequence.ToArray());
            mTree.AddBranch(Branch<KeyData>.MergeBranches(mHotkeyMacrosModeBranch, branchSequence));
            Macros macro = new Macros(textBoxMacrosName.Text, mSequenceCollectionReader.ReadSequence, mMacroCollectionReader.ReadSequence);
            mMacrosCollection.Add(macro);
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

            int index = mMacrosCollection.IndexOf(macro);
            mTree.RemoveBranch(index);

            mMacrosCollection.Remove(macro);
        }

        private void CleanRowsMacrosButton_Click(object sender, RoutedEventArgs e)
        {
            mTree.ClearTree();
            mMacrosCollection.Clear();
        }
    }
}