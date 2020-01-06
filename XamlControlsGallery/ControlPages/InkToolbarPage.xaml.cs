using Windows.UI.Core;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace AppUIBasics.ControlPages
{
    public sealed partial class InkToolbarPage : Page
    {
        private InkPresenter _inkPresenter;

        public InkToolbarPage()
        {
            this.InitializeComponent();

            _inkPresenter = inkCanvas.InkPresenter;
            _inkPresenter.InputDeviceTypes =
                CoreInputDeviceTypes.Mouse | CoreInputDeviceTypes.Pen | CoreInputDeviceTypes.Touch;

            UpdatePen();
        }

        private void UpdatePen()
        {
            if (_inkPresenter != null)
            {
                var defaultAttributes = _inkPresenter.CopyDefaultDrawingAttributes();

                defaultAttributes.PenTip = (bool)penTipShape.IsChecked ? PenTipShape.Circle : PenTipShape.Rectangle;

                _inkPresenter.UpdateDefaultDrawingAttributes(defaultAttributes);
            }
        }

        private void clearAll_Click(object sender, RoutedEventArgs e)
        {
            _inkPresenter.StrokeContainer.Clear();
        }

        private void PenTip_Checked(object sender, RoutedEventArgs e)
        {
            UpdatePen();
        }
    }
}
