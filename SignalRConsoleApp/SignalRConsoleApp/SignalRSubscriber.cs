using LocationsAPI.Models;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SignalRConsoleApp
{
    public class SignalRSubscriber
    {
        private readonly HubConnection _connection;

        public SignalRSubscriber(string hubUrl)
        {
            _connection = new HubConnectionBuilder()
                .WithUrl(hubUrl)
                .Build();
        }

        public async Task StartAsync()
        {
            _connection.On<LocationRequest>("ReceiveRequest", HandleRequest);
            await _connection.StartAsync();
            Console.WriteLine("SignalR connection started.");
        }

        public async Task StopAsync()
        {
            await _connection.StopAsync();
            Console.WriteLine("SignalR connection stopped.");
        }

        private void HandleRequest(LocationRequest request)
        {
            Console.WriteLine("Received new request: " + JsonSerializer.Serialize(request));
        }
    }
}
