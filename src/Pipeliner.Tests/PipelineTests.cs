using Moq;
using NUnit.Framework;
using Pipeliner.Contexts;
using Pipeliner.Factories;
using Pipeliner.Middleware;
using System;
using System.Linq;
using System.Threading;

namespace Pipeliner.Core
{
    public class Pipeline
    {
        [SetUp]
        public void Setup()
        {
            // setup middlewares 
            TestMiddlewareOne = new TestMiddlewareOne();
            TestMiddlewareTwo = new TestMiddlewareTwo();
            TestMiddlewareThree = new TestMiddlewareThree();

            // setup IOC Provider
            var ioCProvider = new Mock<IMiddlewareProvider<TestPipelineContext>>();
            ioCProvider.Setup(x => x.Middlewares).Returns(new IMiddleware<TestPipelineContext>[] {
                    TestMiddlewareOne,
                    TestMiddlewareTwo,
                    TestMiddlewareThree});
            IoCProvider = ioCProvider.Object;
        }

        [Test]
        public void CanBeBuilt()
        {
            // Arrange 
            var pipeLineType = 100;

            // Act 
            var pipeline = new PipelineFactory.BuildPipeline<TestMiddlewareBase, TestPipelineContext>(IoCProvider)
                .Build(pipeLineType);

            // Assert 
            Assert.NotNull(pipeline);
        }

        [Test]
        public void CanBeExecuted()
        {
            // Arrange 
            var pipeLineType = 100;
            var pipeline = new PipelineFactory.BuildPipeline<TestMiddlewareBase, TestPipelineContext>(IoCProvider)
                .Build(pipeLineType);
            var context = new TestPipelineContext();

            // Act 
            pipeline.Execute(context);

            // Assert 
            Assert.NotNull(pipeline);
        }

        [Test]
        public void EmptyCanBeExecuted()
        {
            // Arrange 
            var pipeLineType = 505;
            var pipeline = new PipelineFactory.BuildPipeline<TestMiddlewareBase, TestPipelineContext>(IoCProvider)
                .Build(pipeLineType);
            var context = new TestPipelineContext();

            // Act 
            pipeline.Execute(context);

            // Assert 
            Assert.NotNull(pipeline);
        }

        [Test]
        public void MakesExecutionMetaDataForEachMiddleware()
        {
            // Arrange 
            var pipeLineType = 100;
            var pipeline = new PipelineFactory.BuildPipeline<TestMiddlewareBase, TestPipelineContext>(IoCProvider)
                .Build(pipeLineType);
            var context = new TestPipelineContext();

            // Act 
            pipeline.Execute(context);
            var count = context.MetaData.ExecutionReports.Count();

            // Assert 
            Assert.AreEqual(2, count);
        }

        [Test]
        public void ExecutionMetaDataIsPopulated()
        {
            // Arrange 
            var pipeLineType = 100;
            var pipeline = new PipelineFactory.BuildPipeline<TestMiddlewareBase, TestPipelineContext>(IoCProvider)
                .Build(pipeLineType);
            var context = new TestPipelineContext();
            var expected = new TestMiddlewareOne();

            // Act 
            pipeline.Execute(context);
            var executionReport = context.MetaData.ExecutionReports.First();

            // Assert 
            Assert.AreEqual(executionReport.ExecutionOrder, expected.ExecutionOrder, "ExecutionOrder");
            Assert.AreEqual(executionReport.TargetPipelineType, expected.TargetPipelineType, "TargetPipelineType");
            Assert.AreEqual(executionReport.ExceptionThrown, null, "ExceptionThrown");
            Assert.True(executionReport.DurationInMs >= 100, "DurationInMs");
        }

        [Test]
        public void ExceptionsAreCaught()
        {
            // Arrange 
            var pipeLineType = 100;
            var pipeline = new PipelineFactory.BuildPipeline<TestMiddlewareBase, TestPipelineContext>(IoCProvider)
                .Build(pipeLineType);
            var context = new TestPipelineContext();
            TestMiddlewareOne.ThrowException = true;

            // Act 
            pipeline.Execute(context);
            var executionReport = context.MetaData.ExecutionReports.First();

            // Assert 
            Assert.IsTrue(executionReport.ExceptionThrown as InvalidOperationException != null);
        }

        [Test]
        public void ExceptionsStopExecution()
        {
            // Arrange 
            var pipeLineType = 100;
            var pipeline = new PipelineFactory.BuildPipeline<TestMiddlewareBase, TestPipelineContext>(IoCProvider)
                .Build(pipeLineType);
            var context = new TestPipelineContext();
            TestMiddlewareOne.ThrowException = true;

            // Act 
            pipeline.Execute(context);
            var count = context.MetaData.ExecutionReports.Count();

            // Assert 
            Assert.AreEqual(1, count);
        }
        [Test]
        public void ContextIsPassedToMiddleware()
        {
            // Arrange 
            var pipeLineType = 100;
            var pipeline = new PipelineFactory.BuildPipeline<TestMiddlewareBase, TestPipelineContext>(IoCProvider)
                .Build(pipeLineType);
            var context = new TestPipelineContext();

            // Act 
            pipeline.Execute(context);

            // Assert 
            Assert.AreEqual(context, TestMiddlewareOne.ExecutedContext);
            Assert.AreEqual(context, TestMiddlewareTwo.ExecutedContext);
        }




        //--------------------------
        //     HELPERS
        //--------------------------        

        internal IMiddlewareProvider<TestPipelineContext> IoCProvider { get; private set; }
        internal TestMiddlewareOne TestMiddlewareOne { get; private set; }
        internal TestMiddlewareTwo TestMiddlewareTwo { get; private set; }
        internal TestMiddlewareThree TestMiddlewareThree { get; private set; }
    }

    public abstract class TestMiddlewareBase : IMiddleware<TestPipelineContext>
    {
        public int TargetPipelineType { get; set; }

        public bool IsEnabled { get; set; }

        public int ExecutionOrder { get; set; }
        public bool ThrowException { get; set; }
        public TestPipelineContext ExecutedContext { get; set; }
        public void Execute(TestPipelineContext context)
        {
            ExecutedContext = context;
            Thread.Sleep(100);
            if (ThrowException)
            {
                throw new InvalidOperationException();
            }
        }
    }

    public class TestMiddlewareOne : TestMiddlewareBase
    {

        public TestMiddlewareOne()
        {
            TargetPipelineType = 100;
            IsEnabled = true;
            ExecutionOrder = 100;
        }

    }

    public class TestMiddlewareTwo : TestMiddlewareBase
    {
        public TestMiddlewareTwo()
        {
            TargetPipelineType = 100;
            IsEnabled = true;
            ExecutionOrder = 200;
        }
    }
    public class TestMiddlewareThree : TestMiddlewareBase
    {
        public TestMiddlewareThree()
        {
            TargetPipelineType = 205;
            IsEnabled = true;
            ExecutionOrder = 300;
        }
    }



    public class TestPipelineContext : IPipelineContext
    {
        public IPipelineMetadata MetaData { get; set; }
    }
}