using NBomber.Contracts.Stats;
using NBomber.CSharp;
using PerformanceTests;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http.Json;

// Usage: <E1|E2|E3> <variant> <failureRate> <repetition> [requestsPerSecond=5] [durationSeconds=40]
// Example: E1 V2 0.4 1
if (args.Length < 4)
{
    Console.WriteLine("Usage: <E1|E2|E3> <variant> <failureRate> <repetition> [rps] [seconds]");
    return;
}

var experiment = args[0];
var variant = args[1];
var failureRate = double.Parse(args[2], CultureInfo.InvariantCulture);
var repetition = int.Parse(args[3]);
var rps = args.Length > 4 ? int.Parse(args[4]) : 5;
var durationSec = args.Length > 5 ? int.Parse(args[5]) : 40;

// E3 uses a gateway that always fails; E1 and E2 use random transient failures
var gatewayMode = experiment == "E3" ? "AlwaysFail" : "RandomTransient";

const int warmupCount = 50;                         // orders used for warm-up (not measured)
var needed = rps * durationSec + warmupCount + 1;  // warm-up + measured orders + small reserve

var userServiceUrl = "https://localhost:7082";
var catalogServiceUrl = "https://localhost:7038";
var cartServiceUrl = "https://localhost:7252";
var orderServiceUrl = "https://localhost:7015";
var paymentServiceUrl = "https://localhost:7213";

var httpClientHandler = new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = (_, _, _, _) => true,  // local dev certificates
    MaxConnectionsPerServer = 200
};

using var userClient = new HttpClient(httpClientHandler) { BaseAddress = new Uri(userServiceUrl) };
using var catalogClient = new HttpClient(httpClientHandler) { BaseAddress = new Uri(catalogServiceUrl) };
using var cartClient = new HttpClient(httpClientHandler) { BaseAddress = new Uri(cartServiceUrl) };
// Local test only: same internal key the services use, needed to delete carts
cartClient.DefaultRequestHeaders.Add("X-Internal-Api-Key", "ZVz3nrKODuKsVl7P");
using var orderClient = new HttpClient(httpClientHandler) { BaseAddress = new Uri(orderServiceUrl) };
// Long timeout so that slow retry sequences (for example 5 retries, about 31 s) are never cut off
using var paymentClient = new HttpClient(httpClientHandler)
{
    BaseAddress = new Uri(paymentServiceUrl),
    Timeout = TimeSpan.FromMinutes(5)
};

// 1. Seed: create fresh Pending orders (not part of the measurement)
var seededOrders = await OrderSeeder.SeedAsync(userClient, catalogClient, cartClient, orderClient, needed);
if (seededOrders.Count < needed)
{
    Console.WriteLine($"Created {seededOrders.Count} orders, {needed} needed. Aborting.");
    return;
}

// 2. Warm-up: variant V0 with a gateway that always succeeds, so the Circuit Breaker windows stay untouched
await SetConfigAsync(paymentClient, "V0", "AlwaysSucceed", 0);
for (var i = 0; i < warmupCount; i++)
{
    var warmupOrder = seededOrders[i];
    await paymentClient.PostAsJsonAsync("/api/payments", new InitiatePaymentDto(warmupOrder.OrderId, warmupOrder.Amount));
}

// 2b. Reset the Circuit Breaker of this variant (only V1 and V3 have one).
// An open circuit does not close by itself: the first request after the break acts as a probe.
// So we send one successful request as that probe, then wait until the 60 s sampling window is empty.
if (variant is "V1" or "V3")
{
    await SetConfigAsync(paymentClient, variant, "AlwaysSucceed", 0);
    Console.WriteLine("Resetting circuit breaker...");
    await Task.Delay(TimeSpan.FromSeconds(16));       // the 15 s break duration has passed
    var probeOrder = seededOrders[^1];                // the last order is reserved for this probe
    await paymentClient.PostAsJsonAsync("/api/payments", new InitiatePaymentDto(probeOrder.OrderId, probeOrder.Amount));
    await Task.Delay(TimeSpan.FromSeconds(61));       // the 60 s sampling window is empty again
}

// 3. Real configuration for this run (variant and failure rate)
await SetConfigAsync(paymentClient, variant, gatewayMode, failureRate);

// Each order is consumed exactly once by one request
var orderQueue = new ConcurrentQueue<SeededOrder>(seededOrders.Skip(warmupCount).Take(rps * durationSec));
var results = new ConcurrentBag<Sample>();

// Counts requests that were actually started, so we can wait for all of them to finish after the load ends
var started = new int[1];

//var errorLogged = new int[1];   // makes sure only the first error body is printed

Directory.CreateDirectory("results");
var errorLock = new object();   // protects the error log file from parallel writes

