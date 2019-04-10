namespace Lightflow.Core
{
    using System;
    using System.Reflection;
    using System.Threading.Tasks;
    using Lightflow.Contexts;
    using Lightflow.Steps;

    public abstract class LightflowStepBaseInvoker
    {
        protected readonly MethodInfo InvokeMethod;

        public Type InputType { get; }

        public Type OutputType { get; }

        protected LightflowStepBaseInvoker(
            Type contextType,
            Type inputType,
            Type outputType)
        {
            this.InputType = inputType;
            this.OutputType = outputType;

            this.InvokeMethod =
                typeof(ILightflowStep<,,>)
                    .MakeGenericType(contextType, inputType, outputType)
                    .GetMethod(nameof(ILightflowStep<int, int, int>.Invoke));
        }

        public abstract Task InvokeStep(ILightflowContext context, object input, Delegate next);
    }
}
