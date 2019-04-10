namespace Lightflow.Core
{
    using Steps;
    using System;
    using System.Reflection;
    using System.Threading.Tasks;

    public class LightflowStepInfo
    {
        private readonly Func<ILightflowStep> factory;
        private readonly MethodInfo invokeMethod;

        public Type InputType { get; }

        public Type OutputType { get; }

        //TODO: A step info that receives and instance
        public LightflowStepInfo(
            Func<ILightflowStep> factory,
            Type contextType,
            Type inputType,
            Type outputType)
        {
            this.factory = factory;
            this.InputType = inputType;
            this.OutputType = outputType;

            this.invokeMethod =
                typeof(ILightflowStep<,,>)
                    .MakeGenericType(contextType, inputType, outputType)
                    .GetMethod(nameof(ILightflowStep<int, int, int>.Invoke));
        }

        public async Task InvokeStep(object context, object input, object handler)
        {
            var step = this.factory();

            await (Task)this.invokeMethod.Invoke(step, new[] { context, input, handler });
        }
    }
}
