using back.DataLayer.Model;
using back.DataLayer.Repository;
using Microsoft.EntityFrameworkCore;

namespace back.Domain.Report;

public class ReportService
{
	private readonly TransactionHistoryRepository _transactionHistoryRepository;

	public ReportService(TransactionHistoryRepository transactionHistoryRepository)
	{
		_transactionHistoryRepository = transactionHistoryRepository;
	}

	public Task<List<TransactionHistory>> GetTransactionHistory(long userId, DateTime? fromDate = null, DateTime? toDate = null, string? currency = null, long? accountId = null)
	{
		var query = _transactionHistoryRepository
			.TransactionHistories
			.AsQueryable()
			.Where(t => t.ToUserId == userId || t.FromUserId == userId);

		if (fromDate.HasValue)
		{
			query = query.Where(t => t.TransactionDate >= fromDate.Value);
		}

		if (toDate.HasValue)
		{
			query = query.Where(t => t.TransactionDate <= toDate.Value);
		}

		if (!string.IsNullOrEmpty(currency))
		{
			query = query.Where(t => t.Currency == currency);
		}

		if (accountId.HasValue)
		{
			query = query.Where(t => t.FromAccountId == accountId.Value || t.ToAccountId == accountId.Value);
		}
		

		return query.ToListAsync();
	}

}
