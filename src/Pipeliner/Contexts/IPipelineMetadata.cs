namespace Pipeliner.Contexts
{
    public interface IPipelineMetadata
    {
        IMiddlewareExecutionReport[] ExecutionReports { get; set; }
    }
}