using System;
using System.Threading;

namespace TCP_chat.src
{
    class Server
    {
        private static ServerCore server;
        private static Thread listenThread;

        public Server() {}

        /// <summary>
        /// Запускает сервер
        /// </summary>
        static void Main()
        {
            try
            {
                Console.Title = "TCP Server";
                server = new ServerCore();
                listenThread = new Thread(new ThreadStart(server.Listen));
                listenThread.Start();
            }
            catch (Exception ex)
            {
                server.Disconnect();
                Console.WriteLine(ex.Message);
            }
        }
    }
}
