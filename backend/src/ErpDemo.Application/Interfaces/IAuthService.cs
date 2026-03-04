using ErpDemo.Application.DTOs;

namespace ErpDemo.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
}
