using System.Linq;

namespace Pipeliner.Contexts
{
    internal class PipelineMetadata : IPipelineMetadata
    {
        public IMiddlewareExecutionReport[] ExecutionReports { get; set; }
        public PipelineMetadata()
        {
            ExecutionReports = Enumerable.Empty<IMiddlewareExecutionReport>().ToArray();
        }
    }
}