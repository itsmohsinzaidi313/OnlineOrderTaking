# Patroni PostgreSQL High Availability Cluster

This project implements a high-availability PostgreSQL cluster using Patroni with etcd for distributed configuration management, HAProxy for load balancing, and a .NET microservice.

## Architecture

```
.NET Microservice → HAProxy → Patroni Cluster (2 PostgreSQL nodes)
                      ↓
                  etcd (Distributed Configuration)
```

## Components

### etcd
- Provides distributed configuration storage for Patroni
- Manages cluster state and leader election
- Single node setup for simplicity (can be scaled to 3 nodes for production)

### Patroni PostgreSQL Cluster (2 nodes)
- **patroni1**: Primary candidate node
- **patroni2**: Replica/standby node
- Automatic failover and switchover capabilities
- Uses PostgreSQL 15 image
- Streaming replication between nodes

### HAProxy Load Balancer
- Routes write operations to the current primary node
- Health checks via Patroni REST API endpoints
- Stats interface available at http://localhost:8404
- Automatic failover detection

### .NET Microservice
- ASP.NET Core 8 API
- Connects through HAProxy for high availability
- Product management API with Entity Framework

## Getting Started

### Prerequisites
- Docker and Docker Compose
- PowerShell (for management script)

### 1. Start the Cluster

#### Using PowerShell Management Script:
```powershell
# Start the entire cluster
.\manage-patroni-cluster.ps1 -Action start

# Check cluster status
.\manage-patroni-cluster.ps1 -Action status

# View logs
.\manage-patroni-cluster.ps1 -Action logs
```

#### Using Docker Compose directly:
```bash
# Start etcd first
docker-compose up -d etcd

# Wait for etcd to be ready, then start Patroni nodes
docker-compose up -d patroni1 patroni2

# Start HAProxy and microservice
docker-compose up -d haproxy microservice
```

### 2. Verify Cluster Status

```bash
# Check container status
docker-compose ps

# Check Patroni cluster status
docker exec patroni1 patronictl -c /etc/patroni.yml list

# Check etcd health
docker exec etcd etcdctl endpoint health
```

### 3. Test the Setup

```bash
# Test microservice
curl http://localhost:8080/products

# Add a product
curl -X POST http://localhost:8080/products \
  -H "Content-Type: application/json" \
  -d '{"name":"Test Product","price":19.99,"description":"A test product"}'
```

## Ports and Endpoints

| Service | Port | Description |
|---------|------|-------------|
| HAProxy (PostgreSQL) | 5433 | Primary database connection |
| Patroni1 (Direct) | 5433 | Direct connection to node 1 |
| Patroni2 (Direct) | 5434 | Direct connection to node 2 |
| Patroni1 REST API | 8008 | Patroni management API |
| Patroni2 REST API | 8009 | Patroni management API |
| Microservice | 8080 | .NET API |
| HAProxy Stats | 8404 | Load balancer statistics |
| etcd | 2389 | etcd client API |

## Connection Details

- **Database Host**: localhost (via HAProxy)
- **Database Port**: 5433
- **Database Name**: mydb
- **Username**: postgres
- **Password**: postgrespass
- **Replication User**: replicator
- **Replication Password**: replicatorpass

## High Availability Features

### Automatic Failover
- If the primary node fails, Patroni automatically promotes a replica
- HAProxy detects the change via health checks
- Applications continue to work with minimal downtime

### Health Monitoring
- Patroni REST API provides health status
- HAProxy performs regular health checks
- Failed nodes are automatically removed from rotation

### Streaming Replication
- Real-time data replication between nodes
- Configurable synchronous/asynchronous replication
- Point-in-time recovery capabilities

## Management Commands

### Using the PowerShell Script

