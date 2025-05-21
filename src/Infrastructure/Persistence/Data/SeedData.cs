using microservice_authentication__api.src.Domain.Entities;
using microservice_authentication__api.src.Infrastructure.Auth;
using Microsoft.AspNetCore.Identity;

namespace microservice_authentication__api.src.Infrastructure.Persistence.Data
{
    public class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider, UserManager<User> userManager)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Lista dos roles que precisas no sistema
            string[] roles = [RolesNames.Admin, RolesNames.Professional, RolesNames.Patient, RolesNames.Manager, RolesNames.Scheduling, RolesNames.Appointment];

            // Criar roles se não existirem
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
            // Verifique se a role "admin" já existe, caso contrário, crie-a
            var roleExist = await roleManager.RoleExistsAsync(RolesNames.Admin);
            if (!roleExist)
            {
                await roleManager.CreateAsync(new IdentityRole(RolesNames.Admin));
            }

            // Verifique se já existe um usuário admin
            var user = await userManager.FindByEmailAsync("admin@admin.com");
            if (user == null)
            {
                if (await userManager.FindByNameAsync("945213730") != null) return;

                // Crie o usuário admin com senha padrão
                user = new User
                {
                    UserName = "945213730",
                    Email = "admin@admin.com",
                    FirstName = "Cristiano",
                    LastName = "Alberto",
                    ExternalReferenceId = "xxx-xxxx-xxxx",
                };

                var result = await userManager.CreateAsync(user, "Admin123!");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, RolesNames.Admin);
                }
            }
        }
    }
}