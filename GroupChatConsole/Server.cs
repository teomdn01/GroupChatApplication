using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerSide
{
    class Server
    {
        private TcpListener _listener;
        
        public Server(TcpListener listener)
        {
            this._listener = listener;
        }

        public void StartServer()
        {
            _listener.Start();
            while (true)
            {
                TcpClient client = _listener.AcceptTcpClient();
                Console.WriteLine("A new client has connected!");
                ClientHandler clientHandler = new ClientHandler(client);
                Thread thread = new Thread(clientHandler.ListenToMessage);
                //thread.IsBackground = true;
                thread.Start();
            }
        }

        //public void RunThread(ClientHandler clientHandler)
        //{
        //    clientHandler.ListenToMessage();
        //}
    }
}
