using VetFlow.Application.Repositories;
using VetFlow.Infrastructure.Persistence;
using VetFlow.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace VetFlow.API.Extensions;

public static class VetFlowServiceExtensions
{
    public static IServiceCollection AddVetFlowDbContext(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var useSqlite = configuration.GetValue<bool>("Database:UseSqlite");

        if (useSqlite)
        {
            var conn = configuration.GetConnectionString("VetFlowSqlite")
                ?? throw new InvalidOperationException("Connection string 'VetFlowSqlite' não encontrada.");
            services.AddDbContext<VetFlowContext>(o => o.UseSqlite(conn));
        }
        else
        {
            var conn = configuration.GetConnectionString("VetFlowOracle")
                ?? throw new InvalidOperationException("Connection string 'VetFlowOracle' não encontrada.");
            services.AddDbContext<VetFlowContext>(o => o.UseOracle(conn));
        }

        return services;
    }

    public static IServiceCollection AddVetFlowRepositories(this IServiceCollection services)
    {
        services.AddScoped<ITutorRepository, TutorRepository>();
        services.AddScoped<IPetRepository, PetRepository>();
        services.AddScoped<IClinicRepository, ClinicRepository>();
        services.AddScoped<IAppointmentRepository, AppointmentRepository>();
        services.AddScoped<IVaccineRepository, VaccineRepository>();
        services.AddScoped<IMedicationRepository, MedicationRepository>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        return services;
    }

    public static IServiceCollection AddVetFlowSwagger(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Title = configuration.GetValue<string>("Swagger:Title") ?? "VetFlow API",
                Version = "v1",
                Description = "API REST para gestão da jornada contínua de saúde do pet — Challenge FIAP 2026 / VetFlow."
            });

            var xml = Path.Combine(AppContext.BaseDirectory, "VetFlow.API.xml");
            if (File.Exists(xml)) options.IncludeXmlComments(xml, includeControllerXmlComments: true);
        });

        return services;
    }
}
