using Microsoft.Reactive.Testing;
using Rx.Common;
using System;
using System.Reactive.Concurrency;
using System.Threading;

namespace Tests.Common
{
    /// <summary>
    /// Lee's implementation of ISchedulerProvider?
    /// </summary>
    public sealed class TestSchedulerProvider : ISchedulerProvider
    {
        private readonly TestScheduler _currentThread = new TestScheduler();
        private readonly TestScheduler _dispatcher = new TestScheduler();
        private readonly TestScheduler _immediate = new TestScheduler();
        private readonly TestScheduler _newThread = new TestScheduler();
        private readonly TestScheduler _threadPool = new TestScheduler();
        private readonly TestScheduler _taskPool = new TestScheduler();
        private readonly TestScheduler _eventLoopScheduler = new TestScheduler();

        #region Implementation of ISchedulerService
        IScheduler ISchedulerProvider.CurrentThread => _currentThread;
        IScheduler ISchedulerProvider.Dispatcher => _dispatcher;
        IScheduler ISchedulerProvider.Immediate => _immediate;
        IScheduler ISchedulerProvider.NewThread => _newThread;
        IScheduler ISchedulerProvider.ThreadPool => _threadPool;
        IScheduler ISchedulerProvider.TaskPool => _taskPool;
        IScheduler ISchedulerProvider.EventLoopScheduler => _eventLoopScheduler;

        IScheduler ISchedulerProvider.EventLoopSchedulerFactory(Func<ThreadStart, Thread> threadFactory)
        {
            return _eventLoopScheduler;
        }

        #endregion

        public TestScheduler CurrentThread => _currentThread;
        public TestScheduler Dispatcher => _dispatcher;
        public TestScheduler Immediate => _immediate;
        public TestScheduler NewThread => _newThread;
        public TestScheduler ThreadPool => _threadPool;
        public TestScheduler TaskPool => _taskPool;
        public TestScheduler EventLoopScheduler => _eventLoopScheduler;
        public TestScheduler EventLoopSchedulerFactory(Func<ThreadStart, Thread> threadFactory) { return _eventLoopScheduler; }
    }
}