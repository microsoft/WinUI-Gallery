// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using WinUIGallery.CatalogExporter;

namespace WinUIGallery.CatalogExporter.Tests;

/// <summary>
/// Pins the behaviour of the last step before publication: removing the $(Token) placeholders the
/// resolver declined to settle.
///
/// The guarantee these tests protect is narrow but absolute - published XAML contains no
/// placeholder - and the way it is kept is by deletion rather than inference. So most of these
/// assert the shape of what is left behind: the attribute carrying the token is gone, everything
/// around it is untouched, and the fragment still parses.
/// </summary>
[TestClass]
public sealed class TokenFallbackTests
{
    [TestMethod]
    public void FragmentWithoutTokens_IsReturnedUnchanged()
    {
        const string Xaml = """<Button Content="Click me" IsEnabled="True" />""";

        Assert.AreEqual(Xaml, TokenFallback.StripFromXaml(Xaml));
    }

    [TestMethod]
    public void TokenInAttribute_RemovesTheWholeAttribute()
    {
        // X1 falls back to its own default, which is what the gallery shows whenever the slider the
        // token bound to was never given an initial value.
        string result = TokenFallback.StripFromXaml("""<Line X1="$(Slider1)" Y1="5" Stroke="Red" />""");

        Assert.AreEqual("""<Line Y1="5" Stroke="Red" />""", result);
    }

    [TestMethod]
    public void TokenInLastAttribute_LeavesASingleSeparatorBeforeTheTagClose()
    {
        string result = TokenFallback.StripFromXaml("""<Line Y1="5" X1="$(Slider1)" />""");

        Assert.AreEqual("""<Line Y1="5" />""", result);
    }

    [TestMethod]
    public void TokenInOnlyAttribute_LeavesTheElementIntact()
    {
        string result = TokenFallback.StripFromXaml("""<StackPanel Orientation="$(Orientation)" />""");

        Assert.AreEqual("<StackPanel />", result);
        Assert.IsTrue(XamlFragment.IsWellFormed(result));
    }

    [TestMethod]
    public void TokenEmbeddedInALargerValue_StillRemovesTheWholeAttribute()
    {
        // InfoBadge writes Style="{StaticResource $(Style)IconInfoBadgeStyle}". Removing only the
        // token would leave a StaticResource key that does not exist, which is worse than no Style
        // at all: it fails at load instead of falling back to the default.
        string result = TokenFallback.StripFromXaml("""<InfoBadge Style="{StaticResource $(Style)IconInfoBadgeStyle}" Value="5" />""");

        Assert.AreEqual("""<InfoBadge Value="5" />""", result);
    }

    [TestMethod]
    public void TwoTokensInOneAttribute_RemoveTheAttributeOnce()
    {
        // RadialGradientBrush writes Center="$(CenterX),$(CenterY)"; the overlapping removals must
        // not cut the surrounding markup twice.
        string result = TokenFallback.StripFromXaml("""<RadialGradientBrush Center="$(CenterX),$(CenterY)" SpreadMethod="Pad" />""");

        Assert.AreEqual("""<RadialGradientBrush SpreadMethod="Pad" />""", result);
    }

    [TestMethod]
    public void AttributeOnItsOwnLine_TakesTheBlankLineWithIt()
    {
        string result = TokenFallback.StripFromXaml(
            """
            <Line
                X1="$(Slider1)"
                Y1="5" />
            """);

        Assert.AreEqual(
            """
            <Line
                Y1="5" />
            """,
            result);
    }

    [TestMethod]
    public void TokenInElementContent_RemovesOnlyTheTokenText()
    {
        // There is no attribute to drop here, so the container keeps its other children and simply
        // loses the content the gallery injected at runtime.
        string result = TokenFallback.StripFromXaml(
            """
            <CommandBar.SecondaryCommands>
                <AppBarButton Label="Keep" />$(MultipleButtonsSecondaryCommands)
            </CommandBar.SecondaryCommands>
            """);

        Assert.AreEqual(
            """
            <CommandBar.SecondaryCommands>
                <AppBarButton Label="Keep" />
            </CommandBar.SecondaryCommands>
            """,
            result);
        Assert.IsTrue(XamlFragment.IsWellFormed(result));
    }

