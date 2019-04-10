#pragma warning disable 4014
namespace Lightflow.Steps
{
    using System;
    using System.Threading;
    using System.Threading.Channels;
    using System.Threading.Tasks;
    using Lightflow.Contexts;

    public class LightflowBufferStep<T> : ILightflowStep<T, T>, IDisposable
    {
        private readonly Channel<Func<Task>> channel;
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private bool eventLoopIsRunning = false;

        public LightflowBufferStep(int boundedCapacity)
        {
            this.channel = Channel.CreateBounded<Func<Task>>(boundedCapacity);
        }

        public LightflowBufferStep()
        {
            this.channel = Channel.CreateUnbounded<Func<Task>>();
        }

        public Type InputType => typeof(T);

        public Type OutputType => typeof(T);

        public async Task Invoke(ILightflowContext context, T input, Func<T, Task> next)
        {
            while (!this.channel.Writer.TryWrite(() => next(input)))
            {
                await this.channel.Writer
                    .WaitToWriteAsync()
                    .ConfigureAwait(false);
            }

            this.EnsureEventLoopIsRunning();
        }

        private void EnsureEventLoopIsRunning()
        {
            if (this.eventLoopIsRunning)
                return;

            lock (this.cancellationTokenSource)
            {
                if (this.eventLoopIsRunning)
                    return;

                Task.Run(async () =>
                    {
                        while (
                            !this.cancellationTokenSource.IsCancellationRequested &&
                            await this.channel.Reader
                                .WaitToReadAsync(this.cancellationTokenSource.Token)
                                .ConfigureAwait(false))
                        {
                            while (this.channel.Reader.TryRead(out var item))
                                await item().ConfigureAwait(false);
                        }
                    },
                    this.cancellationTokenSource.Token);

                this.eventLoopIsRunning = true;
            }
        }

        public void Dispose()
        {
            this.cancellationTokenSource.Cancel();
        }
    }
}
