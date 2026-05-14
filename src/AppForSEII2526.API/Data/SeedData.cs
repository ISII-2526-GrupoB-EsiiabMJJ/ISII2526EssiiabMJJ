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

            var existingUser1 = await userManager.FindByEmailAsync("Pasatpetruvlad@gmail.com");
            if (existingUser1 == null)
            {
                var user1 = new ApplicationUser
                {
                    UserName = "Pasatpetruvlad@gmail.com",
                    Email = "Pasatpetruvlad@gmail.com",
                    Name = "Petru",
                    Surname = "Vlad",
                    EmailConfirmed = true
                };

                var result1 = await userManager.CreateAsync(user1, "Pepe_0114");

                if (!result1.Succeeded)
                {
                    throw new Exception("Error creando el usuario Petru: " +
                        string.Join(", ", result1.Errors.Select(e => e.Description)));
                }
            }

            var existingUser2 = await userManager.FindByEmailAsync("jaime@uclm.es");
            if (existingUser2 == null)
            {
                var user2 = new ApplicationUser
                {
                    UserName = "jaime@uclm.es",
                    Email = "jaime@uclm.es",
                    Name = "Jaime",
                    Surname = "Catedra",
                    EmailConfirmed = true
                };

                var result2 = await userManager.CreateAsync(user2, "Aa123456789@");

                if (!result2.Succeeded)
                {
                    throw new Exception("Error creando el usuario Jaime: " +
                        string.Join(", ", result2.Errors.Select(e => e.Description)));
                }
            }
            var existingUser3 = await userManager.FindByEmailAsync("maria@uclm.es");
            if (existingUser3 == null)
            {
                var user3 = new ApplicationUser
                {
                    UserName = "maria@uclm.es",
                    Email = "maria@uclm.es",
                    Name = "Maria",
                    Surname = "Test",
                    EmailConfirmed = true
                };

                var result3 = await userManager.CreateAsync(user3, "Aa123456789@");

                if (!result3.Succeeded)
                {
                    throw new Exception("Error creando el usuario Maria: " +
                        string.Join(", ", result3.Errors.Select(e => e.Description)));
                }
            }
        }
    }
}