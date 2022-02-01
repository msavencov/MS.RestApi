using MS.RestApi.Generators.Client;
using MS.RestApi.Generators.Common;
using MS.RestApi.Generators.Pipe;
using MS.RestApi.Generators.Server;
using MS.RestApi.Generators.Utils;

namespace MS.RestApi.Generators
{
    internal class ApiGenGeneratorPipe : Pipeline<ApiGenContext>
    {
        public ApiGenGeneratorPipe(ApiGenContext context) : base(context)
        {
            Add<AddApiRequests>();
            
            if (context.Config.GenerateControllers)
            {
                Add<AddControllers>();
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
}