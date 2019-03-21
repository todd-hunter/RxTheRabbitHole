using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Rx.Common
{
    public static class SubscriptionExtensions
    {
        public static IObservable<T> BeforeSubscribe<T>(this IObservable<T> source, Action action, IDisposable d = null)
        {
            return Observable.Create<T>(o =>
            {
                var c = new CompositeDisposable();
                if (d != null)
                {
                    c.Add(d);
                }
                try
                {
                    action();
                    c.Add(source.Subscribe(o));
                }
                catch (Exception ex)
                {
                    o.OnError(ex);
                }
                return c;
            });
        }

        public static IObservable<T> AfterSubscribe<T>(this IObservable<T> source, Action action, IDisposable d = null)
        {
            return Observable.Create<T>(o =>
            {
                var c = new CompositeDisposable();
                if (d != null)
                {
                    c.Add(d);
                }
                c.Add(source.Subscribe(o));
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    o.OnError(ex);
                }
                return c;
            });
        }

        public static IObservable<T> ToObservable<T>(this Func<T> f)
        {
            var stream = Observable.Create<T>(o =>
            {
                try
                {
                    o.OnNext(f());
                    o.OnCompleted();
                }
                catch (Exception ex)
                {
                    o.OnError(ex);
                }
                return Disposable.Empty;
            });

            return stream;
        }

        public static IObservable<T2> ToObservable<T, T2>(this Func<T, T2> f, T x)
        {
            var stream = Observable.Create<T2>(o =>
            {
                try
                {
                    o.OnNext(f(x));
                    o.OnCompleted();
                }
                catch (Exception ex)
                {
                    o.OnError(ex);
                }
                return Disposable.Empty;
            });

            return stream;
        }
    }
}
