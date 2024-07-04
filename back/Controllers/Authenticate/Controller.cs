using back.Domain.Authenticate;
using Microsoft.AspNetCore.Mvc;

namespace back.Controllers.Authenticate;

[ApiController]
[Route("api/[controller]")]
public class AuthenticateController : ControllerBase
{
	private readonly AuthenticateService _authenticationService;
	private readonly ApiErrorFactory<AuthenticateController> _errorFactory;
	private readonly ILogger<AuthenticateController> _logger;

	public AuthenticateController(
		AuthenticateService authenticationService,
		ILogger<AuthenticateController> logger,
		ApiErrorFactory<AuthenticateController> errorFactory
	)
	{
		_errorFactory = errorFactory;
		_authenticationService = authenticationService;
		_logger = logger;
	}

	[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiError))]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
	[HttpPost("login")]
	public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginModel model)
	{
		try
		{
			var result = await _authenticationService.Login(new LoginArgs
			{
				Password = model.Password,
				Username = model.Login,
			});

			var cookieKey = "refreshToken";

			Response.Cookies.Append(cookieKey, value: result.RefreshToken, 
				new CookieOptions {
					MaxAge = result.Expiration - DateTime.Now, 
					HttpOnly = true, 
					Secure = true, 
					SameSite = SameSiteMode.Strict
			});

			return Ok(new LoginResponse
			{
				Token = result.Token,
				Expiration = result.Expiration
			});
		}
		catch(IncorrectCredentialsException) 
		{
			return BadRequest(
				_errorFactory.Create(
					new Domain.ErrorCode(1, "Incorrect credentials")
				)
			);
		}
		catch(Exception ex)
		{
			_logger.LogError("on login: {}", ex);
			return BadRequest(
				_errorFactory.Create(
					new Domain.ErrorCode(-1, "Unknown error")
				)
			);
		}
	}
	
	
	[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiError))]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
	[HttpPost("register")]
	public async Task<ActionResult> Register([FromBody] RegisterModel model)
	{
		try
		{	
			await _authenticationService.RegisterUser(new Domain.Authenticate.RegisterModel {
				Login = model.Login,
				Password = model.Password
			});

			return Ok();
		}
		catch(UserAlreadyExistException)
		{
			return BadRequest(
				_errorFactory.Create(
					new Domain.ErrorCode(2, "User already exist")
				)
			);
		}

	}
	

	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
	[HttpPost("logout")]
	public async Task<ActionResult> Logout()
	{
		try
		{
			var refreshToken = HttpContext.Request.Cookies["refreshToken"];

			if (string.IsNullOrEmpty(refreshToken))
			{
				return BadRequest(
					_errorFactory.Create(
						new Domain.ErrorCode(5, "Refresh token not found")
					)
				);
			}

			await _authenticationService.Logout(refreshToken);

			Response.Cookies.Delete("refreshToken");

			return Ok();
		}
		catch (Exception ex)
		{
			_logger.LogError("On logout: {}", ex);
			return BadRequest(
				_errorFactory.Create(
					new Domain.ErrorCode(-1, "Unknown error")
				)
			);
		}
	}

	[HttpPost("refresh-token")]
	public async Task<ActionResult<RefreshTokenResponse>> RefreshToken()
	{
		var refreshToken = HttpContext.Request.Cookies["refreshToken"];

		var result = await _authenticationService.RefreshToken(new RefreshTokenModel { RefreshToken = refreshToken });

		return Ok(new RefreshTokenResponse
		{
			AccessToken = result.AccessToken
		});
	}


}