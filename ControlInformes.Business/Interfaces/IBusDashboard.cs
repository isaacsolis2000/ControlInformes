using ControlInformes.Business.DTOs;
using ControlInformes.Utils;

namespace ControlInformes.Business.Interfaces;

public interface IBusDashboard
{
    Task<ApiResponse<DashboardDto>> GetDashboardAsync(int ano, int mes);
}