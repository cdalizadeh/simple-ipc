using System;
using System.Reactive.Subjects;
using PubSubIpc.Shared;

namespace PubSubIpc.Server
{
    public class LocalPublisherClient : IPublisherClient, IPublisher
    {
        private readonly Subject<string> _dataReceived = new Subject<string>();
        public IObservable<string> DataReceived => _dataReceived;

        public LocalPublisherClient()
        {

        }

        public void Send(string message)
        {
            _dataReceived.OnNext(message);
        }
    }
}