using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentCar.Areas.Admin.Models.ViewModel;
using RentCar.DAL;
using RentCar.Data;
using RentCar.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentCar.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = RoleConstants.SuperAdmin)]
    public class UserController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;

        public UserController(AppDbContext dbContext, UserManager<IdentityUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _dbContext.Users.ToListAsync();
            var userList = new List<UserViewModel>();


            foreach (var user in users)
            {
                userList.Add(new UserViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Role = (await _userManager.GetRolesAsync(user))[0],
                    BlockedUsers = await _dbContext.BlockedUsers.ToListAsync()
                });
            }
            return View(userList);
        }


        #region
        //***** Change Role *****//
        //public async Task<IActionResult> ChangeRole(string id)
        //{
        //    IdentityUser user = await _userManager.FindByIdAsync(id);

        //    return View(new UserViewModel
        //    {
        //        Id = user.Id,
        //        UserName = user.UserName,
        //        Email = user.Email,
        //        Role = (await _userManager.GetRolesAsync(user))[0],
        //        Roles = new string[] {RoleConstants.Admin, RoleConstants.User, RoleConstants.SuperAdmin}
        //    });
        //}

        //[HttpPost]
        //[ActionName("ChangeRole")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> ChangeRolePost(string id, ChangeRoleViewModel model)
        //{
        //    IdentityUser user = await _userManager.FindByIdAsync(id);

        //    await _userManager.AddToRoleAsync(user, model.RoleName);

        //    return RedirectToAction("ChangeRole", "User", new { id = id });
        //}
        #endregion


        //***** Block User *****//
        public async Task<IActionResult> Block(string id)
        {
            IdentityUser user = await _userManager.FindByIdAsync(id);

            return View(new BlockUserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Role = (await _userManager.GetRolesAsync(user))[0]
            });
        }

        [HttpPost]
        [ActionName("Block")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BlockUser(string id)
        {
            IdentityUser user = await _userManager.FindByIdAsync(id);

            BlockedUser blockedUser = new BlockedUser
            {
                UserID = user.Id
            };

            await _dbContext.BlockedUsers.AddAsync(blockedUser);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index), "User");
        }

        //***** UnBlock User *****//
        public async Task<IActionResult> UnBlock(string id)
        {
            IdentityUser user = await _userManager.FindByIdAsync(id);

            return View(new UnBlockUserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Role = (await _userManager.GetRolesAsync(user))[0]
            });
        }

        [HttpPost]
        [ActionName("UnBlock")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnBlockUser(string id)
        {
            IdentityUser user = await _userManager.FindByIdAsync(id);

            var blockedUsers = await _dbContext.BlockedUsers.ToListAsync();

            BlockedUser unblockedUser = new BlockedUser();

            for (int i = 0; i < blockedUsers.Count; i++)
            {
                if(blockedUsers[i].UserID == user.Id)
                {
                    unblockedUser = blockedUsers[i];
                }
            }

            _dbContext.BlockedUsers.Remove(unblockedUser);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index), "User");
        }
    }
}