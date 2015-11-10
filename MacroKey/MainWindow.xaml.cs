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
            InitializeDataContext();
            mMacroCollectionReader.CollectionChanged += ReadMacro_CollectionChanged;
            mHookerKey.SetHook();

            StateWalker<KeyData> machineWalker = new StateWalker<KeyData>(mTreeRoot);
            mHookerKey.Hooked += (obj) =>
            {
                var arg = (KeyHookEventArgs)obj;
                State<KeyData> currentState = machineWalker.WalkStates(new KeyData(arg.VirtualKeyCode, arg.KeyMassage, arg.Time));
                return currentState is FunctionalState<KeyData> ? (bool)((FunctionalState<KeyData>)currentState).ExecuteFunction() : true;
                
            };
        }

        private void ReadMacro_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                int value = delayDefault.Text == string.Empty ? 0 : int.Parse(delayDefault.Text);
                foreach (var item in e.NewItems)
                    ((KeyDataDelay)item).Delay = value;
            }
        }

        private void InitializeDataContext()
        {
            recordSequenceButton.DataContext = mSequenceCollectionReader;
            listSequenceKey.DataContext = mSequenceCollectionReader;
            recordMacroButton.DataContext = mMacroCollectionReader;
            listMacroKey.DataContext = mMacroCollectionReader;
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

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)   
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
            if (reader.Count == 0)
                return;

            mHotkeyExecuteGUIBranch = new Branch<KeyData>(
                reader,
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
            mTreeRoot.SetState(new List<Branch<KeyData>> { mHotkeyExecuteGUIBranch, mHotkeyMacrosModeBranch });
        }

        private void HotkeyBoxMacrosMode_LostFocus(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            HookKeyDataReader reader = (HookKeyDataReader)element.DataContext;
            reader.StopRecord();
            if (reader.Count == 0)
                return;

            mHotkeyMacrosModeBranch = new Branch<KeyData>(reader, mKeyDataEqualityComparer);
            mHotkeyMacrosModeBranch.AddState(mTreeSequence);
            mTreeRoot.SetState(new List<Branch<KeyData>> { mHotkeyExecuteGUIBranch, mHotkeyMacrosModeBranch });
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

            if (mSequenceCollectionReader.Count == 0)
            {
                MessageBox.Show("Sequence is empty", "Error", MessageBoxButton.OK);
                return;
            }
            if(mMacroCollectionReader.Count == 0)
            {
                MessageBox.Show("Macros is empty", "Error", MessageBoxButton.OK);
                return;
            }
            if (macrosModHotkeyBox.Text == string.Empty)
            {
                MessageBox.Show("Macros mode hotkey is empty", "Error", MessageBoxButton.OK);
                return;
            }
            IEnumerable<Func<object, object>> functions = Enumerable.Repeat<Func<object, object>>(obj => false, mSequenceCollectionReader.Count);
            Branch<KeyData> branchSequence = new Branch<KeyData>(mSequenceCollectionReader, functions, mKeyDataEqualityComparer);
            branchSequence.SetFunctionBranch(obj =>
            {
                mSenderKey.SendKeyPress((KeyDataDelay[])obj);
                return false;
            }, mMacroCollectionReader.ToArray());

            mTreeSequence.AddState(branchSequence);

            Macros macro = new Macros(textBoxMacrosName.Text, mSequenceCollectionReader, mMacroCollectionReader);
            mMacrosCollection.Add(macro);

            textBoxMacrosName.Clear();
            mSequenceCollectionReader.Clear();
            mMacroCollectionReader.Clear();
        }

        private void DeleteRowSequenceButton_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            mSequenceCollectionReader.Remove((KeyData)element.DataContext);
        }

        private void DeleteRowMacroButton_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            mMacroCollectionReader.Remove((KeyDataDelay)element.DataContext);
        }

        private void DeleteRowMacrosButton_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            Macros macro = (Macros)element.DataContext;

            int index = mMacrosCollection.IndexOf(macro);
            mTreeSequence.RemoveAtState(index);

            mMacrosCollection.Remove(macro);
        }

        private void CleanRowsSequenceButton_Click(object sender, RoutedEventArgs e)
        {
            mSequenceCollectionReader.Clear();
        }

        private void CleanRowsMacroButton_Click(object sender, RoutedEventArgs e)
        {
            mMacroCollectionReader.Clear();
        }

        private void CleanRowsMacrosButton_Click(object sender, RoutedEventArgs e)
        {
            mTreeSequence.ClearTree();
            mMacrosCollection.Clear();
        }

        private void delay_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            int value;
            if (!int.TryParse(e.Text, out value))
                e.Handled = true;
        }

        private void delayDefault_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            int value = textBox.Text == string.Empty ? 0 : int.Parse(delayDefault.Text);
            for (int i = 0; i < mMacroCollectionReader.Count; i++)
            {
                var item = mMacroCollectionReader[i];
                mMacroCollectionReader[i] = new KeyDataDelay(item.VirtualKeyCode, (int)item.Message, item.Time, value);
            }
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Macros macros = (Macros)((ListViewItem)sender).Content;
            mMacroCollectionReader.Collection = macros.Macro;
            mSequenceCollectionReader.Collection = macros.Sequence;
            textBoxMacrosName.Text = macros.Name;
        }

        private void hotkeyBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox element = (TextBox)sender;
        }
    }
}