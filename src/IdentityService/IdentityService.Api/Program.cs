using FluentValidation;
using FluentValidation.AspNetCore;
using IdentityService.Application.Authentication.Login;
using IdentityService.Application.Abstractions;
using IdentityService.Infrastructure;
using IdentityService.Infrastructure.Persistence;
using IdentityService.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register MediatR handlers from application assembly
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(LoginCommandHandler).Assembly));

// Register FluentValidation validators
builder.Services.AddValidatorsFromAssemblyContaining<LoginCommandValidator>();
builder.Services.AddFluentValidationAutoValidation();

// Register Identity infrastructure
builder.Services.AddIdentityInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/swagger/v1/swagger.json",
            "IdentityService.Api v1");
    });

    // Seed development data
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<IdentityDbContextSeed>>();

        //await context.Database.EnsureCreatedAsync();  //creates database tables directly without using EF Core migrations
        await IdentityDbContextSeed.SeedDevelopmentDataAsync(context, passwordHasher, logger);
    }
}

// Exception middleware should be early in the pipeline
app.UseIdentityExceptionHandler();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
