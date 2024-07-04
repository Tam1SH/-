using System.Security.Claims;
using back.DataLayer.Model;
using back.Domain.PersonalAccountService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace back.Controllers.PersonalAccountManager;

[ApiController]
[Route("api/[controller]")]
public class PersonalAccountManager : ControllerBase
{
	private readonly PersonalAccountService _personalAccountService;
	private readonly ApiErrorFactory<PersonalAccountManager> _errorFactory;
	private readonly ILogger<PersonalAccountManager> _logger;

	public PersonalAccountManager(
		PersonalAccountService personalAccountService,
		ILogger<PersonalAccountManager> logger,
		ApiErrorFactory<PersonalAccountManager> errorFactory
	)
	{
		_personalAccountService = personalAccountService;
		_logger = logger;
		_errorFactory = errorFactory;
	}


	[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiError))]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<PersonalAccount>))]
	[HttpGet("accounts/{userId}")]
	public async Task<ActionResult<List<PersonalAccount>>> GetPersonalAccounts(long userId)
	{
		try
		{
			var accounts = await _personalAccountService.GetPersonalsAccounts(userId);
			return Ok(accounts);
		}
		catch (Exception ex)
		{
			_logger.LogError("On getting personal accounts: {}", ex);
			return BadRequest(
				_errorFactory.Create(
					new Domain.ErrorCode(-1, "Unknown error")
				)
			);
		}
	}

	[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiError))]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
	[HttpPost("create")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	public async Task<ActionResult> CreatePersonalAccount([FromBody] CreateAccountModel model)
	{
		try
		{
			if (User.Identity is not ClaimsIdentity claimsIdentity)
				return Unauthorized();
			

			var authorizedUser = AuthorizedUser.FromIdentity(claimsIdentity);
			if (authorizedUser == null)
				return Unauthorized();
			

			await _personalAccountService.CreatePersonalAccount(authorizedUser.Id, model.Currency);

			return Ok();
		}
		catch (CannotCreateMorePersonalAccountsException)
		{
			return BadRequest(
				_errorFactory.Create(
					new Domain.ErrorCode(1, "Cannot create more personal accounts")
				)
			);
		}
		catch (Exception ex)
		{
			_logger.LogError("On create personal account: {}", ex);
			return BadRequest(
				_errorFactory.Create(
					new Domain.ErrorCode(-1, "Unknown error")
				)
			);
		}
	}

	[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiError))]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
	[HttpPost("transfer")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	public async Task<ActionResult> TransferFunds([FromBody] TransferFundsModel model)
	{
		try
		{
			if (User.Identity is not ClaimsIdentity claimsIdentity)
				return Unauthorized();
			
			var authorizedUser = AuthorizedUser.FromIdentity(claimsIdentity);
			if (authorizedUser == null)
				return Unauthorized();
			
			await _personalAccountService.TransferFunds(
				FromUserId: authorizedUser.Id, 
				fromAccountId: model.FromAccountId, 
				ToUserId: model.ToUserId, 
				toAccountId: model.ToAccountId, 
				amount: model.Amount
			);
			return Ok();
		}
		catch (AccountNotFoundException)
		{
			return BadRequest(
				_errorFactory.Create(
					new Domain.ErrorCode(2, "Account not found")
				)
			);
		}
		catch (InsufficientFundsException)
		{
			return BadRequest(
				_errorFactory.Create(
					new Domain.ErrorCode(3, "Insufficient funds")
				)
			);
		}
		catch (CurrencyConversionException)
		{
			return BadRequest(
				_errorFactory.Create(
					new Domain.ErrorCode(4, "Currency conversion error")
				)
			);
		}
		catch (Exception ex)
		{
			_logger.LogError("On transfer funds: {}", ex);
			return BadRequest(
				_errorFactory.Create(
					new Domain.ErrorCode(-1, "Unknown error")
				)
			);
		}
	}

	[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiError))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(decimal))]
    [HttpPost("convert")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult<decimal>> ConvertCurrency([FromBody] ConvertCurrencyModel model)
    {
        try
        {
            if (User.Identity is not ClaimsIdentity claimsIdentity)
                return Unauthorized();

            var authorizedUser = AuthorizedUser.FromIdentity(claimsIdentity);
            if (authorizedUser == null)
                return Unauthorized();

            var convertedAmount = await _personalAccountService.ConvertCurrency(model.AccountId, model.TargetCurrency);
            return Ok(convertedAmount);
        }
        catch (AccountNotFoundException)
        {
            return BadRequest(
                _errorFactory.Create(
                    new Domain.ErrorCode(2, "Account not found")
                )
            );
        }
        catch (CurrencyConversionException)
        {
            return BadRequest(
                _errorFactory.Create(
                    new Domain.ErrorCode(4, "Currency conversion error")
                )
            );
        }
        catch (Exception ex)
        {
            _logger.LogError("On convert currency: {}", ex);
            return BadRequest(
                _errorFactory.Create(
                    new Domain.ErrorCode(-1, "Unknown error")
                )
            );
        }
    }
	
}