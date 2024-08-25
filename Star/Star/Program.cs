using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using MyLibrary;
using System.Reflection;
using System.Collections.Specialized;
using System.Data.SqlTypes;
using static System.Net.Mime.MediaTypeNames;
using System.Text.Json;
using System.Diagnostics.Metrics;

public class Star
{

    static void Main(string[] args)
    {
        StringCollection collectionMessege = new StringCollection();
        String servCount = null;
        string result = null;
        Socket socket0 = null;
        Socket socket1 = null;
        Socket socket2 = null;
        Socket socket3 = null;

        bool Count = true;

        FileStream js_File_Server0 = new FileStream("..\\..\\..\\..\\..\\TCP_Server\\TCP_Server\\bin\\Debug\\net8.0\\TCPServConfig_0.json", FileMode.Open);
        Сonductor conductor0 = JsonSerializer.Deserialize<Сonductor>(js_File_Server0);
        IPEndPoint server0_ipEndPoint = new IPEndPoint(IPAddress.Parse(conductor0.ipAddr), conductor0.port);
        js_File_Server0.Close();

        FileStream js_File_Server1 = new FileStream("..\\..\\..\\..\\..\\TCP_Server\\TCP_Server\\bin\\Debug\\net8.0\\TCPServConfig_1.json", FileMode.Open);
        Сonductor conductor1 = JsonSerializer.Deserialize<Сonductor>(js_File_Server1);
        IPEndPoint server1_ipEndPoint = new IPEndPoint(IPAddress.Parse(conductor1.ipAddr), conductor1.port);
        js_File_Server1.Close();

        FileStream js_File_Server2 = new FileStream("..\\..\\..\\..\\..\\TCP_Server\\TCP_Server\\bin\\Debug\\net8.0\\TCPServConfig_2.json", FileMode.Open);
        Сonductor conductor2 = JsonSerializer.Deserialize<Сonductor>(js_File_Server2);
        IPEndPoint server2_ipEndPoint = new IPEndPoint(IPAddress.Parse(conductor2.ipAddr), conductor2.port);
        js_File_Server2.Close();

        FileStream js_File_Client_Server = new FileStream("..\\..\\..\\..\\..\\Client_Server\\Client_Server\\bin\\Debug\\net8.0\\ClientServConfig_0.json", FileMode.Open);
        Сonductor conductor3 = JsonSerializer.Deserialize<Сonductor>(js_File_Client_Server);
        IPEndPoint Client_Server_ipEndPoint = new IPEndPoint(IPAddress.Parse(conductor3.ipAddr), conductor3.port);
        js_File_Client_Server.Close();

       
        Parallel.Invoke(
                       () => {  socket0 = clientConnectTo(10, server0_ipEndPoint);},

                       () => {  socket1 = clientConnectTo(11, server1_ipEndPoint);},

                       () => {  socket2 = clientConnectTo(12, server2_ipEndPoint);},

                       () => {  socket3 = clientConnectToClient_Serv(Client_Server_ipEndPoint);}
                           );
       
        while (Count) {
    
            Console.Write("\n0 - exit, 1 - Read, 2 - NoRead: ");
            servCount = Console.ReadLine();

            Parallel.Invoke(
                       
                       () => { collectionMessege.Add(sendMessageTCP_server(socket0, servCount)); },

                       () => { collectionMessege.Add(sendMessageTCP_server(socket1, servCount)); },

                       () => { collectionMessege.Add(sendMessageTCP_server(socket2, servCount)); }

                           );

            if (Object.Equals(servCount, "0") == true)
            {
                Console.WriteLine("Завершение работы");
                Count = false;
            }

            result = checkEqualityCollection(collectionMessege);

            sendMassegeClient_Serv(socket3, result, servCount);

            collectionMessege.Clear();

        }

    }

    
    static Socket clientConnectTo(int port, IPEndPoint server_ipEndPoint)
    {
        Socket result = null;
        
        try
        {
            IPAddress client_ipAddr = IPAddress.Parse("127.0.0.1");
            IPEndPoint client_ipEndPoint = new IPEndPoint(client_ipAddr, port);
            Socket client_soket = new Socket(client_ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            client_soket.Connect(server_ipEndPoint);
            Console.WriteLine("\n Подключенo к TCP серверу " + client_soket.RemoteEndPoint.ToString());

            result = client_soket;
 

        }
        catch (Exception ex)
        {
            Console.WriteLine("clientConnectTo");
            Console.WriteLine($"Исключение: {ex.Message}");
            Console.WriteLine($"Метод: {ex.TargetSite}");
            Console.WriteLine($"Трассировка стека: {ex.StackTrace}");

        }
        return result;


    }

    static string sendMessageTCP_server(Socket client_soket, string servCount) {
        string? result = null;

        try {

            byte[] bytes = new byte[1024];
            string message = servCount;
            byte[] msg = Encoding.UTF8.GetBytes(message);
            int bytesSent = client_soket.Send(msg);

            int bytesRec = client_soket.Receive(bytes);
            Console.WriteLine("\nОтвет от TCP сервера " + client_soket.RemoteEndPoint.ToString() + " : {0}\n", Encoding.UTF8.GetString(bytes, 0, bytesRec));
            result = Encoding.UTF8.GetString(bytes, 0, bytesRec);



        }
        catch (Exception ex) {

            Console.WriteLine("clientConnectTo");
            Console.WriteLine($"Исключение: {ex.Message}");
            Console.WriteLine($"Метод: {ex.TargetSite}");
            Console.WriteLine($"Трассировка стека: {ex.StackTrace}");

        }

        return result;
    }

    static string checkEqualityCollection(StringCollection collectionMessege)
    {
        string? result = null;
        try
        {
            // - выделенная часть с датами и сообщения от сервера
            string? exampleMessage = null;
            // пример ответа от сервера, для последующей сборки result текущего метода. 
            string? exampleMessage2 = null;            
            //разбили сообщение от сервера на 2 части до  "Data1;Data2" и после 
            string? messPart1 = null;
            string? messPart2 = null;
            //переменная для хранения и последущего Split
            string? messy = null;
            //массив хранящий вырезанные пары дат из сообщений серверов
            string[] mess2 = new string[collectionMessege.Count];
            //массив сборки разбитых дат по одной
            string[]? mess3 = null;
            // коллекция для сборки разбитых дат и последущего их сравнения.
            StringCollection Coll = new StringCollection();


            for (int i = 0; i < collectionMessege.Count; i++)
            {
                mess2[i] = collectionMessege[i].Substring(13, 13);

                if (i == 0)
                {
                    messPart1 = collectionMessege[i].Substring(0, 13);
                    messPart2 = collectionMessege[i].Substring(26, 3);

                }
            }

            for (int i = 0; i < collectionMessege.Count; i++)
            {
                messy = mess2[i];
                mess3 = messy.Split(';');

                for (int j = 0; j < mess3.Length; j++)
                {
                    Coll.Add(mess3[j]);
                }
            }

            // вывод пар до

            for (int i = 0; i < Coll.Count; i++)
            {
                Console.WriteLine(" [{0}]  {1} ", i, Coll[i]);
            }
            Console.WriteLine();


            for (int i = 0; i <= Coll.Count / 2; i = i + 2)
            {
                if (Object.Equals(Coll[i], Coll[i + 2]) == false)
                {
                    for (int q = 0; q < Coll.Count; q = q + 2)
                    {
                        Coll[q] = "NoRead";
                    }
                }

                for (int j = 1; j <= Coll.Count / 2; j = j + 2)
                {

                    if (Object.Equals(Coll[j], Coll[j + 2]) == false)
                    {
                        for (int w = 1; w < Coll.Count; w = w + 2)
                        {
                            Coll[w] = "NoRead";
                        }
                    }
                }
            }

            // вывод пар после 

            for (int i = 0; i < Coll.Count; i++)
            {
                Console.WriteLine(" [{0}]  {1} ", i, Coll[i]);
            }

            //Сборка итогового сообщения 
            for (int i = 0; i < Coll.Count;)
            {

                result = messPart1 + Coll[i] + ";" + Coll[i + 1] + messPart2;
                break;
            }
            
            Console.WriteLine();
            Console.WriteLine("result " + result + "\n");

        }
        catch (Exception ex) {
            Console.WriteLine("checkEqualityCollection");
            Console.WriteLine($"Исключение: {ex.Message}");
            Console.WriteLine($"Метод: {ex.TargetSite}");
            Console.WriteLine($"Трассировка стека: {ex.StackTrace}");
        }
        return result;
    }
    
    static Socket clientConnectToClient_Serv(IPEndPoint server_ipEndPoint)
    {

        IPAddress client_ipAddr = IPAddress.Parse("127.0.0.1");
        IPEndPoint client_ipEndPoint = new IPEndPoint(client_ipAddr, 13);
        Socket client_soket = new Socket(client_ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        try
        {
            client_soket.Connect(server_ipEndPoint);
            Console.WriteLine("\n Подключенo к Client серверу " + client_soket.RemoteEndPoint.ToString());
                     
        }
        catch (Exception ex)
        {
            Console.WriteLine("clientConnectToClient_Serv");
            Console.WriteLine($"Исключение: {ex.Message}");
            Console.WriteLine($"Метод: {ex.TargetSite}");
            Console.WriteLine($"Трассировка стека: {ex.StackTrace}");

        }
        return client_soket;
    }


    static void sendMassegeClient_Serv(Socket client_soket, string result, string servCount) {

        try {

            if (servCount == "0") {
                result = servCount;
            }

            byte[] bytes = new byte[1024];
            string message = result;
            byte[] msg = Encoding.UTF8.GetBytes(message);
            int bytesSent = client_soket.Send(msg);

            int bytesRec = client_soket.Receive(bytes);
            Console.WriteLine("\nОтвет от Client сервера " + client_soket.RemoteEndPoint.ToString() + " : {0}\n", Encoding.UTF8.GetString(bytes, 0, bytesRec));


        }
        catch (Exception ex) {
            Console.WriteLine("clientConnectToClient_Serv");
            Console.WriteLine($"Исключение: {ex.Message}");
            Console.WriteLine($"Метод: {ex.TargetSite}");
            Console.WriteLine($"Трассировка стека: {ex.StackTrace}");

           }
    }
}