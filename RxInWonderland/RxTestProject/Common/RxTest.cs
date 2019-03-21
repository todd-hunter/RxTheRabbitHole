using FluentAssertions;
using Rx.Common;
using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Tests.Common;

namespace Tests.Common
{
    public abstract class RxTest<T>
    {
        protected IObservable<T> Sut;
        protected T Result;
        protected Exception Error;
        protected bool HasCompleted;
        protected int Counter;
        protected readonly IList<Exception> Errors = new List<Exception>();
        protected readonly IList<T> Results = new List<T>();
        protected IDisposable Subscription;
        protected CompositeDisposable Subscriptions;
        protected readonly TestSchedulerProvider Schedulers = new TestSchedulerProvider();

        protected void Init()
        {
            Errors.Clear();
            Results.Clear();
            Error = null;
            Result = default(T);
            Counter = 0;
            Subscription.DisposeSafe();
            Subscriptions.DisposeSafe();
            Subscriptions = new CompositeDisposable();
        }

        protected virtual void Subscribe(IScheduler scheduler = null)
        {
            Subscribe(t => { }, scheduler);
        }

        protected virtual void Subscribe(Action<T> action,
            IScheduler scheduler = null,
            IObservable<T> stream = null)
        {
            if (stream == null)
            {
                stream = Sut;
            }

            if (scheduler != null)
            {
                stream = stream.ObserveOn(scheduler);
            }

            Subscription = stream
                .Subscribe(t =>
                {
                    Counter++;
                    Result = t;
                    Results.Add(t);
                    action(t);
                },
                    ex =>
                    {
                        Error = ex;
                        Errors.Add(ex);
                    },
                    () => { HasCompleted = true; });

            Subscriptions.Add(Subscription);
        }

        protected virtual void CleanUp()
        {
            Subscriptions.DisposeSafe();
            Subscriptions = null;
            Subscription = null;
        }

        protected void ThereShouldHaveBeenNoErrors()
        {
            Error.Should().BeNull();
            Errors.Count.Should().Be(0);
        }

        protected void StreamCompleted()
        {
            HasCompleted.Should().BeTrue();
        }

        protected void StreamNotCompleted()
        {
            HasCompleted.Should().BeFalse();
        }

        protected void TimeAdvances(int seconds = 1)
        {
            Schedulers.Dispatcher.AdvanceBy(TimeSpan.FromSeconds(seconds).Ticks);
            Schedulers.TaskPool.AdvanceBy(TimeSpan.FromSeconds(seconds).Ticks);
            Schedulers.Dispatcher.AdvanceBy(TimeSpan.FromSeconds(seconds).Ticks);
            Schedulers.TaskPool.AdvanceBy(TimeSpan.FromSeconds(seconds).Ticks);
        }
    }
}