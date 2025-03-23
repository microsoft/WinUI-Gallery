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
    public static ObservableCollection<MathStucture> GetStructureCollection()
    {
        var structures = new ObservableCollection<MathStucture>
        {
            new("Structures/StackedFraction_Light.png", "Structures/StackedFraction_Dark.png", "Stacked Fraction", "/"),
            new("Structures/Superscript_Light.png", "Structures/Superscript_Dark.png", "Superscript", "\\begin^\\end "),
            new("Structures/Subscript_Light.png", "Structures/Subscript_Dark.png", "Subscript", "\\begin_\\end "),
            new("Structures/SuperscriptSubscript_Light.png", "Structures/SuperscriptSubscript_Dark.png", "Superscript and Subscript", "()^_"),
            new("Structures/SuperscriptSubscriptLeft_Light.png", "Structures/SuperscriptSubscriptLeft_Dark.png", "Superscript and Subscript", "(^_)"),
            new("Structures/SquareRoot_Light.png", "Structures/SquareRoot_Dark.png", "Square Root", "\\sqrt "),
            new("Structures/RadicalWithDegree_Light.png", "Structures/RadicalWithDegree_Dark.png", "Radical with Degree", "\\sqrt(&)"),
            new("Structures/SquareRootWithDegree_Light.png", "Structures/SquareRootWithDegree_Dark.png", "Square Root with Degree", "\\sqrt(2&)"),
            new("structures/CubicRoot_Light.png", "structures/CubicRoot_Dark.png", "Cubic Root", "\\cbrt "),
        };
        return structures;
    }

    [DllImport("user32.dll")]
    private static extern IntPtr LoadKeyboardLayout(string pwszKLID, uint Flags);

    [DllImport("user32.dll")]
    private static extern IntPtr GetKeyboardLayout(uint idThread);

    [DllImport("user32.dll")]
    private static extern short VkKeyScanA(char ch); // Enforce US layout

    [DllImport("user32.dll", SetLastError = true)]
    private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

    private const uint KLF_ACTIVATE = 1;
    private const byte VK_SHIFT = 0x10;
    private const uint KEYEVENTF_KEYUP = 0x0002;

    public static void TypeCommand(string text)
    {
        ApplicationLanguages.PrimaryLanguageOverride = "en-US";
        LoadKeyboardLayout("00000409", KLF_ACTIVATE); // en-US layout

        foreach (char c in (text + " "))
        {
            short vks = VkKeyScanA(c);
            byte keyCode = (byte)(vks & 0xFF);
            bool shiftRequired = (vks & 0x100) != 0;

            if (shiftRequired)
            {
                keybd_event(VK_SHIFT, 0, 0, 0);
            }
            keybd_event(keyCode, 0, 0, 0);
            keybd_event(keyCode, 0, 2, 0);

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
