using CartService.Application.Interfaces;
using CartService.Domain.Entities;
using CartService.Infrastructure.Persistence;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CartService.Infrastructure.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly CartDbContext _context;

        public CartRepository(CartDbContext context)
        {
            _context = context;
        }

        private async Task<System.Data.IDbConnection> GetOpenConnectionAsync()
        {
            var connection = _context.Database.GetDbConnection();
            if (connection.State != System.Data.ConnectionState.Open)
                await ((System.Data.Common.DbConnection)connection).OpenAsync();
            return connection;
        }

        public async Task<Cart?> GetCartWithItemsByUserIdAsync(Guid userId)
        {
            var connection = await GetOpenConnectionAsync();

            const string sql = @"
            SELECT c.Id, c.UserId, c.TotalAmount, c.CreatedAt, c.UpdatedAt
            FROM Carts c
            WHERE c.UserId = @UserId;

            SELECT ci.Id, ci.CartId, ci.ProductId, ci.Quantity, ci.PricePerUnit
            FROM CartItems ci
            INNER JOIN Carts c ON ci.CartId = c.Id
            WHERE c.UserId = @UserId";

            using var multi = await connection.QueryMultipleAsync(sql, new { UserId = userId });

            var cart = await multi.ReadFirstOrDefaultAsync<Cart>();
            if (cart == null) return null;

            var items = await multi.ReadAsync<CartItem>();
            cart.CartItems = items.ToList();

            return cart;
        }

        public async Task InsertCartWithItemAsync(Cart cart, CartItem item)
        {
            var connection = await GetOpenConnectionAsync();
            using var transaction = connection.BeginTransaction();
            try
            {
                await connection.ExecuteAsync(
                    "INSERT INTO Carts (Id, UserId, TotalAmount, CreatedAt, UpdatedAt) VALUES (@Id, @UserId, @TotalAmount, @CreatedAt, @UpdatedAt)",
                    cart, transaction);

                item.Id = await connection.ExecuteScalarAsync<Guid>(
                    "INSERT INTO CartItems (CartId, ProductId, Quantity, PricePerUnit) OUTPUT INSERTED.Id VALUES (@CartId, @ProductId, @Quantity, @PricePerUnit)",
                    item, transaction);

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task InsertCartItemAsync(Cart cart, CartItem item)
        {
            var connection = await GetOpenConnectionAsync();
            using var transaction = connection.BeginTransaction();
            try
            {
                item.Id = await connection.ExecuteScalarAsync<Guid>(
                    "INSERT INTO CartItems (CartId, ProductId, Quantity, PricePerUnit) OUTPUT INSERTED.Id VALUES (@CartId, @ProductId, @Quantity, @PricePerUnit)",
                    item, transaction);

                await connection.ExecuteAsync(
                    "UPDATE Carts SET TotalAmount = @TotalAmount, UpdatedAt = @UpdatedAt WHERE Id = @Id",
                    cart, transaction);

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task UpdateCartAndItemAsync(Cart cart, CartItem item)
        {
            var connection = await GetOpenConnectionAsync();
            using var transaction = connection.BeginTransaction();
            try
            {
                await connection.ExecuteAsync(
                    "UPDATE Carts SET TotalAmount = @TotalAmount, UpdatedAt = @UpdatedAt WHERE Id = @Id",
                    cart, transaction);

                await connection.ExecuteAsync(
                    "UPDATE CartItems SET Quantity = @Quantity, PricePerUnit = @PricePerUnit WHERE Id = @Id",
                    item, transaction);

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task DeleteCartItemAndUpdateCartAsync(Guid cartItemId, Cart cart)
        {
            var connection = await GetOpenConnectionAsync();
            using var transaction = connection.BeginTransaction();
            try
            {
                await connection.ExecuteAsync(
                    "DELETE FROM CartItems WHERE Id = @Id",
                    new { Id = cartItemId }, transaction);

                await connection.ExecuteAsync(
                    "UPDATE Carts SET TotalAmount = @TotalAmount, UpdatedAt = @UpdatedAt WHERE Id = @Id",
                    cart, transaction);

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task DeleteCartAsync(Guid cartId)
        {
            var connection = await GetOpenConnectionAsync();
            await connection.ExecuteAsync(
                "DELETE FROM Carts WHERE Id = @CartId", new { CartId = cartId });
        }
    }
}
