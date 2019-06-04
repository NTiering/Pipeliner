using Pipeliner.Contexts;

namespace Pipeliner.Middleware
{
    public interface IMiddlewareProvider<T>
        where T : IPipelineContext
    {
       IMiddleware<T>[] Middlewares { get; }
    }
}