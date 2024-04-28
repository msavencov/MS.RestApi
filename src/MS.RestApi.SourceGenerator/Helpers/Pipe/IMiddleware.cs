namespace MS.RestApi.SourceGenerator.Helpers.Pipe;

internal interface IMiddleware<TContext>
{
    public void Execute(TContext context);
}