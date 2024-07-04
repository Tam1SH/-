
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace back.Utils 
{
	public class JWTCredentialsOptions 
	{
		//In minutes
		public const ulong ExpireDuration = 60;
		public const string Issuer = "SomeoneIssuer";
		public const string Audience = "SomeoneAudience";
		static public SymmetricSecurityKey SymmetricKey => new SymmetricSecurityKey(
			Encoding.UTF8.GetBytes("SomeTooSecretKeyAAAAAAAAAAAAAAAAAAAA")
		);
	}
}