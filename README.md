# WebhookEngine 🚀

> A production-grade, portfolio-ready .NET 10 Webhook delivery infrastructure library. Designed for distributed systems with true enterprise reliability in mind.

WebhookEngine is an open-source, scalable webhook delivery system that handles exactly what a modern distributed system needs: strict idempotency, durable queues, circuit breaking, and dead-lettering. 

I built this because I noticed that most "webhook tutorials" just fire a `HttpClient.PostAsync` and call it a day. In the real world, endpoints fail, servers restart, and customer systems go down. This library solves that using **Polly**, **EF Core**, and a **Clean Vertical Slice Architecture**.

---

## 🌟 Key Features

### Distributed-Systems First
- **Idempotency Guarantee:** Deduplication based on the `Event-Id` header (At-least-once delivery with strict idempotency support).
- **Exponential Backoff & Jitter:** Uses Polly's `Backoff.DecorrelatedJitterBackoffV2` algorithm to prevent thundering herds on recovery.
- **Per-Destination Circuit Breakers:** If one customer's endpoint dies, only their circuit opens—other endpoints are unaffected.
- **Dead Letter Queue (DLQ):** Auto-sweeping after 8 failed attempts, with manual replay support.

### Production-Grade Polish
- **Security:** HMAC-SHA256 Payload Signing (`X-Webhook-Signature`) and timestamp headers to prevent replay attacks.
- **Multi-Tenant Design:** Tenant isolation built into the core domain.
- **Observability:** Structured JSON logging ready for OpenTelemetry.

---

## 🏗️ Architecture

Instead of traditional layered mud, WebhookEngine uses **Vertical Slice Architecture** for the API and a pure **Domain-Driven Design (DDD)** core.

```
WebhookEngine.Domain       (No dependencies, pure C# records)
WebhookEngine.Core         (The library itself, HMAC signing, Delivery orchestration)
WebhookEngine.Infrastructure (EF Core configurations, PostgreSQL integration)
WebhookEngine.Api          (Minimal API endpoints mapped by feature)
WebhookEngine.Worker       (Background IHostedService for processing pending deliveries)
```

## 🚀 Getting Started

1. Ensure you have Docker running for PostgreSQL.
2. Spin up the infrastructure:
   ```bash
   docker-compose up -d
   ```
3. Run the API and the Worker simultaneously.
   ```bash
   dotnet run --project WebhookEngine.Api
   dotnet run --project WebhookEngine.Worker
   ```

### 1. Registering an Endpoint

```bash
POST /api/endpoints
{
  "tenantId": "c0a80121-0000-0000-0000-000000000000",
  "url": "https://httpbin.org/post",
  "subscribedEvents": ["invoice.*"]
}
```

### 2. Sending an Event

```bash
POST /api/webhooks/send
{
  "tenantId": "c0a80121-0000-0000-0000-000000000000",
  "eventType": "invoice.created",
  "payloadJson": "{\"invoice_id\": 1001, \"amount\": 250.00}",
  "idempotencyKey": "evt_998877"
}
```
*The Worker will immediately pick this up from PostgreSQL and dispatch it to `httpbin.org/post` with a valid `X-Webhook-Signature`.*

---

## 🛠️ How to Add a New Feature

This project uses **Vertical Slices**. To add a new API endpoint (e.g., `GetDeliveryHistory`):

1. **Create a Folder:** Navigate to `WebhookEngine.Api/Features/` and create `GetDeliveryHistory/`.
2. **Add the Code:** Create `GetDeliveryHistoryFeature.cs` containing your Minimal API `MapEndpoint()` method.
3. **Register It:** Call `GetDeliveryHistoryFeature.MapEndpoint(app);` in `Program.cs`.

*No hunting across 5 projects to wire up controllers, interfaces, and services!*

---

*SEO tags: .NET 10 Webhooks, C# Webhook Architecture, Distributed Systems .NET, Polly Retry C#, EF Core PostgreSQL JSONB, Minimal API Vertical Slice Architecture, Open Source Webhook Delivery C#, C# Idempotency Pattern, CQRS C# Example, Senior .NET Developer Portfolio.*
