using System;
using System.Collections.Generic;
using System.Linq;
using Pipeliner.Contexts;
using Pipeliner.Middleware;

namespace Pipeliner.Pipeline
{
    internal class StandardPipeline<T> : IPipeline<T>
         where T : IPipelineContext

    {
        private readonly int _pipelineType;
        private readonly IOrderedEnumerable<IMiddleware<T>> _middlewares;

        public StandardPipeline(int pipelineType, IOrderedEnumerable<IMiddleware<T>> middlewares)
        {
            _pipelineType = pipelineType;
            _middlewares = middlewares;
        }

        public void Execute(T context)
        {
            var reports = new List<IMiddlewareExecutionReport>();
            context.MetaData = context.MetaData ?? new PipelineMetadata();
            foreach (var middleware in _middlewares)
            {
                if (AnyHaveErrored(context.MetaData.ExecutionReports) || middleware.IsEnabled == false)
                {
                    continue;
                }

                reports.Add(Execute(middleware, context));
                context.MetaData.ExecutionReports = reports.AsReadOnly().ToArray();
            }
            
        }

        private static IMiddlewareExecutionReport Execute(IMiddleware<T> middleware, T context)
        {
            var rtn = new MiddlewareExecutionReport();

            try
            {
                rtn.TargetPipelineType = middleware.TargetPipelineType;
                rtn.ExecutionOrder = middleware.ExecutionOrder;
                rtn.IsEnabled = middleware.IsEnabled;
                if (rtn.IsEnabled)
                {
                    middleware.Execute(context);
                }
               
            }
            catch (Exception ex)
            {
                rtn.ExceptionThrown = ex;
            }


            rtn.Stop();
            return rtn;
        }

        private static bool AnyHaveErrored(IMiddlewareExecutionReport[] executionReports)
        {
            if (executionReports == null) return false;
            if (executionReports.Count() == 0) return false;
            if (executionReports.Any(x => x.ExceptionThrown != null)) return true;
            return false;
        }

        
    }
}