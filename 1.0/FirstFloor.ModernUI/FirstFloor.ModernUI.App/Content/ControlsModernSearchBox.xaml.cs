using FirstFloor.ModernUI.Presentation;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FirstFloor.ModernUI.App.Content
{
    /// <summary>
    /// Interaction logic for ControlsModernSearchBox.xaml
    /// </summary>
    public partial class ControlsModernSearchBox : UserControl
    {
        /// <summary>
        /// Command executed when the user presses Enter in the command-demo SearchBox.
        /// Bound via RelativeSource in the XAML.
        /// </summary>
        public ICommand SearchCommand { get; }

        public ControlsModernSearchBox()
        {
            SearchCommand = new RelayCommand(param =>
            {
                lastSearch.Text = param as string ?? string.Empty;
            });

            InitializeComponent();
        }
    }
}
