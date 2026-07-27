using ServerSideCountriesProject_MeravTomer.DAL;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// With <Nullable>enable</Nullable> set project-wide, [ApiController] infers
// an implicit [Required] on every non-nullable reference-type property of a
// [FromBody] model (e.g. UserVisitedCountry.ReviewText, Country.Cca3/Name/
// Region/...). Several endpoints intentionally accept partial objects
// (marking a country as visited only needs userId + country.countryId), so
// that implicit requirement rejects perfectly valid requests with a 400.
// This restores the property's actual nullability as the only source of
// truth for "is this required".
builder.Services.AddControllers(options =>
{
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
});
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
if (true )//app.Environment.IsDevelopment()
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors(ClientAppCorsPolicy);

app.UseAuthorization();

app.MapControllers();

app.Run();
