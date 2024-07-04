using back.DataLayer.Model;
using Microsoft.EntityFrameworkCore;
namespace back.DataLayer.Repository
{
    public class UserRepository: BaseRepository
    {
        public UserRepository(DataBaseContext dbContext): base(dbContext) { }
        
        public DbSet<Users> Users => _dbContext.Users;

		public DbSet<Tokens> Tokens => _dbContext.Tokens;

    }
}
 