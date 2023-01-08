using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace IPTV
{
    class Program
    {
        static void Main(string[] args)
        {
            // Set the IP address and port for the IPTV channel
            IPAddress ip = IPAddress.Parse("0.0.0.0");
            int port = 1234;

            // Create a new TcpListener and start listening for incoming connections
            TcpListener listener = new TcpListener(ip, port);
            listener.Start();

            Console.WriteLine("IPTV channel is running on " + ip + ":" + port);

            // Keep listening for incoming connections
            while (true)
            {
                // Accept an incoming connection
                TcpClient client = listener.AcceptTcpClient();

                // Get the network stream from the client
                NetworkStream stream = client.GetStream();

                // Read the file path from the stream
                byte[] filePathBytes = new byte[4096];
                int bytesRead = stream.Read(filePathBytes, 0, 4096);
                string filePath = System.Text.Encoding.UTF8.GetString(filePathBytes, 0, bytesRead);

                // Check if the file exists
                if (!File.Exists(filePath))
                {
                    // If the file does not exist, send an error message to the client
                    byte[] errorMessage = System.Text.Encoding.UTF8.GetBytes("Error: file does not exist");
                    stream.Write(errorMessage, 0, errorMessage.Length);
                }
                else
                {
                    // If the file exists, open the file and send its contents to the client
                    FileStream fileStream = File.OpenRead(filePath);
                    byte[] fileContents = new byte[fileStream.Length];
                    fileStream.Read(fileContents, 0, (int)fileStream.Length);
                    stream.Write(fileContents, 0, fileContents.Length);

                    // Close the file stream
                    fileStream.Close();
                }

                // Close the stream and client
                stream.Close();
                client.Close();
            }
        }
    }
}
