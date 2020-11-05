using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace TCP_client
{
    class Client
    {
        private static string nickname;
        private const string host = "127.0.0.1";
        private const int port = 8000;
        private static TcpClient client;
        private static NetworkStream stream;

        public Client() { }

        /// <summary>
        /// Создает клиент
        /// </summary>
        static void Main()
        {
            Console.Write("Введите nickname: ");
            nickname = Console.ReadLine();
            Console.Title = nickname + " chat";
            client = new TcpClient();
            try
            {
                client.Connect(host, port);
                stream = client.GetStream();

                string message = nickname;
                byte[] data = Encoding.Unicode.GetBytes(message);
                stream.Write(data, 0, data.Length);

                Thread receiveThread = new Thread(new ThreadStart(ReceiveMessage));
                receiveThread.Start();
                Console.WriteLine("Добро пожаловать, {0}", nickname);
                SendMessage();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Disconnect();
            }
        }
        /// <summary>
        /// Отправляет сообщение
        /// </summary>
        static void SendMessage()
        {
            Console.WriteLine("Введите сообщение:");

            while (true)
            {
                string message = Console.ReadLine();
                byte[] data = Encoding.Unicode.GetBytes(message);
                stream.Write(data, 0, data.Length);
            }
        }

        /// <summary>
        /// Получает сообщения
        /// </summary>
        static void ReceiveMessage()
        {
            while (true)
            {
                try
                {
                    byte[] data = new byte[64];
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    } while (stream.DataAvailable);

                    string message = builder.ToString();
                    Console.WriteLine(message);
                }
                catch
                {
                    Console.WriteLine("Соединение прервано!");
                    Console.ReadLine();
                    Disconnect();
                }
            }
        }

        /// <summary>
        /// Отключает клиента
        /// </summary>
        static void Disconnect()
        {
            stream?.Close();
            client?.Close();
        }
    }
}
