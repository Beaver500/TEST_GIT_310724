using MyLibrary;
using System.Net.Sockets;
using System.Net;
using System.Text.Json;
using System.Text;

public class ClientServMain {
    static void Main(string[] args)
    {
        
            FileStream server_Client = new FileStream("ClientServConfig_0.json", FileMode.Open);
            Client_Server client_Server = JsonSerializer.Deserialize<Client_Server>(server_Client);
            IPEndPoint client_server_ipEndPoint = new IPEndPoint(IPAddress.Parse(client_Server?.ipAddr), client_Server.port);
            Socket client_server_soket = new Socket(client_server_ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            server_Client.Close();

            // serverConnectTo2(client_server_ipEndPoint, client_server_soket, ';'); // через текущий проект
            client_Server.serverConnectTo(client_server_ipEndPoint, client_server_soket); // через библиотеку


    }
    

} 