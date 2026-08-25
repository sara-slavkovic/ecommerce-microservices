using NBomber.Contracts;
using NBomber.Contracts.Stats;
using NBomber.CSharp;
using System.Collections.Concurrent;
using System.Net.Http.Json;

// 1. EXECUTABLE TOP-LEVEL STATEMENTS (Must come first in C#)
var userServiceUrl = "https://localhost:7082";
var catalogServiceUrl = "https://localhost:7038";
var cartServiceUrl = "https://localhost:7252";
var orderServiceUrl = "https://localhost:7015";
var paymentServiceUrl = "https://localhost:7213";

var httpClientHandler = new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = (_, _, _, _) => true
};

using var userClient = new HttpClient(httpClientHandler) { BaseAddress = new Uri(userServiceUrl) };
using var catalogClient = new HttpClient(httpClientHandler) { BaseAddress = new Uri(catalogServiceUrl) };
using var cartClient = new HttpClient(httpClientHandler) { BaseAddress = new Uri(cartServiceUrl) };
using var orderClient = new HttpClient(httpClientHandler) { BaseAddress = new Uri(orderServiceUrl) };
using var paymentClient = new HttpClient(httpClientHandler) { BaseAddress = new Uri(paymentServiceUrl) };

// Run seed phase
var seededOrders = await SeedOrdersAsync(userClient, catalogClient, cartClient, orderClient);

if (seededOrders.Count == 0)
{
    Console.WriteLine("No seeded orders available. Exiting.");
    return;
}

// Thread-safe queue - each order is consumed exactly once across parallel requests
var orderQueue = new ConcurrentQueue<SeededOrder>(seededOrders);

// Scenario Definition
var scenario = Scenario.Create("initiate_payment", async context =>
{
    if (!orderQueue.TryDequeue(out var order))
        return Response.Fail(statusCode: "NO_DATA");

    var response = await paymentClient.PostAsJsonAsync("/api/payments",
        new InitiatePaymentDto(order.OrderId, order.Amount));

    var statusCodeStr = ((int)response.StatusCode).ToString();

    return response.IsSuccessStatusCode
        ? Response.Ok(statusCode: statusCodeStr)
        : Response.Fail(statusCode: statusCodeStr);
})
.WithLoadSimulations(
    Simulation.Inject(rate: 2, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(15))
);

// Runner Execution
NBomberRunner
    .RegisterScenarios(scenario)
    .WithReportFolder("reports")
    .WithReportFileName($"payment-loadtest-{DateTime.Now:yyyyMMdd-HHmmss}")
    .WithReportFormats(ReportFormat.Html, ReportFormat.Csv)
    .Run();

// 2. LOCAL FUNCTIONS (Must come after top-level executable code)
async Task<List<SeededOrder>> SeedOrdersAsync(
    HttpClient userCli, HttpClient catalogCli, HttpClient cartCli, HttpClient orderCli)
{
    Console.WriteLine("Seed: Loading users and products...");

    var users = await userCli.GetFromJsonAsync<List<UserDto>>("/api/users")
                ?? throw new Exception("Unable to load users.");
    var products = await catalogCli.GetFromJsonAsync<List<ProductDto>>("/api/products")
                   ?? throw new Exception("Unable to load products.");

    if (users.Count == 0) throw new Exception("No users in database.");
    if (products.Count == 0) throw new Exception("No products in database.");

    var seeded = new List<SeededOrder>();

    for (var i = 0; i < users.Count; i++)
    {
        var user = users[i];
        var product = products[i % products.Count];

        var addItemResponse = await cartCli.PostAsJsonAsync("/api/carts/items",
            new CreateCartItemDto(user.Id, product.Id, 1));

        if (!addItemResponse.IsSuccessStatusCode)
        {
            Console.WriteLine($"  Skipping user {user.Username}: cart add failed ({addItemResponse.StatusCode})");
            continue;
        }

        var createOrderResponse = await orderCli.PostAsJsonAsync("/api/orders",
            new CreateOrderDto(user.Id, "Test Street 1", "Belgrade", "11000", "Serbia"));

        if (!createOrderResponse.IsSuccessStatusCode)
        {
            Console.WriteLine($"  Skipping user {user.Username}: order create failed ({createOrderResponse.StatusCode})");
            continue;
        }

        var order = await createOrderResponse.Content.ReadFromJsonAsync<OrderDto>();
        if (order == null) continue;

        seeded.Add(new SeededOrder(order.Id, order.TotalAmount));
        Console.WriteLine($"  Order created: {order.Id} ({order.TotalAmount} RSD) for user {user.Username}");
    }

    Console.WriteLine($"Seed finished: {seeded.Count} orders ready for payment.\n");
    return seeded;
}

// 3. TYPE / RECORD DECLARATIONS (Must come at the very bottom)
record UserDto(Guid Id, string Username);
record ProductDto(Guid Id);
record CreateCartItemDto(Guid UserId, Guid ProductId, int Quantity);
record CreateOrderDto(Guid UserId, string Address, string City, string PostalCode, string Country);
record OrderDto(Guid Id, Guid UserId, string Status, decimal TotalAmount);
record InitiatePaymentDto(Guid OrderId, decimal Amount);
record SeededOrder(Guid OrderId, decimal Amount);