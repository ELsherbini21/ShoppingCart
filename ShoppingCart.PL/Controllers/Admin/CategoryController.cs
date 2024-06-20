using AutoMapper;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.FileProviders;
using NuGet.Configuration;
using ShoppingCart.BLL.Interfaces;
using ShoppingCart.DAL.Models;
using ShoppingCart.PL.ViewModels;
using Stripe.Identity;

namespace ShoppingCart.PL.Controllers.Admin
{

    [Authorize]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public CategoryController(IUnitOfWork unitOfWork, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            this._userManager = userManager;
        }

        #region Index
        public async Task<IActionResult> Index()
        {

            var appUser = await _userManager.GetUserAsync(User);

            var Model = await _unitOfWork.CategoryRepository.GetAllAsync();

            var viewModel = _mapper.Map<IEnumerable<Category>, IEnumerable<CategoryViewModel>>(Model);

            return View(viewModel);
        }

        #endregion


        #region Create
        public async Task<IActionResult> Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(CategoryViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var Model = _mapper.Map<CategoryViewModel, Category>(viewModel);

                _unitOfWork.CategoryRepository.Add(Model);

                await _unitOfWork.Save();

                return RedirectToAction(nameof(Index));
            }

            return View(nameof(Create));
        }

        #endregion


        #region Update
        public async Task<IActionResult> Update(int? id) => await LogicForRUDOperations(id, nameof(Update));

        [HttpPost]
        public async Task<IActionResult> Update([FromRoute] int? id, CategoryViewModel viewModel)
        {
            var Model = _mapper.Map<CategoryViewModel, Category>(viewModel);

            return await LogicForUpdateAndDeletePost(id, viewModel, () => _unitOfWork.CategoryRepository.Update(Model), nameof(Update));

        }

        #endregion


        #region Details
        public async Task<IActionResult> Details(int? id) => await LogicForRUDOperations(id, nameof(Details));

        #endregion


        #region Delete
        public async Task<IActionResult> Delete(int? id) => await LogicForRUDOperations(id, nameof(Delete));

        [HttpPost]
        public async Task<IActionResult> Delete([FromRoute] int? id, CategoryViewModel viewModel)
        {
            var Model = _mapper.Map<CategoryViewModel, Category>(viewModel);

            return await LogicForUpdateAndDeletePost(id, viewModel, () => _unitOfWork.CategoryRepository.Delete(Model), nameof(Update));
        }

        #endregion


        #region Helper Methods 
        private async Task<IActionResult> LogicForRUDOperations(int? id, string viewName)
        {
            if (id is null)
                return BadRequest();

            var Model = await _unitOfWork.CategoryRepository.GetByIdAsync(id.Value);

            if (Model is null)
                return NotFound();

            var viewModel = _mapper.Map<Category, CategoryViewModel>(Model);

            return View(viewName, viewModel);
        }

        private async Task<IActionResult> LogicForUpdateAndDeletePost(int? id, CategoryViewModel viewModel, Action logic, string viewName)
        {
            if (id.Value != viewModel.Id)
                return BadRequest();

            if (ModelState.IsValid)
            {
                try
                {
                    var Model = _mapper.Map<CategoryViewModel, Category>(viewModel);

                    logic();

                    await _unitOfWork.Save();

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {

                    ModelState.AddModelError(string.Empty, ex.Message);
                }

            }

            return View(viewName);
        }



        #endregion




    }
}
