using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;

namespace Server;

class Program
{
    public static void StartListening(int port)
    {
        // Привязываем сокет ко всем интерфейсам на текущей машинe
        var ipAddress = IPAddress.Any;
        var localEndPoint = new IPEndPoint(ipAddress, port);

        // CREATE
        using (Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
        {
            try
            {
                // BIND
                listener.Bind(localEndPoint);

                // LISTEN
                listener.Listen(10);

                var messageHistory = new List<string>();

                while (true)
                {
                    // ACCEPT
                    using (Socket handler = listener.Accept())
                    {
                        byte[] buffer = new byte[1024];

                        // RECEIVE
                        int bytesReceived = handler.Receive(buffer);
                        string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesReceived);

                        Console.WriteLine("Message received: {0}", receivedData);
                        messageHistory.Add(receivedData);

                        // Объединяем все сообщения в одну строку
                        string combinedMessages = string.Join(Environment.NewLine, messageHistory);

                        // Преобразуем объединенную строку в массив байт
                        byte[] responseMessage = Encoding.UTF8.GetBytes(combinedMessages);

                        // SEND
                        handler.Send(responseMessage);

                        // RELEASE
                        handler.Shutdown(SocketShutdown.Both);
                        handler.Close();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e);
            }
        }
    }

    static void Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.WriteLine("Invalid usage\nUsage: dotnet run <port>");
            return;
        }

        bool isPortValid = int.TryParse(args[0], out int port);

        if (!isPortValid)
        {
            Console.WriteLine("Invalid port");
            return;
        }

        StartListening(port);
    }
}