```powershell
# Start cluster
.\manage-patroni-cluster.ps1 -Action start

# Stop cluster
.\manage-patroni-cluster.ps1 -Action stop

# Restart cluster
.\manage-patroni-cluster.ps1 -Action restart

# Show cluster status
.\manage-patroni-cluster.ps1 -Action status

# View logs for specific node
.\manage-patroni-cluster.ps1 -Action logs -Node patroni1

# Trigger failover
.\manage-patroni-cluster.ps1 -Action failover

# Switchover to specific node
.\manage-patroni-cluster.ps1 -Action switchover -Target patroni2

# Clean up (removes all data)
.\manage-patroni-cluster.ps1 -Action cleanup
```

### Manual Patroni Commands

```bash
# Show cluster status
docker exec patroni1 patronictl -c /etc/patroni.yml list

# Trigger manual failover
docker exec patroni1 patronictl -c /etc/patroni.yml failover --force

# Perform switchover to specific node
docker exec patroni1 patronictl -c /etc/patroni.yml switchover --candidate patroni2 --force

# Restart a node
docker exec patroni1 patronictl -c /etc/patroni.yml restart patroni1

# Reload configuration
docker exec patroni1 patronictl -c /etc/patroni.yml reload patroni1
```

## Monitoring

### HAProxy Statistics
Visit http://localhost:8404 to view:
- Backend server status
- Connection statistics
- Health check results
- Traffic distribution

### Patroni REST API
```bash
# Check node status
curl http://localhost:8008/

# Check if node is primary
curl http://localhost:8008/primary

# Check if node is replica
curl http://localhost:8008/replica

# Get node configuration
curl http://localhost:8008/config
```

### Database Monitoring
```bash
# Connect to database via HAProxy
docker exec -it haproxy psql -h patroni1 -U postgres -d mydb

# Check replication status
docker exec patroni1 psql -U postgres -c "SELECT * FROM pg_stat_replication;"

# Check database size
docker exec patroni1 psql -U postgres -c "SELECT pg_size_pretty(pg_database_size('mydb'));"
```

## Troubleshooting

### Common Issues

1. **Cluster not starting**
   - Check etcd logs: `docker logs etcd`
   - Ensure etcd is running before starting Patroni nodes
   - Check network connectivity between containers

2. **No primary elected**
   - Verify etcd is healthy: `docker exec etcd etcdctl endpoint health`
   - Check Patroni logs: `docker logs patroni1`
   - Ensure bootstrap configuration is correct

3. **Connection failures**
   - Check HAProxy stats at http://localhost:8404
   - Verify Patroni health endpoints
   - Check firewall and network settings

4. **Replication issues**
   - Check replication user credentials
   - Verify pg_hba.conf settings
   - Monitor replication lag

### Logs and Debugging

```bash
# View all logs
docker-compose logs -f

# View specific service logs
docker logs patroni1 -f
docker logs patroni2 -f
docker logs etcd -f
docker logs haproxy -f

# Check Patroni configuration
docker exec patroni1 cat /etc/patroni.yml

# Check PostgreSQL logs
docker exec patroni1 tail -f /var/lib/postgresql/data/pgdata/log/postgresql-*.log
```

### Reset and Recovery

```bash
# Stop all services
docker-compose down

# Remove all data (WARNING: This deletes everything)
docker-compose down -v
docker volume prune -f

# Start fresh
docker-compose up -d
```

## Production Considerations

1. **etcd Cluster**: Scale to 3 nodes for production
2. **Security**: Use SSL/TLS for all connections
3. **Backup**: Implement regular backup strategy
4. **Monitoring**: Add comprehensive monitoring and alerting
5. **Resources**: Tune memory and CPU allocations
6. **Storage**: Use persistent volumes with appropriate storage class

## Architecture

```
Microservice → HAProxy → Patroni (Primary/Replica PostgreSQL nodes)
                ↓
           Health Checks & Load Balancing
```

## Components

### Etcd Cluster (3 nodes)
- Provides distributed configuration and leader election for Patroni
- Ensures consensus and coordination between PostgreSQL nodes

### Patroni PostgreSQL Cluster (3 nodes)
- **patroni1**: Primary candidate
- **patroni2**: Replica/standby
- **patroni3**: Replica/standby
- Automatic failover and switchover capabilities

