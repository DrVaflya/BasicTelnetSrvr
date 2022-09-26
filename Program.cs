using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TCPPrototype
{
    class Server
    {
        static string MessageHandler(string message) // Простой, но пока что рабочий обработчик сообщений
        {
            switch(message)
            {
                case "ping":
                    return "pong";
                default:
                    return "Unknown command";
            }
        }
        static int port = 23;
        static void Main(string[] args)
        {
            string megabuffer = ""; //Тот же буффер, но в качестве строки
            TcpListener tcpListener = null;
            IPAddress LocalAddr = IPAddress.Parse("127.0.0.1");
            byte[] buffer = new byte[1]; //Буффер для получения символов
            byte[] responsebuffer = new byte[1024]; //Буффер для отправки обратного сообщения
            try
            {
                tcpListener = new TcpListener(LocalAddr,port);
                tcpListener.Start();
                string response = "";
                while(true)
                {
                    Console.WriteLine("Awaiting new connections");
                    TcpClient client = tcpListener.AcceptTcpClient();
                    Console.WriteLine("New connection established");
                    NetworkStream networkStream = client.GetStream();
                    while (networkStream.Read(buffer) > 0) //Цикл работает, пока поступают хоть какие-то данные
                    {
                        if (buffer[0] == 13) //В буффер пришел Enter
                        {
                            if (megabuffer == "close") //Команда на разрыв соединения со стороны хоста
                            {
                                break;
                            }
                            else
                            {
                                response = MessageHandler(megabuffer);
                                megabuffer = "";
                                responsebuffer = Encoding.Default.GetBytes(response);
                                networkStream.Write(responsebuffer, 0, responsebuffer.Length);
                                response = "";
                            }
                        }
                        else
                        {
                            if (buffer[0] != 10)
                            {
                                megabuffer += Encoding.UTF8.GetString(buffer);
                            }
                        }
                    }
                }
            }
            catch(Exception ex) 
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if(tcpListener != null)
                {
                    tcpListener.Stop();
                    Console.WriteLine("Connection was severed");
                }
            }
        }
    }
}
