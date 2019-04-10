namespace Lightflow.Core
{
    using System;
    using System.Threading.Tasks;
    using Lightflow.Contexts;
    using Lightflow.Steps;

    public class LightflowStepFactoryInvoker : LightflowStepBaseInvoker
    {
        private readonly Func<ILightflowStep> factory;

        public LightflowStepFactoryInvoker(
            Func<ILightflowStep> factory,
            Type contextType,
            Type inputType,
            Type outputType)
        : base(contextType, inputType, outputType)
        {
            this.factory = factory;
        }

        public override Task InvokeStep(ILightflowContext context, object input, Delegate next)
        {
            var step = this.factory();

            return (Task)this.InvokeMethod.Invoke(step, new[] { context, input, next });
        }
    }
}
