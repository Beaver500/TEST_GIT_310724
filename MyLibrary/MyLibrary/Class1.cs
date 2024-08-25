using Microsoft.VisualBasic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;

namespace MyLibrary
{
    public abstract class Server
    {
        public virtual string? ipAddr { get; set; }
        public virtual int port { get; set; }
        //некоторый метод
        public virtual void serverConnectTo(IPEndPoint server_ipEndPoint, Socket server_soket)
        {
            // Начинаем слушать соединения.
            server_soket.Bind(server_ipEndPoint);
            server_soket.Listen(10);

            //соединения с клиентом
            Socket handler = server_soket.Accept();
            //Мы дождались клиента, пытающегося с нами соединиться
            Console.WriteLine("Клиент подключился к  {0}", server_ipEndPoint);
        }

        public virtual void messegeNoRead(IPEndPoint server_ipEndPoint, Socket server_soket)
        {

            server_soket.Bind(server_ipEndPoint);
            server_soket.Listen(10);


            Socket handler = server_soket.Accept();
            string? data = null;


            Console.WriteLine("Клиент подключился к  {0}", server_ipEndPoint);

            byte[] bytes = new byte[1024];
            int bytesRec = handler.Receive(bytes);
            data += Encoding.UTF8.GetString(bytes, 0, bytesRec);


            DateTime Data1 = DateTime.Today;
            DateTime Data2 = Data1.AddMonths(12);

            //messege
            // string reply = "#90#010102#27" + "000000" + ";" + "000000" + "#91";
            //string reply = "#90#010102#27" + Data1.ToString("ddMMyy") + ";" + "000000" + "#91";
            string reply = "#90#010102#27" + "000000" + ";" + Data2.ToString("ddMMyy") + "#91";
            byte[] msg = Encoding.UTF8.GetBytes(reply);
            handler.Send(msg);
        }

    }
    public class TCP_Server : Server
    {
        public override string? ipAddr { get; set; }
        public override int port { get; set; }

        public override void serverConnectTo(IPEndPoint server_ipEndPoint, Socket server_soket)
        {
            
                // Начинаем слушать соединения.
                Console.WriteLine("Ждем подключения  {0}", server_ipEndPoint);
                server_soket.Bind(server_ipEndPoint);
                server_soket.Listen(10);

                //Формирование даты
                DateTime Data1 = DateTime.Today;
                DateTime Data2 = Data1.AddMonths(12);

                byte[] bytes = new byte[1024];

                //соединения с клиентом
                Socket handler = server_soket.Accept();
               
                // дождались клиента, пытающегося соединиться
                Console.WriteLine("Клиент подключился к  {0}", server_ipEndPoint);

            while (true)
            {
                try
                {
                    string? data = null;
                    int bytesRec = handler.Receive(bytes);
                    data += Encoding.UTF8.GetString(bytes, 0, bytesRec);
                    string reply = null;

                    if (Object.Equals(data, "1") == true)
                    {
                        //Read
                        reply = "#90#010102#27" + Data1.ToString("ddMMyy") + ";" + Data2.ToString("ddMMyy") + "#91";

                    }
                    else if (Object.Equals(data, "2") == true)
                    {
                        //NoRead - random
                        string[] arr = { "#90#010102#27" + "000000" + ";" + Data2.ToString("ddMMyy") + "#91", "#90#010102#27" + Data1.ToString("ddMMyy") + ";" + "000000" + "#91", "#90#010102#27" + Data1.ToString("ddMMyy") + ";" + Data2.ToString("ddMMyy") + "#91" };

                        reply += arr[new Random().Next(0, arr.Length - 1)];

                    }
                    else if (Object.Equals(data, "0") == true)
                    {

                        handler.Shutdown(SocketShutdown.Both);
                        handler.Close();
                        break;

                    }
                    //messege

                    byte[] msg = Encoding.UTF8.GetBytes(reply);
                    handler.Send(msg);

                }
                catch (Exception ex)
                {
                    Console.WriteLine("serverConnectTo_TCP_Server");
                    Console.WriteLine($"Исключение: {ex.Message}");
                    Console.WriteLine($"Метод: {ex.TargetSite}");
                    Console.WriteLine($"Трассировка стека: {ex.StackTrace}");
                }
            }
            Console.WriteLine("завершение работы TCP сервера: {0}", server_ipEndPoint);

        }

      
        //Тест
        public override void messegeNoRead(IPEndPoint server_ipEndPoint, Socket server_soket)
        {

            try
            {
                Console.WriteLine("Ждем подключения  {0}", server_ipEndPoint);
                // Начинаем слушать соединения. 
                server_soket.Bind(server_ipEndPoint);
                server_soket.Listen(10);

                //соединения с клиентом
                Socket handler = server_soket.Accept();
                string? data = null;

                //Мы дождались клиента, пытающегося с нами соединиться
                Console.WriteLine("Клиент подключился к  {0}", server_ipEndPoint);

                byte[] bytes = new byte[1024];
                int bytesRec = handler.Receive(bytes);
                data += Encoding.UTF8.GetString(bytes, 0, bytesRec);
                

                //Формирование даты
                DateTime Data1 = DateTime.Today;
                DateTime Data2 = Data1.AddMonths(12);

                //messege
                //string reply = "#90#010102#27" + "000000" + ";" + "000000" + "#91";
                //string reply = "#90#010102#27" + Data1.ToString("ddMMyy") + ";" + "000000" + "#91";
                string reply = "#90#010102#27" + "000000" + ";" + Data2.ToString("ddMMyy") + "#91";

                //if (data == "1")
                //{
                //     reply = "#90#010102#27" + "000000" + ";" + Data2.ToString("ddMMyy") + "#91";
                //}
                
                byte[] msg = Encoding.UTF8.GetBytes(reply);
                handler.Send(msg);

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();

                // return reply;
            }
            catch (Exception ex)
            {
                Console.WriteLine("messegeNoRead");
                Console.WriteLine($"Исключение: {ex.Message}");
                Console.WriteLine($"Метод: {ex.TargetSite}");
                Console.WriteLine($"Трассировка стека: {ex.StackTrace}");
            }


        }

    }

