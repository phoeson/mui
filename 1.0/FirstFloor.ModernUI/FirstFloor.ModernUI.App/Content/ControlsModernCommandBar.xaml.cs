using System.Windows;
using System.Windows.Controls;
using FirstFloor.ModernUI.Windows.Controls;

namespace FirstFloor.ModernUI.App.Content
{
    public partial class ControlsModernCommandBar : UserControl
    {
        public ControlsModernCommandBar()
        {
            InitializeComponent();
        }

        private void OnWarehouseButtonClick(object sender, RoutedEventArgs e)
        {
            var btn = sender as CommandBarButton;
            if (btn != null) WarehouseFeedback.Text = "Invoked: " + btn.Label;
        }

        private void OnOrderButtonClick(object sender, RoutedEventArgs e)
        {
            var btn = sender as CommandBarButton;
            if (btn != null) OrderFeedback.Text = "Invoked: " + btn.Label;
        }
    }
}
