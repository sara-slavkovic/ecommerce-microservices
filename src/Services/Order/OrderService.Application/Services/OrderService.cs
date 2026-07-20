using FluentValidation;
using OrderService.Application.DTOs;
using OrderService.Application.Interfaces;
using OrderService.Domain.Entities;
using OrderService.Domain.Enums;
using SharedKernel.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderService.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICatalogServiceClient _catalogServiceClient;
        private readonly ICartServiceClient _cartServiceClient;
        private readonly IInventoryServiceClient _inventoryServiceClient;
        private readonly IValidator<CreateOrderDto> _createValidator;

        public OrderService(IOrderRepository orderRepository, ICatalogServiceClient catalogServiceClient, ICartServiceClient cartServiceClient, IInventoryServiceClient inventoryServiceClient, IValidator<CreateOrderDto> createValidator)
        {
            _orderRepository = orderRepository;
            _catalogServiceClient = catalogServiceClient;
            _cartServiceClient = cartServiceClient;
            _inventoryServiceClient = inventoryServiceClient;
            _createValidator = createValidator;
        }

        public async Task<OrderDto?> GetOrderByIdAsync(Guid id)
        {
            var order = await _orderRepository.GetOrderByIdAsync(id);
            if (order == null) return null;
            return await MapToDtoAsync(order);
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(Guid userId)
        {
            var orders = await _orderRepository.GetOrdersByUserIdAsync(userId);
            var dtos = new List<OrderDto>();

            foreach (var order in orders)
                dtos.Add(await MapToDtoAsync(order));

            return dtos;
        }

        public async Task<OrderDto> CreateOrderAsync(CreateOrderDto dto)
        {
            var validationResult = await _createValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var cart = await _cartServiceClient.GetCartByUserIdAsync(dto.UserId);

            if (cart == null || !cart.CartItems.Any())
                throw new BadRequestException("Cart is empty or does not exist.");

            var reservedItems = new List<CartItemSnapshotDto>();
            try
            { // reserve stock for every cart item
                foreach (var item in cart.CartItems)
                {
                    await _inventoryServiceClient.ReserveStockAsync(new InventoryChangeDto
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity
                    });
                    reservedItems.Add(item);
                }
            }
            catch
            {
                foreach (var reserved in reservedItems)
                {
                    await _inventoryServiceClient.ReleaseStockAsync(new InventoryChangeDto
                    {
                        ProductId = reserved.ProductId,
                        Quantity = reserved.Quantity
                    });
                }
                throw new ConflictException("Failed to reserve stock. Order creation has been rolled back.");
            }

            var order = new Order
            {
                Id = Guid.NewGuid(),
                UserId = dto.UserId,
                Status = OrderStatus.Pending,
                TotalAmount = cart.TotalAmount,
                Address = dto.Address,
                City = dto.City,
                PostalCode = dto.PostalCode,
                Country = dto.Country,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                OrderItems = cart.CartItems.Select(ci => new OrderItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    PricePerUnit = ci.PricePerUnit,
                    TotalPrice = ci.Quantity * ci.PricePerUnit
                }).ToList()
            };

            await _orderRepository.AddOrderAsync(order);
            await _orderRepository.SaveChangesAsync();

            return await MapToDtoAsync(order);
        }

        public async Task CompleteOrderAsync(Guid orderId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            if (order == null)
                throw new NotFoundException("Order not found.");

            if (order.Status != OrderStatus.Pending)
                throw new ConflictException($"Order cannot be completed. Current status: {order.Status}.");

            foreach (var item in order.OrderItems)
            {
                await _inventoryServiceClient.ConfirmStockDeductionAsync(new InventoryChangeDto
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                });
            }

            await _cartServiceClient.DeleteCartAsync(order.UserId);

            order.Status = OrderStatus.Paid;
            order.UpdatedAt = DateTime.Now;
            await _orderRepository.SaveChangesAsync();
        }

        public async Task CancelOrderAsync(Guid orderId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            if (order == null)
                throw new NotFoundException("Order not found.");

            if (order.Status != OrderStatus.Pending)
                throw new ConflictException($"Order cannot be cancelled. Current status: {order.Status}.");

            foreach (var item in order.OrderItems)
            {
                await _inventoryServiceClient.ReleaseStockAsync(new InventoryChangeDto
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                });
            }

            order.Status = OrderStatus.Cancelled;
            order.UpdatedAt = DateTime.Now;
            await _orderRepository.SaveChangesAsync();
        }

        private async Task<OrderDto> MapToDtoAsync(Order order)
        {
            var orderItemDtos = new List<OrderItemDto>();

            foreach (var item in order.OrderItems)
            {
                var product = await _catalogServiceClient.GetProductSnapshotByIdAsync(item.ProductId);

                orderItemDtos.Add(new OrderItemDto
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    ProductName = product?.Name ?? string.Empty,
                    ProductImageUrl = product?.ImageUrl ?? string.Empty,
                    Quantity = item.Quantity,
                    PricePerUnit = item.PricePerUnit,
                    TotalPrice = item.TotalPrice
                });
            }


            return new OrderDto
            {
                Id = order.Id,
                UserId = order.UserId,
                Status = order.Status.ToString(),
                TotalAmount = order.TotalAmount,
                Address = order.Address,
                City = order.City,
                PostalCode = order.PostalCode,
                Country = order.Country,
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt,
                OrderItems = orderItemDtos
            };
        }
    }
}
