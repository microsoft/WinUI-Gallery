using System;
using System.Runtime.InteropServices;
using AppUIBasics.Helper;
using Microsoft.UI;
using Microsoft.UI.Composition;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Windows.Graphics;
using Windows.UI.ViewManagement;
using WinRT;
using Color = Windows.UI.Color;

namespace AppUIBasics.ControlPages
{
    public sealed partial class AcrylicPage : Page
    {
        private AcrylicSampleWindow defaultWindow;
        private AcrylicSampleWindow customWindow;
        private Window ownerWindow;
        private bool optionsInitialized;

        public AcrylicPage()
        {
            this.InitializeComponent();
            Loaded += AcrylicPage_Loaded;
            Unloaded += AcrylicPage_Unloaded;
        }

        private void AcrylicPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!optionsInitialized)
            {
                ColorSelector.SelectedIndex = ColorSelectorInApp.SelectedIndex = 0;
                FallbackColorSelector.SelectedIndex = FallbackColorSelectorInApp.SelectedIndex = 0;
                OpacitySlider.Value = OpacitySliderInApp.Value = OpacitySliderLumin.Value = 0.8;
                LuminositySlider.Value = 0.8;
                optionsInitialized = true;
            }

            if (ownerWindow == null)
            {
                ownerWindow = WindowHelper.GetWindowForElement(this);
                if (ownerWindow != null)
                {
                    ownerWindow.Closed += OwnerWindow_Closed;
                }
            }

