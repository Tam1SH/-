using back.DataLayer.Model;
using Microsoft.EntityFrameworkCore;

namespace back.DataLayer.Repository;

public class TransactionHistoryRepository : BaseRepository
{
	public TransactionHistoryRepository(DataBaseContext dbContext) : base(dbContext) { }

	public DbSet<TransactionHistory> TransactionHistories => _dbContext.TransactionHistories;
}
