using FirstFloor.ModernUI.Windows.Controls;
using System.Windows;
using System.Windows.Controls;

namespace FirstFloor.ModernUI.App.Content
{
    /// <summary>
    /// Interaction logic for ControlsModernButtonGroup.xaml
    /// </summary>
    public partial class ControlsModernButtonGroup : UserControl
    {
        public ControlsModernButtonGroup()
        {
            InitializeComponent();
        }

        private void OnActionClick(object sender, RoutedEventArgs e)
        {
            var item = sender as ButtonGroupItem;
            if (item != null)
            {
                lastAction.Text = item.Content as string ?? string.Empty;
            }
        }
    }
}
