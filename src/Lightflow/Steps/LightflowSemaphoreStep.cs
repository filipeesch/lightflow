namespace Lightflow.Steps
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Lightflow.Contexts;

    public class LightflowSemaphoreStep<T> : ILightflowStep<T, T>
    {
        private readonly SemaphoreSlim semaphore;

        public LightflowSemaphoreStep(SemaphoreSlim semaphore)
        {
            this.semaphore = semaphore;
        }

        public Type InputType => typeof(T);

        public Type OutputType => typeof(T);

        public async Task Invoke(ILightflowContext context, T input, Func<T, Task> next)
        {
            await this.semaphore.WaitAsync().ConfigureAwait(false);

            try
            {
                await next(input).ConfigureAwait(false);
            }
            finally
            {
                this.semaphore.Release();
            }
        }
    }
}
