using BlogProjectPrac7.Data;
using BlogProjectPrac7.Enums;
using BlogProjectPrac7.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlogProjectPrac7.Services
{

    public class DataService
    {
        private readonly ApplicationDbContext _dbcontext;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<BlogUser> _userManager;

        public DataService(ApplicationDbContext dbcontext, RoleManager<IdentityRole> roleManager, UserManager<BlogUser> userManager)
        {
            _dbcontext = dbcontext;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task ManageDataAsync()
        {
            //Task: Create the DB from the current Migrations
            await _dbcontext.Database.MigrateAsync();

            //Task 1: Seed a few Roles into the system
            await SeedRolesAsync();

            //Task 2: Seed a few users into the system.
            await SeedUsersAsync();
        }

        private async Task SeedRolesAsync()
        {
            //If there are already roles in the system, do nothing.
            if (_dbcontext.Roles.Any()) { return; }

            //Otherwise we want to create a few roles.
            foreach (var role in Enum.GetNames(typeof(BlogRole)))
            {
                //we need to use the role manager(_roleManager) to create roles
                await _roleManager.CreateAsync(new IdentityRole(role));

            }
        }

        private async Task SeedUsersAsync()
        {
            //If there are already Users in the system, do nothing.
            if (_dbcontext.Users.Any()) { return; }
            //Admin
            //Step 1: Creates a new instance of BlogUser.
            var adminUser = new BlogUser()
            {
                Email = "mskntozakhe@gmail.com",
                UserName = "mskntozakhe@gmail.com",
                FirstName = "Mandlakapheli",
                LastName = "Ntozakhe",
                PhoneNumber = "0798593237",
                EmailConfirmed = true
            };

            //Step 2: Use the UserManager to create a new user that is define by the adminUser
            await _userManager.CreateAsync(adminUser, "Abc&123");

            //Step 3: Add this new user to the Admin Role.
            await _userManager.AddToRoleAsync(adminUser, BlogRole.Administrator.ToString());
            //////////////////////////////////////////////////////////////////////////////////
            ///
            //Moderator
            //Step 1: Creates a new instance of BlogUser.
            var modUser = new BlogUser()
            {
                Email = "mpentozakhe@gmail.com",
                UserName = "mpentozakhe@gmail.com",
                FirstName = "Sililo",
                LastName = "Ntozakhe",
                PhoneNumber = "0829243237",
                EmailConfirmed = true
            };

            //Step 2: Use the UserManager to create a new user that is define by the modUser
            await _userManager.CreateAsync(modUser, "Abc&123");

            //Step 3: Add this new user to the Moderator Role.
            await _userManager.AddToRoleAsync(modUser, BlogRole.Moderator.ToString());
        }

    }

}
