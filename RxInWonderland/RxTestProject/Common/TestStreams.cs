using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Tests.Common
{
    public static class TestStreams
    {
        public static IObservable<int> HasOneError()
        {
            return ErrorsUntil(1);
        }

        public static IObservable<int> ErrorsUntil(int until)
        {
            var count = 0;
            return Observable.Create<int>(o =>
            {
                if (count < until)
                {
                    count++;
                    o.OnError(new Exception("UNIT TEST ERROR " + count));
                }
                else
                {
                    o.OnNext(42);
                    o.OnCompleted();
                }

                return Disposable.Empty;
            });
        }

        public static IObservable<int> YeildsOne()
        {
            return Observable.Return(1);
        }
    }
}