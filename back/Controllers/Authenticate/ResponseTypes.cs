using System.ComponentModel.DataAnnotations;

namespace back.Controllers.Authenticate
{
	public class LoginResponse
	{
		public string Token { get; set; }
		public DateTime Expiration { get; set; }
	}

	public class RefreshTokenResponse 
	{
		public string AccessToken { get; set; }
	}
}