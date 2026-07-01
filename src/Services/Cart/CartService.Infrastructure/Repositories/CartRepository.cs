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

        public async Task InsertCartAsync(Cart cart)
        {
            var connection = await GetOpenConnectionAsync();
            await connection.ExecuteAsync(
                "INSERT INTO Carts (Id, UserId, TotalAmount, CreatedAt, UpdatedAt) VALUES (@Id, @UserId, @TotalAmount, @CreatedAt, @UpdatedAt)", cart);
        }

        public async Task UpdateCartAsync(Cart cart)
        {
            var connection = await GetOpenConnectionAsync();
            await connection.ExecuteAsync(
                "UPDATE Carts SET TotalAmount = @TotalAmount, UpdatedAt = @UpdatedAt WHERE Id = @Id", cart);
        }

        public async Task InsertCartItemAsync(CartItem item)
        {
            var connection = await GetOpenConnectionAsync();
            await connection.ExecuteAsync(
                "INSERT INTO CartItems (Id, CartId, ProductId, Quantity, PricePerUnit) VALUES (@Id, @CartId, @ProductId, @Quantity, @PricePerUnit)", item);
        }

        public async Task UpdateCartItemAsync(CartItem item)
        {
            var connection = await GetOpenConnectionAsync();
            await connection.ExecuteAsync(
                "UPDATE CartItems SET Quantity = @Quantity WHERE Id = @Id", item);
        }

        public async Task DeleteCartItemAsync(Guid cartItemId)
        {
            var connection = await GetOpenConnectionAsync();
            await connection.ExecuteAsync(
                "DELETE FROM CartItems WHERE Id = @Id", new { Id = cartItemId });
        }

        public async Task DeleteCartAsync(Guid cartId)
        {
            var connection = await GetOpenConnectionAsync();
            await connection.ExecuteAsync(
                "DELETE FROM Carts WHERE Id = @CartId", new { CartId = cartId });
        }
    }
}
