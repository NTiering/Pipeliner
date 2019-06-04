using Pipeliner.Contexts;

namespace Pipeliner.Middleware
{
    public interface IMiddleware<T>
        where T : IPipelineContext
    {
        int TargetPipelineType { get; }
        bool IsEnabled { get; }
        int ExecutionOrder { get; }
        void Execute(T context);
    }
}
