using Microsoft.EntityFrameworkCore;
using ProjectWithASPNET8.Model.Context;
using ProjectWithASPNET8.Business;
using ProjectWithASPNET8.Business.Implementations;
using ProjectWithASPNET8.Repository;
using Serilog;
using Microsoft.AspNetCore.Components.Routing;
using MySqlConnector;
using EvolveDb;
using ProjectWithASPNET8.Repository.Generic;
using Microsoft.Net.Http.Headers;
using ProjectWithASPNET8.Hypermidia.Filters;
using ProjectWithASPNET8.Hypermidia.Enricher;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Rewrite;
using ProjectWithASPNET8.Services;
using ProjectWithASPNET8.Services.Implementations;
using ProjectWithASPNET8.Configurations;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);
var appName = "REST API's RESTFull from 0 to Azure with ASP.NET Core 8 and Docker";
var appVersion = "v1";
var appDescription = "API RESTFull developed in course";

// Add services to the container.
builder.Services.AddRouting(options => options.LowercaseUrls = true);

var tokenConfigurations = new TokenConfiguration();

new ConfigureFromConfigurationOptions<TokenConfiguration>(
        builder.Configuration.GetSection("TokenConfigurations")
    )
    .Configure(tokenConfigurations);

//Configurando apenas um instancia por vez
builder.Services.AddSingleton(tokenConfigurations);

//Definindo os parametros de autenticação
builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,
			ValidIssuer = tokenConfigurations.Issuer,
			ValidAudience = tokenConfigurations.Audience,
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenConfigurations.Secret)),
		};
	});

builder.Services.AddAuthorization(auth =>
{
	auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
		.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
		.RequireAuthenticatedUser().Build());
});

//Add Cors
builder.Services.AddCors(options => options.AddDefaultPolicy(builder =>
{
	builder.AllowAnyOrigin()
	.AllowAnyMethod()
	.AllowAnyHeader();
}));

builder.Services.AddControllers();

//Configuração Swagger
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc(appVersion,
		new OpenApiInfo
		{
			Title = appName,
			Version = appVersion,
			Description = appDescription,
			Contact = new OpenApiContact
			{
				Name = "Vinicius Costa",
				Url = new Uri("https://github.com/ViniciusOrram/ProjectWithASPNET8")
			}
		});
});



//Connection MySql
var connection = builder.Configuration["MySqlConnection:MySqlConnectionString"];
builder.Services.AddDbContext<MySqlContext>(options => options.UseMySql(
    connection,
    new MySqlServerVersion(new Version(8, 0, 36)))
);

if (builder.Environment.IsDevelopment())
{
    MigrateDatabase(connection);
}

builder.Services.AddMvc( options =>
{
	options.RespectBrowserAcceptHeader = true;
	options.FormatterMappings.SetMediaTypeMappingForFormat("xml", MediaTypeHeaderValue.Parse("application/xml"));
	options.FormatterMappings.SetMediaTypeMappingForFormat("json", MediaTypeHeaderValue.Parse("application/json"));
})
	.AddXmlSerializerFormatters();

var filterOption = new HyperMediaFilterOption();
filterOption.ContentResponseEnricherList.Add(new PersonEnricher());
filterOption.ContentResponseEnricherList.Add(new BookEnricher());

builder.Services.AddSingleton(filterOption);


//Versioning API
builder.Services.AddApiVersioning();

//Dependency Injection
builder.Services.AddScoped<IPersonBusiness, PersonBusinessImplementation>();

builder.Services.AddScoped<IBookBusiness, BookBusinessImplementation>();

builder.Services.AddScoped<ILoginBusiness, LoginBusinessImplementation>();

builder.Services.AddTransient<ITokenService, TokenService>();

builder.Services.AddTransient<IUserRepository, UserRepository>();

builder.Services.AddTransient<IPersonRepository, PersonRepository>();

builder.Services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

//Habilitando CORS
app.UseCors();

//Responsavel por gerar o json da nossa aplicação
app.UseSwagger();

//Responsavel por gerar a pagina html
app.UseSwaggerUI(c =>
{
	c.SwaggerEndpoint("/swagger/v1/swagger.json",
		$"{appName} - {appVersion}");
});

var option = new RewriteOptions();
option.AddRedirect("^$", "swagger");
app.UseRewriter(option);

app.UseAuthorization();

app.MapControllers();

app.MapControllerRoute("DefaultApi", "{controller=values}/v{version=apiVersion}/{id?}");

app.Run();


void MigrateDatabase(string connection)
{
	try
	{
		var evolveConnection = new MySqlConnection(connection);
		var evolve = new Evolve(evolveConnection, Log.Information)
		{
			Locations = new List<string> { "db/migrations", "db/dataset" },
			IsEraseDisabled = true,
		};
		evolve.Migrate();
	}
	catch (Exception ex) 
	{
		Log.Error("Database migration failed", ex);
		throw;
	}
}