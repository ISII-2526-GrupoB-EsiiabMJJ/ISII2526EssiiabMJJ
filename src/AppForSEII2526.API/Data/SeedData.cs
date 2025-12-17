using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using AppForSEII2526.API.Models;

namespace AppForSEII2526.API.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // Verifica si el usuario ya existe
            var existingUser = await userManager.FindByEmailAsync("Petruvladpasa@gmail.com");
            if (existingUser == null)
            {
                var user = new ApplicationUser
                {
                    UserName = "Pasatpetruvlad@gmail.com",
                    Email = "Pasatpetruvlad@gmail.com",
                    Name = "Petru",
                    Surname = "Vlad",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, "Pepe_0114");

                if (!result.Succeeded)
                {
                    throw new Exception("Error creando el usuario inicial: " +
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
    }
}
