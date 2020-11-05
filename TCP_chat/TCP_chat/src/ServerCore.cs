using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;

namespace TCP_chat.src
{
    public class ServerCore
    {
        private static TcpListener tcpListener;
        List<ClientCore> clients = new List<ClientCore>();  // коллекция клиентов
        public ServerCore(){}

        /// <summary>
        /// Добавляет новое соединение в коллекцию сервера
        /// </summary>
        /// <param name="clientCore">Клиент</param>
        protected internal void AddConnection(ClientCore clientCore)
        {
            clients.Add(clientCore);
        }

        /// <summary>
        /// Удаляет соединение из коллекции сервера 
        /// </summary>
        /// <param name="id">Id клиента</param>
        protected internal void RemoveConnection(string id)
        {
            // возвращает первый элемент, подходящий под условие функции или дефолтный
            ClientCore client = clients.FirstOrDefault(c => c.Id == id);

            // удаление клиента из списка подключений
            if (client != null)
            {
                clients.Remove(client);
            }

        }

        /// <summary>
        /// Запускает прослушивание входящих подключений в отдельных потоках
        /// </summary>
        protected internal void Listen()
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Any, 8000);
                tcpListener.Start();
                Console.WriteLine("Сервер запущен. Ожидание подключений...");

                while (true)
                {
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();

                    ClientCore clientCore = new ClientCore(tcpClient, this);
                    Thread clientThread = new Thread(new ThreadStart(clientCore.Process));
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Disconnect();
            }
        }

        /// <summary>
        /// Транслирует сообщения подключенным клиентам 
        /// </summary>
        /// <param name="message">Сообщение</param>
        /// <param name="id">Id клиента</param>
        protected internal void BroadcastMessage(string message, string id)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            for (int i = 0; i < clients.Count; i++)
            {
                if (clients[i].Id != id)
                {
                    clients[i].Stream.Write(data, 0, data.Length);
                }
                
            }
        }

        /// <summary>
        /// Отключает всех клиентов
        /// </summary>
        protected internal void Disconnect()
        {

        }
    }
}
