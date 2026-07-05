using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderService.Infrastructure.Persistence.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");

            builder.HasKey(o => o.Id);

            builder.Property(o => o.UserId).IsRequired();

            builder.Property(o => o.Status)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(o => o.TotalAmount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(o => o.Address)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(o => o.City)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(o => o.PostalCode)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(o => o.Country)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(o => o.CreatedAt).IsRequired();
            builder.Property(o => o.UpdatedAt).IsRequired();

            builder.HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
