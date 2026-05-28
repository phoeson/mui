using System.Windows;
using System.Windows.Controls;

namespace FirstFloor.ModernUI.App.Content
{
    public partial class ControlsModernInfoBar : UserControl
    {
        public ControlsModernInfoBar()
        {
            InitializeComponent();
        }

        private void OnReopenClick(object sender, RoutedEventArgs e)
        {
            dismissDemo.IsOpen = true;
        }
    }
}