    [TestMethod]
    public void TokenInsideAComment_IsRemovedWithoutDisturbingTheProse()
    {
        string result = TokenFallback.StripFromXaml("""<!-- The Layout used: $(SampleCodeLayout) --><Grid />""");

        Assert.AreEqual("<!-- The Layout used:  --><Grid />", result);
    }

    [TestMethod]
    public void AngleBracketInsideAnAttributeValue_DoesNotEndTheTagEarly()
    {
        // A naive scan for '>' would treat the comparison as the end of the tag and then mistake
        // the following attribute for element content, leaving the token in place.
        string result = TokenFallback.StripFromXaml("""<TextBlock Text="a > b" Width="$(Slider1)" />""");

        Assert.AreEqual("""<TextBlock Text="a > b" />""", result);
    }

    [TestMethod]
    public void CrLfSnippet_KeepsItsLineEndings()
    {
        string result = TokenFallback.StripFromXaml("<Grid>\r\n    <Line X1=\"$(Slider1)\" Y1=\"5\" />\r\n</Grid>");

        Assert.AreEqual("<Grid>\r\n    <Line Y1=\"5\" />\r\n</Grid>", result);
    }

    [TestMethod]
    public void ATokenStandingInForAWholeAttribute_IsRemovedWithoutBreakingTheTag()
    {
        Assert.AreEqual(
            """<Button Content="Standard XAML button" Click="Button_Click"/>""",
            TokenFallback.StripFromXaml("""<Button Content="Standard XAML button" Click="Button_Click" $(IsEnabled)/>"""));

        Assert.AreEqual(
            "<StackPanel>\r\n    <TextBlock Text=\"Hi\" />\r\n</StackPanel>",
            TokenFallback.StripFromXaml("<StackPanel $(Orientation)>\r\n    <TextBlock Text=\"Hi\" />\r\n</StackPanel>"));
    }

    [TestMethod]
    public void EveryRemoval_LeavesAWellFormedFragment()
    {
        string[] fragments =
        [
            """<Line X1="$(Slider1)" Y1="$(Slider2)" X2="$(Slider3)" Y2="$(Slider4)" />""",
            """<ToggleSwitch Header="Toggle work" IsOn="True$(isOff)" />""",
            """<Border BorderThickness="$(BorderThickness)" BorderBrush="$(BorderBrush)"><TextBlock Text="Hi" /></Border>""",
            """<Canvas><Rectangle Canvas.Left="$(Left)" Canvas.Top="$(Top)" Canvas.ZIndex="$(Z)" /></Canvas>""",
            """<Button Content="Click" $(IsEnabled)/>""",
            """<Button $(IsEnabled) $(Style) Content="Click" />""",
            """<ItemsRepeater $(Layout)><TextBlock Text="Hi" /></ItemsRepeater>""",
        ];

        foreach (string fragment in fragments)
        {
            string result = TokenFallback.StripFromXaml(fragment);

            Assert.IsFalse(TokenFallback.ContainsToken(result), $"Placeholder survived in: {result}");
            Assert.IsTrue(XamlFragment.IsWellFormed(result), $"Fragment stopped parsing: {result}");
        }
    }

    [TestMethod]
    public void TokenNames_AreReportedOnceInOrderOfAppearance()
    {
        List<string> names = TokenFallback.TokenNames("""<Line X1="$(Slider1)" Y1="$(Slider2)" X2="$(Slider1)" />""");

        CollectionAssert.AreEqual(new[] { "Slider1", "Slider2" }, names);
    }

    [TestMethod]
    public void ContainsToken_IgnoresTextThatMerelyLooksLikeOne()
    {
        Assert.IsFalse(TokenFallback.ContainsToken("""<TextBlock Text="Cost: $5 (approx)" />"""));
        Assert.IsFalse(TokenFallback.ContainsToken(null));
    }
}
