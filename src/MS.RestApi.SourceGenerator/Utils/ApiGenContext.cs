using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace MS.RestApi.SourceGenerator.Utils
{
    internal class ApiGenContext
    {
        public ApiGenConfig Config { get; private init; }
        public ApiGenSymbols KnownSymbols { get; private init; }
        public GeneratorExecutionContext Context { get; private init; }
        public ApiGenRequestCollection Requests { get; set; } = new();
        public HashSet<ApiGenSourceCode> SourceCode { get; set; } = new();

        public static ApiGenContext Create(GeneratorExecutionContext context)
        {
            return new ApiGenContext
            {
                Context = context,
                Config = ApiGenConfig.Init(context),
                KnownSymbols = ApiGenSymbols.Init(context)
            }.Validate();
        }

        private ApiGenContext Validate()
        {
            if (Config.AssemblyToScan is null or { Length: 0 })
            {
                throw new ApiGenException(10, $"Nothing to scan. Add at least one assembly to scan request from using '{nameof(Config.AssemblyToScan)}'.")
                {
                    Category = "Configuration"
                };
            }
            
            if (Config.GenerateControllers && KnownSymbols.ControllerBase == null)
            {
                throw new ApiGenTypeNotFoundException(10, nameof(KnownSymbols.ControllerBase));
            }

            return this;
        }
    }
}