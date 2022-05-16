using Windows.UI.Xaml.Controls;

namespace AppUIBasics.ControlPages
{
    public sealed partial class ExpanderPage : Page
    {
        public ExpanderPage()
        {
            this.InitializeComponent();
        }

        private void ExpandDirectionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string expandDirection = e.AddedItems[0].ToString();

            switch (expandDirection)
            {
                case "Down":
                default:
                    Expander1.ExpandDirection = Microsoft.UI.Xaml.Controls.ExpandDirection.Down;
                    Expander1.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Top;
                    break;

                case "Up":
                    Expander1.ExpandDirection = Microsoft.UI.Xaml.Controls.ExpandDirection.Up;
                    Expander1.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Bottom;
                    break;   
            }
        }
    }


}
