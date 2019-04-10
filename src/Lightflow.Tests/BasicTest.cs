namespace Lightflow.Tests
{
    using System.Threading.Tasks;
    using Lightflow.Builder;
    using Lightflow.Contexts;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class BasicTest
    {
        [TestInitialize]
        public void Setup()
        {
        }

        [TestMethod]
        public async Task HandlerTest()
        {
            // Arrange
            const int flowInput = 5;

            var builder = new LightflowBuilder<int, OutputContext<int>>();

            var flow = builder
                .Use<int>((context, input, next) => next(input * 2))
                .Use<int>((context, input, next) => next(input * 2))
                .Use<int>((context, input, next) =>
                {
                    context.Output = input;
                    return Task.CompletedTask; //TODO: An overload with a sync signature
                })
                .Build();

            // Act
            var result = await flow.Execute(flowInput);

            // Assert
            Assert.AreEqual(20, result.Output);
        }
    }
}
