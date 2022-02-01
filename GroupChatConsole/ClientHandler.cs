using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;

namespace ServerSide
{
    internal class ClientHandler
    {
        public static List<ClientHandler> ClientHandlers { get; set; } = new List<ClientHandler>();
        public TcpClient TcpClient { get; }
        public NetworkStream NetworkStream { get; set; }
        public string ClientUsername { get; set; }

        public StreamReader Reader { get; set; }
        public StreamWriter Writer { get; set; }

        public ClientHandler(TcpClient client)
        {
            try
            {
                TcpClient = client;
                NetworkStream = client.GetStream();
                Reader = new StreamReader(NetworkStream);
                Writer = new StreamWriter(NetworkStream);
                ClientUsername = Reader.ReadToEnd();
                ClientHandlers.Add(this);
                BroadcastMessage("SERVER: " + ClientUsername + " has entered the chat");
            } 
            catch(ArgumentException e)
            {
                Console.WriteLine("Constructor ArgumentException: " + e.Message);
                CloseEverything(TcpClient, Reader, Writer);
            }
            catch (IOException e)
            {
                Console.WriteLine("Constructor IOException" + e.Message);
                CloseEverything(TcpClient, Reader, Writer);
            }

        }

        public void BroadcastMessage(string messageToSend)
        {
            foreach (ClientHandler clientHandler in ClientHandlers)
            {
                try
                {
                    if (clientHandler.ClientUsername != this.ClientUsername)
                    {
                        Writer.Write(messageToSend);
                        //flush the stream so the client gets the message
                        NetworkStream.Flush();
                    }
                }
                catch (IOException e)
                {
                    CloseEverything(TcpClient, Reader, Writer);
                }
            }
        }

        public void ListenToMessage()
        {
            string messageFromClient;
            while (TcpClient != null && TcpClient.Client != null && TcpClient.Client.Connected)
            {
                //blocks the method until receives the message
                try
                {
                    messageFromClient = Reader.ReadToEnd();
                    BroadcastMessage(messageFromClient);
                } 
                catch (IOException e)
                {
                    CloseEverything(TcpClient, Reader, Writer);
                    break;
                }
                
            }
        }

        public void RemoveClientHandler()
        {
            ClientHandlers.Remove(this);
            BroadcastMessage("SERVER: " + ClientUsername + " has left the chat");
        }

        public void CloseEverything(TcpClient tcpClient, StreamReader reader, StreamWriter writer)
        {
            RemoveClientHandler();
            if (tcpClient != null)
                tcpClient.Close();

            if (reader != null)
                reader.Close();

            if (writer != null)
                writer.Close();

        }
    }
}