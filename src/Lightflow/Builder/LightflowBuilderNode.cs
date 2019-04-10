namespace Lightflow.Builder
{
    using System;
    using System.Collections.Generic;
    using Lightflow.Contexts;
    using Lightflow.Core;
    using Lightflow.Steps;

    public class LightflowBuilderNode<TLightflowInput, TStepInput, TContext> where TContext : ILightflowContext, new()
    {
        private readonly List<LightflowStepInfo> steps;

        public LightflowBuilderNode(List<LightflowStepInfo> filters)
        {
            this.steps = filters;
        }

        public LightflowBuilderNode<TLightflowInput, TOutput, TContext> Use<TOutput>(LightflowHandler<TContext, TStepInput, TOutput> handler)
        {
            this.steps.Add(new LightflowStepInfo(
                () => new HandlerLightflowStep<TContext, TStepInput, TOutput>(handler),
                typeof(TContext),
                typeof(TStepInput),
                typeof(TOutput)));

            return new LightflowBuilderNode<TLightflowInput, TOutput, TContext>(this.steps);
        }

        public LightflowBuilderNode<TLightflowInput, TOutput, TContext> Use<TOutput>(Func<ILightflowStep<TContext, TStepInput, TOutput>> factory)
        {
            this.steps.Add(new LightflowStepInfo(
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
