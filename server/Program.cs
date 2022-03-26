using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

class Server
{
    public static void Main()
    {
        Int32 port = 12345;
        int counter = 0;
        IPAddress localAddr = IPAddress.Parse("127.0.0.1");
        TcpListener server = new TcpListener(localAddr, port);
        server.Start();
        Console.WriteLine("Server start!");

        try
        {
            while (true)
            {
                counter += 1;
                TcpClient client = server.AcceptTcpClient();

                StartTheThread(client, Convert.ToString(counter));
                Console.WriteLine(Process.GetCurrentProcess().Threads.Count);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
        finally
        {
            server.Stop();
        }

    }

    static public void StartTheThread(TcpClient param1, string param2)
    {
        var t = new Thread(() => KeepListening(param1, param2));
        t.Start();
    }

    private static void KeepListening(TcpClient client, string name)
    {
        string clientIp = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
        string info = $"IP: {clientIp}, name: {name}";
        Console.WriteLine($"Client connected with {info}");

        try
        {
            while (client.Connected)
            {
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[2048];
                int bytesRead;
                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                {

                    string msg = Encoding.ASCII.GetString(buffer);
                    Console.WriteLine($"Receive {msg} from client: {info}");

                    msg = msg.ToUpper();

                    buffer = Encoding.ASCII.GetBytes(msg);

                    stream.Write(buffer, 0, bytesRead);
                    Console.WriteLine($"Send {msg} to client: {info}");
                }

                stream.Flush();
                stream.Close();
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        finally
        {
            client.Close();
            Console.WriteLine($"Client disconnected with {info}");
        }
    }
}