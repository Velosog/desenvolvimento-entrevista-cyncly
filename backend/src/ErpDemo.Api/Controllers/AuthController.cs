using ErpDemo.Application.DTOs;
using ErpDemo.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ErpDemo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Tags("Autenticação")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Realiza login e retorna token JWT
    /// </summary>
    /// <remarks>
    /// Usuários seed:
    /// - admin@demo.com / 123456 (Admin)
    /// - operator@demo.com / 123456 (Operator)
    /// </remarks>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request)
    {
        var result = await _authService.LoginAsync(request);
        return Ok(result);
    }
}
