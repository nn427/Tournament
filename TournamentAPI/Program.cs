using Microsoft.EntityFrameworkCore;
using TournamentAPI.Models;
using TournamentAPI.Profiles;
using TournamentAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<LocalDBContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("WebDatabase")));
builder.Services.AddAutoMapper(typeof(TournamentProfile));
builder.Services.AddScoped<TournamentService>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
