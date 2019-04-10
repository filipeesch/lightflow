namespace Lightflow.Steps
{
    using System;
    using System.Threading.Tasks;
    using Lightflow.Contexts;

    public interface ILightflowStep
    {
    }

    public interface ILightflowStep<in TContext, in TInput, out TOutput> : ILightflowStep
    {
        Task Invoke(TContext context, TInput input, Func<TOutput, Task> next);
    }

    public interface ILightflowStep<in TInput, out TOutput> : ILightflowStep<ILightflowContext, TInput, TOutput>
    {
    }
}
