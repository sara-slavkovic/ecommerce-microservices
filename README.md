# E-Commerce Microservices

A modular e-commerce system (**Beauty Store**) built with .NET microservices on the backend and a React frontend, using Clean Architecture and resilience patterns for inter-service communication.

## Tech Stack

**Backend**
- .NET 10 / ASP.NET Core Web API
- SQL Server 17
- Entity Framework Core 10.0.10
- Clean Architecture (Domain / Application / Infrastructure / Api layers per service)
- FluentValidation for request validation
- Polly for resilience (retry, timeout, circuit breaker, rate limiter, and fallback – e.g. Cart falls back to cached product images when CatalogService is unavailable) on inter-service HTTP calls
- Global exception handling via `IExceptionHandler` (ProblemDetails responses)
- Swashbuckle 10.2.3 (Swagger/OpenAPI documentation)

**Frontend**
- Node.js 24.18.0 / npm 11.16.0
- React 19.2.7 + React DOM 19.2.7
- Vite 8.1.1
- React Router DOM 7.18.1
- Axios 1.18.1
- Context API for global state (auth session, cart count, toasts, confirm dialogs)

## Architecture

Each business capability (users, products, cart, orders, payments) runs as its own independent microservice. Every service follows Clean Architecture – split into Domain, Application, Infrastructure, and Api layers. Services communicate over HTTP; internal-only endpoints are protected with an API key filter (`[InternalApiKey]`).

### Services
- **UserService** – registration, login, profile management
- **CatalogService** – products, categories, product image uploads
- **InventoryService** – stock tracking, reserve/release/confirm/restock (used internally by Cart/Order flows)
- **CartService** – shopping cart per user (add/update/remove items)
- **OrderService** – order creation and lifecycle (Created → Pending → Paid/Cancelled)
- **PaymentService** – payment initiation, talks to the Mock Payment Gateway, updates Order status
- **MockPaymentGateway** – simulates a payment processor with configurable simulation modes (AlwaysSucceed, AlwaysFail, AlwaysFailFatal, Random, FailNTimesThenSucceed, SlowResponse) for testing resilience

A shared **SharedKernel** library holds cross-cutting concerns reused across all services – common domain exceptions, the base exception handlers, and the `[InternalApiKey]` filter.

### Resilience
PaymentService → MockPaymentGateway and other inter-service calls use Polly policies (timeout, retry, circuit breaker). Simulation mode is read via `IOptionsSnapshot`, so it can be changed live in `appsettings.json` without restarting the service.

A custom `DelegatingHandler` (`PaymentAttemptTrackingHandler`) captures every individual attempt made by the HTTP client – including retries – and persists them to the database (`PaymentAttempts` table), giving full visibility into how Polly behaved for each payment.

Load-tested with NBomber under multiple failure scenarios (transient failures, permanent outages, slow responses, business-rule rejections, and recovery-after-failure), comparing behavior with and without Polly to validate retry backoff, circuit breaker trip/half-open behavior, and timeout cutoffs under concurrent load.

### Idempotency & data consistency
- MockPaymentGateway caches charge results by `IdempotencyKey`, so retried requests (e.g. after a client-side timeout) return the original outcome instead of double-charging.
- Order completion/cancellation use flag-based idempotency (`IsInventoryConfirmed`, `IsCartDeleted`, `IsStockReleased`) so partial failures between services don't leave orders stuck in an inconsistent state.

## Running the Backend

Each service has its own `launchSettings.json` with an `https` profile. When running via `dotnet run` directly (not through Visual Studio), specify the profile explicitly:
```
cd src/Services/<ServiceName>/<ServiceName>.Api
dotnet run --launch-profile https
```

Repeat for each service in its own terminal (UserService, CatalogService, InventoryService, CartService, OrderService, PaymentService, MockPaymentGateway), or use Visual Studio's **Multiple Startup Projects** to launch them all together.

Each service listens on its own HTTPS port (see each project's `launchSettings.json`). CORS is enabled on every service to allow requests from the frontend at `http://localhost:5173`.

### Payment simulation
Change `PaymentSimulationSettings:Mode` in PaymentService's `appsettings.json` (values: `AlwaysSucceed`, `AlwaysFail`, `AlwaysFailFatal`, `Random`, `FailNTimesThenSucceed`, `SlowResponse`) to test different checkout outcomes without restarting the service.

## Running the Frontend
```
cd frontend
npm install
npm run dev
```

App runs at `http://localhost:5173`. Update service ports in `src/api/axiosInstances.js` if your local ports differ.

### Frontend structure
```
src/
api/ # one file per service, wraps axios calls
components/ # reusable UI (Navbar, ProductCard, QuantityInput, Spinner, etc.)
hooks/ # useAuth, useCart, useToast, useConfirm (Context-based global state)
pages/ # route-level views
```

### Key frontend features
- Login/Register with session persisted in `localStorage`
- Product catalog with category filtering and product detail pages
- Cart with live quantity updates and item count badge
- Checkout flow: shipping → payment (simulated card entry) → order result
- Order history
- Profile editing
- Admin panel (`/admin/products`, restricted to a specific username) for creating, editing, deleting products and restocking inventory
- Centralized error handling that surfaces backend `ProblemDetails` messages, with graceful fallback messages when a service is unavailable
- Toast notifications and custom confirm dialogs (no native browser alerts)
