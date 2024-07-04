using System.Security.Claims;

namespace back.Domain.Authenticate
{
	public class LoginResult
	{
		public string Token { get; set; }
		public string RefreshToken { get; set; }
		public DateTime Expiration { get; set; }
		public ClaimsPrincipal Principal { get; set; }
	}
	
	public class RefreshTokenResult
	{
		public string AccessToken { get; set; }
	};

}