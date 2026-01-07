using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace WinUIGallery.SourceGenerator;

[Generator]
internal sealed partial class NavigationPageMapperGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var additionalFiles = context
            .AdditionalTextsProvider.Where(af => af.Path.EndsWith(".json", StringComparison.OrdinalIgnoreCase));

        var compilationProvider = context.CompilationProvider;

        var buildProperties = context.AnalyzerConfigOptionsProvider
            .Select((provider, _) =>
            {
                provider.GlobalOptions.TryGetValue("build_property.ProjectDir", out var projectDir);
                provider.GlobalOptions.TryGetValue("build_property.RootNamespace", out var rootNamespace);
                provider.GlobalOptions.TryGetValue($"build_property.NavigationMappingsNamespace", out var stringsNamespace);

                return (projectDir, rootNamespace, stringsNamespace);
            });

        var combined =
            additionalFiles
                .Combine(compilationProvider)
                .Combine(buildProperties);

        context.RegisterSourceOutput(combined, (ctx, data) =>
        {
            var ((files, compilation), (projectDir, rootNamespace, stringsNamespace)) = data;

            Execute(ctx, files, compilation, stringsNamespace, projectDir, rootNamespace);
        });
    }

    private void Execute(SourceProductionContext ctx, AdditionalText file, Compilation compilation, string @namespace, string projectDir, string rootNamespace)
    {
        var jsonText = file.GetText(ctx.CancellationToken)?.ToString();
        if (!TryDeserializeJson(jsonText ?? "", out var root))
        {
            ctx.ReportDiagnostic(Diagnostic.Create(
                new DiagnosticDescriptor(
                    "DEVGEN1001",
                    "NavigationPageMappings Skipped",
                    $"Skipping {file.Path} because it's not valid JSON for NavigationPageMappings.",
                    "SourceGenerator",
                    DiagnosticSeverity.Warning,
                    isEnabledByDefault: true),
                Location.None));
            return;
        }

        var pageNames = new List<string>();
        foreach (var group in root.Groups)
        {
            foreach (var item in group.Items)
            {
                if (!string.IsNullOrEmpty(item.UniqueId))
                    pageNames.Add(item.UniqueId);
            }
        }

        if (pageNames.Count == 0)
            return;

        var projectNamespace = @namespace;
        if (string.IsNullOrEmpty(projectNamespace))
        {
            projectNamespace = rootNamespace;

            if (string.IsNullOrEmpty(projectNamespace))
            {
                projectNamespace = compilation.AssemblyName ?? "WinUIGallery";
            }
        }

        // Generate source
        var sb = new StringBuilder();
        var sourceFiles = file.Path.Replace(projectDir, "");

        _ = sb.AppendFullHeader($"//{sourceFiles}");
        sb.AppendLine();
        sb.AppendLine("using System;");
        sb.AppendLine("using System.Collections.Generic;");
        sb.AppendLine();
        sb.AppendLine($"namespace {projectNamespace};");
        sb.AppendLine("public partial class NavigationPageMappings");
        sb.AppendLine("{");
        sb.AppendLine("    public static Dictionary<string, Type> PageDictionary { get; } = new Dictionary<string, Type>");
        sb.AppendLine("    {");

        foreach (var pageName in pageNames)
            sb.AppendLine($"        {{\"{pageName}\", typeof({pageName})}},");

        sb.AppendLine("    };");
        sb.AppendLine("}");

        var fileName = $"NavigationPageMappings_{System.IO.Path.GetFileNameWithoutExtension(file.Path)}.g.cs";
        ctx.AddSource(fileName, SourceText.From(sb.ToString(), Encoding.UTF8));
    }

    private static bool TryDeserializeJson(string jsonText, out Root? root)
    {
        root = null;
        if (string.IsNullOrWhiteSpace(jsonText))
            return false;

        try
        {
            root = JsonSerializer.Deserialize(jsonText, typeof(Root), RootContext.Default) as Root;
            return root != null && root.Groups != null;
        }
        catch (JsonException)
        {
            // Skip invalid JSON without failing
            return false;
        }
    }
}
