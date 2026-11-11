using Grpc.Net.Client;
using GrpcGreeterClient;

public class GreeterClientWrapper
{
    private readonly Greeter.GreeterClient _client;

    public GreeterClientWrapper(string url)
    {
        var channel = GrpcChannel.ForAddress(url);
        _client = new Greeter.GreeterClient(channel);
    }

    public async Task<string> SayHello(string name)
    {
        var reply = await _client.SayHelloAsync(new HelloRequest { Name = name });
        return reply.Message;
    }
}

public class Program
{
    public static async Task Main(string[] args)
    {
        var client = new GreeterClientWrapper("http://localhost:5099");
        // var client = new GreeterClientWrapper("https://localhost:5099");
        var greeting = await client.SayHello("GreeterClient");
        Console.WriteLine("Greeting: " + greeting);
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}
