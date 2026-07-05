using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderService.Infrastructure.Persistence.Configurations
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("OrderItems");

            builder.HasKey(oi => oi.Id);

            builder.Property(oi => oi.OrderId).IsRequired();

            builder.Property(oi => oi.ProductId).IsRequired();

            builder.Property(oi => oi.Quantity).IsRequired();

            builder.Property(oi => oi.PricePerUnit)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(oi => oi.TotalPrice)
                .IsRequired()
                .HasColumnType("decimal(18,2)");
        }
    }
}
