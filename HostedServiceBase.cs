using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace SharpUtils
{
    /// <summary>
    /// ASP.NET hosted service base class
    /// </summary>
    public abstract class HostedServiceBase : IHostedService, IDisposable
    {
        private readonly TimeSpan dueTime;
        private readonly TimeSpan period;

        private Timer timer;

        /// <summary>
        /// Service provider
        /// </summary>
        protected IServiceProvider Services { get; }

        protected HostedServiceBase(IServiceProvider services, TimeSpan dueTime, TimeSpan period)
        {
            Services = services;
            this.dueTime = dueTime;
            this.period = period;
        }

        /// <summary>
        /// Background worker task
        /// </summary>
        protected abstract Task DoWork();

        /// <inheritdoc />
        public Task StartAsync(CancellationToken cancellationToken)
        {
            timer = new Timer(DoWork, null, dueTime, period);
            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            try
            {
                await DoWork();
            }
            catch (Exception e)
            {
                Console.WriteLine(e); // replace by any logger
                throw;
            }
        }

        /// <inheritdoc />
        public Task StopAsync(CancellationToken cancellationToken)
        {
            timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            timer?.Dispose();
        }
    }
}
