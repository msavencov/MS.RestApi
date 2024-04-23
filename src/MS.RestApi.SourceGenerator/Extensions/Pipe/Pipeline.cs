using System.Collections.Generic;

namespace MS.RestApi.SourceGenerator.Extensions.Pipe;

internal class Pipeline<TContext>
{
    private readonly TContext _context;

    protected Pipeline(TContext context)
    {
        _context = context;
    }

    private readonly HashSet<IMiddleware<TContext>> _middlewares = new();

    public void Add<TMiddleware>() where TMiddleware : IMiddleware<TContext>, new() => Add(new TMiddleware());
    public void Add(IMiddleware<TContext> middleware)
    {
        _middlewares.Add(middleware);
    }

    public void Run()
    {
        foreach (var middleware in _middlewares)
        {
            middleware.Execute(_context);
        }
    }
}