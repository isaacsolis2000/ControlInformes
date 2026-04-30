using ControlInformes.Business.DTOs;
using ControlInformes.Utils;

namespace ControlInformes.Business.Interfaces;

public interface IBusAuth
{
    Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto dto);
}
