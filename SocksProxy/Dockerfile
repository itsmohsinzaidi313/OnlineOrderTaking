FROM alpine:latest

RUN apk add --no-cache openssh-client

# Create ssh directory
RUN mkdir -p /root/.ssh && chmod 700 /root/.ssh

# Copy key during build (requires key file in build context)
COPY /ssh-keys/id_ed25519 /root/.ssh/id_ed25519
RUN chmod 600 /root/.ssh/id_ed25519

# Create SSH config to disable strict checking
RUN echo "StrictHostKeyChecking no" > /root/.ssh/config && \
    echo "UserKnownHostsFile /dev/null" >> /root/.ssh/config && \
    echo "IdentitiesOnly yes" >> /root/.ssh/config

# Script to start proxy
COPY start-proxy.sh /usr/local/bin/
RUN chmod +x /usr/local/bin/start-proxy.sh

ENTRYPOINT ["/usr/local/bin/start-proxy.sh"]