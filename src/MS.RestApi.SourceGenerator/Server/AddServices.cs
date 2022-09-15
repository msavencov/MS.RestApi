using System.Collections.Generic;
using System.Text;
using MS.RestApi.SourceGenerator.Builder;
using MS.RestApi.SourceGenerator.Common;
using MS.RestApi.SourceGenerator.Extensions;
using MS.RestApi.SourceGenerator.Pipe;
using MS.RestApi.SourceGenerator.Utils;

namespace MS.RestApi.SourceGenerator.Server;

internal class AddServices : IMiddleware<ApiGenContext>
{
    public void Execute(ApiGenContext context)
    {
        if (context.Requests.Count == 0)
        {
            throw new ApiGenException(10, "No any request found.");
        }

        foreach (var item in context.Requests.AsServices())
        {
            var service = new ApiGenSourceCode
            {
                Name = $"{context.Config.ServerServiceNamespace}.{item.ServiceName}.cs",
                Source = BuildServiceSourceCode(context, item.ServiceName, item.Operations),
            };
            context.SourceCode.Add(service);
        }
    }
        
        
    private string BuildServiceSourceCode(ApiGenContext context, string serviceName, IEnumerable<ApiGenRequest> actions)
    {
        var builder = new StringBuilder();
        var writer = new IndentedWriter(builder, 0);
            
        var config = context.Config;
        var symbol = context.KnownSymbols;

        writer.WriteLine($"namespace {context.Config.ServerServiceNamespace}");
        writer.WriteBlock(nsw =>
        {
            nsw.WriteLine($"public interface {ApiGenSymbols.GetServiceInterfaceName(serviceName)} : {symbol.IApiService.FullName()}");
            nsw.WriteBlock(cw =>
            {
                foreach (var request in actions)
                {
                    var returnResult = request.GetResponseTypeName(context);
                    var name = request.GetMethodName();
                    var model = request.Request.FullName();
                    var ct = symbol.CancellationToken.FullName();
                    var method = $"{returnResult} {name}({model} model, {ct} ct = default);";

                    cw.WriteLine(method);
                }
            });
        });
            
        return builder.ToString();
    }
}