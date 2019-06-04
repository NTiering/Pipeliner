using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Pipeliner.Contexts
{
public interface IMiddlewareExecutionReport
    {
        Exception ExceptionThrown { get; }
        long DurationInMs { get; }
        int TargetPipelineType { get; set; }
        int ExecutionOrder { get; set; }
        bool IsEnabled { get; set; }
    }
}