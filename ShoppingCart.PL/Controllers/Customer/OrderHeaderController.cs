using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ShoppingCart.BLL.Interfaces;
using ShoppingCart.DAL.Models;
using ShoppingCart.PL.ViewModels;
using System.Composition;

namespace ShoppingCart.PL.Controllers.Customer
{
    [Authorize]
    public class OrderHeaderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderHeaderController(IUnitOfWork unitOfWork, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
        }

        #region GetDataFromDisplayShoppingList && Index
        [HttpPost]
        public async Task<IActionResult> GetDataFromDisplayShoppingList(double orderTotal, List<CartViewModel> cartViewModelList)
        {
            if (ModelState.IsValid)
            {
                var Model = _unitOfWork.CartRepository.GetAllAsync().Result.ToList();

                TempData["ListOfCart"] = cartViewModelList;

                TempData.Keep();

                return RedirectToAction(nameof(Index) , orderTotal);
            }

            return RedirectToAction("DisplayShoppingCartList", "Cart");
        }

        public async Task<IActionResult> Index(double? orderTotal)
        {
            if (orderTotal is not null)
            {
                var ListOfCart = TempData["ListOfCart"];


                var appUser = await _userManager.GetUserAsync(User);

                var viewModel = new OrderHeaderViewModel()
                {
                    Name = $"{appUser.FirstName} {appUser.LastName}",
                    Phone = appUser.Phone,
                    OrderTotal = orderTotal.Value,
                    Address = appUser.Address,
                    City = appUser.City,
                    DateOfOrder = DateTime.Now,
                    DateOfShipping = DateTime.Now,
                    ApplicationUserId = appUser.Id
                };
                return View(viewModel);
            }
            return RedirectToAction("DisplayShoppingCartList", "Cart");
        }

        #endregion

        [HttpPost]
        public async Task<IActionResult> CompletePurchase(OrderHeaderViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                foreach (var item in (List<Cart>)TempData["ListOfCart"])
                {

                }
            }
            return View(nameof(Index));
        }


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
