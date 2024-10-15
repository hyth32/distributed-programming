using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Chain
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length < 3 || !int.TryParse(args[0], out int listeningPort) ||
                !int.TryParse(args[2], out int nextPort))
            {
                Console.WriteLine("Invalid usage.\nUsage: dotnet run <listening-port> <next-host> <next-port> [true]");
                return;
            }

            string nextHost = args[1];
            bool isInitiator = args.Length == 4 && bool.TryParse(args[3], out bool initiatorValue) && initiatorValue;

            if (isInitiator)
            {
                await InitiateProcess(listeningPort, nextHost, nextPort);
            }
            else
            {
                await ExecuteProcess(listeningPort, nextHost, nextPort);
            }
        }

        private static async Task InitiateProcess(int listeningPort, string nextHost, int nextPort)
        {
            using var (receiveSocket, senderSocket) = await SetupSockets(listeningPort, nextHost, nextPort);
            int x = GetInputValue();

            using Socket handler = receiveSocket.Accept();
            await TransmitAndReceiveValues(senderSocket, handler, x);
        }

        private static async Task ExecuteProcess(int listeningPort, string nextHost, int nextPort)
        {
            using var (receiveSocket, senderSocket) = await SetupSockets(listeningPort, nextHost, nextPort);
            int x = GetInputValue();

            using Socket handler = receiveSocket.Accept();
            int y = ReceiveData(handler);
            Console.WriteLine("Received value: {0}", y);

            int maxNumber = Math.Max(x, y);
            Console.WriteLine("Max of x and y: {0}", maxNumber);
            TransmitData(senderSocket, maxNumber);

            await TransmitAndReceiveValues(senderSocket, handler, maxNumber);
        }

        private static async Task<(Socket receiveSocket, Socket senderSocket)> SetupSockets(int listeningPort, string nextHost, int nextPort)
        {
            var receiverSocket = CreateSocket(IPAddress.Any, listeningPort);
            var senderSocket = CreateSocket(IPAddress.Parse(nextHost == "localhost" ? "127.0.0.1" : nextHost), nextPort);

            while (true)
            {
                try
                {
                    await senderSocket.ConnectAsync(new IPEndPoint(senderSocket.RemoteEndPoint.Address, nextPort));
                    break;
                }
                catch
                {
                    await Task.Delay(1000);
                }
            }

            return (receiverSocket, senderSocket);
        }

        private static Socket CreateSocket(IPAddress ipAddress, int port)
        {
            var socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(new IPEndPoint(ipAddress, port));
            socket.Listen(10);
            return socket;
        }

        private static int GetInputValue()
        {
            if (!int.TryParse(Console.ReadLine(), out int value))
            {
                Console.WriteLine("Invalid input value");
                return 0;
            }
            return value;
        }

        private static async Task TransmitAndReceiveValues(Socket senderSocket, Socket handler, int value)
        {
            TransmitData(senderSocket, value);
            Console.WriteLine("Sending value: {0}", value);

            int receivedValue = ReceiveData(handler);
            Console.WriteLine("Received value: {0}", receivedValue);
            TransmitData(senderSocket, receivedValue);
            Console.WriteLine("Sending value: {0}", receivedValue);
        }

        private static void TransmitData(Socket socket, int data)
        {
            byte[] buffer = BitConverter.GetBytes(data);
            socket.Send(buffer);
        }

        private static int ReceiveData(Socket socket)
        {
            byte[] buffer = new byte[sizeof(int)];
            socket.Receive(buffer);
            return BitConverter.ToInt32(buffer, 0);
        }
    }
}
