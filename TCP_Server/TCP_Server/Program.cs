using MyLibrary;
using System.Net.Sockets;
using System.Net;
using System.Text.Json;
using System.Text;
public class ServerMain {
    static void Main(string[] args) {
        //Sapport_project_TOP\TCP_Server\TCP_Server\bin\Debug\net8.0
        FileStream js_File_Server0 = new FileStream("TCPServConfig_0.json", FileMode.Open);
        TCP_Server server0 = JsonSerializer.Deserialize<TCP_Server>(js_File_Server0);
        IPEndPoint server0_ipEndPoint = new IPEndPoint(IPAddress.Parse(server0.ipAddr), server0.port);
        Socket server0_soket = new Socket(server0_ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        js_File_Server0.Close();

        FileStream js_File_Server1 = new FileStream("TCPServConfig_1.json", FileMode.Open);
        TCP_Server server1 = JsonSerializer.Deserialize<TCP_Server>(js_File_Server1);
        IPEndPoint server1_ipEndPoint = new IPEndPoint(IPAddress.Parse(server1.ipAddr), server1.port);
        Socket server1_Soket = new Socket(server1_ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        js_File_Server1.Close();

        FileStream js_File_Server2 = new FileStream("TCPServConfig_2.json", FileMode.Open);
        TCP_Server server2 = JsonSerializer.Deserialize<TCP_Server>(js_File_Server2);
        IPEndPoint server2_ipEndPoint = new IPEndPoint(IPAddress.Parse(server2.ipAddr), server2.port);
        Socket server2_soket = new Socket(server2_ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        js_File_Server2.Close();
        

        Parallel.Invoke(
            () =>{ server0.serverConnectTo(server0_ipEndPoint, server0_soket);},
            () =>{ server1.serverConnectTo(server1_ipEndPoint, server1_Soket); },
            () => { server2.messegeNoRead(server2_ipEndPoint, server2_soket); }
            //() => { server2.serverConnectTo(server2_ipEndPoint, server2_soket); }
                        
            );


    }

  
}

