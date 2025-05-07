using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace WinUIGallery.SamplePages;

public sealed partial class TabContentSampleControl : UserControl
{
    public TabContentSampleControl()
    {
        this.InitializeComponent();
    }
    public bool IsInProgress
    {
        get { return (bool)GetValue(IsInProgressProperty); }
        set { SetValue(IsInProgressProperty, value); }
    }

    public static readonly DependencyProperty IsInProgressProperty = DependencyProperty.Register("IsInProgress", typeof(bool), typeof(TabContentSampleControl), new PropertyMetadata(false));

 
}
