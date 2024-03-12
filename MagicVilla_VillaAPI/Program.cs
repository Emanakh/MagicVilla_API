var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
	options.AddPolicy(name: "angular",
					  policy =>
					  {
						  policy.AllowAnyHeader();
						  //policy.AllowAnyOrigin();

						  policy.WithOrigins("http://localhost:4200"); //angular origin 
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
