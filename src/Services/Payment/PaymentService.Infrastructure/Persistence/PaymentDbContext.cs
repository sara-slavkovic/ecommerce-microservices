using Microsoft.EntityFrameworkCore;
using PaymentService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentService.Infrastructure.Persistence
{
    public class PaymentDbContext : DbContext
    {
        public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options)
        {
        }

        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<PaymentAttempt> PaymentAttempts => Set<PaymentAttempt>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PaymentDbContext).Assembly);
        }
    }
}
