using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ClientSide
{
    class Client
    {
        public TcpClient TcpClient { get; set; }
        public NetworkStream NetworkStream { get; set; }
        public StreamReader Reader { get; set; }
        public StreamWriter Writer { get; set; }
        public string Username { get; set; }

        public Client(TcpClient tcpClient, string username)
        {
            try
            {
                this.TcpClient = tcpClient;
                NetworkStream = tcpClient.GetStream();
                Reader = new StreamReader(NetworkStream);
                Writer = new StreamWriter(NetworkStream);
                this.Username = username;
            }
            catch (ArgumentException e)
            {
                CloseEverything(TcpClient, Reader, Writer);
            }
            catch (IOException e)
            {
                CloseEverything(TcpClient, Reader, Writer);
            }

        }

        public void SendMessage()
        {
            try
            {
                Writer.Write(Username);
                NetworkStream.Flush();
                Writer.Flush();

                while (TcpClient != null && TcpClient.Client != null && TcpClient.Client.Connected)
                {
                    string message = Console.In.ReadLine();
                    Writer.Write(Username + ": " + message);
                    NetworkStream.Flush();
                }
            }
            catch (IOException e)
            {
                CloseEverything(TcpClient, Reader, Writer);
            }
        }

        //runs on a separate thread
        public void ListenForMessage()
        {
            String messageFromGroupChat;
            while (TcpClient != null && TcpClient.Client != null && TcpClient.Client.Connected)
            {
                try
                {
                    messageFromGroupChat = Reader.ReadToEnd();
                    Console.WriteLine(messageFromGroupChat);
                }
                catch (IOException e)
                {
                    CloseEverything(TcpClient, Reader, Writer);
                }

            }
        }

        public void CloseEverything(TcpClient tcpClient, StreamReader reader, StreamWriter writer)
        {
            if (tcpClient != null)
                tcpClient.Close();

            if (reader != null)
                reader.Close();

            if (writer != null)
                writer.Close();

        }
    }
}
