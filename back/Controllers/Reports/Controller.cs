
using System.Security.Claims;
using back.DataLayer.Model;
using back.Domain.Report;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace back.Controllers.Reports;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
	private readonly ReportService _reportService;

	public ReportsController(ReportService reportService)
	{
		_reportService = reportService;
	}

	[HttpPost("transaction-history")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	public async Task<ActionResult<List<TransactionHistoryResponse>>> GetTransactionHistory([FromBody] GetTransactionHistoryModel model)
	{
		if (User.Identity is not ClaimsIdentity claimsIdentity)
			return Unauthorized();
		
		var authorizedUser = AuthorizedUser.FromIdentity(claimsIdentity);
		if (authorizedUser == null)
			return Unauthorized();

		var history = await _reportService.GetTransactionHistory(authorizedUser.Id, model.FromDate, model.ToDate, model.Currency, model.AccountId);
		
		return Ok(history.Select(h => new TransactionHistoryResponse {
			Amount = h.Amount,
			Currency = h.Currency,
			FromAccountId = h.FromAccountId,
			ToAccountId = h.ToAccountId,
			TransactionDate = h.TransactionDate,
			ReceiverUserId = h.ToUserId,
		}));
	}
}