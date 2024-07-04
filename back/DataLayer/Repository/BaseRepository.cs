
namespace back.DataLayer.Repository;

public abstract class BaseRepository
{
	protected readonly DataBaseContext _dbContext;
	public DataBaseContext DbContext => _dbContext;
	
	public BaseRepository(DataBaseContext dbContext) { 
		_dbContext = dbContext;
	}

	public static async Task<TResult> BeginTransaction<TResult>(DataBaseContext _dbContext, Func<Task<TResult>> action)
	{
		using var trans = await _dbContext.Database.BeginTransactionAsync();

		try
		{
			var result = await action();
			await trans.CommitAsync();
			return result;
		}
		catch
		{
			await trans.RollbackAsync();
			throw;
		}
	}
	
	public async Task SaveChangesAsync() 
	{
		await _dbContext.SaveChangesAsync();
	}
	
}


