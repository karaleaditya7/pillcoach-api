using System;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace OntrackDb.Hub
{
    public class WebSocketMiddleware
    {
        private readonly WebSocketServerConnectionaManager _manager = new WebSocketServerConnectionaManager();
        private readonly RequestDelegate _next;

        public WebSocketMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                Console.WriteLine("WebSocket Connected");
                string ConnID = _manager.AddSocket(webSocket);
                await SendConnIDAsync(webSocket, ConnID); //Call to new method here

                await ReceiveMessage(webSocket, async (result, buffer) =>
                {
                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        Console.WriteLine("Message Recieved");
                        await RouteJSONMessageAsync(Encoding.UTF8.GetString(buffer, 0, result.Count), ConnID);

                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        Console.WriteLine("Connection Message Closed");
                        return;
                    }
                });
            }

            // Call the next delegate/middleware in the pipeline
            await _next(context);


        }
        //sends connectionid back to the server
        private async Task SendConnIDAsync(WebSocket socket, string connID)
        {
            var buffer = Encoding.UTF8.GetBytes("ConnID: " + connID);
            await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
        }


        private async Task ReceiveMessage(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
        {
            var buffer = new byte[1024 * 4];

            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(buffer: new ArraySegment<byte>(buffer),
                                                                   cancellationToken: CancellationToken.None);
                handleMessage(result, buffer);
            }
        }

        private async Task RouteJSONMessageAsync(string messages, string ConnID)
        {

            //var routeOb = JsonConvert.DeserializeObject<dynamic>(message);
            string message = messages.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries).First();
            string connectId = messages.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries).Last();
            if (Guid.TryParse(message, out Guid guidOutput))
            {
                Console.WriteLine("Targeted");
                var sock = _manager.GetAllSockets().FirstOrDefault(s => s.Key == message);
                if (sock.Value != null)
                {
                    if (sock.Value.State == WebSocketState.Open)
                        await sock.Value.SendAsync(Encoding.UTF8.GetBytes(message), WebSocketMessageType.Text, true, CancellationToken.None);
                }
                else
                {
                    Console.WriteLine("Invalid Recipient");
                }
            }
            else
            {
                Console.WriteLine("Broadcast");
                foreach (var sock in _manager.GetAllSockets())
                {
                    if (sock.Key == connectId || sock.Key == ConnID)
                    {
                        if (sock.Value.State == WebSocketState.Open)
                            await sock.Value.SendAsync(Encoding.UTF8.GetBytes(message), WebSocketMessageType.Text, true, CancellationToken.None);
                    }

                }
            }
        }
    }
}
