using System;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using Windows.Globalization;

namespace WinUIGallery.Helpers;

public static class MathModeHelper
{
    public static ObservableCollection<MathSymbol> GetSymbolsCollection()
    {
        var symbols = new ObservableCollection<MathSymbol>
        {
            new("\u221E", "Infinity", "\\infty", "U+221E"),
            new("\u2248", "Approximately Equal", "\\approx", "U+2248"),
            new("\u00D7", "Multiplication", "\\times", "U+00D7"),
            new("\u00F7", "Division", "\\div", "U+00F7"),
            new("\u00B1", "Plus-Minus", "\\pm", "U+00B1"),
            new("\u2213", "Minus-Plus", "\\mp", "U+2213"),
            new("\u22C5", "Dot Product", "\\cdot", "U+22C5"),
            new("\u2218", "Function Composition", "\\circ", "U+2218"),
            new("\u22C6", "Star Operator", "\\star", "U+22C6"),
            new("\u2022", "Bullet Operator", "\\bullet", "U+2022"),
            new("\u2295", "Direct Sum", "\\oplus", "U+2295"),
            new("\u2297", "Tensor Product", "\\otimes", "U+2297"),
            new("\u2229", "Intersection", "\\cap", "U+2229"),
            new("\u222A", "Union", "\\cup", "U+222A"),
            new("\u2216", "Set Difference", "\\setminus", "U+2216"),
            new("\u2205", "Empty Set", "\\emptyset", "U+2205"),
            new("\u2283", "Superset", "\\supset", "U+2283"),
            new("\u2282", "Subset", "\\subset", "U+2282"),
            new("\u2287", "Superset or Equal", "\\supseteq", "U+2287"),
            new("\u2286", "Subset or Equal", "\\subseteq", "U+2286"),
            new("\u22A5", "Perpendicular", "\\perp", "U+22A5"),
            new("\u2225", "Parallel", "\\parallel", "U+2225"),
            new("\u2226", "Not Parallel", "\\nparallel", "U+2226"),
            new("\u21D2", "Implies", "\\Rightarrow", "U+21D2"),
            new("\u21D0", "Left Implies", "\\Leftarrow", "U+21D0"),
            new("\u21D4", "If and Only If", "\\Leftrightarrow", "U+21D4"),
            new("\u2200", "For All", "\\forall", "U+2200"),
            new("\u2203", "There Exists", "\\exists", "U+2203"),
            new("\u2204", "Does Not Exist", "\\nexists", "U+2204"),
            new("\u00AC", "Logical NOT", "\\neg", "U+00AC"),
            new("\u2228", "Logical OR", "\\lor", "U+2228"),
            new("\u2227", "Logical AND", "\\land", "U+2227"),
            new("\u2234", "Therefore", "\\therefore", "U+2234"),
            new("\u2235", "Because", "\\because", "U+2235"),
            new("\u221D", "Proportional To", "\\propto", "U+221D"),
            new("\u221A", "Square Root", "\\surd", "U+221A"),
            new("\u0305", "Overline", "\\overline()", "U+0305"),
            new("\u0332", "Underline", "\\underline()", "U+0332"),
            new("\u03C0", "Pi", "\\pi", "U+03C0"),
            new("\u03B1", "Alpha", "\\alpha", "U+03B1"),
            new("\u03B2", "Beta", "\\beta", "U+03B2"),
            new("\u03B3", "Gamma", "\\gamma", "U+03B3"),
            new("\u03B4", "Delta", "\\delta", "U+03B4"),
            new("\u03B5", "Epsilon", "\\epsilon", "U+03B5"),
            new("\u03B6", "Zeta", "\\zeta", "U+03B6"),
            new("\u03B7", "Eta", "\\eta", "U+03B7"),
            new("\u03B8", "Theta", "\\theta", "U+03B8"),
            new("\u03B9", "Iota", "\\iota", "U+03B9"),
            new("\u03BA", "Kappa", "\\kappa", "U+03BA"),
            new("\u03BB", "Lambda", "\\lambda", "U+03BB"),
            new("\u03BC", "Mu", "\\mu", "U+03BC"),
            new("\u03BD", "Nu", "\\nu", "U+03BD"),
            new("\u03BE", "Xi", "\\xi", "U+03BE"),
            new("\u03C1", "Rho", "\\rho", "U+03C1"),
            new("\u03C3", "Sigma", "\\sigma", "U+03C3"),
            new("\u03C4", "Tau", "\\tau", "U+03C4"),
            new("\u03C5", "Upsilon", "\\upsilon", "U+03C5"),
            new("\u03C6", "Phi", "\\phi", "U+03C6"),
            new("\u03C7", "Chi", "\\chi", "U+03C7"),
            new("\u03C8", "Psi", "\\psi", "U+03C8"),
            new("\u03C9", "Omega", "\\omega", "U+03C9"),
            new("\u2211", "Summation", "\\sum", "U+2211"),
            new("\u222B", "Integral", "\\int", "U+222B"),
            new("\u222E", "Contour Integral", "\\oint", "U+222E"),
            new("\u220F", "Product", "\\prod", "U+220F"),
            new("\u2210", "Coproduct", "\\coprod", "U+2210")
        };
        return symbols;
    }

