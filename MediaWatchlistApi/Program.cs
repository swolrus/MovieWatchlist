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
    .AddValidators()
    .AddHasher(builder);

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
app.MapMoviesEndpoints();
app.MapUserEndpoints();
app.UseHttpsRedirection();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.MigrateAsync();
}

app.Run();