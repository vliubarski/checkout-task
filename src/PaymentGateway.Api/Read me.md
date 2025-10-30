# Payment Gateway - Design Considerations & Assumptions

## 1. Overall Architecture

The system follows a **clean layered architecture**, divided into
distinct areas of responsibility:

-   **API Layer (Controllers)** -- Handles HTTP requests and responses
    (e.g., `PaymentsController`).
-   **Application Layer (Services)** -- Implements core business logic
    (e.g., `PaymentService`, `PaymentValidator`).
-   **Infrastructure Layer** -- Manages integration with external
    systems (e.g., `BankService`, repositories).
-   **Domain / Models Layer** -- Contains business entities and DTOs
    (e.g., `Payment`, `Currency`).

This structure ensures a clear separation of concerns, better
testability, and easier maintenance.

------------------------------------------------------------------------

## 2. Design Principles & Key Patterns

### Dependency Injection (DI)

All services and repositories are registered in the DI container for
flexibility and testability.\

###  Null Object Pattern

Instead of returning `null` for missing payments, the service returns a
`GetPaymentNullResponse`.\
This avoids `null` checks and improves API readability.

###  DTO Separation

Dedicated DTOs are used for external API contracts: -
`PostPaymentRequest` (incoming request) - `GetPaymentResponse`,
`ProcessPaymentResult` (outgoing responses)


###  Validation

Validation is encapsulated in `PaymentValidator`.\
It checks for: - Card number format and length - Expiry month/year
validity - Supported currency (via `Currency` enum) - Positive amount -
CVV format


###  External Gateway Integration

`BankService` uses an injected `HttpClient` via `AddHttpClient` to
interact with a mock or real bank API.\
Responses are deserialized using `System.Text.Json` with
case-insensitive settings and optional converters like
`NullableGuidConverter`.

------------------------------------------------------------------------

## 3. Testing Strategy

###  Unit Tests

-   **Services** -- `PaymentServiceTests` validate core payment logic
    and error handling.
-   **Validator** -- `PaymentValidatorTests` cover all validation paths.
-   **Repository** -- `PaymentsRepositoryTests` ensure add/retrieve
    consistency.
-   **Converters** -- `NullableGuidConverterTests` confirm correct
    (de)serialization.

###  Integration Tests

-   Controller-level tests simulate real HTTP calls using
    `WebApplicationFactory`.
-   `PaymentsRepository` is injected as a singleton to persist across
    requests.

------------------------------------------------------------------------

## 4. Assumptions

1.  The project runs in a **.NET 8** environment.
2.  The **bank endpoint** (mock) is accessible at
    `http://localhost:8080/payments`.
3.  No persistent database is used --- the repository is in-memory for
    simplicity.
4.  Added a human-readable PaymentStatusDescription field to improve API 
    clarity - this duplicates the status value intentionally for better 
    visibility in responses.

------------------------------------------------------------------------

## 5. Future Improvements

-   Replace in-memory repository with a real database (e.g., EF Core, SQL).
-   Add retry policies and circuit breakers around `BankService` (Polly integration).
-   Introduce AutoMapper for DTO-to-entity conversions.
-   Implement better logging and telemetry (structured logs, correlation IDs).
