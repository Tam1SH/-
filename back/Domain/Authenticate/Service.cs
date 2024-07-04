using back.Controllers;
using back.DataLayer.Model;
using back.DataLayer.Repository;
using back.Utils;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
namespace back.Domain.Authenticate;


public class ExpiredTokenException : Exception { }

public class UserNotFoundException : Exception { }

public class UserAlreadyExistException : Exception { }

public class IncorrectCredentialsException : Exception { }


public class AuthenticateService
{
	private readonly UserRepository userRepository;
	private readonly ILogger _logger;

	public AuthenticateService(
		UserRepository userRepository,
		ILogger<AuthenticateService> logger
		)
	{
		this.userRepository = userRepository;
		_logger = logger;
	}

	public class SaltedPassword
	{
		public string SaltedHashPass { get; set; }
		public string Salt { get; set; }
	}

	static public SaltedPassword HashingPassword(string pass)
	{

		var hashPass = Cryptography.sha256(pass);

		var salt = Cryptography.sha256(
			Convert.ToBase64String(RandomNumberGenerator.GetBytes(hashPass.Length)
		));

		var saltedPass = Cryptography.sha256(hashPass + salt);

		return new()
		{
			Salt = salt,
			SaltedHashPass = saltedPass,
		};

	}

	public async Task<Users?> FindUserWithCredential(string login, string password, bool passIsSalted = false)
	{

		var nonAccessedUser = await userRepository
			.Users
			.SingleOrDefaultAsync(u => u.Login == login);
		

		if (nonAccessedUser == null) {
			return null;
		}

		var hashPass = Cryptography.sha256(password);
		var saltedPass = Cryptography.sha256(hashPass + nonAccessedUser.Salt);

		return nonAccessedUser.Password == (passIsSalted == true ? password : saltedPass)
			? nonAccessedUser
			: null;
	}

	async public Task RegisterUser(RegisterModel model)
	{

		var user = await userRepository
			.Users
			.FirstOrDefaultAsync(u => u.Login.ToLower() == model.Login.ToLower());


		if (user != null)
			throw new UserAlreadyExistException();

		var salo = HashingPassword(model.Password);

		var newUser = new Users()
		{
			Login = model.Login,
			Password = salo.SaltedHashPass,
			Salt = salo.Salt,
		};

		userRepository.Users.Add(newUser);
		await userRepository.SaveChangesAsync();

		_logger.LogInformation("new user [login: {}]", newUser.Login);

	}

    public async Task Logout(string refreshToken)
    {
        var userToken = await userRepository.Tokens.SingleOrDefaultAsync(t => t.RefreshToken == refreshToken);

        if (userToken != null)
        {
            userRepository.Tokens.Remove(userToken);
            await userRepository.SaveChangesAsync();
        }
    }
	
	public async Task<LoginResult> Login(LoginArgs args)
	{
		
		var claims = 
			await GetPrincipalFromCredential(args.Username, args.Password) 
				?? throw new IncorrectCredentialsException();
		
		var token = CreateToken(claims.Claims.ToList());
		var refreshToken = GenerateRefreshToken();

		var EntityId = claims.Claims
			.Where(claim => claim.Type == "id")
			.Select(claim => long.Parse(claim.Value))
			.FirstOrDefault();

		var tokens = new Tokens
		{
			UserId = EntityId,
			ExpiresIn = DateTime.Now.ToUniversalTime().AddDays(180),
			RefreshToken = refreshToken,
			LastActivity = DateTime.Now.ToUniversalTime(),
		};

		userRepository.Tokens.Add(tokens);
		
		await userRepository.SaveChangesAsync();

		return new LoginResult
		{
			RefreshToken = refreshToken,
			Expiration = tokens.ExpiresIn,
			Principal = claims,
			Token = new JwtSecurityTokenHandler().WriteToken(token)
		};
	}

	
	public async Task<RefreshTokenResult> RefreshToken(RefreshTokenModel model)
	{

		var userToken = 
			await userRepository.Tokens.SingleOrDefaultAsync(t => t.RefreshToken == model.RefreshToken) 
				?? throw new UserNotFoundException();
		var user = 
			await userRepository.Users.SingleOrDefaultAsync(u => u.Id == userToken.UserId) 
				?? throw new UserNotFoundException();

		if (userToken.ExpiresIn <= DateTime.Now)
		{
			throw new ExpiredTokenException();
		}
		
		userToken.LastActivity = DateTime.Now;
		var newAccessToken = CreateToken(
			AuthorizedUser.FromUser(user).GetClaims()
		);

		userRepository.Tokens.Update(userToken);
		
		await userRepository.SaveChangesAsync();

		return new RefreshTokenResult
		{
			AccessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken)
		};
	}



	private static JwtSecurityToken CreateToken(List<Claim> authClaims)
	{
		var token = new JwtSecurityToken(
			issuer: JWTCredentialsOptions.Issuer,
			audience: JWTCredentialsOptions.Audience,
			expires: DateTime.Now.AddMinutes(JWTCredentialsOptions.ExpireDuration),
			claims: authClaims,
			signingCredentials: new SigningCredentials(
				JWTCredentialsOptions.SymmetricKey, 
				SecurityAlgorithms.HmacSha256
			)
		);
		
		return token;

	}

	private async Task<ClaimsPrincipal?> GetPrincipalFromCredential(string username, string? password = null)
	{

		var user = password is null
			? await userRepository
					.Users
					.SingleOrDefaultAsync(u => u.Login == username)
			: await FindUserWithCredential(username, password);

		if (user != null)
		{
			var authorizedUser = AuthorizedUser.FromUser(user);

			var claims = authorizedUser.GetClaims();

			return new ClaimsPrincipal(
				new ClaimsIdentity(claims)
			);
		}

		return null;
	
	}

	private static string GenerateRefreshToken()
	{
		var randomNumber = new byte[64];
		using var rng = RandomNumberGenerator.Create();
		rng.GetBytes(randomNumber);
		return Convert.ToBase64String(randomNumber);
	}

}


