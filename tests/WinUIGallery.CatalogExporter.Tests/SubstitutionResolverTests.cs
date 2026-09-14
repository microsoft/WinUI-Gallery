// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WinUIGallery.CatalogExporter;

namespace WinUIGallery.CatalogExporter.Tests;

/// <summary>
/// Pins the rules the substitution resolver uses to turn $(Token) placeholders into the values the
/// gallery shows on first load. The bias throughout is that leaving a token unresolved is safe,
/// while publishing a value the gallery never shows is not - so most of these tests assert that an
/// ambiguous case is skipped rather than guessed.
/// </summary>
[TestClass]
public sealed class SubstitutionResolverTests
{
    private const string Header =
        """
        <Page xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
              xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
              xmlns:controls="using:WinUIGallery.Controls">
        """;

    private static (XElement Example, XElement Root) Parse(string inner)
    {
        XDocument document = XDocument.Parse(Header + inner + "</Page>");
        XElement root = document.Root!;
        XElement example = root.Descendants().First(e => e.Name.LocalName == "ControlExample");
        return (example, root);
    }

    private static Dictionary<string, string> Map(string inner)
    {
        (XElement example, XElement root) = Parse(inner);
        return SubstitutionResolver.BuildMap(example, root);
    }

    [TestMethod]
    public void LiteralValue_IsUsedVerbatimIncludingSurroundingWhitespace()
    {
        // Several snippets write DefaultLabelPosition="Right"$(IsSticky), relying on the value to
        // bring its own separating spaces. Trimming it would produce malformed XAML.
        Dictionary<string, string> map = Map(
            """
            <controls:ControlExample>
              <controls:ControlExample.Substitutions>
                <controls:ControlExampleSubstitution Key="IsSticky" Value=" IsSticky=&quot;True&quot; " />
              </controls:ControlExample.Substitutions>
            </controls:ControlExample>
            """);

        Assert.AreEqual(" IsSticky=\"True\" ", map["IsSticky"]);
    }

    [TestMethod]
    public void DisabledSubstitution_ResolvesToEmptyString()
    {
        // ControlExampleSubstitution.ValueAsString returns string.Empty when IsEnabled is false.
        Dictionary<string, string> map = Map(
            """
            <controls:ControlExample>
              <controls:ControlExample.Substitutions>
                <controls:ControlExampleSubstitution Key="Extra" IsEnabled="False" Value="Nope" />
              </controls:ControlExample.Substitutions>
            </controls:ControlExample>
            """);

        Assert.AreEqual(string.Empty, map["Extra"]);
    }

    [TestMethod]
    public void UnresolvableIsEnabled_LeavesTokenAlone()
    {
        // The condition binds to a property the markup never sets, so whether the gallery shows
        // this value is unknowable here. Emitting it anyway could invent markup the app omits.
        Dictionary<string, string> map = Map(
            """
            <controls:ControlExample>
              <controls:ControlExample.Substitutions>
                <controls:ControlExampleSubstitution Key="IsSticky" IsEnabled="{x:Bind Bar.IsSticky, Mode=OneWay}" Value=" IsSticky=&quot;True&quot; " />
              </controls:ControlExample.Substitutions>
            </controls:ControlExample>
            <CommandBar x:Name="Bar" />
            """);

        Assert.IsFalse(map.ContainsKey("IsSticky"));
    }

    [TestMethod]
    public void BoundProperty_ResolvesToTheControlsInitialAttribute()
    {
        Dictionary<string, string> map = Map(
            """
            <controls:ControlExample>
              <controls:ControlExample.Substitutions>
                <controls:ControlExampleSubstitution Key="Spacing" Value="{x:Bind Panel.Spacing, Mode=OneWay}" />
              </controls:ControlExample.Substitutions>
            </controls:ControlExample>
            <StackPanel x:Name="Panel" Spacing="12" />
            """);

        Assert.AreEqual("12", map["Spacing"]);
    }

    [TestMethod]
    public void BoundProperty_WithNoInitialValue_IsLeftUnresolved()
    {
        // The control relies on the framework default. Reproducing those defaults would mean
        // encoding WinUI's type metadata here, and getting one wrong publishes a false value.
        Dictionary<string, string> map = Map(
            """
            <controls:ControlExample>
              <controls:ControlExample.Substitutions>
                <controls:ControlExampleSubstitution Key="Spacing" Value="{x:Bind Panel.Spacing, Mode=OneWay}" />
              </controls:ControlExample.Substitutions>
            </controls:ControlExample>
            <StackPanel x:Name="Panel" />
            """);

        Assert.IsFalse(map.ContainsKey("Spacing"));
    }