### HAProxy Load Balancer
- Routes connections to the primary PostgreSQL node for writes
- Health checks via Patroni REST API
- Stats interface available at http://localhost:8404

### Microservice
- .NET 8 API connecting through HAProxy
- Configured to use HAProxy as database host

## Getting Started

### 1. Start the entire stack:
```bash
docker-compose up -d
```

### 2. Check the status:
```bash
# Check all containers
docker-compose ps

# Check Patroni cluster status
docker exec patroni1 patronictl -c /etc/patroni/patroni.yml list

# Check HAProxy stats
# Open http://localhost:8404 in your browser
```

### 3. Initialize the database:
```bash
# Connect to the primary database through HAProxy
docker exec -it haproxy psql -h patroni1 -U postgres -d postgres -f /init-db.sql
```

### 4. Test your microservice:
```bash
# Check if microservice is running
curl http://localhost:8080

# Get products
curl http://localhost:8080/products

# Add a product
curl -X POST http://localhost:8080/products \
  -H "Content-Type: application/json" \
  -d '{"name":"Test Product","price":19.99,"description":"A test product"}'
```

## Ports

- **5433**: PostgreSQL (through HAProxy)
- **8080**: Microservice API
- **8404**: HAProxy stats interface

## HAProxy ports (read/write separation)

- Host port 5433 -> HAProxy `pg_rw` frontend: routes to the current primary (use for write operations and services that require read-write access).
- Host port 5434 -> HAProxy `pg_ro` frontend: routes to replicas (use for read-only/reporting services to reduce load on the primary).

Recommendations:
- Services that perform writes (e.g., the main microservice, seeding service) should use port 5433.

If you expose these ports via Docker Compose, make sure `docker-compose.yml` maps both 5433 and 5434 for HAProxy (this repo already configures both).

## Connection Details

- **Database Host**: haproxy (from within Docker network) or localhost (from host)
- **Database Port**: 5433
- **Database Name**: mydb
- **Username**: postgres
- **Password**: postgrespass

## High Availability Features

### Automatic Failover
If the primary PostgreSQL node fails, Patroni will automatically promote a replica to become the new primary.

### Health Checks
HAProxy continuously monitors the health of PostgreSQL nodes through Patroni's REST API.

### Load Balancing
- Write operations go to the primary node
- Read operations can be distributed across replicas (if configured)

## Monitoring

### Patroni Status
```bash
docker exec patroni1 patronictl -c /etc/patroni/patroni.yml list
```

### HAProxy Stats
Visit http://localhost:8404 to see:
- Backend server status
- Connection statistics
- Health check results

### Database Connections
```bash
# Connect directly to primary
docker exec -it patroni1 psql -U postgres -d mydb

# Connect through HAProxy
docker exec -it haproxy psql -h patroni1 -U postgres -d mydb
```

## Scaling and Maintenance

### Adding a Replica
1. Add a new Patroni service to docker-compose.yml
2. Update HAProxy configuration to include the new node
3. Restart the services

### Manual Switchover
```bash
# Switch primary to patroni2
docker exec patroni1 patronictl -c /etc/patroni/patroni.yml switchover --candidate patroni2
```

### Backup
```bash
# Create a backup from the primary
docker exec patroni1 pg_dump -U postgres mydb > backup.sql
```

## Troubleshooting

### Check Logs
```bash
# Patroni logs
docker logs patroni1
docker logs patroni2
docker logs patroni3

# HAProxy logs
docker logs haproxy

# Microservice logs
docker logs microservice

# Etcd logs
docker logs etcd1
```

### Common Issues

1. **Cluster not forming**: Check etcd connectivity and logs
2. **No primary elected**: Verify etcd cluster is healthy
3. **Connection refused**: Check HAProxy health checks and Patroni status
4. **Microservice can't connect**: Verify network connectivity and connection string

### Reset Cluster
```bash
# Stop all services
docker-compose down -v

# Remove volumes (WARNING: This will delete all data)
docker volume rm $(docker volume ls -q | grep patroni)

# Start fresh
docker-compose up -d
```