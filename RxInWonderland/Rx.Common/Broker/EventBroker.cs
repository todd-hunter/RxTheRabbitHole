using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Rx.Common.Broker
{

    public class EventBroker : IEventBroker
    {
        private readonly ISubject<object> _subject = new Subject<object>();

        public IObservable<TEvent> Events<TEvent>()
        {
            return _subject.OfType<TEvent>().AsObservable();
        }

        public void Publish<TEvent>(TEvent e)
        {
            _subject.OnNext((object)e);
        }
    }
}
