using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ClientSide
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter your username for the group chat: ");
            string username = Console.ReadLine();
            TcpClient tcpClient;
            try
            {
                 tcpClient = new TcpClient();
                 tcpClient.Connect(IPAddress.Loopback, 1234);
            }
            catch (ArgumentOutOfRangeException e)
            {
                Console.WriteLine("Error when running client program");
                return;
            }
            catch (SocketException e)
            {
                Console.WriteLine("Error when running client program");
                return;
            }
            
            Client client = new Client(tcpClient, username);
            Thread listenerThread = new Thread(client.ListenForMessage);
            listenerThread.Start();
            client.SendMessage();
        }
    }
}
