namespace Lightflow.Steps
{
    using System;
    using System.Threading.Tasks;
    using Lightflow.Core;

    public class HandlerLightflowStep<TContext, TInput, TOutput> : ILightflowStep<TContext, TInput, TOutput>
    {
        private readonly LightflowHandler<TContext, TInput, TOutput> handler;

        public HandlerLightflowStep(LightflowHandler<TContext, TInput, TOutput> handler)
        {
            this.handler = handler;
        }

        public Task Invoke(TContext context, TInput input, Func<TOutput, Task> next)
        {
            return this.handler(context, input, next);
        }
    }
}
