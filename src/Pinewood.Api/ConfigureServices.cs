using FluentValidation;
using MediatR;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Pinewood.Api.Common.Behaviours;
using Pinewood.Api.Infrastructure.Persistence;

namespace Pinewood.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        
        services.AddMediatR(x => x.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        
        return services;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        SqliteConnection connection = new("Filename=:memory:");
        connection.Open();

        services.AddDbContextPool<DatabaseContext>(options => options
            .UseSqlite(connection)
        );
        
        return services;
    }
}