namespace Lightflow
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading.Tasks;
    using Lightflow.Contexts;
    using Lightflow.Core;

    public class Lightflow<TInput, TContext> where TContext : ILightflowContext, new()
    {
        private static readonly MethodInfo ExecuteStepMethodInfo =
            typeof(Lightflow<TInput, TContext>).GetMethod(nameof(ExecuteStep), BindingFlags.NonPublic | BindingFlags.Instance);

        private readonly List<LightflowStepBaseInvoker> invokers;
        private readonly TContext context = new TContext();

        private readonly Dictionary<LightflowStepBaseInvoker, Delegate> compilationCache =
            new Dictionary<LightflowStepBaseInvoker, Delegate>();

        public Lightflow(List<LightflowStepBaseInvoker> invokers)
        {
            this.invokers = invokers;

            this.CompileInvokers();
        }

        private void CompileInvokers()
        {
            var invoker = this.invokers.GetEnumerator();

            while (invoker.MoveNext())
            {
                this.compilationCache.Add(invoker.Current, this.CompileInvoker(invoker));
            }
        }

        public async Task<TContext> Execute(TInput input)
        {
            await this.ExecuteStep(this.invokers.GetEnumerator(), input);

            return this.context;
        }

        private Task ExecuteStep(List<LightflowStepBaseInvoker>.Enumerator invoker, object input)
        {
            if (!invoker.MoveNext())
                return Task.CompletedTask;

            var handler = this.compilationCache[invoker.Current];

            return invoker.Current.InvokeStep(this.context, input, handler);
        }

        private Delegate CompileInvoker(IEnumerator<LightflowStepBaseInvoker> invoker)
        {
            var outParam = Expression.Parameter(invoker.Current.OutputType, "output");

            var lambda = Expression.Lambda(
                Expression.Call(
                    Expression.Constant(this),
                    ExecuteStepMethodInfo,
                    Expression.Constant(invoker),
                    Expression.Convert(
                        outParam,
                        typeof(object))),
                outParam);

            return lambda.Compile();
        }
    }
}
