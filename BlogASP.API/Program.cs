using BlogASP.API.Helpers;
using BlogASP.API.Repository.Implements;
using BlogASP.API.Repository.Interfaces;
using BlogASP.API.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// JWT configuration
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
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
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]))
    };
});
builder.Services.AddAuthorization();

//Bind mongodb Settings
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection(nameof(MongoDbSettings)));

// Add services to the container.
// Add the Repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPostRepository, PostRepository>();
// Add PasswordHasherHelper to DI container
builder.Services.AddTransient<PasswordHasherHelper>();
builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        // Specify Swagger JSON file location and title
        options.SwaggerEndpoint("/openapi/v1.json", "OpenAPI V1");
        // Set Swagger UI to load automatically on the base URL (e.g., https://localhost:5001/)
        options.RoutePrefix = string.Empty; // This makes Swagger UI available at the root (e.g., https://localhost:5001/)
    });

}

app.UseHttpsRedirection();

app.UseAuthentication();  // Enable JWT authentication
app.UseAuthorization();

app.MapControllers();

app.Run();
