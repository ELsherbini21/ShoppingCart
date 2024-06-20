using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShoppingCart.BLL.Interfaces;
using ShoppingCart.DAL.Models;
using ShoppingCart.PL.ViewModels;

namespace ShoppingCart.PL.Controllers.Admin
{
    [Authorize]
    public class RoleController : Controller
    {

        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public RoleController(RoleManager<IdentityRole> roleManager,
          UserManager<ApplicationUser> userManager,
          IUnitOfWork unitOfWork)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            {
                var Models = _roleManager.Roles.ToList();

                return View(Models);
            }
        }

        public async Task<IActionResult> Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(RoleViewModel roleViewModel)
        {
            if (ModelState.IsValid)
            {
                var roleIsExist = await _roleManager.FindByNameAsync(roleViewModel.Name);

                if (roleIsExist is not null)
                {
                    ModelState.AddModelError("Name", $"{roleViewModel.Name} is Exist");

                    return RedirectToAction(nameof(Index));
                }

                await _roleManager.CreateAsync(new IdentityRole(roleViewModel.Name.Trim()));


            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(string roleId)
        {
            if (roleId is null)
                return BadRequest();

            var role = await _roleManager.FindByIdAsync(roleId);

            var roleViewModel = new RoleViewModel()
            {
                Id = roleId,
                Name = role.Name
            };



            return View("PartialViews/DeletePartialView", roleViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(RoleViewModel roleFormViewModel, string roleId)
        {
            if (roleFormViewModel is null)
                return BadRequest();

            var role = await _roleManager.FindByIdAsync(roleId);

            if (role is null)
                return NotFound();

            await _roleManager.DeleteAsync(role);


            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> SelectUsers(string roleId)
        {
            if (roleId is null)
                return BadRequest();

            var role = await _roleManager.FindByIdAsync(roleId);

            ViewBag.roleObjectInfo = role;
            if (role is null)
                return NotFound();

            var usersInRole = new List<UsersInRoleViewModel>();

            foreach (var user in _userManager.Users.ToList())
            {
                var userInRole = new UsersInRoleViewModel()
                {
                    AppUserId = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserName = user.UserName,
                    Email = user.Email
                };

                if (await _userManager.IsInRoleAsync(user, role.Name) == true)
                    userInRole.IsSelected = true;
                else
                    userInRole.IsSelected = false;

                usersInRole.Add(userInRole);
            }
            return View(usersInRole);

        }

        [HttpPost]
        public async Task<IActionResult> SelectUsers(List<UsersInRoleViewModel> usersInRole, string roleId)
        {
            if (usersInRole.Count() <= 0 || roleId is null)
                return BadRequest();

            var role = await _roleManager.FindByIdAsync(roleId);

            if (role is not null)
            {
                if (ModelState.IsValid)
                {
                    foreach (var userViewModel in usersInRole)
                    {
                        var appuser = await _userManager.FindByIdAsync(userViewModel.AppUserId);

                        if (userViewModel.IsSelected == true && await _userManager.IsInRoleAsync(appuser, role.Name) == false)
                            await _userManager.AddToRoleAsync(appuser, role.Name);
                        else if (userViewModel.IsSelected == false && await _userManager.IsInRoleAsync(appuser, role.Name) == true)
                            await _userManager.RemoveFromRoleAsync(appuser, role.Name);

                    }
                    await _unitOfWork.Save();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(usersInRole);
        }



        public async Task<IActionResult> EditName(string roleId)
        {
            if (roleId is null) return BadRequest();

            var role = await _roleManager.FindByIdAsync(roleId);

            if (role is null) return NotFound();

            var roleViewModel = new RoleViewModel()
            {
                Id = role.Id,
                Name = role.Name
            };

            return View("PartialViews/EditNamePartialView", roleViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditName(RoleViewModel roleFormViewModel)
        {

            if (ModelState.IsValid)
            {
                var roleModel = await _roleManager.FindByIdAsync(roleFormViewModel.Id);

                if (roleFormViewModel is null)
                    return NotFound();

                roleModel.Name = roleFormViewModel.Name;

                var result = await _roleManager.UpdateAsync(roleModel);

                if (result.Succeeded)
                    return RedirectToAction(nameof(Index));

                await _unitOfWork.Save();

            }

            return View(roleFormViewModel);
        }





        private async Task<List<ApplicationUser>> GetUsersByRoleId(string roleId)
        {
            var role = _roleManager.FindByIdAsync(roleId);

            if (role is not null)
            {
                var users = await _userManager.GetUsersInRoleAsync(role.Result.Name);

                return users.ToList();
            }

            return new List<ApplicationUser>();
        }
    }
}
