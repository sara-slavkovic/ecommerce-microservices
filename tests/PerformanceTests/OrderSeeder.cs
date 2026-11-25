using System.Collections.Concurrent;
using System.Net;
using System.Net.Http.Json;

namespace PerformanceTests
{
    // Creates fresh Pending orders before each run, one cart item and one order per user and round.
    // Carts are emptied before the first round and right after every order, so every order has exactly one item.
    public static class OrderSeeder
    {
        public static async Task<List<SeededOrder>> SeedAsync(
            HttpClient userClient, HttpClient catalogClient, HttpClient cartClient, HttpClient orderClient, int needed)
        {
            Console.WriteLine($"Seed: creating {needed} orders...");

            var users = await userClient.GetFromJsonAsync<List<UserDto>>("/api/users")
                        ?? throw new Exception("Unable to load users.");
            var products = await catalogClient.GetFromJsonAsync<List<ProductDto>>("/api/products")
                           ?? throw new Exception("Unable to load products.");
            if (users.Count == 0) throw new Exception("No users in database.");
            if (products.Count == 0) throw new Exception("No products in database.");

            // 1. Empty all carts first, because failed payments of earlier runs leave their items in the carts
            await Parallel.ForEachAsync(users, new ParallelOptions { MaxDegreeOfParallelism = 8 },
                async (user, ct) => await DeleteCartAsync(cartClient, user.Id, ct));

            var seeded = new ConcurrentBag<SeededOrder>();
            var firstFailureLogged = 0;

            // Prints only the first failure, so the console is not flooded
            void LogFirstFailure(string step, HttpResponseMessage response)
            {
                if (Interlocked.Exchange(ref firstFailureLogged, 1) == 0)
                    Console.WriteLine($"  First failure at '{step}': HTTP {(int)response.StatusCode}");
            }

            var round = 0;
            while (seeded.Count < needed && round < 100)
            {
                var countBefore = seeded.Count;
                var usersThisRound = Math.Min(users.Count, needed - seeded.Count);
                var currentRound = round;

                await Parallel.ForEachAsync(Enumerable.Range(0, usersThisRound),
                    new ParallelOptions { MaxDegreeOfParallelism = 8 },
                    async (i, ct) =>
                    {
                        var user = users[i];
                        var product = products[(i + currentRound) % products.Count];

                        var addToCart = await cartClient.PostAsJsonAsync("/api/carts/items",
                            new CreateCartItemDto(user.Id, product.Id, 1), ct);
                        if (!addToCart.IsSuccessStatusCode) { LogFirstFailure("add to cart", addToCart); return; }

                        var createOrder = await orderClient.PostAsJsonAsync("/api/orders",
                            new CreateOrderDto(user.Id, "Test Street 1", "Belgrade", "11000", "Serbia"), ct);

                        // Whatever happened, empty the cart so the next order of this user again has exactly one item
                        await DeleteCartAsync(cartClient, user.Id, ct);

                        if (!createOrder.IsSuccessStatusCode) { LogFirstFailure("create order", createOrder); return; }

                        var order = await createOrder.Content.ReadFromJsonAsync<OrderDto>(ct);
                        if (order != null) seeded.Add(new SeededOrder(order.Id, order.TotalAmount));
                    });

                round++;
                Console.WriteLine($"  Round {round}: {seeded.Count}/{needed} orders");
                if (seeded.Count == countBefore) break;   // nothing new was created, stop instead of looping forever
            }

            Console.WriteLine($"Seed finished: {seeded.Count} orders.\n");
            return seeded.ToList();
        }

        private static async Task DeleteCartAsync(HttpClient cartClient, Guid userId, CancellationToken ct)
        {
            var response = await cartClient.DeleteAsync($"/api/carts/user/{userId}", ct);

            // 404 means the user has no cart, which is fine
            if (response.StatusCode != HttpStatusCode.NotFound && !response.IsSuccessStatusCode)
                throw new Exception($"Deleting the cart of user {userId} failed: HTTP {(int)response.StatusCode}.");
        }
    }
}
