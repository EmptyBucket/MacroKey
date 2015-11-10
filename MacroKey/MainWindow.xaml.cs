using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MacroKey.Machine;
using MacroKey.Keyboard;
using MacroKey.LowLevelApi;
using System.Collections.Generic;
using System;
using MacroKey.LowLevelApi.Hook;
using MacroKey.LowLevelApi.HookReader;
using System.Collections.Specialized;

namespace MacroKey
{
    public partial class MainWindow : Window
    {
        private static HookerKey mHookerKey = new HookerKey();
        private static SenderKeyInput mSenderKey = new SenderKeyInput();
        private static KeyDataEqualityComparer mKeyDataEqualityComparer = new KeyDataEqualityComparer();

        private Tree<KeyData> mTreeRoot = new Tree<KeyData>(mKeyDataEqualityComparer);
        private Tree<KeyData> mTreeSequence = new Tree<KeyData>(mKeyDataEqualityComparer);
        private List<Branch<KeyData>> listBranchSequence = new List<Branch<KeyData>>();

        private Branch<KeyData> mHotkeyExecuteGUIBranch = new Branch<KeyData>(mKeyDataEqualityComparer);
        private Branch<KeyData> mHotkeyMacrosModeBranch = new Branch<KeyData>(mKeyDataEqualityComparer);

        private HookKeyDataReader mHotkeyExecuteGUIReader = new HookKeyDataReader(mHookerKey);
        private HookKeyDataReader mHotkeyMacrosModeReader = new HookKeyDataReader(mHookerKey);
        private HookKeyDataReader mSequenceCollectionReader = new HookKeyDataReader(mHookerKey);
        private HookKeyDataDelayReader mMacroCollectionReader = new HookKeyDataDelayReader(mHookerKey);

        private ObservableCollection<Macros> mMacrosCollection = new ObservableCollection<Macros>();

        public MainWindow()
        {
            InitializeComponent();
            Loaded += Page_Loaded;
        }

        void Page_Loaded(object sender, RoutedEventArgs e)
        {
            mHookerKey.SetHook();

            TreeWalker<KeyData> machineWalker = new TreeWalker<KeyData>(mTreeRoot);
            mHookerKey.Hooked += (obj) =>
            {
                var arg = (KeyHookEventArgs)obj;
                State<KeyData> currentState = machineWalker.WalkStates(new KeyData(arg.VirtualKeyCode, arg.KeyMassage, arg.Time));
                if (currentState is FunctionalState<KeyData>)
                {
                    FunctionalState<KeyData> functionalState = (FunctionalState<KeyData>)currentState;
                    return (bool)functionalState.FunctionState(functionalState.FunctionArg);
                }
                else
                    return true;
            };
            InitializeDataContext();
            mMacroCollectionReader.ReadSequence.CollectionChanged += ReadSequence_CollectionChanged;
        }

