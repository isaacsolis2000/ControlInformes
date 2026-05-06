using ControlInformes.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ControlInformes.API.Controllers;

[ApiController]
[Route("api/dashboard")]
public class DashboardController : ControllerBase
{
    private readonly IBusDashboard _busDashboard;

    public DashboardController(IBusDashboard busDashboard)
    {
        _busDashboard = busDashboard;
    }

    [HttpGet]
    public async Task<IActionResult> GetDashboard(
        [FromQuery] int anoServicio,
        [FromQuery] int? mes)          // ← mes ahora es opcional
    {
        var response = await _busDashboard.GetDashboardAsync(anoServicio, mes);
        return StatusCode(response.HttpCode, response);
    }
}