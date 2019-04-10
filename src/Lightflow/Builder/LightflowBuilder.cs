namespace Lightflow.Builder
{
    using System;
    using System.Collections.Generic;
    using Lightflow.Contexts;
    using Lightflow.Core;
    using Lightflow.Steps;

    public class LightflowBuilder<TLightflowInput, TContext> where TContext : ILightflowContext, new()
    {
        public LightflowBuilderNode<TLightflowInput, TOutput, TContext> Use<TOutput>(LightflowHandler<TContext, TLightflowInput, TOutput> handler)
        {
            var steps = new List<LightflowStepInfo>();

            var node = new LightflowBuilderNode<TLightflowInput, TLightflowInput, TContext>(steps);
            return node.Use(handler);
        }

        public LightflowBuilderNode<TLightflowInput, TOutput, TContext> Use<TOutput>(Func<ILightflowStep<TContext, TLightflowInput, TOutput>> factory)
        {
            var steps = new List<LightflowStepInfo>();

            var node = new LightflowBuilderNode<TLightflowInput, TLightflowInput, TContext>(steps);
            return node.Use(factory);
        }
    }
}
