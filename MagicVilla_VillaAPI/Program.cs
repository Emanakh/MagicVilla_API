using MagicVilla_VillaAPI.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(option =>
{
	option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString"));
});

Log.Logger = new LoggerConfiguration().MinimumLevel.Information().WriteTo.File("log/villaLogs.txt", rollingInterval: RollingInterval.Day).CreateLogger();
builder.Host.UseSerilog();

builder.Services.AddControllers(option =>
{
	//option.ReturnHttpNotAcceptable = true; 
}
).AddNewtonsoftJson();
builder.Services.AddCors(options =>
{
	options.AddPolicy(name: "angular",
					  policy =>
					  {
						  policy.AllowAnyHeader();
						  policy.WithOrigins("http://localhost:4200");
						  policy.AllowAnyMethod();
					  });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseCors("angular");
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
