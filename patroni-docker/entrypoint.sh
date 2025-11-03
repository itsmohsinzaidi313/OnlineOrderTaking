#!/bin/bash
set -e

# Wait for etcd to be available
echo "Waiting for etcd to be available..."
until curl -s http://etcd:2379/health > /dev/null 2>&1; do
    echo "Waiting for etcd..."
    sleep 2
done
echo "etcd is available"

# Create Patroni configuration
mkdir -p /etc/patroni

# Create postgres user and set ownership
chown -R postgres:postgres /var/lib/postgresql
chown -R postgres:postgres /etc/patroni

# Generate config based on environment variables
cat > /etc/patroni/patroni.yml << EOF
scope: ${PATRONI_SCOPE:-postgres-cluster}
namespace: ${PATRONI_NAMESPACE:-/service/}
name: ${PATRONI_NAME}

restapi:
  listen: ${PATRONI_RESTAPI_LISTEN:-0.0.0.0:8008}
  connect_address: ${PATRONI_RESTAPI_CONNECT_ADDRESS}

etcd3:
  hosts: ${PATRONI_ETCD3_HOSTS:-etcd:2379}

bootstrap:
  dcs:
    ttl: 30
    loop_wait: 10
    retry_timeout: 60
    maximum_lag_on_failover: 1048576
    postgresql:
      use_pg_rewind: true
      use_slots: true
      parameters:
        max_wal_senders: 3
        max_replication_slots: 3
        wal_level: replica
        hot_standby: on
        wal_keep_size: 128MB
        wal_log_hints: on
  initdb:
  - encoding: UTF8
  - locale: en_US.UTF-8
  pg_hba:
  # Allow replication connections from the Docker network used by the compose
  # setup. Using 172.18.0.0/16 here is acceptable for local dev; in production
  # use a tighter CIDR or explicit IPs.
  - host replication replicator 127.0.0.1/32 md5
  - host replication replicator 0.0.0.0/0 md5
  - host all all 0.0.0.0/0 md5
  - host all all 12.0.0.0/8 md5

postgresql:
  listen: ${PATRONI_POSTGRESQL_LISTEN:-0.0.0.0:5433}
  connect_address: ${PATRONI_POSTGRESQL_CONNECT_ADDRESS}
  data_dir: ${PATRONI_POSTGRESQL_DATA_DIR:-/var/lib/postgresql/data/pgdata}
  pgpass: /tmp/pgpass
  authentication:
    replication:
      username: ${PATRONI_POSTGRESQL_AUTHENTICATION_REPLICATION_USERNAME:-replicator}
      password: ${PATRONI_POSTGRESQL_AUTHENTICATION_REPLICATION_PASSWORD:-replicatorpass}
    superuser:
      username: ${PATRONI_POSTGRESQL_AUTHENTICATION_SUPERUSER_USERNAME:-postgres}
      password: ${PATRONI_POSTGRESQL_AUTHENTICATION_SUPERUSER_PASSWORD:-postgrespass}

tags:
    nofailover: false
    noloadbalance: false
    clonefrom: false
    nosync: false
EOF

# Set proper ownership
chown postgres:postgres /etc/patroni/patroni.yml

echo "Starting Patroni as postgres user..."
# Run Patroni as postgres user
exec gosu postgres patroni /etc/patroni/patroni.yml