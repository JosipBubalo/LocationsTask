// See https://aka.ms/new-console-template for more information

using SignalRConsoleApp;

class Program
{
    static async Task Main(string[] args)
    {
        var SignalRSubscriber = new SignalRSubscriber("https://localhost:7188/hub");
        await SignalRSubscriber.StartAsync();

        Console.WriteLine("Press any key to stop...");
        Console.ReadKey();

        await SignalRSubscriber.StopAsync();
    }
}
