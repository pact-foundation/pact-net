using System;
using System.Threading;
using System.Threading.Tasks;

namespace Provider
{
    public static class Program
    {
        public static async Task Main()
        {
            var generator = new StockEventGenerator(new StockEventSender());
            var cts = new CancellationTokenSource();

            Task eventLoop = Task.Run(async () =>
            {
                while (!cts.IsCancellationRequested)
                {
                    Console.WriteLine("Publishing events...");

                    await generator.GenerateEventsAsync();
                    await Task.Delay(3000, cts.Token);
                }

                Console.WriteLine("Finished publishing events");
            }, cts.Token);

            Console.WriteLine("Press Ctrl+C to exit");
            Console.ReadLine();

            cts.Cancel(false);
            await eventLoop;

            Console.WriteLine("Successfully exited");
        }
    }
}