    public static ObservableCollection<MathStucture> GetStructuresCollection()
    {
        var structures = new ObservableCollection<MathStucture>
        {
            new("Structures/StackedFraction_Light.png", "Structures/StackedFraction_Dark.png", "Stacked Fraction", "/"),
            new("Structures/SkewedFraction_Light.png", "Structures/SkewedFraction_Dark.png", "Skewed Fraction","\\sdiv "),
            new("Structures/LinearFraction_Light.png", "Structures/LinearFraction_Dark.png", "Linear Fraction", "\\ldiv "),
            new("Structures/Superscript_Light.png", "Structures/Superscript_Dark.png", "Superscript", "\\begin^\\end "),
            new("Structures/Subscript_Light.png", "Structures/Subscript_Dark.png", "Subscript", "\\begin_\\end "),
            new("Structures/SuperscriptSubscript_Light.png", "Structures/SuperscriptSubscript_Dark.png", "Superscript and Subscript", "\\open\\close^_ "),
            new("Structures/SuperscriptSubscriptLeft_Light.png", "Structures/SuperscriptSubscriptLeft_Dark.png", "Left Superscript and Subscript", "(^_)"),
            new("Structures/SquareRoot_Light.png", "Structures/SquareRoot_Dark.png", "Square Root", "\\sqrt "),
            new("Structures/RadicalWithDegree_Light.png", "Structures/RadicalWithDegree_Dark.png", "Radical with Degree", "\\sqrt(&)"),
            new("Structures/SquareRootWithDegree_Light.png", "Structures/SquareRootWithDegree_Dark.png", "Square Root with Degree", "\\sqrt(2&)"),
            new("structures/CubicRoot_Light.png", "structures/CubicRoot_Dark.png", "Cubic Root", "\\cbrt "),
        };
        return structures;
    }

