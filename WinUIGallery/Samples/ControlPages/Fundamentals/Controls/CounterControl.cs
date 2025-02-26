using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace WinUIGallery.Samples.ControlPages.Fundamentals.Controls;

public enum CounterMode
{
    Increment,
    Decrement
}

public sealed class CounterControl : Control
{
    public static readonly DependencyProperty CountProperty =
        DependencyProperty.Register(nameof(Count), typeof(int), typeof(CounterControl), new PropertyMetadata(0));

    public static readonly DependencyProperty ModeProperty =
        DependencyProperty.Register(nameof(Mode), typeof(CounterMode), typeof(CounterControl), new PropertyMetadata(CounterMode.Increment));

    public CounterControl()
    {
        this.DefaultStyleKey = typeof(CounterControl);
    }

    public int Count
    {
        get => (int)GetValue(CountProperty);
        set => SetValue(CountProperty, value);
    }

    public CounterMode Mode
    {
        get => (CounterMode)GetValue(ModeProperty);
        set => SetValue(ModeProperty, value);
    }

    private Button ActionButton;
    private TextBlock CountText;

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        ActionButton = GetTemplateChild(nameof(ActionButton)) as Button;
        CountText = GetTemplateChild(nameof(CountText)) as TextBlock;

        if (ActionButton is not null)
        {
            ActionButton.Click += (sender, e) =>
            {
                Count = Mode == CounterMode.Increment ? Count + 1 : Count - 1;
                UpdateUI();
            };

            UpdateButtonText();
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (CountText is not null)
        {
            CountText.Text = Count.ToString();
        }
    }

    private void UpdateButtonText()
    {
        if (ActionButton is not null)
        {
            ActionButton.Content = Mode == CounterMode.Increment ? "Increase" : "Decrease";
        }
    }
}

