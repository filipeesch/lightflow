namespace Lightflow.Core
{
    using System;
    using System.Threading.Tasks;

    public delegate Task LightflowHandler<in TContext, in TInput, out TOutput>(TContext context, TInput input, Func<TOutput, Task> next);
}