        private void ReadSequence_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            int value = delayDefault.Text == string.Empty ? 0 : int.Parse(delayDefault.Text);
            if (e.NewItems != null)
            foreach (var item in e.NewItems)
                ((KeyDataDelay)item).Delay = value;
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
                HookKeyDataReader hookSequenceReader = (HookKeyDataReader)buttonSender.DataContext;
                hookSequenceReader.StartRecord();
            }
            else
                StopAllRecord();
        }

        private void StartRecordMacro_Click(object sender, RoutedEventArgs e)
        {
            Button buttonSender = (Button)sender;
            if (buttonSender.Content.ToString() == "Record Sequence")
            {
                StopAllRecord();
                buttonSender.Content = "Stop Record";
                HookKeyDataDelayReader hookSequenceReader = (HookKeyDataDelayReader)buttonSender.DataContext;
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
            HookKeyDataReader reader = (HookKeyDataReader)element.DataContext;
            reader.StartNewRecord();
        }

        private void HotkeyBoxExecuteGUI_LostFocus(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            HookKeyDataReader reader = (HookKeyDataReader)element.DataContext;
            reader.StopRecord();

            if (reader.ReadSequence.Count == 0)
            {
                MessageBox.Show("Execute GUI hotkey is empty", "Error", MessageBoxButton.OK);
                return;
            }

            mHotkeyExecuteGUIBranch = new Branch<KeyData>(
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
                },
                mKeyDataEqualityComparer);
            mTreeRoot.SetPart(new List<Branch<KeyData>> { mHotkeyExecuteGUIBranch, mHotkeyMacrosModeBranch });
        }

        private void HotkeyBoxMacrosMode_LostFocus(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            HookKeyDataReader reader = (HookKeyDataReader)element.DataContext;
            reader.StopRecord();

            if (reader.ReadSequence.Count == 0)
            {
                MessageBox.Show("Macros mode hotkey is empty", "Error", MessageBoxButton.OK);
                return;
            }

            mHotkeyMacrosModeBranch = new Branch<KeyData>(reader.ReadSequence, mKeyDataEqualityComparer);
            mHotkeyMacrosModeBranch.AddPart(mTreeSequence);
            mTreeRoot.SetPart(new List<Branch<KeyData>> { mHotkeyMacrosModeBranch, mHotkeyExecuteGUIBranch });
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
            IEnumerable<Func<object, object>> functions = Enumerable.Repeat<Func<object, object>>(obj => false, mSequenceCollectionReader.ReadSequence.Count);
            Branch<KeyData> branchSequence = new Branch<KeyData>(mSequenceCollectionReader.ReadSequence, functions, mKeyDataEqualityComparer);
            branchSequence.SetFunctionBranch(obj =>
            {
                mSenderKey.SendKeyPress((KeyDataDelay[])obj);
                return false;
            }, mMacroCollectionReader.ReadSequence.ToArray());

            mTreeSequence.AddPart(branchSequence.StartBranchState);
            listBranchSequence.Add(branchSequence);

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
            ObservablePropertyCollection<KeyData> listViewDataContext = (ObservablePropertyCollection<KeyData>)listView.DataContext;
            listViewDataContext.Remove((KeyData)element.DataContext);
        }

        private void DeleteRowMacroButton_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            ListView listView = FindParent<ListView>(element);
            ObservablePropertyCollection<KeyDataDelay> listViewDataContext = (ObservablePropertyCollection<KeyDataDelay>)listView.DataContext;
            listViewDataContext.Remove((KeyDataDelay)element.DataContext);
        }

        private void CleanRowsSequenceButton_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            ListView listView = FindParent<ListView>(element);
            ((ObservablePropertyCollection<KeyData>)listView.DataContext).Clear();
        }

        private void CleanRowsMacroButton_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            ListView listView = FindParent<ListView>(element);
            ((ObservablePropertyCollection<KeyDataDelay>)listView.DataContext).Clear();
        }

        private void DeleteRowMacrosButton_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            Macros macro = (Macros)element.DataContext;

            int index = mMacrosCollection.IndexOf(macro);
            mTreeSequence.RemoveState(index);

            mMacrosCollection.Remove(macro);
        }

        private void CleanRowsMacrosButton_Click(object sender, RoutedEventArgs e)
        {
            mTreeSequence.ClearTree();
            mMacrosCollection.Clear();
        }

        private void delay_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            string text = e.Text;
            try
            {
                int.Parse(text);
            }
            catch
            {
                e.Handled = true;
            }
        }

        private void delayDefault_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            int value = delayDefault.Text == string.Empty ? 0 : int.Parse(delayDefault.Text);
            for (int i = 0; i < mMacroCollectionReader.ReadSequence.Count; i++)
            {
                var item = mMacroCollectionReader.ReadSequence[i];
                mMacroCollectionReader.ReadSequence[i] = new KeyDataDelay(item.VirtualKeyCode, (int)item.Message, item.Time, value);
            }
        }
    }
}