using System.Windows;
using System.Windows.Controls;

namespace FirstFloor.ModernUI.App.Content
{
    public partial class ControlsModernStepIndicator : UserControl
    {
        public ControlsModernStepIndicator()
        {
            InitializeComponent();
        }

        private void OnNext(object sender, RoutedEventArgs e)
        {
            var indicator = InteractiveIndicator;
            int max = indicator.Items.Count - 1;
            if (indicator.CurrentStep < max)
            {
                indicator.CurrentStep++;
                InteractiveFeedback.Text = "CurrentStep = " + indicator.CurrentStep;
            }
            else
            {
                InteractiveFeedback.Text = "Already at last step (" + indicator.CurrentStep + ")";
            }
        }

        private void OnBack(object sender, RoutedEventArgs e)
        {
            var indicator = InteractiveIndicator;
            if (indicator.CurrentStep > 0)
            {
                indicator.CurrentStep--;
                InteractiveFeedback.Text = "CurrentStep = " + indicator.CurrentStep;
            }
            else
            {
                InteractiveFeedback.Text = "Already at first step";
            }
        }
    }
}
