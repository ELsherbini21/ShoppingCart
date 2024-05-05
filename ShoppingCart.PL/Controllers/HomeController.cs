using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;
using ShoppingCart.BLL.Interfaces;
using ShoppingCart.DAL.Models;
using ShoppingCart.PL.Models;
using ShoppingCart.PL.ViewModels;
using System.Diagnostics;

namespace ShoppingCart.PL.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public HomeController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var Model = await _unitOfWork.ProductRepository.GetAllAsync();

            var viewModel = _mapper.Map<IEnumerable<Product>, IEnumerable<ProductViewModel>>(Model);

            return View(viewModel);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult _AccessDenied()
        {
            return View();
        }
        public IActionResult _ErrorPage()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
