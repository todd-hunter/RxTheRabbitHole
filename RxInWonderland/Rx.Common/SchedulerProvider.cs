using System;
using System.Reactive.Concurrency;
using System.Threading;

namespace Rx.Common
{
    /// <summary>
    /// Lee's scheduler provider with a few changes as time went on?
    /// http://introtorx.com/Content/v1.0.10621.0/01_WhyRx.html
    /// </summary>
    public sealed class SchedulerProvider : ISchedulerProvider
    {

        /// <summary>
        /// Add a reference to System.Reactive.Windows.Threading.dll and then use ObserveOnDispatcher or ObserveOn(new DispatcherScheduler()). 
        /// Edit: If you're using NuGet then you'll need the Rx-WPF package or Rx-Silverlight package.
        /// The latest release notes are in a sticky post on the forum: http://social.msdn.microsoft.com/Forums/en/rx/thread/6bfabdae-aff4-49ec-9b46-841a05baccdd
        /// The DispatcherScheduler (by accessing the static Dispatcher property) will schedule actions on the current Dispatcher, which is beneficial to Silverlight developers who use Rx. 
        /// Specified actions are then delegated to the Dispatcher.BeginInvoke() method in Silverlight.
        /// When actions are scheduled using the DispatcherScheduler, they are effectively marshaled to the Dispatcher's BeginInvoke method. 
        /// When an action is scheduled for future work, then a DispatcherTimer is created with a matching interval. The callback for the timer's tick will stop 
        /// the timer and re-schedule the work onto the DispatcherScheduler. If the DispatcherScheduler determines that the dueTime is actually not in the 
        /// future then no timer is created, and the action will just be scheduled normally.
        /// </summary>s
        public IScheduler Dispatcher => null; //DispatcherScheduler.Current;
        

        //public IScheduler Dispatcher
        //{
        //    get { return DispatcherScheduler.Current; }
        //}

        /// <summary>
        /// The CurrentThreadScheduler (by accessing the static CurrentThread property) will schedule actions to be performed on the thread that makes the original call.
        /// The action is not executed immediately, but is placed in a queue and only executed after the current action is complete. 
        /// The CurrentThreadScheduler acts like a message queue or a Trampoline. If you schedule an action that itself schedules an action, 
        /// the CurrentThreadScheduler will queue the inner action to be performed later; in contrast, the ImmediateScheduler would start
        ///  working on the inner action straight away.
        /// </summary>
        public IScheduler CurrentThread => Scheduler.CurrentThread;

        /// <summary>
        ///  The ImmediateScheduler (by accessing the static Immediate property) will start the specified action immediately.
        ///  If you call Schedule(Action) then it will just invoke the action. If you schedule the action to be invoked in the future, the 
        /// ImmediateScheduler will invoke a Thread.Sleep for the given period of time and then execute the action. 
        /// In summary, the ImmediateScheduler is synchronous.
        /// </summary>
        public IScheduler Immediate => Scheduler.Immediate;

        /// <summary>
        /// NewThreadScheduler (by accessing the static NewThread property) schedules actions on a new thread, and is optimal for scheduling 
        /// long running or blocking actions.
        /// The trampoline happens on a separate thread.
        /// When you call Schedule on the NewThreadScheduler, you are actually creating an EventLoopScheduler under the covers.
        /// This way, any nested scheduling will happen on the same thread. Subsequent (non-nested) calls to Schedule will create a new EventLoopScheduler 
        /// and call the thread factory function for a new thread too.
        /// </summary>
        public IScheduler NewThread => NewThreadScheduler.Default;

        /// <summary>
        /// Should be used only when TaskPool is appropriate but unavailable on your platform.
        /// Consider Scheduler.Default OR add reference to System.Reactive.Platform.Services & use Schedluer.Default OR pick specific from System.Reactive.Concurrecy
        /// ThreadPoolScheduler (by accessing the static ThreadPool property) schedules actions on the thread pool. Both pool schedulers are optimized for short-running actions.
        /// The ThreadPoolScheduler will simply just tunnel requests to the ThreadPool. For requests that are scheduled as soon as possible, the action is just sent to ThreadPool.QueueUserWorkItem. For requests that are scheduled in the future, a System.Threading.Timer is used.
        /// As all actions are sent to the ThreadPool, actions can potentially run out of order. Unlike the previous schedulers we have looked at, nested calls are not guaranteed to be processed serially.
        /// </summary>
        public IScheduler ThreadPool => ThreadPoolScheduler.Instance;

        /// <summary>
        /// The TaskPoolScheduler is very similar to the ThreadPoolScheduler and, when available (depending on your target framework), you should favor it over the later. Like the ThreadPoolScheduler, nested scheduled actions are not guaranteed to be run on the same thread.
        /// Takes precedence over ThreadPool. Useful for when the warm-up penalty for starting a new thread costs too much compared to the cost of the computation itself.  Should not be used when blocking will occur for lengthy periods of time; e.g., putting a pooled thread to sleep is wasteful.
        /// Consider Scheduler.Default OR add reference to System.Reactive.Platform.Services & use Schedluer.Default OR pick specific from System.Reactive.Concurrecy.
        /// Represents an object that schedules units of work using a provided TaskFactory. Both pool schedulers are optimized for short-running actions.
        /// </summary>
        public IScheduler TaskPool => TaskPoolScheduler.Default;

        /// <summary>
        /// The EventLoopScheduler allows you to designate a specific thread to a scheduler. Like the CurrentThreadScheduler that acts 
        /// like a trampoline for nested scheduled actions, the EventLoopScheduler provides the same trampoline mechanism. The difference is that you 
        /// provide an EventLoopScheduler with the thread you want it to use for scheduling instead, of just picking up the current thread.
        /// </summary>
        public IScheduler EventLoopScheduler => new EventLoopScheduler();

        /// <summary>
        /// The EventLoopScheduler allows you to designate a specific thread to a scheduler. Like the CurrentThreadScheduler that acts 
        /// like a trampoline for nested scheduled actions, the EventLoopScheduler provides the same trampoline mechanism. The difference is that you 
        /// provide an EventLoopScheduler with the thread you want it to use for scheduling instead, of just picking up the current thread.
        /// </summary>
        public IScheduler EventLoopSchedulerFactory(Func<ThreadStart, Thread> threadFactory)
        {
            return new EventLoopScheduler(threadFactory);
        }

    }
}
