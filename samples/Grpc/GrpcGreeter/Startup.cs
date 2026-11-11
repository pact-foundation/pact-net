using GrpcGreeter.Services;

namespace GrpcGreeter;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        this.Configuration = configuration;
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        // Add services to the container.
        services.AddGrpc();
        services.AddGrpcReflection();

    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseRouting();
        app.UseEndpoints(b =>
        {
            // Configure the HTTP request pipeline.
            b.MapGrpcService<GreeterService>();
            b.MapGet("/",
                () =>
                    "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
            b.MapGrpcReflectionService();
        });
    }
}
