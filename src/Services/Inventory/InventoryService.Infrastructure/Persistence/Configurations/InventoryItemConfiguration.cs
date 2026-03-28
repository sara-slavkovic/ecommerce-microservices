using InventoryService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryService.Infrastructure.Persistence.Configurations
{
    public class InventoryItemConfiguration : IEntityTypeConfiguration<InventoryItem>
    {
        public void Configure(EntityTypeBuilder<InventoryItem> builder)
        {
            builder.ToTable("InventoryItems");

            builder.HasKey(i => i.Id);

            builder.Property(i => i.ProductId).IsRequired();

            builder.Property(i => i.AvailableQuantity).IsRequired();

            builder.Property(i => i.ReservedQuantity).IsRequired();

            builder.Property(i => i.LastUpdatedAt).IsRequired();

            builder.HasIndex(i => i.ProductId).IsUnique();
        }
    }
}
