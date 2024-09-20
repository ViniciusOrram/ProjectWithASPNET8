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

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

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

builder.Services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

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