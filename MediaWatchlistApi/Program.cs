using Scalar.AspNetCore;
using MicroElements.AspNetCore.OpenApi.FluentValidation;
using Microsoft.EntityFrameworkCore;
using MediaWatchlistApi.Application;
using MediaWatchlistApi.Endpoints;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
builder.Services.AddAppDbContext(configuration)
    .AddServices()
    .AddAuthorization()
    .AddValidators();

builder.Services.AddOpenApi();
// 2. Register FluentValidation rules for OpenAPI
builder.Services.AddFluentValidationRulesToOpenApi();
// 3. Add built-in OpenAPI with the FluentValidation schema transformer
builder.Services.AddOpenApi(options =>
{
    options.AddFluentValidationRules();
});

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}
app.MapGet("/", () => Results.Redirect("/api/movies/"));
app.MapMoviesEndpoints();
app.UseHttpsRedirection();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.MigrateAsync();
}

app.Run();