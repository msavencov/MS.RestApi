using MS.RestApi.SourceGenerator.Client;
using MS.RestApi.SourceGenerator.Common;
using MS.RestApi.SourceGenerator.Pipe;
using MS.RestApi.SourceGenerator.Server;
using MS.RestApi.SourceGenerator.Utils;

namespace MS.RestApi.SourceGenerator;

internal class ApiGenGeneratorPipe : Pipeline<ApiGenContext>
{
    public ApiGenGeneratorPipe(ApiGenContext context) : base(context)
    {
        Add<AddApiRequests>();
            
        if (context.Config.GenerateControllers)
        {
            if (context.Config.UseMediatorHandlers)
            {
                Add<AddControllersWithMediator>();
            }
            else
            {
                Add<AddControllers>();
                Add<AddServices>();
            }
        }
            
        if (context.Config.GenerateClient)
        {
            Add<AddClientInterfaces>();
            Add<AddClientImplementation>();
            Add<AddClientDIExtensions>();
        }
            
        Add<AddSourceCode>();
    }
}