using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using MS.RestApi.SourceGenerator.Descriptors;
using MS.RestApi.SourceGenerator.Exceptions;
using MS.RestApi.SourceGenerator.Helpers;
using MS.RestApi.SourceGenerator.Helpers.Pipe;
using MS.RestApi.SourceGenerator.Tests.Helpers;

namespace MS.RestApi.SourceGenerator.Generators.Server;

internal class AddControllers : IMiddleware<ApiGenContext>
{
    public void Execute(ApiGenContext context)
    {
        var symbols = context.Symbols;
        var options = context.Options;
        var conventions = options.ServerConventions;
        var useMediator = options.GenerateControllers == GenerateControllers.WithMediator;
        
        if (useMediator && symbols.IMediator is null)
        {
            throw ApiGenException.RequiredAssemblyReference("Mediator");
        }

        foreach (var (serviceName, requests) in context.Services)
        {
            var builder = new StringBuilder();
            var writer = new IndentedWriter(builder, 0);

            var service = conventions.ServiceInterface(serviceName);
            var serviceFullname = useMediator ? symbols.IMediator?.ToDisplayString() : $"{service.Namespace}.{service.Name}";
            var controller = conventions.ControllerName(serviceName);
            var methodAttribute = symbols.HttpPostAttribute.ToDisplayString();
            var routeAttribute = symbols.RouteAttribute.ToDisplayString();
            var fromAttribute = symbols.FromBodyAttribute.ToDisplayString();
            var fromRouteAttribute = symbols.FromRouteAttribute.ToDisplayString();
            var customModelBuilders = new List<Action<IndentedWriter>>();
            
            writer.WriteHeaderLines();
            writer.WriteLine($"namespace {controller.Namespace}");
            writer.WriteBlock(ns =>
            {
                ns.WriteLine($"[{symbols.ApiControllerAttribute.ToDisplayString()}]");
                ns.WriteLine($"public class {controller.Name}({serviceFullname} service) : {symbols.ControllerBase.ToDisplayString()}");
                ns.WriteBlock(cb =>
                {
                    foreach (var action in requests)
                    {
                        var request = action.Request;
                        var requestType = request.ToDisplayString();
                        var serviceMethodName = useMediator ? "Send" : request.Name;
                        var routeArguments = ParseRouteArguments(action);
                        var routeArgumentsList = routeArguments.Select(t => $"[{fromRouteAttribute}] {t.Type.ToDisplayString()} {t.Name}, ").Join();
                        
                        cb.WriteLine($"/// <inheritdoc cref=\"{requestType}\"/>");
                        cb.WriteLine($"[{methodAttribute}, {routeAttribute}(\"{options.GetRoute(action.Endpoint)}\")]");
                        cb.WriteLine($"public {action.ReturnType} {request.Name}Generated({routeArgumentsList}[{fromAttribute}] {requestType} model, {symbols.CancellationToken.ToDisplayString()} token)");
                        cb.WriteBlock(mb =>
                        {
                            if (request.IsRecord)
                            {
                                foreach (var routeArgument in routeArguments)
                                {
                                    mb.WriteLine($"model = model with {{ {routeArgument.Name} = {routeArgument.Name} }};");
                                }
                            }
                            else
                            {
                                foreach (var routeArgument in routeArguments)
                                {
                                    mb.WriteLine($"model.{routeArgument.Name} = {routeArgument.Name};");
                                }
                            }
                            
                            mb.WriteLine($"return service.{serviceMethodName}(model, token);");
                        });
                    }
                });

                foreach (var modelBuilder in customModelBuilders)
                {
                    modelBuilder(ns);
                }
            });
            
            context.Result.Add(new ApiGenSourceCode
            {
                Name = $"{controller.Namespace}.{controller.Name}.g.cs",
                Source = builder.ToString()
            });
        }
    }

    private string? BuildCustomRequestType(ApiRequestDescriptor action, KnownSymbols symbols, ICollection<Action<IndentedWriter>> additionalModelBuilders)
    {
        var request = action.Request;
        var routeArguments = ParseRouteArguments(action);

        if (routeArguments.Count == 0)
        {
            return null;
        }
        var classOrRecord = request.IsRecord ? "record" : "class";
        var name = request.Name + "Generated";
        var modelWriter = new Action<IndentedWriter>(writer =>
        {
            writer.WriteLine($"public {classOrRecord} {name} : {request.ToDisplayString()}");
            writer.WriteBlock(cw =>
            {
                foreach (var propertySymbol in routeArguments)
                {
                    var modifier = propertySymbol.IsVirtual ? SyntaxKind.OverrideKeyword : SyntaxKind.NewKeyword;
                    var propertyDeclaration = SymbolHelper.CreatePropertyDeclaration(propertySymbol)
                                                          .AddModifiers(SyntaxFactory.Token(modifier));
                    var property = propertyDeclaration.NormalizeWhitespace()
                                                      .ToFullString();
                    
                    cw.WriteLine($"[{symbols.FromRouteAttribute.ToDisplayString()}]");
                    cw.WriteLine(property);
                }
            });
        });
        additionalModelBuilders.Add(modelWriter);
        
        return name;
    }

    private static readonly Regex ParseRouteArgumentsRegex = new Regex(@"\{(?<param>\w+)\}", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    
    private List<IPropertySymbol> ParseRouteArguments(ApiRequestDescriptor action)
    {
        var matches = ParseRouteArgumentsRegex.Matches(action.Endpoint).OfType<Match>();
        var parameters = from match in matches.Select(t => t.Groups["param"].Value)
                         from property in action.Request.GetMembers().OfType<IPropertySymbol>()
                         where match == property.Name
                         select property;
                         
        
        return parameters.ToList();
    }
}