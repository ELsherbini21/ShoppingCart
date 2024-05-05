using AutoMapper;
using ShoppingCart.DAL.Models;
using ShoppingCart.PL.ViewModels;

namespace ShoppingCart.PL.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Category, CategoryViewModel>().ReverseMap();

            CreateMap<Product, ProductViewModel>().ReverseMap();

            CreateMap<Cart, CartViewModel>().ReverseMap();

            CreateMap<OrderHeader, OrderHeaderViewModel>().ReverseMap();

            CreateMap<OrderDetail, OrderDetailViewModel>().ReverseMap();

            CreateMap<ApplicationUser , UsersOrdersHeaderViewModel>().ReverseMap();
        }
    }
}
