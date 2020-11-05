using System;
using System.Net.Sockets;
using System.Text;

namespace TCP_chat.src
{
    public class ClientCore
    {
        protected internal string Id { get; private set; }
        protected internal NetworkStream Stream { get; private set; }
        string nickname;
        TcpClient client;
        ServerCore server;

        /// <summary>
        /// Конструктор ClientCore
        /// </summary>
        /// <param name="tcpClient">Клиент</param>
        /// <param name="serverCore">Сервер</param>
        public ClientCore(TcpClient tcpClient, ServerCore serverCore)
        {
            Id = Guid.NewGuid().ToString();
            client = tcpClient;
            server = serverCore;
            serverCore.AddConnection(this);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Process()
        {
            try
            {
                Stream = client.GetStream();
                string message = GetMessage();
                nickname = message;

                message = nickname + " вошел в чат";
                server.BroadcastMessage(message, this.Id);
                Console.WriteLine(message);

                while (true)
                {
                    try
                    {
                        message = GetMessage();
                        message = String.Format("{0}: {1}", nickname, message);
                        Console.WriteLine(message);
                        server.BroadcastMessage(message, this.Id);
                    }
                    catch
                    {
                        message = String.Format("{0}: вышел из чата", nickname);
                        Console.WriteLine(message);
                        server.BroadcastMessage(message, this.Id);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                server.RemoveConnection(this.Id);
                Close();
            }
        }

        /// <summary>
        /// Получает сообщение
        /// </summary>
        /// <returns></returns>
        private string GetMessage()
        {
            byte[] data = new byte[64]; //buffer
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = Stream.Read(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            } while (Stream.DataAvailable);

            return builder.ToString();
        }

        /// <summary>
        /// Закрывает соединение
        /// </summary>
        protected internal void Close()
        {
            Stream?.Close();
            client?.Close();
        }
    }
}
