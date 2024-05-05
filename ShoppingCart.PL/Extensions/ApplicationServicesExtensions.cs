using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using ShoppingCart.DAL.Data;
using ShoppingCart.DAL.Models;
using ShoppingCart.BLL.Interfaces;
using ShoppingCart.BLL.Repositories;
using ShoppingCart.PL.Helpers;
using ShoppingCart.PL.Helpers.interfaces;
using ShoppingCart.PL.Helpers.Models;

namespace ShoppingCart.PL.Extensions
{
    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {



            services.AddScoped<IProductRepository, ProductRepository>();

            services.AddScoped<ICategoryRepository, CategoryRepository>();

            services.AddScoped<ICartRepository, CartRepository>();

            services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();

            services.AddScoped<IOrderHeaderRepository, OrderHeaderRepository>();

            services.AddScoped<IEmailSettings, EmailSettings>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddAutoMapper(map =>
            map.AddProfile(new MappingProfile())
             );


            return services;
        }
    }
}
