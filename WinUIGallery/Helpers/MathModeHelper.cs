// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Xml.Linq;

namespace WinUIGallery.Helpers;

/// <summary>
/// Represents different categories of mathematical structures.
/// </summary>
public enum StructuresCategory
{
    BasicStructures,
    Integrals,
    LargeOperators,
    Accents,
    LimitAndFunctions,
    Matrices
}

public static class MathModeHelper
{
    /// <summary>
    /// Retrieves a collection of commonly used mathematical symbols.
    /// </summary>
    /// <returns>
    /// An <see cref="List{MathSymbol}"/> containing mathematical symbols, 
    /// their descriptions, Unicode NPT representations, and Unicode values.
    /// </returns>
    public static List<MathSymbol> GetSymbols()
    {
        var symbols = new List<MathSymbol>
        {
            new("\u221E", "Infinity", "\\infty", "U+221E"),
            new("\u2248", "Approximately Equal", "\\approx", "U+2248"),
            new("\u00D7", "Multiplication", "\\times", "U+00D7"),
            new("\u00F7", "Division", "\\div", "U+00F7"),
            new("\u00B1", "Plus-minus", "\\pm", "U+00B1"),
            new("\u2213", "Minus-plus", "\\mp", "U+2213"),
            new("\u22C5", "Dot product", "\\cdot", "U+22C5"),
            new("\u2218", "Function composition", "\\circ", "U+2218"),
            new("\u22C6", "Star operator", "\\star", "U+22C6"),
            new("\u2022", "Bullet operator", "\\bullet", "U+2022"),
            new("\u2295", "Direct sum", "\\oplus", "U+2295"),
            new("\u2297", "Tensor product", "\\otimes", "U+2297"),
            new("\u2229", "Intersection", "\\cap", "U+2229"),
            new("\u222A", "Union", "\\cup", "U+222A"),
            new("\u2216", "Set difference", "\\setminus", "U+2216"),
            new("\u2205", "Empty set", "\\emptyset", "U+2205"),
            new("\u2283", "Superset", "\\supset", "U+2283"),
            new("\u2282", "Subset", "\\subset", "U+2282"),
            new("\u2287", "Superset or equal", "\\supseteq", "U+2287"),
            new("\u2286", "Subset or equal", "\\subseteq", "U+2286"),
            new("\u22A5", "Perpendicular", "\\perp", "U+22A5"),
            new("\u2225", "Parallel", "\\parallel", "U+2225"),
            new("\u2226", "Not parallel", "\\nparallel", "U+2226"),
            new("\u21D2", "Implies", "\\Rightarrow", "U+21D2"),
            new("\u21D0", "Left implies", "\\Leftarrow", "U+21D0"),
            new("\u21D4", "If and only if", "\\Leftrightarrow", "U+21D4"),
            new("\u2200", "For all", "\\forall", "U+2200"),
            new("\u2203", "There exists", "\\exists", "U+2203"),
            new("\u2204", "Does not exist", "\\nexists", "U+2204"),
            new("\u00AC", "Logical NOT", "\\neg", "U+00AC"),
            new("\u2228", "Logical OR", "\\lor", "U+2228"),
            new("\u2227", "Logical AND", "\\land", "U+2227"),
            new("\u2234", "Therefore", "\\therefore", "U+2234"),
            new("\u2235", "Because", "\\because", "U+2235"),
            new("\u221D", "Proportional to", "\\propto", "U+221D"),
            new("\u221A", "Square root", "\\surd", "U+221A"),
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

    /// <summary>
    /// Retrieves a collection of mathematical structures based on the specified category.
    /// Uses collection initializers for better performance and readability.
    /// </summary>
    /// <param name="category">The category of mathematical items to retrieve.</param>
    /// <returns>
    /// A <see cref="List{MathStructure}"/> containing mathematical items related to the specified category.
    /// </returns>
    public static List<MathStructure> GetStructures(StructuresCategory category)
    {
        return category switch
        {
            StructuresCategory.BasicStructures => new List<MathStructure>
        {
            new("Structures/StackedFraction", "stacked fraction", "/", "/ "),
            new("Structures/SkewedFraction", "Skewed fraction","\\sdiv ", "\u2044 "),
            new("Structures/LinearFraction", "Linear fraction", "\\ldiv ", "\u2215 "),
            new("Structures/StackObject", "Stack Object", "\\atop ", "\u00A6 "),
            new("Structures/Superscript", "Superscript", "\\begin^\\end ", "\u3016^\u3017 "),
            new("Structures/Subscript", "Subscript", "\\begin_\\end ", "\u3016_\u3017 "),
            new("Structures/SuperscriptSubscript", "Superscript and subscript", "\\open\\close^_ ", "\u251C\u2524^_  "),
            new("Structures/SuperscriptSubscriptLeft", "Left superscript and subscript", "(^_)", "(^_) "),
            new("Structures/SquareRoot", "Square root", "\\sqrt ", "\u221A "),
            new("Structures/RadicalWithDegree", "Radical with degree", "\\sqrt(&)", "\u221A(&) "),
            new("Structures/SquareRootWithDegree", "Square root with degree", "\\sqrt(2&)", "\u221A(2&) "),
            new("structures/CubicRoot", "Cubic root", "\\cbrt ", "\u221B "),
        },

            StructuresCategory.Integrals => new List<MathStructure>
        {
            new("Integrals/Integral", "Integral", "\\int4", "\u222B4 "),
            new("Integrals/IntegralWithLimits", "Integral with limits", "\\int24", "\u222B24 "),
            new("Integrals/IntegralWithStackedLimits", "Integral with stacked limits", "\\int28", "\u222B28 "),
            new("Integrals/DoubleIntegral", "Double integral", "\\iint4", "\u222C4 "),
            new("Integrals/DoubleIntegralWithLimits", "Double integral with limits", "\\iint24", "\u222C24 "),
            new("Integrals/DoubleIntegralWithStackedLimits", "Double Integral with stacked limits", "\\iint28", "\u222C28 "),
            new("Integrals/TripleIntegral", "Triple integral", "\\iiint4", "\u222C4 "),
            new("Integrals/TripleIntegralWithLimits", "Triple integral with limits", "\\iiint24", "\u222D24 "),
            new("Integrals/TripleIntegralWithStackedLimits", "Triple integral with stacked limits", "\\iiint28", "\u222D28 "),
            new("Integrals/ContourIntegral", "Contour integral", "\\oint4", "\u222E4 "),
            new("Integrals/ContourIntegralWithLimits", "Contour integral with limits", "\\oint24", "\u222E24 "),
            new("Integrals/ContourIntegralWithStackedLimits", "Contour integral with stacked limits", "\\oint28", "\u222E28 "),
            new("Integrals/DoubleContourIntegral", "Double contour integral", "\\oiint4", "\u222F4 "),
            new("Integrals/DoubleContourIntegralWithLimits", "Double contour integral with limits", "\\oiint24", "\u222F24 "),
            new("Integrals/DoubleContourIntegralWithStackedLimits", "Double contour integral with stacked limits", "\\oiint28", "\u222F28 "),
            new("Integrals/TripleContourIntegral", "Triple contour integral", "\\oiiint4", "\u22304 "),
            new("Integrals/TripleContourIntegralWithLimits", "Triple contour integral with limits", "\\oiiint24", "\u223024 "),
            new("Integrals/TripleContourIntegralWithStackedLimits", "Triple contour integral with stacked limits", "\\oiiint28", "\u223028 ")        },

            StructuresCategory.LargeOperators => new List<MathStructure>
        {
            new("LargeOperators/Summation", "Summation", "\\sum4", "\u22114 "),
            new("LargeOperators/SummationWithLimits", "Summation with limits", "\\sum28", "\u221128 "),
            new("LargeOperators/SummationWithStackedLimits", "Summation with stacked limits", "\\sum24", "\u221124 "),
            new("LargeOperators/SummationWithLowerLimit", "Summation with lower limit", "\\sum12", "\u221112 "),
            new("LargeOperators/SummationWithStackedLowerLimit", "Summation with stacked lower limit", "\\sum8", "\u22118 "),
            new("LargeOperators/Product", "Product", "\\prod4", "\u220F4 "),
            new("LargeOperators/ProductWithLimits", "Product with limits", "\\prod28", "\u220F28 "),
            new("LargeOperators/ProductWithStackedLimits", "Product with stacked limits", "\\prod24", "\u220F24 "),
            new("LargeOperators/ProductWithLowerLimit", "Product with lower limit", "\\prod12", "\u220F12 "),
            new("LargeOperators/ProductWithStackedLowerLimit", "Product with stacked lower limit", "\\prod8", "\u220F8 "),
            new("LargeOperators/CoProduct", "CoProduct", "\\coprod4", "\u22104 "),
            new("LargeOperators/CoProductWithLimits", "CoProduct with limits", "\\coprod28", "\u221028 "),
            new("LargeOperators/CoProductWithStackedLimits", "CoProduct with stacked limits", "\\coprod24", "\u221024 "),
            new("LargeOperators/CoProductWithLowerLimit", "CoProduct with lower limit", "\\coprod12", "\u221012 "),
            new("LargeOperators/CoProductWithStackedLowerLimit", "CoProduct with stacked lower limit", "\\coprod8", "\u22108 "),
            new("LargeOperators/Union", "Union", "\\bigcup4", "\u22C34 "),
            new("LargeOperators/UnionWithLimits", "Union with Limits", "\\bigcup28", "\u22C328 "),
            new("LargeOperators/UnionWithStackedLimits", "Union with stacked limits", "\\bigcup24", "\u22C324 "),
            new("LargeOperators/UnionWithLowerLimit", "Union with lower limit", "\\bigcup12", "\u22C312 "),
            new("LargeOperators/UnionWithStackedLowerLimit", "Union with stacked lower limit", "\\bigcup8", "\u22C38 "),
            new("LargeOperators/Intersection", "Intersection", "\\bigcap4", "\u22C24 "),
            new("LargeOperators/IntersectionWithLimits", "Intersection with Limits", "\\bigcap28", "\u22C24 "),
            new("LargeOperators/IntersectionWithStackedLimits", "Intersection with stacked limits", "\\bigcap24", "\u22C224 "),
            new("LargeOperators/IntersectionWithLowerLimit", "Intersection with lower limit", "\\bigcap12", "\u22C212 "),
            new("LargeOperators/IntersectionWithStackedLowerLimit", "Intersection with stacked lower limit", "\\bigcap8", "\u22C28 "),
            new("LargeOperators/LogicalOr", "Logical OR", "\\bigvee4", "\u22C14 "),
            new("LargeOperators/LogicalOrWithLimits", "Logical OR with Limits", "\\bigvee28", "\u22C128 "),
            new("LargeOperators/LogicalOrWithStackedLimits", "Logical OR with stacked limits", "\\bigvee24", "\u22C124 "),
            new("LargeOperators/LogicalOrWithLowerLimit", "Logical OR with lower limit", "\\bigvee12", "\u22C112 "),
            new("LargeOperators/LogicalOrWithStackedLowerLimit", "Logical OR with stacked lower limit", "\\bigvee8", "\u22C18 "),
            new("LargeOperators/LogicalAnd", "Logical AND", "\\bigwedge4", "\u22C04 "),
            new("LargeOperators/LogicalAndWithLimits", "Logical AND with Limits", "\\bigwedge28", "\u22C028 "),
            new("LargeOperators/LogicalAndWithStackedLimits", "Logical AND with stacked limits", "\\bigwedge24", "\u22C024 "),
            new("LargeOperators/LogicalAndWithLowerLimit", "Logical AND with lower limit", "\\bigwedge12", "\u22C012 "),
            new("LargeOperators/LogicalAndWithStackedLowerLimit", "Logical AND with stacked lower limit", "\\bigwedge8", "\u22C08 ")
        },

            StructuresCategory.Accents => new List<MathStructure>
        {
            new("Accents/Dot", "Dot", "\\dot ", "\u0307 "),
            new("Accents/DoubleDot", "Double dot", "\\ddot ", "\u0308 "),
            new("Accents/TripleDot", "Triple dot", "\\dddot ", "\u20DB "),
            new("Accents/Hat", "Hat", "\\hat ", "\u0302 "),
            new("Accents/Check", "Check", "\\check ", "\u030C "),
            new("Accents/Acute", "Acute", "\\acute ", "\u0301 "),
            new("Accents/Grave", "Grave", "\\grave ", "\u0300 "),
            new("Accents/Breve", "Breve", "\\breve ", "\u0306 "),
            new("Accents/Tilde", "Tilde", "\\tilde ", "\u0303 "),
            new("Accents/Bar", "Bar", "\\bar ", "\u0305 "),
            new("Accents/DoubleOverbar", "Double overbar", "\\Bar ", "\u033F "),
            new("Accents/Overbrace", "Overbrace", "\\open\\overbrace\\close ", "\u251C\u23DE\u2524 "),
            new("Accents/Underbrace", "Underbrace", "\\open\\underbrace\\close ", "\u251C\u23DF\u2524 "),
            new("Accents/GroupingCharacterAbove", "Grouping character above", "\\open\\overbrace\\above\\close ", "\u251C\u23DE\u2534\u2524 "),
            new("Accents/GroupingCharacterBelow", "Grouping character below", "\\open\\underbrace\\below\\close ", "\u251C\u23DF\u252C\u2524 "),
            new("Accents/LeftwardsArrowAbove", "Leftwards arrow above", "\\lvec ", "\u20D6 "),
            new("Accents/RightwardsArrowAbove", "Rightwards arrow above", "\\vec ", "\u20D7 "),
            new("Accents/Left-RightArrowAbove", "Left-Right arrow above", "\\open\\tvec\\close ", "\u251C\u20E1\u2524 "),
            new("Accents/LeftwardsHarpoonAbove", "Leftwards harpoon above", "\\lhvec ", "\u20D0 "),
            new("Accents/RightwardsHarpoonAbove", "Rightwards harpoon above", "\\hvec ", "\u20D1 "),
            new("Accents/Overbar", "Overbar", "\\overline ", "\u00AF "),
            new("Accents/Underbar", "Underbar", "\\underline ", "\u2581 ")
        },

            StructuresCategory.LimitAndFunctions => new List<MathStructure>
        {
            new("LimitAndFunctions/Limit", "Limit", "lim\\below(\\dots) ", "lim\u252C(\u2026) "),
            new("LimitAndFunctions/Minimum", "Minimum", "min\\below(\\dots) ", "min\u252C(\u2026) "),
            new("LimitAndFunctions/Maximum", "Maximum", "max\\below(\\dots) ", "max\u252C(\u2026) "),
            new("LimitAndFunctions/LogarithmWithEmptyBase", "Logarithm with empty base", "log_ "),
            new("LimitAndFunctions/LogarithmWithNoBase", "Logarithm with no base", "log "),
            new("LimitAndFunctions/NaturalLogarithm", "Natural logarithm", "ln "),
            new("LimitAndFunctions/SineFunction", "Sine function", "sin "),
            new("LimitAndFunctions/CosineFunction", "Cosine function", "cos "),
            new("LimitAndFunctions/TangentFunction", "Tangent function", "tan "),
            new("LimitAndFunctions/CosecantFunction", "Cosecant function", "csc "),
            new("LimitAndFunctions/SecantFunction", "Secant function", "sec "),
            new("LimitAndFunctions/CotangentFunction", "Cotangent function", "cot "),
            new("LimitAndFunctions/InverseSineFunction", "Inverse sine function", "sin^-1 "),
            new("LimitAndFunctions/InverseCosineFunction", "Inverse cosine function", "cos^-1 "),
            new("LimitAndFunctions/InverseTangentFunction", "Inverse tangent function", "tan^-1 "),
            new("LimitAndFunctions/InverseCosecantFunction", "Inverse cosecant function", "csc^-1 "),
            new("LimitAndFunctions/InverseSecantFunction", "Inverse secant function", "sec^-1 "),
            new("LimitAndFunctions/InverseCotangentFunction", "Inverse cotangent function", "cot^-1 "),
            new("LimitAndFunctions/HyperbolicSineFunction", "Hyperbolic sine function", "sinh "),
            new("LimitAndFunctions/HyperbolicCosineFunction", "Hyperbolic cosine function", "cosh "),
            new("LimitAndFunctions/HyperbolicTangentFunction", "Hyperbolic tangent function", "tanh "),
            new("LimitAndFunctions/HyperbolicCosecantFunction", "Hyperbolic cosecant function", "csch "),
            new("LimitAndFunctions/HyperbolicSecantFunction", "Hyperbolic secant function", "sech "),
            new("LimitAndFunctions/HyperbolicCotangentFunction", "Hyperbolic cotangent function", "coth "),
            new("LimitAndFunctions/InverseHyperbolicSineFunction", "Inverse hyperbolic sine function", "sinh^-1 "),
            new("LimitAndFunctions/InverseHyperbolicCosineFunction", "Inverse hyperbolic cosine function", "cosh^-1 "),
            new("LimitAndFunctions/InverseHyperbolicTangentFunction", "Inverse hyperbolic tangent function", "tanh^-1 "),
            new("LimitAndFunctions/InverseHyperbolicCosecantFunction", "Inverse hyperbolic cosecant function", "csch^-1 "),
            new("LimitAndFunctions/InverseHyperbolicSecantFunction", "Inverse hyperbolic secant function", "sech^-1 "),
            new("LimitAndFunctions/InverseHyperbolicCotangentFunction", "Inverse hyperbolic cotangent function", "coth^-1 ")
        },

            StructuresCategory.Matrices => new List<MathStructure>
        {
            new("Matrices/1x2EmptyMatrix", "1\u00D72 empty matrix", "\\matrix(\\open\\close &\\open\\close  ) ", "\u25A0(\u251C\u2524 &\u251C\u2524 ) "),
            new("Matrices/2x1EmptyMatrix", "2\u00D71 empty matrix", "\\matrix(\\open\\close @\\open\\close  ) ", "\u25A0(\u251C\u2524 @\u251C\u2524 ) "),
            new("Matrices/2x2EmptyMatrix", "2\u00D72 empty matrix", "\\matrix(\\open\\close &\\open\\close @\\open\\close &\\open\\close  ) ", "\u25A0(\u251C\u2524 &\u251C\u2524 @\u251C\u2524 &\u251C\u2524 ) "),
            new("Matrices/1x3EmptyMatrix", "1\u00D73 empty matrix", "\\matrix(\\open\\close &\\open\\close &\\open\\close  )", "\u25A0(\u251C\u2524 &\u251C\u2524 &\u251C\u2524 ) "),
            new("Matrices/3x1EmptyMatrix", "3\u00D71 empty matrix", "\\matrix(\\open\\close @\\open\\close @\\open\\close  )", "\u25A0(\u251C\u2524 @\u251C\u2524 @\u251C\u2524 ) "),
            new("Matrices/2x2IdentityMatrixWithZeros", "2\u00D72 identity matrix with zeros", "\\matrix(1&0@0&1) ", "\u25A0(1&0@0&1) "),
            new("Matrices/2x2IdentityMatrix", "2\u00D72 identity matrix", "\\matrix(1&@&1) ", "\u25A0(1&@&1) "),
            new("Matrices/3x3IdentityMatrixWithZeros", "3\u00D73 identity matrix with zeros", "\\matrix(1&0&0@0&1&0@0&0&1) ", "\u25A0(1&0&0@0&1&0@0&0&1) "),
            new("Matrices/3x3IdentityMatrix", "3\u00D73 Identity matrix", "\\matrix(1&&@&1&@&&1) ", "\u25A0(1&&@&1&@&&1) ")
        },

            _ => new List<MathStructure>()
        };
    }

    /// <summary>
    /// Formats the given MathML string by parsing it into an XDocument
    /// and returning a properly indented string representation.
    /// </summary>
    /// <param name="mathML">The MathML string to format.</param>
    /// <returns>
    /// A well-formatted MathML string with proper indentation.
    /// If parsing fails, returns the original input.
    /// </returns>
    /// <remarks>
    /// This method uses XDocument for XML formatting.
    /// If parsing fails, it logs an error using Debug.WriteLine and returns the original MathML string.
    /// Consider additional error handling if needed.
    /// </remarks>
    public static string FormatMathML(string mathML)
    {
        try
        {
            XDocument doc = XDocument.Parse(mathML);
            return doc.ToString(); // This automatically formats with proper indentation
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error formatting MathML: {ex.Message}");
            return mathML; // Return the original in case of an error
        }
    }
}

public class MathSymbol
{
    public string Value { get; set; }
    public string Name { get; set; }
    public string UnicodeNPT { get; set; }
    public string Unicode { get; set; }

    public MathSymbol(string value, string name, string unicodeNPT, string unicode)
    {
        Value = value;
        Name = name;
        UnicodeNPT = unicodeNPT;
        Unicode = unicode;
    }
}

public class MathStructure
{
    public string LightImageSource { get; set; }
    public string DarkImageSource { get; set; }
    public string Name { get; set; }
    public string UnicodeNPT { get; set; }
    public string Unicode { get; set; }

    public MathStructure(string ImageUri, string name, string unicodeNPT, string unicode)
    {
        LightImageSource = "ms-appx:///Assets/MathModeImages/" + ImageUri + "_Light.png";
        DarkImageSource = "ms-appx:///Assets/MathModeImages/" + ImageUri + "_Dark.png";
        Name = name;
        UnicodeNPT = unicodeNPT;
        Unicode = unicode;
    }

    public MathStructure(string ImageUri, string name, string sharedUnicode)
    {
        LightImageSource = "ms-appx:///Assets/MathModeImages/" + ImageUri + "_Light.png";
        DarkImageSource = "ms-appx:///Assets/MathModeImages/" + ImageUri + "_Dark.png";
        Name = name;
        UnicodeNPT = sharedUnicode;
        Unicode = sharedUnicode;
    }
}

public static class InputSender
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

    /// <summary>
    /// Sends a sequence of Unicode characters individually.
    /// </summary>
    /// <param name="unicodeText">The Unicode string to be sent character by character.</param>
    /// <remarks>
    /// This method uses <c>SendUnicodeCharacter()</c> to simulate keyboard input for each character
    /// in the provided string using the Win32 <c>SendInput</c> API.
    /// 
    /// Ideal for scenarios where text integrity is critical, such as math editors, programming environments, or input automation.
    /// 
    /// If a character fails to send, a detailed exception is thrown with the Unicode code point that caused the failure.
    /// </remarks>
    public static void SendUnicodeText(string unicodeText)
    {
        foreach (char c in unicodeText)
        {
            try
            {
                SendUnicodeCharacter(c);
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                throw new Exception(
                    $"Failed to send character '{c}' (U+{((int)c):X4}). Unicode text input aborted.",
                    ex
                );
            }
        }
    }

}