    [TestMethod]
    public void NullableBoolPath_ReadsTheUnderlyingAttribute()
    {
        // x:Bind spells a nullable bool as IsChecked.Value, but the markup attribute is IsChecked.
        Dictionary<string, string> map = Map(
            """
            <controls:ControlExample>
              <controls:ControlExample.Substitutions>
                <controls:ControlExampleSubstitution Key="Checked" Value="{x:Bind Box.IsChecked.Value, Mode=OneWay}" />
              </controls:ControlExample.Substitutions>
            </controls:ControlExample>
            <CheckBox x:Name="Box" IsChecked="True" />
            """);

        Assert.AreEqual("True", map["Checked"]);
    }

    [TestMethod]
    public void ConverterFunction_IsLeftUnresolved()
    {
        Dictionary<string, string> map = Map(
            """
            <controls:ControlExample>
              <controls:ControlExample.Substitutions>
                <controls:ControlExampleSubstitution Key="OnTop" Value="{x:Bind BoolToLowerString(Toggle.IsOn), Mode=OneWay}" />
              </controls:ControlExample.Substitutions>
            </controls:ControlExample>
            <ToggleSwitch x:Name="Toggle" IsOn="True" />
            """);

        Assert.IsFalse(map.ContainsKey("OnTop"));
    }

    [TestMethod]
    public void CastSelectorBinding_ResolvesThroughTheCast()
    {
        Dictionary<string, string> map = Map(
            """
            <controls:ControlExample>
              <controls:ControlExample.Substitutions>
                <controls:ControlExampleSubstitution Key="Mode" Value="{x:Bind ((ComboBoxItem)Combo.SelectedItem).Content, Mode=OneWay}" />
              </controls:ControlExample.Substitutions>
            </controls:ControlExample>
            <ComboBox x:Name="Combo" SelectedIndex="1">
              <ComboBoxItem Content="Single" />
              <ComboBoxItem Content="Multiple" />
            </ComboBox>
            """);

        Assert.AreEqual("Multiple", map["Mode"]);
    }

    [TestMethod]
    public void SelectedItem_UsesTheItemMarkedIsSelected()
    {
        Dictionary<string, string> map = Map(
            """
            <controls:ControlExample>
              <controls:ControlExample.Substitutions>
                <controls:ControlExampleSubstitution Key="Tint" Value="{x:Bind Combo.SelectedItem, Mode=OneWay}" />
              </controls:ControlExample.Substitutions>
            </controls:ControlExample>
            <ComboBox x:Name="Combo">
              <ComboBoxItem>Red</ComboBoxItem>
              <ComboBoxItem IsSelected="True">Blue</ComboBoxItem>
            </ComboBox>
            """);

        Assert.AreEqual("Blue", map["Tint"]);
    }

    [TestMethod]
    public void SelectorWithNoDeclaredSelection_IsLeftUnresolved()
    {
        // Such a selector starts empty, so assuming the first item would publish a value the
        // gallery does not show.
        Dictionary<string, string> map = Map(
            """
            <controls:ControlExample>
              <controls:ControlExample.Substitutions>
                <controls:ControlExampleSubstitution Key="Tint" Value="{x:Bind Combo.SelectedItem, Mode=OneWay}" />
              </controls:ControlExample.Substitutions>
            </controls:ControlExample>
            <ComboBox x:Name="Combo">
              <ComboBoxItem Content="Red" />
            </ComboBox>
            """);

        Assert.IsFalse(map.ContainsKey("Tint"));
    }

    [TestMethod]
    public void ExampleWithoutSubstitutions_ProducesAnEmptyMap()
    {
        Assert.AreEqual(0, Map("<controls:ControlExample />").Count);
    }

    [TestMethod]
    public void Apply_ReplacesKnownTokensAndPreservesUnknownOnes()
    {
        Dictionary<string, string> map = new(StringComparer.Ordinal) { ["Known"] = "yes" };

        Assert.AreEqual(
            "a=\"yes\" b=\"$(Unknown)\"",
            SubstitutionResolver.Apply("a=\"$(Known)\" b=\"$(Unknown)\"", map));
    }

    [TestMethod]
    public void Apply_LeavesUnterminatedTokenTextUntouched()
    {
        Dictionary<string, string> map = new(StringComparer.Ordinal) { ["Known"] = "yes" };

        Assert.AreEqual("literal $(Known", SubstitutionResolver.Apply("literal $(Known", map));
    }

    [TestMethod]
    public void Apply_WithEmptyMap_ReturnsInputUnchanged()
    {
        Assert.AreEqual("$(Anything)", SubstitutionResolver.Apply("$(Anything)", new Dictionary<string, string>()));
    }
}
