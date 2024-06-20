using AutoMapper;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.Extensions.FileProviders;
using NuGet.Packaging.Core;
using NuGet.Protocol;
using ShoppingCart.BLL.Interfaces;
using ShoppingCart.DAL.Models;
using ShoppingCart.PL.Controllers.Admin;
using ShoppingCart.PL.Helpers.Enums;
using ShoppingCart.PL.ViewModels;
using Stripe;
using Stripe.BillingPortal;
using System.Reflection;
using System.Security.Claims;

namespace ShoppingCart.PL.Controllers.Customer
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CartController(SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        #region Index
        public async Task<IActionResult> Index()
        {
            if (_signInManager.IsSignedIn(User))
            {
                var appUser = await _userManager.GetUserAsync(User);

                if (appUser is not null)
                {
                    var ListOfShoppingCartItems_Model = _unitOfWork.CartRepository.GetAllAsync().Result
                        .Where(cart => cart.ApplicationUserId == appUser.Id);

                    var ListOfShoppingCartItems_viewModel = _mapper.Map<IEnumerable<Cart>, IEnumerable<CartViewModel>>(ListOfShoppingCartItems_Model);

                    return View(ListOfShoppingCartItems_viewModel);
                }

                return View("_ErrorPage");
            }

            return View("_AccessDenied");
        }

        #endregion


        #region Summary

        public async Task<IActionResult> Summary()
        {
            var appUser = await _userManager.GetUserAsync(User);

            if (appUser is not null)
            {
                var ListOfCartModel = _unitOfWork.CartRepository.GetAllAsync().Result.Where(cart => cart.ApplicationUserId == appUser.Id);

                if (ListOfCartModel.Count() > 0)
                {
                    Random rnd = new Random();

                    var orderHeaderViewModel = new OrderHeaderViewModel()
                    {
                        ApplicationUser = appUser,

                        ApplicationUserId = appUser.Id,

                        Name = appUser.FirstName + " " + appUser.LastName,

                        Phone = appUser.PhoneNumber,

                        Address = appUser.Address,

                        City = appUser.City,

                        PostalCode = rnd.Next(111111, 999999).ToString()

                    };

                    foreach (var item in ListOfCartModel)
                        orderHeaderViewModel.OrderTotal += item.Product.Price * item.Count;


                    return View(orderHeaderViewModel);
                }
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Summary(OrderHeaderViewModel orderHeaderViewModel)
        {
            if (ModelState.IsValid)
            {

                var appUser = await _userManager.GetUserAsync(User);

                if (appUser is null)
                    return NotFound();

                var ListOfCart = _unitOfWork.CartRepository.GetAllAsync().Result.Where(cart => cart.ApplicationUserId == appUser.Id);

                orderHeaderViewModel.OrderStatus = OrderStatusEnum.Pending.ToString();
                orderHeaderViewModel.PaymentStatus = PaymentStatusEnum.Pending.ToString();
                orderHeaderViewModel.DateOfOrder = DateTime.Now;
                orderHeaderViewModel.ApplicationUserId = appUser.Id;

                try
                {
                    var orderHeaderModel = _mapper.Map<OrderHeaderViewModel, OrderHeader>(orderHeaderViewModel);

                    _unitOfWork.OrderHeaderRepository.Add(orderHeaderModel);

                    await _unitOfWork.Save();

                    var id = orderHeaderModel.Id;

                    await OrderSuccess(id, OrderStatusEnum.Pending.ToString(), PaymentStatusEnum.Pending.ToString());

                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }

                //return RedirectToAction(nameof(HomeController.Index), "Home");
            }

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpPost]
        public async Task<IActionResult> OrderSuccess(int? id, string? OrderStatus, string? PaymentStatus)
        {
            if (id is null)
                return BadRequest();

            var orderHeaderModel = await _unitOfWork.OrderHeaderRepository.GetByIdAsync(id.Value);

            if (orderHeaderModel is null)
                return NotFound();


            _unitOfWork.BeginTransactionAsync();

            try
            {
                orderHeaderModel.OrderStatus = OrderStatus;

                orderHeaderModel.PaymentStatus = PaymentStatus;

                _unitOfWork.OrderHeaderRepository.Update(orderHeaderModel);

                var appUser = await _userManager.GetUserAsync(User);

                if (appUser is null)
                    return NotFound();

                var ListOfCartToRemove = _unitOfWork.CartRepository.GetAllAsync().Result.Where(cart => cart.ApplicationUserId == appUser.Id).ToList();

                foreach (var cartToRemove in ListOfCartToRemove)
                    _unitOfWork.CartRepository.Delete(cartToRemove);

                AddProductToOrderDetail(ListOfCartToRemove, id.Value);



                await _unitOfWork.Save();

                _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception)
            {

                _unitOfWork.RollbackTransactionAsync();
            }



            return RedirectToAction(nameof(Index), "OrderDetail");

        }

        private void AddProductToOrderDetail(List<Cart> ListOfCart, int orderHeaderId)
        {
            foreach (var item in ListOfCart)
            {
                var orderDetailsModel = new OrderDetail()
                {
                    ProductId = item.ProductId,
                    OrderHeaderId = orderHeaderId,
                    Count = item.Count,
                    Price = item.Product.Price
                };

                _unitOfWork.OrderDetailRepository.Add(orderDetailsModel);
            }
        }
        #endregion


        #region Create
        public async Task<IActionResult> Create(int? productId)
        {
            if (!_signInManager.IsSignedIn(User))
                return RedirectToAction(nameof(ErrorController.LogInError), "Error");

            var appUser = await _userManager.GetUserAsync(User);

            if (appUser is not null && productId is not null)
            {

                var cartViewModel = new CartViewModel()
                {
                    ApplicationUserId = appUser.Id,
                    Product = await _unitOfWork.ProductRepository.GetByIdAsync(productId.Value),
                    ProductId = productId.Value,
                    ApplicationUser = appUser,


                };

                return View(cartViewModel);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CartViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var Model = _mapper.Map<CartViewModel, Cart>(viewModel);

                _unitOfWork.CartRepository.Add(Model);

                await _unitOfWork.Save();

                return RedirectToAction(nameof(Index));
            }

            return View(nameof(Create));
        }

        #endregion


        #region Not Used 




        #region Update
        public async Task<IActionResult> Update(int? id) => await LogicForRUDOperations(id, nameof(Update));

        [HttpPost]
        public async Task<IActionResult> Update([FromRoute] int? id, CartViewModel viewModel)
        {
            var Model = _mapper.Map<CartViewModel, Cart>(viewModel);

            return await LogicForUpdateAndDeletePost(id, viewModel, () => _unitOfWork.CartRepository.Update(Model), nameof(Update));

        }

        #endregion


        #region Details
        public async Task<IActionResult> Details(int? id) => await LogicForRUDOperations(id, nameof(Details));

        #endregion


        #region Delete



        //[HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            var Model = await _unitOfWork.CartRepository.GetByIdAsync(id.Value);

            var viewModel = _mapper.Map<Cart, CartViewModel>(Model);

            return await LogicForUpdateAndDeletePost(id, viewModel, () => _unitOfWork.CartRepository.Delete(Model), nameof(Index));
        }

        #endregion



        #region Helper Methods 
        private async Task<IActionResult> LogicForRUDOperations(int? id, string viewName)
        {
            if (id is null)
                return BadRequest();

            var Model = await _unitOfWork.CartRepository.GetByIdAsync(id.Value);

            if (Model is null)
                return NotFound();

            var viewModel = _mapper.Map<Cart, CartViewModel>(Model);

            return View(viewName, viewModel);
        }

        private async Task<IActionResult> LogicForUpdateAndDeletePost(int? id, CartViewModel viewModel, Action logic, string viewName)
        {
            if (id.Value != viewModel.Id)
                return BadRequest();

            if (ModelState.IsValid)
            {
                try
                {
                    var Model = _mapper.Map<CartViewModel, Cart>(viewModel);

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


        public async Task<IActionResult> EditOrder(int? id)
        {
            if (id is null)
                return View("_ErrorPage");

            var model = await _unitOfWork.CartRepository.GetByIdAsync(id.Value);

            if (model is null)
                return View("_ErrorPage");

            var viewModel = _mapper.Map<Cart, CartViewModel>(model);

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditOrder(int id, CartViewModel ViewModel)
        {

            if (id != ViewModel.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                var model = _mapper.Map<CartViewModel, Cart>(ViewModel);

                _unitOfWork.CartRepository.Update(model);

                await _unitOfWork.Save();

                return RedirectToAction(nameof(Index));
            }

            return View(ViewModel);
        }


        #endregion


    }

}
