using Pipeliner.Contexts;
using Pipeliner.Middleware;
using Pipeliner.Pipeline;
using System.Linq;
namespace Pipeliner.Factories
{
    public class PipelineFactory
    {
        public class BuildPipeline<Tmiddleware,IpipelintContext>
            where IpipelintContext : IPipelineContext
            where Tmiddleware : IMiddleware<IpipelintContext>
        {
            private readonly IMiddlewareProvider<IpipelintContext> _ioCProvider;

            public BuildPipeline(IMiddlewareProvider<IpipelintContext> ioCProvider)
            {
                _ioCProvider = ioCProvider;
            }

            public IPipeline<IpipelintContext> Build(int pipelineType)
            {
                var middlewares = _ioCProvider.Middlewares
                    .Where(x => x.TargetPipelineType == pipelineType)
                    .OrderBy(x => x.ExecutionOrder);

                var rtn = new StandardPipeline<IpipelintContext>(pipelineType, middlewares);

                return rtn;
            }
        }
    }
}
