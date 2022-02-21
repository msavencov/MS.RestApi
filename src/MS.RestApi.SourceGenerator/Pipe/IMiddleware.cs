namespace MS.RestApi.SourceGenerator.Pipe
{
    internal interface IMiddleware<TContext>
    {
        public void Execute(TContext context);
    }
}