    public static ObservableCollection<MathStucture> GetIntegralsCollection()
    {
        var integrals = new ObservableCollection<MathStucture>
        {
            new("Integrals/Integral_Light.png", "Integrals/Integral_Dark.png", "Integral", "\\int4"),
            new("Integrals/IntegralWithLimits_Light.png", "Integrals/IntegralWithLimits_Dark.png", "Integral with Limits", "\\int24"),
            new("Integrals/IntegralWithStackedLimits_Light.png", "Integrals/IntegralWithStackedLimits_Dark.png", "Integral with Stacked Limits", "\\int28"),
            new("Integrals/DoubleIntegral_Light.png", "Integrals/DoubleIntegral_Dark.png", "Double Integral", "\\iint4"),
            new("Integrals/DoubleIntegralWithLimits_Light.png", "Integrals/DoubleIntegralWithLimits_Dark.png", "Double Integral with Limits", "\\iint24"),
            new("Integrals/DoubleIntegralWithStackedLimits_Light.png", "Integrals/DoubleIntegralWithStackedLimits_Dark.png", "Double Integral with Stacked Limits", "\\iint28"),
            new("Integrals/TripleIntegral_Light.png", "Integrals/TripleIntegral_Dark.png", "Triple Integral", "\\iiint4"),
            new("Integrals/TripleIntegralWithLimits_Light.png", "Integrals/TripleIntegralWithLimits_Dark.png", "Triple Integral with Limits", "\\iiint24"),
            new("Integrals/TripleIntegralWithStackedLimits_Light.png", "Integrals/TripleIntegralWithStackedLimits_Dark.png", "Triple Integral with Stacked Limits", "\\iiint28"),
            new("Integrals/ContourIntegral_Light.png", "Integrals/ContourIntegral_Dark.png", "Contour Integral", "\\oint4"),
            new("Integrals/ContourIntegralWithLimits_Light.png", "Integrals/ContourIntegralWithLimits_Dark.png", "Contour Integral with Limits", "\\oint24"),
            new("Integrals/ContourIntegralWithStackedLimits_Light.png", "Integrals/ContourIntegralWithStackedLimits_Dark.png", "Contour Integral with Stacked Limits", "\\oint28"),
            new("Integrals/DoubleContourIntegral_Light.png", "Integrals/DoubleContourIntegral_Dark.png", "Double Contour Integral", "\\oiint4"),
            new("Integrals/DoubleContourIntegralWithLimits_Light.png", "Integrals/DoubleContourIntegralWithLimits_Dark.png", "Double Contour Integral with Limits", "\\oiint24"),
            new("Integrals/DoubleContourIntegralWithStackedLimits_Light.png", "Integrals/DoubleContourIntegralWithStackedLimits_Dark.png", "Double Contour Integral with Stacked Limits", "\\oiint28"),
            new("Integrals/TripleContourIntegral_Light.png", "Integrals/TripleContourIntegral_Dark.png", "Triple Contour Integral", "\\oiiint4"),
            new("Integrals/TripleContourIntegralWithLimits_Light.png", "Integrals/TripleContourIntegralWithLimits_Dark.png", "Triple Contour Integral with Limits", "\\oiiint24"),
            new("Integrals/TripleContourIntegralWithStackedLimits_Light.png", "Integrals/TripleContourIntegralWithStackedLimits_Dark.png", "Triple Contour Integral with Stacked Limits", "\\oiiint28"),
        };
        return integrals;
    }

    public static ObservableCollection<MathStucture> GetLargeOperatorsCollection()
    {
        var integrals = new ObservableCollection<MathStucture>
        {
            new("LargeOperators/Summation_Light.png", "LargeOperators/Summation_Dark.png", "Summation", "\\sum4"),
            new("LargeOperators/SummationWithLimits_Light.png", "LargeOperators/SummationWithLimits_Dark.png", "Summation with Limits", "\\sum28"),
            new("LargeOperators/SummationWithStackedLimits_Light.png", "LargeOperators/SummationWithStackedLimits_Dark.png", "Summation with Stacked Limits", "\\sum24"),
            new("LargeOperators/SummationWithLowerLimit_Light.png", "LargeOperators/SummationWithLowerLimit_Dark.png", "Summation with Lower Limit", "\\sum12"),
            new("LargeOperators/SummationWithStackedLowerLimit_Light.png", "LargeOperators/SummationWithStackedLowerLimit_Dark.png", "Summation with Stacked Lower Limit", "\\sum8"),
            new("LargeOperators/Product_Light.png", "LargeOperators/Product_Dark.png", "Product", "\\prod4"),
            new("LargeOperators/ProductWithLimits_Light.png", "LargeOperators/ProductWithLimits_Dark.png", "Product with Limits", "\\prod28"),
            new("LargeOperators/ProductWithStackedLimits_Light.png", "LargeOperators/ProductWithStackedLimits_Dark.png", "Product with Stacked Limits", "\\prod24"),
            new("LargeOperators/ProductWithLowerLimit_Light.png", "LargeOperators/ProductWithLowerLimit_Dark.png", "Product with Lower Limit", "\\prod12"),
            new("LargeOperators/ProductWithStackedLowerLimit_Light.png", "LargeOperators/ProductWithStackedLowerLimit_Dark.png", "Product with Stacked Lower Limit", "\\prod8"),
            new("LargeOperators/CoProduct_Light.png", "LargeOperators/CoProduct_Dark.png", "CoProduct", "\\amalg4"),
            new("LargeOperators/CoProductWithLimits_Light.png", "LargeOperators/CoProductWithLimits_Dark.png", "CoProduct with Limits", "\\amalg28"),
            new("LargeOperators/CoProductWithStackedLimits_Light.png", "LargeOperators/CoProductWithStackedLimits_Dark.png", "CoProduct with Stacked Limits", "\\amalg24"),
            new("LargeOperators/CoProductWithLowerLimit_Light.png", "LargeOperators/CoProductWithLowerLimit_Dark.png", "CoProduct with Lower Limit", "\\amalg12"),
            new("LargeOperators/CoProductWithStackedLowerLimit_Light.png", "LargeOperators/CoProductWithStackedLowerLimit_Dark.png", "CoProduct with Stacked Lower Limit", "\\amalg8"),
        };
        return integrals;
    }

