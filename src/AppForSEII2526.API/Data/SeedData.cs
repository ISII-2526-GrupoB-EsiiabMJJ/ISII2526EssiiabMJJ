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
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            await CreateUserIfNotExists(
                userManager,
                "Pasatpetruvlad@gmail.com",
                "Pepe_0114",
                "Petru",
                "Vlad");

            await CreateUserIfNotExists(
                userManager,
                "jaime@uclm.es",
                "Aa123456789@",
                "Jaime",
                "Catedra");

            await CreateUserIfNotExists(
                userManager,
                "maria@uclm.es",
                "Aa123456789@",
                "Maria",
                "Test");

            await CreateUserIfNotExists(
               userManager,
               "prueba@uclm.es",
               "Aa123456789@",
               "Prueba",
               "Pruebas");

            SeedRepairs(context);
        }

        private static async Task CreateUserIfNotExists(
            UserManager<ApplicationUser> userManager,
            string email,
            string password,
            string name,
            string surname)
        {
            var existingUser = await userManager.FindByEmailAsync(email);

            if (existingUser != null)
            {
                return;
            }

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                Name = name,
                Surname = surname,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                throw new Exception($"Error creando el usuario {email}: " +
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        private static void SeedRepairs(ApplicationDbContext context)
        {
            var basicScale = GetOrCreateScale(context, "Básica");
            var mediumScale = GetOrCreateScale(context, "Media");
            var luxuryScale = GetOrCreateScale(context, "Lujo");

            AddRepairIfNotExists(
                context,
                "Cambio de pantalla",
                "Sustitución de pantalla rota o dañada.",
                89.99m,
                basicScale);

            AddRepairIfNotExists(
                context,
                "Cambio de batería",
                "Sustitución de batería degradada o defectuosa.",
                49.99m,
                mediumScale);

            AddRepairIfNotExists(
                context,
                "Reparación de placa base",
                "Diagnóstico y reparación avanzada de placa base.",
                149.99m,
                luxuryScale);

            AddRepairIfNotExists(
                context,
                "Reparación de conector de carga",
                "Sustitución o reparación del puerto de carga del dispositivo.",
                59.99m,
                basicScale);

            AddRepairIfNotExists(
                context,
                "Sustitución de cámara",
                "Cambio del módulo de cámara trasera o frontal.",
                74.99m,
                mediumScale);

            AddRepairIfNotExists(
                context,
                "Diagnóstico completo",
                "Revisión completa del dispositivo y detección de averías.",
                29.99m,
                luxuryScale);

            context.SaveChanges();
        }

        private static Scale GetOrCreateScale(ApplicationDbContext context, string name)
        {
            var scale = context.Scales.FirstOrDefault(s => s.Name == name);

            if (scale != null)
            {
                return scale;
            }

            scale = new Scale(name);

            context.Scales.Add(scale);
            context.SaveChanges();

            return scale;
        }

        private static void AddRepairIfNotExists(
            ApplicationDbContext context,
            string name,
            string description,
            decimal cost,
            Scale scale)
        {
            var exists = context.Repairs.Any(r => r.Name == name);

            if (exists)
            {
                return;
            }

            var repair = new Repair(name, description, cost, scale.Id)
            {
                Scale = scale
            };

            context.Repairs.Add(repair);
        }
    }
}