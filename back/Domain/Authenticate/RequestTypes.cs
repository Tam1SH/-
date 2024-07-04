using System.ComponentModel.DataAnnotations;

namespace back.Domain.Authenticate
{
	public class LoginArgs
	{
		public string Username { get; set; }
		public string Password { get; set; }
	}

	public class RefreshTokenModel
	{
		public string? RefreshToken  { get; set; }
	};

	public class RegisterModel
	{
		[StringLength(30)]
		[MinLength(4)]
		[RegularExpression(@"^[a-zA-Z0-9]+$")]
		public string Login { get; set; }
		public string Password { get; set; }
	}
	
}