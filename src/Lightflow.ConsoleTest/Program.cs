using System;

namespace Lightflow.ConsoleTest
{
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Lightflow.Builder;
    using Lightflow.Contexts;
    using Lightflow.Steps;

    class Program
    {
        static async Task Main(string[] args)
        {
            var flows = new[]
            {
                CreateFlow(),
                CreateFlow(),
                CreateFlow(),
                CreateFlow(),
                CreateFlow()
            };

            while (true)
            {
                var input = Console.ReadLine();

                if (!int.TryParse(input, out var times))
                    continue;

                var sw = Stopwatch.StartNew();

                Parallel.ForEach(flows, async flow =>
                {
                    for (int i = 0; i < times; i++)
                    {
                        var obj = new TransitObject();
                        obj.Number = i;
                        await flow.Execute(obj);
                    }
                });

                sw.Stop();

                Console.WriteLine("Elapsed: {0}", sw.ElapsedMilliseconds);
            }
        }

        private static Lightflow<TransitObject, Ctx> CreateFlow()
        {
            var builder = new LightflowBuilder<TransitObject, Ctx>();

            return builder
                .Use(new LightflowBufferStep<TransitObject>(10))
                .Use<TransitObject>(async (context, input, next) =>
                {
                    input.Text = Guid.NewGuid().ToString();

                    await Task.Delay(1); //.ConfigureAwait(false);

                    await next(input); //.ConfigureAwait(false);
                })
                .Use<TransitObject>(async (context, input, next) =>
                {
                    input.Text = Guid.NewGuid().ToString();

                    await Task.Delay(1); //.ConfigureAwait(false);

                    await next(input); //.ConfigureAwait(false);
                })
                .Use(new LightflowBufferStep<TransitObject>(10))
                .Use<TransitObject>(async (context, input, next) =>
                {
                    input.Text = Guid.NewGuid().ToString();

                    context.Output = "teste";

                    await Task.Delay(1); //.ConfigureAwait(false);

                    Console.WriteLine("Number: {0}", input.Number);

                    await next(input); //.ConfigureAwait(false);
                })
                .Build();
        }

        public class TransitObject
        {
            public int Number { get; set; }

            public string Text { get; set; }
        }

        public class Ctx : ILightflowContext
        {
            public string Output { get; set; }
        }
    }
}
