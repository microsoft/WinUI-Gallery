using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System.Threading.Tasks;

namespace WinUIGallery.Samples.SamplePages;

public sealed partial class SampleWindow1 : Window
{
    public SampleWindow1(string WindowTitle, int Width, int Height, int X, int Y, TitleBarTheme TitleBarPreferredTheme)
    {
        this.InitializeComponent();

        // Set the window title
        AppWindow.Title = WindowTitle;

        // Set the window size (including borders)
        AppWindow.Resize(new Windows.Graphics.SizeInt32(Width, Height));

        // Set the window position on screen
        AppWindow.Move(new Windows.Graphics.PointInt32(X, Y));

        // Set the preferred theme for the title bar
        AppWindow.TitleBar.PreferredTheme = TitleBarPreferredTheme;

        // Set the taskbar icon (displayed in the taskbar)
        AppWindow.SetTaskbarIcon("Assets/Tiles/GalleryIcon.ico");

        // Set the title bar icon (displayed in the window's title bar)
        AppWindow.SetTitleBarIcon("Assets/Tiles/GalleryIcon.ico");

        // Set the window icon (affects both taskbar and title bar, can be omitted if the above two are set)
        // appWindow.SetIcon("Assets/Tiles/GalleryIcon.ico"); 
    }

    private async void Show_Click(object sender, RoutedEventArgs e)
    {
        AppWindow.Hide();
        await Task.Delay(3000);
        AppWindow.Show();
    }

    private void Hide_Click(object sender, RoutedEventArgs e)
    {
        AppWindow.Hide();
    }

    private void Close_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
}
