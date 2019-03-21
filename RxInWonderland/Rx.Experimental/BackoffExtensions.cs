using Rx.Common;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Rx.Experimental
{
    /// <summary>
    /// This code is 'Experimental' for a reason: please beware of inner-dragons.
    /// I was trying to highlight the difficulty with 'Back Off Retry' in an RX discussion where previous solutions I'd seen used 
    /// recursion and had blown stack issues.
    /// Watching the tube/train arrive that evening it was out of service; again... It occurred to me you could probably use observables that contain inner-observables
    /// to avoid the recursion problem. Maybe...
    /// 
    /// Back on the Tube then:
    /// You can ignore the out of service trains and wait for a train that arrives that isn't broken. The tunnel is the outer one and the trains the inner ones. 
    /// Should be easy right? 
    /// I never found this one easy... And the code below is, sadly, still way to complicated. 
    ///     A train/stream yielding OK is treated as the infinite inner observable (zero, many, or an infinite number of Londoners getting out) 
    ///     An out-of-service train(stream yields error)is also just another broken tube train; get off it and wait for the next one
    ///   
    /// *** I'd suggest just sticking with .Retry(1) and keeping life easy. ***
    /// </summary>
    public static class BackoffExtensions
    {
        /// <summary>
        /// On exception 'conditionally' re-try after n milliseconds (which is the back-off). 
        /// This uses inner streams to yield the future attempt via am happy/unhappy path (functional railway style?).
        /// </summary>
        public static IObservable<T> BackOff<T>(this IObservable<T> source,
           ISchedulerProvider schedulers,
           int maxNumberOfRetrys = 3,
           Func<Exception, bool> shouldBackOff = null,
           Func<int, TimeSpan> nextDelay = null)
        {
            //streams for future retry attempts
            var sos = new Subject<IObservable<T>>();

            //yields result for the external consumer
            var result = new Subject<T>();

            //default func for next delay interval based on the current retry count
            Func<int, TimeSpan> standardDelay = i => TimeSpan.FromMilliseconds(666 * i);
            var delay = nextDelay ?? standardDelay;

            //default returns true so we back off on this error OR use the consumer supplied func
            Func<Exception, bool> always = e => true;
            var should = shouldBackOff ?? always;

            var resources = new CompositeDisposable();
            var sp = schedulers ?? new SchedulerProvider();
            var retryCount = 0;

            //subscribe to the internal nested streams
            resources.Add(
                sos.Subscribe(nextAttempt =>
                {
                    //if there is an individual stream inside the streams of streams (SOS) wrap it in a catch
                    resources.Add(nextAttempt
                        .Catch<T, Exception>(ex =>
                        {
                            //error happened so trip (sadly) down the unhappy path
                            retryCount = retryCount + 1;
                            if (retryCount <= maxNumberOfRetrys && should(ex))
                            {
                                //it is still OK to try again, schedule a future retry on the source, and return nothing
                                resources.Add(Observable.Timer(delay(retryCount), sp.TaskPool).Subscribe(_ => { sos.OnNext(source); }));
                                return Observable.Never<T>();
                            }
                            //it is NOT OK to try again, yield the exception out to the external consumer
                            result.OnError(ex);
                            sos.OnCompleted();
                            return Observable.Throw<T>(ex);
                        })
                        .Subscribe(happyPathResult =>
                        {
                            //happy path/no error, yay, skip happily onwards and yield the value to the external consumer
                            retryCount = 0;
                            result.OnNext(happyPathResult);
                        },
                        ex => { result.OnError(ex); },
                        () => { result.OnCompleted(); }));
                },
                ex => { result.OnError(ex); },
                () => result.OnCompleted()));

            //lazy action to get started by pushing in the first attempt when subscribed to
            Action startBackOff = () => sos.OnNext(source);
            //once the external subscriber subscribes then start the underlying backoff
            return result.AsObservable().AfterSubscribe(startBackOff, resources);
        }
    }
}
