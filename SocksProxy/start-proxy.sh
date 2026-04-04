#!/bin/sh

sleep 2

echo "Starting SOCKS proxy to $JUMP_BOX_USER@$JUMP_BOX_HOST"

if [ ! -f /ssh-keys/id_ed25519 ]; then
    echo "ERROR: /ssh-keys/id_ed25519 not found"
    exit 1
fi

cp /ssh-keys/id_ed25519 /root/.ssh/id_ed25519
chmod 600 /root/.ssh/id_ed25519

if ! ssh -i /root/.ssh/id_ed25519 \
         -o ConnectTimeout=5 \
         -o BatchMode=yes \
         -o PasswordAuthentication=no \
         -T "$JUMP_BOX_USER@$JUMP_BOX_HOST" "exit"; then
    echo "ERROR: Cannot connect to jump box"
    exit 1
fi

exec ssh -i /root/.ssh/id_ed25519 \
    -D 0.0.0.0:1080 \
    -N \
    -o ServerAliveInterval=30 \
    "$JUMP_BOX_USER@$JUMP_BOX_HOST"