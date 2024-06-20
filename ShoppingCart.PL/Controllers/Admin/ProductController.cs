using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using ShoppingCart.BLL.Interfaces;
using ShoppingCart.DAL.Models;
using ShoppingCart.PL.Helpers;
using ShoppingCart.PL.ViewModels;

using System.Text.Json;

namespace ShoppingCart.PL.Controllers.Admin
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #region Index
        public async Task<IActionResult> Index(int? categoryId)
        {
            IEnumerable<Product> Model;

            if (categoryId is null)
                Model = await _unitOfWork.ProductRepository.GetAllAsync();
            else
                Model = _unitOfWork.ProductRepository.GetAllAsync().Result.Where(product => product.CategoryId == categoryId);

            var viewModel = _mapper.Map<IEnumerable<Product>, IEnumerable<ProductViewModel>>(Model);

            return View(viewModel);
        }

        #endregion


        #region Create
        public async Task<IActionResult> Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(ProductViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var Model = _mapper.Map<ProductViewModel, Product>(viewModel);

                    Model.ImageUrl = DocumentSettings.UploadFile(viewModel.FormFile, "Images");

                    _unitOfWork.ProductRepository.Add(Model);

                    await _unitOfWork.Save();

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {

                    ModelState.AddModelError(string.Empty, ex.Message);
                }

            }

            return View(nameof(Create));
        }

        #endregion


        #region Update
        public async Task<IActionResult> Update(int? id)
        {
            ViewData["ImageUrl"] = _unitOfWork.ProductRepository.GetByIdAsync(id.Value).Result.ImageUrl;

            return await LogicForRUDOperations(id, nameof(Update));
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromRoute] int? id, ProductViewModel viewModel)
        {
            if (viewModel.FormFile is not null)
                viewModel.ImageUrl = DocumentSettings.UploadFile(viewModel.FormFile, "Images");

            var Model = _mapper.Map<ProductViewModel, Product>(viewModel);

            return await LogicForUpdateAndDeletePost(id, viewModel, () => _unitOfWork.ProductRepository.Update(Model), nameof(Update));

        }

        #endregion


        #region Details
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id) => await LogicForRUDOperations(id, nameof(Details));

        #endregion


        #region Delete
        public async Task<IActionResult> Delete(int? id) => await LogicForRUDOperations(id, nameof(Delete));

        [HttpPost]
        public async Task<IActionResult> Delete([FromRoute] int? id, ProductViewModel viewModel)
        {
            var Model = _mapper.Map<ProductViewModel, Product>(viewModel);

            return await LogicForUpdateAndDeletePost(id, viewModel, () => _unitOfWork.ProductRepository.Delete(Model), nameof(Update));
        }

        #endregion


        #region Helper Methods 
        private async Task<IActionResult> LogicForRUDOperations(int? id, string viewName)
        {
            if (id is null)
                return BadRequest();

            var Model = await _unitOfWork.ProductRepository.GetByIdAsync(id.Value);

            if (Model is null)
                return NotFound();

            var viewModel = _mapper.Map<Product, ProductViewModel>(Model);

            return View(viewName, viewModel);
        }

        private async Task<IActionResult> LogicForUpdateAndDeletePost(int? id, ProductViewModel viewModel, Action logic, string viewName)
        {
            if (id.Value != viewModel.Id)
                return BadRequest();

            if (ModelState.IsValid)
            {
                try
                {
                    var Model = _mapper.Map<ProductViewModel, Product>(viewModel);

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

        private async Task<IActionResult> GetAllProducts()
        {
            var products = await _unitOfWork.ProductRepository.GetAllAsync();

            return Json(new { data = products });
        }



        #endregion
    }
}
