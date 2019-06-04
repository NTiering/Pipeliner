using System;
using System.Diagnostics;

namespace Pipeliner.Contexts
{
    internal class MiddlewareExecutionReport : IMiddlewareExecutionReport
    {
        private readonly Stopwatch _stopwatch;      
        public Exception ExceptionThrown { get; set; }
        public int TargetPipelineType { get; set; }
        public int ExecutionOrder { get; set; }
        public bool IsEnabled { get; set; }
        public long DurationInMs => _stopwatch.ElapsedMilliseconds;

        public MiddlewareExecutionReport()
        {
            _stopwatch = Stopwatch.StartNew();
        }

        public void Stop()
        {
            _stopwatch.Stop();
        }

    }
}