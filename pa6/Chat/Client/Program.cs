using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client;

class Program
{
    public static void StartClient(string hostaddress, int port, string msg)
    {
        try
        {
            // Разрешение сетевых имён
            IPAddress ipAddress = (hostAddress.Equals("localhost", StringComparison.OrdinalIgnoreCase)) 
                    ? IPAddress.Loopback 
                    : IPAddress.Parse(hostAddress);

            IPEndPoint remoteEndPoint = new IPEndPoint(ipAddress, port);

            // CREATE
            using (Socket clientSocket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
                {
                    try
                    {
                        // Подключение
                        clientSocket.Connect(remoteEndPoint);

                        // Отправка сообщения
                        byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                        clientSocket.Send(messageBytes);

                        // Прием ответа
                        byte[] buffer = new byte[1024];
                        int bytesReceived = clientSocket.Receive(buffer);
                        string response = Encoding.UTF8.GetString(buffer, 0, bytesReceived);

                        Console.WriteLine("{0}", response);

                        // Завершение соединения
                        clientSocket.Shutdown(SocketShutdown.Both);
                    }
                    catch (ArgumentNullException argEx)
                    {
                        Console.WriteLine("ArgumentNullException : {0}", argEx);
                    }
                    catch (SocketException socketEx)
                    {
                        Console.WriteLine("SocketException : {0}", socketEx);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Unexpected exception : {0}", ex);
                    }
                }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    static void Main(string[] args)
    {
        if (args.Length != 3)
        {
            Console.WriteLine("Invalid usage.\nUsage: dotnet run <hostAddress> <port> <msg>");
            return;
        }

        string hostAddress = args[0];
        bool isPortValid = int.TryParse(args[1], out int port);
        string message = args[2];

        if (String.IsNullOrEmpty(message))
        {
            Console.WriteLine("Empty message.");
            return;
        }

        if (!isPortValid)
        {
            Console.WriteLine("Invalid port");
            return;
        }

        StartClient(hostAddress, port, message);
    }
}