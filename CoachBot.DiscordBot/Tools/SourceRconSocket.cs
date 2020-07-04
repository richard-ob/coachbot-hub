using RconSharp;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace CoachBot.Extensions
{
    /// <summary>
     /// .NET Core implementation of <see cref="INetworkSocket"/> interface
     /// </summary>
    public class RconSocket : INetworkSocket
    {
        private TcpClient _client;

        /// <summary>
        /// Class constructor
        /// </summary>
        public RconSocket()
        {

        }

        /// <summary>
        /// Connect the socket to the remote endpoint
        /// </summary>
        /// <param name="host">remote host address</param>
        /// <param name="port">remote host port</param>
        /// <returns>True if the connection was successfully; False if the connection is already estabilished</returns>
        /// <exception cref="ArgumentException">is thrown when host parameter is null or empty, or when port parameter value is less than 1</exception>
        public async Task<bool> ConnectAsync(string host, int port)
        {
            if (string.IsNullOrEmpty(host))
                throw new ArgumentException("Invalid host name: must be a non null non empty string containing the host's address");

            if (port < 1)
                throw new ArgumentException("Port parameter must be a positive value");

            if (_client == null)
                _client = new TcpClient(AddressFamily.InterNetwork);

            if (_client.Connected)
                return false;

            await _client.ConnectAsync(host, port);
            return true;
        }

        /// <summary>
        /// Gets whether the connection is opened or not
        /// </summary>
        public bool IsConnected
        {
            get { return _client == null ? false : _client.Connected; }
        }

        /// <summary>
        /// Send a Rcon command to the remote server
        /// </summary>
        /// <param name="data">Rcon command data</param>
        /// <returns>The response to the command sent</returns>
        public async Task<byte[]> SendDataAndReadResponseAsync(byte[] data)
        {
            if (!IsConnected)
                throw new InvalidOperationException("You must call Connect() method before seinding data");

            NetworkStream ns = _client.GetStream();
            await ns.WriteAsync(data, 0, data.Length);
            await ns.FlushAsync();
            byte[] buffer = new byte[4096];
            int offset = 0;
            while (offset < 14)
            {
                await Task.Delay(100);
                offset += await ns.ReadAsync(buffer, offset, buffer.Length);
            }
            byte[] result = new byte[offset];
            Array.Copy(buffer, 0, result, 0, offset);
            return result;
        }

        /// <summary>
        /// Close the connection to the remote endpoint
        /// </summary>
        public void CloseConnection()
        {
            if (_client != null && _client.Connected)
            {
                _client.Close();
                _client = null;
            }
        }
    }
}
