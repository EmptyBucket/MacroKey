using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MacroKeyMVVM.ViewModel;

namespace MacroKeyMVVM
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Closing += (s, e) => ViewModelLocator.Cleanup();
        }

        private void Grid_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }

        private const string StartResordTitle = "Start record";
        private const string StopRecordTitle = "Stop record";

        private void recordSequenceButton_Click(object sender, RoutedEventArgs e)
        {
            if (recordSequenceButton.Content.ToString() == StartResordTitle)
                recordSequenceButton.Content = StopRecordTitle;
            else
                recordSequenceButton.Content = StartResordTitle;
            recordMacroButton.Content = StartResordTitle;
        }

        private void recordMacroButton_Click(object sender, RoutedEventArgs e)
        {
            if (recordMacroButton.Content.ToString() == StartResordTitle)
                recordMacroButton.Content = StopRecordTitle;
            else
                recordMacroButton.Content = StartResordTitle;
            recordSequenceButton.Content = StartResordTitle;
        }

        private void textBox_GotFocus(object sender, RoutedEventArgs e)
        {
            recordMacroButton.Content = StartResordTitle;
            recordSequenceButton.Content = StartResordTitle;
        }

        private void delay_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            int value;
            if (!int.TryParse(e.Text, out value))
                e.Handled = true;
        }

        private void delay_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == string.Empty)
                textBox.Text = "0";
        }
    }
}