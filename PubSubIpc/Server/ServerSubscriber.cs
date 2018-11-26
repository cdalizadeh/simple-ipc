using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using log4net;

namespace PubSubIpc.Server
{
    class ServerSubscriber
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private Dictionary<string, IDisposable> _subscriptions = new Dictionary<string, IDisposable>();
        private readonly Connection _connection;

        public static Dictionary<string, ServerPublisher> Publishers;


        public ServerSubscriber(Connection connection)
        {
            Action<ControlCommand> onNext = (cc) =>
            {
                if (cc.Control == ControlBytes.Subscribe)
                {
                    Subscribe(cc.Data);
                }
                else if (cc.Control == ControlBytes.Unsubscribe)
                {
                    Unsubscribe(cc.Data);
                }
                else
                {
                    log.Error("Unknown control byte");
                }
            };
            _connection = connection;
            _connection.ControlReceived.Subscribe(onNext);
            _connection.InitReceiving();
            _connection.InitSending();
        }

        public void Subscribe(string publisherId)
        {
            log.Info($"Subscribing to Publisher ({publisherId})");
            //check if publisher exists
            var publisher = Publishers[publisherId];
            _subscriptions[publisherId] = publisher.DataReceived.Subscribe(_connection._sendDataSubject);
        }

        public void Unsubscribe(string publisherId)
        {
            log.Info($"Unsubscribing from Publisher ({publisherId})");
            _subscriptions[publisherId].Dispose();
            _subscriptions.Remove(publisherId);
        }
    }
}