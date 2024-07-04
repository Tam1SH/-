using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;

namespace back.DataLayer.Model 
{
	[NotMapped]
	public class AuthorizedUser 
	{
		public long Id { get; set; }
		public string Login { get; set; }
		
		public static AuthorizedUser FromUser(Users user) 
		{
			return new() 
			{
				Id = user.Id, 
				Login = user.Login
			};
		}
		public static AuthorizedUser? FromIdentity(ClaimsIdentity claimsIdentity)
        {
            var login = claimsIdentity.FindFirst("login");
            var id = claimsIdentity.FindFirst("id");
            if (login != null && id != null) {
                return new()
                {
                    Id = long.Parse(id.Value),
                    Login = login.Value
                };
            }        
            return null;
        }

		public List<Claim> GetClaims() {
			return new() {
				new ("id", Id.ToString()),
				new ("login", Login),
			};	
		}

	}
}