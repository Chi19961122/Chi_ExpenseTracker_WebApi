using AutoMapper;
using Chi.ExpenseTracker.Api.Models.Parameters;
using Chi.ExpenseTracker.Api.Models.ViewModels;
using Chi.ExpenseTracker.Common.Models.InfoModels;
using Chi.ExpenseTracker.Services.Auth;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chi.ExpenseTracker.Api.Controllers;

/// <summary>
/// 認證 API（註冊、登入、換發權杖）。
/// </summary>
[Route("api/[controller]")]
public class AuthController : ApiControllerBase
{
    private readonly IAuthService _authService;
    private readonly IMapper _mapper;
    private readonly IValidator<RegisterParameter> _registerValidator;
    private readonly IValidator<LoginParameter> _loginValidator;

    /// <summary>
    /// 建立認證 Controller。
    /// </summary>
    public AuthController(
        IAuthService authService,
        IMapper mapper,
        IValidator<RegisterParameter> registerValidator,
        IValidator<LoginParameter> loginValidator)
    {
        _authService = authService;
        _mapper = mapper;
        _registerValidator = registerValidator;
        _loginValidator = loginValidator;
    }

    /// <summary>
    /// 註冊新使用者。
    /// </summary>
    /// <param name="parameter">註冊參數。</param>
    /// <param name="cancellationToken">取消權杖。</param>
    /// <returns>建立的使用者（不含密碼）。</returns>
    /// <response code="201">註冊成功。</response>
    /// <response code="400">參數驗證失敗。</response>
    /// <response code="409">電子郵件已被註冊。</response>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(UserViewModel), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UserViewModel>> RegisterAsync(
        [FromBody] RegisterParameter parameter,
        CancellationToken cancellationToken)
    {
        var validation = await _registerValidator.ValidateAsync(parameter, cancellationToken);
        if (validation.IsValid is false)
        {
            return ValidationProblemFrom(validation);
        }

        var info = _mapper.Map<RegisterInfo>(parameter);
        var user = await _authService.RegisterAsync(info, cancellationToken);
        if (user is null)
        {
            return Conflict(new ProblemDetails { Title = "電子郵件已被註冊", Status = StatusCodes.Status409Conflict });
        }

        return Created($"/api/auth/register/{user.UserId}", _mapper.Map<UserViewModel>(user));
    }

    /// <summary>
    /// 以電子郵件與密碼登入，成功回傳 JWT 與使用者資訊。
    /// </summary>
    /// <param name="parameter">登入參數。</param>
    /// <param name="cancellationToken">取消權杖。</param>
    /// <returns>權杖與使用者。</returns>
    /// <response code="200">登入成功。</response>
    /// <response code="400">參數驗證失敗。</response>
    /// <response code="401">帳號或密碼錯誤。</response>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthViewModel>> LoginAsync(
        [FromBody] LoginParameter parameter,
        CancellationToken cancellationToken)
    {
        var validation = await _loginValidator.ValidateAsync(parameter, cancellationToken);
        if (validation.IsValid is false)
        {
            return ValidationProblemFrom(validation);
        }

        var result = await _authService.LoginAsync(parameter.Email, parameter.Password, cancellationToken);
        if (result is null)
        {
            return Unauthorized();
        }

        return Ok(_mapper.Map<AuthViewModel>(result));
    }

    /// <summary>
    /// 以目前有效的權杖換發新的 JWT（延長工作階段，前端於到期前呼叫）。
    /// </summary>
    /// <param name="cancellationToken">取消權杖。</param>
    /// <returns>新的權杖與使用者。</returns>
    /// <response code="200">換發成功。</response>
    /// <response code="401">權杖無效或已過期。</response>
    [HttpPost("refresh")]
    [Authorize]
    [ProducesResponseType(typeof(AuthViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthViewModel>> RefreshAsync(CancellationToken cancellationToken)
    {
        var email = CurrentUserEmail;
        if (string.IsNullOrEmpty(email))
        {
            return Unauthorized();
        }

        var result = await _authService.RefreshAsync(email, cancellationToken);
        if (result is null)
        {
            return Unauthorized();
        }

        return Ok(_mapper.Map<AuthViewModel>(result));
    }
}
