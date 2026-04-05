using CartService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CartService.Infrastructure.Persistence.Configurations
{
    public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
    {
        public void Configure(EntityTypeBuilder<CartItem> builder)
        {
            builder.ToTable("CartItems");

            builder.HasKey(ci => ci.Id);

            builder.Property(ci => ci.ProductId)
                .IsRequired();

            builder.Property(ci => ci.Quantity)
                .IsRequired();

            builder.Property(ci => ci.PricePerUnit)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.HasIndex(ci => new { ci.CartId, ci.ProductId })
                .IsUnique();
        }
    }
}
