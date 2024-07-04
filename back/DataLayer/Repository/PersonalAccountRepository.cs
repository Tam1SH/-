using back.DataLayer.Model;
using Microsoft.EntityFrameworkCore;
namespace back.DataLayer.Repository
{
	public class PersonalAccountRepository: BaseRepository
	{
		public PersonalAccountRepository(DataBaseContext dbContext): base(dbContext) { }
		
		public DbSet<PersonalAccount> PersonalAccounts => _dbContext.PersonalAccounts;
		public DbSet<CurrencyExchange> CurrencyExchanges => _dbContext.CurrencyExchanges;
		
	}
}
 