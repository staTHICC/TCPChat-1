using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace TCPChat_Server
{
    internal class Program
    {
        static List<User> users = new List<User>();

        static void Main(string[] args)
        {
            Console.Title = "Server";

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream,
                ProtocolType.Tcp); //TCP socket

            IPAddress address = IPAddress.Parse("127.0.0.1"); //server ip

            IPEndPoint endRemotePoint = new IPEndPoint(address, 7632); //creating endpoint //server port

            socket.Bind(endRemotePoint); //binding socket to endpoint

            socket.Listen(1); //слушаем на наличие клиентов

            Console.WriteLine("Ожидаем звонка от клиента");

            while (true)
            {
                Socket client = socket.Accept(); //принять подключение
                User user = new User()
                {
                    Socket = client,
                    Name = RecieveMessage(client)
                };
                users.Add(user);

                Console.WriteLine($"[SERVER]: {user.Name} на связи!");
                SendMessageToClients($"[SERVER]: {user.Name} на связи!", user.Socket);

                //менеджер для приёма сообщений
                Thread threadRecieve = new Thread(RecieveMessageForManager); //создаём менеджера (что)
                threadRecieve.Start(user);
                //менеджер для отправки сообщений
                //Thread threadSend = new Thread(SendMessageForManager);
                //threadSend.Start(user);
            }
        }

        private static string RecieveMessage(Socket client)
        {
            byte[] bytes = new byte[1024];
            int num_bytes = client.Receive(bytes); //получение сообщения от клиента
            return Encoding.Unicode.GetString(bytes, 0, num_bytes);
        }

        private static void RecieveMessageForManager(Object objUser)
        {
            User user = objUser as User;

            while (true)
            {
                string message = RecieveMessage(user.Socket);
                Console.WriteLine($"[{user.Name}]: {message}");
                SendMessageToClients($"[{user.Name}]: {message}", user.Socket);
            }
        }

        private static void SendMessageForManager(Object objUser)
        {
            User user = objUser as User;

            while (true)
            {
                string sendMessage = $"[{user.Name}]: {Console.ReadLine()}";
                //SendMessage(client, sendMessage);

                //SendMessageToClientsForManager(sendMessage);
            }
        }

        private static void SendMessageToClients(string message, Socket sender)
        {
            foreach (var user in users)
            {
                if (user.Socket != sender)
                    SendMessage(user.Socket, message);
            }
        }

        private static void SendMessage(Socket client, string message)
        {
            byte[] bytes_answer = Encoding.Unicode.GetBytes(message);
            client.Send(bytes_answer);
        }
    }

    public class User
    {
        public string Name { get; set; }
        public Socket Socket { get; set; }
    }
}
