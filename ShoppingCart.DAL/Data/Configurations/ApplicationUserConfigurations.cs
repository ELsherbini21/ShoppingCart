using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoppingCart.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.DAL.Data.Configurations
{
    public class ApplicationUserConfigurations : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(user => user.FirstName).HasMaxLength(50);

            builder.Property(user => user.LastName).HasMaxLength(50);
        }
    }

    public class ProductConfigurations : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");


        }
    }


    public class CategoryConfigurations : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categories");

            builder.HasMany(category => category.Products)

                .WithOne(product => product.Category)

                .OnDelete(DeleteBehavior.Cascade)

                .HasForeignKey(product => product.CategoryId);
        }
    }

    public class CartConfigurations : IEntityTypeConfiguration<Cart>
    {
        public void Configure(EntityTypeBuilder<Cart> builder)
        {
            builder.ToTable("Carts");

            builder.HasKey(cart => cart.Id);

            builder.Property(cart => cart.Id).UseIdentityColumn(10, 10);

        }
    }
}
