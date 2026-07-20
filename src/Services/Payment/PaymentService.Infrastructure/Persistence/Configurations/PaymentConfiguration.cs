using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentService.Infrastructure.Persistence.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("Payments");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.OrderId).IsRequired();

            builder.Property(p => p.Status)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(p => p.Amount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(p => p.IdempotencyKey)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.CreatedAt).IsRequired();

            builder.HasIndex(p => p.OrderId).IsUnique();
            builder.HasIndex(p => p.IdempotencyKey).IsUnique();

            builder.HasMany(p => p.PaymentAttempts)
                .WithOne(pa => pa.Payment)
                .HasForeignKey(pa => pa.PaymentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
