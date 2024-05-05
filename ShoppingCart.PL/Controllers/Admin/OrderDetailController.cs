using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Project;
using Newtonsoft.Json.Serialization;
using ShoppingCart.BLL.Interfaces;
using ShoppingCart.DAL.Models;
using ShoppingCart.PL.Helpers.Enums;
using ShoppingCart.PL.ViewModels;
using Stripe.Treasury;
using System.Linq;

namespace ShoppingCart.PL.Controllers.Admin
{
    [Authorize("Admin")]
    public class OrderDetailController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderDetailController(IUnitOfWork unitOfWork, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {

            var ListOfUsersThatContainOrdersHeader = (from appUser in _userManager.Users.ToList()
                                                      join orderHeader in _unitOfWork.OrderHeaderRepository.GetAllAsync().Result
                                                      on appUser.Id equals orderHeader.ApplicationUserId
                                                      select new UsersOrdersHeaderViewModel()
                                                      {
                                                          Id = appUser.Id,
                                                          FirstName = appUser.FirstName,
                                                          LastName = appUser.LastName,
                                                          UserName = appUser.UserName,
                                                          Email = appUser.Email,
                                                          Phone = appUser.Phone ?? appUser.PhoneNumber,
                                                          OrderHeaderId = orderHeader.Id,
                                                          OrderHeaderName = orderHeader.Name,
                                                          OrderTotal = _unitOfWork.OrderDetailRepository.GetAllAsync().Result.Where(order => order.OrderHeaderId == orderHeader.Id).Sum(cart => cart.Price),
                                                          OrderStatus = orderHeader.OrderStatus.ToString()
                                                      }).ToList();

            return View(ListOfUsersThatContainOrdersHeader);
        }

        public async Task<IActionResult> ManageOrder(int? orderHeaderId)
        {
            if (orderHeaderId == null)
                return BadRequest();

            var Model = await _unitOfWork.OrderHeaderRepository.GetByIdAsync(orderHeaderId.Value);

            if (Model is null)
                return NotFound();

            var viewModel = _mapper.Map<OrderHeader, OrderHeaderViewModel>(Model);

            return View(viewModel);

        }

        //[HttpPost]
        public async Task<IActionResult> Delete(int? orderHeaderId)
        {
            if (orderHeaderId == null)
                return BadRequest();

            var orderHeaderModel = await _unitOfWork.OrderHeaderRepository.GetByIdAsync(orderHeaderId.Value);

            if (orderHeaderModel is not null && orderHeaderModel is not null)
            {
                _unitOfWork.OrderHeaderRepository.Delete(orderHeaderModel);
                await _unitOfWork.Save();

                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(ManageOrder), orderHeaderId);

        }

        public async Task<IActionResult> Details(int? orderHeaderId)
        {
            if (orderHeaderId == null)
                return BadRequest();

            double totalOrder = 0;

            foreach (var i in _unitOfWork.OrderDetailRepository.GetAllAsync().Result.Where(order => order.OrderHeaderId == orderHeaderId))
            {
                totalOrder += (i.Price * i.Count);
            }

            var orderViewModel = new OrderViewModel()
            {
                OrderHeaderId = orderHeaderId.Value,
                OrderHeader = await _unitOfWork.OrderHeaderRepository.GetByIdAsync(orderHeaderId.Value),
                OrderDetails = _unitOfWork.OrderDetailRepository.GetAllAsync().Result.Where(order => order.OrderHeaderId == (orderHeaderId.Value)),
                OrderTotal = totalOrder

            };

            return View(orderViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Details(OrderViewModel viewModel)
        {
            if (viewModel is not null)
            {
                var orderHeaderModel = await _unitOfWork.OrderHeaderRepository.GetByIdAsync(viewModel.OrderHeaderId);

                if (orderHeaderModel != null)
                {
                    orderHeaderModel.Name = viewModel.OrderHeader.Name;
                    orderHeaderModel.Phone = viewModel.OrderHeader.Phone;
                    orderHeaderModel.Address = viewModel.OrderHeader.Address;
                    orderHeaderModel.City = viewModel.OrderHeader.City;
                    orderHeaderModel.State = viewModel.OrderHeader.State;
                    orderHeaderModel.PostalCode = viewModel.OrderHeader.PostalCode;

                    if (viewModel.OrderHeader.Carrier is not null)
                        orderHeaderModel.Carrier = viewModel.OrderHeader.Carrier;

                    if (viewModel.OrderHeader.TrackingNumber is not null)
                        orderHeaderModel.TrackingNumber = viewModel.OrderHeader.TrackingNumber;

                    _unitOfWork.OrderHeaderRepository.Update(orderHeaderModel);

                    await _unitOfWork.Save();

                    return RedirectToAction(nameof(Index));
                }
            }

            return View(viewModel);
        }

        public async Task<IActionResult> InProcess(int? OrderHeaderId)
        {
            if (OrderHeaderId is not null)
            {
                var model = await _unitOfWork.OrderHeaderRepository.GetByIdAsync(OrderHeaderId.Value);

                model.OrderStatus = OrderStatusEnum.UnderProcessing.ToString();

                _unitOfWork.OrderHeaderRepository.Update(model);

                await _unitOfWork.Save();

                return RedirectToAction(nameof(Details), new { orderHeaderId = OrderHeaderId.Value });
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Shipped(OrderViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var model = await _unitOfWork.OrderHeaderRepository.GetByIdAsync(viewModel.OrderHeaderId);

                if (model is not null)
                {
                    model.Carrier = viewModel.OrderHeader.Carrier;
                    model.TrackingNumber = viewModel.OrderHeader.TrackingNumber;
                    model.OrderStatus = OrderStatusEnum.Shipped.ToString();
                    model.DateOfShipping = DateTime.Now;

                    _unitOfWork.OrderHeaderRepository.Update(model);

                    await _unitOfWork.Save();

                    return RedirectToAction(nameof(Index));
                }
            }

            return RedirectToAction(nameof(Details), new { orderHeaderId = viewModel.OrderHeaderId });
        }

        public async Task<IActionResult> CancelOrder(int? OrderHeaderId)
        {
            if (OrderHeaderId is not null)
            {
                var model = await _unitOfWork.OrderHeaderRepository.GetByIdAsync(OrderHeaderId.Value);

                if (model is not null)
                {
                    model.OrderStatus = OrderStatusEnum.Cancelled.ToString();

                    _unitOfWork.OrderHeaderRepository.Update(model);

                    await _unitOfWork.Save();

                    return RedirectToAction(nameof(Details), new { orderHeaderId = OrderHeaderId.Value });
                }
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
