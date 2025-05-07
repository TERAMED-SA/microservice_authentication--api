using dotenv.net;
using microservice_authentication__api.src.Domain.Entities;
using microservice_authentication__api.src.Infrastructure.Config;
using microservice_authentication__api.src.Infrastructure.Data;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
DotEnv.Load();
var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddAppServices(builder.Configuration)
    .AddJwtAuth(builder.Configuration)
    .AddSwaggerWithJwt()
    .AddPolicies();

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

var app = builder.Build();

// Inicialização do banco e seed do usuário admin
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<User>>();
    await SeedData.Initialize(services, userManager);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();


app.Run();
