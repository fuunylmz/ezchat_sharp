using System.Net;
using System.Net.Sockets;
using System.Text;

class ezchat_sharp
{
   public static void Main(string[] args)
    {
        try
        {
            StartServer();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

    private static void StartServer()
    {
        var listener = new TcpListener(IPAddress.Parse("0.0.0.0"), 9999);
            listener.Start();
            Console.WriteLine("Server started. Waiting for a connection...");
            
            
            var client = listener.AcceptTcpClient();
                Console.WriteLine("Client connected.");
                HandleClient(client);
        
    }

    private static void HandleClient(TcpClient client)
    {
        var stream = client.GetStream();
            var data = new byte[1024];
            while (true)
            {
                int length;
                try
                {
                    length = stream.Read(data, 0, data.Length);
                }
                catch (IOException)
                {
                    // 如果读取过程中出现IO错误，则认为客户端已断开连接
                    Console.WriteLine("Client disconnected.");
                    return;
                }

                if (length == 0) continue;  // 接收到空数据时跳过

                var message = Encoding.UTF8.GetString(data, 0, length);
                Console.WriteLine("收到了消息: " + message);
            }
    }
}