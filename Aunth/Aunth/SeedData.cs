using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Aunth
{
    public static class SeedData
    {
        public static async Task EnsureSeedData(IServiceProvider provider)
        {
            var roleMgr = provider.GetRequiredService<RoleManager<IdentityRole>>();
            foreach (var roleName in RoleNames.AllRoles)
            {
                var role = roleMgr.FindByNameAsync(roleName).Result;

                if (role == null)
                {
                    var result = roleMgr.CreateAsync(new IdentityRole { Name = roleName }).Result;
                    if (!result.Succeeded) throw new Exception(result.Errors.First().Description);
                }
            }

            var userMgr = provider.GetRequiredService<UserManager<IdentityUser>>();

            var adminResult = await userMgr.CreateAsync(DefaultUsers.Administrator, "Administrator");
            var userResult = await userMgr.CreateAsync(DefaultUsers.User, "User");
            var moderatorResult = await userMgr.CreateAsync(DefaultUsers.Moderator, "Moderator");
            var workerResult = await userMgr.CreateAsync(DefaultUsers.Worker, "Worker");

            if (adminResult.Succeeded || userResult.Succeeded || moderatorResult.Succeeded || workerResult.Succeeded)
            {
                var adminUser = await userMgr.FindByEmailAsync(DefaultUsers.Administrator.Email);
                var commonUser = await userMgr.FindByEmailAsync(DefaultUsers.User.Email);
                var moderatorUser = await userMgr.FindByEmailAsync(DefaultUsers.Moderator.Email);
                var workerUser = await userMgr.FindByEmailAsync(DefaultUsers.Worker.Email);

                await userMgr.AddToRoleAsync(adminUser, RoleNames.Administrator);
                await userMgr.AddToRoleAsync(commonUser, RoleNames.User);
                await userMgr.AddToRoleAsync(moderatorUser, RoleNames.Moderator);
                await userMgr.AddToRoleAsync(workerUser, RoleNames.Worker);
            }
        }
    }

    public static class RoleNames
    {
        public const string Administrator = "Администратор";
        public const string User = "Пользователь";
        public const string Moderator = "Модератор";
        public const string Worker = "Сотрудник";

        public static IEnumerable<string> AllRoles
        {
            get
            {
                yield return Administrator;
                yield return User;
                yield return Moderator;
                yield return Worker;
                
            }
        }
    }

    public static class DefaultUsers
    {
        public static readonly IdentityUser Administrator = new IdentityUser
        {
            Email = "Admin@test.ru",
            EmailConfirmed = true,
            UserName = "Administrator"
            
        };

        public static readonly IdentityUser Moderator = new IdentityUser
        {
            Email = "Moderator@test.ru",
            EmailConfirmed = true,
            UserName = "Moderator",
        };

        public static readonly IdentityUser User = new IdentityUser
        {
            Email = "User@test.ru",
            EmailConfirmed = true,
            UserName = "User"
        };
        
        public static readonly IdentityUser Worker = new IdentityUser
        {
            Email = "Worker@test.ru",
            EmailConfirmed = true,
            UserName = "Worker"
        };

        public static IEnumerable<IdentityUser> AllUsers
        {
            get
            {
                yield return Administrator;
                yield return Moderator;
                yield return User;
                yield return Worker;
            }
        }
    }
}