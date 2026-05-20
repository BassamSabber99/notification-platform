# Scalable Notification Platform

A high-performance, multi-tenant notification system built with .NET 8 that handles 10M+ messages/day with support for Email, SMS, Push Notifications, and WhatsApp.

## Features

- **Multi-Channel Support**: Email, SMS, Push Notifications, WhatsApp
- **High Performance**: 10M+ messages/day capacity
- **Distributed Architecture**: Message queues, caching, asynchronous processing
- **Retry Mechanism**: Exponential backoff with configurable policies
- **Rate Limiting**: Per-provider rate limiting with token bucket algorithm
- **Multi-Tenant**: Complete tenant isolation with dedicated configurations
- **Audit Logging**: Complete message lifecycle tracking
- **Monitoring & Alerts**: Real-time metrics and health checks
- **High Availability**: Load balancing, database replication, circuit breakers

## Architecture

```
┌─────────────┐
│   API       │
│  Gateway    │
└──────┬──────┘
       │
       ▼
┌──────────────────────────────────────┐
│      Notification Service            │
│  ┌────────────────────────────────┐  │
│  │  Request Validation            │  │
│  │  Rate Limiting                 │  │
│  │  Tenant Isolation              │  │
│  └────────────────────────────────┘  │
└──────┬──────────────────────────────┘
       │
       ▼
┌──────────────────────────────────────┐
│    Message Queue (RabbitMQ/SQS)      │
│  ┌────────────────────────────────┐  │
│  │  Email Queue                   │  │
│  │  SMS Queue                     │  │
│  │  Push Notification Queue       │  │
│  │  WhatsApp Queue                │  │
│  └──────���─────────────────────────┘  │
└──────┬──────────────────────────────┘
       │
       ├─────┬─────────┬─────────┬─────────┐
       ▼     ▼         ▼         ▼         ▼
    ┌────┐┌────┐   ┌────┐   ┌────┐   ┌────┐
    │Email││SMS │   │Push│   │WhatsApp│ │
    │Svc  ││Svc │   │Svc │   │  Svc  │ │
    └────┘└────┘   └────┘   └────┘   └────┘
       │     │         │        │        │
       └─────┴─────────┴────────┴────────┘
              │
              ▼
    ┌─────────────────────┐
    │  Database           │
    │  - Messages         │
    │  - Audit Logs       │
    │  - Tenant Config    │
    │  - Rate Limits      │
    └─────────────────────┘
              │
              ▼
    ┌─────────────────────┐
    │  Monitoring Stack   │
    │  - Prometheus       │
    │  - Grafana          │
    │  - ELK              │
    └─────────────────────┘
```

## Project Structure

```
notification-platform/
├── src/
│   ├── NotificationPlatform.Api/           # API Gateway
│   ├── NotificationPlatform.Core/          # Domain models & interfaces
│   ├── NotificationPlatform.Infrastructure/# Data access, external integrations
│   ├── NotificationPlatform.Services/      # Business logic
│   ├── NotificationPlatform.Workers/       # Background workers
│   └── NotificationPlatform.Common/        # Utilities, helpers
├── tests/
│   ├── NotificationPlatform.Tests/         # Unit tests
│   └── NotificationPlatform.IntegrationTests/
├── docker-compose.yml
├── kubernetes/                             # K8s manifests
└── docs/
```

## Quick Start

### Prerequisites
- .NET 8 SDK
- PostgreSQL 14+
- Redis 7+
- RabbitMQ 3.12+
- Docker & Docker Compose

### Installation

```bash
# Clone repository
git clone https://github.com/BassamSabber99/notification-platform.git
cd notification-platform

# Restore dependencies
dotnet restore

# Run with Docker Compose
docker-compose up -d

# Apply migrations
dotnet ef database update --project src/NotificationPlatform.Infrastructure

# Start API
dotnet run --project src/NotificationPlatform.Api
```

## Configuration

### appsettings.json

```json
{
  "NotificationPlatform": {
    "MessageQueueSettings": {
      "Provider": "RabbitMQ",
      "ConnectionString": "amqp://guest:guest@localhost:5672/"
    },
    "CacheSettings": {
      "Provider": "Redis",
      "ConnectionString": "localhost:6379"
    },
    "RateLimiting": {
      "Email": { "RequestsPerMinute": 1000 },
      "SMS": { "RequestsPerMinute": 500 },
      "Push": { "RequestsPerMinute": 2000 },
      "WhatsApp": { "RequestsPerMinute": 300 }
    },
    "RetryPolicy": {
      "MaxRetries": 5,
      "InitialDelaySeconds": 30,
      "BackoffMultiplier": 2.0
    }
  }
}
```

## API Endpoints

### Send Notification

```http
POST /api/v1/notifications/send
Content-Type: application/json
Authorization: Bearer {token}

{
  "tenantId": "tenant-123",
  "channels": ["email", "sms"],
  "recipient": {
    "email": "user@example.com",
    "phoneNumber": "+1234567890"
  },
  "subject": "Welcome",
  "body": "Welcome to our platform",
  "metadata": {
    "customField": "value"
  }
}
```

### Get Message Status

```http
GET /api/v1/notifications/{messageId}
Authorization: Bearer {token}
```

### Get Audit Logs

```http
GET /api/v1/audit-logs?tenantId={tenantId}&pageNumber=1&pageSize=50
Authorization: Bearer {token}
```

## Monitoring & Alerts

### Key Metrics
- Messages per second (by channel)
- Failed message rate
- Retry counts
- Provider response times
- Queue depths
- Cache hit rates

### Grafana Dashboards
- Real-time message flow
- Error rates and types
- Provider performance
- Tenant usage
- System health

## Performance Benchmarks

| Metric | Target | Current |
|--------|--------|----------|
| Messages/sec | 115+ | - |
| P95 Latency | <500ms | - |
| Error Rate | <0.1% | - |
| Availability | 99.99% | - |

## Contributing

See [CONTRIBUTING.md](./CONTRIBUTING.md)

## License

MIT License - see LICENSE file

## Support

For issues and questions, please open an issue on GitHub.