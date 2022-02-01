namespace MS.RestApi.Generators.Pipe
{
    internal interface IMiddleware<TContext>
    {
        public void Execute(TContext context);
    }
}