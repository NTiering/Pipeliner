using Pipeliner.Contexts;

namespace Pipeliner.Pipeline
{
    public interface IPipeline<T>
        where T : IPipelineContext
    {
        void Execute(T context);
    }
}
