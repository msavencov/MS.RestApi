namespace MS.RestApi.SourceGenerator.Extensions.Pipe;

internal interface IMiddleware<TContext>
{
    public void Execute(TContext context);
}