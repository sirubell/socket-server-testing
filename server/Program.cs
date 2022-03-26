using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

class Server
{
    public static void Main()
    {
        TcpListener server = null;
        List<Player> players = new List<Player>();
        int counter = 0;
        try
        {
            Int32 port = 12345;
            // IPAddress localAddr = IPAddress.Parse(GetLocalIPAddress());
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");

            server = new TcpListener(localAddr, port);
            server.Start();

            Byte[] bytes = new Byte[256];

            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                counter += 1;

                Player player = new Player(client, Convert.ToString(counter));
                players.Add(player);
            }
        }
        catch (SocketException e)
        {
            Console.WriteLine("SocketException: {0}", e);
        }
        finally
        {
            if (server != null)
            {
                server.Stop();
            }
        }

        Console.WriteLine("\nHit enter to continue...");
        Console.Read();
    }
    private static string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        throw new Exception("No network adapters with an IPv4 address in the system!");
    }
}
public class Player
{
    TcpClient clientSocket;
    string clNo = String.Empty;
    public Player(TcpClient inClientSocket, string clineNo)
    {
        this.clientSocket = inClientSocket;
        this.clNo = clineNo;
        Thread ctThread = new Thread(keepListening);
        ctThread.Start();
    }
    private void keepListening()
    {
        byte[] bytes = new byte[128];

        Console.WriteLine("Socket: " + clNo + " connected!");
        while (clientSocket.Connected)
        {
            try
            {
                NetworkStream stream = clientSocket.GetStream();
                Console.WriteLine("if this message shows too many times, it is a bug.");

                int i;

                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    String data = Encoding.ASCII.GetString(bytes, 0, i);
                    Console.WriteLine("Received: {0}", data);

                    data = data.ToUpper();

                    byte[] msg = Encoding.ASCII.GetBytes(data);

                    stream.Write(msg, 0, msg.Length);
                    Console.WriteLine("Sent: {0}", data);
                }

                stream.Flush();
                // must Close, idk why
                stream.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("SocketException: {0}", e);
                break;
            }
        }
        Console.WriteLine("Socket: " + clNo + " disconnected!");
    }
}