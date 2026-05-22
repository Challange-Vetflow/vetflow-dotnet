using VetFlow.API.Extensions;
using VetFlow.Infrastructure.Persistence;

namespace VetFlow.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddVetFlowDbContext(builder.Configuration);
        builder.Services.AddVetFlowRepositories();
        builder.Services.AddVetFlowSwagger(builder.Configuration);
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddControllers();

        var app = builder.Build();

        if (app.Environment.IsDevelopment() && builder.Configuration.GetValue<bool>("Database:UseSqlite"))
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<VetFlowContext>();
            db.Database.EnsureCreated();
        }

        app.UseSwagger();
        app.UseSwaggerUI(o =>
        {
            o.SwaggerEndpoint("/swagger/v1/swagger.json", "VetFlow API v1");
            o.RoutePrefix = string.Empty; // Swagger na raiz
        });

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}
