using GrpcGreeter.Services;

public class GrpcGreeterService
{
    public static async Task Main(string[] args)
    {
        await RunApp(args, CancellationToken.None);
    }

    public static async Task RunApp(string[] args, CancellationToken cancellationToken)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddGrpc();
        builder.Services.AddGrpcReflection();


        var app = builder.Build();

        // Configure the HTTP request pipeline.
        app.MapGrpcService<GreeterService>();
        app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
        app.MapGrpcReflectionService();

        var cancellationTokenSource = new CancellationTokenSource();
        var periodicCancellationTask = PeriodicallyCheckCancellation(cancellationToken, cancellationTokenSource);

        await app.RunAsync();

        cancellationTokenSource.Cancel();
        await periodicCancellationTask;
    }

    private static async Task PeriodicallyCheckCancellation(CancellationToken cancellationToken, CancellationTokenSource periodicCancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            if (periodicCancellationToken.Token.IsCancellationRequested)
            {
                periodicCancellationToken.Cancel();
                break;
            }

            await Task.Delay(1000); // Check for cancellation every second
        }
    }
}