var scenario = Scenario.Create("initiate_payment", async context =>
{
    if (!orderQueue.TryDequeue(out var order))
        return Response.Fail(statusCode: "NO_DATA");

    Interlocked.Increment(ref started[0]);

    var stopwatch = Stopwatch.StartNew();
    var httpStatus = 0;
    var paymentStatus = "Error";   // stays "Error" if the request throws
    var attempts = 0;              // number of recorded gateway calls for this order
    try
    {
        var response = await paymentClient.PostAsJsonAsync("/api/payments",
            new InitiatePaymentDto(order.OrderId, order.Amount));
        httpStatus = (int)response.StatusCode;

        //// Print the body of the first error response, to see why the API refused the payment
        //if (!response.IsSuccessStatusCode && Interlocked.Exchange(ref errorLogged[0], 1) == 0)
        //    Console.WriteLine($"First error body: {await response.Content.ReadAsStringAsync()}");
        // Save the body of every error response to a file (NBomber redraws the console, so messages get lost there)
        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            lock (errorLock)
                File.AppendAllText("results/errors.txt",
                    $"{DateTime.Now:HH:mm:ss} order={order.OrderId} http={httpStatus} {errorBody}{Environment.NewLine}");
        }

        if (response.IsSuccessStatusCode)
        {
            // The API answers 2xx even when the payment itself failed; the real outcome is in "Status"
            var body = await response.Content.ReadFromJsonAsync<PaymentResponse>();
            paymentStatus = body?.Status ?? "Error";
            attempts = body?.PaymentAttempts?.Count ?? 0;
        }
    }
    catch
    {
        // keep defaults, the request is counted as an HTTP error
    }
    stopwatch.Stop();

    results.Add(new Sample(stopwatch.Elapsed.TotalMilliseconds, paymentStatus, attempts, httpStatus));
    return httpStatus is >= 200 and < 300
        ? Response.Ok(statusCode: httpStatus.ToString())
        : Response.Fail(statusCode: httpStatus.ToString());
})
.WithoutWarmUp()
.WithLoadSimulations(
    // constant arrival rate: rps requests per second for durationSec seconds
    Simulation.Inject(rate: rps, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(durationSec))
);

var runName = $"{experiment}_{variant}_{failureRate.ToString("0.##", CultureInfo.InvariantCulture)}_{repetition}";
NBomberRunner
    .RegisterScenarios(scenario)
    .WithReportFolder($"reports/{runName}")
    .WithReportFileName(runName)
    .WithReportFormats(ReportFormat.Html, ReportFormat.Csv)
    .Run();

// Slow requests (for example long retry sequences) may still be running when NBomber stops.
// Wait until every started request has finished (at most 120 s), so no measurement is cut off.
var waitUntil = DateTime.UtcNow.AddSeconds(120);
while (results.Count < Volatile.Read(ref started[0]) && DateTime.UtcNow < waitUntil)
    Thread.Sleep(500);

if (results.Count < started[0])
    Console.WriteLine($"Warning: {started[0] - results.Count} requests did not finish in time.");

// 4. Metrics for this run, appended as one row to results/experiments.csv
var all = results.ToArray();
if (all.Length == 0)
{
    Console.WriteLine("No results collected.");
    return;
}

var apiAnswered = all.Where(s => s.Http is >= 200 and < 300).ToArray();
var succeeded = all.Where(s => s.PaymentStatus == "Succeeded").ToArray();
var latencies = all.Select(s => s.LatencyMs).OrderBy(x => x).ToArray();
var latenciesSucceeded = succeeded.Select(s => s.LatencyMs).OrderBy(x => x).ToArray();

// Percentile on a sorted array (nearest-rank method)
double Percentile(double[] sorted, double p) =>
    sorted.Length == 0 ? double.NaN : sorted[Math.Max(0, (int)Math.Ceiling(p * sorted.Length) - 1)];
string Format(double d) => double.IsNaN(d) ? "" : d.ToString("0.##", CultureInfo.InvariantCulture);

var mean = latencies.Average();
var stdev = latencies.Length > 1
    ? Math.Sqrt(latencies.Sum(x => (x - mean) * (x - mean)) / (latencies.Length - 1))
    : 0;
var successRatePct = 100.0 * succeeded.Length / all.Length;                                              // payments with status Succeeded
var callsPerOrder = apiAnswered.Length > 0 ? apiAnswered.Average(s => (double)s.Attempts) : double.NaN;  // gateway calls per order
var blockedPct = apiAnswered.Length > 0
    ? 100.0 * apiAnswered.Count(s => s.Attempts == 0) / apiAnswered.Length
    : double.NaN;                                                                                        // orders that never reached the gateway (circuit open)
var httpErrors = all.Count(s => s.Http is < 200 or >= 300);

Directory.CreateDirectory("results");
var file = "results/experiments.csv";
if (!File.Exists(file))
    File.WriteAllText(file,
        "experiment,variant,failureRate,rep,rps,durationSec,requests,successRatePct,p50Ms,p95Ms,p95SucceededMs," +
        "callsPerOrder,blockedPct,httpErrors,meanMs,minMs,maxMs,stdevMs\n");

var row = string.Join(",", new object[]
{
    experiment, variant, Format(failureRate), repetition, rps, durationSec, all.Length,
    Format(successRatePct), Format(Percentile(latencies, 0.5)), Format(Percentile(latencies, 0.95)),
    Format(Percentile(latenciesSucceeded, 0.95)), Format(callsPerOrder), Format(blockedPct), httpErrors,
    Format(mean), Format(latencies.First()), Format(latencies.Last()), Format(stdev)
});
File.AppendAllText(file, row + "\n");

Console.WriteLine($"\nRESULT {runName}: successRate={Format(successRatePct)}%  p50={Format(Percentile(latencies, 0.5))}  " +
                  $"p95={Format(Percentile(latencies, 0.95))}  callsPerOrder={Format(callsPerOrder)}  " +
                  $"blocked={Format(blockedPct)}%  httpErrors={httpErrors}");

// Tells the payment service which gateway variant, simulation mode and failure rate to use
static async Task SetConfigAsync(HttpClient client, string variant, string mode, double failureRate)
{
    var response = await client.PostAsJsonAsync("/api/experiment/config", new { variant, mode, failureRate });
    response.EnsureSuccessStatusCode();
}