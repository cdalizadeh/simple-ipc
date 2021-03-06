using System;
using System.Linq;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Text;
using log4net;
using SimpleIpc.Shared;

namespace SimpleIpc.Server
{
    /// <summary>
    /// A connection object to use within the IPC server.
    /// </summary>
    internal sealed class ServerConnection : Connection
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public IObservable<string> MessageReceived => _dataReceivedSubject
            .Where(bytes => bytes[0] != (byte)ControlBytes.Escape)
            .Select(bytes => Encoding.ASCII.GetString(bytes, 0, bytes.Length));
        public IObservable<ControlCommand> ControlReceived => _dataReceivedSubject
            .Where(bytes => bytes[0] == (byte)ControlBytes.Escape)
            .Select(bytes => ControlCommand.FromByteArray(bytes));

        public ServerConnection(Socket socket)
        {
            _socket = socket;
        }
    }
}