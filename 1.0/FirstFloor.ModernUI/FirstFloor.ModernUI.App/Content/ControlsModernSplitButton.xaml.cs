using System.Windows;
using System.Windows.Controls;

namespace FirstFloor.ModernUI.App.Content
{
    public partial class ControlsModernSplitButton : UserControl
    {
        public ControlsModernSplitButton()
        {
            InitializeComponent();
        }

        private void OnSplitButtonClick(object sender, RoutedEventArgs e)
        {
            var mi = sender as MenuItem;
            DispatchFeedback.Text = "Invoked: " + (mi != null ? (string)mi.Header : "Dispatch All");
        }

        private void OnPrintClick(object sender, RoutedEventArgs e)
        {
            var mi = sender as MenuItem;
            PrintFeedback.Text = "Invoked: " + (mi != null ? (string)mi.Header : "Print Labels");
        }

        private void OnExportClick(object sender, RoutedEventArgs e)
        {
            var mi = sender as MenuItem;
            ExportFeedback.Text = "Invoked: " + (mi != null ? (string)mi.Header : "Export to CSV");
        }
    }
}
