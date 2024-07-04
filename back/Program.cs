using back.DataLayer.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SpaServices.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.FileProviders;
using back.Controllers;
using back.Domain.Authenticate;
using Swashbuckle.AspNetCore.SwaggerGen;
using back.Domain.PersonalAccountService;
using back.Domain.Report;
using Microsoft.OpenApi.Models;
using back.Utils;
var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
});

builder.Services.AddControllers();
builder.Services.AddScoped<DataBaseContext>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<PersonalAccountRepository>();
builder.Services.AddScoped<TransactionHistoryRepository>();
builder.Services.AddScoped<AuthenticateService>();
builder.Services.AddScoped<PersonalAccountService>();
builder.Services.AddScoped<ReportService>();
builder.Services.AddScoped(typeof(ApiErrorFactory<>));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument();
builder.Services.AddSwaggerGen(c =>
{
    c.CustomOperationIds(apiDesc =>
    {
        return apiDesc.TryGetMethodInfo(out var methodInfo) ? methodInfo.Name : null;
    });

	c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
			Array.Empty<string>()
		}
    });
	
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = JWTCredentialsOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = JWTCredentialsOptions.Audience,
            ValidateLifetime = true,
            IssuerSigningKey = JWTCredentialsOptions.SymmetricKey,
            ValidateIssuerSigningKey = true
        };
		options.Events = new JwtBearerEvents
		{
			OnAuthenticationFailed = context =>
			{
				Console.WriteLine("Authentication failed: " + context.Exception.Message);
				return Task.CompletedTask;
			},
			OnTokenValidated = context =>
			{
				Console.WriteLine("Token validated: " + context.SecurityToken);
				return Task.CompletedTask;
			}
		};

    });

builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder(new string[]
    {
        JwtBearerDefaults.AuthenticationScheme
    })
    .RequireAuthenticatedUser()
    .Build();

});


builder.Services.AddDbContext<DataBaseContext>(options =>
    options.UseNpgsql(builder.Configuration["connectionDb"]), ServiceLifetime.Scoped);

builder.Services.AddSpaStaticFiles(configuration =>
{
    configuration.RootPath = "ClientAppDist";
});

var app = builder.Build();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "ClientAppDist")),
    RequestPath = ""
});

if (app.Environment.IsDevelopment())
{
   	app.UseSwagger();
    app.UseSwaggerUi(); 
}

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.UseSpaStaticFiles();
app.UseSpa(spa =>
{
    spa.Options.SourcePath = "ClientAppDist";

    if (app.Environment.IsDevelopment())
    {
        // Проксирует запросы, которые не метчаться самим asp.net, в dev-контейнер vue.
        spa.UseProxyToSpaDevelopmentServer("http://front:5173");
    }
});

app.Run();
