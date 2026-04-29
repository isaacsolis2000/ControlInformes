using ControlInformes.Application.Features.Excel;
using ControlInformes.Domain.Interfaces;
using ControlInformes.Infrastructure.Persistence;
using ControlInformes.Infrastructure.Repositories;
using ControlInformes.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ControlInformes.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IExcelService, ExcelService>();

        return services;
    }
}
