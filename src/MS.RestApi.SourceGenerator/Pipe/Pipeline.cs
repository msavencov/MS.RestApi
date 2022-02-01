using System.Collections.Concurrent;

namespace MS.RestApi.Generators.Pipe
{
    internal class Pipeline<TContext>
    {
        protected readonly TContext Context;

        protected Pipeline(TContext context)
        {
            Context = context;
        }

        private readonly BlockingCollection<IMiddleware<TContext>> _queue = new();

        public void Add<TMiddleware>() where TMiddleware : IMiddleware<TContext>, new() => Add(new TMiddleware());
        public void Add(IMiddleware<TContext> middleware)
        {
            _queue.Add(middleware);
        }

        public void Run()
        {
            _queue.CompleteAdding();
            
            foreach (var middleware in _queue.GetConsumingEnumerable())
            {
                middleware.Execute(Context);
            }
        }
    }
}