using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace AppUIBasics.ControlPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
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

        private void strokeSize_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            UpdatePen();
        }

        private void UpdatePen()
        {
            if (_inkPresenter != null)
            {
                var defaultAttributes = _inkPresenter.CopyDefaultDrawingAttributes();

                defaultAttributes.Size = new Size(strokeSize.Value, strokeSize.Value);
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
