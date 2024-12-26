using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Common.Utility;
using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.Infrastructure.Data
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDBContext _Db;
        public DbInitializer(ApplicationDBContext Db, UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _Db = Db;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public void Initialize()
        {
            try
            {
                if (_Db.Database.GetPendingMigrations().Count() > 0)
                {
                    _Db.Database.Migrate();
                }

                if (!_roleManager.RoleExistsAsync(SD.Role_Admin).GetAwaiter().GetResult())
                {
                    _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).Wait();
                    _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).Wait();

                    _userManager.CreateAsync(new ApplicationUser
                    {
                        UserName = "admin@barbod.dev",
                        Email = "admin@barbod.dev",
                        Name = "Admin Barbod",
                        NormalizedUserName = "ADMIN@BARBOD.DEV",
                        NormalizedEmail = "ADMIN@BARBOD.DEV",
                        PhoneNumber = "111122223333"
                    }, "Admin123*").GetAwaiter().GetResult();

                    ApplicationUser user = _Db.ApplicationUsers.FirstOrDefault(x => x.Email == "admin@barbod.dev");
                    _userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();

                }


            }
            catch (Exception ex)
            {
                {
                    throw;
                }
            }
        }
    }
}
