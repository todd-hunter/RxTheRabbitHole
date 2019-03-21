using System;

namespace Rx.Common.Broker
{
    public interface IEventBroker
    {
        IObservable<TEvent> Events<TEvent>();
        void Publish<TEvent>(TEvent e);
    }
}
