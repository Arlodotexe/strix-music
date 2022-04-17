using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MemberProxySourceGenerator;

[Generator]
public class MemberProxySourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var classes = context.SyntaxProvider
            .CreateSyntaxProvider(
                static (node, _) => node is FieldDeclarationSyntax { Parent: ClassDeclarationSyntax or RecordDeclarationSyntax, AttributeLists.Count: > 0 },
                static (context, _) => ((FieldDeclarationSyntax)context.Node).Declaration.Variables.Select(v => (IFieldSymbol)context.SemanticModel.GetDeclaredSymbol(v)!))
            .Where(static m => m is not null)
            .SelectMany(static (x, _) => x!);

        // Get all attributes + the original type symbol.
        var allAttributeData = classes.SelectMany(static (sym, _) => sym.GetAttributes().Select(x => (sym, x)));

        // Find and reconstruct actual attributes + the original type symbol.
        var attributes = allAttributeData
            .Select(static (x, _) => (x.Item2.TryReconstructAs<MemberProxyAttribute>(), x.Item1))
            .Where(static x => x.Item1 is not null)
            .Select(static (x, _) => (x.Item1!, x.Item2))
            .Collect();

        context.RegisterSourceOutput(attributes, (ctx, data) =>
        {
            foreach (var item in data)
            {
                if (item.Item1.MemberFlags == MemberTypes.None)
                    continue;

                var symbol = item.Item2;

                var containingMembers = symbol.Type.GetMembers();
                var inheritedMembers = symbol.Type.AllInterfaces.SelectMany(x => x.GetMembers());
                var allMembers = containingMembers.Concat(inheritedMembers);

                if (item.Item1.MemberFlags.HasFlag(MemberTypes.Events))
                {
                    GenerateEventProxy(symbol, allMembers, ctx);
                }
            }
        });

    }

    private static void GenerateEventProxy(IFieldSymbol symbol, IEnumerable<ISymbol> allMembers, SourceProductionContext ctx)
    {
        var events = allMembers.Where(x => x.Kind == SymbolKind.Event && x is IEventSymbol)
                               .Select(x => (IEventSymbol)x);

        var eventDeclarations = GenerateEventDeclarations(events);
        var eventHandlerMethods = GenerateEventHandlerMethods(events);
        var eventAttachDeclarations = GenerateEventAttachDeclarations(symbol.Name.TrimStart('_'), events);
        var eventDetachDeclarations = GenerateEventDetachDeclarations(symbol.Name.TrimStart('_'), events);

        var body = $@"private void AttachEvents({symbol.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)} {symbol.Name.TrimStart('_')})
        {{
             {string.Join(Environment.NewLine, eventAttachDeclarations)}
        }}

        private void DetachEvents({symbol.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)} {symbol.Name.TrimStart('_')})
        {{
             {string.Join(Environment.NewLine, eventDetachDeclarations)}
        }}

        {string.Join("\n\n", eventDeclarations)}

        {string.Join("\n\n", eventHandlerMethods)}";

        var partialClass = GeneratePartialClass(symbol.ContainingType, body);

        ctx.AddSource($"{symbol.ContainingType.Name}.Events.{symbol.Type.Name}", partialClass);

        static IEnumerable<string> GenerateEventAttachDeclarations(string instanceName, IEnumerable<IEventSymbol> events)
        {
            foreach (var ev in events)
                yield return $"{instanceName}.{ev.Name} += On{ev.Name};";
        }
        
        static IEnumerable<string> GenerateEventDetachDeclarations(string instanceName, IEnumerable<IEventSymbol> events)
        {
            foreach (var ev in events)
                yield return $"{instanceName}.{ev.Name} -= On{ev.Name};";
        }

        static IEnumerable<string> GenerateEventDeclarations(IEnumerable<IEventSymbol> events)
        {
            foreach (var ev in events)
                yield return $@"/// <inheritdoc/>
        public event {ev.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}? {ev.Name};";
        }

        static IEnumerable<string> GenerateEventHandlerMethods(IEnumerable<IEventSymbol> events)
        {
            foreach (var ev in events)
            {
                var parameters = ((INamedTypeSymbol)ev.Type).DelegateInvokeMethod?.Parameters;
                if (parameters is null)
                    continue;

                yield return
            @$"private void On{ev.Name}({string.Join(", ", parameters?.Select(x => $"{x.Type} {x.Name}"))})
        {{
                {ev.Name}?.Invoke({string.Join(", ", parameters?.Select(x => x.Name))});
        }}";
            }
        }
    }

    private static string GeneratePartialClass(INamedTypeSymbol symbol, string body)
        => @$"
namespace {symbol.ContainingNamespace}
{{
    public partial class {symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}
    {{
        {body}
    }}
}}
";
}

