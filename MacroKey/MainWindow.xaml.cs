using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MacroKey
{
    public partial class MainWindow : Window
    {
        private HookerKeys m_hookerKeys;
        private Machine<KeyData> m_machine;
        public ObservableCollection<KeyData> EnterKeys { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Loaded += Page_Loaded;
        }

        void Page_Loaded(object sender, RoutedEventArgs e)
        {
            m_hookerKeys = new HookerKeys();
            EnterKeys = new ObservableCollection<KeyData>();
            State<KeyData> startState = new State<KeyData>();
            m_machine = new Machine<KeyData>(startState);
            listSequenceKey.DataContext = EnterKeys;
        }

        private void StartRecordSequence_Click(object sender, RoutedEventArgs e)
        {
            Button buttonSender = (Button)sender;
            if(buttonSender.Content.ToString() == "Record")
            {
                EnterKeys.Clear();
                m_hookerKeys.SetHook((key, keyState) =>
                {
                    EnterKeys.Add(new KeyData(key, keyState));
                    return true;
                });
                buttonSender.Content = "Stop";
            }
            else
            {
                m_hookerKeys.Unhook();
                buttonSender.Content = "Record";
                State<KeyData> currentState = new State<KeyData>();
                State<KeyData> saveStartState = currentState;
                foreach (var item in EnterKeys)
                {
                    State<KeyData> newState = new State<KeyData>();
                    currentState.AddNextState(item, newState);
                    currentState = newState;
                }
                m_machine.AddBranch(saveStartState);
            }
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
