FROM postgres:15

# Install system dependencies
RUN apt-get update && apt-get install -y \
    python3-pip \
    python3-dev \
    python3-venv \
    libpq-dev \
    curl \
    gosu \
    && rm -rf /var/lib/apt/lists/*

# Create virtual environment for Patroni
RUN python3 -m venv /opt/patroni-venv

# Install Patroni in virtual environment
RUN /opt/patroni-venv/bin/pip install --upgrade pip
RUN /opt/patroni-venv/bin/pip install patroni[etcd3] psycopg2-binary

# Create symlinks to make patroni accessible globally
RUN ln -s /opt/patroni-venv/bin/patroni /usr/local/bin/patroni
RUN ln -s /opt/patroni-venv/bin/patronictl /usr/local/bin/patronictl

# Create entrypoint script
COPY entrypoint.sh /entrypoint.sh
RUN chmod +x /entrypoint.sh
EXPOSE 8008 5432
ENTRYPOINT ["/entrypoint.sh"]