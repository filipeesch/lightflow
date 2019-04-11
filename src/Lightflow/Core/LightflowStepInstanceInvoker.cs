namespace Lightflow.Core
{
    using System;
    using Lightflow.Steps;

    public class LightflowStepInstanceInvoker : LightflowStepBaseInvoker
    {
        private readonly ILightflowStep step;

        public LightflowStepInstanceInvoker(ILightflowStep step, Type contextType)
            : base(contextType, step.InputType, step.OutputType)
        {
            this.step = step;
        }

        protected override ILightflowStep GetStepinstance() => this.step;
    }
}
