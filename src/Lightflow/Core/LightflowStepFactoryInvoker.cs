namespace Lightflow.Core
{
    using System;
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

        protected override ILightflowStep GetStepinstance() => this.factory();
    }
}
