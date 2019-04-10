namespace Lightflow.Core
{
    using System;
    using System.Threading.Tasks;
    using Lightflow.Contexts;
    using Lightflow.Steps;

    public class LightflowStepInstanceInvoker : LightflowStepBaseInvoker
    {
        private readonly ILightflowStep step;

        public LightflowStepInstanceInvoker(ILightflowStep step, Type contextType)
            : base(contextType, step.InputType, step.OutputType)
        {
            this.step = step;
        }

        public override Task InvokeStep(ILightflowContext context, object input, Delegate next)
        {
            return (Task)this.InvokeMethod.Invoke(this.step, new[] { context, input, next });
        }
    }
}
