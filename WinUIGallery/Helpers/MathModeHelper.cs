// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

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
            new("\u03C9", "Omega", "\\omega", "U+03C9")
        };
        return symbols;
    }

    public static ObservableCollection<MathStucture> GetStructuresCollection()
    {
        var structures = new ObservableCollection<MathStucture>
        {
            new("Structures/StackedFraction", "Stacked Fraction", "/"),
            new("Structures/SkewedFraction", "Skewed Fraction","\\sdiv "),
            new("Structures/LinearFraction", "Linear Fraction", "\\ldiv "),
            new("Structures/Superscript", "Superscript", "\\begin^\\end "),
            new("Structures/Subscript", "Subscript", "\\begin_\\end "),
            new("Structures/SuperscriptSubscript", "Superscript and Subscript", "\\open\\close^_ "),
            new("Structures/SuperscriptSubscriptLeft", "Left Superscript and Subscript", "(^_)"),
            new("Structures/SquareRoot", "Square Root", "\\sqrt "),
            new("Structures/RadicalWithDegree", "Radical with Degree", "\\sqrt(&)"),
            new("Structures/SquareRootWithDegree", "Square Root with Degree", "\\sqrt(2&)"),
            new("structures/CubicRoot", "Cubic Root", "\\cbrt "),
        };
        return structures;
    }

    public static ObservableCollection<MathStucture> GetIntegralsCollection()
    {
        var integrals = new ObservableCollection<MathStucture>
        {
            new("Integrals/Integral", "Integral", "\\int4"),
            new("Integrals/IntegralWithLimits", "Integral with Limits", "\\int24"),
            new("Integrals/IntegralWithStackedLimits", "Integral with Stacked Limits", "\\int28"),
            new("Integrals/DoubleIntegral", "Double Integral", "\\iint4"),
            new("Integrals/DoubleIntegralWithLimits", "Double Integral with Limits", "\\iint24"),
            new("Integrals/DoubleIntegralWithStackedLimits", "Double Integral with Stacked Limits", "\\iint28"),
            new("Integrals/TripleIntegral", "Triple Integral", "\\iiint4"),
            new("Integrals/TripleIntegralWithLimits", "Triple Integral with Limits", "\\iiint24"),
            new("Integrals/TripleIntegralWithStackedLimits", "Triple Integral with Stacked Limits", "\\iiint28"),
            new("Integrals/ContourIntegral", "Contour Integral", "\\oint4"),
            new("Integrals/ContourIntegralWithLimits", "Contour Integral with Limits", "\\oint24"),
            new("Integrals/ContourIntegralWithStackedLimits", "Contour Integral with Stacked Limits", "\\oint28"),
            new("Integrals/DoubleContourIntegral", "Double Contour Integral", "\\oiint4"),
            new("Integrals/DoubleContourIntegralWithLimits", "Double Contour Integral with Limits", "\\oiint24"),
            new("Integrals/DoubleContourIntegralWithStackedLimits", "Double Contour Integral with Stacked Limits", "\\oiint28"),
            new("Integrals/TripleContourIntegral", "Triple Contour Integral", "\\oiiint4"),
            new("Integrals/TripleContourIntegralWithLimits", "Triple Contour Integral with Limits", "\\oiiint24"),
            new("Integrals/TripleContourIntegralWithStackedLimits", "Triple Contour Integral with Stacked Limits", "\\oiiint28"),
        };
        return integrals;
    }

    public static ObservableCollection<MathStucture> GetLargeOperatorsCollection()
    {
        var integrals = new ObservableCollection<MathStucture>
        {
            new("LargeOperators/Summation", "Summation", "\\sum4"),
            new("LargeOperators/SummationWithLimits", "Summation with Limits", "\\sum28"),
            new("LargeOperators/SummationWithStackedLimits", "Summation with Stacked Limits", "\\sum24"),
            new("LargeOperators/SummationWithLowerLimit", "Summation with Lower Limit", "\\sum12"),
            new("LargeOperators/SummationWithStackedLowerLimit", "Summation with Stacked Lower Limit", "\\sum8"),
            new("LargeOperators/Product", "Product", "\\prod4"),
            new("LargeOperators/ProductWithLimits", "Product with Limits", "\\prod28"),
            new("LargeOperators/ProductWithStackedLimits", "Product with Stacked Limits", "\\prod24"),
            new("LargeOperators/ProductWithLowerLimit", "Product with Lower Limit", "\\prod12"),
            new("LargeOperators/ProductWithStackedLowerLimit", "Product with Stacked Lower Limit", "\\prod8"),
            new("LargeOperators/CoProduct", "CoProduct", "\\coprod4"),
            new("LargeOperators/CoProductWithLimits", "CoProduct with Limits", "\\coprod28"),
            new("LargeOperators/CoProductWithStackedLimits", "CoProduct with Stacked Limits", "\\coprod24"),
            new("LargeOperators/CoProductWithLowerLimit", "CoProduct with Lower Limit", "\\coprod12"),
            new("LargeOperators/CoProductWithStackedLowerLimit", "CoProduct with Stacked Lower Limit", "\\coprod8"),
            new("LargeOperators/Union", "Union", "\\bigcup4"),
            new("LargeOperators/UnionWithLimits", "Union with Limits", "\\bigcup28"),
            new("LargeOperators/UnionWithStackedLimits", "Union with Stacked Limits", "\\bigcup24"),
            new("LargeOperators/UnionWithLowerLimit", "Union with Lower Limit", "\\bigcup12"),
            new("LargeOperators/UnionWithStackedLowerLimit", "Union with Stacked Lower Limit", "\\bigcup8"),
            new("LargeOperators/Intersection", "Intersection", "\\bigcap4"),
            new("LargeOperators/IntersectionWithLimits", "Intersection with Limits", "\\bigcap28"),
            new("LargeOperators/IntersectionWithStackedLimits", "Intersection with Stacked Limits", "\\bigcap24"),
            new("LargeOperators/IntersectionWithLowerLimit", "Intersection with Lower Limit", "\\bigcap12"),
            new("LargeOperators/IntersectionWithStackedLowerLimit", "Intersection with Stacked Lower Limit", "\\bigcap8"),
            new("LargeOperators/LogicalOr", "Logical OR", "\\bigvee4"),
            new("LargeOperators/LogicalOrWithLimits", "Logical OR with Limits", "\\bigvee28"),
            new("LargeOperators/LogicalOrWithStackedLimits", "Logical OR with Stacked Limits", "\\bigvee24"),
            new("LargeOperators/LogicalOrWithLowerLimit", "Logical OR with Lower Limit", "\\bigvee12"),
            new("LargeOperators/LogicalOrWithStackedLowerLimit", "Logical OR with Stacked Lower Limit", "\\bigvee8"),
            new("LargeOperators/LogicalAnd", "Logical AND", "\\bigwedge4"),
            new("LargeOperators/LogicalAndWithLimits", "Logical AND with Limits", "\\bigwedge28"),
            new("LargeOperators/LogicalAndWithStackedLimits", "Logical AND with Stacked Limits", "\\bigwedge24"),
            new("LargeOperators/LogicalAndWithLowerLimit", "Logical AND with Lower Limit", "\\bigwedge12"),
            new("LargeOperators/LogicalAndWithStackedLowerLimit", "Logical AND with Stacked Lower Limit", "\\bigwedge8")
        };
        return integrals;
    }

    public static ObservableCollection<MathStucture> GetAccentsCollection()
    {
        var accents = new ObservableCollection<MathStucture>
        {
            new("Accents/Dot", "Dot", "\\dot "),
            new("Accents/DoubleDot", "Double Dot", "\\ddot "),
            new("Accents/TripleDot", "Triple Dot", "\\dddot "),
            new("Accents/Hat", "Hat", "\\hat "),
            new("Accents/Check", "Check", "\\check "),
            new("Accents/Acute", "Acute", "\\acute "),
            new("Accents/Grave", "Grave", "\\grave "),
            new("Accents/Breve", "Breve", "\\breve "),
            new("Accents/Tilde", "Tilde", "\\tilde "),
            new("Accents/Bar", "Bar", "\\bar "),
            new("Accents/DoubleOverbar", "Double Overbar", "\\Bar "),
            new("Accents/Overbrace", "Overbrace", "\\open\\overbrace\\close "),
            new("Accents/Underbrace", "Underbrace", "\\open\\underbrace\\close "),
            new("Accents/GroupingCharacterAbove", "Grouping Character Above", "\\overbrace\\above "),
            new("Accents/GroupingCharacterBelow", "Grouping Character Below", "\\underbrace\\below "),
            new("Accents/LeftwardsArrowAbove", "Leftwards Arrow Above", "\\lvec "),
            new("Accents/RightwardsArrowAbove", "Rightwards Arrow Above", "\\vec "),
            new("Accents/Left-RightArrowAbove", "Left-Right Arrow Above", "\\open\\tvec\\close "),
            new("Accents/LeftwardsHarpoonAbove", "Leftwards Harpoon Above", "\\lhvec "),
            new("Accents/RightwardsHarpoonAbove", "Rightwards Harpoon Above", "\\hvec "),
            new("Accents/Overbar", "Overbar", "\\overline "),
            new("Accents/Underbar", "Underbar", "\\underline ")
        };
        return accents;
    }

    public static ObservableCollection<MathStucture> GetLimitAndFunctionsCollection()
    {
        var limitAndFunctions = new ObservableCollection<MathStucture>
        {
            new("LimitAndFunctions/Limit", "Limit", "\\lim\\below(\\dots)"),
            new("LimitAndFunctions/Minimum", "Minimum", "\\min\\below(\\dots)"),
            new("LimitAndFunctions/Maximum", "Maximum", "\\max\\below(\\dots)"),
            new("LimitAndFunctions/LogarithmWithEmptyBase", "Logarithm with Empty Base", "\\log_"),
            new("LimitAndFunctions/LogarithmWithNoBase", "Logarithm with No Base", "\\log"),
            new("LimitAndFunctions/NaturalLogarithm", "Natural Logarithm", "\\ln")
        };
        return limitAndFunctions;
    }

    public static void TypeCommand(string text)
    {
        // Append a space at the end to ensure the formula is fully processed.
        foreach (char c in (text + " "))
        {
            KeyboardInputSender.SendUnicodeCharacter(c);
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

    public MathStucture(string ImageUri, string name, string command)
    {
        LightImageSource = "ms-appx:///Assets/MathModeImages/" + ImageUri + "_Light.png";
        DarkImageSource = "ms-appx:///Assets/MathModeImages/" + ImageUri + "_Dark.png";
        Name = name;
        Command = command;
    }
}

public class KeyboardInputSender
{
    [StructLayout(LayoutKind.Sequential)]
    struct INPUT
    {
        public uint type;
        public InputUnion u;
    }

    [StructLayout(LayoutKind.Explicit)]
    struct InputUnion
    {
        [FieldOffset(0)] public MOUSEINPUT mi;
        [FieldOffset(0)] public KEYBDINPUT ki;
        [FieldOffset(0)] public HARDWAREINPUT hi;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct MOUSEINPUT
    {
        public int dx;
        public int dy;
        public uint mouseData;
        public uint dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct KEYBDINPUT
    {
        public ushort wVk;
        public ushort wScan;
        public uint dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct HARDWAREINPUT
    {
        public uint uMsg;
        public ushort wParamL;
        public ushort wParamH;
    }

    const uint INPUT_KEYBOARD = 1;
    const uint KEYEVENTF_KEYUP = 0x0002;
    const uint KEYEVENTF_UNICODE = 0x0004;

    [DllImport("user32.dll", SetLastError = true)]
    static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

    /// <summary>
    /// Sends a single Unicode character as keyboard input to the active window.
    /// </summary>
    /// <param name="character">The Unicode character to send.</param>
    /// <remarks>
    /// This method uses the SendInput API to simulate keyboard input for Unicode characters,
    /// witout the need to switch keyboard layouts or input methods.
    /// 
    /// - The first input simulates a key press (keydown) using the Unicode scan code.
    /// - The second input simulates a key release (keyup) to complete the character entry.
    /// 
    /// If the input fails, it throws a Win32 exception with the system error code.
    /// </remarks>
    public static void SendUnicodeCharacter(char character)
    {
        INPUT[] inputs = new INPUT[2];
        int structSize = Marshal.SizeOf(typeof(INPUT));

        // Key Down (Unicode)
        inputs[0] = new INPUT
        {
            type = INPUT_KEYBOARD,
            u = new InputUnion
            {
                ki = new KEYBDINPUT
                {
                    wVk = 0, // Virtual key is not used for Unicode input
                    wScan = character, // Unicode character to send
                    dwFlags = KEYEVENTF_UNICODE, // Unicode flag for key down
                    time = 0,
                    dwExtraInfo = IntPtr.Zero
                }
            }
        };

        // Key Up (Unicode)
        inputs[1] = new INPUT
        {
            type = INPUT_KEYBOARD,
            u = new InputUnion
            {
                ki = new KEYBDINPUT
                {
                    wVk = 0,
                    wScan = character,
                    dwFlags = KEYEVENTF_UNICODE | KEYEVENTF_KEYUP, // Unicode key up event
                    time = 0,
                    dwExtraInfo = IntPtr.Zero
                }
            }
        };

        uint result = SendInput((uint)inputs.Length, inputs, structSize);

        if (result == 0)
        {
            int error = Marshal.GetLastWin32Error();
            throw new System.ComponentModel.Win32Exception(error);
        }
    }
}