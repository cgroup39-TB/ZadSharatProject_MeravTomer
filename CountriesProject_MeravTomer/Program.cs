using ServerSideCountriesProject_MeravTomer.DAL;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Allows the static ClientSide app (opened from a different origin, e.g. a
// local file server or Live Server port) to call this Web API.
const string ClientAppCorsPolicy = "ClientAppCorsPolicy";

builder.Services.AddCors(options =>
{
    options.AddPolicy(ClientAppCorsPolicy, policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(ClientAppCorsPolicy);

app.UseAuthorization();

app.MapControllers();

app.Run();
