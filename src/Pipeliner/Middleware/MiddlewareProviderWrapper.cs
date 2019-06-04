using Pipeliner.Contexts;
using System;

namespace Pipeliner.Middleware
{
    public class MiddlewareProviderWrapper<T> : IMiddlewareProvider<T>
        where T : IPipelineContext
    {
        private Func<IMiddleware<T>[]> _getMiddlewares;

        public IMiddleware<T>[] Middlewares => _getMiddlewares();

        public MiddlewareProviderWrapper(Func<IMiddleware<T>[]> getMiddlewares)
        {
            _getMiddlewares = getMiddlewares;
        }        
    }
}
