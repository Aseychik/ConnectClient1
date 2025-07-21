using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace ConnectClient1
{
    class ClientWF
    {
        public static NetworkStream sendmessage;
        public static bool isConnected = false;
        public static TcpClient client;

        public static void ClientMain(ref TextBox outputText, string serverIpPort, string clientName)
        {
            try
            {
                if (!serverIpPort.Contains(":")) throw new Exception("Wrong server Ip or server port");

                string serverIp = serverIpPort.Split(':')[0];
                if (!IPAddress.TryParse(serverIp, out IPAddress address)) throw new Exception("Wrong server Ip");


                outputText.AppendText(Environment.NewLine + $"Your client name: {clientName}" + Environment.NewLine + "Connection attempt");

                int port = int.Parse(serverIpPort.Split(':')[1]);


                client = new TcpClient();

                try
                {
                    outputText.AppendText(Environment.NewLine + $"Connecting to {serverIp}:{port}...");
                    client.Connect(serverIp, port);

                    // First send our name to the server
                    NetworkStream stream = client.GetStream();
                    byte[] nameData = Encoding.UTF8.GetBytes(clientName);
                    stream.Write(nameData, 0, nameData.Length);

                    outputText.AppendText(Environment.NewLine + "Connected to server! Type messages (exit to quit):");
                    isConnected = true;

                    sendmessage = stream;
                }
                catch (SocketException ex)
                {
                    outputText.AppendText(Environment.NewLine + $"Connection error: {ex.Message}");
                    outputText.AppendText(Environment.NewLine + $"Error code: {ex.ErrorCode}");
                }
                catch (Exception ex)
                {
                    outputText.AppendText(Environment.NewLine + $"Client error: {ex.GetType().Name}");
                    outputText.AppendText(Environment.NewLine + $"Error details: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                outputText.AppendText(Environment.NewLine + "Error: " + ex.Message);
            }
        }

        static void ReceiveMessages(NetworkStream stream)
        {
            byte[] buffer = new byte[1024];

            while (true)
            {
                try
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    Console.WriteLine(message);
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"Receive error: {ex.Message}");
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Message receive error: {ex.Message}");
                    break;
                }
            }
        }

        public static void SendMessages(NetworkStream stream, string message, ref TextBox outputText)
        {
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(message);
                stream.Write(data, 0, data.Length);
            }
            catch (IOException ex)
            {
                outputText.AppendText(Environment.NewLine + $"Send error: {ex.Message}");
            }
            catch (Exception ex)
            {
                outputText.AppendText(Environment.NewLine + $"Message send error: {ex.Message}");
            }
        }
    }

    class Client
    {
        public static void ClientMain()
        {
            Console.WriteLine("Chat client started");

            string clientName, serverIp = " ", s;
            int port = 8068;

            string path = @"config.txt";
            bool connectFromPath = false;

            if (File.Exists(path))
            {
                using (StreamReader sr = File.OpenText(path))
                {
                    string[] str = new string[] { sr.ReadLine(), sr.ReadLine() };
                    Console.WriteLine(string.Join("", new string[] { "connect to ", str[0], ":", str[1], " ? (y/n)" }));
                    try
                    {
                        if (Console.ReadLine().ToLower() == "y")
                        {
                            connectFromPath = true;

                            serverIp = str[0];
                            port = int.Parse(str[1]);
                        }
                    }
                    catch { connectFromPath = false; }
                }

                // Create a file to write to.

            }

            Console.Write("Enter your name: ");
            clientName = Console.ReadLine();

            if (!connectFromPath)
            {
                Console.Write("Enter server IP address: ");
                serverIp = Console.ReadLine();

                Console.Write("Enter port (default 8068): ");
                string portInput = Console.ReadLine();

                port = string.IsNullOrEmpty(portInput) ? 8068 : int.Parse(portInput);
                Console.WriteLine("save this connection? (y/n)");
                if (Console.ReadLine() == "y")
                {
                    using (StreamWriter sw = File.CreateText(path))
                    {
                        sw.WriteLine(serverIp);
                        sw.WriteLine(port.ToString());
                    }
                    Console.WriteLine("config saved");
                }
            }

            TcpClient client = new TcpClient();

            try
            {
                Console.WriteLine($"Connecting to {serverIp}:{port}...");
                client.Connect(serverIp, port);

                // First send our name to the server
                NetworkStream stream = client.GetStream();
                byte[] nameData = Encoding.UTF8.GetBytes(clientName);
                stream.Write(nameData, 0, nameData.Length);

                Console.WriteLine("Connected to server! Type messages (exit to quit):");

                var receiveThread = new System.Threading.Thread(() => ReceiveMessages(stream));
                receiveThread.Start();

                SendMessages(stream);
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"Connection error: {ex.Message}");
                Console.WriteLine($"Error code: {ex.ErrorCode}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Client error: {ex.GetType().Name}");
                Console.WriteLine($"Error details: {ex.Message}");
            }
            finally
            {
                client.Close();
            }

            Console.WriteLine("Client disconnected. Press any key...");
            Console.ReadKey();
        }

        static void ReceiveMessages(NetworkStream stream)
        {
            byte[] buffer = new byte[1024];

            while (true)
            {
                try
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine(message);
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"Receive error: {ex.Message}");
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Message receive error: {ex.Message}");
                    break;
                }
            }
        }

        static void SendMessages(NetworkStream stream)
        {
            while (true)
            {
                try
                {
                    string message = Console.ReadLine();
                    if (message.ToLower() == "exit")
                        break;

                    byte[] data = Encoding.UTF8.GetBytes(message);
                    stream.Write(data, 0, data.Length);
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"Send error: {ex.Message}");
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Message send error: {ex.Message}");
                    break;
                }
            }
        }
    }
}
