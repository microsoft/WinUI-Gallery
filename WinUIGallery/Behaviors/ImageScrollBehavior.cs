using WinUIGallery.Helper;
using Microsoft.Xaml.Interactivity;
using System.Linq;
using Windows.Storage;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace WinUIGallery.Behaviors
{
    public partial class ImageScrollBehavior : DependencyObject, IBehavior
    {
        private const int _opacityMaxValue = 250;
        private const int _alpha = 255;
        private const int _maxFontSize = 28;
        private const int _minFontSize = 10;
        private const int scrollViewerThresholdValue = 85;

        private ScrollViewer scrollViewer;
        private ListViewBase listGridView;

        public DependencyObject AssociatedObject { get; private set; }

        public Control TargetControl
        {
            get { return (Control)GetValue(TargetControlProperty); }
            set { SetValue(TargetControlProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TargetControl.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TargetControlProperty =
            DependencyProperty.Register("TargetControl", typeof(Control), typeof(ImageScrollBehavior), new PropertyMetadata(null));

        public void Attach(DependencyObject associatedObject)
        {
            AssociatedObject = associatedObject;
            if (!GetScrollViewer())
            {
                ((ListViewBase)associatedObject).Loaded += ListGridView_Loaded;
            }
        }

        private void ListGridView_Loaded(object sender, RoutedEventArgs e)
        {
            GetScrollViewer();
            listGridView = sender as ListViewBase;
        }

        private bool GetScrollViewer()
        {
            scrollViewer = Helper.UIHelper.GetDescendantsOfType<ScrollViewer>(AssociatedObject).FirstOrDefault();
            if (scrollViewer != null)
            {
                scrollViewer.ViewChanging += ScrollViewer_ViewChanging;
                return true;
            }
            return false;
        }

        private void ScrollViewer_ViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
        {
            double verticalOffset = ((ScrollViewer)sender).VerticalOffset;
            var header = (PageHeader)TargetControl;
            header.BackgroundColorOpacity = verticalOffset / _opacityMaxValue;
            header.AcrylicOpacity = 0.3 * (1 - (verticalOffset / _opacityMaxValue));
            if (verticalOffset < 10)
            {
                VisualStateManager.GoToState(header, "DefaultForeground", false);
                header.BackgroundColorOpacity = 0;
                header.FontSize = 28;
                header.AcrylicOpacity = 0.3;
            }
            else if (verticalOffset > scrollViewerThresholdValue)
            {
                VisualStateManager.GoToState(header, "AlternateForeground", false);
                header.FontSize = _minFontSize;
            }
            else
            {
                if (ThemeHelper.ActualTheme != ElementTheme.Dark)
                {
                    VisualStateManager.GoToState(header, "DefaultForeground", false);
                    Color foreground = new Color() { A = (byte)((verticalOffset > scrollViewerThresholdValue) ? 0 : (_alpha * (1 - (verticalOffset / scrollViewerThresholdValue)))) };
                    foreground.R = foreground.G = foreground.B = 0;
                    header.Foreground = new SolidColorBrush(foreground);
                }
                else
                {
                    VisualStateManager.GoToState(header, "AlternateForeground", false);
                }

                header.FontSize = -(((verticalOffset / scrollViewerThresholdValue) * (_maxFontSize - _minFontSize)) - _maxFontSize);
            }
        }

        public void Detach()
        {
            ((ListViewBase)AssociatedObject).Loaded -= ListGridView_Loaded;
            AssociatedObject = null;
        }
    }
}
