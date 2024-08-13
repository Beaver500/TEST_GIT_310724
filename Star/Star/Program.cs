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
        /*
        ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~Консольный вариант ПО 

        0 - Экстренное завершение работы ПО с высвобождением ресурсов

                   1) *вывод сообщения* : "начать подключение к серверам?"
                   y/n or 0 (exit)
                   y - *подключаемся* 
                   n - *закрываем приложение*: "END"


                   2) *вывод сообщения* : "Отправить сообщение?"
                    y/n or 0 (exit)
                                         
                   y - *вывод сообщения* :"Read(r)/NoRead(n)" 

                            r/n or 0 (exit)

                            r - ПО отправляет сообщение "Read", получает ответ, передает Клиент_серву, Клиент_серв выводит итоговый Read/NoRead 
                                   
                                    *возвращение к началу 2 шага *: "Отправить сообщение?"*

                            n - ПО отправляет сообщение "NoRead", получает ответ, передает Клиент_серву, Клиент_серв выводит итоговый Read/NoRead 
                                   
                                    *возвращение к началу 2 шага *: "Отправить сообщение?"*
                        
                    n - *вывод сообщения* : "Отключиться от серверов?"
                      
                           y/n or 0 (exit)
                           y - *переход к шагу 1* : "начать подключение к серверам?"
                           n - *возвращение к началу 2 шага *: "Отправить сообщение?"
       ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

       ------------------------------------------------------------- Вариант ПО с WPF

           Окнос с кнопками:
            "Подклбчение к серверам"
            "Стоп"
            "Read"
            "NoRead"
            *консоль?*

                "подключение к серверам" - *подключаемся*
                "стоп" - *закрываем приложение*: "END"

            
                
               Кнопка "Начать обмен?" - *выполняется обмен сообщение и Клиент_серв выводит итоговый Read/NoRead*
                    
                


       -------------------------------------------------------------

                    */


        String servCount = null;
        string result = null;
        StringCollection collectionMessege = new StringCollection();

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




        do
        {
    
            Console.Write(" 0 - выход , 1 - Read, 2 - NoRead \n");
            servCount = Console.ReadLine();

            switch (servCount)
            {

                case "1":
                    Parallel.Invoke(
                       () => { collectionMessege.Add(clientConnectTo(10, server0_ipEndPoint, servCount)); },

                       () => { collectionMessege.Add(clientConnectTo(11, server1_ipEndPoint, servCount)); },

                       () => { collectionMessege.Add(clientConnectTo(12, server2_ipEndPoint, servCount)); }
                           );

                    result = checkEqualityCollection(collectionMessege);

                    clientConnectToClient_Serv(Client_Server_ipEndPoint, result);


                    break;

                case "2":
                    Parallel.Invoke(
                      () => { collectionMessege.Add(clientConnectTo(10, server0_ipEndPoint, servCount)); },

                      () => { collectionMessege.Add(clientConnectTo(11, server1_ipEndPoint, servCount)); },

                      () => { collectionMessege.Add(clientConnectTo(12, server2_ipEndPoint, servCount)); }
                          );

                    result = checkEqualityCollection(collectionMessege);

                    clientConnectToClient_Serv(Client_Server_ipEndPoint, result);

                    break;
                case "0":
                    /*
                     Освобождение ресурсов(отключение клиента от серверов с сообщением об этом)
                    Это будет отдельный метод7
                     */


                    // servCount = "0";
                    break;
            }

        } while (servCount != "0");
        Console.WriteLine("\nEND");

    }

    
    static string clientConnectTo(int port, IPEndPoint server_ipEndPoint, string servCount)
    {
        string result = null;
        
        try
        {
            IPAddress client_ipAddr = IPAddress.Parse("127.0.0.1");
            IPEndPoint client_ipEndPoint = new IPEndPoint(client_ipAddr, port);
            Socket client_soket = new Socket(client_ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            client_soket.Connect(server_ipEndPoint);

            
            byte[] bytes = new byte[1024];
            string message = servCount;
            byte[] msg = Encoding.UTF8.GetBytes(message);
            int bytesSent = client_soket.Send(msg);

            // Получаем ответ от сервера. выводим 
            int bytesRec = client_soket.Receive(bytes);
            Console.WriteLine("\nОтвет от TCP сервера " + client_soket.RemoteEndPoint.ToString() + " : {0}\n", Encoding.UTF8.GetString(bytes, 0, bytesRec));
            result = Encoding.UTF8.GetString(bytes, 0, bytesRec);

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

    
    static void clientConnectToClient_Serv(IPEndPoint server_ipEndPoint, string result)
    {
        
        try
        {
            IPAddress client_ipAddr = IPAddress.Parse("127.0.0.1");
            IPEndPoint client_ipEndPoint = new IPEndPoint(client_ipAddr, 3);
            Socket client_soket = new Socket(client_ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            client_soket.Connect(server_ipEndPoint);

            byte[] bytes = new byte[1024];
            string message = result;
            byte[] msg = Encoding.UTF8.GetBytes(message);
            int bytesSent = client_soket.Send(msg);
            

            int bytesRec = client_soket.Receive(bytes);
            Console.WriteLine("\nОтвет от Client сервера " + client_soket.RemoteEndPoint.ToString() + " : {0}\n", Encoding.UTF8.GetString(bytes, 0, bytesRec));


        }
        catch (Exception ex)
        {
            Console.WriteLine("clientConnectToClient_Serv");
            Console.WriteLine($"Исключение: {ex.Message}");
            Console.WriteLine($"Метод: {ex.TargetSite}");
            Console.WriteLine($"Трассировка стека: {ex.StackTrace}");

        }

    }

}