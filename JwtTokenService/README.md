# JwtTokenService

Background microservice that listens to RabbitMQ requests, issues minimal JWT tokens (containing a userId claim), and publishes responses back to the Gateway queue so the Gateway can forward them to SignalR clients.

Behavior

- Listens on the request queue configured under `RabbitMQ:RequestQueueName` (default: `services.jwt.request-queue`).
- Expects a payload containing a user id (payload may be a JSON string like `{ "userId": "..." }` or a plain string user id).
- Publishes a response to the queue configured under `RabbitMQ:ResponseQueueName` (default: `services.gateway.request-queue`) with route `jwt.response` and payload `{ token, userId }`.

Notes

- Uses symmetric key in `appsettings.json` under `Jwt:Key`. Replace with a secure key or provide via environment variable in production.
- Connection registry is still available in-memory. For multi-instance deployments you must replace this with a shared store (Redis, DB, etc.).

How to build & run

1. cd JwtTokenService
2. dotnet build
3. dotnet run

The service runs as a background worker and does not expose HTTP endpoints.
