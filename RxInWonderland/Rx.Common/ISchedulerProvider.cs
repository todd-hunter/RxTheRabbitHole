using System;
using System.Reactive.Concurrency;
using System.Threading;

namespace Rx.Common
{
    public interface ISchedulerProvider
    {
        IScheduler CurrentThread { get; }
        IScheduler Dispatcher { get; }
        IScheduler EventLoopScheduler { get; }
        IScheduler Immediate { get; }
        IScheduler NewThread { get; }
        IScheduler TaskPool { get; }
        IScheduler ThreadPool { get; }

        IScheduler EventLoopSchedulerFactory(Func<ThreadStart, Thread> threadFactory);
    }
}