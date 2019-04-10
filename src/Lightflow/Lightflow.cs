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

        private readonly List<LightflowStepInfo> infos;
        private readonly TContext context = new TContext();

        private readonly Dictionary<LightflowStepInfo, Delegate> compilationCache =
            new Dictionary<LightflowStepInfo, Delegate>();

        public Lightflow(List<LightflowStepInfo> steps)
        {
            this.infos = steps;

            this.CompileHandlers();
        }

        private void CompileHandlers()
        {
            var info = this.infos.GetEnumerator();

            while (info.MoveNext())
            {
                this.compilationCache.Add(info.Current, this.CompileHandler(info));
            }
        }

        public async Task<TContext> Execute(TInput input)
        {
            await this.ExecuteStep(this.infos.GetEnumerator(), input);

            return this.context;
        }

        private Task ExecuteStep(List<LightflowStepInfo>.Enumerator info, object input)
        {
            if (!info.MoveNext())
                return Task.CompletedTask;

            var handler = this.compilationCache[info.Current];

            return info.Current.InvokeStep(this.context, input, handler);
        }

        private Delegate CompileHandler(IEnumerator<LightflowStepInfo> info)
        {
            var outParam = Expression.Parameter(info.Current.OutputType, "output");

            var lambda = Expression.Lambda(
                Expression.Call(
                    Expression.Constant(this),
                    ExecuteStepMethodInfo,
                    Expression.Constant(info),
                    Expression.Convert(
                        outParam,
                        typeof(object))),
                outParam);

            return lambda.Compile();
        }
    }
}
