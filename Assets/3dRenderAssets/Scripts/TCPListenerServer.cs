using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class TCPListenerServer
{
    private TcpListener server;
    private CancellationTokenSource cancelToken;
    private readonly int tcpPort;

    public TCPListenerServer(int tcpPort)
    {
        this.tcpPort = tcpPort;
    }

    public async void Start()
    {
        cancelToken = new CancellationTokenSource();
        await StartServerAsync();
    }

    private async Task StartServerAsync()
    {
        server = new TcpListener(IPAddress.Any, tcpPort);
        server.Start();
        Debug.Log("TCPLISTENER >>> Server started on port " + tcpPort);

        while (!cancelToken.Token.IsCancellationRequested)
        {
            try
            {
                TcpClient client = await server.AcceptTcpClientAsync();
                _ = HandleClientAsync(client);
            }
            catch (ObjectDisposedException)
            {
                Debug.Log("Server Stopped !!");
                break;
            }
        }
    }

    private async Task HandleClientAsync(TcpClient client)
    {
        using (NetworkStream stream = client.GetStream())
        {
            byte[] buffer = new byte[1024];
            int bytesRead;

            while (!cancelToken.Token.IsCancellationRequested && (bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancelToken.Token)) > 0)
            {
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Debug.Log("Received: " + message);

                // Process the message
                //message = message.Replace("\\", "\\\\");
                string[] args = Regex.Matches(message, @"[\""].+?[\""]|[^ ]+")
                    .Cast<Match>()
                    .Select(m => m.Value)
                    .ToArray();

                Debug.Log("Converted: " + string.Join("; ", args));
                ((InitApp)GameObject.Find("InitApp").GetComponent<InitApp>()).Reload(args);

                // Echo the message back to the client
                byte[] responseBuffer = Encoding.UTF8.GetBytes("Received: " + message);
                await stream.WriteAsync(responseBuffer, 0, responseBuffer.Length, cancelToken.Token);
            }
        }
        client.Close();
    }

    public void Destroy()
    {
        Debug.Log("Cancelling");
        cancelToken.Cancel();
        server.Stop();
    }
}