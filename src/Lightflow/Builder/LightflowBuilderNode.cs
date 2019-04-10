namespace Lightflow.Builder
{
    using System;
    using System.Collections.Generic;
    using Lightflow.Contexts;
    using Lightflow.Core;
    using Lightflow.Steps;

    public class LightflowBuilderNode<TLightflowInput, TStepInput, TContext> where TContext : ILightflowContext, new()
    {
        private readonly List<LightflowStepBaseInvoker> steps;

        internal LightflowBuilderNode(List<LightflowStepBaseInvoker> filters)
        {
            this.steps = filters;
        }

        public LightflowBuilderNode<TLightflowInput, TOutput, TContext> Use<TOutput>(LightflowHandler<TContext, TStepInput, TOutput> handler)
        {
            this.steps.Add(new LightflowStepInstanceInvoker(
                new HandlerLightflowStep<TContext, TStepInput, TOutput>(handler),
                typeof(TContext)));

            return new LightflowBuilderNode<TLightflowInput, TOutput, TContext>(this.steps);
        }

        public LightflowBuilderNode<TLightflowInput, TOutput, TContext> Use<TOutput>(ILightflowStep<TContext, TStepInput, TOutput> step)
        {
            this.steps.Add(new LightflowStepInstanceInvoker(step, typeof(TContext)));

            return new LightflowBuilderNode<TLightflowInput, TOutput, TContext>(this.steps);
        }

        public LightflowBuilderNode<TLightflowInput, TOutput, TContext> Use<TOutput>(Func<ILightflowStep<TContext, TStepInput, TOutput>> factory)
        {
            this.steps.Add(new LightflowStepFactoryInvoker(
                factory,
                typeof(TContext),
                typeof(TStepInput),
                typeof(TOutput)));

            return new LightflowBuilderNode<TLightflowInput, TOutput, TContext>(this.steps);
        }

        public Lightflow<TLightflowInput, TContext> Build()
        {
            return new Lightflow<TLightflowInput, TContext>(this.steps);
        }
    }
}
