var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(option =>
{
	option.ReturnHttpNotAcceptable = true; //controllers will return a 406 Not Acceptable HTTP status code if the client's requested media type is not supported by the API, and it will only allow responses in the JSON format.
}
).AddNewtonsoftJson(); //add newtownsoft json to handle patch requests
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