    public class Client_Server : Server
    {
        protected internal string ipAddr { get; set; }
        protected internal int port { get; set; }
        public override  void serverConnectTo(IPEndPoint server_ipEndPoint, Socket server_soket)
        {

            string? dataSplit = null;
            string? result = null;
            string[]? mess = null;
            string? messPart = null;

            DateTime Data1Check = DateTime.Today;
            DateTime Data2Check = Data1Check.AddMonths(12);

                Console.WriteLine("Ждем подключения  {0}", server_ipEndPoint);
                // Начинаем слушать соединения.
                server_soket.Bind(server_ipEndPoint);
                server_soket.Listen(10);

                //соединения с клиентом
                Socket handler = server_soket.Accept();
                Console.WriteLine("Клиент подключился к  {0}", server_ipEndPoint);

            while (true)
            {
                try
                {
                    string? data = null;
                    byte[] bytes = new byte[1024];
                    int bytesRec = handler.Receive(bytes);

                    data += Encoding.UTF8.GetString(bytes, 0, bytesRec);

                    if (data == "0")
                    {
                        handler.Shutdown(SocketShutdown.Both);
                        handler.Close();
                        break;
                    }


                    dataSplit = data.Substring(13, 13);

                    Console.WriteLine("dataSplit = " + dataSplit);

                    mess = dataSplit.Split(';');

                        for (int i = 0; i <= 1; i++)
                        {

                            if (mess[i] == Data1Check.ToString("ddMMyy") && mess[i = i + 1] == Data2Check.ToString("ddMMyy"))
                            {
                                result = "READ";


                            }
                            else {
                                result = "!!!NO READ!!!";
                            }                            
                        }

                    byte[] msg = Encoding.UTF8.GetBytes(result);
                    handler.Send(msg);

                }
                catch (Exception ex)
                {
                    Console.WriteLine("serverConnectTo_Client_Server");
                    Console.WriteLine($"Исключение: {ex.Message}");
                    Console.WriteLine($"Метод: {ex.TargetSite}");
                    Console.WriteLine($"Трассировка стека: {ex.StackTrace}");
                }

            }
            Console.WriteLine("завершение работы TCP сервера: {0}", server_ipEndPoint);
        }
    }
}

    //класс который помогает узнать IPEndPoint серверов для клиента.
    public class Сonductor
    {
        public string ipAddr { get; set; }
        public int port { get; set; }
    }
