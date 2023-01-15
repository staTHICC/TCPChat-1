using System;
using System.Net;
using System.Net.Sockets;
using System.Security.AccessControl;
using System.Text; //для кодирования
using System.Threading;

namespace TCPChat_Client
{
    internal class Program
    {

        static void Main(string[] args)
        {
            Console.Title = "Client";

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream,
                ProtocolType.Tcp);

            IPAddress address = IPAddress.Parse("127.0.0.1");
            IPEndPoint endPoint = new IPEndPoint(address, 7632);

            Console.Write("Нажмите Enter для подключения");

            Console.ReadLine();

            socket.Connect(endPoint); //connecting to remote endpoint

            Random random = new Random();
            Console.Write("\nПожалуйста, введите ваше имя: ");
            string name = Console.ReadLine();
            if (name == "")
                name = $"User{random.Next(int.MinValue, int.MaxValue)}";
            SendMessage(socket, name);

            User user = new User()
            {
                Name = name,
                Socket = socket
            };

            //Action<Socket> taskSendMessage = SendMessageForTask;
            //Action<Socket> taskRecieveMessage = RecieveMessageForTask;

            //IAsyncResult resSend = taskSendMessage.BeginInvoke(socket, null, null);
            //resRecieve = taskRecieveMessage.BeginInvoke(socket, null, null);

            //taskSendMessage.EndInvoke(resSend);
            //taskRecieveMessage.EndInvoke(resRecieve);

            Thread recieveThread = new Thread(RecieveMessageForTask);
            Thread sendThread = new Thread(SendMessageForTask);
            recieveThread.Start(user);
            sendThread.Start(user);
        }

        private static void SendMessageForTask(Object objUser)
        {
            User user = objUser as User;

            while (true)
            {
                Console.Write($"[{user.Name}]: ");
                string message = Console.ReadLine();
                SendMessage(user.Socket, message);
            }
        }

        public static void RecieveMessageForTask(Object objUser)
        {
            User user = objUser as User;

            while (true)
            {
                Console.WriteLine(RecieveMessage(user.Socket));
            }
        }

        private static void SendMessage(Socket client, string message)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(message);
            client.Send(bytes); //sending data to server
        }

        private static string RecieveMessage(Socket client)
        {
            byte[] byte_answer = new byte[1024];
            int num_bytes = client.Receive(byte_answer); //получение сообщения от сервера
            return Encoding.Unicode.GetString(byte_answer, 0, num_bytes);
        }
    }

    public class User
    {
        public string Name { get; set; }
        public Socket Socket { get; set; }
    }
}
