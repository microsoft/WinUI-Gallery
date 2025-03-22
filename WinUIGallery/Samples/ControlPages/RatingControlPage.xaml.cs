using Microsoft.UI.Xaml.Controls;

namespace WinUIGallery.ControlPages;

public sealed partial class RatingControlPage : Page
{
    public RatingControlPage()
    {
        InitializeComponent();
    }

    private void RatingControl1_ValueChanged(RatingControl sender, object args)
    {
        RatingControl1.Caption = "Your rating";
    }
}