    // Import the necessary functions from user32.dll to manipulate keyboard input.
    [DllImport("user32.dll")]
    private static extern IntPtr LoadKeyboardLayout(string pwszKLID, uint Flags);

    [DllImport("user32.dll")]
    private static extern short VkKeyScanA(char ch); // Converts a character to its virtual-key code for the current keyboard layout.

    [DllImport("user32.dll", SetLastError = true)]
    private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

    // Import the LocaleNameToLCID function from kernel32.dll
    [DllImport("Kernel32.dll", SetLastError = true)]
    private static extern uint LocaleNameToLCID([MarshalAs(UnmanagedType.LPWStr)] string lpName, uint dwFlags);

    // Constants for keyboard events.
    private const uint KLF_ACTIVATE = 1; // Activates the specified keyboard layout.
    private const byte VK_SHIFT = 0x10; // Virtual-key code for the Shift key.
    private const uint KEYEVENTF_KEYUP = 0x0002; // Flag indicating a key release event.

    /// <summary>
    /// Simulates typing a command character by character using virtual keyboard events.
    /// This is necessary because math formulas are rendered dynamically as they are typed.
    /// If text is simply set in a RichEditBox, it will not trigger formula rendering.
    /// </summary>
    public static void TypeCommand(string text)
    {
        ApplicationLanguages.PrimaryLanguageOverride = "en-US";

        // Converts `"en-US"` locale name to LCID number.
        uint lcid = LocaleNameToLCID("en-US", 0);

        // Convert LCID to hexadecimal string format for LoadKeyboardLayout
        string klid = lcid.ToString("X8"); // Format LCID as 4-digit hex

        // Load the keyboard layout
        LoadKeyboardLayout(klid, KLF_ACTIVATE);

        // Append a space at the end to ensure the formula is fully processed.
        foreach (char c in (text + " "))
        {
            // Get the virtual-key code for the character.
            short vks = VkKeyScanA(c);
            byte keyCode = (byte)(vks & 0xFF);
            bool shiftRequired = (vks & 0x100) != 0; // Check if the Shift key is needed.

            // Press the Shift key if required.
            if (shiftRequired)
            {
                keybd_event(VK_SHIFT, 0, 0, 0);
            }

            // Simulate key press and release.
            keybd_event(keyCode, 0, 0, 0); // Key press
            keybd_event(keyCode, 0, 2, 0); // Key release

            // Release the Shift key if it was pressed.
            if (shiftRequired)
            {
                keybd_event(VK_SHIFT, 0, KEYEVENTF_KEYUP, 0);
            }
        }
    }
}

public class MathSymbol
{
    public string Value { get; set; }
    public string Name { get; set; }
    public string Command { get; set; }
    public string Unicode { get; set; }

    public MathSymbol(string value, string name, string command, string unicode)
    {
        Value = value;
        Name = name;
        Command = command;
        Unicode = unicode;
    }
}

public class MathStucture
{
    public string LightImageSource { get; set; }
    public string DarkImageSource { get; set; }
    public string Name { get; set; }
    public string Command { get; set; }

    public MathStucture(string LightImageUri, string DarkImageUri, string name, string command)
    {
        LightImageSource = "ms-appx:///Assets/MathModeImages/" + LightImageUri;
        DarkImageSource = "ms-appx:///Assets/MathModeImages/" + DarkImageUri;
        Name = name;
        Command = command;
    }
}
