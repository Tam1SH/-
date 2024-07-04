
namespace back.Controllers.Authenticate;

public class LoginModel
{
	public string Login { get; set; }
	public string Password { get; set; }
}

public class RegisterModel
{
	// [StringLength(30)]
	// [MinLength(4)]
	// [RegularExpression(@"^[a-zA-Z0-9]+$")]
	public string Login { get; set; }
	public string Password { get; set; }
}
