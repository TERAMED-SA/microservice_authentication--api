using dotenv.net;
using microservice_authentication__api.src.Domain.Entities;
using microservice_authentication__api.src.Infrastructure.Config;
using microservice_authentication__api.src.Infrastructure.Persistence.Data;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
DotEnv.Load();
var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddAppServices(builder.Configuration)
    .AddJwtAuth(builder.Configuration)
    .AddSwaggerWithJwt()
    .AddPolicies()
    .AddInfrastructure();

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

var app = builder.Build();
//Inicializa as migrates
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
    dbContext.Database.Migrate();
}
// Inicialização do banco e seed do usuário admin
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<User>>();
    await SeedData.Initialize(services, userManager);
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    c.RoutePrefix = "swagger";  // Acesso via /swagger
});
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();


app.Run();
