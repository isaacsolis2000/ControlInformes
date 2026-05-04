using ControlInformes.Data.Implementations;
using ControlInformes.Data.Interfaces;
using ControlInformes.Data.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ControlInformes.Data;

public static class DependencyInjection
{
    public static IServiceCollection AddData(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IDatPublicador, DatPublicador>();
        services.AddScoped<IDatInformeMensual, DatInformeMensual>();
        services.AddScoped<IDatAsistencia, DatAsistencia>();
        services.AddScoped<IDatUsuario, DatUsuario>();
        services.AddScoped<IExcelService, ExcelService>();
        services.AddScoped<IDatGrupo, DatGrupo>();

        return services;
    }
}