            UpdateDesktopStatus();
        }

        private void AcrylicPage_Unloaded(object sender, RoutedEventArgs e)
        {
            DetachOwnerWindow();
            CloseSampleWindows();
        }

        private void OwnerWindow_Closed(object sender, WindowEventArgs args)
        {
            DetachOwnerWindow();
            CloseSampleWindows();
        }

        private void DetachOwnerWindow()
        {
            if (ownerWindow != null)
            {
                ownerWindow.Closed -= OwnerWindow_Closed;
                ownerWindow = null;
            }
        }

        private void CloseSampleWindows()
        {
            var defaultSample = defaultWindow;
            var customSample = customWindow;
            defaultWindow = null;
            customWindow = null;
            defaultSample?.Close();
            customSample?.Close();
        }

        private void OpenDefaultAcrylicWindow_Click(object sender, RoutedEventArgs e)
        {
            if (defaultWindow == null)
            {
                defaultWindow = new AcrylicSampleWindow(custom: false, theme: ActualTheme);
                defaultWindow.StatusChanged += DesktopWindow_StatusChanged;
                defaultWindow.Closed += DefaultWindow_Closed;
                WindowHelper.TrackWindow(defaultWindow);
            }

            defaultWindow.Activate();
            UpdateDesktopStatus();
        }

        private void OpenCustomAcrylicWindow_Click(object sender, RoutedEventArgs e)
        {
            if (customWindow == null)
            {
                customWindow = new AcrylicSampleWindow(custom: true, theme: ActualTheme);
                customWindow.StatusChanged += DesktopWindow_StatusChanged;
                customWindow.Closed += CustomWindow_Closed;
                WindowHelper.TrackWindow(customWindow);
            }

            UpdateCustomDesktopAcrylic();
            customWindow.Activate();
            UpdateDesktopStatus();
        }

        private void DefaultWindow_Closed(object sender, WindowEventArgs args)
        {
            var window = (AcrylicSampleWindow)sender;
            window.StatusChanged -= DesktopWindow_StatusChanged;
            window.Closed -= DefaultWindow_Closed;
            defaultWindow = null;
            UpdateDesktopStatus();
        }

        private void CustomWindow_Closed(object sender, WindowEventArgs args)
        {
            var window = (AcrylicSampleWindow)sender;
            window.StatusChanged -= DesktopWindow_StatusChanged;
            window.Closed -= CustomWindow_Closed;
            customWindow = null;
            UpdateDesktopStatus();
        }

        private void DesktopWindow_StatusChanged(object sender, EventArgs e)
        {
            UpdateDesktopStatus();
        }

        private void UpdateDesktopStatus()
        {
            string unopenedStatus = DesktopAcrylicController.IsSupported()
                ? "Open the sample to inspect desktop acrylic and its current fallback state."
                : "Desktop acrylic is not supported on this device. The sample will explicitly show a solid-color fallback.";
            DefaultDesktopStatus.Text = defaultWindow?.Status ?? unopenedStatus;
            CustomDesktopStatus.Text = customWindow?.Status ?? unopenedStatus;
        }

        private void UpdateCustomDesktopAcrylic()
        {
            if (customWindow != null &&
                ColorSelector.SelectedItem is SolidColorBrush tint &&
                FallbackColorSelector.SelectedItem is SolidColorBrush fallback)
            {
                customWindow.SetCustomColors(tint.Color, fallback.Color, OpacitySlider.Value);
            }
        }

        private void Slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (Equals(sender, OpacitySlider))
            {
                UpdateCustomDesktopAcrylic();
                return;
            }

            Rectangle shape = Equals(sender, OpacitySliderInApp) ? CustomAcrylicShapeInApp : CustomAcrylicShapeLumin;
            if (shape?.Fill is AcrylicBrush brush)
            {
                brush.TintOpacity = e.NewValue;
            }
        }

        private void ColorSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Equals(sender, ColorSelector))
            {
                UpdateCustomDesktopAcrylic();
            }
            else if (CustomAcrylicShapeInApp?.Fill is AcrylicBrush brush &&
                     e.AddedItems.Count > 0 && e.AddedItems[0] is SolidColorBrush tint)
            {
                brush.TintColor = tint.Color;
            }
        }

        private void FallbackColorSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Equals(sender, FallbackColorSelector))
            {
                UpdateCustomDesktopAcrylic();
            }
            else if (CustomAcrylicShapeInApp?.Fill is AcrylicBrush brush &&
                     e.AddedItems.Count > 0 && e.AddedItems[0] is SolidColorBrush fallback)
            {
                brush.FallbackColor = fallback.Color;
            }
        }

        private void LuminositySlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (CustomAcrylicShapeLumin?.Fill is AcrylicBrush brush)
            {
                brush.TintLuminosityOpacity = e.NewValue;
            }
        }

        private sealed class AcrylicSampleWindow : Window
        {
            private readonly bool custom;
            private readonly Grid root;
            private readonly InfoBar status;
            private readonly AccessibilitySettings accessibilitySettings = new AccessibilitySettings();
            private DesktopAcrylicController controller;
            private SystemBackdropConfiguration configuration;
            private Color fallbackColor = Colors.Green;
            private string unavailableReason;
            private bool isClosed;

            public event EventHandler StatusChanged;
            public string Status => status.Message;

            public AcrylicSampleWindow(bool custom, ElementTheme theme)
            {
                this.custom = custom;
                Title = custom ? "Custom desktop acrylic" : "Default desktop acrylic";
                root = new Grid { RequestedTheme = theme };
                status = new InfoBar
                {
                    IsOpen = true,
                    IsClosable = false,
                    Title = Title,
                    Margin = new Thickness(24),
                    VerticalAlignment = VerticalAlignment.Top,
                    Content = new TextBlock
                    {
                        Text = custom
                            ? "Change the controls on the Acrylic page, then activate this window. The tint stays fixed across light/dark themes. The clear area below shows the desktop material."
                            : "Move this window over another window to see background acrylic. The clear area below shows the desktop material; default colors follow the app theme.",
                        TextWrapping = TextWrapping.Wrap
                    }
                };
                root.Children.Add(status);
                Content = root;
                AppWindow.Resize(new SizeInt32(640, 420));

                Activated += SampleWindow_Activated;
                Closed += SampleWindow_Closed;
                root.ActualThemeChanged += Root_ActualThemeChanged;
                accessibilitySettings.HighContrastChanged += AccessibilitySettings_HighContrastChanged;
                InitializeDesktopAcrylic();
            }

            private void InitializeDesktopAcrylic()
            {
                if (!DesktopAcrylicController.IsSupported())
                {
                    unavailableReason = "Desktop acrylic is not supported on this device. Showing a solid-color fallback.";
                    UpdateStatus();
                    return;
                }

                try
                {
                    DispatcherQueue.EnsureSystemDispatcherQueue();
                    configuration = new SystemBackdropConfiguration
                    {
                        IsInputActive = true,
                        Theme = GetBackdropTheme()
                    };
                    controller = new DesktopAcrylicController();
                    controller.SetSystemBackdropConfiguration(configuration);
                    controller.StateChanged += Controller_StateChanged;
                    if (!controller.AddSystemBackdropTarget(this.As<ICompositionSupportsSystemBackdrop>()))
                    {
                        ReleaseController();
                        unavailableReason = "Desktop acrylic could not attach to this window. Showing a solid-color fallback.";
                    }
                }
                catch (Exception ex) when (ex is COMException || ex is NotSupportedException)
                {
                    ReleaseController();
                    unavailableReason = $"Desktop acrylic could not be initialized (0x{ex.HResult:X8}). Showing a solid-color fallback.";
                }

                UpdateStatus();
            }

            public void SetCustomColors(Color tintColor, Color fallback, double tintOpacity)
            {
                fallbackColor = fallback;
                if (controller != null)
                {
                    controller.TintColor = tintColor;
                    controller.FallbackColor = fallback;
                    controller.TintOpacity = (float)tintOpacity;
                }

                UpdateStatus();
            }

            private SystemBackdropTheme GetBackdropTheme()
            {
                return root.ActualTheme == ElementTheme.Dark ? SystemBackdropTheme.Dark : SystemBackdropTheme.Light;
            }

            private void SampleWindow_Activated(object sender, WindowActivatedEventArgs args)
            {
                if (configuration != null)
                {
                    configuration.IsInputActive = args.WindowActivationState != WindowActivationState.Deactivated;
                }
            }

            private void Root_ActualThemeChanged(FrameworkElement sender, object args)
            {
                if (configuration != null)
                {
                    configuration.Theme = GetBackdropTheme();
                }

                UpdateStatus();
            }

            private void AccessibilitySettings_HighContrastChanged(AccessibilitySettings sender, object args)
            {
                DispatcherQueue.TryEnqueue(UpdateStatus);
            }

            private void Controller_StateChanged(ISystemBackdropControllerWithTargets sender, object args)
            {
                // Controller notifications need not arrive on the XAML thread.
                DispatcherQueue.TryEnqueue(UpdateStatus);
            }

            private void UpdateStatus()
            {
                if (isClosed)
                {
                    return;
                }

                if (controller == null)
                {
                    Color solidColor = custom ? fallbackColor :
                        root.ActualTheme == ElementTheme.Dark ? Color.FromArgb(255, 32, 32, 32) : Colors.White;
                    if (accessibilitySettings.HighContrast)
                    {
                        solidColor = new UISettings().GetColorValue(UIColorType.Background);
                    }

                    root.Background = new SolidColorBrush(solidColor);
                    status.Severity = InfoBarSeverity.Warning;
                    status.Message = unavailableReason + (accessibilitySettings.HighContrast
                        ? " High contrast uses the system background color instead of the custom fallback."
                        : string.Empty);
                }
                else
                {
                    // Only the controller paints the background, including policy-driven fallback.
                    root.Background = null;
                    status.Severity = controller.State == SystemBackdropState.Active ? InfoBarSeverity.Informational : InfoBarSeverity.Warning;
                    status.Message = controller.State switch
                    {
                        SystemBackdropState.Active => "Desktop acrylic is active: the material samples the desktop and windows behind this window.",
                        SystemBackdropState.HighContrast => "High-contrast fallback is active. Windows uses its high-contrast color instead of the custom fallback.",
                        _ => "Solid-color fallback is active. Window inactivity, transparency settings, power policy or device conditions can disable acrylic."
                    };
                }

                StatusChanged?.Invoke(this, EventArgs.Empty);
            }

            private void ReleaseController()
            {
                if (controller != null)
                {
                    controller.StateChanged -= Controller_StateChanged;
                    controller.Dispose();
                    controller = null;
                }

                configuration = null;
            }

            private void SampleWindow_Closed(object sender, WindowEventArgs args)
            {
                isClosed = true;
                Activated -= SampleWindow_Activated;
                Closed -= SampleWindow_Closed;
                root.ActualThemeChanged -= Root_ActualThemeChanged;
                accessibilitySettings.HighContrastChanged -= AccessibilitySettings_HighContrastChanged;
                ReleaseController();
            }
        }
    }
}
