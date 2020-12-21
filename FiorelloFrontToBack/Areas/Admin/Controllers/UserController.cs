using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FiorelloFrontToBack.DAL;
using FiorelloFrontToBack.Models;
using FiorelloFrontToBack.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FiorelloFrontToBack.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _context;
        public UserController(UserManager<AppUser> userManager,
                               AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.currentUserId = _userManager.GetUserId(HttpContext.User);
            List<AppUser> users = _userManager.Users.ToList();
            List<UserVM> usersVM = new List<UserVM>();
            List<string> roles = new List<string> { "Admin", "Member" };
            foreach (AppUser user in users)
            {
                UserVM userVM = new UserVM()
                {
                    Id = user.Id,
                    Fullname = user.Fullname,
                    Email = user.Email,
                    Username = user.UserName,
                    Status = user.HasDeleted,
                    Role = (await _userManager.GetRolesAsync(user))[0],
                };
                usersVM.Add(userVM);
            }
            return View(usersVM);
        }
        public async Task<IActionResult> ChangeStatus(string id)
        {
            if (id == null) return NotFound();
            AppUser user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            return View(user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("ChangeStatus")]
        public async Task<IActionResult> ChangeStatusPost(string id)
        {

            if (id == null) return NotFound();
            AppUser user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            if (user.HasDeleted)
                user.HasDeleted = false;
            else
                user.HasDeleted = true;
            await _userManager.UpdateAsync(user);
            //await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult ResetPassword()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(string id, ResetPasswordVM reset)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            if (id == null) return NotFound();
            AppUser user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            string token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, reset.Password);
            if (result.Succeeded)
            {
                return Content("succeed");
            }
            //await _userManager.RemovePasswordAsync(user);
            //await _userManager.AddPasswordAsync(user,reset.Password);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> ChangeRole(string id)
        {
            if (id == null) return NotFound();
            AppUser user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            UserVM userVM = await GetUserVMAsync(user);
            return View(userVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeRole(string id, string role)
        {
            if (id == null || role==null) return NotFound();
            AppUser user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            string oldRole = (await _userManager.GetRolesAsync(user))[0];
            IdentityResult addRoleResult = await _userManager.AddToRoleAsync(user, role);
            if (!addRoleResult.Succeeded)
            {
                ModelState.AddModelError("", "There is a probelm in a program");
                UserVM userVM = await GetUserVMAsync(user);
                return View(userVM);
            }
            IdentityResult removeResult = await _userManager.RemoveFromRoleAsync(user, oldRole);
            if (!removeResult.Succeeded)
            {
                ModelState.AddModelError("", "There is problem in a program");
                UserVM userVM = await GetUserVMAsync(user);
                return View(userVM);
            }
            return RedirectToAction(nameof(Index));
        }
        private async Task<UserVM> GetUserVMAsync(AppUser user)
        {
            List<string> roles = new List<string> { "Admin", "Member" };
            UserVM userVM = new UserVM
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.UserName,
                Fullname = user.Fullname,
                Status = user.HasDeleted,
                Role = (await _userManager.GetRolesAsync(user))[0],
                Roles = roles,
            };
            return userVM;
        }
    }
}
