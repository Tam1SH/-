using back.DataLayer.Model;
using Microsoft.EntityFrameworkCore;
namespace back.DataLayer.Repository;

public class DataBaseContext : DbContext
{
	public DbSet<Users> Users { get; set; }
	public DbSet<Tokens> Tokens { get; set; }
	public DbSet<PersonalAccount> PersonalAccounts { get; set; }
	public DbSet<CurrencyExchange> CurrencyExchanges  { get; set; }
	public DbSet<TransactionHistory> TransactionHistories { get; set; }
	public DataBaseContext(DbContextOptions<DataBaseContext> options) : base(options) {
		AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
	}
}

