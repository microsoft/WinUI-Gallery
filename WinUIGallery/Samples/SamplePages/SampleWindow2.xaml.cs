using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Windows.Graphics;

namespace WinUIGallery.Samples.SamplePages;

public sealed partial class SampleWindow2 : Window
{
    public SampleWindow2()
    {
        this.InitializeComponent();
        AppWindow.SetIcon("Assets/Tiles/GalleryIcon.ico");
        AppWindow.TitleBar.PreferredTheme = TitleBarTheme.UseDefaultAppMode;

        // Center the window on the screen.
        CenterWindow();
    }

    // Centers the given AppWindow on the screen based on the available display area.
    private void CenterWindow()
    {
        if (AppWindow == null) return; // Ensure the AppWindow instance is valid.

        // Get the display area associated with the window.
        DisplayArea displayArea = DisplayArea.GetFromWindowId(AppWindow.Id, DisplayAreaFallback.Nearest);
        if (displayArea == null) return; // Ensure the display area is valid.

        // Calculate the centered position within the work area.
        RectInt32 workArea = displayArea.WorkArea;
        PointInt32 centeredPosition = new PointInt32(
            (workArea.Width - AppWindow.Size.Width) / 2,
            (workArea.Height - AppWindow.Size.Height) / 2
        );

        AppWindow.Move(centeredPosition); // Move the window to the calculated position.
    }
}
