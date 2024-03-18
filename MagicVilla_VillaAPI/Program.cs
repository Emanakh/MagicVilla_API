using MagicVilla_VillaAPI;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Repository.IRepository;
using MagicVilla_VillaAPI.Repository.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(option =>
{
	option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString"));
});

builder.Services.AddAuthentication(
	options =>
	{
		//install bearer package
		options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
		options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
	}
	).AddJwtBearer(options =>
	{
		options.RequireHttpsMetadata = false;
		// By setting it to false, you allow JWT authentication to work over HTTP during development, for example, but it's important to ensure HTTPS in production for security reasons.
		options.SaveToken = true;
		//By setting SaveToken to true, the JWT bearer authentication handler will store the token after successfully validating it. This can be useful if you need access to the token later in your application's code, for example, to extract additional information from it or to perform custom validation. //Once the token is validated, it will be available within the HttpContext for the duration of the request. You can access it using HttpContext.Request.Headers["Authorization"] or through the HttpContext.GetTokenAsync() method. By default, the token is stored in the AuthenticationProperties.Items dictionary under the key "access_token".
		var secretKey = builder.Configuration.GetValue<string>("ApiSettings:secret");
		var secretKeyInBytes = Encoding.ASCII.GetBytes(secretKey);
		var key = new SymmetricSecurityKey(secretKeyInBytes);


		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuerSigningKey = true, //??
			IssuerSigningKey = key,
			ValidateIssuer = false,
			ValidateAudience = false

		};



	});

builder.Services.AddScoped<IUserRepo, UserRepo>();
builder.Services.AddScoped<APIResponse, APIResponse>();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
	.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddScoped<IVillaRepository, VillaRepository>();
builder.Services.AddScoped<IVillaNumberRepository, VillaNumberRepository>();
builder.Services.AddAutoMapper(typeof(MappingConfig));

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
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(options =>
{
	options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		Description =
		   "JWT Authorization header using the Bearer scheme. \r\n\r\n " +
		   "Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\n" +
		   "Example: \"Bearer 12345abcdef\"",
		Name = "Authorization",
		In = ParameterLocation.Header,
		Scheme = "Bearer"
	});
	options.AddSecurityRequirement(new OpenApiSecurityRequirement()
	{
		{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference
							{
								Type = ReferenceType.SecurityScheme,
								Id = "Bearer"
							},
				Scheme = "oauth2",
				Name = "Bearer",
				In = ParameterLocation.Header
			},
			new List<string>()
		}
	});


});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseCors("angular");
app.UseHttpsRedirection();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
