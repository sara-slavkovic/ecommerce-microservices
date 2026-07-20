using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentService.Infrastructure.Persistence.Configurations
{
    public class PaymentAttemptConfiguration : IEntityTypeConfiguration<PaymentAttempt>
    {
        public void Configure(EntityTypeBuilder<PaymentAttempt> builder)
        {
            builder.ToTable("PaymentAttempts");

            builder.HasKey(pa => pa.Id);

            builder.Property(pa => pa.PaymentId).IsRequired();

            builder.Property(pa => pa.AttemptNumber).IsRequired();

            builder.Property(pa => pa.StatusCode).IsRequired();

            builder.Property(pa => pa.ErrorMessage)
                .HasMaxLength(500)
                .IsRequired(false);

            builder.Property(pa => pa.StartedAt).IsRequired();
            builder.Property(pa => pa.FinishedAt).IsRequired();
            builder.Property(pa => pa.DurationMs).IsRequired();
        }
    }
}
